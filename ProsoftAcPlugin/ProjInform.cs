using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using System.Collections.Specialized;
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

namespace ProsoftAcPlugin
{
    public partial class projectinForm : Form
    {
        public projectinForm()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            //var form = new PluginForm();
            //form.Show();
            //Plugin.usestate = (uint)Plugin.use.Residential;
            Plugin.projtypestate = (uint)cmb_projtype.SelectedIndex;
            Commands.bNewproj = true;
            Plugin.usestate = (uint)cmb_plotuse.SelectedIndex;
            //Commands.AddDoc();
            this.Close();
            var crtedlyrsfrm = new NBCLayers.Createdlayerlists();
            crtedlyrsfrm.Show();
            WriteToNODProjtypeANDPlotUse();
            DocumentCollection acDocMgr = Application.DocumentManager;
            Document acDoc = acDocMgr.MdiActiveDocument;
            Commands.SignDraw(acDoc);
            //FileSaving();
        }
        public static void WriteToNODProjtypeANDPlotUse()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var db = currentDocument.Database;
            var ed = currentDocument.Editor;
            try
            {
                using (DocumentLock docLock = currentDocument.LockDocument())
                {
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        var nod = (DBDictionary)trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite);
                        DBDictionary prevaldict;
                        if (nod.Contains("PrevalProjectType"))
                        {
                            prevaldict = (DBDictionary)trans.GetObject(nod.GetAt("PrevalProjectType"), OpenMode.ForWrite);
                        }
                        else
                        {
                            trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite);
                            prevaldict = new DBDictionary();
                            nod.SetAt("PrevalProjectType", prevaldict);
                            trans.AddNewlyCreatedDBObject(prevaldict, true);
                        }

                        Xrecord myXrecord = new Xrecord();
                        prevaldict.SetAt("ProjectType", myXrecord);
                        string projtype = Commands.ProjecttypeTostring(Plugin.projtypestate);
                        ResultBuffer resbuf = new ResultBuffer(new TypedValue(5, projtype));
                        myXrecord.Data = resbuf;
                        trans.AddNewlyCreatedDBObject(myXrecord, true);

