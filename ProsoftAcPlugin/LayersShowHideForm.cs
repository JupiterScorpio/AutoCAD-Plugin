using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Forms.VisualStyles;

namespace ProsoftAcPlugin
{
    public partial class LayersShowHideForm : Form
    {
        List<string> allayrs = new List<string>();
        List<CheckBox> chklist = new List<CheckBox>();
        List<string> onlyrs = new List<string>();
        List<string> allonstrlist = new List<string>();
        List<string> alloffstrlist = new List<string>();
        public LayersShowHideForm()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.GetChklist();
            Commands.TurnOnLayers(allonstrlist,alloffstrlist);
            this.Close();
        }

        private void LayersShowHideForm_Load(object sender, EventArgs e)
        {
            bool overwidth=false;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            allayrs = Commands.LayersToList(acCurDb);
            Plugin.allLayers = allayrs;
            Point curpos = new Point(30, 30);
            int maxlen = 0, maxnum=0 ;
            for (int i = 0; i < allayrs.Count; i++)
            {
                CheckBox chk = new CheckBox();
                chk.Left = curpos.X;
                chk.Top = curpos.Y;
                chk.Text = allayrs[i];
                if (maxlen < allayrs[i].Length)
                {
                    maxlen = allayrs[i].Length;
                    maxnum = i;
                }                    
                chk.Visible = true;
                chk.AutoSize = true;
                chklist.Add(chk);
                curpos.X = curpos.X;
                curpos.Y = curpos.Y + (int)(chk.Height * 1.3);
                if (curpos.Y>(int)(this.groupBox1.Height-chk.Height))
                {
                    curpos.Y = 30;
                    curpos.X += (int)(chklist[maxnum].Width*1.2);
                    maxlen = 0;
                    maxnum = 0;
                    overwidth = true;
                }              
                if((curpos.X+ (int)(chk.Width * 1.2)) > this.groupBox1.Right)
                {
                    this.Width += (int)(chk.Width * 1.3);
                    this.groupBox1.Width = this.Width - 10;
                }                
                this.Controls.Add(chklist[i]);
                groupBox1.Controls.Add(chklist[i]);
            }
            if(overwidth)
            { 
                this.btn_cancel.Left = this.Width - this.btn_cancel.Width - 30;
                this.btn_ok.Left = this.btn_cancel.Left - this.btn_ok.Width - 30;
                this.Width = Convert.ToInt32(Convert.ToDouble( chklist[maxnum].Left) +Convert.ToDouble( chklist[maxnum].Width * 1.3));
                
                this.all_chk.Left = this.Width - this.all_chk.Width - 30;
                this.Width = groupBox1.Right + 10;
                overwidth = false;
            }
            CheckOnlayers();
        }
        private void GetChklist()
        {
            foreach(CheckBox chk in chklist)
            {
                if (chk.Checked)
                    allonstrlist.Add(chk.Text);
                else
                    alloffstrlist.Add(chk.Text);
            }
        }
        private void all_chk_CheckedChanged(object sender, EventArgs e)
        {
            foreach(CheckBox chk in chklist)
            {
                if (all_chk.Checked)
                    chk.Checked = true;
                else
                    chk.Checked = false;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Plugin.blyrsh = false;
            this.Close();
        }
        private void CheckOnlayers()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            foreach (string str in allayrs)
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                       OpenMode.ForRead) as LayerTable;
                    LayerTableRecord acLyrTblRec;
                    acLyrTblRec = acTrans.GetObject(acLyrTbl[str],
                                                        OpenMode.ForWrite) as LayerTableRecord;
                    if (acLyrTbl.Has(str) == true&& acLyrTblRec.IsOff == false)
                    {
                        onlyrs.Add(str);
                    }
                    acLyrTbl.UpgradeOpen();
                    acTrans.Commit();
                }
            }
            foreach(CheckBox chk in chklist)
            {
                if (onlyrs.Contains(chk.Text))
                    chk.Checked = true;
            }
        }

        private void LayersShowHideForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Plugin.blyrsh = false;
        }
    }
}
