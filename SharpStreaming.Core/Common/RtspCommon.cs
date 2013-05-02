using System.Text.RegularExpressions;

namespace Simon.SharpStreaming.Core
{
    public static class RtspCommon
    {
        /// <summary>
        /// Parses the request string.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <param name="cmdName">Name of the CMD.</param>
        /// <param name="urlPreSuffix">The URL pre suffix.</param>
        /// <param name="urlSuffix">The URL suffix.</param>
        /// <param name="cseq">The cseq.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParseRequestString(string srcString,
            out string cmdName, out string rtspUrl,
            out string urlPreSuffix, out string urlSuffix, 
            out string cseq)
        {
            cmdName = string.Empty;
            rtspUrl = string.Empty;
            urlPreSuffix = string.Empty;
            urlSuffix = string.Empty;
            cseq = string.Empty;

            string[] resultArray = null;
            // Splits the source string to multiline:
            string[] lineArray = Regex.Split(srcString, @"[\r\n]+");
            if (lineArray.Length < 2)
            {
                return false;
            }

            // Splits the first line:
            resultArray = Regex.Split(lineArray[0], " ");
            if (resultArray.Length < 2)
            {
                return false;
            }

            // Looks for the rtsp command name like the following: 
            // OPTIONS, DESCRIBE, SETUP, PLAY, PAUSE, TEARDOWN
            cmdName = resultArray[0].Trim();

            // Looks for the URL suffix and URL pre-suffix:
            rtspUrl = resultArray[1];
            int startIndex = rtspUrl.LastIndexOf('/');
            urlSuffix = rtspUrl.Substring(startIndex + 1);
            urlPreSuffix = urlSuffix;

            // Looks for "CSeq:" in the second line:
            if (lineArray[1].Contains("CSeq:"))
            {
                cseq = lineArray[1].Substring(("CSeq:").Length).Trim();
            }

            return true;
        }

