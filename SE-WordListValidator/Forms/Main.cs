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
        private readonly WordListFactory _wlf;
        private readonly Logger _log;
        private string _dictionaryPath;

        public Main()
        {
            InitializeComponent();
            Icon = Properties.Resources.FormIcon;

            _log = new Logger(textBoxTerminal);
            _wlf = new WordListFactory(_log);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dictdir = Path.Combine(appdata, "Subtitle Edit", "Dictionaries");
            if (Directory.Exists(dictdir))
            {
                GetDictionaryFiles(dictdir);
            }
            else
            {
                using (var fDialog = new FolderBrowserDialog())
                {
                    if (fDialog.ShowDialog() == DialogResult.OK)
                    {
                        GetDictionaryFiles(fDialog.SelectedPath);
                    }
                }
            }
        }

        private void GetDictionaryFiles(string dictdir)
        {
            foreach (var path in Directory.EnumerateFiles(dictdir, "*OCRFixReplaceList.xml"))
            {
                var wl = _wlf.CreateOcrFixReplaceList(path);
                treeViewWordLists.Nodes[0].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
            }
            foreach (var path in Directory.EnumerateFiles(dictdir, "*NoBreakAfterList.xml"))
            {
                var wl = _wlf.CreateNoBreakAfterList(path);
                treeViewWordLists.Nodes[1].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
            }
            foreach (var path in Directory.EnumerateFiles(dictdir, "*names_etc.xml"))
            {
                var wl = _wlf.CreateNamesEtcList(path);
                treeViewWordLists.Nodes[2].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
            }
            foreach (var path in Directory.EnumerateFiles(dictdir, "??_??_user.xml"))
            {
                var wl = _wlf.CreateUserList(path);
                treeViewWordLists.Nodes[3].Nodes.Add(new TreeNode { Tag = wl, Text = wl.Name, ContextMenuStrip = contextMenuStripWordLists });
            }
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

        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fb = new FolderBrowserDialog())
            {
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    _dictionaryPath = fb.SelectedPath;
                    Reload(_dictionaryPath);
                }
            }
        }

        private void Reload(string path)
        {
            // Todo: Cleanup conatiners
            GetDictionaryFiles(path);
        }
    }
}