                        Xrecord myXrecord1 = new Xrecord();
                        prevaldict.SetAt("PlotUse", myXrecord1);
                        string plotuse = Commands.PlotusetoString(Plugin.usestate);
                        ResultBuffer resbuf1 = new ResultBuffer(new TypedValue(1, plotuse));
                        myXrecord1.Data = resbuf1;
                        trans.AddNewlyCreatedDBObject(myXrecord1, true);
                        trans.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
        public void FileSaving()
        {
            System.Windows.Forms.SaveFileDialog SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            SaveFileDialog1.InitialDirectory = @"D:\";
            SaveFileDialog1.RestoreDirectory = true;
            SaveFileDialog1.Title = "Select a DWG file name";
            SaveFileDialog1.DefaultExt = "dwg";
            SaveFileDialog1.Filter = "Drawing files (*.dwg)|*.txt|All files (*.*)|*.*";
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.ShowDialog();
            if (SaveFileDialog1.FileName == "")
            {
                return;
            }
            Plugin.strCurDocPath = SaveFileDialog1.FileName;
            if (Plugin.strCurDocPath.Contains(".dwg"))
                Plugin.strCurDocPath = Path.ChangeExtension(Plugin.strCurDocPath, ".dwg");
            int posstring = SaveFileDialog1.FileName.LastIndexOf(".");
            string strJsonName = SaveFileDialog1.FileName.Remove(posstring, SaveFileDialog1.FileName.Length - posstring);
            Plugin.strCurJsonPath = Path.ChangeExtension(Plugin.strCurDocPath, "json");
            var crtedlyrsfrm = new NBCLayers.Createdlayerlists();
            crtedlyrsfrm.Show();
        }
        private void DTCP_GPs_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.DTCP_GPs;
        }

        private void DTCP_UDAGps_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.DTCP_UDAGps;
        }

        private void HMDA_ULBs_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.HMDA_ULBs;
            
        }

        private void DTCP_ULBs_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.DTCP_ULBs;
            
        }

        private void HMDA_GPs_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.HMDA_GPs;
            
        }

        private void GHMC_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)Plugin.m_enuAuthority.GHMC;
        }

        private void LAYOUT_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.b_PLAN = true;
            btn_ok.Enabled = true;
        }

        private void BUILDING_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.b_PLAN = false;
            btn_ok.Enabled = true;
        }

        private void list_projtype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmb_apptype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.apptypestate = (uint)cmb_apptype.SelectedIndex;
        }

        private void projectinForm_Load(object sender, EventArgs e)
        {

        }

        private void cmb_plotuse_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selNum = cmb_plotuse.SelectedIndex;
            Plugin.usestate = (uint)cmb_plotuse.SelectedIndex;
            switch(selNum)
            {
                case 0:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Residental Bldg");
                    cmb_plotsubuse.Items.Add("Bungalow");
                    cmb_plotsubuse.Items.Add("Semidetached");
                    cmb_plotsubuse.Items.Add("Row House");
                    cmb_plotsubuse.Items.Add("Low income group and EWS Housing");
                    cmb_plotsubuse.Items.Add("Group Housing");
                    cmb_plotsubuse.Items.Add("Farm House");
                    cmb_plotsubuse.Items.Add("Hostel");
                    cmb_plotsubuse.Items.Add("Dormitory");
                    cmb_plotsubuse.Items.Add("Boarding");
                    cmb_plotsubuse.Items.Add("Dharamshala");
                    cmb_plotsubuse.Items.Add("Guest House");
                    cmb_plotsubuse.Items.Add("Staff Quarters");
                    cmb_plotsubuse.Items.Add("Old Age HOme");
                    cmb_plotsubuse.Items.Add("Orphanages");
                    cmb_plotsubuse.Items.Add("Other Residental Building");
                    cmb_plotsubuse.Items.Add("Individual Row House");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 1:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Shop");
                    cmb_plotsubuse.Items.Add("Store");
                    cmb_plotsubuse.Items.Add("Retail Shop");
                    cmb_plotsubuse.Items.Add("Bank");
                    cmb_plotsubuse.Items.Add("Safe Deposit Vault");
                    cmb_plotsubuse.Items.Add("Shopping Centre/mall");
                    cmb_plotsubuse.Items.Add("Showroom");
                    cmb_plotsubuse.Items.Add("Commercial Bldg");
                    cmb_plotsubuse.Items.Add("Market");
                    cmb_plotsubuse.Items.Add("Shopping Mall");
                    cmb_plotsubuse.Items.Add("Departmental Store");
                    cmb_plotsubuse.Items.Add("Shopping Malls with Multiplexes");
                    cmb_plotsubuse.Items.Add("SuperMarkets");
                    cmb_plotsubuse.Items.Add("Convenience Markets");
                    cmb_plotsubuse.Items.Add("Resicomm Bldg");
                    cmb_plotsubuse.Items.Add("Office");
                    cmb_plotsubuse.Items.Add("Shop and Office");
                    cmb_plotsubuse.Items.Add("Professional Office");
                    cmb_plotsubuse.Items.Add("Corporate Office");
                    cmb_plotsubuse.Items.Add("Business Office");
                    cmb_plotsubuse.Items.Add("IT Office");
                    cmb_plotsubuse.Items.Add("Bio-Technology(BT) Office");
                    cmb_plotsubuse.Items.Add("Information-technology IT/ITES");
                    cmb_plotsubuse.Items.Add("Corporate Commercial");
                    cmb_plotsubuse.Items.Add("Restaurant");
                    cmb_plotsubuse.Items.Add("3 Star Hotel");
                    cmb_plotsubuse.Items.Add("4 Star Hotel");
                    cmb_plotsubuse.Items.Add("5 Star Hotel");
                    cmb_plotsubuse.Items.Add("Hotel");
                    cmb_plotsubuse.Items.Add("Lodging");
                    cmb_plotsubuse.Items.Add("Holiday Resort");
                    cmb_plotsubuse.Items.Add("Service orRepair establishments");
                    cmb_plotsubuse.Items.Add("Clinic");
                    cmb_plotsubuse.Items.Add("Kiosk");
                    cmb_plotsubuse.Items.Add("Service Station");
                    cmb_plotsubuse.Items.Add("Pathological Lab");
                    cmb_plotsubuse.Items.Add("Booth");
                    cmb_plotsubuse.Items.Add("Parlor");
                    cmb_plotsubuse.Items.Add("Bakery");
                    cmb_plotsubuse.Items.Add("Training Institue");
                    cmb_plotsubuse.Items.Add("Public Library");
                    cmb_plotsubuse.Items.Add("Court House");
                    cmb_plotsubuse.Items.Add("Call Centers");
                    cmb_plotsubuse.Items.Add("Junk Yard");
                    cmb_plotsubuse.Items.Add("Godowns");
                    cmb_plotsubuse.Items.Add("Ware House");
                    cmb_plotsubuse.Items.Add("Good Storage");
                    cmb_plotsubuse.Items.Add("Cold Storage");
                    cmb_plotsubuse.Items.Add("Petrol Pump");
                    cmb_plotsubuse.Items.Add("Petrol Filling Station(With Service Bay)");
                    cmb_plotsubuse.Items.Add("Petrol Filling Station(Without Service Bay)");
                    cmb_plotsubuse.Items.Add("Parking Complex(Parking Lot)");
                    cmb_plotsubuse.Items.Add("Gas Godown");
                    cmb_plotsubuse.Items.Add("Wholesale Commercial Market");
                    cmb_plotsubuse.Items.Add("Other Commercial Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 2:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Industrial Building");
                    cmb_plotsubuse.Items.Add("Service Industry");
                    cmb_plotsubuse.Items.Add("HouseHold Industry");
                    cmb_plotsubuse.Items.Add("Light Industry");
                    cmb_plotsubuse.Items.Add("Medium Industry");
                    cmb_plotsubuse.Items.Add("Heavy Industry");
                    cmb_plotsubuse.Items.Add("Workshop");
                    cmb_plotsubuse.Items.Add("Industrial Laboratory");
                    cmb_plotsubuse.Items.Add("Power Plant");
                    cmb_plotsubuse.Items.Add("Assembly Plant");
                    cmb_plotsubuse.Items.Add("Refinery");
                    cmb_plotsubuse.Items.Add("Gas Plant");
                    cmb_plotsubuse.Items.Add("Mill");
                    cmb_plotsubuse.Items.Add("Factory");
                    cmb_plotsubuse.Items.Add("Dairy");
                    cmb_plotsubuse.Items.Add("Godown");
                    cmb_plotsubuse.Items.Add("Small Scale Industries");
                    cmb_plotsubuse.Items.Add("Other Industrial Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 3:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Theatre");
                    cmb_plotsubuse.Items.Add("Cinema");
                    cmb_plotsubuse.Items.Add("MultiPlex");
                    cmb_plotsubuse.Items.Add("Auditorium");
                    cmb_plotsubuse.Items.Add("Cultural Complex");
                    cmb_plotsubuse.Items.Add("Recreation Building");
                    cmb_plotsubuse.Items.Add("Patriotic");
                    cmb_plotsubuse.Items.Add("Place of Assembly");
                    cmb_plotsubuse.Items.Add("Conference Hall");
                    cmb_plotsubuse.Items.Add("Social Club");
                    cmb_plotsubuse.Items.Add("Religious Building");
                    cmb_plotsubuse.Items.Add("Assembly Hall");
                    cmb_plotsubuse.Items.Add("Drama Hall");
                    cmb_plotsubuse.Items.Add("City Hall");
                    cmb_plotsubuse.Items.Add("Town Hall");
                    cmb_plotsubuse.Items.Add("Dance Hall");
                    cmb_plotsubuse.Items.Add("Club");
                    cmb_plotsubuse.Items.Add("Meeting Hall");
                    cmb_plotsubuse.Items.Add("Lecture Hall");
                    cmb_plotsubuse.Items.Add("Mangal Karyalaya");
                    cmb_plotsubuse.Items.Add("Banquet Hall");
                    cmb_plotsubuse.Items.Add("Marriage Hall");
                    cmb_plotsubuse.Items.Add("Community Hall");
                    cmb_plotsubuse.Items.Add("Party Plot");
                    cmb_plotsubuse.Items.Add("Exihibition Centre");
                    cmb_plotsubuse.Items.Add("Amusement Building/Park");
                    cmb_plotsubuse.Items.Add("Games Centre");
                    cmb_plotsubuse.Items.Add("Museum");
                    cmb_plotsubuse.Items.Add("Skating Ring");
                    cmb_plotsubuse.Items.Add("Stadium");
                    cmb_plotsubuse.Items.Add("Gymnasia");
                    cmb_plotsubuse.Items.Add("Sports Complex");
                    cmb_plotsubuse.Items.Add("Art Gallery");
                    cmb_plotsubuse.Items.Add("Circus");
                    cmb_plotsubuse.Items.Add("GymKhana");
                    cmb_plotsubuse.Items.Add("Welfare Center");
                    cmb_plotsubuse.Items.Add("Public transportation station and Recreation");
                    cmb_plotsubuse.Items.Add("Public Utility Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 4:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Public Utility Bldg");
                    cmb_plotsubuse.Items.Add("Police Station");
                    cmb_plotsubuse.Items.Add("Post Office");
                    cmb_plotsubuse.Items.Add("TeleCommunication");
                    cmb_plotsubuse.Items.Add("Public Distribution System Shop");
                    cmb_plotsubuse.Items.Add("Fire Station");
                    cmb_plotsubuse.Items.Add("Bill Collection Centre");
                    cmb_plotsubuse.Items.Add("Broadcasting-Transmission Station");
                    cmb_plotsubuse.Items.Add("EB Office");
                    cmb_plotsubuse.Items.Add("Telegraph Office");
                    cmb_plotsubuse.Items.Add("Public Garage");
                    cmb_plotsubuse.Items.Add("Public Parking");
                    cmb_plotsubuse.Items.Add("Sub-Station");
                    cmb_plotsubuse.Items.Add("Water Works");
                    cmb_plotsubuse.Items.Add("Other Public utility Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 5:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Transportation Building");
                    cmb_plotsubuse.Items.Add("Transport Terminal");
                    cmb_plotsubuse.Items.Add("Traffic-Transport related Facility");
                    cmb_plotsubuse.Items.Add("Passenger Station (Bus-Railway)");
                    cmb_plotsubuse.Items.Add("Auto Stand");
                    cmb_plotsubuse.Items.Add("Taxi Stands");
                    cmb_plotsubuse.Items.Add("Bus Stand");
                    cmb_plotsubuse.Items.Add("Bus Terminal");
                    cmb_plotsubuse.Items.Add("Truck Terminal");
                    cmb_plotsubuse.Items.Add("Railway Station");
                    cmb_plotsubuse.Items.Add("Airport");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 6:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Educational Building");
                    cmb_plotsubuse.Items.Add("School");
                    cmb_plotsubuse.Items.Add("Primary School");
                    cmb_plotsubuse.Items.Add("Nursery School");
                    cmb_plotsubuse.Items.Add("High School");
                    cmb_plotsubuse.Items.Add("Secondary-Higher Secondary School");
                    cmb_plotsubuse.Items.Add("College");
                    cmb_plotsubuse.Items.Add("Research Institution");
                    cmb_plotsubuse.Items.Add("Educational Institution");
                    cmb_plotsubuse.Items.Add("Railway Station");
                    cmb_plotsubuse.Items.Add("Library");
                    cmb_plotsubuse.Items.Add("Technical School");
                    cmb_plotsubuse.Items.Add("Coaching Class");
                    cmb_plotsubuse.Items.Add("Middle School");
                    cmb_plotsubuse.Items.Add("Tutorial Centre");
                    cmb_plotsubuse.Items.Add("Research and Development");
                    cmb_plotsubuse.Items.Add("Other Educational Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 7:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Nursing Home");
                    cmb_plotsubuse.Items.Add("Dispensary");
                    cmb_plotsubuse.Items.Add("Clinic");
                    cmb_plotsubuse.Items.Add("Lab");
                    cmb_plotsubuse.Items.Add("Indoor Patients Wards");
                    cmb_plotsubuse.Items.Add("Hospital");
                    cmb_plotsubuse.Items.Add("Special Hospital");
                    cmb_plotsubuse.Items.Add("Private Hospital");
                    cmb_plotsubuse.Items.Add("Govt-Semi Govt. Hospital");
                    cmb_plotsubuse.Items.Add("Research and Training Center");
                    cmb_plotsubuse.Items.Add("Rehabilitation Center");
                    cmb_plotsubuse.Items.Add("Govt. Dispensary");
                    cmb_plotsubuse.Items.Add("Maternity Home");
                    cmb_plotsubuse.Items.Add("Health Centre");
                    cmb_plotsubuse.Items.Add("Medical Building");
                    cmb_plotsubuse.Items.Add("Sanatoria");
                    cmb_plotsubuse.Items.Add("Forensic Science Laboratory");
                    cmb_plotsubuse.Items.Add("Other Medical Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 8:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Storage Bldg");
                    cmb_plotsubuse.Items.Add("Godown");
                    cmb_plotsubuse.Items.Add("WareHouse");
                    cmb_plotsubuse.Items.Add("Cold Storage Depot");
                    cmb_plotsubuse.Items.Add("Other Building");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 9:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Villa");
                    break;
                case 10:
                    cmb_plotsubuse.Items.Clear();
                    cmb_plotsubuse.Items.Add("Parking tower/Parking complex");
                    cmb_plotsubuse.Items.Add("Villa");
                    break;

            }
        }

        private void cmb_projtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.projtypestate = (uint)(cmb_projtype.SelectedIndex);
        }

        private void cmb_casetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.casetypestate = (uint)cmb_casetype.SelectedIndex;
        }

        private void cmb_plotsubuse_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.subuse = cmb_plotsubuse.SelectedItem.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.religousstate = (uint)comboBox1.SelectedIndex;
        }

        private void cmb_subLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.sublocationstate = (uint)cmb_subLocation.SelectedIndex;
        }

        private void cmb_authority_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.authoritystate = (uint)cmb_authority.SelectedIndex;
        }
    }
}
