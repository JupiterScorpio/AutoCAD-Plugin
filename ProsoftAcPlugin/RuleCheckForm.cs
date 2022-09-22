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
    public partial class RuleCheckForm : Form
    {
        public RuleCheckForm()
        {
            InitializeComponent();
        }

        private void treeView1_NodeMouseClick(object sender, TreeViewEventArgs e)
        {
            string nodeText = treeView1.SelectedNode.Text;
            ErrorCauseDisplay(nodeText);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RuleCheckForm_Load(object sender, EventArgs e)
        {
            foreach(ruleError re in Commands.errlist)
            {
                TreeNode node = new TreeNode(re.lyrname);
                treeView1.Nodes.Add(node);
                for (int i=0;i<re.errorCnt;i++)
                {
                    TreeNode childnode = new TreeNode(re.lyrname+"--"+i.ToString());
                    node.Nodes.Add(childnode);
                }
                if (Commands.errlist.Count == 0)
                    textBox1.Text = "There are no Errors.";
            }  
            
        }
        private void ErrorCauseDisplay(string str)
        {
            if (!str.Contains("--"))
                return;
            int pos = str.IndexOf("--");
            string layername = str.Substring(0, pos);
            int number =Convert.ToInt32( str.Substring(pos + 2));
            switch(layername)
            {
                case "_Window":
                    textBox1.Text = Commands.windowerrcause[number];
                    break;
                case "_Room":
                    textBox1.Text = Commands.roomerrcause[number];
                    break;
            }
        }
    }
}
