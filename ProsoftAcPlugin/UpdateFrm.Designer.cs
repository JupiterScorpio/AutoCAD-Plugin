namespace NBCLayers
{
    partial class UpdateFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateFrm));
            this.autoupdatechk = new System.Windows.Forms.CheckBox();
            this.notfic_lbl = new System.Windows.Forms.Label();
            this.button_y = new System.Windows.Forms.Button();
            this.btn_n = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dwnloadprogress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // autoupdatechk
            // 
            this.autoupdatechk.AutoSize = true;
            this.autoupdatechk.Location = new System.Drawing.Point(23, 102);
            this.autoupdatechk.Name = "autoupdatechk";
            this.autoupdatechk.Size = new System.Drawing.Size(83, 17);
            this.autoupdatechk.TabIndex = 0;
            this.autoupdatechk.Text = "AutoUpdate";
            this.autoupdatechk.UseVisualStyleBackColor = true;
            this.autoupdatechk.CheckedChanged += new System.EventHandler(this.autoupdatechk_CheckedChanged);
            // 
            // notfic_lbl
            // 
            this.notfic_lbl.AutoSize = true;
            this.notfic_lbl.Location = new System.Drawing.Point(12, 20);
            this.notfic_lbl.Name = "notfic_lbl";
            this.notfic_lbl.Size = new System.Drawing.Size(217, 13);
            this.notfic_lbl.TabIndex = 1;
            this.notfic_lbl.Text = "An update of Preval version 2.6 is available. ";
            // 
            // button_y
            // 
            this.button_y.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button_y.Location = new System.Drawing.Point(201, 98);
            this.button_y.Name = "button_y";
            this.button_y.Size = new System.Drawing.Size(75, 23);
            this.button_y.TabIndex = 2;
            this.button_y.Text = "Yes";
            this.button_y.UseVisualStyleBackColor = true;
            // 
            // btn_n
            // 
            this.btn_n.Location = new System.Drawing.Point(282, 98);
            this.btn_n.Name = "btn_n";
            this.btn_n.Size = new System.Drawing.Size(75, 23);
            this.btn_n.TabIndex = 3;
            this.btn_n.Text = "No";
            this.btn_n.UseVisualStyleBackColor = true;
            this.btn_n.Click += new System.EventHandler(this.btn_n_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Do you want to download and install latest version? ";
            // 
            // dwnloadprogress
            // 
            this.dwnloadprogress.Location = new System.Drawing.Point(15, 69);
            this.dwnloadprogress.Name = "dwnloadprogress";
            this.dwnloadprogress.Size = new System.Drawing.Size(342, 10);
            this.dwnloadprogress.TabIndex = 5;
            // 
            // UpdateFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 129);
            this.Controls.Add(this.dwnloadprogress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_n);
            this.Controls.Add(this.button_y);
            this.Controls.Add(this.notfic_lbl);
            this.Controls.Add(this.autoupdatechk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateFrm";
            this.Text = "Update Preval";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autoupdatechk;
        private System.Windows.Forms.Label notfic_lbl;
        private System.Windows.Forms.Button button_y;
        private System.Windows.Forms.Button btn_n;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar dwnloadprogress;
    }
}