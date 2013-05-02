using System;
using System.Net.Sockets;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// Holds the socket event arguments.
    /// </summary>
    public class TSocketEventArgs : EventArgs
    {
        /// <summary>
        /// The client socket of TCP.
        /// </summary>
        public readonly Socket ClientSocket;

        /// <summary>
        /// The received buffer.
        /// </summary>
        public readonly byte[] RecvBuffer;

        /// <summary>
        /// The size of the received buffer.
        /// </summary>
        public readonly int RecvBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TSocketEventArgs"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public TSocketEventArgs(Socket socket)
        {
            this.ClientSocket = socket;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TSocketEventArgs"/> class.
        /// </summary>
        /// <param name="recvBuffer">The receive buffer.</param>
        /// <param name="recvBufferSize">Size of the receive buffer.</param>
        public TSocketEventArgs(byte[] recvBuffer, int recvBufferSize)
        {
            RecvBuffer = recvBuffer;
            RecvBufferSize = recvBufferSize;
        }
    }
}
