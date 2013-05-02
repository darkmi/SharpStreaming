using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using LogThis;
using Microsoft.DirectX.AudioVideoPlayback;
using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingClient
{
    public partial class ClientForm : Form
    {
        #region Private Member Fields
        private RtspClient rtspClient = null;
        private Video ourVideo = null;

        private bool isLocalFile;
        private bool isStreaming;

        private string filePath;
        private string fileNameOrUrl;
        private string tempDirectory;

        private double duration;

        private PlayerState currentState = PlayerState.Ready;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientForm"/> class.
        /// </summary>
        public ClientForm()
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
            // Initializes the client log.
            string logLocation = Utils.CreateDirectory("Log");
            string logFileName = "ClientLogFile";
            Log.UseSensibleDefaults(logFileName, logLocation, eloglevel.info);

            // If there is a "Temporary Play Files" directory, may be it has
            // one or more files, so tries to delete the whole directory 
            // included the sub directories and files.
            // Note: Uses this way in order to clean up all temporary play files.
            tempDirectory = "Temporary Play Files";
            if (Directory.Exists(tempDirectory))
            {
                try
                {
                    Directory.Delete(tempDirectory, true);
                }
                catch
                {
                    // Ignores the exceptions.
                }
            }

            // Creates the "Temporary Play Files"'s directory 
            // in the directory where the current player exists.
            tempDirectory = Utils.CreateDirectory(tempDirectory);

            UpdateClientStatus();
        }
        #endregion

        #region Client Control Methods
        /// <summary>
        /// Opens the local file or the stream.
        /// </summary>
        private void Open()
        {
            // Tries to stop the player, especially in the case of the
            // player is in streaming mode, and at the same time we open
            // an another file or url.
            Stop();

            if (isLocalFile)
            {
                OpenLocalFile(fileNameOrUrl);
                isStreaming = false;
            }
            else
            {
                bool result;
                string fileName;
                string hostAddress;
                int hostPort;

                result = Utils.ParseRequestUrl(fileNameOrUrl, out hostAddress, out hostPort, out fileName);
                if (!result)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- Open", "Parse request url failed!");
                    return;
                }

                filePath = Utils.AddBackSlash(tempDirectory) + fileName;

                currentState = PlayerState.Buffering;
                UpdateClientStatus();

                result = OpenStream(hostAddress, hostPort);
                if (!result)
                {
                    currentState = PlayerState.Ready;
                    UpdateClientStatus();
                    return;
                }

                // Sets the current playing in streaming mode.
                isStreaming = true;
                UpdateClientStatus();
            }
        }

        /// <summary>
        /// Opens the local file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void OpenLocalFile(string fileName)
        {
            try
            {
                CleanUp();

                ourVideo = new Video(fileName);
                //ourVideo.Ending += new EventHandler(ourVideo_Ending);
                ourVideo.Owner = picPlayWindow;

                // Start playing now:
                ourVideo.Play();

                if (isLocalFile || duration == 0)
                {
                    duration = ourVideo.Duration;
                }
                
                this.currentState = PlayerState.Playing;
                this.lblDuration.Text = Utils.ConvertTimeToString(duration);
                this.timer1.Enabled = true;

                UpdateClientStatus();
            }
            catch(System.Exception e)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                MessageBox.Show(string.Format("Could not open the file: {0}.\r\nPlease try again or make sure that you have installed the decoder.", fileInfo.Name), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- OpenLocalFile", string.Format("Could not open the file: {0}", fileInfo.Name), e.Message);
            }
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <param name="hostAddress">The host address.</param>
        /// <param name="hostPort">The host port.</param>
        /// <returns>Succeeded or failed.</returns>
        private bool OpenStream(string hostAddress, int hostPort)
        {
            if (rtspClient == null)
            {
                rtspClient = new RtspClient();

                HookClientEvents();
            }

            this.lblCurrentState.Text = "Try to connect the server...";
            this.Refresh();

            bool result = rtspClient.ConnectServer(hostAddress, hostPort);
            if (!result)
            {
                MessageBox.Show(string.Format("Could not connect the server[{0}:{1}]!", hostAddress, hostPort.ToString()), "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- OpenStream", "Could not connect the server!");
                
                return false;
            }

            result = rtspClient.OpenStream(fileNameOrUrl, filePath);
            if (!result)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- OpenStream", "Could not open the stream!");

                rtspClient.DisconnectServer();

                return false;
            }

            duration = rtspClient.Duration;

            Utils.OutputMessage(false, MsgLevel.Debug, "ClientForm -- OpenStream", "Open the stream successfully.");

            return true;
        }

        /// <summary>
        /// Plays playing.
        /// </summary>
        private void Play()
        {
            if (ourVideo != null)
            {
                ourVideo.Play();
                currentState = PlayerState.Playing;
            }

            UpdateClientStatus();
        }

        /// <summary>
        /// Pauses playing the media file.
        /// </summary>
        private void Pause()
        {
            if (ourVideo != null)
            {
                ourVideo.Pause();
                currentState = PlayerState.Paused;
            }

            UpdateClientStatus();
        }

        /// <summary>
        /// Stops playing the media file.
        /// </summary>
        private void Stop()
        {
            if (isStreaming)
            {
                if (rtspClient != null)
                {
                    bool result = rtspClient.TeardownStream();
                    if (!result)
                    {
                        Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- Stop", "Could not stop the stream!");
                        return;
                    }

                    result = rtspClient.DisconnectServer();
                    if (!result)
                    {
                        Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- Stop", "Could not disconnect the server!");
                        return;
                    }

                    Utils.OutputMessage(false, MsgLevel.Debug, "ClientForm -- Stop", "Stop the stream successfully.");
                }
            }

            if (ourVideo != null)
            {
                ourVideo.Stop();
            }

            currentState = PlayerState.Stopped;
            UpdateClientStatus();
        }

        /// <summary>
        /// Handles the Ending event of the ourVideo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ourVideo_Ending(object sender, EventArgs e)
        {
            if (ourVideo != null)
            {
                ourVideo.Stop();
            }

            currentState = PlayerState.Stopped;
            UpdateClientStatus();
        }

        /// <summary>
        /// Determines whether [is playing completed] with [the specified position].
        /// </summary>
        /// <param name="position">The current position or the seek position.</param>
        /// <returns>
        /// 	<c>true</c> if [is playing completed] with [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPlayingCompleted(double position)
        {
            double temp = duration - position;

            // If "position" is not far from the end,
            // We could see this playing media is already in the end.
            if (temp < Constants.TIMEGAP_STOPALWAY)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        private void CleanUp()
        {
            if (ourVideo != null)
            {
                ourVideo.Dispose();
                ourVideo = null;
            }

            isLocalFile = false;
            isStreaming = false;

            this.timer1.Enabled = false;
            this.colorSlider1.Value = 0;
            this.lblCurrentPos.Text = "00:00:00";
            this.lblDuration.Text = "00:00:00";
            this.lblCurrentState.Text = "Ready";
        }

        #endregion

        #region Events Handler
        /// <summary>
        /// Hooks the client events.
        /// </summary>
        private void HookClientEvents()
        {
            rtspClient.ExceptionOccurred += new EventHandler<TExceptionEventArgs>(rtspClient_ExceptionOccurred);
        }

        /// <summary>
        /// Handles the ExceptionOccurred event of the rtspClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TExceptionEventArgs"/> instance containing the event data.</param>
        void rtspClient_ExceptionOccurred(object sender, TExceptionEventArgs e)
        {
            Utils.OutputMessage(false, MsgLevel.Error, "ClientForm -- rtspClientExceptionOccurred", e.ExceptionMessage);
        }
        #endregion

        #region Update Client Status
        /// <summary>
        /// Updates the client status.
        /// </summary>
        private void UpdateClientStatus()
        {
            switch (this.currentState)
            {
                case PlayerState.Stopped:
                    this.btnOpen.Enabled = true;
                    this.btnPlay.Enabled = true;
                    this.btnPause.Enabled = false;
                    this.btnStop.Enabled = false;
                    this.btnFullScreen.Enabled = false;
                    this.lblCurrentState.Text = "Stopped";
                    this.lblCurrentPos.Text = "00:00:00";
                    this.colorSlider1.Value = 0;
                    this.colorSlider1.Enabled = true;
                    break;

                case PlayerState.Paused:
                    this.btnOpen.Enabled = true;
                    this.btnPlay.Enabled = true;
                    this.btnPause.Enabled = false;
                    this.btnStop.Enabled = true;
                    this.btnFullScreen.Enabled = true;
                    this.lblCurrentState.Text = "Paused";
                    this.colorSlider1.Enabled = true;
                    break;

                case PlayerState.Playing:
                    this.btnOpen.Enabled = true;
                    this.btnPlay.Enabled = false;
                    this.btnPause.Enabled = true;
                    this.btnStop.Enabled = true;
                    this.btnFullScreen.Enabled = true;
                    this.lblCurrentState.Text = "Playing";
                    this.colorSlider1.Enabled = true;
                    break;

                case PlayerState.Buffering:
                    this.btnOpen.Enabled = true;
                    this.btnPlay.Enabled = false;
                    this.btnPause.Enabled = false;
                    this.btnStop.Enabled = true;
                    this.btnFullScreen.Enabled = false;
                    this.lblCurrentState.Text = "Buffering, please wait...";
                    this.colorSlider1.Enabled = true;
                    break;

                case PlayerState.Ready:
                default:
                    this.btnOpen.Enabled = true;
                    this.btnPlay.Enabled = false;
                    this.btnPause.Enabled = false;
                    this.btnStop.Enabled = false;
                    this.btnFullScreen.Enabled = false;
                    this.lblCurrentState.Text = "Ready";
                    this.lblCurrentPos.Text = "00:00:00";
                    this.colorSlider1.Value = 0;
                    this.colorSlider1.Enabled = false;
                    break;
            }
        }
        #endregion

        #region Form/Controls Event Handler
        /// <summary>
        /// Handles the Load event of the ClientForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ClientForm_Load(object sender, EventArgs e)
        {
            Initialize();

            Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "The sharp streaming client has been loaded.");
        }

        /// <summary>
        /// Handles the FormClosing event of the ClientForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tries to stop the player before closes the window.
            Stop();

            CleanUp();

            Utils.OutputMessage(false, MsgLevel.Info, string.Empty, "The sharp streaming client has been closed.");
        }

        /// <summary>
        /// Handles the Tick event of the timer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            double currentPos = 0;

            switch (currentState)
            {
                case PlayerState.Stopped:
                case PlayerState.Paused:
                case PlayerState.Playing:
                    if (ourVideo != null)
                    {
                        currentPos = ourVideo.CurrentPosition;
                    }
                    
                    if (IsPlayingCompleted(currentPos))
                    {
                        Stop();
                    }
                    break;

                case PlayerState.Buffering:
                    break;

                default:
                    currentPos = 0;
                    break;
            }

            this.colorSlider1.Value = (int)((currentPos / duration) * this.colorSlider1.Maximum);
            this.lblCurrentPos.Text = Utils.ConvertTimeToString(currentPos);
        }

        /// <summary>
        /// Handles the Scroll event of the colorSlider1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ScrollEventArgs"/> instance containing the event data.</param>
        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            double seekPosition = 0;

            // Gets the seek position value:
            seekPosition = ((double)this.colorSlider1.Value / this.colorSlider1.Maximum) * duration;

            if (IsPlayingCompleted(seekPosition))
            {
                Stop();
            }

            if (isLocalFile)
            {
                // Set the current position directly:
                if (ourVideo != null)
                {
                    ourVideo.CurrentPosition = seekPosition;
                }
            }
            else
            {
                if (rtspClient != null)
                {
                    // Judges this position can be play or not:
                    bool result = rtspClient.IsContinuePlaying(seekPosition);
                    if (result)
                    {
                        // Set the current position directly:
                        if (ourVideo != null)
                        {
                            ourVideo.CurrentPosition = seekPosition;
                        }
                    }
                    else
                    {
                        // Pauses the current playing:
                        Pause();

                        // Seeks the stream and wait for playing:
                        result = rtspClient.SeekStream(seekPosition);
                        if (!result)
                        {
                            Stop();
                        }

                        // Sets the current position which will be shown to the user.
                        this.colorSlider1.Value = (int)((seekPosition / duration) * this.colorSlider1.Maximum);
                        this.lblCurrentPos.Text = Utils.ConvertTimeToString(seekPosition);

                        // Sets the current state to the play buffering:
                        currentState = PlayerState.Buffering;
                        UpdateClientStatus();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenForm frmOpen = new OpenForm();
            DialogResult dlgResult = frmOpen.ShowDialog();

            if (dlgResult == DialogResult.OK)
            {
                isLocalFile = frmOpen.IsLocalFile;
                fileNameOrUrl = frmOpen.FileNameOrUrl;

                this.Refresh();

                Open();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnPlay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play();
        }

        /// <summary>
        /// Handles the Click event of the btnPause control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Handles the Click event of the btnFullScreen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            if (ourVideo != null)
            {
                ourVideo.Fullscreen = true;
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
        #endregion
    }
}
