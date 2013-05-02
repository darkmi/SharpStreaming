using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX.AudioVideoPlayback;

namespace Simon.SharpStreaming.Core
{
    public class MPEG2TransportStreamFileSource : MediaSource
    {
        #region Private Member Fields
        /// <summary>
        /// A lock of the file operation.
        /// </summary>
        private static object mutex = new object();

        /// <summary>
        /// The object of the FileStream.
        /// </summary>
        private FileStream fileStream;

        private string fileName;

        private long fileSize;

        private double duration;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        public long FileSize
        {
            get { return this.fileSize; }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public double Duration
        {
            get { return this.duration; }
        }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MPEG2TransportStreamFileSource"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public MPEG2TransportStreamFileSource(string fileName)
        {
            this.fileName = fileName;

            InitFileStream();
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Seeks within the file.
        /// </summary>
        /// <param name="seekNPT">The seek NPT(Normal Play Time).</param>
        public override void SeekWithinFile(double seekNPT)
        {
            lock (mutex)
            {
                try
                {
                    long offset = (long)((seekNPT / duration) * fileSize);

                    if (fileStream != null)
                    {
                        fileStream.Seek(offset, SeekOrigin.Begin);
                    }
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "MPEG2TransportStreamFileSource -- SeekWithinFile", e.Message);
                }
            }
        }

        /// <summary>
        /// Gets the next frame.
        /// </summary>
        /// <param name="frameBuffer">The frame buffer.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="position">The position.</param>
        public override void GetNextFrame(ref byte[] frameBuffer, ref int frameSize, ref long position)
        {
            lock (mutex)
            {
                try
                {
                    if (fileStream != null)
                    {
                        position = fileStream.Position;
                        frameSize = fileStream.Read(frameBuffer, 0, PreferredFrameSize);
                    } 
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "MPEG2TransportStreamFileSource -- GetNextFrame", e.Message);
                }
            }
        }

        /// <summary>
        /// Closes the media source.
        /// </summary>
        public override void CloseMediaSource()
        {
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the file stream object.
        /// </summary>
        private void InitFileStream()
        {
            lock (mutex)
            {
                try
                {
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    fileSize = fileStream.Length;
                    GetMediaDuration();
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "MPEG2TransportStreamFileSource -- InitFileStream", e.Message);
                }
            }
        }

        /// <summary>
        /// Gets the duration of the media file.
        /// </summary>
        private void GetMediaDuration()
        {
            using (Video video= new Video(fileName))
            {
                duration = video.Duration;
            }
        }
        #endregion
    }
}
