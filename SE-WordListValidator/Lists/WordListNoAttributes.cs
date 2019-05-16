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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    partial class WordListFactory
    {
        private abstract class WordListNoAttributes : WordListBase
        {
            protected sealed class SubListSpec
            {
                public string RootName { get; set; }
                public string ItemName { get; set; }
                public SubListSpec[] SubLists { get; set; }
            }

            private class SubList : List<SubList>
            {
                public int Depth { get; }
                public string Name { get; }
                public SubList Parent { get; }
                public ItemList Items { get; }
                public List<XmlNode> Comments { get; }
                public List<XmlNode> EndComments { get; }

                public SubList(SubListSpec spec)
                    : base(1)
                {
                    Name = string.Empty;
                    Comments = new List<XmlNode>(1);
                    EndComments = new List<XmlNode>(1);
                    Add(new SubList(this, spec));
                }

                private SubList(SubList parent, SubListSpec spec)
                    : base(spec.SubLists?.Length ?? 0)
                {
                    Name = spec.RootName;
                    Comments = new List<XmlNode>(1);
                    EndComments = new List<XmlNode>(1);
                    Parent = parent;
                    Depth = parent.Depth + 1;
                    if (spec.ItemName != null)
                    {
                        Items = new ItemList(spec.ItemName);
                    }
                    if (spec.SubLists != null)
                    {
                        foreach (var sub in spec.SubLists)
                        {
                            Add(new SubList(this, sub));
                        }
                    }
                }

                public SubList this[string name]
                {
                    get
                    {
                        return Find(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    }
                }

                public bool Contains(string name)
                {
                    return Exists(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
            }

            private class ItemList : KeyedCollection<string, Item>
            {
                public string Name { get; }

                public ItemList(string itemName)
                    : base(StringComparer.Ordinal)
                {
                    Name = itemName;
                }

                public bool Add(string text, out Item item)
                {
                    item = null;
                    Dictionary?.TryGetValue(text, out item);
                    if (item == null)
                    {
                        Add((item = new Item(text)));
                        return true;
                    }
                    return false;
                }

                protected override string GetKeyForItem(Item item)
                {
                    return item.Text;
                }
            }

            private class Item
            {
                public string Text { get; }
                public List<XmlNode> Comments { get; }

                public Item(string text)
                {
                    Text = text;
                    Comments = new List<XmlNode>(1);
                }
            }

            private readonly SubListSpec _RootSpec;

            protected WordListNoAttributes(WordListFactory wlf, string path, SubListSpec spec)
                : base(wlf, path)
            {
                _RootSpec = spec;
            }

            protected sealed override void ValidateRoot(XmlDocument document, XmlReader reader)
            {
                var targetComments = (List<XmlNode>)null;
                var comments = new List<XmlNode>(11);
                var list = new SubList(_RootSpec);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Whitespace)
                    {
                        if (targetComments != null && reader.Value.ContainsLinefeed())
                        {
                            if (comments.Count > 0)
                            {
                                targetComments.AddRange(comments);
                                comments.Clear();
                            }
                            targetComments = null;
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
                    else if (reader.Depth == list.Depth && reader.NodeType == XmlNodeType.Element)
                    {
                        if (!reader.HasAttributes && reader.Name.Equals(list.Items?.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            targetComments = null;
                        }
                        else if (!reader.HasAttributes && list.Contains(reader.Name))
                        {
                            list = list[reader.Name];
                            targetComments = list.Comments;
                            if (comments.Count > 0)
                            {
                                targetComments.AddRange(comments);
                                comments.Clear();
                            }
                            if (reader.IsEmptyElement)
                            {
                                targetComments = list.EndComments;
                                list = list.Parent;
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("Invalid element <{0}…> in <{1}>", reader.Name, list.Name));
                        }
                    }
                    else if (reader.Depth == list.Depth + 1 && reader.NodeType == XmlNodeType.Text)
                    {
                        if(!string.IsNullOrWhiteSpace(reader.Value))
                        {
                            if (!list.Items.Add(reader.Value, out var item))
                            {
                                _factory.Verbose(reader, string.Format("Removed duplicate »{0}«", reader.Value));
                            }
                            if (comments.Count > 0)
                            {
                                item.Comments.AddRange(comments);
                                comments.Clear();
                            }
                            targetComments = item.Comments;
                        }
                    }
                    else if (reader.Depth == list.Depth && reader.NodeType == XmlNodeType.EndElement)
                    {
                    }
                    else if (reader.Depth == list.Depth - 1 && reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (comments.Count > 0)
                        {
                            list.EndComments.AddRange(comments);
                            comments.Clear();
                        }
                        targetComments = list.EndComments;
                        list = list.Parent;
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
                document.AppendChild(GenerateRoot(document, document, list[0]));
                if (comments.Count > 0)
                {
                    foreach (var c in comments)
                    {
                        document.AppendChild(c);
                    }
                    comments.Clear();
                }
            }

            private XmlNode GenerateRoot(XmlDocument document, XmlNode parent, SubList list)
            {
                var root = document.CreateElement(list.Name);
                parent.AppendChild(root);
                foreach (var c in list.Comments)
                {
                    parent.InsertBefore(c, root);
                }
                foreach (var sl in list)
                {
                    root.AppendChild(GenerateRoot(document, root, sl));
                }
                if (list.Items != null)
                {
                    var itemName = list.Items.Name;
                    foreach (var item in list.Items.OrderBy(i => i.Text, _comparer))
                    {
                        foreach (var c in item.Comments)
                        {
                            root.AppendChild(c);
                        }
                        root.AppendChild(document.CreateElement(itemName)).AppendChild(document.CreateTextNode(item.Text));
                    }
                }
                foreach (var c in list.EndComments)
                {
                    parent.AppendChild(c);
                }
                return root;
            }

        }
    }
}
