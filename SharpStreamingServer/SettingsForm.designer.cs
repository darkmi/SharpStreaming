namespace Simon.SharpStreamingServer
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tvOptions = new System.Windows.Forms.TreeView();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.gbSeparatorTop = new System.Windows.Forms.GroupBox();
            this.lblOptionsName = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbSeparatorBottom = new System.Windows.Forms.GroupBox();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "settings_normal.png");
            this.imageList1.Images.SetKeyName(1, "settings_selected.png");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(419, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel(&C)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(161, 42);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(346, 206);
            this.pnlMain.TabIndex = 16;
            // 
            // tvOptions
            // 
            this.tvOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvOptions.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvOptions.FullRowSelect = true;
            this.tvOptions.HideSelection = false;
            this.tvOptions.ImageIndex = 0;
            this.tvOptions.ImageList = this.imageList1;
            this.tvOptions.ItemHeight = 35;
            this.tvOptions.Location = new System.Drawing.Point(0, 0);
            this.tvOptions.Name = "tvOptions";
            this.tvOptions.SelectedImageIndex = 1;
            this.tvOptions.ShowLines = false;
            this.tvOptions.ShowRootLines = false;
            this.tvOptions.Size = new System.Drawing.Size(161, 252);
            this.tvOptions.TabIndex = 0;
            this.tvOptions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvOptions_AfterSelect);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlHeader.Controls.Add(this.gbSeparatorTop);
            this.pnlHeader.Controls.Add(this.lblOptionsName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(161, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(346, 42);
            this.pnlHeader.TabIndex = 17;
            // 
            // gbSeparatorTop
            // 
            this.gbSeparatorTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSeparatorTop.Location = new System.Drawing.Point(0, 36);
            this.gbSeparatorTop.Name = "gbSeparatorTop";
            this.gbSeparatorTop.Size = new System.Drawing.Size(359, 5);
            this.gbSeparatorTop.TabIndex = 13;
            this.gbSeparatorTop.TabStop = false;
            // 
            // lblOptionsName
            // 
            this.lblOptionsName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblOptionsName.Location = new System.Drawing.Point(10, 9);
            this.lblOptionsName.Name = "lblOptionsName";
            this.lblOptionsName.Size = new System.Drawing.Size(311, 17);
            this.lblOptionsName.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(337, 10);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbSeparatorBottom
            // 
            this.gbSeparatorBottom.BackColor = System.Drawing.Color.Transparent;
            this.gbSeparatorBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSeparatorBottom.Location = new System.Drawing.Point(161, 248);
            this.gbSeparatorBottom.Name = "gbSeparatorBottom";
            this.gbSeparatorBottom.Size = new System.Drawing.Size(346, 4);
            this.gbSeparatorBottom.TabIndex = 13;
            this.gbSeparatorBottom.TabStop = false;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.tvOptions);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(161, 252);
            this.pnlLeft.TabIndex = 14;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.Transparent;
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 252);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(507, 47);
            this.pnlBottom.TabIndex = 15;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Simon.SharpStreamingServer.Properties.Resources.bg;
            this.ClientSize = new System.Drawing.Size(507, 299);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.gbSeparatorBottom);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.pnlHeader.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TreeView tvOptions;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.GroupBox gbSeparatorTop;
        private System.Windows.Forms.Label lblOptionsName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbSeparatorBottom;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlBottom;
    }
}