using System;
using System.Text;
using System.Windows.Forms;
using LogThis;
using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingServer
{
    public partial class ServerForm : Form
    {
        #region Private Member Fields
        /// <summary>
        /// Declares an object of RtspServer.
        /// </summary>
        private RtspServer rtspServer;

        /// <summary>
        /// A flag shows that whether the server is running or not.
        /// </summary>
        private bool isServerRunning;

        /// <summary>
        /// A flag shows that whether we quit the server now or not.
        /// </summary>
        private bool isQuitNow;

        /// <summary>
        /// The mode of the form closing, if it was true, then minimum the 
        /// form to the tray, else close the form and exit.
        /// </summary>
        private bool isMinToTray;

        /// <summary>
        /// The server port number.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// The maximum connection count of the client session.
        /// </summary>
        private int maxConnectionCount;

        /// <summary>
        /// The maximum time-out of the client session.
        /// </summary>
        private int maxSessionTimeout;

        /// <summary>
        /// The media file catalog.
        /// </summary>
        private string fileCatalog;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerForm"/> class.
        /// </summary>
        public ServerForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
        }

        #region Initialize
        /// <summary>
        /// Initializes.
        /// </summary>
        private void Initialize()
        {
            this.lblLocalTime.Text = DateTime.Now.ToString();

            // Initializes the server log.
            string logLocation = Utils.CreateDirectory("Log");
            string logFileName = "ServerLogFile";
            Log.UseSensibleDefaults(logFileName, logLocation, eloglevel.info);

            AddServerInfo("Server is wait for running...");

            InitFromConfigFile();
        }

        /// <summary>
        /// Initializes from config file.
        /// </summary>
        private void InitFromConfigFile()
        {
            Config cfg = new Config();
            string result = string.Empty;

            result = cfg.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_CLOSEMODE);
            if (result.Equals("0"))
            {
                isMinToTray = true;
            }
            else
            {
                isMinToTray = false;
            }

            result = cfg.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXCONNECTIONCOUNT);
            maxConnectionCount = Utils.StringToInteger(result);

            result = cfg.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXSESSIONTIMEOUT);
            maxSessionTimeout = Utils.StringToInteger(result);

            result = cfg.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_SERVERPORT);
            serverPort = Utils.StringToInteger(result);

            result = cfg.GetConfigValue(Constants.CONFIG_SECTION_CATALOG, Constants.CONFIG_KEY_FILECATALOG);
            fileCatalog = result;
        }
        #endregion

        #region Start/Stop Server Methods
        /// <summary>
        /// Starts the server.
        /// </summary>
        private void StartServer()
        {
            Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Try to start the server.");

            rtspServer = new RtspServer(serverPort, maxConnectionCount, maxSessionTimeout, fileCatalog);

            if (rtspServer == null)
            {
                return;
            }

            HookServerEvents();

            bool result = rtspServer.StartServer();
            if (result)
            {
                isServerRunning = true;
                UpdateServerStatus();

                Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Start the server successfully.");
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        private void StopServer()
        {
            if (rtspServer == null)
            {
                return;
            }

            Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Try to stop the server.");

            bool result = rtspServer.StopServer();
            if (result)
            {
                isServerRunning = false;
                UpdateServerStatus();

                Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Stop the server successfully.");
            }
        }
        #endregion

        #region Events Handler
        /// <summary>
        /// Hooks the server events.
        /// </summary>
        private void HookServerEvents()
        {
            rtspServer.ServerStarted += new EventHandler(rtspServer_ServerStarted);
            rtspServer.ServerStopped += new EventHandler(rtspServer_ServerStopped);
            rtspServer.ExceptionOccurred += new EventHandler<TExceptionEventArgs>(rtspServer_ExceptionOccurred);

            rtspServer.ClientSessionRejected += new EventHandler(rtspServer_ClientSessionRejected);
            rtspServer.ClientSessionConnected += new EventHandler<TClientSessionEventArgs>(rtspServer_ClientSessionConnected);
            rtspServer.ClientSessionDisconnected += new EventHandler<TClientSessionEventArgs>(rtspServer_ClientSessionDisconnected);
            rtspServer.ClientSessionTimeout += new EventHandler<TClientSessionEventArgs>(rtspServer_ClientSessionTimeout);
            rtspServer.ClientSessionUpdated += new EventHandler<TClientSessionEventArgs>(rtspServer_ClientSessionUpdated);
        }

        /// <summary>
        /// Handles the ClientSessionUpdated event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TClientSessionEventArgs"/> instance containing the event data.</param>
        void rtspServer_ClientSessionUpdated(object sender, TClientSessionEventArgs e)
        {
            UpdateClientInfo(e.SessionInfo);
            AddServerInfo(string.Format("Client [{0}:{1}] sent a [{2}] request.",
                e.SessionInfo.ClientSessionIP, e.SessionInfo.ClientSessionPort.ToString(), e.SessionInfo.SessionState));
        }

        /// <summary>
        /// Handles the ClientSessionTimeout event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TClientSessionEventArgs"/> instance containing the event data.</param>
        void rtspServer_ClientSessionTimeout(object sender, TClientSessionEventArgs e)
        {
            AddServerInfo(string.Format("Client [{0}:{1}] was time out.",
                e.SessionInfo.ClientSessionIP, e.SessionInfo.ClientSessionPort.ToString()));
        }

        /// <summary>
        /// Handles the ClientSessionDisconnected event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TClientSessionEventArgs"/> instance containing the event data.</param>
        void rtspServer_ClientSessionDisconnected(object sender, TClientSessionEventArgs e)
        {
            RemoveClientInfo(e.SessionInfo);
            AddServerInfo(string.Format("Client [{0}:{1}] was disconnected.",
                e.SessionInfo.ClientSessionIP, e.SessionInfo.ClientSessionPort.ToString()));
        }

        /// <summary>
        /// Handles the ClientSessionConnected event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TClientSessionEventArgs"/> instance containing the event data.</param>
        void rtspServer_ClientSessionConnected(object sender, TClientSessionEventArgs e)
        {
            AddClientInfo(e.SessionInfo);
            AddServerInfo(string.Format("Client [{0}:{1}] was connected.", 
                e.SessionInfo.ClientSessionIP, e.SessionInfo.ClientSessionPort.ToString()));
        }

        /// <summary>
        /// Handles the ClientSessionRejected event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void rtspServer_ClientSessionRejected(object sender, EventArgs e)
        {
            AddServerInfo("The server has reach the maximum client session count.");
        }

        /// <summary>
        /// Handles the ExceptionOccurred event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TExceptionEventArgs"/> instance containing the event data.</param>
        void rtspServer_ExceptionOccurred(object sender, TExceptionEventArgs e)
        {
            AddServerInfo("An exception occurred: " + e.ExceptionMessage);
        }

        /// <summary>
        /// Handles the ServerStopped event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void rtspServer_ServerStopped(object sender, EventArgs e)
        {
            this.lvClientInfo.Items.Clear();
            AddServerInfo("Server has been stopped successfully!");
        }

        /// <summary>
        /// Handles the ServerStarted event of the rtspServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void rtspServer_ServerStarted(object sender, EventArgs e)
        {
            AddServerInfo("Server has been started successfully!");
        }
        #endregion

        #region Private Methods
        #region Add/Update Server Information/Status
        /// <summary>
        /// Adds the server info.
        /// </summary>
        /// <param name="info">The info.</param>
        private void AddServerInfo(string info)
        {
            this.lstServerInfo.BeginUpdate();

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(DateTime.Now.ToString());
            sb.Append("] ");
            sb.Append(info);

            this.lstServerInfo.Items.Insert(0, sb.ToString());

            if (this.lstServerInfo.Items.Count > 1000)
            {
                this.lstServerInfo.Items.Clear();
            }

            this.lstServerInfo.EndUpdate();
        }

        /// <summary>
        /// Updates the server status.
        /// </summary>
        private void UpdateServerStatus()
        {
            if (isServerRunning)
            {
                this.btnStartServer.Enabled = false;
                this.btnStopServer.Enabled = true;

                this.lblServerState.Text = "Server is running...";
            }
            else
            {
                this.btnStartServer.Enabled = true;
                this.btnStopServer.Enabled = false;

                this.lblServerState.Text = "Server is stopped...";
            }
        }
        #endregion

        #region Add/Update/Remove Client Information
        /// <summary>
        /// Adds the client info.
        /// </summary>
        /// <param name="clientSessionInfo">The client session info.</param>
        private void AddClientInfo(ClientSessionInfo clientSessionInfo)
        {
            this.lvClientInfo.BeginUpdate();

            ListViewItem lvi = new ListViewItem(clientSessionInfo.ClientSessionId.ToString());
            lvi.SubItems.Add(clientSessionInfo.ClientSessionIP);
            lvi.SubItems.Add(clientSessionInfo.ClientSessionPort.ToString());
            lvi.SubItems.Add(clientSessionInfo.SessionConnectTime.ToString());
            lvi.SubItems.Add(clientSessionInfo.SessionState);
            lvi.SubItems.Add(clientSessionInfo.StreamName);

            this.lvClientInfo.Items.Add(lvi);

            this.lvClientInfo.EndUpdate();
        }

        /// <summary>
        /// Updates the client info.
        /// </summary>
        /// <param name="clientSessionInfo">The client session info.</param>
        private void UpdateClientInfo(ClientSessionInfo clientSessionInfo)
        {
            this.lvClientInfo.BeginUpdate();

            string sessionId = clientSessionInfo.ClientSessionId.ToString();
            ListViewItem lvi = this.lvClientInfo.FindItemWithText(sessionId);
            if (lvi != null && lvi.SubItems.Count > 0)
            {
                lvi.SubItems[1].Text = clientSessionInfo.ClientSessionIP;
                lvi.SubItems[2].Text = clientSessionInfo.ClientSessionPort.ToString();
                lvi.SubItems[3].Text = clientSessionInfo.SessionConnectTime.ToString();
                lvi.SubItems[4].Text = clientSessionInfo.SessionState;
                lvi.SubItems[5].Text = clientSessionInfo.StreamName;
            }

            this.lvClientInfo.EndUpdate();
        }

        /// <summary>
        /// Removes the client info.
        /// </summary>
        /// <param name="clientSessionInfo">The client session info.</param>
        private void RemoveClientInfo(ClientSessionInfo clientSessionInfo)
        {
            this.lvClientInfo.BeginUpdate();

            string sessionId = clientSessionInfo.ClientSessionId.ToString();
            ListViewItem lvi = this.lvClientInfo.FindItemWithText(sessionId);
            if (lvi != null)
            {
                lvi.Remove();
            }

            this.lvClientInfo.EndUpdate();
        }
        #endregion

        /// <summary>
        /// Hides the server.
        /// </summary>
        private void HideServer()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.notifyIcon1.ShowBalloonTip(2000, "Sharp Streaming Server", "The server has already minimize to tray, you can revert it with left clicking.", ToolTipIcon.Info);
        }
        #endregion
        
        #region Form/Controls Event Handler
        /// <summary>
        /// Handles the Load event of the ServerForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ServerForm_Load(object sender, EventArgs e)
        {
            Initialize();

            Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "The sharp streaming server has been loaded.");
        }

        /// <summary>
        /// Handles the FormClosing event of the ServerForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isQuitNow && isMinToTray)
            {
                HideServer();
                e.Cancel = true;
                return;
            }

            DialogResult dlgResult = MessageBox.Show("Do you really want to quit the server?", "Quit Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dlgResult == DialogResult.Yes)
            {
                // Stop the rtsp server if it is still running.
                if (rtspServer != null && isServerRunning)
                {
                    Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Try to stop the server.");

                    bool result = rtspServer.StopServer();
                    if (result)
                    {
                        Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "Stop the server successfully.");
                    }
                }

                Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "The sharp streaming server has been closed.");
            }
            else
            {
                isQuitNow = false;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lblLocalTime.Text = DateTime.Now.ToString();
        }

        /// <summary>
        /// Handles the Click event of the btnStartServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            StartServer();
        }

        /// <summary>
        /// Handles the Click event of the btnStopServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnStopServer_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        /// <summary>
        /// Handles the Click event of the btnHideServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnHideServer_Click(object sender, EventArgs e)
        {
            HideServer();
        }

        /// <summary>
        /// Handles the Click event of the btnSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm frmSettings = new SettingsForm();

            DialogResult dlgResult = frmSettings.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                // Initializes again with config file.
                InitFromConfigFile();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the btnQuit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            isQuitNow = true;
            Close();
        }

        /// <summary>
        /// Handles the MouseClick event of the notifyIcon1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }
        #endregion
        
    }
}
