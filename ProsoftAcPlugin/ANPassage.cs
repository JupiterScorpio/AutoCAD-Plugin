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
    public partial class ANPassage : Form
    {
        string strSelitem;
        public ANPassage()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (width_txt.Text != "" && strSelitem != "")
            {
                ProsoftAcPlugin.Plugin.ANPgeitem = strSelitem;
                ProsoftAcPlugin.Plugin.ANPgewidth = width_txt.Text + " mt. Wide ";
                ProsoftAcPlugin.Plugin.bANPge = true;
                this.Close();
            }
            else
                MessageBox.Show("Please input correct value", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            strSelitem = comboBox1.SelectedItem.ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
