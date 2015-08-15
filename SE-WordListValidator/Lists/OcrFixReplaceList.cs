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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    partial class WordListFactory
    {
        private sealed class OcrFixReplaceList : WordListBase
        {
            private static readonly SubListInfo[] SubListDefinitions = new SubListInfo[]
            {
                new SubListInfo { Name = "WholeWords",         ItemName = "Word",      MethodName = "FromTo" },
                new SubListInfo { Name = "PartialWordsAlways", ItemName = "WordPart",  MethodName = "FromTo" },
                new SubListInfo { Name = "PartialWords",       ItemName = "WordPart",  MethodName = "FromTo" },
                new SubListInfo { Name = "PartialLines",       ItemName = "LinePart",  MethodName = "FromTo" },
                new SubListInfo { Name = "PartialLinesAlways", ItemName = "LinePart",  MethodName = "FromTo" },
                new SubListInfo { Name = "BeginLines",         ItemName = "Beginning", MethodName = "FromTo" },
                new SubListInfo { Name = "EndLines",           ItemName = "Ending",    MethodName = "FromTo" },
                new SubListInfo { Name = "WholeLines",         ItemName = "Line",      MethodName = "FromTo" },
                new SubListInfo { Name = "RegularExpressions", ItemName = "RegEx",     MethodName = null     }
            };
            private struct SubListInfo
            {
                public string Name;
                public string ItemName;
                public string MethodName;
            }

            private delegate void SubListValidator(SubList list, XmlDocument document, XmlReader reader);
            private struct SubList
            {
                public string ItemName;
                public XmlElement NodeSelf;
                public SubListValidator Validate;
                public Dictionary<string, XmlElement> Elements;
            }
            private Regex _regex;

            public OcrFixReplaceList(WordListFactory wlf, string path)
                : base(wlf, path)
            {
            }

            protected override void ValidateRoot(XmlDocument document, XmlReader reader)
            {
                var root = document.CreateElement("OCRFixReplaceList");
                var lists = new Dictionary<string, SubList>();
                foreach (var sld in SubListDefinitions)
                {
                    var methodName = "Validate" + (sld.MethodName ?? sld.Name);
                    var method = (SubListValidator)GetType().
                                 GetMethod(methodName, (BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)).
                                 CreateDelegate(typeof(SubListValidator), this);
                    var node = document.CreateElement(sld.Name);
                    root.AppendChild(node);
                    lists.Add(sld.Name, new SubList { NodeSelf = node, ItemName = sld.ItemName, Validate = method, Elements = new Dictionary<string, XmlElement>() });
                }
                var comments = new List<XmlNode>();
                var element  = (XmlElement)null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Whitespace)
                    {
                        if (element != null && reader.Value.ContainsLinefeed())
                        {
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    element.ParentNode.InsertBefore(c, element);
                                }
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
                    else if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    {
                        if (!reader.HasAttributes && lists.ContainsKey(reader.Name))
                        {
                            var list = lists[reader.Name];
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    root.InsertBefore(c, list.NodeSelf);
                                }
                                comments.Clear();
                            }
                            if (reader.IsEmptyElement)
                                element = list.NodeSelf;
                            else
                                list.Validate(list, document, reader);
                        }
                        else
                        {
                            throw new Exception(string.Format("Invalid element <{0}…> in <{1}>", reader.Name, root.Name));
                        }
                    }
                    else if (reader.Depth == 1)
                    {
                        throw new Exception(string.Format("Unexpected {0} node in <{1}>", reader.NodeType, root.Name));
                    }
                    else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                    }
                    else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.EndElement)
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
                        if (reader.IsEmptyElement)
                        {
                            element = root;
                        }
                        document.AppendChild(root);
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
            }

            private void ValidateRegularExpressions(SubList list, XmlDocument document, XmlReader reader)
            {
                var lastitem = list.NodeSelf;
                var comments = new List<XmlNode>();
                while (reader.Read())
                {
                    if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Element)
                    {
                        var item = (XmlElement)null;
                        if (reader.IsEmptyElement && reader.AttributeCount == 2 && reader.Name == list.ItemName)
                        {
                            var find = reader["find"];
                            var repl = reader["replaceWith"];
                            if (!string.IsNullOrEmpty(find) && repl != null)
                            {
                                var key = find + "\u0000" + repl;
                                if (list.Elements.ContainsKey(key))
                                {
                                    item = list.Elements[key];
                                    _factory.Logger.Verbose(string.Format("Removed duplicate |{0}| ==> |{1}|", find, repl));
                                }
                                else
                                {
                                    list.NodeSelf.AppendChild(item = document.CreateElement(list.ItemName));
                                    list.Elements.Add(key, item);
                                    try
                                    {
                                        _regex = new Regex(reader.Value, RegexOptions.CultureInvariant);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(string.Format("Invalid regex \"{0}\": {1}", reader.Value, ex.Message));
                                    }
                                    item.SetAttribute("find", find);
                                    item.SetAttribute("replaceWith", repl);
                                }
                            }
                        }
                        if (item != null)
                        {
                            if (lastitem == null)
                            {
                                lastitem = item;
                            }
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    lastitem.ParentNode.InsertBefore(c, lastitem);
                                }
                                comments.Clear();
                            }
                            lastitem = item;
                        }
                        else
                        {
                            throw new Exception(string.Format("Invalid element <{0}…> in <{1}>", reader.Name, list.NodeSelf.Name));
                        }
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Whitespace)
                    {
                        if (lastitem != null && reader.Value.ContainsLinefeed())
                        {
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    lastitem.ParentNode.InsertBefore(c, lastitem);
                                }
                                comments.Clear();
                            }
                            lastitem = null;
                        }
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Comment)
                    {
                        comments.Add(document.CreateComment(reader.Value.ReplaceWhiteSpace(" ")));
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.CDATA)
                    {
                        comments.Add(document.CreateCDataSection(reader.Value.ReplaceWhiteSpace()));
                    }
                    else if (reader.Depth == 1 && reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (comments.Count > 0)
                        {
                            foreach (var c in comments)
                            {
                                list.NodeSelf.AppendChild(c);
                            }
                            comments.Clear();
                        }
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                throw new Exception(string.Format("Unexpected {0} node in <{1}>", reader.NodeType, list.NodeSelf.Name));
            }

            private void ValidateFromTo(SubList list, XmlDocument document, XmlReader reader)
            {
                var lastitem = list.NodeSelf;
                var comments = new List<XmlNode>();
                while (reader.Read())
                {
                    if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Element)
                    {
                        var item = (XmlElement)null;
                        if (reader.IsEmptyElement && reader.AttributeCount == 2 && reader.Name == list.ItemName)
                        {
                            var from = reader["from"];
                            var to = reader["to"];
                            if (!string.IsNullOrEmpty(from) && to != null)
                            {
                                var key = string.Format("{0}\0{1}", from, to);
                                if (list.Elements.ContainsKey(key))
                                {
                                    item = list.Elements[key];
                                    _factory.Logger.Verbose(string.Format("Removed duplicate |{0}| ==> |{1}|", from, to));
                                }
                                else
                                {
                                    item = document.CreateElement(list.ItemName);
                                    item.SetAttribute("from", from);
                                    item.SetAttribute("to", to);
                                    list.NodeSelf.AppendChild(item);
                                    list.Elements.Add(key, item);
                                }
                            }
                        }
                        if (item != null)
                        {
                            if (lastitem == null)
                            {
                                lastitem = item;
                            }
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    lastitem.ParentNode.InsertBefore(c, lastitem);
                                }
                                comments.Clear();
                            }
                            lastitem = item;
                        }
                        else
                        {
                            throw new Exception(string.Format("Invalid element <{0}…> in <{1}>", reader.Name, list.NodeSelf.Name));
                        }
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Whitespace)
                    {
                        if (lastitem != null && reader.Value.ContainsLinefeed())
                        {
                            if (comments.Count > 0)
                            {
                                foreach (var c in comments)
                                {
                                    lastitem.ParentNode.InsertBefore(c, lastitem);
                                }
                                comments.Clear();
                            }
                            lastitem = null;
                        }
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.Comment)
                    {
                        comments.Add(document.CreateComment(reader.Value.ReplaceWhiteSpace(" ")));
                    }
                    else if (reader.Depth == 2 && reader.NodeType == XmlNodeType.CDATA)
                    {
                        comments.Add(document.CreateCDataSection(reader.Value.ReplaceWhiteSpace()));
                    }
                    else if (reader.Depth == 1 && reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (comments.Count > 0)
                        {
                            foreach (var c in comments)
                            {
                                list.NodeSelf.AppendChild(c);
                            }
                            comments.Clear();
                        }
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                throw new Exception(string.Format("Unexpected {0} node in <{1}>", reader.NodeType, list.NodeSelf.Name));
            }

        }
    }
}
