using System;
using System.Windows.Forms;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Exception = System.Exception;
using System.Collections.Generic;
using System.Text;
using NBCLayers;

namespace ProsoftAcPlugin
{
    public partial class PluginForm : Form
    {        
        public static int occNum;
        public PluginForm()
        {
            InitializeComponent();
        }

        private void PluginForm_Load(object sender, EventArgs e)
        {
            
        }       

        private void rad_resedient_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.usestate  = (uint)Plugin.use.Residential;
            Commands.bNewproj = true;
            Commands.AddDoc();
            var crtedlyrsfrm = new NBCLayers.Createdlayerlists();
            crtedlyrsfrm.Show();
            this.Close();
        }

        private void rad_commercial_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.usestate = (uint)Plugin.use.Commercial;
            occupNumCtrl.Enabled = true;
        }

        private void rad_institutional_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.usestate = (uint)Plugin.use.Institutional;
            occupNumCtrl.Enabled = true;
        }
        
        private void rad_assembly_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.usestate = (uint)Plugin.use.Assembly;
            occupNumCtrl.Enabled = true;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if(occupNumCtrl.Text!="")
            {
                occNum = Convert.ToInt32(occupNumCtrl.Text);
            }
            else
                occNum = 0;
            Commands.bNewproj = true;
            Commands.AddDoc();
            var crtedlyrsfrm = new NBCLayers.Createdlayerlists();
            crtedlyrsfrm.Show();
            this.Close();
        }

        private void occupNumCtrl_TextChanged(object sender, EventArgs e)
        {            
            bool enteredLetter = false;
            Queue<char> text = new Queue<char>();
            foreach (var ch in this.occupNumCtrl.Text)
            {
                if (char.IsDigit(ch))
                {
                    text.Enqueue(ch);
                }
                else
                {
                    enteredLetter = true;
                }
            }

            if (enteredLetter)
            {
                MessageBox.Show("Please enter only Number", "Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }else
                btn_ok.Enabled = true;
        } 
    }
}
