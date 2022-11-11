using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Exception = System.Exception;
using System.Text.RegularExpressions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Autodesk.AutoCAD.Colors;
using Excel = Microsoft.Office.Interop.Excel;
using AcadDocument = Autodesk.AutoCAD.ApplicationServices.Document;
using AcadWindows = Autodesk.AutoCAD.Windows;
using NBCLayers;
using System.Windows.Input;

namespace ProsoftAcPlugin
{
    public partial class RuleCheckForm : Form
    {
        public RuleCheckForm()
        {
            InitializeComponent();
            //MessageBox.Show("RuleCheck Result");
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
            if (Commands.errlist.Count == 0)
                textBox1.Text = "There are no Errors.";
            foreach (ruleError re in Commands.errlist)
            {
                if(re.errorCnt!=0)
                {
                    TreeNode node = new TreeNode(re.lyrname);
                    treeView1.Nodes.Add(node);
                    for (int i = 0; i < re.errorCnt; i++)
                    {
                        TreeNode childnode = new TreeNode(re.lyrname + "--" + i.ToString());
                        node.Nodes.Add(childnode);
                    }
                }
            }  
            
        }
        private void ErrorCauseDisplay(string str)
        {
            if (!str.Contains("--"))
                return;
            int pos = str.IndexOf("--");
            string layername = str.Substring(0, pos);
            int number =Convert.ToInt32( str.Substring(pos + 2));
            List<string> errStrList = new List<string>();
            
            foreach (ruleError err in Commands.errlist)
            {
                if (err.lyrname == layername)
                {
                    Document curdoc = Application.DocumentManager.MdiActiveDocument;
                    var database = curdoc.Database;
                    var ed = curdoc.Editor;
                    List<ObjectId> tmpobjlist = new List<ObjectId>();
                    //MessageBox.Show(err.objIdlist.Count.ToString());
                    tmpobjlist.Add(err.objIdlist[number]);
                    if (layername == "_Amenity")
                        tmpobjlist = err.objIdlist;
                    using (DocumentLock docLock = curdoc.LockDocument())
                    {
                        using (Transaction acTrans = database.TransactionManager.StartTransaction())
                        {
                            ed.SetImpliedSelection(tmpobjlist.ToArray());
                            acTrans.Commit();
                        }
                        ed.UpdateScreen();
                    }
                    
                    string strtemp = err.errcause;
                    while(strtemp!="")
                    {
                        int postrim = strtemp.IndexOf("-", 1);
                        if (postrim!=-1)
                        {
                            string strbuf = strtemp.Substring(0, postrim);
                            strtemp=strtemp.Remove(0, postrim);
                            errStrList.Add(strbuf);
                        }
                        else
                        {
                            errStrList.Add(strtemp);
                            strtemp = "";
                        }                        
                    }
                    textBox1.Text = errStrList[number];
                }                    
            }
        }
    }
}
