using System;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// Holds the exception event arguments.
    /// </summary>
    public class TExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// The exception message.
        /// </summary>
        public readonly string ExceptionMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The ex.</param>
        public TExceptionEventArgs(Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[TargetSite: ");
            sb.Append(exception.TargetSite);
            sb.Append("] ");
            sb.Append(exception.Message);

            this.ExceptionMessage = sb.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TExceptionEventArgs(string message)
        {
            this.ExceptionMessage = message;
        }
    }
}
