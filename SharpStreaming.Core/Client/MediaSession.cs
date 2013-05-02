using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Simon.SharpStreaming.Core
{
    public class MediaSession
    {
        private string sdpDescription;

        #region Public Properties
        /// <summary>
        /// Gets or sets the RTP source.
        /// </summary>
        /// <value>The RTP source.</value>
        public RtpSource Source { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        public long FileSize { get; protected set; }

        /// <summary>
        /// Gets or sets the play start time.
        /// </summary>
        /// <value>The play start time.</value>
        public double PlayStartTime { get; protected set; }

        /// <summary>
        /// Gets or sets the play end time.
        /// </summary>
        /// <value>The play end time.</value>
        public double PlayEndTime { get; protected set; }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSession"/> class.
        /// </summary>
        /// <param name="sdpDescription">The SDP description.</param>
        public MediaSession(string sdpDescription)
        {
            this.sdpDescription = sdpDescription;
        }
        #endregion

        /// <summary>
        /// Resolves the SDP description.
        /// </summary>
        /// <returns>Success or failed.</returns>
        public bool ResolveSdpDescription()
        {
            if (string.IsNullOrEmpty(sdpDescription))
            {
                return false;
            }

            // Splits the SDP description string to multiline:
            string[] sdpArray = Regex.Split(sdpDescription, @"[\r\n]+");

            bool result = ParseSdpRangeAttribute(sdpArray);
            if (!result)
            {
                return false;
            }

            result = ParseSdpSizeAttribute(sdpArray);
            if (!result)
            {
                return false;
            }

            result = ParseSdpControlAttribute(sdpArray);
            if (!result)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the RTP source.
        /// </summary>
        /// <param name="fileSink">The file sink.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="serverRtpPort">The server RTP port.</param>
        /// <param name="serverRtcpPort">The server RTCP port.</param>
        /// <param name="clientRtpPort">The client RTP port.</param>
        /// <param name="clientRtcpPort">The client RTCP port.</param>
        /// <returns>Succeeded or failed.</returns>
        public bool CreateRtpSource(FileSink fileSink, string serverAddress, 
            int serverRtpPort, int serverRtcpPort,
            int clientRtpPort, int clientRtcpPort)
        {
            // Creates rtp socket and rtcp socket:
            ClientSocketBase clientRtpSocket = null;
            ClientSocketBase clientRtcpSocket = null;
            Socket socket = null;
            socket = Utils.CreateUdpSocket(clientRtpPort);
            if (socket == null)
            {
                return false;
            }

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(serverAddress), serverRtpPort);
            clientRtpSocket = new ClientSocketUdp(socket, (EndPoint)iep);

            socket = Utils.CreateUdpSocket(clientRtcpPort);
            if (socket != null)
            {
                iep = new IPEndPoint(IPAddress.Parse(serverAddress), serverRtcpPort);
                clientRtcpSocket = new ClientSocketUdp(socket, (EndPoint)iep);
            }

            Source = new RtpSource(fileSink, clientRtpSocket);
            if (Source == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the SDP range attribute.
        /// </summary>
        /// <param name="sdpArray">The SDP array.</param>
        /// <returns>Succeeded or failed.</returns>
        private bool ParseSdpRangeAttribute(string[] sdpArray)
        {
            string temp = string.Empty;

            foreach (string s in sdpArray)
            {
                if (s.Contains("a=range:npt="))
                {
                    temp = s;
                    break;
                }
            }

            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            temp = temp.Substring(("a=range:npt=").Length);

            string rangeStartStr = string.Empty;
            string rangeEndStr = string.Empty;
            string[] rangeArray = Regex.Split(temp, "-");
            if (rangeArray.Length < 2)
            {
                return false;
            }
            else if (rangeArray.Length == 2)
            {
                rangeStartStr = rangeArray[0].Trim();
                rangeEndStr = rangeArray[1].Trim();
            }

            PlayStartTime = Utils.StringToDouble(rangeStartStr);
            PlayEndTime = Utils.StringToDouble(rangeEndStr);

            return true;
        }

        /// <summary>
        /// Parses the SDP size attribute.
        /// </summary>
        /// <param name="sdpArray">The SDP array.</param>
        /// <returns>Succeeded or failed.</returns>
        private bool ParseSdpSizeAttribute(string[] sdpArray)
        {
            string temp = string.Empty;

            foreach (string s in sdpArray)
            {
                if (s.Contains("a=size:fs="))
                {
                    temp = s;
                    break;
                }
            }

            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            string fileSize = temp.Substring(("a=size:fs=").Length).Trim();

            FileSize = Utils.StringToLong(fileSize);

            return true;
        }

        /// <summary>
        /// Parses the SDP control attribute.
        /// </summary>
        /// <param name="sdpArray">The SDP array.</param>
        /// <returns>Succeeded or failed.</returns>
        private bool ParseSdpControlAttribute(string[] sdpArray)
        {
            string temp = string.Empty;

            foreach (string s in sdpArray)
            {
                if (s.Contains("a=control:"))
                {
                    temp = s;
                    break;
                }
            }

            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            string trackId = temp.Substring(("a=control:").Length).Trim();

            return true;
        }
    }
}
