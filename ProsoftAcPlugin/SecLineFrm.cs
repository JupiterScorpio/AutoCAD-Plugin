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
    public partial class SecLineFrm : Form
    {
        public SecLineFrm()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void multi_opt_CheckedChanged(object sender, EventArgs e)
        {
            msecName_txt.Enabled = true;
        }

        private void SecLineFrm_Load(object sender, EventArgs e)
        {
            this.single_opt.Checked = true;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void single_opt_CheckedChanged(object sender, EventArgs e)
        {
            msecName_txt.Enabled=false;
        }
    }
}
