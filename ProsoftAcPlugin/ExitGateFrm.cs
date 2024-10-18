using ProsoftAcPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBCLayers
{
    public partial class ExitGateFrm : Form
    {
        public ExitGateFrm()
        {
            InitializeComponent();
        }

        private void ExitGateFrm_Load(object sender, EventArgs e)
        {

        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            if(width_txt.Text!=""&& height_txt.Text!=""&& depth_txt.Text!=""&& Name_txt.Text!="")
            {
                Plugin.nCurwidth = Convert.ToSingle(width_txt.Text);
                Plugin.nCurheight = Convert.ToSingle(height_txt.Text);
                Plugin.nCurDepth = Convert.ToSingle(depth_txt.Text);
                Commands.InsdoorName = Name_txt.Text.ToUpper();
            }            
            this.Close();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
