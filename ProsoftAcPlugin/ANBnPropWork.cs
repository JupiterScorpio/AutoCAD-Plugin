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
    public partial class ANBnPropWork : Form
    {
        public ANBnPropWork()
        {
            InitializeComponent();
        }

        private void ANBnPropWork_Load(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Plugin.ANBNPbuilding = "";
            ProsoftAcPlugin.Plugin.ANBNPwing = "";
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Plugin.bANBNP = false;
            ProsoftAcPlugin.Plugin.bANBNPlnCnt = 0;
            this.Close();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (wing_txt.Text != "" && build_txt.Text != "")
            {
                ProsoftAcPlugin.Plugin.ANBNPbuilding = " ("+build_txt.Text+")";
                ProsoftAcPlugin.Plugin.ANBNPwing = wing_txt.Text;
                ProsoftAcPlugin.Plugin.bANBNP = false;
                ProsoftAcPlugin.Plugin.bANBNPlnCnt = 0;
                ProsoftAcPlugin.Commands.CallImplement();
                this.Close();
            }
            else
                MessageBox.Show("Please input correct value", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
        }
    }
}
