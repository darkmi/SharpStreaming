using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// The client session which inherits the ClientSessionInfo.
    /// </summary>
    public class ClientSession : ClientSessionInfo
    {
        #region Private Member Fields
        /// <summary>
        /// The object of RtspServer.
        /// </summary>
        private RtspServer rtspServer;

        /// <summary>
        /// The object of ClientSocketBase (TCP socket)
        /// </summary>
        private ClientSocketBase clientSocket;

        /// <summary>
        /// The object of ServerMediaSession.
        /// </summary>
        private ServerMediaSession serverMediaSession;

        /// <summary>
        /// The object of StreamState.
        /// </summary>
        private StreamState streamState;

        /// <summary>
        /// RTSP response to the client.
        /// </summary>
        private string response;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the state of the current session.
        /// </summary>
        /// <value>The state of the current session.</value>
        public ClientSessionState CurrentSessionState { get; protected set; }
        #endregion

        #region Public Class Events
        public event EventHandler<TExceptionEventArgs> ExceptionOccurred;
        public event EventHandler<TClientSessionEventArgs> ClientSessionUpdated;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSession"/> class.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="socket">The socket.</param>
        /// <param name="rtspServer">The RtspServer object.</param>
        public ClientSession(int sessionId, Socket socket, RtspServer rtspServer)
        {
            base.ClientSessionId = sessionId;
            base.SessionConnectTime = DateTime.Now;
            base.SessionLastTime = base.SessionConnectTime;
            this.CurrentSessionState = ClientSessionState.Active;

            this.rtspServer = rtspServer;

            if (socket != null)
            {
                IPEndPoint iep = socket.RemoteEndPoint as IPEndPoint;
                if (iep != null)
                {
                    base.ClientSessionIP = iep.Address.ToString();
                    base.ClientSessionPort = iep.Port;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Begins to receive the client request data with asynchronous mode.
        /// </summary>
        /// <param name="socket">The socket of the TCP.</param>
        public void ReceiveClientRequest(Socket socket)
        {
            ClientSocketTcp clientSocketTcp = new ClientSocketTcp(socket);
            clientSocket = clientSocketTcp;

            clientSocket.ClientTeardown += new EventHandler(this.OnClientTeardown);
            clientSocket.DatagramReceived += new EventHandler<TSocketEventArgs>(this.OnDatagramReceived);
            clientSocket.ExceptionOccurred += new EventHandler<TExceptionEventArgs>(this.OnExceptionOccurred);

            clientSocket.ReceiveDatagram();
        }

        /// <summary>
        /// Checks the client session is timeout or not.
        /// </summary>
        /// <param name="maxSessionTimeout">maximum client session timeout milliseconds</param>
        /// <returns>timeout or not</returns>
        public bool CheckTimeout(int maxSessionTimeout)
        {
            bool isTimeout = false;
            TimeSpan ts = DateTime.Now.Subtract(SessionLastTime);
            int elapsedSeconds = Math.Abs((int)ts.TotalSeconds);

            if (elapsedSeconds > maxSessionTimeout)
            {
                CurrentSessionState = ClientSessionState.Inactive;
                isTimeout = true;
            }

            return isTimeout;
        }

        /// <summary>
        /// Closes the client session.
        /// </summary>
        public void Close()
        {
            // First, ends the current playing if possible.
            if (serverMediaSession != null && streamState != null)
            {
                serverMediaSession.DeleteStream(streamState);
            }

            // Then, closes the clientSocket.
            if (clientSocket != null)
            {
                clientSocket.CloseClientSocket();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the incoming request.
        /// </summary>
        /// <param name="recvBuffer">The receive buffer.</param>
        /// <param name="recvBufferSize">Size of the receive buffer.</param>
        private void HandleIncomingRequest(byte[] recvBuffer, int recvBufferSize)
        {
            string cmdName;
            string rtspUrl;
            string urlPreSuffix;
            string urlSuffix;
            string cseq;

            string request = Utils.BytesToString(recvBuffer, recvBufferSize);

            Utils.OutputMessage(false, MsgLevel.Debug, string.Empty, request);

            // Parses the request string into command name and 'CSeq', then handle the command.
            bool isSucceeded = RtspCommon.ParseRequestString(
                request, out cmdName, out rtspUrl, out urlPreSuffix, out urlSuffix, out cseq);

            if (isSucceeded)
            {
                switch (cmdName)
                {
                    case Constants.RTSP_CMD_OPTIONS:
                        HandleOptionsCmd(cseq);
                        break;
                    case Constants.RTSP_CMD_DESCRIBE:
                        HandleDescribeCmd(cseq, rtspUrl, urlSuffix, request);
                        break;
                    case Constants.RTSP_CMD_SETUP:
                        HandleSetupCmd(cseq, urlPreSuffix, urlSuffix, request);
                        break;
                    case Constants.RTSP_CMD_PLAY:
                        HandlePlayCmd(cseq, rtspUrl, request);
                        break;
                    case Constants.RTSP_CMD_PAUSE:
                        HandlePauseCmd(cseq);
                        break;
                    case Constants.RTSP_CMD_TEARDOWN:
                        HandleTearDownCmd(cseq);
                        break;
                    default:
                        HandleNotSupportCmd(cseq);
                        break;
                }

                SessionState = cmdName;
                this.OnClientSessionUpdated(this);
            }
            else
            {
                // Parses request string failed!
                HandleBadCmd();
            }

            // After we handle the client request, we must send the response to the client.
            // *****
            //Converts string to bytes.
            byte[] sendBuffer = Utils.StringToBytes(response);

            // Sends the rtsp response to the client.
            clientSocket.SendDatagram(sendBuffer, sendBuffer.Length);
        }

        #region Handle All Kinds Of Commands
        /// <summary>
        /// Handles the options CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandleOptionsCmd(string cseq)
        {
            response = string.Format("RTSP/1.0 200 OK\r\nCSeq: {0}\r\nDate: {1}\r\nPublic: {2}\r\n\r\n",
                cseq, DateTime.Now.ToLocalTime(), Constants.RTSP_ALLOW_COMMAND);
        }

        /// <summary>
        /// Handles the describe CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        /// <param name="rtspUrl">The rtsp url.</param>
        /// <param name="urlSuffix">The URL suffix.</param>
        /// <param name="request">The request string.</param>
        private void HandleDescribeCmd(string cseq, string rtspUrl, string urlSuffix, string request)
        {
            base.StreamName = urlSuffix;

            // Begins by looking up the "ServerMediaSession" object for the specified "urlSuffix":
            serverMediaSession = rtspServer.LookupServerMediaSession(urlSuffix);
            if (serverMediaSession == null)
            {
                HandleNotFoundCmd(cseq);
                return;
            }

            // Then, assembles a SDP description for this session:
            string sdpDescription = serverMediaSession.GenerateSdpDescription();
            if (string.IsNullOrEmpty(sdpDescription))
            {
                HandleNotFoundCmd(cseq);
                return;
            }

            int sdpDescriptionSize = sdpDescription.Length;

            // Generates the response information now.
            StringBuilder sb = new StringBuilder();
            sb.Append("RTSP/1.0 200 OK\r\n");
            sb.AppendFormat("CSeq: {0}\r\n", cseq);
            sb.AppendFormat("Date: {0}\r\n", DateTime.Now.ToLocalTime());
            sb.AppendFormat("Content-Base: {0}\r\n", rtspUrl);
            sb.Append("Content-Type: application/sdp\r\n");
            sb.AppendFormat("Content-Length: {0}\r\n\r\n", sdpDescriptionSize);
            sb.AppendFormat("{0}", sdpDescription);

            response = sb.ToString();
        }

        /// <summary>
        /// Handles the setup CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        /// <param name="urlPreSuffix">The URL pre suffix.</param>
        /// <param name="urlSuffix">The URL suffix.</param>
        /// <param name="request">The request string.</param>
        private void HandleSetupCmd(string cseq, string urlPreSuffix, string urlSuffix, string request)
        {
            // Note: At present, I will use only one streaming mode of RTP_UDP 
            // to transfer the data between server and client.

            // First, judges that the server media session exists or not.
            if (serverMediaSession == null)
            {
                serverMediaSession = rtspServer.LookupServerMediaSession(urlPreSuffix);
                if (serverMediaSession == null)
                {
                    HandleNotFoundCmd(cseq);
                    return;
                }
            }

            // Looks for a "Transport:" header in the request string,
            // to extract client parameters:
            int clientRtpPort;
            int clientRtcpPort;

            bool result = RtspCommon.ParseClientTransportHeader(request,
                out clientRtpPort, out clientRtcpPort);
            if (!result)
            {
                HandleUnsupportedTransportCmd(cseq);
                return;
            }

            // Then, gets the server parameters from the server media session:
            streamState = null;
            string destinationAddr = base.ClientSessionIP;
            string sourceAddr = Utils.GetLocalAddresses()[0];
            int serverRtpPort = rtspServer.GenerateRtpPortNumber();
            int serverRtcpPort = serverRtpPort + 1;

            result = serverMediaSession.GetStreamParameters(base.ClientSessionId,
                base.ClientSessionIP, clientRtpPort, clientRtcpPort,
                serverRtpPort, serverRtcpPort, ref streamState);
            if (!result)
            {
                HandleUnsupportedTransportCmd(cseq);
                return;
            }

            // Generates the response information now.
            StringBuilder sb = new StringBuilder();
            sb.Append("RTSP/1.0 200 OK\r\n");
            sb.AppendFormat("CSeq: {0}\r\n", cseq);
            sb.AppendFormat("Date: {0}\r\n", DateTime.Now.ToLocalTime());
            sb.Append("Transport: RTP/AVP;unicast;");
            sb.AppendFormat("destination={0};source={1};", destinationAddr, sourceAddr);
            sb.AppendFormat("client_port={0}-{1};", clientRtpPort, clientRtcpPort);
            sb.AppendFormat("server_port={0}-{1}\r\n", serverRtpPort, serverRtcpPort);
            sb.AppendFormat("Session: {0}\r\n\r\n", base.ClientSessionId);

            response = sb.ToString();
        }

        /// <summary>
        /// Handles the play CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        /// <param name="rtspUrl">The rtsp url.</param>
        /// <param name="request">The request string.</param>
        private void HandlePlayCmd(string cseq, string rtspUrl, string request)
        {
            if (serverMediaSession == null || streamState == null)
            {
                return;
            }

            // Parses the client's "Scale:" header:
            string scaleHeader = string.Empty;
            float scale;
            bool hasScaleHeader = RtspCommon.ParseScaleHeader(request, out scale);
            if (hasScaleHeader)
            {
                scaleHeader = string.Format("Scale: {0}\r\n", scale);
            }

            // Parses the client's "Range:" header:
            string rangeHeader = string.Empty;
            double rangeStart = 0.0;
            double rangeEnd = 0.0;
            bool hasRangeHeader = RtspCommon.ParseRangeHeader(request, out rangeStart, out rangeEnd);

            double duration = this.serverMediaSession.GetDuration();

            if (rangeEnd <= 0.0 || rangeEnd > duration)
            {
                rangeEnd = duration;
            }

            if (rangeStart < 0.0)
            {
                rangeStart = 0.0;
            }
            else if (rangeEnd > 0.0 && scale > 0.0 && rangeStart > rangeEnd)
            {
                rangeStart = rangeEnd;
            }

            if (hasRangeHeader)
            {
                if (rangeStart == 0.0 && scale >= 0.0)
                {
                    rangeHeader = string.Format("Range: npt={0}-\r\n", rangeStart);
                }
                else
                {
                    rangeHeader = string.Format("Range: npt={0}-{1}\r\n", rangeStart, rangeEnd);
                }
            }
 
            // Handles any require of scaling on session before starting streaming:
            if (hasScaleHeader)
            {
                // No implementation at present.
            }

            // Handles any require of seeking on session before starting streaming:
            if (hasRangeHeader)
            {
                serverMediaSession.SeekStream(streamState, rangeStart);
            }

            // Now, starts the stream:
            int rtpSeqNum = 0;
            int rtpTimestamp = 0;
            serverMediaSession.StartStream(streamState, ref rtpSeqNum, ref rtpTimestamp);

            string rtpInfo = string.Format("RTP-Info: url={0}/{1};seq={2};rtptime={3}\r\n",
                rtspUrl, serverMediaSession.TrackId, rtpSeqNum, rtpTimestamp);

            // Generates the response information now.
            StringBuilder sb = new StringBuilder();
            sb.Append("RTSP/1.0 200 OK\r\n");
            sb.AppendFormat("CSeq: {0}\r\n", cseq);
            sb.AppendFormat("Date: {0}\r\n", DateTime.Now.ToLocalTime());
            sb.AppendFormat("{0}\r\n{1}\r\n", scaleHeader, rangeHeader);
            sb.AppendFormat("Session: {0}\r\n{1}\r\n", base.ClientSessionId, rtpInfo);

            response = sb.ToString();
        }

        /// <summary>
        /// Handles the pause CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandlePauseCmd(string cseq)
        {
            if (serverMediaSession == null || streamState == null)
            {
                return;
            }

            serverMediaSession.PauseStream(streamState);

            response = string.Format("RTSP/1.0 200 OK\r\nCSeq: {0}\r\nDate: {1}\r\nSession: {2}\r\n\r\n",
                cseq, DateTime.Now.ToLocalTime(), base.ClientSessionId);
        }

        /// <summary>
        /// Handles the tear down CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandleTearDownCmd(string cseq)
        {
            if (serverMediaSession == null || streamState == null)
            {
                return;
            }

            response = string.Format("RTSP/1.0 200 OK\r\nCSeq: {0}\r\nDate: {1}\r\n",
                cseq, DateTime.Now.ToLocalTime());

            serverMediaSession.DeleteStream(streamState);

            BeginTeardown();
        }

        /// <summary>
        /// Handles the bad CMD.
        /// </summary>
        private void HandleBadCmd()
        {
            response = string.Format("RTSP/1.0 400 Bad Request\r\nDate: {0}\r\nAllow: {1}\r\n\r\n",
                DateTime.Now.ToLocalTime(), Constants.RTSP_ALLOW_COMMAND);

            BeginTeardown();
        }

        /// <summary>
        /// Handles the not support CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandleNotSupportCmd(string cseq)
        {
            response = string.Format("RTSP/1.0 405 Method Not Allowed\r\nCSeq: {0}\r\nDate: {1}\r\nAllow: {2}\r\n\r\n",
                cseq, DateTime.Now.ToLocalTime(), Constants.RTSP_ALLOW_COMMAND);

            BeginTeardown();
        }

        /// <summary>
        /// Handles the not found CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandleNotFoundCmd(string cseq)
        {
            response = string.Format("RTSP/1.0 404 Stream Not Found\r\nCSeq: {0}\r\nDate: {1}\r\n",
                cseq, DateTime.Now.ToLocalTime());

            BeginTeardown();
        }

        /// <summary>
        /// Handles the unsupported transport CMD.
        /// </summary>
        /// <param name="cseq">The cseq.</param>
        private void HandleUnsupportedTransportCmd(string cseq)
        {
            response = string.Format("RTSP/1.0 461 Unsupported Transport\r\nCSeq: {0}\r\nDate: {1}\r\n",
                cseq, DateTime.Now.ToLocalTime());

            BeginTeardown();
        }
        #endregion

        /// <summary>
        /// Begins tear down the client.
        /// </summary>
        private void BeginTeardown()
        {
            CurrentSessionState = ClientSessionState.Inactive;
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [client tear down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnClientTeardown(object sender, EventArgs e)
        {
            BeginTeardown();
        }

        /// <summary>
        /// Called when [datagram received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TSocketEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDatagramReceived(object sender, TSocketEventArgs e)
        {
            SessionLastTime = DateTime.Now;
            HandleIncomingRequest(e.RecvBuffer, e.RecvBufferSize);
        }

        /// <summary>
        /// Called when [exception occurred].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExceptionOccurred(object sender, TExceptionEventArgs e)
        {
            EventHandler<TExceptionEventArgs> handler = this.ExceptionOccurred;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [client session updated].
        /// </summary>
        /// <param name="clientSessionInfo">The client session info.</param>
        protected virtual void OnClientSessionUpdated(ClientSessionInfo clientSessionInfo)
        {
            EventHandler<TClientSessionEventArgs> handler = this.ClientSessionUpdated;
            if (handler != null)
            {
                TClientSessionEventArgs e = new TClientSessionEventArgs(clientSessionInfo);
                handler(this, e);
            }
        }
        #endregion
    }
}
