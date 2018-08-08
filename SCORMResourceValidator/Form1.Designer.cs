namespace SCORMResourceValidator
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUserPrompt = new System.Windows.Forms.Label();
            this.grpPIFFilesFound = new System.Windows.Forms.GroupBox();
            this.listPIFFilesFound = new System.Windows.Forms.ListBox();
            this.grpManifestFilesFound = new System.Windows.Forms.GroupBox();
            this.listManifestFilesFound = new System.Windows.Forms.ListBox();
            this.grpPIFFilesMissing = new System.Windows.Forms.GroupBox();
            this.warningPiffmissing = new System.Windows.Forms.PictureBox();
            this.listPIFFilesMissing = new System.Windows.Forms.ListBox();
            this.grpManifestFilesMissing = new System.Windows.Forms.GroupBox();
            this.warningMFMising = new System.Windows.Forms.PictureBox();
            this.listManifestFilesMissing = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnValidate = new System.Windows.Forms.Button();
            this.lblPIFFilename = new System.Windows.Forms.Label();
            this.SelectPIF = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkViewLogs = new System.Windows.Forms.LinkLabel();
            this.goldbarpnl = new System.Windows.Forms.Panel();
            this.goldbarpnlbottom = new System.Windows.Forms.Panel();
            this.logFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblLogFileDir = new System.Windows.Forms.Label();
            this.btnSelectLogDir = new System.Windows.Forms.Button();
            this.lblPrevLogs = new System.Windows.Forms.Label();
            this.lbllogvalidate = new System.Windows.Forms.Label();
            this.warningLogsfail = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblADLlogFileDir = new System.Windows.Forms.Label();
            this.btnSelectADLLogDir = new System.Windows.Forms.Button();
            this.lblADLTestSuitelogs = new System.Windows.Forms.Label();
            this.ADLlogFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.warningtestsuitelogfail = new System.Windows.Forms.PictureBox();
            this.lblTSlogvalidate = new System.Windows.Forms.Label();
            this.grpPIFFilesFound.SuspendLayout();
            this.grpManifestFilesFound.SuspendLayout();
            this.grpPIFFilesMissing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPiffmissing)).BeginInit();
            this.grpManifestFilesMissing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningMFMising)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningLogsfail)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningtestsuitelogfail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(13, 140);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblStatus.Size = new System.Drawing.Size(917, 25);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "no file selected";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Visible = false;
            // 
            // lblUserPrompt
            // 
            this.lblUserPrompt.AutoSize = true;
            this.lblUserPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPrompt.Location = new System.Drawing.Point(15, 35);
            this.lblUserPrompt.Name = "lblUserPrompt";
            this.lblUserPrompt.Size = new System.Drawing.Size(548, 20);
            this.lblUserPrompt.TabIndex = 4;
            this.lblUserPrompt.Text = "Select the \"Browse\" button and chose a SCORM PIF to validate.";
            // 
            // grpPIFFilesFound
            // 
            this.grpPIFFilesFound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPIFFilesFound.Controls.Add(this.listPIFFilesFound);
            this.grpPIFFilesFound.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPIFFilesFound.Location = new System.Drawing.Point(13, 172);
            this.grpPIFFilesFound.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpPIFFilesFound.Name = "grpPIFFilesFound";
            this.grpPIFFilesFound.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpPIFFilesFound.Size = new System.Drawing.Size(917, 96);
            this.grpPIFFilesFound.TabIndex = 6;
            this.grpPIFFilesFound.TabStop = false;
            this.grpPIFFilesFound.Text = "Files found in PIF";
            // 
            // listPIFFilesFound
            // 
            this.listPIFFilesFound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPIFFilesFound.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listPIFFilesFound.FormattingEnabled = true;
            this.listPIFFilesFound.ItemHeight = 16;
            this.listPIFFilesFound.Location = new System.Drawing.Point(6, 21);
            this.listPIFFilesFound.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listPIFFilesFound.Name = "listPIFFilesFound";
            this.listPIFFilesFound.ScrollAlwaysVisible = true;
            this.listPIFFilesFound.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listPIFFilesFound.Size = new System.Drawing.Size(906, 68);
            this.listPIFFilesFound.TabIndex = 0;
            // 
            // grpManifestFilesFound
            // 
            this.grpManifestFilesFound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpManifestFilesFound.Controls.Add(this.listManifestFilesFound);
            this.grpManifestFilesFound.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpManifestFilesFound.Location = new System.Drawing.Point(13, 276);
            this.grpManifestFilesFound.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpManifestFilesFound.Name = "grpManifestFilesFound";
            this.grpManifestFilesFound.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpManifestFilesFound.Size = new System.Drawing.Size(917, 99);
            this.grpManifestFilesFound.TabIndex = 7;
            this.grpManifestFilesFound.TabStop = false;
            this.grpManifestFilesFound.Text = "Resources found in Manifest";
            // 
            // listManifestFilesFound
            // 
            this.listManifestFilesFound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listManifestFilesFound.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listManifestFilesFound.FormattingEnabled = true;
            this.listManifestFilesFound.ItemHeight = 16;
            this.listManifestFilesFound.Location = new System.Drawing.Point(6, 21);
            this.listManifestFilesFound.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listManifestFilesFound.Name = "listManifestFilesFound";
            this.listManifestFilesFound.ScrollAlwaysVisible = true;
            this.listManifestFilesFound.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listManifestFilesFound.Size = new System.Drawing.Size(906, 68);
            this.listManifestFilesFound.TabIndex = 0;
            // 
            // grpPIFFilesMissing
            // 
            this.grpPIFFilesMissing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPIFFilesMissing.BackColor = System.Drawing.SystemColors.Control;
            this.grpPIFFilesMissing.Controls.Add(this.warningPiffmissing);
            this.grpPIFFilesMissing.Controls.Add(this.listPIFFilesMissing);
            this.grpPIFFilesMissing.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grpPIFFilesMissing.Location = new System.Drawing.Point(13, 385);
            this.grpPIFFilesMissing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpPIFFilesMissing.Name = "grpPIFFilesMissing";
            this.grpPIFFilesMissing.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpPIFFilesMissing.Size = new System.Drawing.Size(917, 97);
            this.grpPIFFilesMissing.TabIndex = 8;
            this.grpPIFFilesMissing.TabStop = false;
            this.grpPIFFilesMissing.Text = "Files found in Manifest and not found in PIF";
            // 
            // warningPiffmissing
            // 
            this.warningPiffmissing.Image = global::SCORMResourceValidator.Properties.Resources.alert_triangle_redyellow_icon;
            this.warningPiffmissing.Location = new System.Drawing.Point(397, 0);
            this.warningPiffmissing.Name = "warningPiffmissing";
            this.warningPiffmissing.Size = new System.Drawing.Size(23, 21);
            this.warningPiffmissing.TabIndex = 2;
            this.warningPiffmissing.TabStop = false;
            this.warningPiffmissing.Visible = false;
            // 
            // listPIFFilesMissing
            // 
            this.listPIFFilesMissing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPIFFilesMissing.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listPIFFilesMissing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listPIFFilesMissing.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listPIFFilesMissing.FormattingEnabled = true;
            this.listPIFFilesMissing.ItemHeight = 16;
            this.listPIFFilesMissing.Location = new System.Drawing.Point(6, 21);
            this.listPIFFilesMissing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listPIFFilesMissing.Name = "listPIFFilesMissing";
            this.listPIFFilesMissing.ScrollAlwaysVisible = true;
            this.listPIFFilesMissing.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listPIFFilesMissing.Size = new System.Drawing.Size(906, 68);
            this.listPIFFilesMissing.TabIndex = 0;
            // 
            // grpManifestFilesMissing
            // 
            this.grpManifestFilesMissing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpManifestFilesMissing.Controls.Add(this.warningMFMising);
            this.grpManifestFilesMissing.Controls.Add(this.listManifestFilesMissing);
            this.grpManifestFilesMissing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpManifestFilesMissing.Location = new System.Drawing.Point(13, 493);
            this.grpManifestFilesMissing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpManifestFilesMissing.Name = "grpManifestFilesMissing";
            this.grpManifestFilesMissing.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpManifestFilesMissing.Size = new System.Drawing.Size(917, 103);
            this.grpManifestFilesMissing.TabIndex = 8;
            this.grpManifestFilesMissing.TabStop = false;
            this.grpManifestFilesMissing.Text = "Files found in PIF and not listed in Manifest";
            // 
            // warningMFMising
            // 
            this.warningMFMising.Image = global::SCORMResourceValidator.Properties.Resources.alert_triangle_redyellow_icon;
            this.warningMFMising.Location = new System.Drawing.Point(397, 0);
            this.warningMFMising.Name = "warningMFMising";
            this.warningMFMising.Size = new System.Drawing.Size(23, 21);
            this.warningMFMising.TabIndex = 1;
            this.warningMFMising.TabStop = false;
            this.warningMFMising.Visible = false;
            // 
            // listManifestFilesMissing
            // 
            this.listManifestFilesMissing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listManifestFilesMissing.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listManifestFilesMissing.FormattingEnabled = true;
            this.listManifestFilesMissing.ItemHeight = 16;
            this.listManifestFilesMissing.Location = new System.Drawing.Point(6, 21);
            this.listManifestFilesMissing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listManifestFilesMissing.Name = "listManifestFilesMissing";
            this.listManifestFilesMissing.ScrollAlwaysVisible = true;
            this.listManifestFilesMissing.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listManifestFilesMissing.Size = new System.Drawing.Size(906, 68);
            this.listManifestFilesMissing.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(946, 28);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToUseToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // howToUseToolStripMenuItem
            // 
            this.howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
            this.howToUseToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.howToUseToolStripMenuItem.Text = "Instructions";
            this.howToUseToolStripMenuItem.Click += new System.EventHandler(this.howToUseToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "PIF file|*.zip";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // btnValidate
            // 
            this.btnValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValidate.Enabled = false;
            this.btnValidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnValidate.Location = new System.Drawing.Point(770, 59);
            this.btnValidate.Margin = new System.Windows.Forms.Padding(4);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(160, 44);
            this.btnValidate.TabIndex = 11;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // lblPIFFilename
            // 
            this.lblPIFFilename.AutoSize = true;
            this.lblPIFFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPIFFilename.Location = new System.Drawing.Point(111, 20);
            this.lblPIFFilename.Name = "lblPIFFilename";
            this.lblPIFFilename.Size = new System.Drawing.Size(111, 17);
            this.lblPIFFilename.TabIndex = 2;
            this.lblPIFFilename.Text = "No PIF selected.";
            // 
            // SelectPIF
            // 
            this.SelectPIF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectPIF.Location = new System.Drawing.Point(6, 14);
            this.SelectPIF.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectPIF.Name = "SelectPIF";
            this.SelectPIF.Size = new System.Drawing.Size(101, 26);
            this.SelectPIF.TabIndex = 1;
            this.SelectPIF.Text = "Browse...";
            this.SelectPIF.UseVisualStyleBackColor = true;
            this.SelectPIF.Click += new System.EventHandler(this.SelectPIF_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.SelectPIF);
            this.groupBox1.Controls.Add(this.lblPIFFilename);
            this.groupBox1.Location = new System.Drawing.Point(13, 55);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(746, 49);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // linkViewLogs
            // 
            this.linkViewLogs.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.linkViewLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkViewLogs.AutoSize = true;
            this.linkViewLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.linkViewLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkViewLogs.Location = new System.Drawing.Point(813, 141);
            this.linkViewLogs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkViewLogs.Name = "linkViewLogs";
            this.linkViewLogs.Size = new System.Drawing.Size(119, 20);
            this.linkViewLogs.TabIndex = 13;
            this.linkViewLogs.TabStop = true;
            this.linkViewLogs.Text = "View Log Files";
            this.linkViewLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkViewLogs.Visible = false;
            this.linkViewLogs.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkViewLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkViewLogs_LinkClicked);
            // 
            // goldbarpnl
            // 
            this.goldbarpnl.BackColor = System.Drawing.Color.Goldenrod;
            this.goldbarpnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.goldbarpnl.Location = new System.Drawing.Point(-4, 111);
            this.goldbarpnl.Name = "goldbarpnl";
            this.goldbarpnl.Size = new System.Drawing.Size(958, 15);
            this.goldbarpnl.TabIndex = 14;
            // 
            // goldbarpnlbottom
            // 
            this.goldbarpnlbottom.BackColor = System.Drawing.Color.Goldenrod;
            this.goldbarpnlbottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.goldbarpnlbottom.Location = new System.Drawing.Point(0, 607);
            this.goldbarpnlbottom.Name = "goldbarpnlbottom";
            this.goldbarpnlbottom.Size = new System.Drawing.Size(958, 15);
            this.goldbarpnlbottom.TabIndex = 15;
            // 
            // logFolderBrowserDialog
            // 
            this.logFolderBrowserDialog.HelpRequest += new System.EventHandler(this.logFolderBrowserDialog_HelpRequest);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblLogFileDir);
            this.groupBox2.Controls.Add(this.btnSelectLogDir);
            this.groupBox2.Location = new System.Drawing.Point(13, 641);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(746, 52);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // lblLogFileDir
            // 
            this.lblLogFileDir.AutoSize = true;
            this.lblLogFileDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogFileDir.Location = new System.Drawing.Point(111, 20);
            this.lblLogFileDir.Name = "lblLogFileDir";
            this.lblLogFileDir.Size = new System.Drawing.Size(144, 17);
            this.lblLogFileDir.TabIndex = 1;
            this.lblLogFileDir.Text = "No Directory selected";
            // 
            // btnSelectLogDir
            // 
            this.btnSelectLogDir.Enabled = false;
            this.btnSelectLogDir.Location = new System.Drawing.Point(6, 14);
            this.btnSelectLogDir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectLogDir.Name = "btnSelectLogDir";
            this.btnSelectLogDir.Size = new System.Drawing.Size(101, 26);
            this.btnSelectLogDir.TabIndex = 0;
            this.btnSelectLogDir.Text = "Browse";
            this.btnSelectLogDir.UseVisualStyleBackColor = true;
            this.btnSelectLogDir.Click += new System.EventHandler(this.btnSelectLogDir_Click);
            // 
            // lblPrevLogs
            // 
            this.lblPrevLogs.AutoSize = true;
            this.lblPrevLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevLogs.Location = new System.Drawing.Point(13, 629);
            this.lblPrevLogs.Name = "lblPrevLogs";
            this.lblPrevLogs.Size = new System.Drawing.Size(477, 18);
            this.lblPrevLogs.TabIndex = 17;
            this.lblPrevLogs.Text = "Optional: Select directory to validate previously made log files.";
            // 
            // lbllogvalidate
            // 
            this.lbllogvalidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbllogvalidate.ForeColor = System.Drawing.Color.OliveDrab;
            this.lbllogvalidate.Location = new System.Drawing.Point(794, 630);
            this.lbllogvalidate.Name = "lbllogvalidate";
            this.lbllogvalidate.Size = new System.Drawing.Size(138, 68);
            this.lbllogvalidate.TabIndex = 18;
            this.lbllogvalidate.Text = "The log files selected were validated and correct.";
            this.lbllogvalidate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbllogvalidate.Visible = false;
            // 
            // warningLogsfail
            // 
            this.warningLogsfail.Image = global::SCORMResourceValidator.Properties.Resources.alert_triangle_redyellow_icon;
            this.warningLogsfail.Location = new System.Drawing.Point(770, 633);
            this.warningLogsfail.Name = "warningLogsfail";
            this.warningLogsfail.Size = new System.Drawing.Size(23, 21);
            this.warningLogsfail.TabIndex = 3;
            this.warningLogsfail.TabStop = false;
            this.warningLogsfail.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblADLlogFileDir);
            this.groupBox3.Controls.Add(this.btnSelectADLLogDir);
            this.groupBox3.Location = new System.Drawing.Point(16, 709);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(746, 52);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            // 
            // lblADLlogFileDir
            // 
            this.lblADLlogFileDir.AutoSize = true;
            this.lblADLlogFileDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblADLlogFileDir.Location = new System.Drawing.Point(111, 20);
            this.lblADLlogFileDir.Name = "lblADLlogFileDir";
            this.lblADLlogFileDir.Size = new System.Drawing.Size(144, 17);
            this.lblADLlogFileDir.TabIndex = 1;
            this.lblADLlogFileDir.Text = "No Directory selected";
            // 
            // btnSelectADLLogDir
            // 
            this.btnSelectADLLogDir.Enabled = false;
            this.btnSelectADLLogDir.Location = new System.Drawing.Point(6, 14);
            this.btnSelectADLLogDir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectADLLogDir.Name = "btnSelectADLLogDir";
            this.btnSelectADLLogDir.Size = new System.Drawing.Size(101, 26);
            this.btnSelectADLLogDir.TabIndex = 0;
            this.btnSelectADLLogDir.Text = "Browse";
            this.btnSelectADLLogDir.UseVisualStyleBackColor = true;
            this.btnSelectADLLogDir.Click += new System.EventHandler(this.btnSelectADLLogDir_Click);
            // 
            // lblADLTestSuitelogs
            // 
            this.lblADLTestSuitelogs.AutoSize = true;
            this.lblADLTestSuitelogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblADLTestSuitelogs.Location = new System.Drawing.Point(14, 697);
            this.lblADLTestSuitelogs.Name = "lblADLTestSuitelogs";
            this.lblADLTestSuitelogs.Size = new System.Drawing.Size(452, 18);
            this.lblADLTestSuitelogs.TabIndex = 19;
            this.lblADLTestSuitelogs.Text = "Optional: Select directory to parse ADL Test Suite log files.";
            // 
            // warningtestsuitelogfail
            // 
            this.warningtestsuitelogfail.Image = global::SCORMResourceValidator.Properties.Resources.alert_triangle_redyellow_icon;
            this.warningtestsuitelogfail.Location = new System.Drawing.Point(770, 702);
            this.warningtestsuitelogfail.Name = "warningtestsuitelogfail";
            this.warningtestsuitelogfail.Size = new System.Drawing.Size(23, 21);
            this.warningtestsuitelogfail.TabIndex = 20;
            this.warningtestsuitelogfail.TabStop = false;
            this.warningtestsuitelogfail.Visible = false;
            // 
            // lblTSlogvalidate
            // 
            this.lblTSlogvalidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTSlogvalidate.ForeColor = System.Drawing.Color.OliveDrab;
            this.lblTSlogvalidate.Location = new System.Drawing.Point(799, 702);
            this.lblTSlogvalidate.Name = "lblTSlogvalidate";
            this.lblTSlogvalidate.Size = new System.Drawing.Size(138, 68);
            this.lblTSlogvalidate.TabIndex = 21;
            this.lblTSlogvalidate.Text = "The log files selected were validated and correct.";
            this.lblTSlogvalidate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTSlogvalidate.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(946, 771);
            this.Controls.Add(this.lblTSlogvalidate);
            this.Controls.Add(this.warningtestsuitelogfail);
            this.Controls.Add(this.lblADLTestSuitelogs);
            this.Controls.Add(this.warningLogsfail);
            this.Controls.Add(this.lbllogvalidate);
            this.Controls.Add(this.lblPrevLogs);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.goldbarpnlbottom);
            this.Controls.Add(this.goldbarpnl);
            this.Controls.Add(this.linkViewLogs);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.grpManifestFilesMissing);
            this.Controls.Add(this.grpPIFFilesMissing);
            this.Controls.Add(this.grpManifestFilesFound);
            this.Controls.Add(this.grpPIFFilesFound);
            this.Controls.Add(this.lblUserPrompt);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SCORM Resource Validator";
            this.grpPIFFilesFound.ResumeLayout(false);
            this.grpManifestFilesFound.ResumeLayout(false);
            this.grpPIFFilesMissing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.warningPiffmissing)).EndInit();
            this.grpManifestFilesMissing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.warningMFMising)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningLogsfail)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningtestsuitelogfail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUserPrompt;
        private System.Windows.Forms.GroupBox grpPIFFilesFound;
        private System.Windows.Forms.ListBox listPIFFilesFound;
        private System.Windows.Forms.GroupBox grpManifestFilesFound;
        private System.Windows.Forms.ListBox listManifestFilesFound;
        private System.Windows.Forms.GroupBox grpPIFFilesMissing;
        private System.Windows.Forms.ListBox listPIFFilesMissing;
        private System.Windows.Forms.GroupBox grpManifestFilesMissing;
        private System.Windows.Forms.ListBox listManifestFilesMissing;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Label lblPIFFilename;
        private System.Windows.Forms.Button SelectPIF;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howToUseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.LinkLabel linkViewLogs;
        private System.Windows.Forms.Panel goldbarpnl;
        private System.Windows.Forms.Panel goldbarpnlbottom;
        private System.Windows.Forms.FolderBrowserDialog logFolderBrowserDialog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblLogFileDir;
        private System.Windows.Forms.Button btnSelectLogDir;
        private System.Windows.Forms.Label lblPrevLogs;
        private System.Windows.Forms.PictureBox warningMFMising;
        private System.Windows.Forms.PictureBox warningPiffmissing;
        private System.Windows.Forms.Label lbllogvalidate;
        private System.Windows.Forms.PictureBox warningLogsfail;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblADLlogFileDir;
        private System.Windows.Forms.Button btnSelectADLLogDir;
        private System.Windows.Forms.Label lblADLTestSuitelogs;
        private System.Windows.Forms.FolderBrowserDialog ADLlogFolderBrowserDialog;
        private System.Windows.Forms.PictureBox warningtestsuitelogfail;
        private System.Windows.Forms.Label lblTSlogvalidate;
    }
}

