
namespace NBCLayers
{
    partial class SecLineFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecLineFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.secname_txt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.msecName_txt = new System.Windows.Forms.TextBox();
            this.single_opt = new System.Windows.Forms.RadioButton();
            this.multi_opt = new System.Windows.Forms.RadioButton();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // secname_txt
            // 
            this.secname_txt.Location = new System.Drawing.Point(99, 38);
            this.secname_txt.Name = "secname_txt";
            this.secname_txt.Size = new System.Drawing.Size(100, 20);
            this.secname_txt.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select Section Line:";
            // 
            // msecName_txt
            // 
            this.msecName_txt.Enabled = false;
            this.msecName_txt.Location = new System.Drawing.Point(134, 141);
            this.msecName_txt.Name = "msecName_txt";
            this.msecName_txt.Size = new System.Drawing.Size(100, 20);
            this.msecName_txt.TabIndex = 3;
            // 
            // single_opt
            // 
            this.single_opt.AutoSize = true;
            this.single_opt.Location = new System.Drawing.Point(30, 111);
            this.single_opt.Name = "single_opt";
            this.single_opt.Size = new System.Drawing.Size(77, 17);
            this.single_opt.TabIndex = 4;
            this.single_opt.TabStop = true;
            this.single_opt.Text = "Single Line";
            this.single_opt.UseVisualStyleBackColor = true;
            this.single_opt.CheckedChanged += new System.EventHandler(this.single_opt_CheckedChanged);
            // 
            // multi_opt
            // 
            this.multi_opt.AutoSize = true;
            this.multi_opt.Location = new System.Drawing.Point(30, 141);
            this.multi_opt.Name = "multi_opt";
            this.multi_opt.Size = new System.Drawing.Size(84, 17);
            this.multi_opt.TabIndex = 5;
            this.multi_opt.TabStop = true;
            this.multi_opt.Text = "Multiple Line";
            this.multi_opt.UseVisualStyleBackColor = true;
            this.multi_opt.CheckedChanged += new System.EventHandler(this.multi_opt_CheckedChanged);
            // 
            // btn_ok
            // 
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Location = new System.Drawing.Point(92, 198);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(62, 23);
            this.btn_ok.TabIndex = 6;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(172, 198);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(62, 23);
            this.btn_cancel.TabIndex = 7;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // SecLineFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 233);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.multi_opt);
            this.Controls.Add(this.single_opt);
            this.Controls.Add(this.msecName_txt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.secname_txt);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SecLineFrm";
            this.ShowInTaskbar = false;
            this.Text = "Section Line Dialog";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SecLineFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox secname_txt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox msecName_txt;
        private System.Windows.Forms.RadioButton single_opt;
        private System.Windows.Forms.RadioButton multi_opt;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
    }
}