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
    public partial class WindowSizeFrm : Form
    {
        public WindowSizeFrm()
        {
            InitializeComponent();
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Plugin.nCurwidth = Convert.ToSingle(width_txt.Text);
            ProsoftAcPlugin.Plugin.nCurheight = Convert.ToSingle(height_txt.Text);
            ProsoftAcPlugin.Plugin.nCurDepth = Convert.ToSingle(depth_txt.Text);
            ProsoftAcPlugin.Commands.InswindName = Name_txt.Text.ToUpper();
            ProsoftAcPlugin.windowrule tmpwind = new ProsoftAcPlugin.windowrule();
            tmpwind.pl = ProsoftAcPlugin.Commands.curPline;
            tmpwind.height = ProsoftAcPlugin.Plugin.nCurheight;
            tmpwind.width = ProsoftAcPlugin.Plugin.nCurwidth;
            ProsoftAcPlugin.Commands.awindowrule.Add(tmpwind);
            this.Close();
        }
    }
}
