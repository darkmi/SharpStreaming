using System;
using System.Collections.Generic;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    public class MPEG2TransportFileServerMediaSession : ServerMediaSession
    {
        #region Private Member Fields
        /// <summary>
        /// The file name.
        /// </summary>
        private string fileName;

        /// <summary>
        /// The file duration.
        /// </summary>
        private double fileDuration;

        /// <summary>
        /// The file size.
        /// </summary>
        private long fileSize;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MPEG2TransportFileServerMediaSession"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public MPEG2TransportFileServerMediaSession(string fileName)
            : base(fileName)
        {
            this.fileName = fileName;
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Seeks the stream source.
        /// </summary>
        /// <param name="inputSource">The input source.</param>
        /// <param name="seekNPT">The seek NPT(Normal Play Time).</param>
        public override void SeekStreamSource(MediaSource inputSource, double seekNPT)
        {
            if (inputSource != null)
            {
                inputSource.SeekWithinFile(seekNPT);
            }
        }

        /// <summary>
        /// Creates the stream source.
        /// </summary>
        /// <returns>The object of MediaSource.</returns>
        public override MediaSource CreateStreamSource()
        {
            string filePath = FileCatalog + fileName;
            MPEG2TransportStreamFileSource fileSource = new MPEG2TransportStreamFileSource(filePath);
            if (fileSource == null)
            {
                // Error
                Utils.OutputMessage(false, MsgLevel.Error, 
                    "MPEG2TransportFileServerMediaSession -- CreateStreamSource",
                    "Could not create an instance of MPEG2TransportStreamFileSource!");
                return null;
            }

            fileDuration = fileSource.Duration;
            fileSize = fileSource.FileSize;

            return fileSource;
        }

        /// <summary>
        /// Creates the RTP sink.
        /// </summary>
        /// <param name="inputSource">The input source.</param>
        /// <param name="clientRtpSocket">The client RTP socket.</param>
        /// <returns>The RtpSink object.</returns>
        public override RtpSink CreateRtpSink(MediaSource inputSource, ClientSocketBase clientRtpSocket)
        {
            int payloadType = 33; // MP2T video codec, defined in RFC 2250.

            RtpSink rtpSink = new RtpSink(payloadType, inputSource, clientRtpSocket);
            if (rtpSink == null)
            {
                // Error
                Utils.OutputMessage(false, MsgLevel.Error, 
                    "MPEG2TransportFileServerMediaSession -- CreateRtpSink",
                    "Could not create an instance of RtpSink!");
            }

            return rtpSink;
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <returns>The type of the media.</returns>
        public override string GetMediaType()
        {
            string mediaType = "Video";

            return mediaType;
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns>The duration.</returns>
        public override double GetDuration()
        {
            return fileDuration;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <returns>The file size.</returns>
        public override long GetFileSize()
        {
            return fileSize;
        }
        #endregion
    }
}
