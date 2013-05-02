namespace Simon.SharpStreaming.Core
{
    /// <summary>
    /// A class that keeps all kinds of constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// The library name.
        /// </summary>
        public static string LIB_NAME = "Sharp Streaming v";

        /// <summary>
        /// The library version.
        /// </summary>
        public static string LIB_VERSION = "1.0.0.1";

        /// <summary>
        /// The filter text of the media files.
        /// </summary>
        public static string FILTER_TEXT = "Video Files (*.avi; *.mov; *.mpg; *.mpeg; *.ts; *.wmv; *.vob; *.dat;)|*.avi; *.mov; *.mpg; *.mpeg; *.ts; *.wmv; *.vob; *.dat";

        /// <summary>
        /// The stop alway of time gap, at present it is set to "0.01" seconds.
        /// </summary>
        public const double TIMEGAP_STOPALWAY = 0.01;

        #region The Const Commands For RTSP
        /// <summary>
        /// RTSP command of OPTIONS
        /// </summary>
        public const string RTSP_CMD_OPTIONS = "OPTIONS";

        /// <summary>
        /// RTSP command of DESCRIBE
        /// </summary>
        public const string RTSP_CMD_DESCRIBE = "DESCRIBE";

        /// <summary>
        /// RTSP command of SETUP
        /// </summary>
        public const string RTSP_CMD_SETUP = "SETUP";

        /// <summary>
        /// RTSP command of PLAY
        /// </summary>
        public const string RTSP_CMD_PLAY = "PLAY";

        /// <summary>
        /// RTSP command of PAUSE
        /// </summary>
        public const string RTSP_CMD_PAUSE = "PAUSE";

        /// <summary>
        /// RTSP command of TEARDOWN
        /// </summary>
        public const string RTSP_CMD_TEARDOWN = "TEARDOWN";
        #endregion

        #region The Constants For RTSP
        /// <summary>
        /// RTSP user agent header
        /// </summary>
        public static readonly string USER_AGENT_HEADER = "RTSP Client -- Simon Huang";

        /// <summary>
        /// RTSP version of 1.0
        /// </summary>
        public static readonly string RTSP_HEADER_VERSION = "RTSP/1.0";

        /// <summary>
        /// RTSP status code of "200"
        /// </summary>
        public static readonly string RTSP_STATUS_CODE_OK = "200";

        /// <summary>
        /// RTSP allow commands
        /// </summary>
        public static readonly string RTSP_ALLOW_COMMAND = "OPTIONS, DESCRIBE, SETUP, PLAY, PAUSE, TEARDOWN";
        #endregion

        #region The Constants For Configuration
        /// <summary>
        /// The config file path.
        /// </summary>
        public static readonly string CONFIG_FILE_NAME = "\\Config.ini";

        /// <summary>
        /// The config section of "General".
        /// </summary>
        public static readonly string CONFIG_SECTION_GENERAL = "General";

        /// <summary>
        /// The config section of "Catalog".
        /// </summary>
        public static readonly string CONFIG_SECTION_CATALOG = "Catalog";

        /// <summary>
        /// The config key of close mode of the server.
        /// </summary>
        public static readonly string CONFIG_KEY_CLOSEMODE = "CloseMode";

        /// <summary>
        /// The config key of maximum connection count of the client session.
        /// </summary>
        public static readonly string CONFIG_KEY_MAXCONNECTIONCOUNT = "MaxConnectionCount";

        /// <summary>
        /// The config key of maximum time-out of the client session.
        /// </summary>
        public static readonly string CONFIG_KEY_MAXSESSIONTIMEOUT = "MaxSessionTimeout";

        /// <summary>
        /// The config key of server port.
        /// </summary>
        public static readonly string CONFIG_KEY_SERVERPORT = "ServerPortNumber";

        /// <summary>
        /// The config key of file catalog.
        /// </summary>
        public static readonly string CONFIG_KEY_FILECATALOG = "FileCatalog";
        #endregion
    }
}
