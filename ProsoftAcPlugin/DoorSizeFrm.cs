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
    public partial class DoorSizeFrm : Form
    {
        bool bheight, bwidth, bdepth;
        public DoorSizeFrm()
        {
            InitializeComponent();
            bheight = false;
            bwidth = false;
            bdepth = false;
        }

        private void DoorSizeFrm_Load(object sender, EventArgs e)
        {

        }
        private void width_txt_TextChanged(object sender, EventArgs e)
        {
            //bool enteredLetter = false;
            //Queue<char> text = new Queue<char>();
            //foreach (var ch in this.width_txt.Text)
            //{
            //    if (char.IsDigit(ch))
            //    {
            //        text.Enqueue(ch);
            //    }
            //    else
            //    {
            //        enteredLetter = true;
            //    }
            //}

            //if (enteredLetter)
            //{
            //    MessageBox.Show("Please enter only Number", "Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //    bwidth = true;
        }

        private void height_txt_TextChanged(object sender, EventArgs e)
        {     
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            Plugin.nCurwidth = Convert.ToSingle(width_txt.Text);
            Plugin.nCurheight = Convert.ToSingle(height_txt.Text);
            Plugin.nCurDepth= Convert.ToSingle(depth_txt.Text);
            Commands.InsdoorName = Name_txt.Text.ToUpper();
            doorrule tmpdoor = new doorrule();
            tmpdoor.pl = Commands.curPline;
            tmpdoor.height = Plugin.nCurheight;
            tmpdoor.width = Plugin.nCurwidth;
            Commands.adoorrule.Add(tmpdoor);
            this.Close();
        }

        private void Name_txt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void depth_txt_TextChanged(object sender, EventArgs e)
        {
            //bool enteredLetter = false;
            //Queue<char> text = new Queue<char>();
            //foreach (var ch in this.depth_txt.Text)
            //{
            //    if (char.IsDigit(ch))
            //    {
            //        text.Enqueue(ch);
            //    }
            //    else
            //    {
            //        enteredLetter = true;
            //    }
            //}

            //if (enteredLetter)
            //{
            //    MessageBox.Show("Please enter only Number", "Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //    bdepth = true;
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
