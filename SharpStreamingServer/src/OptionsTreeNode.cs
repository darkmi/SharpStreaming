using System.Windows.Forms;

namespace Simon.SharpStreamingServer
{
    /// <summary>
    /// A class that manages the options' nodes.
    /// </summary>
    public class OptionsTreeNode : TreeNode
    {
        /// <summary>
        /// The control that is related to the options tree node
        /// </summary>
        private BaseControl optionControl;

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        /// <value>The name of the option.</value>
        public string OptionName
        {
            get { return this.optionControl.OptionName; }
        }

        /// <summary>
        /// Gets the option control.
        /// </summary>
        /// <value>The option control.</value>
        public BaseControl OptionControl
        {
            get { return this.optionControl; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsTreeNode"/> class.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="relatedControl">The related control.</param>
        public OptionsTreeNode(string nodeName, BaseControl relatedControl) : base(nodeName)
        {
            this.optionControl = relatedControl;
        }
    }
}
