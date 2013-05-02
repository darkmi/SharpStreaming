using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingServer
{
    public partial class GeneralControl : BaseControl
    {
        /// <summary>
        /// The close mode of the server.
        /// </summary>
        private string closeMode;

        /// <summary>
        /// The maximum connection count.
        /// </summary>
        private int maxConnectionCount;

        /// <summary>
        /// The maximum session time-out.
        /// </summary>
        private int maxSessionTimeout;

        /// <summary>
        /// The server port.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public GeneralControl(string name) : base(name)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        public override void Initialize()
        {
            string result = MyConfig.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_CLOSEMODE);
            closeMode = result;

            result = MyConfig.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXCONNECTIONCOUNT);
            maxConnectionCount = Utils.StringToInteger(result);

            result = MyConfig.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXSESSIONTIMEOUT);
            maxSessionTimeout = Utils.StringToInteger(result);

            result = MyConfig.GetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_SERVERPORT);
            serverPort = Utils.StringToInteger(result);

            if (closeMode.Equals("0"))
            {
                this.rbMin.Checked = true;
            }
            else
            {
                this.rbQuit.Checked = true;
            }

            this.numMaxConnCount.Value = (decimal)maxConnectionCount;
            this.numMaxTimeout.Value = (decimal)maxSessionTimeout;
            this.numServerPort.Value = (decimal)serverPort;
        }

        /// <summary>
        /// Applies the changes and saves them.
        /// </summary>
        public override void Apply()
        {
            if (this.rbMin.Checked)
            {
                closeMode = "0";
            }
            else if (this.rbQuit.Checked)
            {
                closeMode = "1";
            }

            maxConnectionCount = (int)this.numMaxConnCount.Value;
            maxSessionTimeout = (int)this.numMaxTimeout.Value;
            serverPort = (int)this.numServerPort.Value;

            MyConfig.SetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_CLOSEMODE, closeMode);
            MyConfig.SetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXCONNECTIONCOUNT, maxConnectionCount);
            MyConfig.SetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_MAXSESSIONTIMEOUT, maxSessionTimeout);
            MyConfig.SetConfigValue(Constants.CONFIG_SECTION_GENERAL, Constants.CONFIG_KEY_SERVERPORT, serverPort);
        }
    }
}
