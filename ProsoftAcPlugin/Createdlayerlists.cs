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
    public partial class Createdlayerlists : Form
    {
        public Createdlayerlists()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Commands.makingLayers();
            this.Close();
        }

        private void Createdlayerlists_Load(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode("Layers Created");
            treeView1.Nodes.Add(node);
            foreach(string st in ProsoftAcPlugin.Plugin.lyrName)
            {
                TreeNode childnode = new TreeNode(st);
                node.Nodes.Add(childnode);
            }
        }
    }
}
