using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A main class for the rtsp client.
    /// </summary>
    public class RtspClient
    {
        #region Private Member Fields
        private static int rtspSeqNum = 0;
        
        private Socket socket = null;
        private MediaSession mediaSession;
        private FilePermission filePermission;

        private double seekTime;

        private int clientRtpPort;
        private int clientRtcpPort;
        private int serverRtpPort;
        private int serverRtcpPort;

        private string clientAddress;
        private string serverAddress;

        private string sessionId;
        private string requestUrl;
        private string response;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public double Duration { get; protected set; }
        #endregion

        #region Class Events
        public EventHandler<TExceptionEventArgs> ExceptionOccurred;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RtspClient"/> class.
        /// </summary>
        public RtspClient()
        {
            this.filePermission = new FilePermission();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Connects the server.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns>Succeeded or failed.</returns>
        public bool ConnectServer(string ip, int port)
        {
            if (socket != null && socket.Connected)
            {
                // It has connected to the server, so we make it disconnected
                // because we need to setup a new connection.
                DisconnectServer();
            }

            bool result = false;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPAddress.Parse(ip), port);

                result = true;
            }
            catch (SocketException se)
            {
                this.OnExceptionOccurred(se);
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
            }

            return result;
        }

        /// <summary>
        /// Disconnects the server.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        public bool DisconnectServer()
        {
            if (socket == null)
            {
                return true;
            }
            else
            {
                if (!socket.Connected)
                {
                    return true;
                }
            }

            bool result = false;

            try
            {
                CloseStream();

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                result = true;
            }
            catch (SocketException se)
            {
                this.OnExceptionOccurred(se);
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
            }

            return result;
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Succeeded or failed.</returns>
        public bool OpenStream(string url, string filePath)
        {
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            // Sets the request url:
            requestUrl = url;

            // Sends "OPTIONS" command and then gets the response:
            bool result = SendOptionsCmd();
            if (!result)
            {
                CloseStream();
                return false;
            }

            // Sends "DESCRIBE" command and then gets the SDP description:
            string sdpDescription = SendDescribeCmd();
            if (string.IsNullOrEmpty(sdpDescription))
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Sdp description is null or empty.");
                CloseStream();
                return false;
            }

            // Creates a media session object from the SDP description which
            // we have just received from the server:
            mediaSession = new MediaSession(sdpDescription);

            // Then, resolves the SDP description and initializes all basic
            // information:
            result = mediaSession.ResolveSdpDescription();
            if (!result)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Resolve sdp description failed.");
                CloseStream();
                return false;
            }

            long fileSize = mediaSession.FileSize;
            Duration = mediaSession.PlayEndTime - mediaSession.PlayStartTime;

            // And then, creates the output file to write the data:
            FileSink fileSink = CreateOutputFile(filePath, fileSize);
            if (fileSink == null)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Create output file failed.");
                CloseStream();
                return false;
            }

            // After that, sends the "SETUP" command and setups the stream:
            result = SendSetupCmd();
            if (!result)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Send setup command failed.");
                CloseStream();
                return false;
            }

            result = mediaSession.CreateRtpSource(fileSink, serverAddress,
                serverRtpPort, serverRtcpPort, clientRtpPort, clientRtcpPort);
            if (!result)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Create the rtp source failed.");
                CloseStream();
                return false;
            }

            // Start the receiving work in asynchronous mode:
            mediaSession.Source.StartPlaying();

            // Finally, sends the "PLAY" command and starts playing stream:
            result = PlayStream();
            if (!result)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtspClient -- OpenStream", "Play the stream failed.");
                CloseStream();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Seeks the stream.
        /// </summary>
        /// <param name="seekTime">The seek time.</param>
        /// <returns>Succeeded or failed.</returns>
        public bool SeekStream(double seekTime)
        {
            this.seekTime = seekTime;

            bool result = PlayStream();

            return result;
        }

        /// <summary>
        /// Plays the stream.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        public bool PlayStream()
        {
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            if (Duration < 0)
            {
                Duration = 0;
            }
            else if (Duration == 0 || Duration > mediaSession.PlayEndTime)
            {
                Duration = mediaSession.PlayEndTime - seekTime;
            }

            double startTime = seekTime;
            double endTime = seekTime + Duration;

            string range;
            if (startTime < 0)
            {
                range = string.Empty;
            }
            else if (endTime < 0)
            {
                range = string.Format("Range: npt={0}-", startTime.ToString());
            }
            else
            {
                range = string.Format("Range: npt={0}-{1}", startTime.ToString(), endTime.ToString());
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_PLAY);    // command name of 'PLAY'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("Session: {0}\r\n", sessionId);    // session id
            sb.AppendFormat("{0}\r\n", range);  // range, 'Range: npt='
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header

            bool isSucceeded = SendRtspRequest(sb.ToString());
            if (!isSucceeded)
            {
                CloseStream();
                return false;
            }

            bool isOk = GetRtspResponse();
            if (!isOk)
            {
                CloseStream();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Pauses the stream.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        public bool PauseStream()
        {
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_PAUSE);    // command name of 'PAUSE'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("Session: {0}\r\n", sessionId);    // session id
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header

            bool isSucceeded = SendRtspRequest(sb.ToString());
            if (!isSucceeded)
            {
                CloseStream();
                return false;
            }

            bool isOk = GetRtspResponse();
            if (!isOk)
            {
                CloseStream();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tear downs the stream.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        public bool TeardownStream()
        {
            if (socket == null || !socket.Connected)
            {
                CloseStream();
                return true;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_TEARDOWN);    // command name of 'TEARDOWN'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("Session: {0}\r\n", sessionId);    // session id
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header
                        
            bool isSucceeded = SendRtspRequest(sb.ToString());
            if (!isSucceeded)
            {
                CloseStream();
                return false;
            }

            bool isOk = GetRtspResponse();
            if (!isOk)
            {
                CloseStream();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        public bool CloseStream()
        {
            mediaSession.Source.CloseFileSink();

            return true;
        }

        /// <summary>
        /// Determines whether [is continue playing] with [the specified position].
        /// </summary>
        /// <param name="position">The position wants to be judge.</param>
        /// <returns>
        /// 	<c>true</c> if [is continue playing] with [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContinuePlaying(double position)
        {
            // TODO: Add some code here

            return true;
        }
        #endregion

        #region Private Methods
        #region Send RTSP Request And Get RTSP Response Methods
        /// <summary>
        /// Sends the RTSP request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Success or failed.</returns>
        private bool SendRtspRequest(string request)
        {
            if (socket == null)
            {
                return false;
            }

            try
            {
                byte[] sendBuffer = Utils.StringToBytes(request);
                int sendBytesCount = socket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                if (sendBytesCount < 1)
                {
                    return false;
                }

                return true;
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
                return false;
            }
        }

        /// <summary>
        /// Gets the RTSP response.
        /// </summary>
        /// <returns>Success or failed.</returns>
        private bool GetRtspResponse()
        {
            bool isOk = false;
            int revBytesCount = 0;
            byte[] revBuffer = new byte[1024 * 4]; // 4 KB buffer
            response = string.Empty;

            // Set the timeout for synchronous receive methods to 
            // 5 seconds (5000 milliseconds.)
            socket.ReceiveTimeout = 5000;

            while (true)
            {
                try
                {
                    revBytesCount = socket.Receive(revBuffer, revBuffer.Length, SocketFlags.None);
                    if (revBytesCount >= 1)
                    {
                        // Here, we have received the data from the server successfully, so we break the loop.
                        break;
                    }
                    else if (socket.ReceiveTimeout >= 5000)
                    {
                        break;
                    }
                }
                catch (SocketException se)
                {
                    // Receive data exception, may be it has come to the ReceiveTimeout or other exception.
                    this.OnExceptionOccurred(se);
                    break;
                }
            }

            // Just looking for the RTSP status code:
            if (revBytesCount >= 1)
            {
                response = Utils.BytesToString(revBuffer, revBytesCount);

                Utils.OutputMessage(false, MsgLevel.Debug, string.Empty, response);

                if (response.StartsWith(Constants.RTSP_HEADER_VERSION))
                {
                    string[] dstArray = response.Split(' ');  // Separate by a blank
                    if (dstArray.Length > 1)
                    {
                        string code = dstArray[1];
                        if (code.Equals(Constants.RTSP_STATUS_CODE_OK)) // RTSP/1.0 200 OK ...
                        {
                            isOk = true;
                        }
                    }
                }
            }

            return isOk;
        }
        #endregion

        /// <summary>
        /// Sends the options CMD.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        private bool SendOptionsCmd()
        {
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_OPTIONS);    // command name of 'OPTIONS'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header

            bool isSucceeded = SendRtspRequest(sb.ToString());
            if (!isSucceeded)
            {
                return false;
            }

            bool isOk = GetRtspResponse();
            if (!isOk)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends the describe CMD.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        private string SendDescribeCmd()
        {
            if (socket == null || !socket.Connected)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_DESCRIBE);    // command name of 'DESCRIBE'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header

            bool isSucceeded = SendRtspRequest(sb.ToString());
            if (!isSucceeded)
            {
                return string.Empty;
            }

            bool isOk = GetRtspResponse();
            if (!isOk)
            {
                return string.Empty;
            }

            // Finds if there has a start flag of sdp description:
            int startOfSdp = response.IndexOf("v=");
            if (startOfSdp == -1)
            {
                return string.Empty;
            }

            string sdpDescription = response.Substring(startOfSdp);

            return sdpDescription;
        }

        /// <summary>
        /// Sends the setup CMD.
        /// </summary>
        /// <returns>Succeeded or failed.</returns>
        private bool SendSetupCmd()
        {
            if (socket == null || !socket.Connected)
            {
                return false;
            }

            bool result = false;
            clientRtpPort = Utils.GenerateRandomNumber(8000, 9000);
            clientRtcpPort = clientRtpPort + 1;
            sessionId = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", Constants.RTSP_CMD_SETUP);    // command name of 'SETUP'
            sb.AppendFormat("{0} RTSP/1.0\r\n", requestUrl);   // request url
            sb.AppendFormat("CSeq: {0}\r\n", (++rtspSeqNum).ToString()); // sequence number
            sb.AppendFormat("Transport: client_port={0}-{1};\r\n", clientRtpPort.ToString(), clientRtcpPort.ToString()); // transport string
            sb.AppendFormat("Session: {0}\r\n", sessionId);   // session id
            sb.AppendFormat("User-Agent: {0}\r\n\r\n", Constants.USER_AGENT_HEADER);    // user agent header

            result = SendRtspRequest(sb.ToString());
            if (!result)
            {
                return false;
            }

            result = GetRtspResponse();
            if (!result)
            {
                return false;
            }

            result = RtspCommon.ParseServerTransportHeader(response,
                out clientAddress, out serverAddress, 
                out serverRtpPort, out serverRtcpPort);
            if (!result)
            {
                return false;
            }

            sessionId = RtspCommon.ParseSessionHeader(response);

            return true;
        }

        /// <summary>
        /// Creates the output file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns>The FileSink object.</returns>
        private FileSink CreateOutputFile(string filePath, long fileSize)
        {
            FileSink fileSink = null;

            fileSink = new FileSink(filePath, fileSize);
            fileSink.UpdateAccessRange += new EventHandler<TFilePermissionEventArgs>(this.OnUpdateAccessRange);

            return fileSink;
        }

        /// <summary>
        /// Updates the access range.
        /// </summary>
        /// <param name="begin">The begin.</param>
        /// <param name="end">The end.</param>
        private void UpdateAccessRange(long begin, long end)
        {
            // TODO: Add some code here
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [update access range].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TFilePermissionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdateAccessRange(object sender, TFilePermissionEventArgs e)
        {
            UpdateAccessRange(e.BeginOffset, e.EndOffset);
        }

        /// <summary>
        /// Called when [exception occurred].
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected virtual void OnExceptionOccurred(Exception ex)
        {
            EventHandler<TExceptionEventArgs> handler = this.ExceptionOccurred;
            if (handler != null)
            {
                TExceptionEventArgs e = new TExceptionEventArgs(ex);
                handler(this, e);
            }
        }
        #endregion
    }
}
