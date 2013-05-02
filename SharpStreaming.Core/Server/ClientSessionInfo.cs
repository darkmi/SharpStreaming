using System;

namespace Simon.SharpStreaming.Core
{
    public class ClientSessionInfo
    {
        /// <summary>
        /// Gets or sets the client session id.
        /// </summary>
        /// <value>The client session id.</value>
        public int ClientSessionId { get; set; }

        /// <summary>
        /// Gets or sets the client session IP.
        /// </summary>
        /// <value>The client session IP.</value>
        public string ClientSessionIP { get; set; }

        /// <summary>
        /// Gets or sets the client session port.
        /// </summary>
        /// <value>The client session port.</value>
        public int ClientSessionPort { get; set; }

        /// <summary>
        /// Gets or sets the session connect time.
        /// </summary>
        /// <value>The session connect time.</value>
        public DateTime SessionConnectTime { get; set; }

        /// <summary>
        /// Gets or sets the session last time.
        /// </summary>
        /// <value>The session last time.</value>
        public DateTime SessionLastTime { get; set; }

        /// <summary>
        /// Gets or sets the state of the session.
        /// </summary>
        /// <value>The state of the session.</value>
        public string SessionState { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>The name of the stream.</value>
        public string StreamName { get; set; }
    }
}
