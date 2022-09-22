using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProsoftAcPlugin
{
    public partial class LayerRenameForm : Form
    {
        private int srcsel = -1, dstsel = -1;
        public LayerRenameForm()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if(Plugin.str_srclyrname == ""|| Plugin.str_dstlyrname == "")
            {
                MessageBox.Show("Select correct layer", "Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            else
            {
                Plugin.b_renamelyr = true;
                Commands.ChangeLayerName(Plugin.str_srclyrname, Plugin.str_dstlyrname);
                this.Close();
            }            
        }

        private void LayerRenameForm_Load(object sender, EventArgs e)
        {
            foreach(string str in Plugin.differentlyrs)
            {
                srclyr_list.Items.Add(str);
            }
            foreach(string str in Plugin.lyrName)
            {
                dstlyr_list.Items.Add(str);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void srclyr_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            srcsel = srclyr_list.SelectedIndex;
            Plugin.str_srclyrname = Plugin.differentlyrs[srclyr_list.SelectedIndex];
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void rename_opt_CheckedChanged(object sender, EventArgs e)
        {
            dstlyr_list.Enabled = false;
            textBox1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Plugin.str_dstlyrname = textBox1.Text;
        }

        private void dstlyr_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            dstsel = dstlyr_list.SelectedIndex;
            Plugin.str_dstlyrname = Plugin.lyrName[dstlyr_list.SelectedIndex];
        }
    }
}
