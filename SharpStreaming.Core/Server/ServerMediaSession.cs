using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Simon.SharpStreaming.Core
{
    public class ServerMediaSession
    {
        #region Private Member Fields
        /// <summary>
        /// The media source object.
        /// </summary>
        private MediaSource mediaSource;

        /// <summary>
        /// The creation time of the server media session.
        /// </summary>
        private DateTime creationTime;

        private string hostAddress;
        private string fileName;
        private string sdpLines;
        private string trackId;

        private int trackNumber;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the track id.
        /// </summary>
        /// <value>The track id.</value>
        public string TrackId
        {
            get
            {
                if (this.trackNumber == 0)
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(this.trackId))
                {
                    this.trackId = string.Format("track{0}", this.trackNumber);
                }

                return this.trackId;
            }
        }

        /// <summary>
        /// Gets or sets the file catalog.
        /// </summary>
        /// <value>The file catalog.</value>
        public string FileCatalog { get; set; }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMediaSession"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public ServerMediaSession(string fileName)
        {
            this.trackNumber = 1;

            this.hostAddress = Utils.GetLocalAddresses()[0];

            this.fileName = fileName;

            this.creationTime = DateTime.Now;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generates the SDP description.
        /// </summary>
        /// <returns>The SDP description.</returns>
        public string GenerateSdpDescription()
        {
            string sdp = string.Empty;
            string descriptionSdp = "Audio & Video, streamed by the SharpStreamingServer";
            string infoSdp = fileName;
            string ourIPAddress = hostAddress;

            int ourIPAddressLength = ourIPAddress.Length;
            int sdpDescriptionLength = 0;

            //string rangeLine = GetSdpRangeLine();
            string sdpLine = GetSdpLines();
            sdpDescriptionLength += sdpLine.Length;

            // Generates the SDP prefix (session-level lines):
            StringBuilder sb = new StringBuilder();
            sb.Append("v=0\r\n"); // protocol version
            sb.AppendFormat("o=- {0}{1} {2} IN IP4 {3}\r\n",
                creationTime.Second.ToString(), creationTime.Millisecond.ToString(), 1, ourIPAddress); // o= <session id><version><address>  owner/creator and session identifier
            sb.AppendFormat("s={0}\r\n", descriptionSdp); // s= <description>
            sb.AppendFormat("i={0}\r\n", infoSdp); // i= <information>
            sb.Append("t=0 0\r\n"); // time the session is active
            //sb.AppendFormat("{0}", rangeLine); // a=range: line
            sb.AppendFormat("{0}", sdpLine); // sdp line, add here??

            sdp = sb.ToString();

            return sdp;
        }

        /// <summary>
        /// Gets the stream parameters.
        /// </summary>
        /// <param name="clientSessionId">The client session id(unused at present).</param>
        /// <param name="clientAddr">The client address.</param>
        /// <param name="clientRtpPort">The client RTP port.</param>
        /// <param name="clientRtcpPort">The client RTCP port.</param>
        /// <param name="serverRtpPort">The server RTP port.</param>
        /// <param name="serverRtcpPort">The server RTCP port.</param>
        /// <param name="streamToken">The stream token.</param>
        /// <returns>Succeeded or failed.</returns>
        public bool GetStreamParameters(int clientSessionId, string clientAddr,
            int clientRtpPort, int clientRtcpPort, int serverRtpPort, int serverRtcpPort,
            ref StreamState streamToken)
        {
            // Creates a new media source:
            MediaSource inputSource = CreateStreamSource();
            if (inputSource == null)
            {
                return false;
            }

            // Creates rtp socket and rtcp socket for the client:
            ClientSocketBase clientRtpSocket = null;
            ClientSocketBase clientRtcpSocket = null;
            Socket socket = null;
            socket = Utils.CreateUdpSocket(serverRtpPort);
            if (socket == null)
            {
                return false;
            }

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(clientAddr), clientRtpPort);
            clientRtpSocket = new ClientSocketUdp(socket, (EndPoint)iep);

            socket = Utils.CreateUdpSocket(serverRtcpPort);
            if (socket != null)
            {
                iep = new IPEndPoint(IPAddress.Parse(clientAddr), clientRtcpPort);
                clientRtcpSocket = new ClientSocketUdp(socket, (EndPoint)iep);
            }

            // Creates a sink for this stream:
            RtpSink rtpSink = CreateRtpSink(inputSource, clientRtpSocket);
            if (rtpSink == null)
            {
                return false;
            }

            // Setups the state of the stream. The stream will get started later:
            streamToken = new StreamState(rtpSink, inputSource, clientRtcpSocket);
            if (streamToken == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Public Virtual Methods
        /// <summary>
        /// Starts the stream.
        /// </summary>
        /// <param name="streamState">State of the stream.</param>
        /// <param name="rtpSeqNum">The RTP seq num.</param>
        /// <param name="rtpTimestamp">The RTP timestamp.</param>
        public virtual void StartStream(StreamState streamState,
            ref int rtpSeqNum, ref int rtpTimestamp)
        {
            if (streamState != null)
            {
                streamState.StartPlaying();

                if (streamState.Sink != null)
                {
                    rtpSeqNum = streamState.Sink.RtpSeqNum;
                    rtpTimestamp = streamState.Sink.RtpTimestamp;
                }
            }
        }

        /// <summary>
        /// Pauses the stream.
        /// </summary>
        /// <param name="streamState">State of the stream.</param>
        public virtual void PauseStream(StreamState streamState)
        {
            if (streamState != null)
            {
                streamState.PausePlaying();
            }
        }

        /// <summary>
        /// Seeks the stream.
        /// </summary>
        /// <param name="streamState">State of the stream.</param>
        /// <param name="seekNPT">The seek NPT.</param>
        public virtual void SeekStream(StreamState streamState, double seekNPT)
        {
            if (streamState != null && streamState.Source != null)
            {
                SeekStreamSource(streamState.Source, seekNPT);
            }
        }

        /// <summary>
        /// Deletes the stream.
        /// </summary>
        /// <param name="streamState">State of the stream.</param>
        public virtual void DeleteStream(StreamState streamState)
        {
            if (streamState != null)
            {
                streamState.EndPlaying();
            }
        }

        /// <summary>
        /// Seeks the stream source.
        /// </summary>
        /// <param name="inputSource">The input source.</param>
        /// <param name="seekNPT">The seek NPT(Normal Play Time).</param>
        public virtual void SeekStreamSource(MediaSource inputSource, double seekNPT)
        {
            // No implementation.
        }

        /// <summary>
        /// Creates the stream source.
        /// </summary>
        /// <returns>The object of MediaSource.</returns>
        public virtual MediaSource CreateStreamSource()
        {
            return null;
        }

        /// <summary>
        /// Creates the RTP sink.
        /// </summary>
        /// <param name="inputSource">The input source.</param>
        /// <param name="clientRtpSocket">The client RTP socket.</param>
        /// <returns>The RtpSink object.</returns>
        public virtual RtpSink CreateRtpSink(MediaSource inputSource, ClientSocketBase clientRtpSocket)
        {
            return null;
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <returns>The type of the media.</returns>
        public virtual string GetMediaType()
        {
            return "Video";
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns>The duration.</returns>
        public virtual double GetDuration()
        {
            return 0;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <returns>The file size.</returns>
        public virtual long GetFileSize()
        {
            return 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the SDP lines.
        /// </summary>
        /// <returns>The sdp lines string.</returns>
        private string GetSdpLines()
        {
            if (string.IsNullOrEmpty(sdpLines))
            {
                MediaSource inputSource = CreateStreamSource();
                if (inputSource == null)    // file not found
                {
                    return string.Empty;
                }
                mediaSource = inputSource;

                string mediaType = GetMediaType();
                int portNumForSdp = 0;
                int rtpPayloadType = 0;
                if (rtpPayloadType == 0)
                {
                    rtpPayloadType = 96 + trackNumber - 1; //if dynamic
                }
                string ipAddress = hostAddress;
                string rtpmapLine = string.Empty;
                string rangeLine = GetSdpRangeLine();
                string sizeLine = GetSdpSizeLine();
                string auxSdpLine = string.Empty;

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("m={0} {1} RTP/AVP {2}\r\n",
                    mediaType, portNumForSdp.ToString(), rtpPayloadType.ToString()); // m=<media><port><fmt list>
                sb.AppendFormat("c=IN IP4 {0}\r\n", ipAddress); // c=address
                sb.AppendFormat("{0}", rtpmapLine); // a=rtpmap:... (if present)
                sb.AppendFormat("{0}", rangeLine); // a=range:... (if present)
                sb.AppendFormat("{0}", sizeLine); // a=size:... (if present) Note: New added.
                sb.AppendFormat("{0}", auxSdpLine); // optional extra SDP line
                sb.AppendFormat("a=control:{0}\r\n", TrackId); // a=control:<track-id>

                sdpLines = sb.ToString();
            }

            return sdpLines;
        }

        /// <summary>
        /// Gets the range line for SDP.
        /// </summary>
        /// <returns>The range line for SDP.</returns>
        private string GetSdpRangeLine()
        {
            string rangeSdpLine = string.Empty;

            double duration = GetDuration();
            if (duration == 0.0)
            {
                rangeSdpLine = "a=range:npt=0-\r\n";
            }
            else
            {
                rangeSdpLine = string.Format("a=range:npt=0-{0}\r\n", duration.ToString());
            }

            return rangeSdpLine;
        }

        /// <summary>
        /// Gets the size line for SDP.
        /// </summary>
        /// <returns>The size line for SDP.</returns>
        private string GetSdpSizeLine()
        {
            string sizeSdpLine = string.Empty;

            long fileSize = GetFileSize();

            sizeSdpLine = string.Format("a=size:fs={0}\r\n", fileSize.ToString());

            return sizeSdpLine;
        }
        #endregion
    }
}
