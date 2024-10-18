﻿using ProsoftAcPlugin;
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
    public partial class FireGateFrm : Form
    {
        public FireGateFrm()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Name_txt.Text = listBox1.SelectedItem.ToString();
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            Plugin.nCurwidth = Convert.ToSingle(width_txt.Text);
            Plugin.nCurheight = Convert.ToSingle(height_txt.Text);
            Plugin.nCurDepth = Convert.ToSingle(depth_txt.Text);
            Commands.InsFiregateName = Name_txt.Text.ToUpper();
            this.Close();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}