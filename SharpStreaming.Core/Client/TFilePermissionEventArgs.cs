using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    public class TFilePermissionEventArgs : EventArgs
    {
        /// <summary>
        /// The begin offset.
        /// </summary>
        public readonly long BeginOffset;

        /// <summary>
        /// The end offset.
        /// </summary>
        public readonly long EndOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="TFilePermissionEventArgs"/> class.
        /// </summary>
        /// <param name="begin">The begin.</param>
        /// <param name="end">The end.</param>
        public TFilePermissionEventArgs(long begin, long end)
        {
            this.BeginOffset = begin;
            this.EndOffset = end;
        }
    }
}
