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
    public partial class WindowSize : Form
    {
        public WindowSize()
        {
            InitializeComponent();
        }

        private void width_txt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void height_txt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            Plugin.nCurwidth = Convert.ToInt32(width_txt.Text);
            Plugin.nCurheight = Convert.ToInt32(height_txt.Text);
            windowrule tmpwindow =new windowrule();
            tmpwindow.pl = Commands.curPLine;
            tmpwindow.height = Plugin.nCurheight;
            tmpwindow.width = Plugin.nCurwidth;
            Commands.awindowrule.Add(tmpwindow);
            this.Close();
        }

        private void WindowSize_Load(object sender, EventArgs e)
        {

        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
