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
using System.IO;
using System.Windows.Forms;

namespace SubtitleEditWordListValidator
{
    public sealed partial class Main : Form
    {
        private delegate bool StringEquals(string a, string b);

        private readonly StringEquals FileNameEquals;
        private readonly WordListFactory _wlf;
        private readonly Logger _log;
        private Settings _settings;
        private string _wordListFolder;

        public Main()
        {
            InitializeComponent();
            Icon = Properties.Resources.FormIcon;

            _log = new TextBoxLogger(textBoxTerminal);
            _wlf = new WordListFactory(_log);

            FileNameEquals = Configuration.IsRunningOnWindows ? new StringEquals(StringEqualsOrdinalIgnoreCase) : new StringEquals(StringEqualsOrdinal);
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
            _settings = Configuration.Settings;
            var recentFolders = _settings.RecentFolders;
            var maxCount = toolStripMenuItemFileRecentFolders.DropDownItems.Count;
            while (recentFolders.Count > maxCount)
                recentFolders.RemoveAt(0);
            recentFolders.Capacity = maxCount;

            toolStripMenuItemFile.ToolTipText = "Subtitle Edit “Dictionaries” folder not found";
            InitializeWordListView(GetDictionaryPath());
        }

        private void MenuStripWordLists_MenuActivate(object sender, EventArgs e)
        {
            var recentFolders = _settings.RecentFolders;
            int index = recentFolders.Count - 1;
            if ((toolStripMenuItemFileRecentFolders.Visible = index > 0))
            {
                foreach (ToolStripMenuItem item in toolStripMenuItemFileRecentFolders.DropDownItems)
                {
                    if ((item.Visible = --index >= 0))
                    {
                        item.Text = recentFolders[index];
                        item.Enabled = true;
                    }
                }
            }
        }

        private void ToolStripMenuItemFileRecentFolders_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var path = item.Text;

            if (Directory.Exists(path))
            {
                InitializeWordListView(path);
            }
            else
            {
                item.Enabled = false;
                var recentFolders = _settings.RecentFolders;
                recentFolders.RemoveAt(recentFolders.FindIndex(fn => FileNameEquals(path, fn)));
            }
        }

        private void ToolStripMenuItemFileChangeFolder_Click(object sender, EventArgs e)
        {
            InitializeWordListView(UserDictionaryPath());
        }