        /// <summary>
        /// Parses the transport string.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <returns>The transport string array.</returns>
        public static string[] ParseTransportString(string srcString)
        {
            string[] transportArray = null;

            // Splits the source string to multiline:
            string[] lineArray = Regex.Split(srcString, @"[\r\n]+");

            string temp = string.Empty;

            // Finds if there has a "Transport" header:
            foreach (string s in lineArray)
            {
                if (s.Contains("Transport:"))
                {
                    temp = s;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(temp))
            {
                // Splits the transport string:
                temp = temp.Substring(("Transport: ").Length);
                transportArray = Regex.Split(temp, ";");
            }

            return transportArray;
        }

        /// <summary>
        /// Parses the port number.
        /// </summary>
        /// <param name="srcArray">The source array.</param>
        /// <param name="subString">The sub string.</param>
        /// <param name="rtpPort">The RTP port.</param>
        /// <param name="rtcpPort">The RTCP port.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParsePortNumber(string[] srcArray, string subString, out int rtpPort, out int rtcpPort)
        {
            rtpPort = 0;
            rtcpPort = 0;

            string temp = string.Empty;
            foreach (string s in srcArray)
            {
                if (s.Contains(subString))
                {
                    temp = s;
                    break;
                }
            }

            // No substring header? Then return false directly.
            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            temp = temp.Substring(subString.Length).Trim();

            string rtpPortStr = string.Empty;
            string rtcpPortStr = string.Empty;
            string[] rangeArray = Regex.Split(temp, "-");
            if (rangeArray.Length == 1)
            {
                rtpPortStr = rangeArray[0].Trim();
            }
            else if (rangeArray.Length == 2)
            {
                rtpPortStr = rangeArray[0].Trim();
                rtcpPortStr = rangeArray[1].Trim();
            }

            rtpPort = Utils.StringToInteger(rtpPortStr);
            rtcpPort = Utils.StringToInteger(rtcpPortStr);

            return true;
        }

        /// <summary>
        /// Parses the client transport header on server side.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <param name="clientRtpPort">The client RTP port.</param>
        /// <param name="clientRtcpPort">The client RTCP port.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParseClientTransportHeader(string srcString, 
            out int clientRtpPort, out int clientRtcpPort)
        {
            clientRtpPort = 0;
            clientRtcpPort = 0;

            string[] transportArray = ParseTransportString(srcString);
            if (transportArray == null)
            {
                return false;
            }

            // Finds if there has a "client_port" header:
            bool result = ParsePortNumber(transportArray, "client_port=", out clientRtpPort, out clientRtcpPort);
            if (!result)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the server transport header on client side.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <param name="clientAddress">The client address.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="serverRtpPort">The server RTP port.</param>
        /// <param name="serverRtcpPort">The server RTCP port.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParseServerTransportHeader(string srcString,
            out string clientAddress, out string serverAddress,
            out int serverRtpPort, out int serverRtcpPort)
        {
            clientAddress = string.Empty;
            serverAddress = string.Empty;
            serverRtpPort = 0;
            serverRtcpPort = 0;

            string[] transportArray = ParseTransportString(srcString);
            if (transportArray == null)
            {
                return false;
            }

            // Finds if there has a "destination" header and a "source" header:
            // Note: On the client side, the "destination" means the "clientAddress",
            // and the "source" means the "serverAddress".
            foreach (string s in transportArray)
            {
                if (s.Contains("destination="))
                {
                    clientAddress = s.Substring(("destination=").Length).Trim();
                }
                else if (s.Contains("source="))
                {
                    serverAddress = s.Substring(("source=").Length).Trim();
                }
            }

            // Could not find the "destination" or the "source":
            if (string.IsNullOrEmpty(clientAddress) ||
                string.IsNullOrEmpty(serverAddress))
            {
                return false;
            }

            // Finds if there has a "server_port" header:
            bool result = ParsePortNumber(transportArray, "server_port=", out serverRtpPort, out serverRtcpPort);
            if (!result)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the session header.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <returns>The session.</returns>
        public static string ParseSessionHeader(string srcString)
        {
            string session = string.Empty;

            // Splits the source string to multiline:
            string[] lineArray = Regex.Split(srcString, @"[\r\n]+");

            foreach (string s in lineArray)
            {
                if (s.Contains("Session:"))
                {
                    session = s.Substring(("Session:").Length).Trim();
                }
            }

            return session;
        }

        /// <summary>
        /// Parses the scale header.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParseScaleHeader(string srcString, out float scale)
        {
            scale = 0.0f;

            // Splits the source string to multiline:
            string[] lineArray = Regex.Split(srcString, @"[\r\n]+");

            string temp = string.Empty;

            // Finds if there has a "Scale" header:
            foreach (string s in lineArray)
            {
                if (s.Contains("Scale: "))
                {
                    temp = s;
                    break;
                }
            }

            // No "Scale" header? Then return false directly.
            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            if (temp.Length > ("Scale: ").Length)
            {
                temp = temp.Substring(("Scale: ").Length).Trim();
            }

            scale = Utils.StringToFloat(temp);
            
            return true;
        }

        /// <summary>
        /// Parses the range header.
        /// </summary>
        /// <param name="srcString">The source string.</param>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        /// <returns>Succeeded or failed.</returns>
        public static bool ParseRangeHeader(string srcString, 
            out double rangeStart, out double rangeEnd)
        {
            rangeStart = 0.0;
            rangeEnd = 0.0;

            // Splits the source string to multiline:
            string[] lineArray = Regex.Split(srcString, @"[\r\n]+");

            string temp = string.Empty;

            // Finds if there has a "Range" header:
            foreach (string s in lineArray)
            {
                if (s.Contains("Range: npt="))
                {
                    temp = s;
                    break;
                }
            }

            // No "Range" header? Then return false directly.
            if (string.IsNullOrEmpty(temp))
            {
                return false;
            }

            temp = temp.Substring(("Range: npt=").Length);

            string rangeStartStr = string.Empty;
            string rangeEndStr = string.Empty;
            string[] rangeArray = Regex.Split(temp, "-");
            if (rangeArray.Length == 1)
            {
                rangeStartStr = rangeArray[0].Trim();
            }
            else if (rangeArray.Length == 2)
            {
                rangeStartStr = rangeArray[0].Trim();
                rangeEndStr = rangeArray[1].Trim();
            }

            rangeStart = Utils.StringToDouble(rangeStartStr);
            rangeEnd = Utils.StringToDouble(rangeEndStr);

            return true;
        }
    }
}
