using System;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// Holds the client session event arguments.
    /// </summary>
    public class TClientSessionEventArgs : EventArgs
    {
        /// <summary>
        /// The client session info.
        /// </summary>
        public readonly ClientSessionInfo SessionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="TClientSessionEventArgs"/> class.
        /// </summary>
        /// <param name="sessionInfo">The session info.</param>
        public TClientSessionEventArgs(ClientSessionInfo sessionInfo)
        {
            this.SessionInfo = sessionInfo;
        }
    }
}
