namespace OPI_TextEditor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            openFileDialog = new OpenFileDialog();
            FilenameDisplay = new ToolStripLabel();
            toolStrip1 = new ToolStrip();
            FileMenu = new ToolStripDropDownButton();
            OpenFile = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            EditMenu = new ToolStripDropDownButton();
            cleanWhitespaceToolStripMenuItem = new ToolStripMenuItem();
            changeCaseToolStripMenuItem = new ToolStripMenuItem();
            AboutMenu = new ToolStripDropDownButton();
            statisticsToolStripMenuItem1 = new ToolStripMenuItem();
            regexTestToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            saveFileDialog1 = new SaveFileDialog();
            richTextBox1 = new RichTextBox();
            loadNewsToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            openFileDialog.Filter = "Allfiles | *.*";
            // 
            // FilenameDisplay
            // 
            FilenameDisplay.Name = "FilenameDisplay";
            FilenameDisplay.Size = new Size(105, 22);
            FilenameDisplay.Text = "Here's be filename";
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = SystemColors.ActiveBorder;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { FileMenu, EditMenu, AboutMenu, FilenameDisplay });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // FileMenu
            // 
            FileMenu.BackColor = SystemColors.Control;
            FileMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { OpenFile, loadNewsToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, exitToolStripMenuItem });
            FileMenu.Image = (Image)resources.GetObject("FileMenu.Image");
            FileMenu.ImageTransparentColor = Color.Magenta;
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(38, 22);
            FileMenu.Text = "File";
            // 
            // OpenFile
            // 
            OpenFile.Name = "OpenFile";
            OpenFile.Size = new Size(186, 22);
            OpenFile.Text = "Open";
            OpenFile.Click += OpenFile_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(186, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(186, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(186, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // EditMenu
            // 
            EditMenu.BackColor = SystemColors.Control;
            EditMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            EditMenu.DropDownItems.AddRange(new ToolStripItem[] { cleanWhitespaceToolStripMenuItem, changeCaseToolStripMenuItem });
            EditMenu.Image = (Image)resources.GetObject("EditMenu.Image");
            EditMenu.ImageTransparentColor = Color.Magenta;
            EditMenu.Name = "EditMenu";
            EditMenu.Size = new Size(40, 22);
            EditMenu.Text = "Edit";
            // 
            // cleanWhitespaceToolStripMenuItem
            // 
            cleanWhitespaceToolStripMenuItem.Name = "cleanWhitespaceToolStripMenuItem";
            cleanWhitespaceToolStripMenuItem.Size = new Size(168, 22);
            cleanWhitespaceToolStripMenuItem.Text = "Clean Whitespace";
            cleanWhitespaceToolStripMenuItem.Click += cleanWhitespaceToolStripMenuItem_Click;
            // 
            // changeCaseToolStripMenuItem
            // 
            changeCaseToolStripMenuItem.Name = "changeCaseToolStripMenuItem";
            changeCaseToolStripMenuItem.Size = new Size(168, 22);
            changeCaseToolStripMenuItem.Text = "Change Case";
            changeCaseToolStripMenuItem.Click += changeCaseToolStripMenuItem_Click;
            // 
            // AboutMenu
            // 
            AboutMenu.BackColor = SystemColors.Control;
            AboutMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AboutMenu.DropDownItems.AddRange(new ToolStripItem[] { statisticsToolStripMenuItem1, regexTestToolStripMenuItem, aboutToolStripMenuItem });
            AboutMenu.Image = (Image)resources.GetObject("AboutMenu.Image");
            AboutMenu.ImageTransparentColor = Color.Magenta;
            AboutMenu.Name = "AboutMenu";
            AboutMenu.Size = new Size(53, 22);
            AboutMenu.Text = "About";
            // 
            // statisticsToolStripMenuItem1
            // 
            statisticsToolStripMenuItem1.Name = "statisticsToolStripMenuItem1";
            statisticsToolStripMenuItem1.Size = new Size(141, 22);
            statisticsToolStripMenuItem1.Text = "Statistics";
            statisticsToolStripMenuItem1.Click += statisticsToolStripMenuItem1_Click;
            // 
            // regexTestToolStripMenuItem
            // 
            regexTestToolStripMenuItem.Name = "regexTestToolStripMenuItem";
            regexTestToolStripMenuItem.Size = new Size(141, 22);
            regexTestToolStripMenuItem.Text = "Regex Check";
            regexTestToolStripMenuItem.Click += regexTestToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(141, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 25);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(800, 425);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // loadNewsToolStripMenuItem
            // 
            loadNewsToolStripMenuItem.Name = "loadNewsToolStripMenuItem";
            loadNewsToolStripMenuItem.Size = new Size(186, 22);
            loadNewsToolStripMenuItem.Text = "Load News";
            loadNewsToolStripMenuItem.Click += loadNewsToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(richTextBox1);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenFileDialog openFileDialog;
        private ToolStripLabel FilenameDisplay;
        private ToolStrip toolStrip1;
        private SaveFileDialog saveFileDialog1;
        private RichTextBox richTextBox1;
        private ToolStripDropDownButton EditMenu;
        private ToolStripMenuItem cleanWhitespaceToolStripMenuItem;
        private ToolStripDropDownButton FileMenu;
        private ToolStripMenuItem OpenFile;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripDropDownButton AboutMenu;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem statisticsToolStripMenuItem1;
        private ToolStripMenuItem regexTestToolStripMenuItem;
        private ToolStripMenuItem changeCaseToolStripMenuItem;
        private ToolStripMenuItem loadNewsToolStripMenuItem;
    }
}
