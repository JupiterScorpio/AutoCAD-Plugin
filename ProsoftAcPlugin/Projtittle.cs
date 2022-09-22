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
    public partial class Projtittle : Form
    {
        public Projtittle()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Commands.InsProjstr = label1.Text + " " + textBox1.Text + " " + label2.Text + " " + textBox2.Text + label3.Text +
                textBox3.Text + " " + label4.Text + " " + textBox4.Text + " " + label5.Text + textBox5.Text + " " + label6.Text + " " + textBox6.Text + " "
                + label7.Text + " " + textBox7.Text + " " + label8.Text + " " + textBox8.Text + " " + label9.Text + " " + label10.Text + 
                " " + textBox9.Text;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
