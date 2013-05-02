using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    public class FileSink
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

        private string filePath;
        private long fileSize;
        #endregion

        #region Public Events
        public event EventHandler<TFilePermissionEventArgs> UpdateAccessRange;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSink"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileSize">Size of the file.</param>
        public FileSink(string filePath, long fileSize)
        {
            this.filePath = filePath;
            this.fileSize = fileSize;

            InitFileStream();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Writes the data to the file.
        /// </summary>
        /// <param name="frameBuffer">The frame buffer.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="position">The position.</param>
        public void WriteDataToFile(byte[] frameBuffer, int frameSize, long position)
        {
            lock (mutex)
            {
                try
                {
                    if (fileStream != null && fileStream.CanSeek)
                    {
                        fileStream.Seek(position, SeekOrigin.Begin);
                    }

                    if (fileStream != null && fileStream.CanWrite)
                    {
                        fileStream.Write(frameBuffer, 0, frameSize);
                        fileStream.Flush();
                    }
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "FileSink -- WriteDataToFile", e.Message);
                }
            }
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        public void CloseFileStream()
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
                    fileStream = new FileStream(filePath, FileMode.Create, 
                        FileAccess.ReadWrite, FileShare.ReadWrite);
                    //fileStream.SetLength(fileSize);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "FileSink -- InitFileStream", e.Message);
                }
            }
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [update access range].
        /// </summary>
        /// <param name="begin">The begin.</param>
        /// <param name="end">The end.</param>
        protected virtual void OnUpdateAccessRange(long begin, long end)
        {
            EventHandler<TFilePermissionEventArgs> handler = this.UpdateAccessRange;
            if (handler != null)
            {
                TFilePermissionEventArgs e = new TFilePermissionEventArgs(begin, end);
                handler(this, e);
            }
        }
        #endregion
    }
}
