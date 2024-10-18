
namespace NBCLayers
{
    partial class MarginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarginForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_side2 = new System.Windows.Forms.Button();
            this.btn_side1 = new System.Windows.Forms.Button();
            this.btn_rear = new System.Windows.Forms.Button();
            this.btn_front = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_depth = new System.Windows.Forms.Button();
            this.btn_width = new System.Windows.Forms.Button();
            this.depth_txt = new System.Windows.Forms.TextBox();
            this.width_txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_side2);
            this.groupBox1.Controls.Add(this.btn_side1);
            this.groupBox1.Controls.Add(this.btn_rear);
            this.groupBox1.Controls.Add(this.btn_front);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 132);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mark Margins";
            // 
            // btn_side2
            // 
            this.btn_side2.Location = new System.Drawing.Point(277, 102);
            this.btn_side2.Name = "btn_side2";
            this.btn_side2.Size = new System.Drawing.Size(75, 23);
            this.btn_side2.TabIndex = 4;
            this.btn_side2.Text = "Side2>>";
            this.btn_side2.UseVisualStyleBackColor = true;
            this.btn_side2.Click += new System.EventHandler(this.btn_side2_Click);
            // 
            // btn_side1
            // 
            this.btn_side1.Location = new System.Drawing.Point(277, 73);
            this.btn_side1.Name = "btn_side1";
            this.btn_side1.Size = new System.Drawing.Size(75, 23);
            this.btn_side1.TabIndex = 3;
            this.btn_side1.Text = "Side1>>";
            this.btn_side1.UseVisualStyleBackColor = true;
            this.btn_side1.Click += new System.EventHandler(this.btn_side1_Click);
            // 
            // btn_rear
            // 
            this.btn_rear.Location = new System.Drawing.Point(277, 44);
            this.btn_rear.Name = "btn_rear";
            this.btn_rear.Size = new System.Drawing.Size(75, 23);
            this.btn_rear.TabIndex = 2;
            this.btn_rear.Text = "Rear>>";
            this.btn_rear.UseVisualStyleBackColor = true;
            this.btn_rear.Click += new System.EventHandler(this.btn_rear_Click);
            // 
            // btn_front
            // 
            this.btn_front.Location = new System.Drawing.Point(277, 16);
            this.btn_front.Name = "btn_front";
            this.btn_front.Size = new System.Drawing.Size(75, 23);
            this.btn_front.TabIndex = 1;
            this.btn_front.Text = "Front>>";
            this.btn_front.UseVisualStyleBackColor = true;
            this.btn_front.Click += new System.EventHandler(this.btn_front_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Click corresponding button to mark the margins.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_depth);
            this.groupBox2.Controls.Add(this.btn_width);
            this.groupBox2.Controls.Add(this.depth_txt);
            this.groupBox2.Controls.Add(this.width_txt);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(3, 158);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(362, 133);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Note:";
            // 
            // btn_depth
            // 
            this.btn_depth.Location = new System.Drawing.Point(272, 90);
            this.btn_depth.Name = "btn_depth";
            this.btn_depth.Size = new System.Drawing.Size(75, 23);
            this.btn_depth.TabIndex = 7;
            this.btn_depth.Text = "Plot Depth";
            this.btn_depth.UseVisualStyleBackColor = true;
            this.btn_depth.Click += new System.EventHandler(this.btn_depth_Click);
            // 
            // btn_width
            // 
            this.btn_width.Location = new System.Drawing.Point(272, 55);
            this.btn_width.Name = "btn_width";
            this.btn_width.Size = new System.Drawing.Size(75, 23);
            this.btn_width.TabIndex = 6;
            this.btn_width.Text = "Plot Width";
            this.btn_width.UseVisualStyleBackColor = true;
            this.btn_width.Click += new System.EventHandler(this.btn_width_Click);
            // 
            // depth_txt
            // 
            this.depth_txt.Location = new System.Drawing.Point(117, 91);
            this.depth_txt.Name = "depth_txt";
            this.depth_txt.Size = new System.Drawing.Size(100, 20);
            this.depth_txt.TabIndex = 5;
            // 
            // width_txt
            // 
            this.width_txt.Location = new System.Drawing.Point(117, 58);
            this.width_txt.Name = "width_txt";
            this.width_txt.Size = new System.Drawing.Size(100, 20);
            this.width_txt.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "PLOT DEPTH";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PLOT WIDTH";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(320, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Please Assign Plot Width and Plot Depth from Plot Poly End Points";
            // 
            // btn_ok
            // 
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Location = new System.Drawing.Point(283, 300);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(61, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // MarginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 333);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MarginForm";
            this.ShowInTaskbar = false;
            this.Text = "Margin";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_side2;
        private System.Windows.Forms.Button btn_side1;
        private System.Windows.Forms.Button btn_rear;
        private System.Windows.Forms.Button btn_front;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox depth_txt;
        private System.Windows.Forms.TextBox width_txt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_depth;
        private System.Windows.Forms.Button btn_width;
    }
}