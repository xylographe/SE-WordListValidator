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
using System.IO;
using System.Windows.Forms;

namespace SubtitleEditWordListValidator
{
    public sealed partial class Main : Form
    {
        private delegate bool StringEquals(string a, string b);

        private readonly StringEquals EqualPathNames;
        private readonly WordListFactory _wlf;
        private readonly Logger _log;
        private string _dictdir;

        public Main()
        {
            InitializeComponent();
            Icon = Properties.Resources.FormIcon;

            _log = new Logger(textBoxTerminal);
            _wlf = new WordListFactory(_log);

            var osPlatform = (int)Environment.OSVersion.Platform;
            var isLikeWindows = !(osPlatform == 4 || osPlatform == 6 || osPlatform == 128);
            EqualPathNames = isLikeWindows ? new StringEquals(StringEqualsOrdinalIgnoreCase) : new StringEquals(StringEqualsOrdinal);
        }

        private bool StringEqualsOrdinalIgnoreCase(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        private bool StringEqualsOrdinal(string a, string b)
        {
            return a.Equals(b, StringComparison.Ordinal);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            InitializeWordListView(GetDictionaryPath());
        }

        private void ToolStripMenuItemDictionaryFolder_Click(object sender, EventArgs e)
        {
            InitializeWordListView(UserDictionaryPath());
        }

        private void InitializeWordListView(string dictdir)
        {
            if (dictdir != null && !EqualPathNames(dictdir, _dictdir))
            {
                _wlf.Close(this);

                toolStripMenuItemDictionary.ToolTipText = string.Empty;
                treeViewWordLists.Nodes[0].Nodes.Clear();
                foreach (var path in Directory.EnumerateFiles(dictdir, "*_OCRFixReplaceList.xml"))
                {
                    var wl = _wlf.CreateOcrFixReplaceList(path);
                    treeViewWordLists.Nodes[0].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[1].Nodes.Clear();
                foreach (var path in Directory.EnumerateFiles(dictdir, "*_NoBreakAfterList.xml"))
                {
                    var wl = _wlf.CreateNoBreakAfterList(path);
                    treeViewWordLists.Nodes[1].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[2].Nodes.Clear();
                foreach (var path in Directory.EnumerateFiles(dictdir, "*names_etc.xml"))
                {
                    var wl = _wlf.CreateNamesEtcList(path);
                    treeViewWordLists.Nodes[2].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[3].Nodes.Clear();
                foreach (var path in Directory.EnumerateFiles(dictdir, "??_??_user.xml"))
                {
                    var wl = _wlf.CreateUserList(path);
                    treeViewWordLists.Nodes[3].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                toolStripMenuItemDictionary.ToolTipText = dictdir;

                _dictdir = dictdir;
            }
        }

        private string GetDictionaryPath()
        {
            try
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var path = Path.Combine(appdata, "Subtitle Edit", "Dictionaries");
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            catch
            {
            }

            try
            {
                var mypath = System.Reflection.Assembly.GetEntryAssembly().Location;
                if (!string.IsNullOrEmpty(mypath))
                {
                    var path = Path.Combine(Path.GetDirectoryName(mypath), "Dictionaries");
                    if (Directory.Exists(path))
                    {
                        return path;
                    }
                }
            }
            catch
            {
            }

            return UserDictionaryPath();
        }

        private string UserDictionaryPath()
        {
            using (var fbd = new FolderBrowserDialog { Description = "Select dictionary folder", ShowNewFolderButton = false })
            {
                try
                {
                    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                }
                catch
                {
                }
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    var path = fbd.SelectedPath;
                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    {
                        return Path.GetFullPath(path);
                    }
                }
            }
            return null;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            _wlf.Close(this);
        }

        private void TreeViewWordLists_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ToolStripMenuItemWordListsValidate_Click(null, null);
        }

        private void TextBoxTerminal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToolStripMenuItemWordListsEdit_Click(null, null);
        }

        private void ToolStripMenuItemWordListsValidate_Click(object sender, EventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    wl.Validate(this);
                }
            }
        }

        private void ToolStripMenuItemWordListsEdit_Click(object sender, EventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    wl.Edit(this);
                }
            }
        }

        private void ToolStripMenuItemWordListsSubmit_Click(object sender, EventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    wl.Submit(this);
                }
            }
        }

        private void ToolStripMenuItemWordListsAcceopt_Click(object sender, EventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    wl.Accept(this);
                }
            }
        }

        private void ToolStripMenuItemWordListsReject_Click(object sender, EventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    wl.Reject(this);
                }
            }
        }

    }
}
