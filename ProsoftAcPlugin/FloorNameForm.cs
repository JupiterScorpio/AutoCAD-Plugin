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

namespace NBCLayers
{
    public partial class FloorNameForm : Form
    {
        public static string strfloorsectionname,strfloorname;
        public static List<string> flrlst = new List<string>();
        public static bool b_okCancel;
        public FloorNameForm()
        {
            InitializeComponent();
        }

        private void FloorNameForm_Load(object sender, EventArgs e)
        {
            chk_mezzanine.Checked = false;
            chk_typical.Checked = false;
            chk_podium.Checked = false;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Commands.bflrReassign = false;
            b_okCancel = false;
            this.Close();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            ProsoftAcPlugin.Commands.bflrReassign = true;
            b_okCancel = true;
            FloorNameFilter();
            this.Close();
            MessageBox.Show("First Select a FloorInSection Layer Polyline", "FloorInSection PolyLine", MessageBoxButtons.OK, MessageBoxIcon.None);
            AssignFloorandFloorInSection();
        }

        private void chk_typical_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_typical.Checked)
            {
                btn_0.Enabled = true;
                btn_1.Enabled = true;
                btn_2.Enabled = true;
                btn_3.Enabled = true;
                btn_4.Enabled = true;
                btn_5.Enabled = true;
                btn_6.Enabled = true;
                btn_7.Enabled = true;
                btn_8.Enabled = true;
                btn_9.Enabled = true;
                ProsoftAcPlugin.Commands.bfloorTypical = true;
            }
            else
            {
                btn_0.Enabled = false;
                btn_1.Enabled = false;
                btn_2.Enabled = false;
                btn_3.Enabled = false;
                btn_4.Enabled = false;
                btn_5.Enabled = false;
                btn_6.Enabled = false;
                btn_7.Enabled = false;
                btn_8.Enabled = false;
                btn_9.Enabled = false;
                cmb_floorname.Text = "";
                ProsoftAcPlugin.Commands.bfloorTypical = false;
            }
        }

        private void chk_mezzanine_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_mezzanine.Checked)
            {
                btn_comma.Enabled = true;
                btn_hypen.Enabled = true;
                btn_and.Enabled = true;
            }else
            {
                btn_comma.Enabled = false;
                btn_hypen.Enabled = false;
                btn_and.Enabled = false;
            }
        }

        private void btn_comma_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += ",";
            btn_comma.Enabled = false;
            btn_hypen.Enabled = false;
            btn_and.Enabled = false;
        }

        private void btn_hypen_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "-";
            btn_comma.Enabled = false;
            btn_hypen.Enabled = false;
            btn_and.Enabled = false;
        }

        private void btn_and_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "&";
            btn_comma.Enabled = false;
            btn_hypen.Enabled = false;
            btn_and.Enabled = false;
        }

        private void btn_0_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "0";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_1_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "1";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_2_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "2";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_3_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "3";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_4_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "4";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_5_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "5";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_6_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "6";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_7_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "7";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_8_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "8";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void btn_9_Click(object sender, EventArgs e)
        {
            cmb_floorname.Text += "9";
            btn_comma.Enabled = true;
            btn_hypen.Enabled = true;
            btn_and.Enabled = true;
        }

        private void cmb_floorname_SelectedIndexChanged(object sender, EventArgs e)
        {
            strfloorsectionname = cmb_floorname.SelectedItem.ToString()+" FLOOR";
            strfloorname = cmb_floorname.SelectedItem.ToString() + " FLOOR PLAN";
        }
        private bool FloorNameFilter()
        {
            if(chk_typical.Checked&&cmb_floorname.Text!="")
            {
                strfloorname = "TYPICAL - " + cmb_floorname.Text + " FLOOR PLAN";
                strfloorsectionname = cmb_floorname.Text + " FLOOR";
                ProsoftAcPlugin.Commands.bfloorTypical = true;
                string strtemp = strfloorsectionname.Replace(" FLOOR","");
                while(strtemp!="")
                {
                    string strsepSign = "";
                    int resultpos = 1000;
                    int Poscom = strtemp.IndexOf(",");
                    int Posand = strtemp.IndexOf("&");
                    int Poshyp = strtemp.IndexOf("-");
                    if (Poscom > 0)
                    {
                        resultpos = Math.Min(resultpos, Poscom);
                        strsepSign = ",";
                    }
                    if (Posand > 0)
                    {
                        resultpos = Math.Min(resultpos, Posand);
                        strsepSign = "&";
                    }
                    if (Poshyp > 0)
                    {
                        resultpos = Math.Min(resultpos, Poshyp);
                        strsepSign = "-";
                    }
                    if (resultpos == Poshyp)
                        strsepSign = "-";
                    if (resultpos == Posand)
                        strsepSign = "&";
                    if (resultpos == Poscom)
                        strsepSign = ",";
                    string strbuf = "";
                    if (resultpos < 0 || resultpos == 1000)
                    {
                        resultpos = strtemp.Length;
                        strbuf = strtemp.Substring(0, resultpos);
                        strtemp = "";
                    }
                    else
                    {
                        strbuf = strtemp.Substring(0, resultpos);
                        strtemp = strtemp.Remove(0, resultpos + 1);
                    }                    
                    if (strsepSign == "-")
                    {
                        int resultpos1 = 100;
                        int Poscom1 = strtemp.IndexOf(",");
                        int Posand1 = strtemp.IndexOf("&");
                        int Poshyp1 = strtemp.IndexOf("-");
                        if (Poscom1 > 0)
                        {
                            resultpos1 = Math.Min(resultpos1, Poscom1);
                        }
                        if (Posand1 > 0)
                        {
                            resultpos1 = Math.Min(resultpos1, Posand1);
                        }
                        if (Poshyp1 > 0)
                        {
                            resultpos1 = Math.Min(resultpos1, Poshyp1);
                        }
                        if(resultpos1==Poshyp1)
                        {
                            MessageBox.Show("Wrong Express! continous - Sign. Try again.");
                            this.Close();
                            return false;
                        }
                        string strbuf1 = "";
                        if (resultpos1 == 100||resultpos1<0)
                        {
                            resultpos1 = strtemp.Length;
                            strbuf1 = strtemp.Substring(0, resultpos1);
                            strtemp = "";
                        }
                        else
                        {
                            strbuf1 = strtemp.Substring(0, resultpos1);
                            strtemp = strtemp.Remove(0, resultpos1 + 1);
                        }
                        for (int i=Convert.ToInt32(strbuf);i<=Convert.ToInt32(strbuf1); i++)
                        {
                            ProsoftAcPlugin.Commands.floornamelst.Add(i.ToString());
                            flrlst.Add(FloorInSectionNameConvertor(i.ToString()));
                        }
                    }
                    else
                    {
                        flrlst.Add(FloorInSectionNameConvertor(strbuf));
                    }      
                }
                return true;
            }
            else
            {
                ProsoftAcPlugin.Commands.tmpfloorsectionName = strfloorsectionname;
                ProsoftAcPlugin.Commands.floornamelst.Add(strfloorsectionname);
                ProsoftAcPlugin.Commands.tmpfloorName = strfloorname;
                ProsoftAcPlugin.Commands.bfloorTypical = false;
                flrlst.Add(strfloorsectionname);
                return true;
            }                
        }
        public static void AssignFloorandFloorInSection()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            ProsoftAcPlugin.Commands.SetLayerCurrent("_FloorInSection");

        FloorNameRpeat:
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    PromptEntityOptions options = new PromptEntityOptions("\nSelect a FloorInSection Polyline: ");
                    options.SetRejectMessage("\nSelected object is no a Polyline.");
                    options.AddAllowedClass(typeof(Polyline), true);
                    PromptEntityResult result = ed.GetEntity(options);
                    if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection"&&result.ObjectId!=null)
                    {
                        if (result.Status == PromptStatus.OK)
                        {
                            Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                            if ((poly != null)&&(poly.Layer== "_FloorInSection"))
                            {
                                TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                                ObjectId mtStyleid = db.Textstyle;
                                if (ts.Has("Romans"))
                                {
                                    mtStyleid = ts["Romans"];
                                }
                                Point3d pickPoint = result.PickedPoint;
                                Point3d oPoint = poly.GetClosestPointTo(pickPoint, ed.GetCurrentView().ViewDirection, false);
                                double param = 0;
                                param = poly.GetParameterAtPoint(oPoint);
                                double sparam = 0, eparam = 0;
                                sparam = (int)param;
                                eparam = sparam + 1;
                                Point3d sp = poly.GetPointAtParameter(sparam);
                                Point3d ep = poly.GetPointAtParameter(eparam);
                                double ang = ProsoftAcPlugin.Commands.Angle(sp, ep);
                                Extents3d ext = poly.GeometricExtents;
                                Point3d min = ext.MinPoint;
                                Point3d max = ext.MaxPoint;
                                Point3d geoCtr = ProsoftAcPlugin.Commands.Polar(min, ProsoftAcPlugin.Commands.Angle(min, max), ProsoftAcPlugin.Commands.Distance(min, max) / 2.0);
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                MText txt = new MText();
                                txt.Contents = flrlst[0];
                                txt.SetDatabaseDefaults(db);
                                Point3d ptleft = ProsoftAcPlugin.Commands.Getleft(poly);
                                Point3d ptright = ProsoftAcPlugin.Commands.Getright(poly);
                                Point3d pttop = ProsoftAcPlugin.Commands.Gettop(poly);
                                Point3d ptbottom = ProsoftAcPlugin.Commands.Getbottom(poly);
                                double width = ptright.X - ptleft.X;
                                double height = pttop.Y - ptbottom.Y;
                                txt.Height = height/3; //<==change to your default height
                                txt.Width = width/3;
                                txt.TextHeight = 0.1;
                                txt.TextStyleId = mtStyleid;
                                txt.Attachment = AttachmentPoint.BottomLeft;
                                txt.Location = new Point3d(ptleft.X+width/2, ptbottom.Y+height/2, 0);
                                btr.AppendEntity(txt);
                                tr.AddNewlyCreatedDBObject(txt, true);
                            }
                            else
                                Application.ShowAlertDialog("PolyLine not selected or incorrect layer!");
                        }

                    }
                    tr.Commit();
                }
                flrlst.RemoveAt(0);
                if (flrlst.Count != 0)
                    goto FloorNameRpeat;
            }

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    ProsoftAcPlugin.Commands.SetLayerCurrent("_Floor");
                    MessageBox.Show("Select a Floor Layer Polyline", "Floor PolyLine", MessageBoxButtons.OK, MessageBoxIcon.None);
                    PromptEntityOptions options1 = new PromptEntityOptions("\nSelect a Floor Polyline: ");
                    options1.SetRejectMessage("\nSelected object is no a Polyline.");
                    options1.AddAllowedClass(typeof(Polyline), true);
                    PromptEntityResult result1 = ed.GetEntity(options1);
                    if ((string)Application.GetSystemVariable("clayer") == "_Floor")
                    {
                        if (result1.Status == PromptStatus.OK)
                        {
                            Polyline poly1 = tr.GetObject(result1.ObjectId, OpenMode.ForRead, false) as Polyline;
                            if (poly1 != null && poly1.Layer == "_Floor")
                            {
                                TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                                ObjectId mtStyleid = db.Textstyle;
                                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                if (ts.Has("Romans"))
                                {
                                    mtStyleid = ts["Romans"];
                                }
                                Point3d pickPoint = result1.PickedPoint;
                                Point3d oPoint = poly1.GetClosestPointTo(pickPoint, ed.GetCurrentView().ViewDirection, false);
                                double param = 0;
                                param = poly1.GetParameterAtPoint(oPoint);
                                double sparam = 0, eparam = 0;
                                sparam = (int)param;
                                eparam = sparam + 1;
                                Point3d sp = poly1.GetPointAtParameter(sparam);
                                Point3d ep = poly1.GetPointAtParameter(eparam);
                                double ang = ProsoftAcPlugin.Commands.Angle(sp, ep);
                                Extents3d ext = poly1.GeometricExtents;
                                Point3d min = ext.MinPoint;
                                Point3d max = ext.MaxPoint;
                                Point3d geoCtr = ProsoftAcPlugin.Commands.Polar(min, ProsoftAcPlugin.Commands.Angle(min, max), ProsoftAcPlugin.Commands.Distance(min, max) / 2.0);
                                MText txt = new MText();
                                txt.Contents = strfloorname;
                                txt.SetDatabaseDefaults(db);
                                Point3d ptleft = ProsoftAcPlugin.Commands.Getleft(poly1);
                                Point3d ptright = ProsoftAcPlugin.Commands.Getright(poly1);
                                Point3d pttop = ProsoftAcPlugin.Commands.Gettop(poly1);
                                Point3d ptbottom = ProsoftAcPlugin.Commands.Getbottom(poly1);
                                double width = ptright.X - ptleft.X;
                                double height = pttop.Y - ptbottom.Y;
                                txt.Height = height/3; //<==change to your default height
                                txt.Width = width/3;
                                txt.TextHeight = 0.3;
                                txt.TextStyleId = mtStyleid;
                                txt.Attachment = AttachmentPoint.BottomLeft;
                                txt.Location = new Point3d(ptleft.X + width / 2, ptbottom.Y + height / 2, 0);
                                btr.AppendEntity(txt);
                                tr.AddNewlyCreatedDBObject(txt, true);
                            }
                            else
                                Application.ShowAlertDialog("PolyLine not selected or incorrect layer!");
                        }
                    }
                    tr.Commit();
                }
            }

            DialogResult dresult = MessageBox.Show("Do you want to assign more floor name?", "AutoCAD", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dresult == DialogResult.Yes)
                ProsoftAcPlugin.Commands.AssignFloorNames();

        }
        public static string FloorInSectionNameConvertor(string strin)
        {
            string strresult = "";
            switch (strin)
            {
                case "1":
                    strresult = "FIRST FLOOR";
                    break;
                case "2":
                    strresult = "SECOND FLOOR";
                    break;
                case "3":
                    strresult = "THIRD FLOOR";
                    break;
                case "4":
                    strresult = "FORTH FLOOR";
                    break;
                case "5":
                    strresult = "FIFTH FLOOR";
                    break;
                case "6":
                    strresult = "SIXTH FLOOR";
                    break;
                case "7":
                    strresult = "SEVENTH FLOOR";
                    break;
                case "8":
                    strresult = "EIGHT FLOOR";
                    break;
                case "9":
                    strresult = "NINETH FLOOR";
                    break;
                case "10":
                    strresult = "TENTH FLOOR";
                    break;
                case "11":
                    strresult = "ELEVENTH FLOOR";
                    break;
                case "12":
                    strresult = "TWELFTH FLOOR";
                    break;
                case "13":
                    strresult = "THIRTEENTH FLOOR";
                    break;
                case "14":
                    strresult = "FOURTEENTH FLOOR";
                    break;
                case "15":
                    strresult = "FIFTEENTH FLOOR";
                    break;
                case "16":
                    strresult = "SIXTEENTH FLOOR";
                    break;
                case "17":
                    strresult = "SEVENTEENTH FLOOR";
                    break;
                case "18":
                    strresult = "EIGHTEENTH FLOOR";
                    break;
                case "19":
                    strresult = "NINETEENTH FLOOR";
                    break;
                case "20":
                    strresult = "TWENTIETH FLOOR";
                    break;
                case "21":
                    strresult = "TWENTY FIRST FLOOR";
                    break;
                case "22":
                    strresult = "TWENTY SECOND FLOOR";
                    break;
                case "23":
                    strresult = "TWENTY THIRD FLOOR";
                    break;
                case "24":
                    strresult = "TWENTY FOURTH FLOOR";
                    break;
                case "25":
                    strresult = "TWENTY FIFTH FLOOR";
                    break;
                case "26":
                    strresult = "TWENTY SIXTH FLOOR";
                    break;
                case "27":
                    strresult = "TWENTY SEVENTH FLOOR";
                    break;
                case "28":
                    strresult = "TWENTY EIGHTH FLOOR";
                    break;
                case "29":
                    strresult = "TWENTY NINETH FLOOR";
                    break;
                case "30":
                    strresult = "THIRTIETH FLOOR";
                    break;
                case "31":
                    strresult = "THIRTY FIRST FLOOR";
                    break;
                case "32":
                    strresult = "THIRTY SECOND FLOOR";
                    break;
                case "33":
                    strresult = "THIRTY THIRD FLOOR";
                    break;
                case "34":
                    strresult = "THIRTY FOURTH FLOOR";
                    break;
                case "35":
                    strresult = "THIRTY FIFTH FLOOR";
                    break;
                case "36":
                    strresult = "THIRTY SIXTH FLOOR";
                    break;
            }
            return strresult;
        }
    }
}
