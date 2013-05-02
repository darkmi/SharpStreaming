using System;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A base class for client socket.
    /// </summary>
    public class ClientSocketBase
    {
        #region Public Class Events
        public event EventHandler ClientTeardown;
        public event EventHandler<TSocketEventArgs> DatagramReceived;
        public event EventHandler<TExceptionEventArgs> ExceptionOccurred;
        #endregion

        #region Public Virtual Methods
        /// <summary>
        /// Sends the datagram with the asynchronous mode.
        /// </summary>
        /// <param name="sendBuffer">The send buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        public virtual void SendDatagram(byte[] sendBuffer, int sendBufferSize)
        {
            // No implementation.
        }

        /// <summary>
        /// Receives the datagram with the asynchronous mode.
        /// </summary>
        public virtual void ReceiveDatagram()
        {
            // No implementation.
        }

        /// <summary>
        /// Closes the client socket.
        /// </summary>
        public virtual void CloseClientSocket()
        {
            // No implementation.
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Ends to send the datagram.
        /// </summary>
        /// <param name="iar">The iar.</param>
        protected virtual void EndSendDatagram(IAsyncResult iar)
        {
            // No implementation.
        }

        /// <summary>
        /// Ends to receive the datagram.
        /// </summary>
        /// <param name="iar">The iar.</param>
        protected virtual void EndReceiveDatagram(IAsyncResult iar)
        {
            // No implementation.
        }

        /// <summary>
        /// Called when [client tear down].
        /// </summary>
        protected virtual void OnClientTeardown()
        {
            EventHandler handler = this.ClientTeardown;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [datagram received].
        /// </summary>
        /// <param name="recvBuffer">The received buffer.</param>
        /// <param name="recvBufferSize">Size of the received buffer.</param>
        protected virtual void OnDatagramReceived(byte[] recvBuffer, int recvBufferSize)
        {
            EventHandler<TSocketEventArgs> handler = this.DatagramReceived;
            if (handler != null)
            {
                TSocketEventArgs e = new TSocketEventArgs(recvBuffer, recvBufferSize);
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
