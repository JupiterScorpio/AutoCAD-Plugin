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
    public partial class ANRamp : Form
    {
        string strSelitem;
        public ANRamp()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (width_txt.Text != "" && strSelitem != "" && height_txt.Text!=""&&length_txt.Text!="")
            {
                ProsoftAcPlugin.Plugin.ANrmpitem = strSelitem;
                ProsoftAcPlugin.Plugin.ANRmpwidth = width_txt.Text + " mt. Wide ";
                ProsoftAcPlugin.Plugin.ANRmphght = height_txt.Text + " mt. Height ";
                ProsoftAcPlugin.Plugin.ANRmplngh=length_txt.Text+ " mt. Length ";
                ProsoftAcPlugin.Plugin.bANPge = true;
                this.Close();
            }
            else
                MessageBox.Show("Please input correct value", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            strSelitem = comboBox1.SelectedItem.ToString();
        }

        private void ANRamp_Load(object sender, EventArgs e)
        {

        }
    }
}
