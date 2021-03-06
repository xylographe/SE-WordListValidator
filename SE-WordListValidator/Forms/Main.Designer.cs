﻿/*
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
namespace SubtitleEditWordListValidator
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("OCRFixReplace");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("NoBreakAfter");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("NamesEtc");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("User");
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.treeViewWordLists = new System.Windows.Forms.TreeView();
            this.menuStripWordLists = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileChangeFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders00 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders01 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders02 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders03 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders04 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders05 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders06 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders07 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders08 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileRecentFolders09 = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxTerminal = new System.Windows.Forms.TextBox();
            this.panelFind = new System.Windows.Forms.Panel();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.buttonFindClear = new System.Windows.Forms.Button();
            this.buttonFindGo = new System.Windows.Forms.Button();
            this.contextMenuStripWordLists = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemWordListsValidate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWordListsEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWordListsSubmit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWordListsAcceopt = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWordListsReject = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.menuStripWordLists.SuspendLayout();
            this.panelFind.SuspendLayout();
            this.contextMenuStripWordLists.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainerMain
            //
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            //
            // splitContainerMain.Panel1
            //
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewWordLists);
            this.splitContainerMain.Panel1.Controls.Add(this.menuStripWordLists);
            this.splitContainerMain.Panel1MinSize = 111;
            //
            // splitContainerMain.Panel2
            //
            this.splitContainerMain.Panel2.Controls.Add(this.textBoxTerminal);
            this.splitContainerMain.Panel2.Controls.Add(this.panelFind);
            this.splitContainerMain.Panel2MinSize = 333;
            this.splitContainerMain.Size = new System.Drawing.Size(800, 600);
            this.splitContainerMain.SplitterDistance = 181;
            this.splitContainerMain.SplitterWidth = 3;
            this.splitContainerMain.TabIndex = 0;
            //
            // treeViewWordLists
            //
            this.treeViewWordLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewWordLists.HideSelection = false;
            this.treeViewWordLists.ItemHeight = 20;
            this.treeViewWordLists.Location = new System.Drawing.Point(0, 24);
            this.treeViewWordLists.Name = "treeViewWordLists";
            treeNode1.Name = "NodeOCRFix";
            treeNode1.Text = "OCRFixReplace";
            treeNode2.Name = "NodeNoBreak";
            treeNode2.Text = "NoBreakAfter";
            treeNode3.Name = "NodeNames";
            treeNode3.Text = "NamesEtc";
            treeNode4.Name = "NodeUser";
            treeNode4.Text = "User";
            this.treeViewWordLists.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.treeViewWordLists.Size = new System.Drawing.Size(181, 576);
            this.treeViewWordLists.TabIndex = 1;
            this.treeViewWordLists.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewWordLists_AfterSelect);
            this.treeViewWordLists.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewWordLists_NodeMouseDoubleClick);
            //
            // menuStripWordLists
            //
            this.menuStripWordLists.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStripWordLists.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStripWordLists.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile});
            this.menuStripWordLists.Location = new System.Drawing.Point(0, 0);
            this.menuStripWordLists.Name = "menuStripWordLists";
            this.menuStripWordLists.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.menuStripWordLists.ShowItemToolTips = true;
            this.menuStripWordLists.Size = new System.Drawing.Size(181, 24);
            this.menuStripWordLists.TabIndex = 0;
            this.menuStripWordLists.MenuActivate += new System.EventHandler(this.MenuStripWordLists_MenuActivate);
            //
            // toolStripMenuItemFile
            //
            this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFileChangeFolder,
            this.toolStripMenuItemFileRecentFolders});
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            this.toolStripMenuItemFile.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItemFile.Text = "&File";
            //
            // toolStripMenuItemFileChangeFolder
            //
            this.toolStripMenuItemFileChangeFolder.Name = "toolStripMenuItemFileChangeFolder";
            this.toolStripMenuItemFileChangeFolder.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileChangeFolder.Text = "&Change folder";
            this.toolStripMenuItemFileChangeFolder.Click += new System.EventHandler(this.ToolStripMenuItemFileChangeFolder_Click);
            //
            // toolStripMenuItemFileRecentFolders
            //
            this.toolStripMenuItemFileRecentFolders.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFileRecentFolders00,
            this.toolStripMenuItemFileRecentFolders01,
            this.toolStripMenuItemFileRecentFolders02,
            this.toolStripMenuItemFileRecentFolders03,
            this.toolStripMenuItemFileRecentFolders04,
            this.toolStripMenuItemFileRecentFolders05,
            this.toolStripMenuItemFileRecentFolders06,
            this.toolStripMenuItemFileRecentFolders07,
            this.toolStripMenuItemFileRecentFolders08,
            this.toolStripMenuItemFileRecentFolders09});
            this.toolStripMenuItemFileRecentFolders.Name = "toolStripMenuItemFileRecentFolders";
            this.toolStripMenuItemFileRecentFolders.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders.Text = "Recent folders";
            //
            // toolStripMenuItemFileRecentFolders00
            //
            this.toolStripMenuItemFileRecentFolders00.Name = "toolStripMenuItemFileRecentFolders00";
            this.toolStripMenuItemFileRecentFolders00.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders00.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders01
            //
            this.toolStripMenuItemFileRecentFolders01.Name = "toolStripMenuItemFileRecentFolders01";
            this.toolStripMenuItemFileRecentFolders01.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders01.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders02
            //
            this.toolStripMenuItemFileRecentFolders02.Name = "toolStripMenuItemFileRecentFolders02";
            this.toolStripMenuItemFileRecentFolders02.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders02.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders03
            //
            this.toolStripMenuItemFileRecentFolders03.Name = "toolStripMenuItemFileRecentFolders03";
            this.toolStripMenuItemFileRecentFolders03.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders03.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders04
            //
            this.toolStripMenuItemFileRecentFolders04.Name = "toolStripMenuItemFileRecentFolders04";
            this.toolStripMenuItemFileRecentFolders04.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders04.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders05
            //
            this.toolStripMenuItemFileRecentFolders05.Name = "toolStripMenuItemFileRecentFolders05";
            this.toolStripMenuItemFileRecentFolders05.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders05.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders06
            //
            this.toolStripMenuItemFileRecentFolders06.Name = "toolStripMenuItemFileRecentFolders06";
            this.toolStripMenuItemFileRecentFolders06.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders06.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders07
            //
            this.toolStripMenuItemFileRecentFolders07.Name = "toolStripMenuItemFileRecentFolders07";
            this.toolStripMenuItemFileRecentFolders07.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders07.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders08
            //
            this.toolStripMenuItemFileRecentFolders08.Name = "toolStripMenuItemFileRecentFolders08";
            this.toolStripMenuItemFileRecentFolders08.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders08.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // toolStripMenuItemFileRecentFolders09
            //
            this.toolStripMenuItemFileRecentFolders09.Name = "toolStripMenuItemFileRecentFolders09";
            this.toolStripMenuItemFileRecentFolders09.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemFileRecentFolders09.Click += new System.EventHandler(this.ToolStripMenuItemFileRecentFolders_Click);
            //
            // textBoxTerminal
            //
            this.textBoxTerminal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTerminal.Location = new System.Drawing.Point(0, 48);
            this.textBoxTerminal.Multiline = true;
            this.textBoxTerminal.Name = "textBoxTerminal";
            this.textBoxTerminal.ReadOnly = true;
            this.textBoxTerminal.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxTerminal.Size = new System.Drawing.Size(616, 552);
            this.textBoxTerminal.TabIndex = 1;
            this.textBoxTerminal.WordWrap = false;
            this.textBoxTerminal.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxTerminal_MouseDoubleClick);
            //
            // panelFind
            //
            this.panelFind.Controls.Add(this.textBoxFind);
            this.panelFind.Controls.Add(this.buttonFindClear);
            this.panelFind.Controls.Add(this.buttonFindGo);
            this.panelFind.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFind.Location = new System.Drawing.Point(0, 0);
            this.panelFind.Name = "panelFind";
            this.panelFind.Size = new System.Drawing.Size(616, 48);
            this.panelFind.TabIndex = 0;
            //
            // textBoxFind
            //
            this.textBoxFind.AcceptsReturn = true;
            this.textBoxFind.AcceptsTab = true;
            this.textBoxFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFind.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFind.Location = new System.Drawing.Point(0, 0);
            this.textBoxFind.Multiline = true;
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(556, 48);
            this.textBoxFind.TabIndex = 0;
            this.textBoxFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxFind_KeyDown);
            //
            // buttonFindClear
            //
            this.buttonFindClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonFindClear.Font = new System.Drawing.Font("Wingdings", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonFindClear.ForeColor = System.Drawing.Color.Red;
            this.buttonFindClear.Location = new System.Drawing.Point(556, 0);
            this.buttonFindClear.Name = "buttonFindClear";
            this.buttonFindClear.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.buttonFindClear.Size = new System.Drawing.Size(30, 48);
            this.buttonFindClear.TabIndex = 1;
            this.buttonFindClear.Text = "";
            this.buttonFindClear.UseMnemonic = false;
            this.buttonFindClear.UseVisualStyleBackColor = true;
            this.buttonFindClear.Click += new System.EventHandler(this.ButtonFindClear_Click);
            //
            // buttonFindGo
            //
            this.buttonFindGo.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonFindGo.Font = new System.Drawing.Font("Wingdings", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonFindGo.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.buttonFindGo.Location = new System.Drawing.Point(586, 0);
            this.buttonFindGo.Name = "buttonFindGo";
            this.buttonFindGo.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.buttonFindGo.Size = new System.Drawing.Size(30, 48);
            this.buttonFindGo.TabIndex = 2;
            this.buttonFindGo.Text = "";
            this.buttonFindGo.UseMnemonic = false;
            this.buttonFindGo.UseVisualStyleBackColor = true;
            this.buttonFindGo.Click += new System.EventHandler(this.ButtonFindGo_Click);
            //
            // contextMenuStripWordLists
            //
            this.contextMenuStripWordLists.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemWordListsValidate,
            this.toolStripMenuItemWordListsEdit,
            this.toolStripMenuItemWordListsSubmit,
            this.toolStripMenuItemWordListsAcceopt,
            this.toolStripMenuItemWordListsReject});
            this.contextMenuStripWordLists.Name = "contextMenuStripWordLists";
            this.contextMenuStripWordLists.ShowImageMargin = false;
            this.contextMenuStripWordLists.Size = new System.Drawing.Size(92, 114);
            //
            // toolStripMenuItemWordListsValidate
            //
            this.toolStripMenuItemWordListsValidate.Name = "toolStripMenuItemWordListsValidate";
            this.toolStripMenuItemWordListsValidate.Size = new System.Drawing.Size(91, 22);
            this.toolStripMenuItemWordListsValidate.Text = "Validate";
            this.toolStripMenuItemWordListsValidate.Click += new System.EventHandler(this.ToolStripMenuItemWordListsValidate_Click);
            //
            // toolStripMenuItemWordListsEdit
            //
            this.toolStripMenuItemWordListsEdit.Name = "toolStripMenuItemWordListsEdit";
            this.toolStripMenuItemWordListsEdit.Size = new System.Drawing.Size(91, 22);
            this.toolStripMenuItemWordListsEdit.Text = "Edit";
            this.toolStripMenuItemWordListsEdit.Click += new System.EventHandler(this.ToolStripMenuItemWordListsEdit_Click);
            //
            // toolStripMenuItemWordListsSubmit
            //
            this.toolStripMenuItemWordListsSubmit.Name = "toolStripMenuItemWordListsSubmit";
            this.toolStripMenuItemWordListsSubmit.Size = new System.Drawing.Size(91, 22);
            this.toolStripMenuItemWordListsSubmit.Text = "Submit";
            this.toolStripMenuItemWordListsSubmit.Click += new System.EventHandler(this.ToolStripMenuItemWordListsSubmit_Click);
            //
            // toolStripMenuItemWordListsAcceopt
            //
            this.toolStripMenuItemWordListsAcceopt.Name = "toolStripMenuItemWordListsAcceopt";
            this.toolStripMenuItemWordListsAcceopt.Size = new System.Drawing.Size(91, 22);
            this.toolStripMenuItemWordListsAcceopt.Text = "Accept";
            this.toolStripMenuItemWordListsAcceopt.Click += new System.EventHandler(this.ToolStripMenuItemWordListsAcceopt_Click);
            //
            // toolStripMenuItemWordListsReject
            //
            this.toolStripMenuItemWordListsReject.Name = "toolStripMenuItemWordListsReject";
            this.toolStripMenuItemWordListsReject.Size = new System.Drawing.Size(91, 22);
            this.toolStripMenuItemWordListsReject.Text = "Reject";
            this.toolStripMenuItemWordListsReject.Click += new System.EventHandler(this.ToolStripMenuItemWordListsReject_Click);
            //
            // Main
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(416, 338);
            this.Name = "Main";
            this.Text = "Validate Subtitle Edit word lists";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.menuStripWordLists.ResumeLayout(false);
            this.menuStripWordLists.PerformLayout();
            this.panelFind.ResumeLayout(false);
            this.panelFind.PerformLayout();
            this.contextMenuStripWordLists.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.MenuStrip menuStripWordLists;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileChangeFolder;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders00;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders01;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders02;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders03;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders04;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders05;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders06;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders07;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders08;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileRecentFolders09;
        private System.Windows.Forms.TreeView treeViewWordLists;
        private System.Windows.Forms.TextBox textBoxTerminal;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripWordLists;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWordListsValidate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWordListsEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWordListsSubmit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWordListsAcceopt;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWordListsReject;
        private System.Windows.Forms.Panel panelFind;
        private System.Windows.Forms.TextBox textBoxFind;
        private System.Windows.Forms.Button buttonFindClear;
        private System.Windows.Forms.Button buttonFindGo;
    }
}
