
namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A base class for the operation of the media file.
    /// </summary>
    public class MediaSource
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets max the size of the frame.
        /// </summary>
        /// <value>The max size of the frame.</value>
        public int MaxFrameSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the preferred frame.
        /// </summary>
        /// <value>The size of the preferred frame.</value>
        public int PreferredFrameSize { get; set; }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSource"/> class.
        /// </summary>
        public MediaSource()
        {
            MaxFrameSize = 1360;
        }
        #endregion

        #region Public Virtual Methods
        /// <summary>
        /// Seeks within the file.
        /// </summary>
        /// <param name="seekNPT">The seek NPT(Normal Play Time).</param>
        public virtual void SeekWithinFile(double seekNPT)
        {
            // No implementation.
        }

        /// <summary>
        /// Gets the next frame.
        /// </summary>
        /// <param name="frameBuffer">The frame buffer.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="position">The position.</param>
        public virtual void GetNextFrame(ref byte[] frameBuffer, ref int frameSize, ref long position)
        {
            // No implementation.
        }

        /// <summary>
        /// Closes the media source.
        /// </summary>
        public virtual void CloseMediaSource()
        {
            // No implementation.
        }
        #endregion
    }
}
