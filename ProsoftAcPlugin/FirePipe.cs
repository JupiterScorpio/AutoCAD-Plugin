using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProsoftAcPlugin;

namespace NBCLayers
{
    public partial class FirePipe : Form
    {
        int selindex = 0;
        public FirePipe()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {

            ProsoftAcPlugin.Plugin.linewgt = selindex;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selindex= listBox1.SelectedIndex;
        }

        private void btn_cncl_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
