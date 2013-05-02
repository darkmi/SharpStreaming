using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A config class that includes the operations of reading and/or
    /// writing from/to the config file of ini. And also, it includes
    /// loading the default config values.
    /// </summary>
    public class Config
    {
        #region Dll Import Declare
        /// <summary>
        /// Writes the private profile string.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The number of characters to be written.</returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string value, string filePath);

        /// <summary>
        /// Gets the private profile string.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="def">The def.</param>
        /// <param name="value">The value.</param>
        /// <param name="size">The size.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The number of characters copied to the buffer, not including the terminating null character.</returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section, string key, string def, StringBuilder value, int size, string filePath);
        #endregion

        #region Private Member Fields
        /// <summary>
        /// A table of the default config values.
        /// </summary>
        private Dictionary<string, string> defaultConfigValueTable;

        /// <summary>
        /// The path of the config file.
        /// </summary>
        private string configFilePath;
        #endregion

        #region Class Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
            defaultConfigValueTable = new Dictionary<string, string>();

            configFilePath = System.Environment.CurrentDirectory + Constants.CONFIG_FILE_NAME;

            this.LoadDefaultConfigValue();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the config value.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public string GetConfigValue(string section, string key)
        {
            string value = string.Empty;

            value = ReadFromIniFile(section, key);
            if (string.IsNullOrEmpty(value))
            {
                value = GetDefaultConfigValue(key);
            }

            return value;
        }

        /// <summary>
        /// Sets the config value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetConfigValue<T>(string section, string key, T value)
        {
            WriteToIniFile(section, key, value.ToString());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads the default config value.
        /// </summary>
        private void LoadDefaultConfigValue()
        {
            defaultConfigValueTable.Add(Constants.CONFIG_KEY_MAXCONNECTIONCOUNT, "500");
            defaultConfigValueTable.Add(Constants.CONFIG_KEY_MAXSESSIONTIMEOUT, "60000");
            defaultConfigValueTable.Add(Constants.CONFIG_KEY_SERVERPORT, "8554");
            defaultConfigValueTable.Add(Constants.CONFIG_KEY_FILECATALOG, "d:\\");
        }

        /// <summary>
        /// Gets the default config value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The default value.</returns>
        private string GetDefaultConfigValue(string key)
        {
            string defaultValue = string.Empty;

            lock (defaultConfigValueTable)
            {
                if (defaultConfigValueTable.ContainsKey(key))
                {
                    defaultValue = defaultConfigValueTable[key];
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Reads from .ini file.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        private string ReadFromIniFile(string section, string key)
        {
            StringBuilder sb = new StringBuilder(255);

            int i = GetPrivateProfileString(section, key, string.Empty, sb, 255, configFilePath);

            return sb.ToString();
        }

        /// <summary>
        /// Writes to .ini file.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void WriteToIniFile(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, configFilePath);
        }
        #endregion
    }
}
