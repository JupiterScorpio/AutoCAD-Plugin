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
    public partial class FloorNameForm : Form
    {
        string strfloorsectionname,strfloorname;
        public FloorNameForm()
        {
            InitializeComponent();
        }

        private void FloorNameForm_Load(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            ProsoftAcPlugin.Plugin.bflrReassign = false;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Commands.tmpfloorsectionName = strfloorsectionname;
            ProsoftAcPlugin.Commands.tmpfloorName = strfloorname;
            this.Close();
            ProsoftAcPlugin.Plugin.bflrReassign = true;
            MessageBox.Show("First Select a FloorInSection Layer Polyline", "FloorInSection PolyLine", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void chk_typical_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_typical.Checked)
            {
                btn_0.Enabled = true;
                btn_1.Enabled = true;
                btn_2.Enabled = true;
                btn_3.Enabled = true;
                btn_4.Enabled = true;
                btn_5.Enabled = true;
                btn_6.Enabled = true;
                btn_7.Enabled = true;
                btn_8.Enabled = true;
                btn_9.Enabled = true;
            }
            else
            {
                btn_0.Enabled = false;
                btn_1.Enabled = false;
                btn_2.Enabled = false;
                btn_3.Enabled = false;
                btn_4.Enabled = false;
                btn_5.Enabled = false;
                btn_6.Enabled = false;
                btn_7.Enabled = false;
                btn_8.Enabled = false;
                btn_9.Enabled = false;
            }
        }

        private void chk_mezzanine_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_mezzanine.Checked)
            {
                btn_comma.Enabled = true;
                btn_hypen.Enabled = true;
                btn_and.Enabled = true;
            }else
            {
                btn_comma.Enabled = false;
                btn_hypen.Enabled = false;
                btn_and.Enabled = false;
            }
        }

        private void cmb_floorname_SelectedIndexChanged(object sender, EventArgs e)
        {
            strfloorsectionname = cmb_floorname.SelectedItem.ToString()+" FLOOR";
            strfloorname = cmb_floorname.SelectedItem.ToString() + " FLOOR PLAN";
        }
    }
}
