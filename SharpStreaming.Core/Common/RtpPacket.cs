using System;

namespace Simon.SharpStreaming.Core
{
    public class RtpPacket
    {
        #region Private Member Fields
        /// <summary>
        /// The RTP version.
        /// </summary>
        private int version = 2;

        /// <summary>
        /// The maker bit, the usage of this bit depends on payload type.
        /// </summary>
        private bool isMaker = false;

        /// <summary>
        /// The RTP payload type.
        /// </summary>
        private int payloadType = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the RTP sequence number.
        /// </summary>
        /// <value>The RTP sequence number.</value>
        public int RtpSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the RTP timestamp.
        /// </summary>
        /// <value>The RTP timestamp.</value>
        public int RtpTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the SSRC.
        /// </summary>
        /// <value>The SSRC.</value>
        public uint SSRC { get; set; }

        /// <summary>
        /// Gets or sets the CSRC.
        /// </summary>
        /// <value>The CSRC.</value>
        public uint[] CSRC { get; set; }
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RtpPacket"/> class.
        /// </summary>
        public RtpPacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RtpPacket"/> class.
        /// </summary>
        /// <param name="payloadType">Type of the payload.</param>
        public RtpPacket(int payloadType)
        {
            this.payloadType = payloadType;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Builds the RTP packet.
        /// </summary>
        ///The Rtp header has the following format:
        ///
        ///0                   1                   2                   3
        ///0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|V=2|P|X|  CC   |M|     PT      |       sequence number         |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|                           timestamp                           |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|           synchronization source (SSRC) identifier            |
        ///+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
        ///|            contributing source (CSRC) identifiers             |
        ///|                             ....                              |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <param name="dataBuffer">The data buffer.</param>
        /// <param name="dataSize">Size of the data.</param>
        /// <param name="packetBuffer">The packet buffer.</param>
        /// <param name="packetSize">Size of the packet.</param>
        public void BuildRtpPacket(byte[] dataBuffer, int dataSize, ref byte[] packetBuffer, ref int packetSize)
        {
            int offset = 0;
            int cc = 0;
            if (CSRC != null)
            {
                cc = CSRC.Length;
            }

            // V P X CC
            packetBuffer[offset++] = (byte)(version << 6 | 0 << 5 | cc & 0xF);

            // M PT
            packetBuffer[offset++] = (byte)(Convert.ToInt32(isMaker) << 7 | payloadType & 0x7F);

            // sequence number
            packetBuffer[offset++] = (byte)(RtpSeqNum >> 8);
            packetBuffer[offset++] = (byte)(RtpSeqNum & 0xFF);

            // timestamp
            packetBuffer[offset++] = (byte)((RtpSeqNum >> 24) & 0xFF);
            packetBuffer[offset++] = (byte)((RtpSeqNum >> 16) & 0xFF);
            packetBuffer[offset++] = (byte)((RtpSeqNum >> 8) & 0xFF);
            packetBuffer[offset++] = (byte)(RtpSeqNum & 0xFF);

            // SSRC
            packetBuffer[offset++] = (byte)((SSRC >> 24) & 0xFF);
            packetBuffer[offset++] = (byte)((SSRC >> 16) & 0xFF);
            packetBuffer[offset++] = (byte)((SSRC >> 8) & 0xFF);
            packetBuffer[offset++] = (byte)(SSRC & 0xFF);

            // CSRCs
            if (CSRC != null)
            {
                foreach (int csrc in CSRC)
                {
                    packetBuffer[offset++] = (byte)((csrc >> 24) & 0xFF);
                    packetBuffer[offset++] = (byte)((csrc >> 16) & 0xFF);
                    packetBuffer[offset++] = (byte)((csrc >> 8) & 0xFF);
                    packetBuffer[offset++] = (byte)(csrc & 0xFF);
                }
            }

            // X
            Array.Copy(dataBuffer, 0, packetBuffer, offset, dataSize);
            packetSize = packetBuffer.Length;
        }

        /// <summary>
        /// Parses the RTP packet.
        /// </summary>
        /// The Rtp header has the following format:
        ///
        ///0                   1                   2                   3
        ///0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|V=2|P|X|  CC   |M|     PT      |       sequence number         |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|                           timestamp                           |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|           synchronization source (SSRC) identifier            |
        ///+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
        ///|            contributing source (CSRC) identifiers             |
        ///|                             ....                              |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <param name="packetBuffer">The packet buffer.</param>
        /// <param name="packetSize">Size of the packet.</param>
        /// <param name="dataBuffer">The data buffer.</param>
        /// <param name="dataSize">Size of the data.</param>
        public void ParseRtpPacket(byte[] packetBuffer, int packetSize, ref byte[] dataBuffer, ref int dataSize)
        {
            int offset = 0;

            // V
            version = packetBuffer[offset] >> 6;

            // P
            bool isPadded = Convert.ToBoolean((packetBuffer[offset] >> 5) & 0x1);

            // X
            bool hasExtention = Convert.ToBoolean((packetBuffer[offset] >> 4) & 0x1);

            // CC
            int csrcCount = packetBuffer[offset++] & 0xF;

            // M
            isMaker = Convert.ToBoolean(packetBuffer[offset] >> 7);

            // PT
            payloadType = packetBuffer[offset++] & 0x7F;

            // sequence number
            RtpSeqNum = packetBuffer[offset++] << 8 | packetBuffer[offset++];

            // timestamp
            RtpTimestamp = packetBuffer[offset++] << 24 | packetBuffer[offset++] << 16 | packetBuffer[offset++] << 8 | packetBuffer[offset++];

            // SSRC
            SSRC = (uint)(packetBuffer[offset++] << 24 | packetBuffer[offset++] << 16 
                | packetBuffer[offset++] << 8 | packetBuffer[offset++]);

            // CSRC
            CSRC = new uint[csrcCount];
            for (int i = 0; i < csrcCount; i++)
            {
                CSRC[i] = (uint)(packetBuffer[offset++] << 24 | packetBuffer[offset++] << 16 
                    | packetBuffer[offset++] << 8 | packetBuffer[offset++]);
            }

            // X
            if (hasExtention)
            {
                // Skip extension
                offset++;
                offset += packetBuffer[offset];
            }

            // TODO: Padding

            // Data
            dataBuffer = new byte[packetSize - offset];
            dataSize = dataBuffer.Length;
            Array.Copy(packetBuffer, offset, dataBuffer, 0, dataSize);
        }
        #endregion
    }
}
