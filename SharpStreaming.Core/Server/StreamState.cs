using System;
using System.Collections.Generic;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A class that represents the state of an ongoing stream.
    /// </summary>
    public class StreamState
    {
        #region Private Member Fields
        private RtpSink rtpSink;
        private MediaSource mediaSource;
        private ClientSocketBase clientRtcpSocket;

        private bool isCurrentlyPlaying;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the sink.
        /// </summary>
        /// <value>The rtp sink.</value>
        public RtpSink Sink
        {
            get { return this.rtpSink; }
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The media source.</value>
        public MediaSource Source
        {
            get { return this.mediaSource; }
        }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamState"/> class.
        /// </summary>
        /// <param name="rtpSink">The RTP sink.</param>
        /// <param name="inputSource">The input source.</param>
        /// <param name="clientRtcpSocket">The client RTCP socket.</param>
        public StreamState(RtpSink rtpSink, MediaSource inputSource, ClientSocketBase clientRtcpSocket)
        {
            this.rtpSink = rtpSink;
            this.mediaSource = inputSource;
            this.clientRtcpSocket = clientRtcpSocket;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the playing.
        /// </summary>
        public void StartPlaying()
        {
            if (!isCurrentlyPlaying)
            {
                if (rtpSink != null)
                {
                    rtpSink.StartPlaying();
                    isCurrentlyPlaying = true;
                }
            }
        }

        /// <summary>
        /// Pauses the playing.
        /// </summary>
        public void PausePlaying()
        {
            if (rtpSink != null)
            {
                rtpSink.StopPlaying();
                isCurrentlyPlaying = false;
            }
        }

        /// <summary>
        /// Ends the playing.
        /// </summary>
        public void EndPlaying()
        {
            if (rtpSink != null)
            {
                rtpSink.EndPlaying();
            }
        }
        #endregion
    }
}
