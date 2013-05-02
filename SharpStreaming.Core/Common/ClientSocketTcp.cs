using System;
using System.Net.Sockets;

namespace Simon.SharpStreaming.Core
{
    public class ClientSocketTcp : ClientSocketBase
    {
        #region Private Member Fields
        /// <summary>
        /// A lock of send datagram or receive datagram.
        /// </summary>
        private static object mutex = new object();

        /// <summary>
        /// The client socket.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// The received buffer.
        /// </summary>
        private byte[] recvBuffer;

        /// <summary>
        /// The received buffer size.
        /// </summary>
        private int recvBufferSize;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSocket"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public ClientSocketTcp(Socket socket)
        {
            this.socket = socket;

            this.recvBufferSize = 1024 * 4; // 4 KB buffer
            this.recvBuffer = new byte[recvBufferSize];
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Sends the datagram with the asynchronous mode.
        /// </summary>
        /// <param name="sendBuffer">The send buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        public override void SendDatagram(byte[] sendBuffer, int sendBufferSize)
        {
            lock (mutex)
            {
                try
                {
                    int bufferOffset = 0;

                    // Sends data asynchronously to a connected socket.
                    if (socket != null && socket.Connected)
                    {
                        socket.BeginSend(sendBuffer, bufferOffset, sendBufferSize, SocketFlags.None, this.EndSendDatagram, this);
                    }
                }
                catch (System.Exception e)
                {
                    this.OnClientTeardown();
                    this.OnExceptionOccurred(e);
                    Utils.OutputMessage(false, MsgLevel.Error, "ClientSocketTcp -- SendDatagram", e.Message);
                }
            }
        }

        /// <summary>
        /// Receives the datagram from the client with asynchronous mode.
        /// </summary>
        public override void ReceiveDatagram()
        {
            lock (mutex)
            {
                try
                {
                    int bufferOffset = 0;

                    // Begins to asynchronously receive data from a connected socket.
                    if (socket != null && socket.Connected)
                    {
                        socket.BeginReceive(recvBuffer, bufferOffset, recvBufferSize, SocketFlags.None, this.EndReceiveDatagram, this);
                    }
                }
                catch (System.Exception e)
                {
                    this.OnClientTeardown();
                    this.OnExceptionOccurred(e);
                    Utils.OutputMessage(false, MsgLevel.Error, "ClientSocketTcp -- ReceiveDatagram", e.Message);
                }
            }
        }

        /// <summary>
        /// Closes the client socket.
        /// </summary>
        public override void CloseClientSocket()
        {
            try
            {
                if (socket != null)
                {
                    // Disables sends and receives on a socket.
                    socket.Shutdown(SocketShutdown.Both);

                    // Closes the socket connection and releases all associated resources.
                    socket.Close();
                }
            }
            catch
            {
                // Shut down it by force, ignores any exceptions.
            }
        }
        #endregion

        #region Protected Override Methods
        /// <summary>
        /// Ends to send the datagram.
        /// </summary>
        /// <param name="iar">The iar.</param>
        protected override void EndSendDatagram(IAsyncResult iar)
        {
            lock (mutex)
            {
                try
                {
                    if (socket != null && socket.Connected)
                    {
                        // Ends a pending asynchronous send.
                        socket.EndSend(iar);
                        iar.AsyncWaitHandle.Close();
                    }
                    else
                    {
                        this.OnClientTeardown();
                    }
                }
                catch (System.Exception e)
                {
                    this.OnClientTeardown();
                    this.OnExceptionOccurred(e);
                    Utils.OutputMessage(false, MsgLevel.Error, "ClientSocketTcp -- EndSendDatagram", e.Message);
                }
            }
        }

        /// <summary>
        /// Ends to receive the datagram.
        /// </summary>
        /// <param name="iar">The iar.</param>
        protected override void EndReceiveDatagram(IAsyncResult iar)
        {
            lock (mutex)
            {
                try
                {
                    if (socket != null && socket.Connected)
                    {
                        // Ends a pending asynchronous read.
                        int readBufferSize = socket.EndReceive(iar);
                        iar.AsyncWaitHandle.Close();

                        if (readBufferSize == 0)
                        {
                            this.OnClientTeardown();
                        }
                        else
                        {
                            // Reports the received datagram to the client session.
                            this.OnDatagramReceived(recvBuffer, readBufferSize);

                            // Continues to receive the datagram from the client.
                            ReceiveDatagram();
                        }
                    }
                    else
                    {
                        this.OnClientTeardown();
                    }
                }
                catch (System.Exception e)
                {
                    this.OnClientTeardown();
                    this.OnExceptionOccurred(e);
                    Utils.OutputMessage(false, MsgLevel.Error, "ClientSocketTcp -- EndReceiveDatagram", e.Message);
                }
            }
        }

        /// <summary>
        /// Called when [client tear down].
        /// </summary>
        protected override void OnClientTeardown()
        {
            // Calls the base class event invocation method.
            base.OnClientTeardown();
        }

        /// <summary>
        /// Called when [datagram received].
        /// </summary>
        /// <param name="recvBuffer">The received buffer.</param>
        /// <param name="recvBufferSize">Size of the received buffer.</param>
        protected override void OnDatagramReceived(byte[] recvBuffer, int recvBufferSize)
        {
            // Calls the base class event invocation method.
            base.OnDatagramReceived(recvBuffer, recvBufferSize);
        }

        /// <summary>
        /// Called when [exception occurred].
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected override void OnExceptionOccurred(Exception ex)
        {
            // Calls the base class event invocation method.
            base.OnExceptionOccurred(ex);
        }
        #endregion
    }
}
