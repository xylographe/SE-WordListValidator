/*
    Copyright © 2015 Waldi Ravens

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
using System.Text.RegularExpressions;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    partial class WordListFactory
    {
        private sealed class NoBreakAfterList : WordListBase
        {
            private Regex _regex;

            public NoBreakAfterList(WordListFactory wlf, string path)
                : base(wlf, path)
            {
            }

            protected override void ValidateRoot(XmlDocument document, XmlReader reader)
            {
                var nmap = new Dictionary<string, List<XmlNode>>(15000, StringComparer.Ordinal);
                var list = new List<string>(10000);
                var root = document.CreateElement("NoBreakAfterList");
                var rootcomments = new List<XmlNode>();
                var comments = new List<XmlNode>();
                var reFalse = "\u0000";
                var reTrue = "\u0001";
                var isRegex = (string)null;
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
                            element = reader.Value + isRegex;
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
                                if (isRegex == reTrue)
                                {
                                    try
                                    {
                                        _regex = new Regex(reader.Value, RegexOptions.CultureInvariant);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(string.Format("Invalid regex \"{0}\": {1}", reader.Value, ex.Message));
                                    }
                                }
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
                        if (!reader.IsEmptyElement && reader.AttributeCount == 1 && reader.Name == "Item" && (isRegex = reader["RegEx"]) != null)
                        {
                            try
                            {
                                isRegex = Convert.ToBoolean(isRegex) ? reTrue : reFalse;
                            }
                            catch
                            {
                                throw new Exception(string.Format("Invalid attribute value <{0} Regex=\"{1}\"> in <{2}>", reader.Name, isRegex, root.Name));
                            }
                        }
                        else if (!reader.IsEmptyElement && !reader.HasAttributes && reader.Name == "Item")
                        {
                            isRegex = reFalse;
                        }
                        else
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
                    var s = "_NoBreakAfterList";
                    s = Name.EndsWith(s, StringComparison.Ordinal) ? Name.Remove(Name.Length - s.Length).Replace('_', '-') : string.Empty;
                    list.Sort(new WordComparer(CultureInfo.GetCultureInfo(s)));
                }
                foreach (var w in list)
                {
                    foreach (var c in nmap[w])
                    {
                        root.AppendChild(c);
                    }
                    var e = document.CreateElement("Item");
                    root.AppendChild(e).AppendChild(document.CreateTextNode(w.Remove(w.Length - 1)));
                    if (w.EndsWith(reTrue, StringComparison.Ordinal))
                    {
                        e.SetAttribute("RegEx", "True");
                    }
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
