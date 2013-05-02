namespace Simon.SharpStreamingServer
{
    partial class GeneralControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numServerPort = new System.Windows.Forms.NumericUpDown();
            this.numMaxTimeout = new System.Windows.Forms.NumericUpDown();
            this.numMaxConnCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbQuit = new System.Windows.Forms.RadioButton();
            this.rbMin = new System.Windows.Forms.RadioButton();
            this.lblWhenCloseInfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numServerPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnCount)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbQuit);
            this.groupBox1.Controls.Add(this.rbMin);
            this.groupBox1.Controls.Add(this.lblWhenCloseInfo);
            this.groupBox1.Controls.Add(this.numServerPort);
            this.groupBox1.Controls.Add(this.numMaxTimeout);
            this.groupBox1.Controls.Add(this.numMaxConnCount);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 200);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // numServerPort
            // 
            this.numServerPort.Location = new System.Drawing.Point(144, 153);
            this.numServerPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numServerPort.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numServerPort.Name = "numServerPort";
            this.numServerPort.Size = new System.Drawing.Size(100, 21);
            this.numServerPort.TabIndex = 6;
            this.numServerPort.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numMaxTimeout
            // 
            this.numMaxTimeout.Location = new System.Drawing.Point(144, 117);
            this.numMaxTimeout.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numMaxTimeout.Name = "numMaxTimeout";
            this.numMaxTimeout.Size = new System.Drawing.Size(100, 21);
            this.numMaxTimeout.TabIndex = 6;
            // 
            // numMaxConnCount
            // 
            this.numMaxConnCount.Location = new System.Drawing.Point(144, 79);
            this.numMaxConnCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxConnCount.Name = "numMaxConnCount";
            this.numMaxConnCount.Size = new System.Drawing.Size(100, 21);
            this.numMaxConnCount.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Default Server Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Max Session Timeout:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Max Connection Count:";
            // 
            // rbQuit
            // 
            this.rbQuit.AutoSize = true;
            this.rbQuit.Location = new System.Drawing.Point(161, 37);
            this.rbQuit.Name = "rbQuit";
            this.rbQuit.Size = new System.Drawing.Size(113, 16);
            this.rbQuit.TabIndex = 16;
            this.rbQuit.Text = "Quit the server";
            this.rbQuit.UseVisualStyleBackColor = true;
            // 
            // rbMin
            // 
            this.rbMin.AutoSize = true;
            this.rbMin.Checked = true;
            this.rbMin.Location = new System.Drawing.Point(7, 37);
            this.rbMin.Name = "rbMin";
            this.rbMin.Size = new System.Drawing.Size(113, 16);
            this.rbMin.TabIndex = 15;
            this.rbMin.TabStop = true;
            this.rbMin.Text = "Minimum to tray";
            this.rbMin.UseVisualStyleBackColor = true;
            // 
            // lblWhenCloseInfo
            // 
            this.lblWhenCloseInfo.AutoSize = true;
            this.lblWhenCloseInfo.Location = new System.Drawing.Point(6, 17);
            this.lblWhenCloseInfo.Name = "lblWhenCloseInfo";
            this.lblWhenCloseInfo.Size = new System.Drawing.Size(197, 12);
            this.lblWhenCloseInfo.TabIndex = 14;
            this.lblWhenCloseInfo.Text = "After close button was clicking:";
            // 
            // GeneralControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.groupBox1);
            this.Name = "GeneralControl";
            this.Size = new System.Drawing.Size(390, 265);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numServerPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numMaxConnCount;
        private System.Windows.Forms.NumericUpDown numServerPort;
        private System.Windows.Forms.NumericUpDown numMaxTimeout;
        private System.Windows.Forms.RadioButton rbQuit;
        private System.Windows.Forms.RadioButton rbMin;
        private System.Windows.Forms.Label lblWhenCloseInfo;
    }
}
