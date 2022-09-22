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
    public partial class PlineCloseFrm : Form
    {
        public PlineCloseFrm()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Show_LineCloseResult()
        {            
            for (int i=0;i<Plugin.unclosedlinelyrNm.Count;i++)
            {
                string[] row = { Plugin.unclosedlinelyrNm[i], Plugin.unclosedlinePtStrt[i], Plugin.unclosedlinePtEnd[i] };
                var listViewItem = new ListViewItem(row);
                listView1.Items.Add(listViewItem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PlineCloseFrm_Load(object sender, EventArgs e)
        {

        }
    }
}
