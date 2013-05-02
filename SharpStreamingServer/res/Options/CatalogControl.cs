using System;
using System.Windows.Forms;

using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingServer
{
    public partial class CatalogControl : BaseControl
    {
        private string fileCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CatalogControl(string name) : base(name)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        public override void Initialize()
        {
            fileCatalog = MyConfig.GetConfigValue(Constants.CONFIG_SECTION_CATALOG, Constants.CONFIG_KEY_FILECATALOG);

            this.txtFileCatalog.Text = fileCatalog;
        }

        /// <summary>
        /// Applies the changes and saves them.
        /// </summary>
        public override void Apply()
        {
            fileCatalog = this.txtFileCatalog.Text;

            MyConfig.SetConfigValue(Constants.CONFIG_SECTION_CATALOG, Constants.CONFIG_KEY_FILECATALOG, fileCatalog);
        }

        /// <summary>
        /// Handles the Click event of the btnBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
            folderBrowserDlg.Description = "Please select a folder directory:";

            DialogResult dlgResult = folderBrowserDlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                string selectedPath = folderBrowserDlg.SelectedPath;
                if (string.IsNullOrEmpty(selectedPath))
                {
                    MessageBox.Show("Please select a folder!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    this.txtFileCatalog.Text = selectedPath;
                }
            }
        }
    }
}
