using System;
using System.Windows.Forms;

namespace Simon.SharpStreamingServer
{
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            InitializeSettingsForm();
        }

        /// <summary>
        /// Initializes the settings form.
        /// </summary>
        private void InitializeSettingsForm()
        {
            OptionsTreeNode optionsNode = new OptionsTreeNode("General", new GeneralControl("General Setting"));
            AddOptionsItem(optionsNode);

            optionsNode = new OptionsTreeNode("Catalog", new CatalogControl("Catalog Setting"));
            AddOptionsItem(optionsNode);
        }

        /// <summary>
        /// Adds the options item.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddOptionsItem(TreeNode node)
        {
            if (tvOptions.Nodes.Count == 0)
            {
                tvOptions.Nodes.Add(node);
                tvOptions.SelectedNode = node;
                tvOptions.Select();
            }
            else
            {
                tvOptions.Nodes.Add(node);
            }

            BaseControl control = ((OptionsTreeNode)node).OptionControl;
            control.Initialize();
        }

        /// <summary>
        /// Removes the options item.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveOptionsItem(string name)
        {
            if (tvOptions.Nodes == null)
            {
                return;
            }

            foreach (TreeNode node in tvOptions.Nodes)
            {
                if (string.Equals(node.Text, name))
                {
                    tvOptions.Nodes.Remove(node);
                    if (node is OptionsTreeNode)
                    {
                        BaseControl control = ((OptionsTreeNode)node).OptionControl;
                        if (pnlMain.Controls.Contains((control)))
                        {
                            pnlMain.Controls.RemoveAt(0);
                        }
                    }

                    break;
                }

                this.RemoveOptionsItem(name);
            }
        }

        /// <summary>
        /// Sets the label text.
        /// </summary>
        /// <param name="labelText">The label text.</param>
        private void SetLabelText(string labelText)
        {
            this.lblOptionsName.Text = labelText;
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tvOptions.Nodes == null)
            {
                return;
            }

            foreach (TreeNode node in tvOptions.Nodes)
            {
                if (node is OptionsTreeNode)
                {
                    BaseControl control = ((OptionsTreeNode)node).OptionControl;
                    control.Apply();
                }
            }

            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the AfterSelect event of the tvOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void tvOptions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is OptionsTreeNode)
            {
                // Removes any controls:
                if (pnlMain.Controls.Count > 0)
                {
                    pnlMain.Controls.RemoveAt(0);
                }

                BaseControl control = ((OptionsTreeNode)e.Node).OptionControl;

                // Adds the current panel to the main panel:
                pnlMain.Controls.Add(control);

                // Sets the name of the panel:
                this.SetLabelText(((OptionsTreeNode)e.Node).OptionName);
            }
        }
    }
}
