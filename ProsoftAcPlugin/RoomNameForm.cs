using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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
using winlichd;
using System.Windows.Input;
using Newtonsoft.Json;
using Autodesk.AutoCAD.Windows;
using static ProsoftAcPlugin.Plugin;
using static System.Net.WebRequestMethods;
using System.Globalization;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;

namespace ProsoftAcPlugin
{
    public partial class RoomNameForm : Form
    {
        public RoomNameForm()
        {
            InitializeComponent();
        }

        
        private void pub_marriagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void atriunrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            Plugin.bARoom = true;
            this.Close();
        }

        private void chbedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void dinnigkitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void livingkitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void livdinrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void studyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void guestrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void com_toilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void attachedtoil_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void servanrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void veranrm_opt_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void tvrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void drawrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void dressrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void multipurrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void passagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void loungerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void workrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void livingrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void diningrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void kitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void puja_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void stre_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void bathrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = bathrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void wcrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void washrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void toilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void combtoilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void kitchenetterm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void familyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void utilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void hallrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void entrancerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void foyerrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void sitoutrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void balcony_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void rmrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pantryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void cabinrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void officerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void bakerrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void receptionrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void restaurantrm_opt_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void cafeteriarm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void shworm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void hotelrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void departmentalrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void conferancehalrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void entrancelobbyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void firectrlrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void waitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void laundryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void shoprm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void bankrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void saferm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_rmrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_auditrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_genralwrdrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_specialrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_cinemarm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_assem_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_entrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_operthetrerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_clinicrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_consultrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_communityrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_meetrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_librm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_labrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_sevbath_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void pub_servtoilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void clsrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void hostelrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void staffrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void kgardenrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void wrkshp_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void storagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void openshedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void shedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void factoryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void godown_opt_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void hotel_opt1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void hotel_opt2_CheckedChanged(object sender, EventArgs e)
        {
            
        }
        public void makingroomname()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Commands. SetLayerCurrent("_Room");
            Plugin.aroompline.Clear();
            Plugin.aroomNmTxt.Clear();
            Plugin.aroomNmTxt.Clear();
            NBCrelate.GetNeededEntitiesOnLayer(db, "_Room");
            var resultSet = Commands.PromptForPolylineSSet("Select the Polylines to name Room");
            if (resultSet == null)
                return;
            ObjectId[] oids = resultSet.GetObjectIds();
            if ((string)Application.GetSystemVariable("clayer") == "_Room" && resultSet.Count != 0)
            {
                foreach (SelectedObject obj in resultSet)
                {
                    if (obj != null)
                    {
                        using (DocumentLock docLock = doc.LockDocument())
                        {
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                curhnd = obj.ObjectId.Handle;
                                curoid = obj.ObjectId;
                                int i = 0;
                                Polyline poly = tr.GetObject(obj.ObjectId, OpenMode.ForRead, false) as Polyline;
                                if (poly.Layer == "_Room")
                                {
                                    if (poly != null)
                                    {
                                        try
                                        {
                                            List<ObjectId> rmvidlist = new List<ObjectId>();
                                            bool bin = false;
                                            foreach (MText txtold in Plugin.aroomNmTxt)
                                            {
                                                bin = NBCrelate.IsPointInside1(txtold.Location, poly);
                                                if (bin)
                                                {
                                                    rmvidlist.Add(txtold.ObjectId);
                                                }
                                            }
                                            foreach (DBText txtold in Plugin.aRoomDBTxt)
                                            {
                                                bin = NBCrelate.IsPointInside1(txtold.Position, poly);
                                                if (bin)
                                                {
                                                    rmvidlist.Add(txtold.ObjectId);
                                                }
                                            }
                                            foreach (ObjectId rmvId in rmvidlist)
                                            {
                                                if (rmvId != ObjectId.Null)
                                                {
                                                    Entity ent = tr.GetObject(rmvId, OpenMode.ForWrite, true) as Entity;
                                                    ent.Erase();
                                                }
                                            }
                                            TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                                            ObjectId mtStyleid = db.Textstyle;
                                            if (ts.Has("Romans"))
                                            {
                                                mtStyleid = ts["Romans"];
                                            }
                                            PickPointSelectedObject ppsd = obj as PickPointSelectedObject;
                                            PickPointDescriptor ppd = ppsd.PickPoint;
                                            Point3d pickPoint = ppd.PointOnLine;
                                            Point3d oPoint = poly.GetClosestPointTo(pickPoint, ed.GetCurrentView().ViewDirection, false);
                                            double param = 0;
                                            param = poly.GetParameterAtPoint(oPoint);
                                            double sparam = 0, eparam = 0;
                                            sparam = (int)param;
                                            eparam = sparam + 1;
                                            Point3d sp = poly.GetPointAtParameter(sparam);
                                            Point3d ep = poly.GetPointAtParameter(eparam);
                                            double ang = Commands.Angle(sp, ep);
                                            Extents3d ext = poly.GeometricExtents;
                                            Point3d min = ext.MinPoint;
                                            Point3d max = ext.MaxPoint;
                                            Point3d geoCtr = Commands.Polar(min, Commands.Angle(min, max), Commands.Distance(min, max) / 2.0);
                                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                            MText txt = new MText();
                                            string str1=Commands.WidthRectPolyline(min, max).ToString("F2");
                                            string str2 = Commands.HeightRectPolyline(min, max).ToString("F2");
                                            //txt.Contents = Commands.tmproomName + "\n" + Math.Round(Convert.ToDouble(Commands.WidthRectPolyline(min, max)), 2).ToString() + " X "
                                            //    + Math.Round(Commands.HeightRectPolyline(min, max), 2).ToString(); 
                                            txt.Contents = Commands.tmproomName + "\n" + str1 + " X " +str2;
                                            txt.SetDatabaseDefaults(db);
                                            Point3d ptleft = Commands.Getleft(poly);
                                            Point3d ptright = Commands.Getright(poly);
                                            Point3d pttop = Commands.Gettop(poly);
                                            Point3d ptbottom = Commands.Getbottom(poly);
                                            double width = ptright.X - ptleft.X;
                                            double height = pttop.Y - ptbottom.Y;
                                            txt.Height = height / 2; //<==change to your default height
                                                                     //txt.Rotation = ang;
                                            txt.Width = width / 2;
                                            txt.TextStyleId = mtStyleid;
                                            txt.TextHeight = 0.3;
                                            txt.Attachment = AttachmentPoint.MiddleCenter;
                                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                                            roomrule tmproomrule = new roomrule();
                                            tmproomrule.width = width;
                                            tmproomrule.height = height;
                                            tmproomrule.pl = poly;
                                            tmproomrule.objid = poly.ObjectId;
                                            tmproomrule.hnd = poly.Id.Handle;
                                            ProsoftAcPlugin.Commands.aroomrule.Add(tmproomrule);
                                            btr.AppendEntity(txt);
                                            tr.AddNewlyCreatedDBObject(txt, true);
                                            ed.UpdateScreen();
                                        }
                                        catch
                                        {
                                            throw;
                                        }
                                    }
                                }
                                else
                                    Application.ShowAlertDialog("Selected Polyline is not Window layer");
                                i++;
                                tr.Commit();
                            }
                        }
                            
                    }
                }
                Commands.brmnamechanged = false;
                this.Show();
            }
        }

        private void RoomNameForm_Load(object sender, EventArgs e)
        {
            
        }

        private void bedrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = bedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void Mbedrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = Mbedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void chbedrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = chbedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void dinnigkitrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = dinnigkitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void livingkitrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = livingkitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void livdinrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = livdinrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void studyrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = studyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void guestrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = guestrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void com_toilrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = com_toilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void attachedtoil_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = attachedtoil_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void servanrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = servanrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void veranrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = veranrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void tvrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = tvrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void drawrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = drawrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void dressrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = dressrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void multipurrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = multipurrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void passagerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = passagerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void loungerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = loungerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void workrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = workrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void livingrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = livingrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void diningrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = diningrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void kitrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = kitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void puja_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = puja_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void stre_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = stre_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void wcrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = wcrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void washrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = washrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void toilrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = toilrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            Commands.brmnamechanged = true;
            makingroomname();
        }

        private void combtoilrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = combtoilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void kitchenetterm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = kitchenetterm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void familyrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = familyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void utilrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = utilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void hallrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = hallrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void entrancerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = entrancerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void foyerrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = foyerrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void sitoutrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = sitoutrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void balcony_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = balcony_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void rmrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = rmrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            Commands.brmnamechanged = true;
            makingroomname();
        }

        private void pantryrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pantryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void cabinrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = cabinrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void officerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = officerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void bakerrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = bakerrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void receptionrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = receptionrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void restaurantrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = restaurantrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void cafeteriarm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = cafeteriarm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void shworm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = shworm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void hotelrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = hotelrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void departmentalrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = departmentalrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void conferancehalrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = conferancehalrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void entrancelobbyrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = entrancelobbyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void firectrlrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = firectrlrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void waitrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = waitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void laundryrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = laundryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void shoprm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = shoprm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void atriunrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = atriunrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            makingroomname();
        }

        private void bankrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = bankrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void saferm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = saferm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_rmrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_rmrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_auditrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_auditrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_genralwrdrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_genralwrdrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_specialrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_specialrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            Commands.brmnamechanged = true;
            makingroomname();
        }

        private void pub_cinemarm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_cinemarm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_assem_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_assem_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_entrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_entrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_operthetrerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_operthetrerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_marriagerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_marriagerm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            makingroomname();
        }

        private void pub_clinicrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_clinicrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_consultrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_consultrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            Commands.brmnamechanged = true;
            makingroomname();
        }

        private void pub_communityrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_communityrm_opt.Text;
            close_btn.Enabled = true;
            this.Hide();
            Commands.brmnamechanged = true;
            makingroomname();
        }

        private void pub_meetrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_meetrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_librm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_librm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_labrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_labrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_sevbath_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_sevbath_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void pub_servtoilrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = pub_servtoilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void clsrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = clsrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void hostelrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = hostelrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void staffrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = staffrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void kgardenrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = kgardenrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void wrkshp_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = wrkshp_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void storagerm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = storagerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void openshedrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = shedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void shedrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = shedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void factoryrm_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = factoryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void godown_opt_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = godown_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void hotel_opt1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = hotel_opt1.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }

        private void hotel_opt2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Commands.tmproomName = hotel_opt2.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
            this.Hide();
            makingroomname();
        }
    }
}
