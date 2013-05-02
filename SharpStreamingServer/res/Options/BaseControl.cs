using System.Windows.Forms;

using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingServer
{
    public partial class BaseControl : UserControl
    {
        /// <summary>
        /// Declares the object of Config.
        /// </summary>
        private Config config;

        /// <summary>
        /// The name of the option.
        /// </summary>
        private readonly string optionName;

        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <value>The config.</value>
        public Config MyConfig
        {
            get { return this.config; }
        }

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        /// <value>The name of the option.</value>
        public string OptionName
        {
            get { return this.optionName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseControl"/> class.
        /// </summary>
        public BaseControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BaseControl(string name) : base()
        {
            this.config = new Config();

            this.optionName = name;
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        public virtual void Initialize()
        {
            // Implementation by default.
        }

        /// <summary>
        /// Applies the changes and saves them.
        /// </summary>
        public virtual void Apply()
        {
            // Implementation by default.
        }
    }
}
