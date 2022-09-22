using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Exception = System.Exception;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Forms.VisualStyles;

namespace ProsoftAcPlugin
{
    public partial class NBCruleCheck : Form
    {
        private int chksel = -1;
        bool bsel;
        public NBCruleCheck()
        {
            InitializeComponent();
        }

        private void NBCruleCheck_Load(object sender, EventArgs e)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Plugin.allLayers = Commands.LayersToList(db);
            foreach (string str in Plugin.allLayers)
            {
                chklyr_list.Items.Add(str);
            }
            bsel = false;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if(!bsel)
            {
                MessageBox.Show("Rule Check");
            }
            else
            {
                this.Close();
            }
                
        }

        private void chklyr_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            chksel = chklyr_list.SelectedIndex;
            Plugin.str_srclyrname = Plugin.allLayers[chklyr_list.SelectedIndex];
            bsel = true;
        }
    }
}
