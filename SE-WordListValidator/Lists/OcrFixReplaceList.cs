/*
    Copyright © 2015-2019 Waldi Ravens

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
using System.Text;
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
                new SubListInfo { Name = "WholeLines",         ItemName = "Line",      MethodName = "FromTo" },
                new SubListInfo { Name = "PartialLinesAlways", ItemName = "LinePart",  MethodName = "FromTo" },
                new SubListInfo { Name = "PartialLines",       ItemName = "LinePart",  MethodName = "FromTo" },
                new SubListInfo { Name = "BeginLines",         ItemName = "Beginning", MethodName = "FromTo" },
                new SubListInfo { Name = "EndLines",           ItemName = "Ending",    MethodName = "FromTo" },
                new SubListInfo { Name = "RegularExpressions", ItemName = "RegEx",     MethodName = null     }
            };
            private Regex _regex;

            private delegate void SubListValidator(SubList list, XmlDocument document, XmlReader reader);

            private struct SubListInfo
            {
                public string Name;
                public string ItemName;
                public string MethodName;
            }

            private struct SubList
            {
                public string ItemName;
                public XmlElement NodeSelf;
                public SubListValidator Validate;
                public Dictionary<string, XmlElement> Elements;
            }

            public OcrFixReplaceList(WordListFactory wlf, string path)
                : base(wlf, path, canFind: true)
            {
            }

            protected override void FindItems(XmlDocument document, string input)
            {
                foreach (XmlElement regex in document.DocumentElement.SelectNodes("RegularExpressions/RegEx"))
                {
                    var find = regex.Attributes["find"].Value;
                    var repl = regex.Attributes["replaceWith"].Value;
                    if (!string.IsNullOrEmpty(find) && repl != null)
                    {
                        var result = Regex.Replace(input, find, repl, (RegexOptions.CultureInvariant | RegexOptions.Multiline));
                        if (result != input)
                        {
                            _factory.Logger.Info(string.Format("{0}\n\t{1}\n ==>\t{2}", regex.OuterXml, input.Replace(Environment.NewLine, "<br />"), result.Replace(Environment.NewLine, "<br />")));
                        }
                    }
                }
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
                    lists.Add(sld.Name.ToUpperInvariant(), new SubList { NodeSelf = node, ItemName = sld.ItemName, Validate = method, Elements = new Dictionary<string, XmlElement>() });
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
                        if (!reader.HasAttributes && lists.ContainsKey(reader.Name.ToUpperInvariant()))
                        {
                            var list = lists[reader.Name.ToUpperInvariant()];
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
                        _xmlWriterSettings.OmitXmlDeclaration = false;
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
                        if (reader.IsEmptyElement && reader.AttributeCount == 2 && reader.Name.Equals(list.ItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            var find = (string)null;
                            var repl = (string)null;
                            {
                                reader.MoveToAttribute(0);
                                switch (reader.Name.ToUpperInvariant())
                                {
                                    case "FIND":
                                        find = reader.Value;
                                        break;
                                    case "REPLACEWITH":
                                        repl = reader.Value;
                                        break;
                                }
                                reader.MoveToAttribute(1);
                                switch (reader.Name.ToUpperInvariant())
                                {
                                    case "FIND":
                                        find = reader.Value;
                                        break;
                                    case "REPLACEWITH":
                                        repl = reader.Value;
                                        break;
                                }
                                reader.MoveToElement();
                            }
                            if (!string.IsNullOrEmpty(find) && repl != null)
                            {
                                var key = find + "\u0000" + repl;
                                if (list.Elements.ContainsKey(key))
                                {
                                    item = list.Elements[key];
                                    _factory.Verbose(reader, string.Format("Removed duplicate »{0}« ==> »{1}«", find, repl));
                                }
                                else
                                {
                                    list.NodeSelf.AppendChild(item = document.CreateElement(list.ItemName));
                                    list.Elements.Add(key, item);
                                    try
                                    {
                                        _regex = new Regex(find, RegexOptions.CultureInvariant);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(string.Format("Invalid regex \"{0}\": {1}", find, ex.Message));
                                    }
                                    repl = ValidateReplacementPattern(reader, _regex, repl);
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
                        if (reader.IsEmptyElement && reader.AttributeCount == 2 && reader.Name.Equals(list.ItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            var from = (string)null;
                            var to = (string)null;
                            {
                                reader.MoveToAttribute(0);
                                switch (reader.Name.ToUpperInvariant())
                                {
                                    case "FROM":
                                        from = reader.Value;
                                        break;
                                    case "TO":
                                        to = reader.Value;
                                        break;
                                }
                                reader.MoveToAttribute(1);
                                switch (reader.Name.ToUpperInvariant())
                                {
                                    case "FROM":
                                        from = reader.Value;
                                        break;
                                    case "TO":
                                        to = reader.Value;
                                        break;
                                }
                                reader.MoveToElement();
                            }
                            if (!string.IsNullOrEmpty(from) && to != null)
                            {
                                var key = from + "\u0000" + to;
                                if (list.Elements.ContainsKey(key))
                                {
                                    item = list.Elements[key];
                                    _factory.Verbose(reader, string.Format("Removed duplicate »{0}« ==> »{1}«", from, to));
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

            private string ValidateReplacementPattern(XmlReader reader, Regex find, string repl)
            {
                var sb = new StringBuilder(repl).Append('\0');
                int index = -1;
                int mode = 0;
                while (++index < sb.Length)
                {
                    char c = sb[index];
                    switch (mode)
                    {
                        case 0:
                            if (c == '$')
                            {
                                mode = 2;
                            }
                            break;
                        case 1: // $$
                            if ("0123456789{".IndexOf(c) >= 0)
                            {
                                _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Perhaps you meant \"{1}${2}\"?", repl, sb.ToString(0, index), c));
                            }
                            index -= 1;
                            mode = 0;
                            break;
                        case 2: // $
                            mode = 0;
                            switch (c)
                            {
                                case '0': case '1': case '2': case '3': case '4':
                                case '5': case '6': case '7': case '8': case '9':
                                    mode = 3;
                                    break;
                                case '{':
                                    mode = 4;
                                    break;
                                case '$':
                                    mode = 1;
                                    break;
                                case '\'':
                                case '`':
                                case '_':
                                    _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Are you sure you want \"${1}\" in the replacement pattern?", repl, c));
                                    break;
                                case '&':
                                case '+':
                                    break;
                                default:
                                    sb.Insert(index, '$');
                                    _factory.Verbose(reader, string.Format("replaceWith=\"{0}\" - Missing '$' inserted: \"{1}\"", repl, sb.ToString(0, index + 1)));
                                    break;
                            }
                            break;
                        case 3: // $[0-9]+
                            if ("0123456789".IndexOf(c) < 0)
                            {
                                var start = sb.ToString(0, index).LastIndexOf('$') + 1;
                                var group = sb.ToString(start, index - start);
                                if (find.GroupNumberFromName(group) < 0)
                                {
                                    _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Group \"{1}\" is not defined in the regular expression pattern!", repl, group));
                                }
                                index -= 1;
                                mode = 0;
                            }
                            break;
                        case 4: // ${
                            mode = 5;
                            if ("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".IndexOf(c) < 0)
                            {
                                _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Invalid group identifier \"{1}\"?", repl, sb.ToString(index - 2, 3)));
                                index -= 1;
                                mode = 0;
                            }
                            break;
                        case 5: // ${[0-9A-Za-z]+
                            if (c == '}')
                            {
                                var start = sb.ToString(0, index).LastIndexOf('{') + 1;
                                var group = sb.ToString(start, index - start);
                                if (find.GroupNumberFromName(group) < 0)
                                {
                                    _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Group \"{1}\" is not defined in the regular expression pattern!", repl, group));
                                }
                                mode = 0;
                            }
                            else if ("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".IndexOf(c) < 0)
                            {
                                var start = sb.ToString(0, index).LastIndexOf('$');
                                _factory.Warn(reader, string.Format("replaceWith=\"{0}\" - Invalid group identifier \"{1}\"?", repl, sb.ToString(start, index - start)));
                                index -= 1;
                                mode = 0;
                            }
                            break;
                    }
                }
                return sb.ToString(0, sb.Length - 1);
            }

        }
    }
}
