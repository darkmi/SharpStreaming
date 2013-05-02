using System;
using System.Windows.Forms;
using Simon.SharpStreaming.Core;

namespace Simon.SharpStreamingClient
{
    public partial class OpenForm : Form
    {
        #region Private Member Fields
        private bool isLocalFile;
        private string fileNameOrUrl;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether this instance is local file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is local file; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalFile
        {
            get { return this.isLocalFile; }
        }

        /// <summary>
        /// Gets the file name or URL.
        /// </summary>
        /// <value>The file name or URL.</value>
        public string FileNameOrUrl
        {
            get { return this.fileNameOrUrl; }
        }
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenForm"/> class.
        /// </summary>
        public OpenForm()
        {
            InitializeComponent();

            UpdateControlStatus();
        }

        /// <summary>
        /// Handles the Click event of the btnBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();

            openFileDlg.Filter = Constants.FILTER_TEXT;
            openFileDlg.Title = "Open video file";

            DialogResult dlgResult = openFileDlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(openFileDlg.FileName))
                {
                    MessageBox.Show("The file name must not be empty, please open again!", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                this.txtLocalFile.Text = openFileDlg.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.rbLocal.Checked)
            {
                if (string.IsNullOrEmpty(this.txtLocalFile.Text.Trim()))
                {
                    MessageBox.Show("The local file name must not be empty, please browse a file!", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }

                isLocalFile = true;
                fileNameOrUrl = this.txtLocalFile.Text.Trim();
            }
            else if (this.rbUrl.Checked)
            {
                if (string.IsNullOrEmpty(this.txtUrl.Text.Trim()))
                {
                    MessageBox.Show("The network url must not be empty, please input again!", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }

                isLocalFile = false;
                fileNameOrUrl = this.txtUrl.Text.Trim();
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the rbLocal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void rbLocal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStatus();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the rbUrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void rbUrl_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStatus();
        }

        /// <summary>
        /// Updates the control status.
        /// </summary>
        private void UpdateControlStatus()
        {
            if (this.rbLocal.Checked)
            {
                this.txtLocalFile.Enabled = true;
                this.txtUrl.Enabled = false;
                this.btnBrowse.Enabled = true;
            }
            else
            {
                this.txtLocalFile.Enabled = false;
                this.txtUrl.Enabled = true;
                this.btnBrowse.Enabled = false;
            }
        }
    }
}
