using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
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
using Autodesk.AutoCAD.Runtime;
using System.Reflection;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using System.Collections.Specialized;
using System.IO;
using Exception = System.Exception;
using System.Text.RegularExpressions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Autodesk.AutoCAD.Colors;
using Excel = Microsoft.Office.Interop.Excel;
using AcadDocument = Autodesk.AutoCAD.ApplicationServices.Document;
using AcadWindows = Autodesk.AutoCAD.Windows;
using NBCLayers;
using System.Windows.Input;

namespace NBCLayers
{
    public partial class WDRFrm : Form
    {
        public WDRFrm()
        {
            InitializeComponent();
        }

        private void WDRFrm_Load(object sender, EventArgs e)
        {
            TreeNode Wnode = new TreeNode("Windows");
            treeView1.Nodes.Add(Wnode);
            int windex = 0;
            foreach (windowrule wrule in ProsoftAcPlugin.Commands.awindowrule)
            {   
                TreeNode childnode = new TreeNode("Window"+"--" + windex.ToString() + "->" + wrule.width.ToString()+" X "+wrule.height.ToString());
                Wnode.Nodes.Add(childnode);
                windex++;
            }
            TreeNode Dnode = new TreeNode("Doors");
            treeView1.Nodes.Add(Dnode);
            int dindex = 0;
            foreach (doorrule drule in ProsoftAcPlugin.Commands.adoorrule)
            {
                TreeNode childnode = new TreeNode("Door"+"--" + dindex.ToString() + "->" + drule.width.ToString() + " X " + drule.height.ToString());
                Dnode.Nodes.Add(childnode);
                dindex++;
            }
            TreeNode Rnode = new TreeNode("Rooms");
            treeView1.Nodes.Add(Rnode);
            int rindex = 0;
            //foreach (roomrule rrule in ProsoftAcPlugin.Commands.aroomrule)
            //{
            //    TreeNode childnode = new TreeNode("Room"+rindex.ToString()+"--"+rrule.width.ToString() + " X " + rrule.height.ToString());
            //    Wnode.Nodes.Add(childnode);
            //    rindex++;
            //}
            foreach(roomrule rrule in ProsoftAcPlugin.Commands.aroomrule)
            {
                double width = Math.Round(rrule.width, 2);
                double height = Math.Round(rrule.height, 2);
                TreeNode childnode = new TreeNode("Room" +  "--" +rindex.ToString()+"->"+ width.ToString() + " X " + height.ToString());
                Rnode.Nodes.Add(childnode);
                rindex++;
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string nodeText = treeView1.SelectedNode.Text;
            ErrorCauseDisplay(nodeText);
        }
        private void ErrorCauseDisplay(string str)
        {
            if (!str.Contains("--"))
                return;
            int pos = str.IndexOf("--");
            int pos1= str.IndexOf("->");
            string layername = str.Substring(0, pos);
            int number = Convert.ToInt32(str.Substring(pos + 2,pos1-pos-2));
            List<string> errStrList = new List<string>();
            switch(layername)
            {
                case "Window":
                    {
                        Document curdoc = Application.DocumentManager.MdiActiveDocument;
                        var database = curdoc.Database;
                        var ed = curdoc.Editor;
                        List<ObjectId> tmpobjlist = new List<ObjectId>();
                        tmpobjlist.Add(ProsoftAcPlugin.Commands.awindowrule[number].objid);
                        using (DocumentLock docLock = curdoc.LockDocument())
                        {
                            using (Transaction acTrans = database.TransactionManager.StartTransaction())
                            {
                                if (tmpobjlist.Count > 0)
                                {
                                    ed.SetImpliedSelection(tmpobjlist.ToArray());
                                    acTrans.Commit();
                                }
                            }
                            ed.UpdateScreen();
                        }
                        break;
                    }
                case "Door":
                    {
                        Document curdoc = Application.DocumentManager.MdiActiveDocument;
                        var database = curdoc.Database;
                        var ed = curdoc.Editor;
                        List<ObjectId> tmpobjlist = new List<ObjectId>();
                        tmpobjlist.Add(ProsoftAcPlugin.Commands.adoorrule[number].objid);
                        using (DocumentLock docLock = curdoc.LockDocument())
                        {
                            using (Transaction acTrans = database.TransactionManager.StartTransaction())
                            {
                                if (tmpobjlist.Count > 0)
                                {
                                    ed.SetImpliedSelection(tmpobjlist.ToArray());
                                    acTrans.Commit();
                                }
                            }
                            ed.UpdateScreen();
                        }
                        break;
                    }
                case "Room":
                    {
                        Document curdoc = Application.DocumentManager.MdiActiveDocument;
                        var database = curdoc.Database;
                        var ed = curdoc.Editor;
                        List<ObjectId> tmpobjlist = new List<ObjectId>();
                        //tmpobjlist.Add(ProsoftAcPlugin.Commands.aroomrule[number].objid);
                        tmpobjlist.Add(ProsoftAcPlugin.Commands.aroomrule[number].objid);
                        using (DocumentLock docLock = curdoc.LockDocument())
                        {
                            using (Transaction acTrans = database.TransactionManager.StartTransaction())
                            {
                                if (tmpobjlist.Count > 0)
                                {
                                    ed.SetImpliedSelection(tmpobjlist.ToArray());
                                    acTrans.Commit();
                                }
                            }
                            ed.UpdateScreen();
                        }
                        break;
                    }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
