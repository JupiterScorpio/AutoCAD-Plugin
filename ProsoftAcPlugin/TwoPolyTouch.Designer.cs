namespace NBCLayers
{
    partial class TwoPolyTouch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TwoPolyTouch));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.down1 = new System.Windows.Forms.RadioButton();
            this.top1 = new System.Windows.Forms.RadioButton();
            this.right1 = new System.Windows.Forms.RadioButton();
            this.left1 = new System.Windows.Forms.RadioButton();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cncl = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.down1);
            this.groupBox1.Controls.Add(this.top1);
            this.groupBox1.Controls.Add(this.right1);
            this.groupBox1.Controls.Add(this.left1);
            this.groupBox1.Location = new System.Drawing.Point(18, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(117, 127);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PolyLine1 is to the";
            // 
            // down1
            // 
            this.down1.AutoSize = true;
            this.down1.Location = new System.Drawing.Point(16, 98);
            this.down1.Name = "down1";
            this.down1.Size = new System.Drawing.Size(93, 17);
            this.down1.TabIndex = 3;
            this.down1.TabStop = true;
            this.down1.Text = "Down Position";
            this.down1.UseVisualStyleBackColor = true;
            this.down1.CheckedChanged += new System.EventHandler(this.Down1_CheckedChanged);
            // 
            // top1
            // 
            this.top1.AutoSize = true;
            this.top1.Location = new System.Drawing.Point(17, 75);
            this.top1.Name = "top1";
            this.top1.Size = new System.Drawing.Size(84, 17);
            this.top1.TabIndex = 2;
            this.top1.TabStop = true;
            this.top1.Text = "Top Position";
            this.top1.UseVisualStyleBackColor = true;
            this.top1.CheckedChanged += new System.EventHandler(this.Top1_CheckedChanged);
            // 
            // right1
            // 
            this.right1.AutoSize = true;
            this.right1.Location = new System.Drawing.Point(17, 52);
            this.right1.Name = "right1";
            this.right1.Size = new System.Drawing.Size(90, 17);
            this.right1.TabIndex = 1;
            this.right1.TabStop = true;
            this.right1.Text = "Right Position";
            this.right1.UseVisualStyleBackColor = true;
            this.right1.CheckedChanged += new System.EventHandler(this.Right1_CheckedChanged);
            // 
            // left1
            // 
            this.left1.AutoSize = true;
            this.left1.Location = new System.Drawing.Point(17, 29);
            this.left1.Name = "left1";
            this.left1.Size = new System.Drawing.Size(83, 17);
            this.left1.TabIndex = 0;
            this.left1.TabStop = true;
            this.left1.Text = "Left Position";
            this.left1.UseVisualStyleBackColor = true;
            this.left1.CheckedChanged += new System.EventHandler(this.Left1_CheckedChanged);
            // 
            // btn_ok
            // 
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Location = new System.Drawing.Point(12, 152);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(61, 25);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.Btn_ok_Click);
            // 
            // btn_cncl
            // 
            this.btn_cncl.Location = new System.Drawing.Point(74, 152);
            this.btn_cncl.Name = "btn_cncl";
            this.btn_cncl.Size = new System.Drawing.Size(61, 25);
            this.btn_cncl.TabIndex = 3;
            this.btn_cncl.Text = "Cancel";
            this.btn_cncl.UseVisualStyleBackColor = true;
            this.btn_cncl.Click += new System.EventHandler(this.Btn_cncl_Click);
            // 
            // TwoPolyTouch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(152, 181);
            this.Controls.Add(this.btn_cncl);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TwoPolyTouch";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TwoPolyTouch";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton down1;
        private System.Windows.Forms.RadioButton top1;
        private System.Windows.Forms.RadioButton right1;
        private System.Windows.Forms.RadioButton left1;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cncl;
    }
}