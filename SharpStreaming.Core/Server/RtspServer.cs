using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A main class for the rtsp server.
    /// </summary>
    public class RtspServer : IDisposable
    {
        #region Private Member Fields
        /// <summary>
        /// A flag that shows whether the current server is closed or not.
        /// </summary>
        private bool isServerClosed = true;

        /// <summary>
        /// A flag that shows whether the resources will be disposed or not.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// The client session sequence number which will be as an unique id for each client.
        /// </summary>
        private int clientSessionSeqNum = 0;

        /// <summary>
        /// The current connection count of the client session.
        /// </summary>
        private int curConnectionCount = 0;

        /// <summary>
        /// The maximum connection count of the client session.
        /// </summary>
        private int maxConnectionCount = 100;

        /// <summary>
        /// The maximum milliseconds of time-out for each client session.
        /// </summary>
        private int maxSessionTimeout = 60000;

        /// <summary>
        /// The minimum rtp port number.
        /// </summary>
        private int minRtpPortNumber = 6001;

        /// <summary>
        /// The maximum rtp port number.
        /// </summary>
        private int maxRtpPortNumber = 9000;

        /// <summary>
        /// The current rtp port number.
        /// </summary>
        private int curRtpPortNumber = 0;

        /// <summary>
        /// The file catalog.
        /// </summary>
        private string fileCatalog;

        /// <summary>
        /// A table that keeps the data of the client session.
        /// </summary>
        private Dictionary<int, ClientSession> clientSessionTable;

        /// <summary>
        /// A table that keeps the data of the server media session.
        /// </summary>
        private Dictionary<string, ServerMediaSession> serverMediaSessionTable;

        /// <summary>
        /// Declares a 'SocketListener' object.
        /// </summary>
        private SocketListener socketListener;

        /// <summary>
        /// Notifies the server listening thread that the current status.
        /// </summary>
        private ManualResetEvent checkSocketAcceptResetEvent;

        /// <summary>
        /// Notifies the client session checking thread that the current status.
        /// </summary>
        private ManualResetEvent checkClientSessionResetEvent;
        #endregion

        #region Public Class Events
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;

        public event EventHandler<TExceptionEventArgs> ExceptionOccurred;

        public event EventHandler ClientSessionRejected;
        public event EventHandler<TClientSessionEventArgs> ClientSessionConnected;
        public event EventHandler<TClientSessionEventArgs> ClientSessionDisconnected;
        public event EventHandler<TClientSessionEventArgs> ClientSessionTimeout;
        public event EventHandler<TClientSessionEventArgs> ClientSessionUpdated;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the max connection count.
        /// </summary>
        /// <value>The max connection count.</value>
        public int MaxConnectionCount
        {
            get { return this.maxConnectionCount; }
        }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RtspServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="maxConnCount">The max connection count.</param>
        /// <param name="maxTimeout">The max session timeout.</param>
        /// <param name="fileCatalog">The file catalog.</param>
        public RtspServer(int port, int maxConnCount, int maxTimeout, string fileCatalog)
        {
            this.maxConnectionCount = maxConnCount;
            this.maxSessionTimeout = maxTimeout;
            this.fileCatalog = Utils.AddBackSlash(fileCatalog);

            clientSessionSeqNum = Utils.GenerateRandomNumber(1000, 9999);

            clientSessionTable = new Dictionary<int, ClientSession>();
            serverMediaSessionTable = new Dictionary<string, ServerMediaSession>();

            checkSocketAcceptResetEvent = new ManualResetEvent(true);
            checkClientSessionResetEvent = new ManualResetEvent(true);

            socketListener = new SocketListener(port);
            socketListener.ClientSocketAccepted += new EventHandler<TSocketEventArgs>(this.OnClientSocketAccepted);
            socketListener.ExceptionOccurred += new EventHandler<TExceptionEventArgs>(this.OnExceptionOccurred);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="RtspServer"/> is reclaimed by garbage collection.
        /// </summary>
        ~RtspServer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                clientSessionTable = null;
                serverMediaSessionTable = null;
            }

            if (checkSocketAcceptResetEvent != null)
            {
                checkSocketAcceptResetEvent.Close();
            }

            if (checkClientSessionResetEvent != null)
            {
                checkClientSessionResetEvent.Close();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns>success or failed</returns>
        public bool StartServer()
        {
            if (!isServerClosed)
            {
                return true;
            }

            if (socketListener == null)
            {
                return false;
            }

            isServerClosed = true;

            ClearCountValues();

            try
            {
                if (!socketListener.CreateServerSocket())
                {
                    return false;
                }

                if (!ThreadPool.QueueUserWorkItem(StartServerListening))
                {
                    return false;
                }

                if (!ThreadPool.QueueUserWorkItem(CheckClientSession))
                {
                    return false;
                }

                isServerClosed = false;

                this.OnServerStarted();
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
                Utils.OutputMessage(false, MsgLevel.Error, e.TargetSite.ToString(), e.Message);
            }

            return !isServerClosed;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns>success or failed</returns>
        public bool StopServer()
        {
            if (isServerClosed)
            {
                return true;
            }

            isServerClosed = true;

            // First, waits for two threads.
            // Blocks the current thread until the current WaitHandle receives a signal.
            checkSocketAcceptResetEvent.WaitOne();
            checkClientSessionResetEvent.WaitOne();

            // And then, closes each client socket.
            if (clientSessionTable != null && socketListener != null)
            {
                lock (clientSessionTable)
                {
                    foreach (ClientSession session in clientSessionTable.Values)
                    {
                        session.Close();
                    }
                }
            }

            // Then, closes the server socket.
            if (socketListener != null)
            {
                socketListener.CloseServerSocket();
            }

            // At last, clears the server media session table & client session table.
            if (serverMediaSessionTable != null)
            {
                lock(serverMediaSessionTable)
                {
                    serverMediaSessionTable.Clear();
                }
            }

            if (clientSessionTable != null)
            {
                lock (clientSessionTable)
                {
                    clientSessionTable.Clear();
                }
            }

            this.OnServerStopped();

            return true;
        }

        /// <summary>
        /// Lookups the server media session.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>The ServerMediaSession object.</returns>
        public ServerMediaSession LookupServerMediaSession(string streamName)
        {
            bool fileExists = false;
            bool smsExists = false;

            // First, checks whether the specified "streamName" exists as a local file.
            string filePath = fileCatalog + streamName;
            fileExists = File.Exists(filePath);

            // Next, checks whether we already have a "ServerMediaSession" for this file.
            ServerMediaSession sms = null;
            lock (serverMediaSessionTable)
            {
                if (serverMediaSessionTable.ContainsKey(streamName))
                {
                    sms = serverMediaSessionTable[streamName];
                }
            }

            // The "ServerMediaSession" exists.
            if (sms != null)
            {
                smsExists = true;
            }

            // Handles the four possibilities for "fileExists" and "smsExists".
            if (!fileExists)
            {
                if (smsExists)
                {
                    // "sms" was created for a file that no longer exists, so removes it.
                    RemoveServerMediaSession(streamName);
                }
            } 
            else
            {
                if (!smsExists)
                {
                    // Creates a new "ServerMediaSession" object for streaming from the named file.
                    // And then add it to the table of ServerMediaSession.
                    sms = AddServerMediaSession(streamName);
                }
            }

            return sms;
        }

        /// <summary>
        /// Generates the RTP port number.
        /// </summary>
        /// <returns>The RTP port number.</returns>
        public int GenerateRtpPortNumber()
        {
            curRtpPortNumber += 2;

            if (curRtpPortNumber < minRtpPortNumber ||
                curRtpPortNumber > maxRtpPortNumber)
            {
                curRtpPortNumber = minRtpPortNumber;
            }

            return curRtpPortNumber;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Starts the server listening, this is a loop until the current server is closed.
        /// </summary>
        /// <param name="state">The state.</param>
        private void StartServerListening(object state)
        {
            checkSocketAcceptResetEvent.Reset();

            Socket clientSocket = null;
            while (!isServerClosed)
            {
                if (socketListener != null)
                {
                    socketListener.StartListening(clientSocket);
                }
            }

            checkSocketAcceptResetEvent.Set();
        }

        /// <summary>
        /// Checks the rtsp client session.
        /// In this method, we do lost of things such as check the client session
        /// is time out or not, check the client session is tear down or not.
        /// </summary>
        /// <param name="state">The state.</param>
        private void CheckClientSession(object state)
        {
            checkClientSessionResetEvent.Reset();

            while (!isServerClosed)
            {
                lock (clientSessionTable)
                {
                    List<int> clientSessionIdList = new List<int>();

                    foreach (ClientSession session in clientSessionTable.Values)
                    {
                        if (isServerClosed)
                        {
                            break;
                        }

                        if (session.CurrentSessionState == ClientSessionState.Inactive)
                        {
                            session.Close();
                            clientSessionIdList.Add(session.ClientSessionId);
                            this.OnClientSessionDisconnected(session);
                        }
                        else
                        {
                            if (session.CheckTimeout(maxSessionTimeout))
                            {
                                this.OnClientSessionTimeout(session);
                            }
                        }
                    }

                    foreach (int id in clientSessionIdList)
                    {
                        clientSessionTable.Remove(id);
                    }

                    clientSessionIdList.Clear();
                }

                // Sleep the current thread, and then continue to check the client session.
                Thread.Sleep(100);
            }

            checkClientSessionResetEvent.Set();
        }

        /// <summary>
        /// Clears the count values.
        /// </summary>
        private void ClearCountValues()
        {
            curConnectionCount = 0;
            clientSessionSeqNum = 0;

            clientSessionTable.Clear();
            serverMediaSessionTable.Clear();
        }

        /// <summary>
        /// Adds a new client session.
        /// </summary>
        /// <param name="clientSocket">The client socket.</param>
        private void AddClientSession(Socket clientSocket)
        {
            Interlocked.Increment(ref clientSessionSeqNum);

            ClientSession clientSession = new ClientSession(clientSessionSeqNum, clientSocket, this);
            clientSession.ExceptionOccurred += new EventHandler<TExceptionEventArgs>(this.OnExceptionOccurred);
            clientSession.ClientSessionUpdated +=new EventHandler<TClientSessionEventArgs>(this.OnClientSessionUpdated);

            lock (clientSessionTable)
            {
                clientSessionTable.Add(clientSessionSeqNum, clientSession);
            }

            // Starts handling the rtsp client request.
            clientSession.ReceiveClientRequest(clientSocket);

            this.OnClientSessionConnected(clientSession);
        }

        /// <summary>
        /// Adds the server media session.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>The ServerMediaSession object.</returns>
        private ServerMediaSession AddServerMediaSession(string streamName)
        {
            ServerMediaSession sms = null;

            MPEG2TransportFileServerMediaSession mpeg2TransportFileSMS = new MPEG2TransportFileServerMediaSession(streamName);
            if (mpeg2TransportFileSMS == null)
            {
                return null; // Creates the MPEG2TransportFileServerMediaSubsession failed!
            }
            else
            {
                sms = mpeg2TransportFileSMS;

                // Also, sets the file catalog:
                mpeg2TransportFileSMS.FileCatalog = fileCatalog;
            }

            lock (serverMediaSessionTable)
            {
                serverMediaSessionTable.Add(streamName, sms);
            }

            return sms;
        }

        /// <summary>
        /// Removes the server media session.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        private void RemoveServerMediaSession(string streamName)
        {
            lock (serverMediaSessionTable)
            {
                if (serverMediaSessionTable.ContainsKey(streamName))
                {
                    serverMediaSessionTable.Remove(streamName);
                }
            }
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [server started].
        /// </summary>
        protected virtual void OnServerStarted()
        {
            EventHandler handler = this.ServerStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [server stopped].
        /// </summary>
        protected virtual void OnServerStopped()
        {
            EventHandler handler = this.ServerStopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [exception occurred].
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected virtual void OnExceptionOccurred(Exception ex)
        {
            EventHandler<TExceptionEventArgs> handler = this.ExceptionOccurred;
            if (handler != null)
            {
                TExceptionEventArgs e = new TExceptionEventArgs(ex);
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [exception occurred].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExceptionOccurred(object sender, TExceptionEventArgs e)
        {
            EventHandler<TExceptionEventArgs> handler = this.ExceptionOccurred;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [client socket accepted].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TSocketEventArgs"/> instance containing the event data.</param>
        protected virtual void OnClientSocketAccepted(object sender, TSocketEventArgs e)
        {
            // Is the current connection count greater than the maximum
            // connection count? If so, we close the client socket, or
            // else, we add the client session as a new client session.
            if (this.curConnectionCount > this.maxConnectionCount)
            {
                if (this.socketListener != null)
                {
                    this.socketListener.CloseClientSocket(e.ClientSocket);
                    this.OnClientSessionRejected();
                }
            }
            else
            {
                AddClientSession(e.ClientSocket);
            }
        }

        /// <summary>
        /// Called when [client session rejected].
        /// </summary>
        protected virtual void OnClientSessionRejected()
        {
            EventHandler handler = this.ClientSessionRejected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [client session connected].
        /// </summary>
        /// <param name="clientSession">The client session.</param>
        protected virtual void OnClientSessionConnected(ClientSession clientSession)
        {
            Interlocked.Increment(ref curConnectionCount);

            EventHandler<TClientSessionEventArgs> handler = this.ClientSessionConnected;
            if (handler != null)
            {
                TClientSessionEventArgs e = new TClientSessionEventArgs(clientSession);
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [client session disconnected].
        /// </summary>
        /// <param name="clientSession">The client session.</param>
        protected virtual void OnClientSessionDisconnected(ClientSession clientSession)
        {
            Interlocked.Decrement(ref curConnectionCount);

            EventHandler<TClientSessionEventArgs> handler = this.ClientSessionDisconnected;
            if (handler != null)
            {
                TClientSessionEventArgs e = new TClientSessionEventArgs(clientSession);
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [client session timeout].
        /// </summary>
        /// <param name="clientSession">The client session.</param>
        protected virtual void OnClientSessionTimeout(ClientSession clientSession)
        {
            Interlocked.Decrement(ref curConnectionCount);

            EventHandler<TClientSessionEventArgs> handler = this.ClientSessionTimeout;
            if (handler != null)
            {
                TClientSessionEventArgs e = new TClientSessionEventArgs(clientSession);
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [client session updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Simon.SharpStreamingServer.Core.TClientSessionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnClientSessionUpdated(object sender, TClientSessionEventArgs e)
        {
            EventHandler<TClientSessionEventArgs> handler = this.ClientSessionUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
