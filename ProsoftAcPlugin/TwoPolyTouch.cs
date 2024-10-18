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

namespace NBCLayers
{
    public partial class TwoPolyTouch : Form
    {
        string directionstr;
        public TwoPolyTouch()
        {
            InitializeComponent();
        }

        private void Btn_ok_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Plugin.twopolystr1 = directionstr;
            this.Close();
        }

        private void Btn_cncl_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Left1_CheckedChanged(object sender, EventArgs e)
        {
            directionstr= "l1";
        }

        private void Right1_CheckedChanged(object sender, EventArgs e)
        {
            directionstr = "r1";
        }

        private void Top1_CheckedChanged(object sender, EventArgs e)
        {
            directionstr = "t1";
        }

        private void Down1_CheckedChanged(object sender, EventArgs e)
        {
            directionstr = "d1";
        }
        public static void maketwopoly()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            Point3d left1 = ProsoftAcPlugin.Commands.Getleft(ProsoftAcPlugin.Plugin.ANBNPpl1);
            Point3d top1= ProsoftAcPlugin.Commands.Gettop(ProsoftAcPlugin.Plugin.ANBNPpl1);
            Point3d right1 = ProsoftAcPlugin.Commands.Getright(ProsoftAcPlugin.Plugin.ANBNPpl1);
            Point3d bottom1 = ProsoftAcPlugin.Commands.Getbottom(ProsoftAcPlugin.Plugin.ANBNPpl1);

            Point3d left2 = ProsoftAcPlugin.Commands.Getleft(ProsoftAcPlugin.Plugin.ANBNPpl2);
            Point3d top2 = ProsoftAcPlugin.Commands.Gettop(ProsoftAcPlugin.Plugin.ANBNPpl2);
            Point3d right2 = ProsoftAcPlugin.Commands.Getright(ProsoftAcPlugin.Plugin.ANBNPpl2);
            Point3d bottom2 = ProsoftAcPlugin.Commands.Getbottom(ProsoftAcPlugin.Plugin.ANBNPpl2);
            double distance = 0;
            switch(ProsoftAcPlugin.Plugin.twopolystr1)
            {
                case "l1":
                    {
                        distance = left2.X - right1.X;
                        using (DocumentLock docLock = acDoc.LockDocument())
                        {
                            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                            {
                                BlockTable acBlkTbl;
                                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                                   OpenMode.ForWrite) as BlockTable;
                                BlockTableRecord acBlkTblRec;
                                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                                      OpenMode.ForWrite) as BlockTableRecord;
                                Point3d acPt3d = new Point3d(right1.X, 0, 0);
                                Vector3d acVec3d = acPt3d.GetVectorTo(new Point3d(left2.X, 0, 0));
                                ProsoftAcPlugin.Plugin.ANBNPpl1.TransformBy(Matrix3d.Displacement(acVec3d));
                                
                                acBlkTblRec.AppendEntity(ProsoftAcPlugin.Plugin.ANBNPpl1);
                                acTrans.AddNewlyCreatedDBObject(ProsoftAcPlugin.Plugin.ANBNPpl1, true);
                                acTrans.Commit();
                            }
                        }                           
                        break;
                    }

                case "r1":
                    {
                        distance = left1.X - right2.X;
                        using (DocumentLock docLock = acDoc.LockDocument())
                        {
                            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                            {
                                BlockTable acBlkTbl;
                                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                                   OpenMode.ForWrite) as BlockTable;
                                BlockTableRecord acBlkTblRec;
                                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                                      OpenMode.ForWrite) as BlockTableRecord;
                                Point3d acPt3d = new Point3d(left1.X, 0, 0);
                                Vector3d acVec3d = acPt3d.GetVectorTo(new Point3d(right2.X, 0, 0));
                                ProsoftAcPlugin.Plugin.ANBNPpl1.TransformBy(Matrix3d.Displacement(acVec3d));
                                acBlkTblRec.AppendEntity(ProsoftAcPlugin.Plugin.ANBNPpl1);
                                acTrans.AddNewlyCreatedDBObject(ProsoftAcPlugin.Plugin.ANBNPpl1, true);
                                acTrans.Commit();
                            }
                        }                            
                        break;
                    }
                case "t1":
                    {
                        distance = bottom1.Y - top2.Y;
                        using (DocumentLock docLock = acDoc.LockDocument())
                        {
                            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                            {
                                BlockTable acBlkTbl;
                                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                                   OpenMode.ForWrite) as BlockTable;
                                BlockTableRecord acBlkTblRec;
                                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                                      OpenMode.ForWrite) as BlockTableRecord;
                                Point3d acPt3d = new Point3d(bottom1.Y, 0, 0);
                                Vector3d acVec3d = acPt3d.GetVectorTo(new Point3d(top2.Y, 0, 0));
                                ProsoftAcPlugin.Plugin.ANBNPpl1.TransformBy(Matrix3d.Displacement(acVec3d));
                                acBlkTblRec.AppendEntity(ProsoftAcPlugin.Plugin.ANBNPpl1);
                                acTrans.AddNewlyCreatedDBObject(ProsoftAcPlugin.Plugin.ANBNPpl1, true);
                                acTrans.Commit();
                            }
                        }                            
                        break;
                    }
                case "d1":
                    {
                        distance = bottom2.Y - top1.Y;
                        using (DocumentLock docLock = acDoc.LockDocument())
                        {
                            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                            {
                                BlockTable acBlkTbl;
                                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                                   OpenMode.ForWrite) as BlockTable;
                                BlockTableRecord acBlkTblRec;
                                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                                      OpenMode.ForWrite) as BlockTableRecord;
                                Point3d acPt3d = new Point3d(bottom2.Y, 0, 0);
                                Vector3d acVec3d = acPt3d.GetVectorTo(new Point3d(top1.Y, 0, 0));
                                ProsoftAcPlugin.Plugin.ANBNPpl1.TransformBy(Matrix3d.Displacement(acVec3d));
                                acBlkTblRec.AppendEntity(ProsoftAcPlugin.Plugin.ANBNPpl1);
                                acTrans.AddNewlyCreatedDBObject(ProsoftAcPlugin.Plugin.ANBNPpl1, true);
                                acTrans.Commit();
                            }
                        }                            
                        break;
                    }
            }
        }
    }
}
