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
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using AcadWindows = Autodesk.AutoCAD.Windows;
using System.Globalization;

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
        public List<ObjectId> tchobjlist = new List<ObjectId>();
        public void Show_LineCloseResult()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            List<ObjectId> objidlist = new List<ObjectId>();
            List<Handle> hndlelist = new List<Handle>();
            TypedValue[] filList = new TypedValue[1] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") };
            SelectionFilter filter = new SelectionFilter(filList);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            Plugin.allLayers = Commands.LayersToList(acCurDb);
            opts.MessageForAdding = "Select polylines: ";
            PromptSelectionResult res = ed.GetSelection(opts, filter);
            if (res.Status != PromptStatus.OK)
                return;
            SelectionSet selSet = res.Value;
            ObjectId[] ids = selSet.GetObjectIds();
            StringBuilder sb = new StringBuilder();
            using (Transaction tr = acCurDb.TransactionManager.StartTransaction())
            {
                Polyline pl = (Polyline)tr.GetObject(ids[0], OpenMode.ForRead);
                foreach (string layername in Plugin.allLayers)
                {
                    //if (layername != pl.Layer)
                    //{
                    if(pl.Closed)
                    {
                        List<Polyline> tmplist = new List<Polyline>();
                        tmplist = GetAllPolylineByLayer(layername);
                        foreach (Polyline pltmp in tmplist)
                        {
                            if(pl!=pltmp)
                            {
                                bool isbtch = false;
                                isbtch = NBCrelate.checkTwoPlineTouch(pl, pltmp);
                                if (isbtch)
                                {
                                    objidlist.Add(pltmp.ObjectId);
                                    tchobjlist.Add(pltmp.ObjectId);
                                    hndlelist.Add(pltmp.ObjectId.Handle);
                                    string[] row = { layername, pltmp.ObjectId.ToString(), pltmp.ObjectId.Handle.ToString() };
                                    var listViewItem = new ListViewItem(row);
                                    listView1.Items.Add(listViewItem);
                                }
                            }                            
                        }
                    }                        
                    //}
                }
                tr.Commit();
            }            
        }
        public static ObjectIdCollection SelectAllPolylineByLayer(string sLayer)
        {
            Document oDwg = Application.DocumentManager.MdiActiveDocument;
            Editor oEd = oDwg.Editor;

            ObjectIdCollection retVal = null;

            try
            {
                // Get a selection set of all possible polyline entities on the requested layer
                PromptSelectionResult oPSR = null;

                TypedValue[] tvs = new TypedValue[] {
            new TypedValue(Convert.ToInt32(DxfCode.Operator), "<and"),
            new TypedValue(Convert.ToInt32(DxfCode.LayerName), sLayer),
            new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
            new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
            new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
            new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
            new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
            new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            new TypedValue(Convert.ToInt32(DxfCode.Operator), "and>")
            };

                SelectionFilter oSf = new SelectionFilter(tvs);
                oPSR = oEd.SelectAll(oSf);
                if (oPSR.Status == PromptStatus.OK)
                {
                    retVal = new ObjectIdCollection(oPSR.Value.GetObjectIds());
                }
                else
                {
                    retVal = new ObjectIdCollection();
                }
            }
            catch (System.Exception ex)
            {
                
            }
            return retVal;
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            string objectstring = "";
            string hndlestring = "";
            List<ObjectId> tmpobjlist = new List<ObjectId>();
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                objectstring = item.SubItems[1].Text;
                hndlestring= item.SubItems[2].Text;
            }
            else
            {
                return;
            }
            foreach(ObjectId objid in tchobjlist)
            {
                if (objid.ToString() == objectstring)
                    tmpobjlist.Add(objid);
            }
            using (DocumentLock docLock = acDoc.LockDocument())
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    if (tmpobjlist.Count > 0)
                    {
                        ed.SetImpliedSelection(tmpobjlist.ToArray());
                        acTrans.Commit();
                    }
                }
                ed.UpdateScreen();
            }
        }

        private void PlineCloseFrm_Load(object sender, EventArgs e)
        {

        }
        public static List<Polyline> GetAllPolylineByLayer(string slayer)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDoc.Database;
            Editor ed = acDoc.Editor;
            List<Polyline> retplylist = new List<Polyline>();
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    var PlineCls = RXObject.GetClass(typeof(Polyline));
                    var MTxtCls = RXObject.GetClass(typeof(MText));
                    var TxtCls = RXObject.GetClass(typeof(DBText));
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            if (subent.Layer == slayer)
                            {
                                if (id.ObjectClass == PlineCls)
                                {
                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                    retplylist.Add(pline);
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
            return retplylist;
        }
    }
}