        private void InitializeWordListView(string wordListFolder)
        {
            if (wordListFolder != null && !FileNameEquals(wordListFolder, _wordListFolder) && _wlf.Close(this))
            {
                toolStripMenuItemFile.ToolTipText = string.Empty;
                treeViewWordLists.Nodes[0].Nodes.Clear();
                panelFind.Visible = false;
                foreach (var path in _wlf.EnumerateOcrFixReplaceFiles(wordListFolder))
                {
                    var wl = _wlf.CreateOcrFixReplaceList(path);
                    treeViewWordLists.Nodes[0].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[1].Nodes.Clear();
                foreach (var path in _wlf.EnumerateNoBreakAfterFiles(wordListFolder))
                {
                    var wl = _wlf.CreateNoBreakAfterList(path);
                    treeViewWordLists.Nodes[1].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[2].Nodes.Clear();
                foreach (var path in _wlf.EnumerateNamesEtcFiles(wordListFolder))
                {
                    var wl = _wlf.CreateNamesEtcList(path);
                    treeViewWordLists.Nodes[2].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                treeViewWordLists.Nodes[3].Nodes.Clear();
                foreach (var path in _wlf.EnumerateUserFiles(wordListFolder))
                {
                    var wl = _wlf.CreateUserList(path);
                    treeViewWordLists.Nodes[3].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
                }
                toolStripMenuItemFile.ToolTipText = wordListFolder;

                UpdateRecentFolders(wordListFolder);
                _wordListFolder = wordListFolder;
            }
        }

        private void UpdateRecentFolders(string path)
        {
            var recentFolders = _settings.RecentFolders;
            var index = recentFolders.FindIndex(fn => FileNameEquals(path, fn));
            if (index >= 0)
                recentFolders.RemoveAt(index);
            else if (recentFolders.Count == recentFolders.Capacity)
                recentFolders.RemoveAt(0);
            recentFolders.Add(path);
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

            var recentFolders = _settings.RecentFolders;
            var index = recentFolders.Count - 1;
            if (index >= 0)
            {
                var path = recentFolders[index];
                if (Directory.Exists(path))
                {
                    return UserDictionaryPath(path);
                }
            }
            return UserDictionaryPath();
        }

        private string UserDictionaryPath(string startPath = null)
        {
            using (var fbd = new FolderBrowserDialog { Description = "Select Subtitle Edit “Dictionaries” folder", ShowNewFolderButton = false })
            {
                try
                {
                    fbd.SelectedPath = startPath ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
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
            _settings.Save();
            if (e.CloseReason != CloseReason.ApplicationExitCall && e.CloseReason != CloseReason.TaskManagerClosing && e.CloseReason != CloseReason.WindowsShutDown && !_wlf.Close(this))
            {
                e.Cancel = true;
            }
        }

        private void TextBoxTerminal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToolStripMenuItemWordListsEdit_Click(null, null);
        }

        private void TreeViewWordLists_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ToolStripMenuItemWordListsValidate_Click(null, null);
        }

        private void TreeViewWordLists_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeViewWordLists.SelectedNode;
            if (node != null)
            {
                var wl = node.Tag as WordList;
                if (wl != null)
                {
                    panelFind.Visible = wl.CanFind;
                    return;
                }
            }
            panelFind.Visible = false;
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
                    treeViewWordLists.Select();
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
                    treeViewWordLists.Select();
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
                    treeViewWordLists.Select();
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
                    treeViewWordLists.Select();
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
                    treeViewWordLists.Select();
                }
            }
        }

        private void TextBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;

            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        tb.SelectAll();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Back:
                        if (tb.SelectionLength == 0)
                        {
                            var breakChars = " \t\r\n\"'.?!,;:{}()[]<>-/#*|¿¡♪♫";
                            var index = tb.SelectionStart;
                            var deleteFrom = index - 1;
                            var s = tb.Text;
                            if (deleteFrom > 0 && deleteFrom < s.Length)
                            {
                                if (s[deleteFrom] == ' ' || s[deleteFrom] == '\t')
                                    deleteFrom--;
                                while (deleteFrom > 0 && breakChars.IndexOf(s[deleteFrom]) < 0)
                                    deleteFrom--;
                                if (deleteFrom == index - 1)
                                {
                                    breakChars = breakChars.Substring(2);
                                    while (deleteFrom > 0 && breakChars.IndexOf(s[deleteFrom - 1]) >= 0)
                                        deleteFrom--;
                                }
                                if (s[deleteFrom] == ' ' || s[deleteFrom] == '\t')
                                    deleteFrom++;
                                tb.Select(deleteFrom, index - deleteFrom);
                                tb.Paste(string.Empty);
                            }
                        }
                        e.SuppressKeyPress = true;
                        break;
                }
            }
        }

        private void ButtonFindGo_Click(object sender, EventArgs e)
        {
            if (textBoxFind.Text.Length > 0)
            {
                var node = treeViewWordLists.SelectedNode;
                if (node != null)
                {
                    var wl = node.Tag as WordList;
                    if (wl != null)
                    {
                        wl.Find(this, textBoxFind.Text);
                        treeViewWordLists.Select();
                    }
                }
            }
            else
            {
                textBoxFind.Select();
            }
        }

        private void ButtonFindClear_Click(object sender, EventArgs e)
        {
            if (textBoxFind.Text.Length > 0)
            {
                textBoxFind.SelectAll();
                textBoxFind.Paste(string.Empty);
            }
            textBoxFind.Select();
        }

    }
}
