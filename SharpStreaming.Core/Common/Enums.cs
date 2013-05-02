
namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// The enum of level to be shown.
    /// </summary>
    public enum MsgLevel
    {
        /// <summary>
        /// The debug level.
        /// </summary>
        Debug,

        /// <summary>
        /// The error level.
        /// </summary>
        Error,

        /// <summary>
        /// The warn level.
        /// </summary>
        Warn,

        /// <summary>
        /// The info level.
        /// </summary>
        Info
    }

    /// <summary>
    /// The enum of the client session state to make sure that
    /// a client session is active or not.
    /// This enum is used on server side.
    /// </summary>
    public enum ClientSessionState
    {
        /// <summary>
        /// The client session is active.
        /// </summary>
        Active,

        /// <summary>
        /// The client session is inactive.
        /// </summary>
        Inactive
    }

    /// <summary>
    /// The enum of the player state.
    /// This enum is used on client side.
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// The player is ready to play.
        /// </summary>
        Ready,

        /// <summary>
        /// The player is buffering.
        /// </summary>
        Buffering,

        /// <summary>
        /// The player is playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The player is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The player is stopped.
        /// </summary>
        Stopped
    }
}
