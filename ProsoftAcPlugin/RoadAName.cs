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
    public partial class RoadAName : Form
    {
        public RoadAName()
        {
            InitializeComponent();
            ProsoftAcPlugin.Plugin.ANexistRdwidth ="";
            ProsoftAcPlugin.Plugin.ANpropRdwidth = "";
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (existing_txt.Text != "" && Prop_txt.Text != "")
            {
                ProsoftAcPlugin.Plugin.ANexistRdwidth = existing_txt.Text + " MT WIDE EXISTING";
                ProsoftAcPlugin.Plugin.ANpropRdwidth = Prop_txt.Text + " MT WIDE PROPOSED";
                ProsoftAcPlugin.Plugin.bANRd = true;
                this.Close();
            }
            else
                MessageBox.Show("Please input correct value", "Error", MessageBoxButtons.OKCancel,MessageBoxIcon.Error);            
        }

        private void btn_cncl_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void existing_txt_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void Prop_txt_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
