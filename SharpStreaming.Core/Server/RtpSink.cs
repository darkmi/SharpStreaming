using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Simon.SharpStreaming.Core
{
    public class RtpSink
    {
        #region Private Member Fields
        /// <summary>
        /// A background worker means we do the task on a single thread.
        /// </summary>
        private BackgroundWorker backgroundWorker;

        private ClientSocketBase clientRtpSocket;
        private MediaSource mediaSource;
        private RtpPacket rtpPacket;

        private bool isCurrentlyPlaying;

        private int rateControlCounter = 0;
        private int maxCountInLowSpeed = 500;
        private int sendControlCounter = 0;
        private int maxCountToSendPerTime = 5;

        private int preferredPacketSize = 1448;

        private int rtpSeqNum;
        private int rtpTimestamp;
        private uint rtpSSRC;
        private string rtpCNAME;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the RTP seq number.
        /// </summary>
        /// <value>The RTP seq number.</value>
        public int RtpSeqNum
        {
            get { return this.rtpSeqNum; }
        }

        /// <summary>
        /// Gets the RTP timestamp.
        /// </summary>
        /// <value>The RTP timestamp.</value>
        public int RtpTimestamp
        {
            get { return this.rtpTimestamp; }
        }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RtpSink"/> class.
        /// </summary>
        /// <param name="payloadType">Type of the payload.</param>
        /// <param name="inputSource">The input source.</param>
        /// <param name="clientRtpSocket">The client RTP socket.</param>
        public RtpSink(int payloadType, MediaSource inputSource, ClientSocketBase clientRtpSocket)
        {
            this.clientRtpSocket = clientRtpSocket;
            this.mediaSource = inputSource;

            this.mediaSource.MaxFrameSize = 1360;
            this.mediaSource.PreferredFrameSize = 1024;

            InitBackgroundWorker();
            InitRtpPacket(payloadType);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the playing.
        /// </summary>
        public void StartPlaying()
        {
            if (backgroundWorker != null)
            {
                isCurrentlyPlaying = true;
                backgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Stops the playing.
        /// </summary>
        public void StopPlaying()
        {
            if (backgroundWorker != null)
            {
                isCurrentlyPlaying = false;
                backgroundWorker.CancelAsync();
            }
        }

        /// <summary>
        /// Ends the playing.
        /// </summary>
        public void EndPlaying()
        {
            if (backgroundWorker != null)
            {
                isCurrentlyPlaying = false;
                backgroundWorker.CancelAsync();
                backgroundWorker.Dispose();
                mediaSource.CloseMediaSource();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the background worker.
        /// </summary>
        private void InitBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
        }

        /// <summary>
        /// Initializes the RtpPacket instance.
        /// </summary>
        private void InitRtpPacket(int payloadType)
        {
            rtpPacket = new RtpPacket(payloadType);

            rtpTimestamp = 1;
            rtpSeqNum = RtpUtils.GenerateSeqNumber();
            rtpSSRC = RtpUtils.GenerateSSRC();
            rtpCNAME = RtpUtils.GenerateCNAME();
        }

        /// <summary>
        /// Handles the DoWork event of the backgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isCurrentlyPlaying)
            {
                /* Because we use UDP to send the packets, and it is an unreliable
                 * transport mode, so we send the packets at low speed in the
                 * begin, and after that, the speed will be more faster.
                 */
                if (rateControlCounter > maxCountInLowSpeed)
                {
                    /* In order to avoids the high using of the CPU,
                     * sleeps the current thread when it sends greater
                     * than "maxCountToSendPerTime".
                     * Note: "maxCountToSendPerTime" must not be set too large,
                     * or else, there will result in a high packet lost rate.
                     */
                    if (sendControlCounter > maxCountToSendPerTime)
                    {
                        Thread.Sleep(1);
                        sendControlCounter = 0;
                    }

                    Interlocked.Increment(ref sendControlCounter);
                }
                else
                {
                    Thread.Sleep(1);
                }

                BuildAndSendPacket();
            }
        }

        /// <summary>
        /// Builds the packet and then sends it.
        /// </summary>
        private void BuildAndSendPacket()
        {
            long position = 0;
            int frameSize = 0;

            if (mediaSource.PreferredFrameSize > mediaSource.MaxFrameSize)
            {
                mediaSource.PreferredFrameSize = mediaSource.MaxFrameSize;
            }
            byte[] frameBuffer = new byte[mediaSource.PreferredFrameSize];

            int packetSize = 0;
            byte[] packetBuffer = new byte[preferredPacketSize];

            if (mediaSource != null)
            {
                mediaSource.GetNextFrame(ref frameBuffer, ref frameSize, ref position);
            }

            if (frameSize > 0)
            {
                // First, builds the internal packet:
                /* The internal packet has the following format:
                 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 * [positionLength][position][dataLengthLength][dataLength][data...]
                 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */
                byte[] bytesPosition = Utils.LongToBytes(position);
                byte[] bytesPositionLength = new byte[1] { (byte)bytesPosition.Length };
                byte[] bytesFrameSize = Utils.LongToBytes((long)frameSize);
                byte[] bytesFrameSizeLength = new byte[1] { (byte)bytesFrameSize.Length };

                int prefixLength = bytesPosition.Length + bytesPositionLength.Length
                    + bytesFrameSize.Length + bytesFrameSizeLength.Length;

                byte[] dataBuffer = new byte[prefixLength + frameSize];

                try
                {
                    // Copies the position length
                    bytesPositionLength.CopyTo(dataBuffer, 0);
                    // Copies the position
                    bytesPosition.CopyTo(dataBuffer, bytesPositionLength.Length);
                    // Copies the frame size's length
                    bytesFrameSizeLength.CopyTo(dataBuffer, bytesPosition.Length + bytesPositionLength.Length);
                    // Copies the frame size
                    bytesFrameSize.CopyTo(dataBuffer, bytesPosition.Length + bytesPositionLength.Length + bytesFrameSizeLength.Length);
                    // Copies the frame buffer
                    frameBuffer.CopyTo(dataBuffer, prefixLength);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "RtpSink -- BuildAndSendPacket", e.Message);
                }

                // Then, builds the RTP packet:
                if (rtpPacket != null)
                {
                    rtpPacket.RtpSeqNum = rtpSeqNum;
                    rtpPacket.RtpTimestamp = rtpTimestamp;
                    rtpPacket.SSRC = rtpSSRC;
                    rtpPacket.BuildRtpPacket(dataBuffer, dataBuffer.Length, ref packetBuffer, ref packetSize);
                }
                
                // Finally, sends the packet:
                if (clientRtpSocket != null)
                {
                    clientRtpSocket.SendDatagram(packetBuffer, packetSize);
                }

                // Increases the RTP sequence number:
                Interlocked.Increment(ref rtpSeqNum);

                // Increase the rate control counter if necessary:
                while (rateControlCounter <= maxCountInLowSpeed)
                {
                    Interlocked.Increment(ref rateControlCounter);
                    break;
                }
            }
        }
        #endregion
    }
}
