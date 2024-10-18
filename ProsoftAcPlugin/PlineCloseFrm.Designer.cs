
namespace ProsoftAcPlugin
{
    partial class PlineCloseFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlineCloseFrm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.LayerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjectId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Handle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LayerName,
            this.ObjectId,
            this.Handle});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(-1, -1);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(512, 373);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // LayerName
            // 
            this.LayerName.Text = "LayerName";
            this.LayerName.Width = 113;
            // 
            // ObjectId
            // 
            this.ObjectId.Text = "ObjectId";
            this.ObjectId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ObjectId.Width = 190;
            // 
            // Handle
            // 
            this.Handle.Text = "Handle";
            this.Handle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Handle.Width = 209;
            // 
            // PlineCloseFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 370);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlineCloseFrm";
            this.ShowInTaskbar = false;
            this.Text = "PlineCloseCheck";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PlineCloseFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader LayerName;
        private System.Windows.Forms.ColumnHeader ObjectId;
        private System.Windows.Forms.ColumnHeader Handle;
    }
}