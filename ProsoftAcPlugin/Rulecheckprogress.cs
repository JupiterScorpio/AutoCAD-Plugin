using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
//using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace NBCLayers
{
    public partial class Rulecheckprogress : Form, IProgressUpdate
    {
        public Rulecheckprogress()
        {
            InitializeComponent();
            this.Location = new Point((int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width 
                , (int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            
        }

        private void Rulecheckprogress_Load(object sender, EventArgs e)
        {
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 100;
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public void ReportProgress(int nPercentage, string msg)
        {
            try
            {
                if (progressBar1.Value > (progressBar1.Maximum - 1))
                {
                    this.Close();
                }

                if (nPercentage == 100)
                {
                    var diff = nPercentage - progressBar1.Value;
                    progressBar1.Value += diff;
                    label1.Text = msg;
                    System.Windows.Forms.Application.DoEvents(); //keep form active in every loop
                    return;
                }
                if (nPercentage < 1)
                    nPercentage = 1;
                progressBar1.Value = nPercentage;
                label1.Text = msg;
                label2.Text = nPercentage.ToString() + "%";
                System.Windows.Forms.Application.DoEvents(); //keep form active in every loop
            }
            catch 
            {
                throw;
            }
        }
        public void disposeDialogBox()
        {
            this.Hide();
            this.Close();
        }
        public Point2d GetCurrentViewSize()
        {
                //Get current view height
                double h = (double)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("VIEWSIZE");
   
                //Get current view width,
                //by calculate current view's width-height ratio
              Point2d screen = (Point2d)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("SCREENSIZE");
              double w = h * (screen.X / screen.Y);
   
                return new Point2d(w, h);
        }
}
    public interface IProgressUpdate
    {
        void ReportProgress(int nPercentage, string msg);
    }
}
