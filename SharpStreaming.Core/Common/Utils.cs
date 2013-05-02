using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using LogThis;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A utilities class.
    /// </summary>
    public static class Utils
    {
        #region Output Message Method
        /// <summary>
        /// Outputs the message to the log file.
        /// If 'showMsgBox' is set to true, then the message box will be shown.
        /// </summary>
        /// <param name="showMsgBox">if set to <c>true</c> [show message box].</param>
        /// <param name="level">The level to be shown.</param>
        /// <param name="messages">The messages.</param>
        public static void OutputMessage(bool showMsgBox, MsgLevel level, string location, params string[] messages)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(location))
            {
                sb.Append("[Location: ");
                sb.Append(location);
                sb.Append("] ");
            }

            foreach (string s in messages)
            {
                sb.Append(s);
                sb.Append(", ");
            }

            string message = string.Empty;
            if (sb.Length > 0)
            {
                // Removes all trailing occurrences of a set of characters 
                // specified in an array from the current String object.
                message = sb.ToString().TrimEnd(',', ' ');
            }

            eloglevel logLevel = eloglevel.info;
            string caption = string.Empty;
            MessageBoxIcon msgBoxIcon = MessageBoxIcon.Information;

            switch (level)
            {
                case MsgLevel.Debug:
                    logLevel = eloglevel.info;
                    caption = "Debug Message";
                    msgBoxIcon = MessageBoxIcon.Information;
                    break;

                case MsgLevel.Error:
                    logLevel = eloglevel.error;
                    caption = "Error Message";
                    msgBoxIcon = MessageBoxIcon.Error;
                    break;

                case MsgLevel.Warn:
                    logLevel = eloglevel.warn;
                    caption = "Warn Message";
                    msgBoxIcon = MessageBoxIcon.Warning;
                    break;

                case MsgLevel.Info:
                    logLevel = eloglevel.info;
                    caption = "Information";
                    msgBoxIcon = MessageBoxIcon.Information;
                    break;

                default:
                    break;
            }

            // Writes the message to the log file.
            Log.LogThis(message, logLevel);

            if (showMsgBox)
            {
                // Displays a message box with specified text, caption, buttons, and icon.
                MessageBox.Show(message, caption, MessageBoxButtons.OK, msgBoxIcon);
            }

            if (level == MsgLevel.Debug)
            {
                // Writes a message followed by a line terminator to 
                // the trace listeners in the Listeners collection.
                Debug.WriteLine(message);
            }
        }
        #endregion

        #region Network Related Methods
        /// <summary>
        /// Creates the UDP socket.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <returns>The UDP socket.</returns>
        public static Socket CreateUdpSocket(int port)
        {
            Socket socket = null;
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(iep);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- CreateUdpSocket", e.Message);
            }

            return socket;
        }

        /// <summary>
        /// Gets the local addresses.
        /// </summary>
        /// <returns>The addresses.</returns>
        public static string[] GetLocalAddresses()
        {
            // Get host name
            string hostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(hostName);

            string[] retval = new string[iphostentry.AddressList.Length];

            int i = 0;
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                retval[i] = ipaddress.ToString();
                i++;
            }

            return retval;
        }
        #endregion

        #region Directory Related Methods
        /// <summary>
        /// Creates the directory, if the directory does not exist, then creates a new one.
        /// </summary>
        /// <param name="dir">The directory that wants to be created.</param>
        /// <returns>The new directory.</returns>
        public static string CreateDirectory(string dir)
        {
            string newDir = string.Empty;

            if (Directory.Exists(dir))
            {
                string currentDir = Directory.GetCurrentDirectory();
                newDir = Utils.AddBackSlash(currentDir) + dir;
            }
            else
            {
                try
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(dir);
                    newDir = dirInfo.FullName;
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "Utils -- CreateDirectory", e.Message);
                }
            }

            return newDir;
        }

        /// <summary>
        /// Adds the back slash.
        /// </summary>
        /// <param name="dir">The directory string.</param>
        /// <returns>The new directory string.</returns>
        public static string AddBackSlash(string dir)
        {
            string newDir;
            bool result = dir.EndsWith("\\");
            if (result)
            {
                newDir = dir;
            }
            else
            {
                newDir = dir + "\\";
            }

            return newDir;
        }
        #endregion

        #region Conversion Methods
        /// <summary>
        /// Converts the time to string.
        /// </summary>
        /// <param name="time">The time to be converted.</param>
        /// <returns>The time format string.</returns>
        public static string ConvertTimeToString(double time)
        {
            string timeFormat = "00:00:00";
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            checked
            {
                hours = (int)time / 3600;
                minutes = ((int)time - (hours * 3600)) / 60;
                seconds = (int)time - hours * 3600 - minutes * 60;
            }

            timeFormat = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

            return timeFormat;
        }

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>Byte array made from the string.</returns>
        public static byte[] StringToBytes(string s)
        {
            byte[] bytes = null;

            try
            {
                bytes = Encoding.Default.GetBytes(s);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- StringToBytes", e.Message);
            }

            return bytes;
        }

        /// <summary>
        /// Converts a byte array array to a string.
        /// </summary>
        /// <param name="b">Byte array to be converted.</param>
        /// <param name="length">Byte array length.</param>
        /// <returns>A string.</returns>
        public static string BytesToString(byte[] b, int length)
        {
            string s = string.Empty;

            try
            {
                s = Encoding.Default.GetString(b, 0, length);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- BytesToString", e.Message);
            }

            return s;
        }

        /// <summary>
        /// Converts a long number to a byte array.
        /// </summary>
        /// <param name="number">The long number to be converted.</param>
        /// <returns>Byte array made from the string.</returns>
        public static byte[] LongToBytes(long number)
        {
            List<byte> bytesList = new List<byte>();
            while (number / 256 > 0)
            {
                bytesList.Add((byte)(number % 256));
                number /= 256;
            }
            bytesList.Add((byte)number);

            byte[] bytes = new byte[bytesList.Count];

            try
            {
                bytesList.CopyTo(bytes);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- LongToBytes", e.Message);
            }

            return bytes;
        }

        /// <summary>
        /// Converts a byte array array to a long number.
        /// </summary>
        /// <param name="b">Byte array to be converted.</param>
        /// <returns>The long number.</returns>
        public static long BytesToLong(byte[] b)
        {
            long number = 0;

            try
            {
                byte[] temp = new byte[8];
                b.CopyTo(temp, 0);
                number = BitConverter.ToInt64(temp, 0);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- BytesToLong", e.Message);
            }

            return number;
        }

        /// <summary>
        /// Converts a byte array array to a integer number.
        /// </summary>
        /// <param name="b">Byte array to be converted.</param>
        /// <returns>The integer number.</returns>
        public static int BytesToInteger(byte[] b)
        {
            int number = 0;

            try
            {
                byte[] temp = new byte[4];
                b.CopyTo(temp, 0);
                number = BitConverter.ToInt32(temp, 0);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- BytesToInteger", e.Message);
            }

            return number;
        }

        /// <summary>
        /// Converts a string to integer.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>The integer number.</returns>
        public static int StringToInteger(string s)
        {
            int number = 0;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    number = int.Parse(s);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "Utils -- StringToInteger", e.Message);
                }
            }

            return number;
        }

        /// <summary>
        /// Converts a string to long number.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>The long number.</returns>
        public static long StringToLong(string s)
        {
            long number = 0;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    number = long.Parse(s);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "Utils -- StringToLong", e.Message);
                }
            }

            return number;
        }

        /// <summary>
        /// Converts a string to float number.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>The float number.</returns>
        public static float StringToFloat(string s)
        {
            float number = 0.0f;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    number = float.Parse(s);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "Utils -- StringToFloat", e.Message);
                }
            }

            return number;
        }

        /// <summary>
        /// Converts a string to double number.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>The double number.</returns>
        public static double StringToDouble(string s)
        {
            double number = 0.0;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    number = double.Parse(s);
                }
                catch (System.Exception e)
                {
                    Utils.OutputMessage(false, MsgLevel.Error, "Utils -- StringToDouble", e.Message);
                }
            }

            return number;
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Parses the request URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Success or failed.</returns>
        public static bool ParseRequestUrl(string url, out string ip, out int port, out string fileName)
        {
            ip = "127.0.0.1";
            port = 8554;
            fileName = string.Empty;

            int length = url.Length;
            int beginIndex = url.IndexOf("://");
            if (beginIndex == -1)
            {
                return false;
            }

            string dstString = url.Substring(beginIndex + 3);

            int lastIndex = dstString.LastIndexOf("/");
            if (lastIndex == -1)
            {
                return false;
            }

            fileName = dstString.Substring(lastIndex + 1);

            int middleIndex = dstString.IndexOf(":");
            if (middleIndex == -1)
            {
                ip = dstString.Substring(0, lastIndex - 1);
            }
            else
            {
                ip = dstString.Substring(0, middleIndex);

                port = StringToInteger(dstString.Substring(middleIndex + 1, lastIndex - middleIndex - 1));
            }

            return true;
        }

        /// <summary>
        /// Generates the random number.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>The number.</returns>
        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            int retval = 0;

            Random random = new Random();

            try
            {
                retval = random.Next(minValue, maxValue);
            }
            catch (System.Exception e)
            {
                Utils.OutputMessage(false, MsgLevel.Error, "Utils -- GenerateRandomNumber", e.Message);
            }

            return retval;
        }
        #endregion
    }
}
