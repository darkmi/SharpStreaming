using System;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A class that provides utility methods for RTP.
    /// </summary>
    public class RtpUtils
    {
        /// <summary>
        /// Generates random SSRC value.
        /// </summary>
        /// <returns>Returns random SSRC value.</returns>
        public static uint GenerateSSRC()
        {
            uint ssrc = (uint)Utils.GenerateRandomNumber(100000, int.MaxValue);

            return ssrc;
        }

        /// <summary>
        /// Generates random CNAME value.
        /// </summary>
        /// <returns>The CNAME value.</returns>
        public static string GenerateCNAME()
        {
            string CNAME = Guid.NewGuid().ToString();

            return CNAME;
        }

        /// <summary>
        /// Generates the random sequence number.
        /// </summary>
        /// <returns>The random sequence number.</returns>
        public static int GenerateSeqNumber()
        {
            // The sequence number must be random because of security:
            int seqNumber = Utils.GenerateRandomNumber(1, 10000);

            return seqNumber;
        }
    }
}
