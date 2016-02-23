/*
    Copyright © 2015-2016 Waldi Ravens

    This file is part of SE-WordListValidator.

    SE-WordListValidator is free software: you can redistribute it
    and/or modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation, either version 3 of
    the License, or (at your option) any later version.

    SE-WordListValidator is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with SE-WordListValidator.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    partial class WordListFactory
    {
        private abstract class WordListNoAttributes : WordListBase
        {
            private readonly string NameSuffix;
            private readonly string RootName;
            private readonly string ItemName;

            public WordListNoAttributes(WordListFactory wlf, string path, string nameSuffix, string rootName, string itemName)
                : base(wlf, path)
            {
                NameSuffix = nameSuffix;
                RootName = rootName;
                ItemName = itemName;
            }

            protected sealed override void ValidateRoot(XmlDocument document, XmlReader reader)
            {
                var nmap = new Dictionary<string, List<XmlNode>>(15000, StringComparer.Ordinal);
                var list = new List<string>(10000);
                var root = document.CreateElement(RootName);
                var rootcomments = new List<XmlNode>();
                var comments = new List<XmlNode>();
                var element = (string)null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Whitespace)
                    {
                        if (element != null && reader.Value.ContainsLinefeed())
                        {
                            if (comments.Count > 0)
                            {
                                nmap[element].AddRange(comments);
                                comments.Clear();
                            }
                            element = null;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Comment)
                    {
                        comments.Add(document.CreateComment(reader.Value.ReplaceWhiteSpace(" ")));
                    }
                    else if (reader.NodeType == XmlNodeType.CDATA)
                    {
                        comments.Add(document.CreateCDataSection(reader.Value.ReplaceWhiteSpace()));
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Text)
                    {
                        if(!string.IsNullOrWhiteSpace(reader.Value))
                        {
                            element = reader.Value;
                            if (nmap.ContainsKey(element))
                            {
                                nmap[element].AddRange(comments);
                                comments.Clear();
                                _factory.Verbose(reader, string.Format("Removed duplicate »{0}«", reader.Value));
                            }
                            else
                            {
                                nmap.Add(element, comments);
                                list.Add(element);
                                comments.Clear();
                            }
                        }
                    }
                    else if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    {
                        if (element != null)
                        {
                            if (comments.Count > 0)
                            {
                                nmap[element].AddRange(comments);
                                comments.Clear();
                            }
                            element = null;
                        }
                        if (reader.HasAttributes || !reader.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(string.Format("Invalid element <{0}…> in <{1}>", reader.Name, root.Name));
                        }
                    }
                    else if (reader.Depth == 1 && reader.NodeType == XmlNodeType.EndElement)
                    {
                    }
                    else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element)
                    {
                        if (comments.Count > 0)
                        {
                            foreach (var c in comments)
                            {
                                document.AppendChild(c);
                            }
                            comments.Clear();
                        }
                        document.AppendChild(root);
                    }
                    else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (comments.Count > 0)
                        {
                            if (element != null)
                            {
                                nmap[element].AddRange(comments);
                                element = null;
                            }
                            else
                            {
                                rootcomments = comments;
                            }
                            comments.Clear();
                        }
                    }
                    else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        _xmlWriterSettings.OmitXmlDeclaration = false;
                    }
                    else
                    {
                        throw new Exception(string.Format("Unexpected {0} node", reader.NodeType));
                    }
                }
                if (comments.Count > 0)
                {
                    foreach (var c in comments)
                    {
                        document.AppendChild(c);
                    }
                    comments.Clear();
                }
                {
                    var s = Name.EndsWith(NameSuffix, StringComparison.Ordinal) ? Name.Remove(Name.Length - NameSuffix.Length).Replace('_', '-') : string.Empty;
                    list.Sort(new WordComparer(CultureInfo.CreateSpecificCulture(s)));
                }
                foreach (var w in list)
                {
                    foreach (var c in nmap[w])
                    {
                        root.AppendChild(c);
                    }
                    var e = document.CreateElement(ItemName);
                    root.AppendChild(e).AppendChild(document.CreateTextNode(w));
                }
                if (rootcomments.Count > 0)
                {
                    foreach (var c in rootcomments)
                    {
                        root.AppendChild(c);
                    }
                    rootcomments.Clear();
                }
            }

        }
    }
}
