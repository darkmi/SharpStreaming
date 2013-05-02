namespace Simon.SharpStreamingServer
{
    partial class ServerForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstServerInfo = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvClientInfo = new System.Windows.Forms.ListView();
            this.colSessionId = new System.Windows.Forms.ColumnHeader();
            this.colSessionIP = new System.Windows.Forms.ColumnHeader();
            this.colSessionPort = new System.Windows.Forms.ColumnHeader();
            this.colSessionConnectTime = new System.Windows.Forms.ColumnHeader();
            this.colSessionState = new System.Windows.Forms.ColumnHeader();
            this.colSessionRequestStream = new System.Windows.Forms.ColumnHeader();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblLocalTimeText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblLocalTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblServerStateText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblServerState = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStartServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnStopServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHideServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnQuit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Sharp Streaming Server";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Simon.SharpStreamingServer.Properties.Resources.bg;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 435);
            this.panel1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.lstServerInfo);
            this.groupBox2.Location = new System.Drawing.Point(12, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(782, 187);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server Running Information";
            // 
            // lstServerInfo
            // 
            this.lstServerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstServerInfo.FormattingEnabled = true;
            this.lstServerInfo.HorizontalScrollbar = true;
            this.lstServerInfo.ItemHeight = 12;
            this.lstServerInfo.Location = new System.Drawing.Point(7, 20);
            this.lstServerInfo.Name = "lstServerInfo";
            this.lstServerInfo.Size = new System.Drawing.Size(768, 160);
            this.lstServerInfo.TabIndex = 0;
            this.lstServerInfo.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.lvClientInfo);
            this.groupBox1.Location = new System.Drawing.Point(13, 196);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(780, 211);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client Session Information";
            // 
            // lvClientInfo
            // 
            this.lvClientInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvClientInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSessionId,
            this.colSessionIP,
            this.colSessionPort,
            this.colSessionConnectTime,
            this.colSessionState,
            this.colSessionRequestStream});
            this.lvClientInfo.FullRowSelect = true;
            this.lvClientInfo.GridLines = true;
            this.lvClientInfo.Location = new System.Drawing.Point(6, 20);
            this.lvClientInfo.Name = "lvClientInfo";
            this.lvClientInfo.Size = new System.Drawing.Size(768, 185);
            this.lvClientInfo.TabIndex = 0;
            this.lvClientInfo.UseCompatibleStateImageBehavior = false;
            this.lvClientInfo.View = System.Windows.Forms.View.Details;
            // 
            // colSessionId
            // 
            this.colSessionId.Text = "Session ID";
            this.colSessionId.Width = 80;
            // 
            // colSessionIP
            // 
            this.colSessionIP.Text = "Session IP";
            this.colSessionIP.Width = 120;
            // 
            // colSessionPort
            // 
            this.colSessionPort.Text = "Port";
            // 
            // colSessionConnectTime
            // 
            this.colSessionConnectTime.Text = "Connect Time";
            this.colSessionConnectTime.Width = 140;
            // 
            // colSessionState
            // 
            this.colSessionState.Text = "Session State";
            this.colSessionState.Width = 100;
            // 
            // colSessionRequestStream
            // 
            this.colSessionRequestStream.Text = "Request Stream";
            this.colSessionRequestStream.Width = 230;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Simon.SharpStreamingServer.Properties.Resources.statusbar_bg;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLocalTimeText,
            this.lblLocalTime,
            this.toolStripStatusLabel2,
            this.lblServerStateText,
            this.lblServerState});
            this.statusStrip1.Location = new System.Drawing.Point(0, 413);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(806, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblLocalTimeText
            // 
            this.lblLocalTimeText.BackColor = System.Drawing.Color.Transparent;
            this.lblLocalTimeText.Name = "lblLocalTimeText";
            this.lblLocalTimeText.Size = new System.Drawing.Size(89, 17);
            this.lblLocalTimeText.Text = "Current Time: ";
            // 
            // lblLocalTime
            // 
            this.lblLocalTime.BackColor = System.Drawing.Color.Transparent;
            this.lblLocalTime.Name = "lblLocalTime";
            this.lblLocalTime.Size = new System.Drawing.Size(65, 17);
            this.lblLocalTime.Text = "local time";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(23, 17);
            this.toolStripStatusLabel2.Text = " | ";
            // 
            // lblServerStateText
            // 
            this.lblServerStateText.Name = "lblServerStateText";
            this.lblServerStateText.Size = new System.Drawing.Size(89, 17);
            this.lblServerStateText.Text = "Server State: ";
            // 
            // lblServerState
            // 
            this.lblServerState.Name = "lblServerState";
            this.lblServerState.Size = new System.Drawing.Size(119, 17);
            this.lblServerState.Text = "Wait for running...";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_bg;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStartServer,
            this.toolStripSeparator1,
            this.btnStopServer,
            this.toolStripSeparator2,
            this.btnHideServer,
            this.toolStripSeparator3,
            this.btnSettings,
            this.toolStripSeparator5,
            this.btnAbout,
            this.toolStripSeparator4,
            this.btnQuit,
            this.toolStripSeparator6});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(806, 60);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStartServer
            // 
            this.btnStartServer.AutoSize = false;
            this.btnStartServer.BackColor = System.Drawing.SystemColors.Control;
            this.btnStartServer.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_start;
            this.btnStartServer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStartServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(85, 50);
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStartServer.ToolTipText = "Start the Sharp Streaming Server";
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 60);
            // 
            // btnStopServer
            // 
            this.btnStopServer.AutoSize = false;
            this.btnStopServer.Enabled = false;
            this.btnStopServer.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_stop;
            this.btnStopServer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStopServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(85, 50);
            this.btnStopServer.Text = "Stop Server";
            this.btnStopServer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStopServer.ToolTipText = "Stop the Sharp Streaming Server";
            this.btnStopServer.Click += new System.EventHandler(this.btnStopServer_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 60);
            // 
            // btnHideServer
            // 
            this.btnHideServer.AutoSize = false;
            this.btnHideServer.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_hide;
            this.btnHideServer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnHideServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHideServer.Name = "btnHideServer";
            this.btnHideServer.Size = new System.Drawing.Size(85, 50);
            this.btnHideServer.Text = "Hide Server";
            this.btnHideServer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnHideServer.ToolTipText = "Hide the Sharp Streaming Server";
            this.btnHideServer.Click += new System.EventHandler(this.btnHideServer_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 60);
            // 
            // btnSettings
            // 
            this.btnSettings.AutoSize = false;
            this.btnSettings.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_setting;
            this.btnSettings.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(85, 50);
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSettings.ToolTipText = "Server Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 60);
            // 
            // btnAbout
            // 
            this.btnAbout.AutoSize = false;
            this.btnAbout.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_about;
            this.btnAbout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(85, 50);
            this.btnAbout.Text = "About";
            this.btnAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAbout.ToolTipText = "About the Sharp Streaming Server";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 60);
            // 
            // btnQuit
            // 
            this.btnQuit.AutoSize = false;
            this.btnQuit.Image = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_quit;
            this.btnQuit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(85, 50);
            this.btnQuit.Text = " Quit";
            this.btnQuit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnQuit.ToolTipText = " Quit the Sharp Streaming Server";
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 60);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Simon.SharpStreamingServer.Properties.Resources.toolbar_bg;
            this.ClientSize = new System.Drawing.Size(806, 495);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sharp Streaming Server";
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnStartServer;
        private System.Windows.Forms.ToolStripButton btnStopServer;
        private System.Windows.Forms.ToolStripButton btnHideServer;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripButton btnAbout;
        private System.Windows.Forms.ToolStripButton btnQuit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblLocalTimeText;
        private System.Windows.Forms.ToolStripStatusLabel lblLocalTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox lstServerInfo;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lblServerStateText;
        private System.Windows.Forms.ToolStripStatusLabel lblServerState;
        private System.Windows.Forms.ListView lvClientInfo;
        private System.Windows.Forms.ColumnHeader colSessionId;
        private System.Windows.Forms.ColumnHeader colSessionIP;
        private System.Windows.Forms.ColumnHeader colSessionPort;
        private System.Windows.Forms.ColumnHeader colSessionConnectTime;
        private System.Windows.Forms.ColumnHeader colSessionState;
        private System.Windows.Forms.ColumnHeader colSessionRequestStream;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

