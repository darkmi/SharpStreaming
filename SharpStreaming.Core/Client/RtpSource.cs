using System;

namespace Simon.SharpStreaming.Core
{
    public class RtpSource
    {
        #region Private Member Fields
        private FileSink fileSink;
        private ClientSocketBase clientRtpSocket;
        private RtpPacket rtpPacket;

        private int preferredFrameSize;
        private int internalPacketSize;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RtpSource"/> class.
        /// </summary>
        /// <param name="fileSink">The file sink.</param>
        /// <param name="clientRtpSocket">The client RTP socket.</param>
        public RtpSource(FileSink fileSink, ClientSocketBase clientRtpSocket)
        {
            this.fileSink = fileSink;
            this.clientRtpSocket = clientRtpSocket;

            this.clientRtpSocket.DatagramReceived += new EventHandler<TSocketEventArgs>(this.OnDatagramReceived);

            rtpPacket = new RtpPacket();

            preferredFrameSize = 1024;
            internalPacketSize = preferredFrameSize + 16;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the playing.
        /// </summary>
        public void StartPlaying()
        {
            if (clientRtpSocket != null)
            {
                clientRtpSocket.ReceiveDatagram();
            }
        }

        /// <summary>
        /// Closes the file sink.
        /// </summary>
        public void CloseFileSink()
        {
            if (fileSink != null)
            {
                fileSink.CloseFileStream();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the incoming packet.
        /// </summary>
        /// <param name="packetBuffer">The packet buffer.</param>
        /// <param name="packetSize">Size of the packet.</param>
        private void HandleIncomingPacket(byte[] packetBuffer, int packetSize)
        {
            long position = 0;
            int frameSize = 0;
            byte[] frameBuffer = new byte[preferredFrameSize];

            int dataSize = 0;
            byte[] dataBuffer = new byte[internalPacketSize];

            // First, Parses the RTP packet:
            if (rtpPacket != null)
            {
                rtpPacket.ParseRtpPacket(packetBuffer, packetSize, ref dataBuffer, ref dataSize);
            }

            // Then, Parses the internal packet:  
            /* The internal packet has the following format:
             * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
             * [positionLength][position][dataLengthLength][dataLength][data...]
             * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
             */
            try
            {
                // Gets and converts the position length
                int positionLength = (int)dataBuffer[0];

                // Gets and converts the position
                byte[] bytesPosition = new byte[positionLength];
                Array.Copy(dataBuffer, 1, bytesPosition, 0, positionLength);
                position = Utils.BytesToLong(bytesPosition);

                // Gets and converts the frame size length
                int frameSizeLength = (int)dataBuffer[1 + positionLength];

                // Gets and converts the frame size
                byte[] bytesFrameSize = new byte[frameSizeLength];
                Array.Copy(dataBuffer, 1 + positionLength + 1, bytesFrameSize, 0, frameSizeLength);
                frameSize = Utils.BytesToInteger(bytesFrameSize);

                // Gets the frame buffer
                Array.Copy(dataBuffer, 1 + positionLength + 1 + frameSizeLength, frameBuffer, 0, frameSize);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "RtpSource -- HandleIncomingPacket", e.Message);
            }

            // Finally, adds the frame to the file:
            if (fileSink != null)
            {
                fileSink.WriteDataToFile(frameBuffer, frameSize, position);
            }
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [datagram received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreaming.Core.TSocketEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDatagramReceived(object sender, TSocketEventArgs e)
        {
            HandleIncomingPacket(e.RecvBuffer, e.RecvBufferSize);
        }
        #endregion
    }
}
