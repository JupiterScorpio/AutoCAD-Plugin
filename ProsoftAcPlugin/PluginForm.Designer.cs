namespace ProsoftAcPlugin
{
    partial class PluginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginForm));
            this.rad_resedient = new System.Windows.Forms.RadioButton();
            this.rad_commercial = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rad_storage = new System.Windows.Forms.RadioButton();
            this.rad_business = new System.Windows.Forms.RadioButton();
            this.rad_mercantile = new System.Windows.Forms.RadioButton();
            this.rad_assembly = new System.Windows.Forms.RadioButton();
            this.rad_institutional = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.occupNumCtrl = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rad_resedient
            // 
            this.rad_resedient.AutoSize = true;
            this.rad_resedient.Location = new System.Drawing.Point(9, 19);
            this.rad_resedient.Name = "rad_resedient";
            this.rad_resedient.Size = new System.Drawing.Size(75, 17);
            this.rad_resedient.TabIndex = 0;
            this.rad_resedient.Text = "Residental";
            this.rad_resedient.UseVisualStyleBackColor = true;
            this.rad_resedient.CheckedChanged += new System.EventHandler(this.rad_resedient_CheckedChanged);
            // 
            // rad_commercial
            // 
            this.rad_commercial.AutoSize = true;
            this.rad_commercial.Location = new System.Drawing.Point(9, 47);
            this.rad_commercial.Name = "rad_commercial";
            this.rad_commercial.Size = new System.Drawing.Size(79, 17);
            this.rad_commercial.TabIndex = 1;
            this.rad_commercial.Text = "Commercial";
            this.rad_commercial.UseVisualStyleBackColor = true;
            this.rad_commercial.CheckedChanged += new System.EventHandler(this.rad_commercial_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rad_storage);
            this.groupBox1.Controls.Add(this.rad_business);
            this.groupBox1.Controls.Add(this.rad_mercantile);
            this.groupBox1.Controls.Add(this.rad_assembly);
            this.groupBox1.Controls.Add(this.rad_institutional);
            this.groupBox1.Controls.Add(this.rad_commercial);
            this.groupBox1.Controls.Add(this.rad_resedient);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 249);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Use";
            // 
            // rad_storage
            // 
            this.rad_storage.AutoSize = true;
            this.rad_storage.Location = new System.Drawing.Point(7, 187);
            this.rad_storage.Name = "rad_storage";
            this.rad_storage.Size = new System.Drawing.Size(137, 17);
            this.rad_storage.TabIndex = 8;
            this.rad_storage.Text = "Storage and Hazardous";
            this.rad_storage.UseVisualStyleBackColor = true;
            // 
            // rad_business
            // 
            this.rad_business.AutoSize = true;
            this.rad_business.Location = new System.Drawing.Point(9, 159);
            this.rad_business.Name = "rad_business";
            this.rad_business.Size = new System.Drawing.Size(133, 17);
            this.rad_business.TabIndex = 5;
            this.rad_business.Text = "Business and Industrial";
            this.rad_business.UseVisualStyleBackColor = true;
            // 
            // rad_mercantile
            // 
            this.rad_mercantile.AutoSize = true;
            this.rad_mercantile.Location = new System.Drawing.Point(9, 131);
            this.rad_mercantile.Name = "rad_mercantile";
            this.rad_mercantile.Size = new System.Drawing.Size(74, 17);
            this.rad_mercantile.TabIndex = 4;
            this.rad_mercantile.Text = "Mercantile";
            this.rad_mercantile.UseVisualStyleBackColor = true;
            // 
            // rad_assembly
            // 
            this.rad_assembly.AutoSize = true;
            this.rad_assembly.Location = new System.Drawing.Point(9, 103);
            this.rad_assembly.Name = "rad_assembly";
            this.rad_assembly.Size = new System.Drawing.Size(69, 17);
            this.rad_assembly.TabIndex = 3;
            this.rad_assembly.Text = "Assembly";
            this.rad_assembly.UseVisualStyleBackColor = true;
            this.rad_assembly.CheckedChanged += new System.EventHandler(this.rad_assembly_CheckedChanged);
            // 
            // rad_institutional
            // 
            this.rad_institutional.AutoSize = true;
            this.rad_institutional.Location = new System.Drawing.Point(9, 75);
            this.rad_institutional.Name = "rad_institutional";
            this.rad_institutional.Size = new System.Drawing.Size(78, 17);
            this.rad_institutional.TabIndex = 2;
            this.rad_institutional.Text = "Institutional";
            this.rad_institutional.UseVisualStyleBackColor = true;
            this.rad_institutional.CheckedChanged += new System.EventHandler(this.rad_institutional_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Occupancy Number :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_ok);
            this.groupBox2.Controls.Add(this.occupNumCtrl);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(5, 258);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 90);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Occupancy Info";
            // 
            // btn_ok
            // 
            this.btn_ok.Enabled = false;
            this.btn_ok.Location = new System.Drawing.Point(100, 57);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 5;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // occupNumCtrl
            // 
            this.occupNumCtrl.Enabled = false;
            this.occupNumCtrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occupNumCtrl.Location = new System.Drawing.Point(115, 30);
            this.occupNumCtrl.Name = "occupNumCtrl";
            this.occupNumCtrl.Size = new System.Drawing.Size(51, 21);
            this.occupNumCtrl.TabIndex = 4;
            this.occupNumCtrl.TextChanged += new System.EventHandler(this.occupNumCtrl_TextChanged);
            // 
            // PluginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(192, 353);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PluginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Use";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PluginForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rad_resedient;
        private System.Windows.Forms.RadioButton rad_commercial;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rad_institutional;
        private System.Windows.Forms.RadioButton rad_business;
        private System.Windows.Forms.RadioButton rad_mercantile;
        private System.Windows.Forms.RadioButton rad_assembly;
        private System.Windows.Forms.RadioButton rad_storage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.TextBox occupNumCtrl;
    }
}