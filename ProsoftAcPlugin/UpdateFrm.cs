using Microsoft.Win32;
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
using Microsoft.Win32;

namespace NBCLayers
{
    public partial class UpdateFrm : Form
    {
        public UpdateFrm()
        {
            InitializeComponent();
        }

        private void btn_n_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void autoupdatechk_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.bautoupdate = !Plugin.bautoupdate;
            //MessageBox.Show(Plugin.bautoupdate.ToString());
            //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Preval");
            //if (Plugin.bautoupdate)
            //{
            //    key.SetValue("Set1", "enable");
            //}else
            //    key.SetValue("Set1", "disable");
        }
    }
}
