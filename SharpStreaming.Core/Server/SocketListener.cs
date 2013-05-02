using System;
using System.Net;
using System.Net.Sockets;

namespace Simon.SharpStreaming.Core
{
    public class SocketListener
    {
        #region Private Member Fields
        /// <summary>
        /// Declares a server listen socket object for listening.
        /// </summary>
        private Socket listenSocket;

        /// <summary>
        /// The server port number.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// The maximum length of the pending connections queue.
        /// </summary>
        private int maxBackLog = 10;

        /// <summary>
        /// The time to wait for a response, in microseconds. 
        /// </summary>
        private int acceptListenTimeInterval = 20;
        #endregion

        #region Public Class Events
        public EventHandler<TSocketEventArgs> ClientSocketAccepted;
        public EventHandler<TExceptionEventArgs> ExceptionOccurred;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketListener"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public SocketListener(int port)
        {
            this.serverPort = port;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the server socket.
        /// </summary>
        /// <returns>success or failed</returns>
        public bool CreateServerSocket()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, serverPort);

            try
            {
                // Initializes a new instance of the Socket class using the specified address family, socket type and protocol.
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Associates a Socket with a local endpoint.
                listenSocket.Bind(iep);
                // Places a Socket in a listening state.
                listenSocket.Listen(maxBackLog);

                return true;
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
                Utils.OutputMessage(false, MsgLevel.Error, e.TargetSite.ToString(), e.Message);

                return false;
            }
        }

        /// <summary>
        /// Starts the listening.
        /// </summary>
        /// <param name="clientSocket">The client TCP socket(null).</param>
        public void StartListening(Socket clientSocket)
        {
            if (listenSocket == null)
            {
                CreateServerSocket();
            }

            try
            {
                // Determines the status of the Socket.
                if (listenSocket.Poll(acceptListenTimeInterval, SelectMode.SelectRead))
                {
                    // Creates a new Socket for a newly created connection.
                    clientSocket = listenSocket.Accept();
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        this.OnClientSocketAccepted(clientSocket);
                    }
                    else
                    {
                        // The client socket is null or it was not connected, so we
                        // try to close it.
                        CloseClientSocket(clientSocket);
                    }
                }
            }
            catch (System.Exception e)
            {
                CloseClientSocket(clientSocket);
                this.OnExceptionOccurred(e);
                Utils.OutputMessage(false, MsgLevel.Error, e.TargetSite.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Closes the server socket if it is not null.
        /// </summary>
        public void CloseServerSocket()
        {
            if (listenSocket == null)
            {
                return;
            }

            try
            {
                // Closes the Socket connection and releases all associated resources.
                listenSocket.Close();
            }
            catch (System.Exception e)
            {
                this.OnExceptionOccurred(e);
                Utils.OutputMessage(false, MsgLevel.Error, e.TargetSite.ToString(), e.Message);
            }
            finally
            {
                listenSocket = null;
            }
        }

        /// <summary>
        /// Closes the client socket if it is not null.
        /// </summary>
        /// <param name="clientSocket">The client socket.</param>
        public void CloseClientSocket(Socket clientSocket)
        {
            if (clientSocket == null)
            {
                return;
            }

            try
            {
                // Disables sends and receives on a Socket.
                clientSocket.Shutdown(SocketShutdown.Both);

                // Closes the Socket connection and releases all associated resources.
                clientSocket.Close();
            }
            catch
            {
                // Shut down it by force, ignores any exceptions.
            }
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [client socket accepted].
        /// </summary>
        /// <param name="clientSocket">The socket.</param>
        protected virtual void OnClientSocketAccepted(Socket clientSocket)
        {
            EventHandler<TSocketEventArgs> handler = this.ClientSocketAccepted;
            if (handler != null)
            {
                TSocketEventArgs e = new TSocketEventArgs(clientSocket);
                handler(this, e);
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
        #endregion
    }
}
