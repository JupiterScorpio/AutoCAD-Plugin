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

namespace ProsoftAcPlugin
{    
    public class Plugin
    {        
        public enum use
        {
            Residential, Commercial, Institutional, Assembly, PublicUtility, TransportationandCommunication, EducationalandIndustrial,
            Medical, Storage, MixedUse, ParkingtowerorParkingcomplex
        }
        public enum apptype
        {
            General, Ressidentalless4000,Ressidentalabove4000
        } 
        public enum projtype
        {
            BuildingPermission,SubDivision,EWSorLIGScheme,LayoutsOpenPlots,LayoutwithoutGatedCommunity,
            LayoutGatedCommunity,Amalgamation,MultistoriedBuildings,GroupScheme,CompoundWall,RowTypeHousingScheme,
            ClusterHousingScheme,ChangeLandUse
        }
        public enum casetype
        {
            New,AdditionorAlteration,Revision,ExtensionofPermission,ChangeLand,Renewal,
            RegularizationConstruction, Demolition,Regularizationlayouts
        }
        public enum religous
        {
            NA,within100,  above100upto300, above300
        }
        public enum subloacation
        {
            New, Existing, Congested, Settlement, RevenueSurvey
        }
        public static string subuse;
        public enum m_enuAuthority
        {
            DTCP_GPs, HMDA_ULBs, HMDA_GPs, DTCP_UDAGps, DTCP_ULBs, GHMC, HMDA
        }
        public  enum subAuthority
        {
            //"Municipalities/GP's/NP in HMDA area",  "All municipal corporation", " Municipalities/NP/GP in UDA areas", "Selection&Special Grade Muncipalities, other Muncipalities NP's/GPs"
            
        } 
        public static uint usestate,apptypestate,projtypestate,casetypestate, religousstate, sublocationstate,subauthoritystate;
        public static uint authoritystate;
        public static bool b_PLAN;  // determine this project is PLAN or LAYOUT
        public static bool b_renamelyr; //determine rename or cancel not included layers
        public static string str_srclyrname, str_dstlyrname;
        public static List<string> differentlyrs = new List<string>();
        public System.Drawing.Color[] _colors = new System.Drawing.Color[3]; //to store all the circle colors        
        public const string _defaultBlockName = "Concentric Circles"; //default block name if not set 
        public readonly Point3d _basePointCenter = new Point3d(5, 5, 0);
        public static List<string> lyrName = new List<string>();
        public static List<string> lyrOn = new List<string>();
        public static List<string> lyrFreeze = new List<string>();
        public static List<string> lyrLock = new List<string>();
        public static List<string> lyrColor = new List<string>();
        public static List<string> lyrLinetype = new List<string>();
        public static List<string> lyrLineweight = new List<string>();
        public static List<string> lyrTrans = new List<string>();
        public static List<string> lyrPlotstyle = new List<string>();
        public static List<string> lyrPlot = new List<string>();
        public static List<string> lyrNewVp = new List<string>();
        public static List<string> lyrUse = new List<string>();
        public static bool blnclosed;   // determine 
        public static List<string> unclosedlinelyrNm = new List<string>();
        public static List<string> unclosedlinePtStrt = new List<string>();
        public static List<string> unclosedlinePtEnd = new List<string>();
        public static bool blyrsh;

        /// <summary>
        /// followings are used to rule check
        /// </summary>
        public static List<string> allLayers = new List<string>();
        public static List<Polyline> awindowpline = new List<Polyline>();
        public static List<Polyline> adoorpline = new List<Polyline>();
        public static List<Polyline> aroompline = new List<Polyline>();
        public static List<Polyline> aplotpline = new List<Polyline>();
        public static List<Polyline> amroadpline = new List<Polyline>();
        public static List<Polyline> aindvSubPltpline = new List<Polyline>();
        public static List<Polyline> ainterroadpline = new List<Polyline>();
        public static List<Polyline> aopenspacepline = new List<Polyline>();
        public static List<Polyline> aAmenitypline = new List<Polyline>();
        public static List<Polyline> aMortgageAreapline = new List<Polyline>();
        public static List<Polyline> asplaypline = new List<Polyline>();
        public static List<Polyline> aBufferpline = new List<Polyline>();
        public static List<Polyline> aElectricpline = new List<Polyline>();
        public static List<Polyline> aWaterBodypline = new List<Polyline>();
        public static List<Polyline> aWaterlinepline = new List<Polyline>();
        public static List<Polyline> aLeftownerspline = new List<Polyline>();
        public static List<Polyline> aSurAuthpline = new List<Polyline>();
        public static List<Polyline> aCompndwllpline = new List<Polyline>();
        public static List<Polyline> aElinepline = new List<Polyline>();
        public static List<Polyline> aGllvlpline = new List<Polyline>();
        public static List<Polyline> aFlrinSecpline = new List<Polyline>();
        public static Polyline aPropWrkpline = new Polyline();
        public static List<Polyline> aParkingpline = new List<Polyline>();
        public static List<Polyline> aDrivewaypline = new List<Polyline>();
        public static List<Polyline> arampline = new List<Polyline>();
        public static List<Polyline> aFloorpline = new List<Polyline>();
        public static List<Polyline> aVShaftpline = new List<Polyline>();
        public static List<Polyline> aVoidpline = new List<Polyline>();
        public static List<Polyline> aAccusepline = new List<Polyline>();

        public static List<MText> awindowNmTxt = new List<MText>();
        public static List<MText> aroomNmTxt = new List<MText>();
        public static List<MText> adoorNmTxt = new List<MText>();
        public static List<MText> aplotNmTxt = new List<MText>();
        public static List<MText> amroadNmTxt = new List<MText>();
        public static List<MText> aindvsubPltTxt = new List<MText>();
        public static List<MText> ainterroadTxt = new List<MText>();
        public static List<MText> aopenspaceTxt = new List<MText>();
        public static List<MText> aAmenityTxt = new List<MText>();
        public static List<MText> aMortgageAreaTxt = new List<MText>();
        public static List<MText> asplayTxt = new List<MText>();
        public static List<MText> aBufferTxt = new List<MText>();
        public static List<MText> aElectricTxt = new List<MText>();
        public static List<MText> aWaterBodyTxt = new List<MText>();
        public static List<MText> aWaterlineTxt = new List<MText>();
        public static List<MText> aLeftOwnersTxt = new List<MText>();
        public static List<MText> aSurAuthTxt = new List<MText>();
        public static List<MText> aCmpWallTxt = new List<MText>();
        public static List<MText> aElineTxt = new List<MText>();
        public static List<MText> aGllvlTxt = new List<MText>();
        public static List<MText> aFlrinSecTxt = new List<MText>();
        public static MText aPropWrkTxt = new MText();
        public static List<MText> aParkingTxt = new List<MText>();
        public static List<MText> aDrivewayTxt = new List<MText>();
        public static List<MText> arampTxt = new List<MText>();
        public static List<MText> aFloorTxt = new List<MText>();
        public static List<MText> aVShafttxt = new List<MText>();
        public static List<MText> aVoidTxt = new List<MText>();
        public static List<MText> aAccuseTxt = new List<MText>();

        public static float nCurwidth, nCurheight,nCurDepth;        //door, window width and height
        //Polyline curPline;
        public static double LeftOwnerArea=0, SurroundtoAuthorityArea=0;
        public static string ANexistRdwidth, ANpropRdwidth;      //only use assign name-road
        public static bool bANRd=false;
        public static bool bflrReassign = true;
        public static bool bANPge = false;          //only use assign name-passage
        public static bool bARoom;
        public static string ANPgeitem, ANPgewidth;
        public static string ANRmpwidth, ANRmplngh, ANRmphght,ANrmpitem;      //only use assign name-ramp
        public static bool bANRmp = false;
        public static string ANBNPwing, ANBNPbuilding;          //only use assign name-building and proposed work
        public static bool bANBNP = false;
        public static int bANBNPlnCnt = 0;
        public static Polyline ANBNPpl1, ANBNPpl2;
        public static Transaction ANBnPTrans;
        public static int elinestate = -1;
        public static double fmarginwidth, fmargindepth;
        

        //public static void ConcentricCircles()
        //{            
        //    var cmd = new Commands();
        //    cmd.Initialize();
        //    cmd.hideshowLPM();
        //    cmd.ChangeLayerOfEntitiess();
        //    cmd.CalculateArea();
        //    cmd.MonitorCommandEvents_Method();
        //    Plugin.blnclosed = true;
        //}       
    }
    public class Commands : IExtensionApplication
    {
        public static List<windowrule> awindowrule = new List<windowrule>();
        public static List<doorrule> adoorrule = new List<doorrule>();
        public static List<plotrule> aplotrule = new List<plotrule>();
        public static List<openspacerule> aopenspacerule = new List<openspacerule>();
        public static List<indvSubPlotrule> aindvsubPltrule = new List<indvSubPlotrule>();
        public static List<amenityrule> aAmenrule = new List<amenityrule>();
        public static List<aMortrule> amortrule = new List<aMortrule>();
        public static List<aSplayrule> asplayrule = new List<aSplayrule>();
        public static List<abufferzonerule> abufrule = new List<abufferzonerule>();
        public static List<aLeftownersrule> aleftownerrule = new List<aLeftownersrule>();
        public static List<aSurroundtoAuthorityrule> asurAuthrule = new List<aSurroundtoAuthorityrule>();
        public static List<aCmpWallrule> aCmpwallrule = new List<aCmpWallrule>();
        public static List<aFlorInSec> aFlrsecrule = new List<aFlorInSec>();

        public static List<ruleError> errlist = new List<ruleError>();

        public static List<string> windowerrcause = new List<string>();
        public static List<string> roomerrcause = new List<string>();
        bool bLpmhs;    // This means Layer properties Manager is hide or show.
        public static Polyline curPline;
        private DocumentCollection docMgr = Application.DocumentManager;
        public static string strTemplatePath = "acad.dwt";   //This is autocad template.
        public static bool bNewproj;        // This is true when new project is created with this plugin.
        public static string tmproomName;   // This is RoomName that is selected in RoomNameForm.
        public static string tmpfloorsectionName,tmpfloorName; // This is floorName that is selected in FloorNameForm.
        public static string tmpmarkstring;      // marking mainroad name.
        public static bool brmnamechanged;  // this get room name is selected in RoomNameForm.
        public static string InsdoorName;
        public static string InswindName;
        public static string InsProjstr;
        public const double Pi = 3.141592;
        public int selcount = 0;
        public void Initialize()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            BuildMenuCUI();
            docMgr.DocumentCreated += DocumentCreated;
            bLpmhs = true;
            doc.SendStringToExecute(
              "LAYERCLOSE" + "\n",
              false, false, false);
            bNewproj = false;
            SubscribeToDoc(doc);
            Plugin.elinestate = -1;
        }
        private void DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute("Application" + "\n", false, false, false);
            bLpmhs = true;
            doc.SendStringToExecute(
              "LAYERCLOSE" + "\n",
              false, false, false);
        }
        private static void readExcel()
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;
            string str;
            int rCnt = 0;
            int cCnt = 0;
            string strpath = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Autodesk\\ApplicationPlugins\\Preval.bundle");
            strpath = strpath.Replace(" (x86)", ""); 
            strpath = strpath + "\\Contents\\" + "layer details.xlsx";
            if (File.Exists(strpath))
            {
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(strpath, 0, true, 5, "", "", true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                range = xlWorkSheet.UsedRange;
                for (rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
                {
                    for (cCnt = 2; cCnt <= range.Columns.Count; cCnt++)
                    {
                        str = Convert.ToString((range.Cells[rCnt, cCnt] as Excel.Range).Value2);
                        switch (cCnt)
                        {
                            case 2:
                                Plugin.lyrName.Add(str);
                                break;
                            case 3:
                                Plugin.lyrOn.Add(str);
                                break;
                            case 4:
                                Plugin.lyrFreeze.Add(str);
                                break;
                            case 5:
                                Plugin.lyrLock.Add(str);
                                break;
                            case 6:
                                Plugin.lyrColor.Add(str);
                                break;
                            case 7:
                                Plugin.lyrLinetype.Add(str);
                                break;
                            case 8:
                                Plugin.lyrLineweight.Add(str);
                                break;
                            case 9:
                                Plugin.lyrTrans.Add(str);
                                break;
                            case 10:
                                Plugin.lyrPlotstyle.Add(str);
                                break;
                            case 11:
                                Plugin.lyrPlot.Add(str);
                                break;
                            case 12:
                                Plugin.lyrNewVp.Add(str);
                                break;
                            case 13:
                                Plugin.lyrUse.Add(str);
                                break;
                        }
                    }
                }
                xlWorkBook.Close(true, null, null);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }    
            else
            {
                MessageBox.Show(strpath +" LayerList File does not Exist");
            }
            
        }
        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        public void Terminate()
        {
        }
        public void BuildMenuCUI()
        {
            string str = System.IO.Directory.GetCurrentDirectory();
            string myCuiFile = System.IO.Directory.GetCurrentDirectory() + "\\Preval.cuix";
            string myCuiFileToSend = myCuiFile;
            const string myCuiSectionName = "Vlad";
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string mainCui = Application.GetSystemVariable("MENUNAME") + ".cuix";
            CustomizationSection cs =
              new CustomizationSection(mainCui);
            PartialCuiFileCollection pcfc = cs.PartialCuiFiles;
            if (pcfc.Contains(myCuiFile))
            {
                ed.WriteMessage(
                  "\nCustomization file \""
                  + myCuiFile
                  + "\" already loaded."
                );
            }
            else
            {
                if (System.IO.File.Exists(myCuiFile))
                {
                    ed.WriteMessage(
                      "\nCustomization file \""
                      + myCuiFile
                      + "\" exists - loading it."
                    );
                    LoadMyCui(myCuiFileToSend);
                }
                else
                {
                    ed.WriteMessage(
                      "\nCustomization file \""
                      + myCuiFile
                      + "\" does not exist - building it."
                    );
                    CustomizationSection pcs = new CustomizationSection();
                    pcs.MenuGroupName = myCuiSectionName;
                    // Let's add a menu group, with two commands
                    MacroGroup mg =
                      new MacroGroup(myCuiSectionName, pcs.MenuGroup);
                    MenuMacro mm1 =
                      new MenuMacro(mg, "Cmd 1", "^C^CCmd1", "ID_MyCmd1");
                    MenuMacro mm2 =
                      new MenuMacro(mg, "Cmd 2", "^C^CCmd2", "ID_MyCmd2");

                    // Now let's add a pull-down menu, with two items
                    StringCollection sc = new StringCollection();
                    sc.Add("POP15");
                    PopMenu pm =
                      new PopMenu(
                      myCuiSectionName,
                      sc,
                      "ID_MyPop1",
                      pcs.MenuGroup
                    );
                    PopMenuItem pmi1 =
                      new PopMenuItem(mm1, "Pop Cmd 1", pm, -1);
                    PopMenuItem pmi2 =
                      new PopMenuItem(mm2, "Pop Cmd 2", pm, -1);

                    // Finally we save the file and load it
                    pcs.SaveAs(myCuiFile);
                    LoadMyCui(myCuiFileToSend);
                }
            }
        }
        private void LoadMyCui(string cuiFile)
        {
            Document doc =
                Application.DocumentManager.MdiActiveDocument;
            object oldCmdEcho =
              Application.GetSystemVariable("CMDECHO");
            object oldFileDia =
              Application.GetSystemVariable("FILEDIA");
            Application.SetSystemVariable("CMDECHO", 0);
            Application.SetSystemVariable("FILEDIA", 0);
            doc.SendStringToExecute(
              "_.cuiload "
              + cuiFile
              + " ",
              false, false, false
            );
        }
        public static void AddDoc()
        {
            string strTemplatePath = "acad.dwt";
            DocumentCollection acDocMgr = Application.DocumentManager;
            Document acDoc = acDocMgr.Add(strTemplatePath);
            acDocMgr.MdiActiveDocument = acDoc;
            SignDraw(acDoc);

        }
        public static void SignDraw(Document curdoc)
        {
            var database = curdoc.Database;
            var ed = curdoc.Editor;
            using (DocumentLock docLock = curdoc.LockDocument())
            {
                using (Transaction acTrans = database.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(database.BlockTableId,
                                                           OpenMode.ForRead, false, true) as BlockTable;
                    BlockTableRecord acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                              OpenMode.ForWrite) as BlockTableRecord;
                    MText acMText = new MText();
                    acMText.SetDatabaseDefaults();
                    acMText.Location = new Point3d(2, 2, 0);
                    acMText.Width = 4;
                    acMText.Contents = "Product Sign";
                    acBlkTblRec.AppendEntity(acMText);
                    acTrans.AddNewlyCreatedDBObject(acMText, true);
                    acTrans.Commit();
                }
                ed.UpdateScreen();
            }
        }
        public void ChangeLayerOfEntitiess()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Ask the user for the layer name, allowing
            // spaces to be entered           
            PromptStringOptions pso = new PromptStringOptions("\nEnter name of layer to search for: ");
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);

            if (pr.Status != PromptStatus.OK)
                return;

            string layerName = pr.StringResult;

            // We won't validate whether the layer exists - // we'll just see what's returned by the selection.
            TypedValue[] tvs = new TypedValue[1];
            tvs[0] = new TypedValue((int)DxfCode.LayerName, layerName);
            SelectionFilter sf = new SelectionFilter(tvs);
            PromptSelectionResult psr = ed.SelectAll(sf);
            int count = 0;
            if (psr.Status == PromptStatus.OK)
                count = psr.Value.Count;
            if (psr.Status == PromptStatus.OK || psr.Status == PromptStatus.Error)
            {
                // Display the count of entities on that layer
                ed.WriteMessage(
                  "\nFound {0} entit{1} on layer \"{2}\".",
                  count, count == 1 ? "y" : "ies", layerName);
                // If there are some on this layer,  // prompt for the layer to move them to
                if (count > 0)
                {
                    pso.Message = "\nEnter new layer for these entities " + "or return to leave them alone: ";
                    pr = ed.GetString(pso);
                    if (pr.Status != PromptStatus.OK || pr.StringResult == "")
                        return;
                    string newLayerName = pr.StringResult;
                    Transaction tr = db.TransactionManager.StartTransaction();
                    using (tr)
                    {
                        // This time we do check whether // the layer exists
                        LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                        if (!lt.Has(newLayerName))
                            ed.WriteMessage("\nLayer not found.");
                        else
                        {
                            int changedCount = 0;
                            // We have the layer table open, so let's // get the layer ID and use that
                            ObjectId lid = lt[newLayerName];
                            foreach (ObjectId id in psr.Value.GetObjectIds())
                            {
                                Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                                ent.LayerId = lid;
                                // Could also have used:
                                //  ent.Layer = newLayerName;
                                // but this way is more efficient and cleaner
                                changedCount++;
                            }
                            ed.WriteMessage("\nChanged {0} entit{1} from " +
                              "layer \"{2}\" to layer \"{3}\".", changedCount,
                              changedCount == 1 ? "y" : "ies", layerName,
                              newLayerName);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        private string GetViewName(Vector3d viewDirection)
        {
            double sqrt033 = Math.Sqrt(1.0 / 3.0);
            switch (viewDirection.GetNormal())
            {
                case Vector3d v when v.IsEqualTo(Vector3d.ZAxis): return "Top";
                case Vector3d v when v.IsEqualTo(Vector3d.ZAxis.Negate()): return "Bottom";
                case Vector3d v when v.IsEqualTo(Vector3d.XAxis): return "Right";
                case Vector3d v when v.IsEqualTo(Vector3d.XAxis.Negate()): return "Left";
                case Vector3d v when v.IsEqualTo(Vector3d.YAxis): return "Back";
                case Vector3d v when v.IsEqualTo(Vector3d.YAxis.Negate()): return "Front";
                case Vector3d v when v.IsEqualTo(new Vector3d(sqrt033, sqrt033, sqrt033)): return "NE Isometric";
                case Vector3d v when v.IsEqualTo(new Vector3d(-sqrt033, sqrt033, sqrt033)): return "NW Isometric";
                case Vector3d v when v.IsEqualTo(new Vector3d(-sqrt033, -sqrt033, sqrt033)): return "SW Isometric";
                case Vector3d v when v.IsEqualTo(new Vector3d(sqrt033, -sqrt033, sqrt033)): return "SE Isometric";
                default: return $"Custom View";
            }
        }
        public static void Frontmargin()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            var ed = currentDocument.Editor;
            SetLayerCurrent("_MarginLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect margin line: ");
            options.SetRejectMessage("\nSelected object is no a margin line.");
            options.AddAllowedClass(typeof(Line), true);
            PromptEntityResult result = ed.GetEntity(options);
            using (DocumentLock docLock = currentDocument.LockDocument())
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {
                    //if ((string)Application.GetSystemVariable("clayer") == "_MarginLine")
                    //{
                    if (result.Status == PromptStatus.OK)
                    {
                        Line line = tr.GetObject(result.ObjectId, OpenMode.ForWrite, false) as Line;
                        line.Color = Getcolor("red");
                    }
                    //}
                    tr.Commit();
                }
            }

        }
        public static void Rearmargin()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            var ed = currentDocument.Editor;
            SetLayerCurrent("_MarginLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect margin line: ");
            options.SetRejectMessage("\nSelected object is no a margin line.");
            options.AddAllowedClass(typeof(Line), true);
            PromptEntityResult result = ed.GetEntity(options);
            using (DocumentLock docLock = currentDocument.LockDocument())
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {
                    //if ((string)Application.GetSystemVariable("clayer") == "_MarginLine")
                    //{
                    if (result.Status == PromptStatus.OK)
                    {
                        Line line = tr.GetObject(result.ObjectId, OpenMode.ForWrite, false) as Line;
                        line.Color = Getcolor("magenta");
                    }
                    //}
                    tr.Commit();
                }
            }
        }
        public static void Side1margin()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            var ed = currentDocument.Editor;
            SetLayerCurrent("_MarginLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect margin line: ");
            options.SetRejectMessage("\nSelected object is no a margin line.");
            options.AddAllowedClass(typeof(Line), true);
            PromptEntityResult result = ed.GetEntity(options);
            using (DocumentLock docLock = currentDocument.LockDocument())
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {
                    //if ((string)Application.GetSystemVariable("clayer") == "_MarginLine")
                    //{
                    if (result.Status == PromptStatus.OK)
                    {
                        Line line = tr.GetObject(result.ObjectId, OpenMode.ForWrite, false) as Line;
                        line.Color = Getcolor("blue");
                    }
                    //}
                    tr.Commit();
                }
            }
        }
        public static void Side2margin()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            var ed = currentDocument.Editor;
            SetLayerCurrent("_MarginLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect margin line: ");
            options.SetRejectMessage("\nSelected object is no a margin line.");
            options.AddAllowedClass(typeof(Line), true);
            PromptEntityResult result = ed.GetEntity(options);
            using (DocumentLock docLock = currentDocument.LockDocument())
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {
                    //if ((string)Application.GetSystemVariable("clayer") == "_MarginLine")
                    //{
                    if (result.Status == PromptStatus.OK)
                    {
                        Line line = tr.GetObject(result.ObjectId, OpenMode.ForWrite, false) as Line;
                        line.Color = Getcolor("green");
                    }
                    //}
                    tr.Commit();
                }
            }
        }
        public static double PlotDepth()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            //Point2d pt1, pt2;
            using (Transaction acCurrTrans = db.TransactionManager.StartTransaction())
            {
                SetLayerCurrent("_MarginLine");
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("");
                // Prompt for the start point
                pPtOpts.Message = "\nEnter the start point of the line: ";
                pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                Point3d ptStart = pPtRes.Value;
                // Prompt for the end point
                pPtOpts.Message = "\nEnter the end point of the line: ";
                pPtOpts.UseBasePoint = true;
                pPtOpts.BasePoint = ptStart;
                pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                Point3d ptEnd = pPtRes.Value;
                Plugin.fmargindepth = Math.Abs(ptStart.Y - ptEnd.Y);
                acCurrTrans.Commit();
            }
            return Plugin.fmargindepth;
        }
        public static double PlotWidth()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            //Point2d pt1, pt2;
            using (Transaction acCurrTrans = db.TransactionManager.StartTransaction())
            {
                SetLayerCurrent("_MarginLine");
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("");
                // Prompt for the start point

                pPtOpts.Message = "\nEnter the start point of the line: ";
                pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                Point3d ptStart = pPtRes.Value;
                // Prompt for the end point
                pPtOpts.Message = "\nEnter the end point of the line: ";
                pPtOpts.UseBasePoint = true;
                pPtOpts.BasePoint = ptStart;

                pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                Point3d ptEnd = pPtRes.Value;
                Plugin.fmarginwidth = Math.Abs(ptEnd.X - ptStart.X);
                acCurrTrans.Commit();
            }
            return Plugin.fmarginwidth;
        }
        public static void MarginSave()
        {

        }
        public void GetPolylineEntitiesOnLayer(Database db, string layerName)
        {
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    var PlineCls = RXObject.GetClass(typeof(Polyline));
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            if (id.ObjectClass == PlineCls)
                            {

                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                if (pline.Layer.Equals(layerName, System.StringComparison.CurrentCultureIgnoreCase))
                                {
                                    if (!pline.Closed)
                                    {
                                        Point3d startpt = pline.StartPoint;
                                        Point3d endpt = pline.EndPoint;
                                        Plugin.blnclosed = false;
                                        Plugin.unclosedlinelyrNm.Add(layerName);
                                        Plugin.unclosedlinePtStrt.Add(Convert.ToString(Convert.ToInt32(startpt.X)) + ", " + Convert.ToString(Convert.ToInt32(startpt.Y)));
                                        Plugin.unclosedlinePtEnd.Add(Convert.ToString(Convert.ToInt32(endpt.X)) + ", " + Convert.ToString(Convert.ToInt32(endpt.Y)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void SetLayerCurrent(string curlay)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // Start a transaction
            using (DocumentLock docLock = acDoc.LockDocument())
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Layer table for read
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                       OpenMode.ForRead) as LayerTable;
                    string sLayerName = curlay;
                    if (acLyrTbl.Has(sLayerName) == true)
                    {
                        // Set the layer Center current
                        acCurDb.Clayer = acLyrTbl[sLayerName];
                        // Save the changes
                        acTrans.Commit();
                    }
                }
            }

        }
        public static ObjectId SetLayerTransparency(string layerName, Byte layerTransparency)
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Database db = activeDoc.Database;
            ObjectId layerId = ObjectId.Null;
            bool done = false;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                if (lt.Has(layerName))
                {
                    layerId = lt[layerName];
                    LayerTableRecord ltr = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                    Byte alpha = (Byte)(255 * (100 - layerTransparency) / 100);
                    Transparency trans = new Transparency(alpha);
                    ltr.Transparency = trans;
                    done = true;
                }
                tr.Commit();
            }
            if (done)
            {
                // RefreshEntities(layerId, activeDoc, db);
            }
            return layerId;
        }
        private void RefreshEntities(ObjectId layerId, Document activeDoc, Database db)
        {
            using (DocumentLock docLock = activeDoc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    foreach (ObjectId entityId in btr)
                    {
                        Entity ent = tr.GetObject(entityId, OpenMode.ForRead) as Entity;
                        if (ent.LayerId.Equals(layerId))
                        {
                            ent.UpgradeOpen();
                            ent.RecordGraphicsModified(true);
                        }
                    }
                    tr.Commit();
                }
            }
        }
        public static Color Getcolor(string str)
        {
            Color result = Color.FromColorIndex(ColorMethod.ByAci, 1);
            Regex regex1 = new Regex(@"^[0-9]{1}$");
            Regex regex2 = new Regex(@"^[0-9]{2}$");
            Regex regex3 = new Regex(@"^[0-9]{3}$");
            if (regex1.IsMatch(str) || regex2.IsMatch(str) || regex3.IsMatch(str))
            {
                result = Color.FromColorIndex(ColorMethod.ByAci, (short)Convert.ToInt16(str));
            }
            else
            if (str.Contains(","))
            {
                str = str.Trim();
                str = str.Replace("(", "");
                str = str.Replace(")", "");
                string[] r = str.Split(',');
                //byte r=str.
                result = Color.FromRgb(Convert.ToByte(r[0].Trim()), Convert.ToByte(r[1].Trim()), Convert.ToByte(r[2].Trim()));
            }
            else
            {
                str = str.ToLower();
                switch (str)
                {
                    case "red":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 1);
                        break;
                    case "yellow":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 2);
                        break;
                    case "green":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 3);
                        break;
                    case "cyan":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        break;
                    case "blue":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 5);
                        break;
                    case "magenta":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        break;
                    case "white":
                        result = Color.FromColorIndex(ColorMethod.ByAci, 7);
                        break;
                }
            }
            return result;
        }
        public static bool getbool(string str)
        {
            bool bresult;
            str = str.ToLower();
            if (str == "false")
                bresult = false;
            else
                bresult = true;
            return bresult;
        }
        public static LineWeight GetLwgt(string str)
        {
            LineWeight resultwei = LineWeight.ByLineWeightDefault;
            str = str.Trim();
            str = str.ToLower();
            switch (str)
            {
                case "byblock":
                    resultwei = LineWeight.ByBlock;
                    break;
                case "bydips":
                    resultwei = LineWeight.ByDIPs;
                    break;
                case "bylayer":
                    resultwei = LineWeight.ByLayer;
                    break;
                case "bylineweightdefault":
                    resultwei = LineWeight.ByLineWeightDefault;
                    break;
                case "lineweight000":
                    resultwei = LineWeight.LineWeight000;
                    break;
                case "lineweight005":
                    resultwei = LineWeight.LineWeight005;
                    break;
                case "lineweight009":
                    resultwei = LineWeight.LineWeight009;
                    break;
                case "lineweight013":
                    resultwei = LineWeight.LineWeight013;
                    break;
                case "lineweight015":
                    resultwei = LineWeight.LineWeight015;
                    break;
                case "lineweight018":
                    resultwei = LineWeight.LineWeight018;
                    break;
                case "lineweight020":
                    resultwei = LineWeight.LineWeight020;
                    break;
                case "lineweight025":
                    resultwei = LineWeight.LineWeight025;
                    break;
                case "lineweight030":
                    resultwei = LineWeight.LineWeight030;
                    break;
                case "lineweight035":
                    resultwei = LineWeight.LineWeight035;
                    break;
                case "lineweight040":
                    resultwei = LineWeight.LineWeight040;
                    break;
                case "lineweight050":
                    resultwei = LineWeight.LineWeight050;
                    break;
                case "lineweight053":
                    resultwei = LineWeight.LineWeight053;
                    break;
                case "lineweight060":
                    resultwei = LineWeight.LineWeight060;
                    break;
                case "lineweight070":
                    resultwei = LineWeight.LineWeight070;
                    break;
                case "lineweight080":
                    resultwei = LineWeight.LineWeight080;
                    break;
                case "lineweight090":
                    resultwei = LineWeight.LineWeight090;
                    break;
                case "lineweight100":
                    resultwei = LineWeight.LineWeight100;
                    break;
                case "lineweight106":
                    resultwei = LineWeight.LineWeight106;
                    break;
                case "lineweight120":
                    resultwei = LineWeight.LineWeight120;
                    break;
                case "lineweight140":
                    resultwei = LineWeight.LineWeight140;
                    break;
                case "lineweight158":
                    resultwei = LineWeight.LineWeight158;
                    break;
                case "lineweight200":
                    resultwei = LineWeight.LineWeight200;
                    break;
                case "lineweight211":
                    resultwei = LineWeight.LineWeight211;
                    break;
            }
            return resultwei;
        }
        public static void SubscribeToDoc(AcadDocument doc)
        {
            //doc.CommandEnded += new CommandEventHandler(doc_CommandEnded);
            var ed = doc.Editor;
            ed.SelectionAdded += Obj_Selected;
        }
        static void doc_CommandEnded(object sender, CommandEventArgs e)
        {
            //(sender as AcadDocument).Editor.WriteMessage(string.Format("\nCommand {0} ended.\n", e.GlobalCommandName));
            //var doc = Application.DocumentManager.MdiActiveDocument;
            //var db = doc.Database;
            //var ed = doc.Editor;

            //if (e.GlobalCommandName.ToString() == "PLINE" && (string)Application.GetSystemVariable("clayer") == "_Window")
            //{
            //    Polyline pline = null;
            //    PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            //    options.SetRejectMessage("\nSelected object is no a Polyline.");
            //    options.AddAllowedClass(typeof(Polyline), true);
            //    PromptEntityResult result = ed.GetEntity(options);
            //    if (result.Status == PromptStatus.OK)
            //    {
            //        // at this point we know an entity have been selected and it is a Polyline
            //        using (var tr = db.TransactionManager.StartTransaction())
            //        {
            //            pline = (Polyline)tr.GetObject(result.ObjectId, OpenMode.ForWrite);
            //            curPline = pline;
            //            //Plugin.awindowpline.Add(pline);
            //            tr.Commit();
            //        }
            //    }
            //    if (pline != null)
            //    {
            //        var frm = new WindowSize();
            //        frm.Show();
            //    }
            //}
        }
        static void Obj_Selected(object sender, SelectionAddedEventArgs e)
        {
            var ids = e.AddedObjects.GetObjectIds();
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
        }

        public static void SelectObjectsCalcArea()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect a pline: ");
            peo.SetRejectMessage("\nNot a pline try again: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = ed.GetEntity(peo);
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                double area = 0;
                Polyline p = (Polyline)acTrans.GetObject(per.ObjectId, OpenMode.ForRead);
                area = p.Area;
                ed.WriteMessage("Area is {0}: ", area);
                Application.ShowAlertDialog(Convert.ToString(Convert.ToInt16(area)));
                acTrans.Commit();
            }
        }
        public static List<string> LayersToList(Database db)
        {
            List<string> lstlay = new List<string>();

            LayerTableRecord layer;
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                using (Transaction tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (ObjectId layerId in lt)
                    {
                        layer = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        lstlay.Add(layer.Name);
                    }
                }
            }
            return lstlay;
        }
        public static void ChangeLayerName(string src, string dst)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction acTrans = db.TransactionManager.StartTransaction())
                {
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(db.LayerTableId,
                                                       OpenMode.ForRead) as LayerTable;
                    LayerTableRecord acLyrTblRec;
                    if (acLyrTbl.Has(src) == true && acLyrTbl.Has(dst) == false)
                    {
                        acLyrTblRec = acTrans.GetObject(acLyrTbl[src],
                                              OpenMode.ForWrite) as LayerTableRecord;
                        acLyrTblRec.Name = dst;
                    }
                    acLyrTbl.UpgradeOpen();
                    acTrans.Commit();
                }
            }
        }
        public static void TurnOnLayers(List<string> onlyrnamelist, List<string> offlyrnamelist)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (DocumentLock docLock = acDoc.LockDocument())
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    foreach (string str in onlyrnamelist)
                    {
                        LayerTable acLyrTbl;
                        acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                           OpenMode.ForRead) as LayerTable;
                        LayerTableRecord acLyrTblRec;
                        if (acLyrTbl.Has(str) == true)
                        {
                            acLyrTblRec = acTrans.GetObject(acLyrTbl[str],
                                                  OpenMode.ForWrite) as LayerTableRecord;
                            acLyrTblRec.IsOff = false;
                        }
                        acLyrTbl.UpgradeOpen();
                    }
                    foreach (string str in offlyrnamelist)
                    {
                        LayerTable acLyrTbl;
                        acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                           OpenMode.ForRead) as LayerTable;
                        LayerTableRecord acLyrTblRec;
                        if (acLyrTbl.Has(str) == true)
                        {
                            acLyrTblRec = acTrans.GetObject(acLyrTbl[str],
                                                  OpenMode.ForWrite) as LayerTableRecord;
                            acLyrTblRec.IsOff = true;
                        }
                        acLyrTbl.UpgradeOpen();
                    }
                    acTrans.Commit();
                }
            }
        }

        public static double Angle(Point3d pt1, Point3d pt2)
        {
            return Math.Atan2((pt2.Y - pt1.Y), (pt2.X - pt1.X));
        }
        public static Point3d Polar(Point3d ptBase, double angle, double distance)
        {
            return new Point3d(ptBase.X + (distance * Math.Cos(angle)), ptBase.Y + (distance * Math.Sin(angle)), 0.0);
        }
        public static double Distance(Point3d pt1, Point3d pt2)
        {
            return Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
        }
        public static double WidthRectPolyLine(Point3d pt1, Point3d pt2)
        {
            return (pt2.X - pt1.X);
        }
        public static double HeightRectPolyLine(Point3d pt1, Point3d pt2)
        {
            return (pt2.Y - pt1.Y);
        }
        public static SelectionSet PromptForPolyLineSSet(String prompt)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            TypedValue[] typedValueArray = new TypedValue[1];
            typedValueArray.SetValue(new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"), 0);

            var selectionFilter = new SelectionFilter(typedValueArray);

            var promptSelectionResult = ed.GetSelection(selectionFilter);

            var selectionSet = promptSelectionResult.Value;

            if (promptSelectionResult.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog($"Number of objects selected: " +
                                        $"{selectionSet.Count.ToString()}");
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");

            }
            return selectionSet;
        }
        public static void AddLightweightPolyline(Polyline pl)
        {

            // Get the current document and database
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            // Start a transaction
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Point3d leftpt = pl.GetPoint3dAt(0);
                Point3d upperpt = pl.GetPoint3dAt(0);
                Point3d rightpt = pl.GetPoint3dAt(0);
                Point3d bottompt = pl.GetPoint3dAt(0);

                int cnt1 = pl.NumberOfVertices;
                for (int i = 0; i < cnt1; i++)
                {
                    Point3d curpt = pl.GetPoint3dAt(i);
                    if (curpt.X < leftpt.X)
                        leftpt = curpt;
                    if (curpt.Y < upperpt.Y)
                        upperpt = curpt;
                    if (curpt.X > rightpt.X)
                        rightpt = curpt;
                    if (curpt.Y > bottompt.Y)
                        bottompt = curpt;
                }
                double basewidth = rightpt.X - leftpt.X;
                double baseheight = bottompt.Y - upperpt.Y;
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = tr.GetObject(db.BlockTableId,
                                                   OpenMode.ForRead) as BlockTable;
                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                      OpenMode.ForWrite) as BlockTableRecord;
                // Create a polyline with two segments (3 points)
                Polyline acPoly = new Polyline();
                acPoly.SetDatabaseDefaults();
                acPoly.AddVertexAt(0, new Point2d(leftpt.X + basewidth / 10, upperpt.Y), 0, 0, 0);
                acPoly.AddVertexAt(1, new Point2d(leftpt.X + basewidth / 10 + Plugin.nCurwidth, upperpt.Y), 0, 0, 0);
                acPoly.AddVertexAt(2, new Point2d(leftpt.X + basewidth / 10 + Plugin.nCurwidth, upperpt.Y - Plugin.nCurDepth), 0, 0, 0);
                acPoly.AddVertexAt(3, new Point2d(leftpt.X + basewidth / 10, upperpt.Y - Plugin.nCurDepth), 0, 0, 0);
                acPoly.Closed = true;
                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(acPoly);
                tr.AddNewlyCreatedDBObject(acPoly, true);
                // Save the new object to the database
                TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                ObjectId mtStyleid = db.Textstyle;

                if (ts.Has("Romans"))
                {
                    mtStyleid = ts["Romans"];
                }

                MText txt = new MText();
                string curlayer = (string)Application.GetSystemVariable("clayer");
                switch (curlayer)
                {
                    case "_Door":
                        txt.Contents = Commands.InsdoorName; //<==change to your default string value
                        break;
                    case "_Window":
                        txt.Contents = Commands.InswindName;
                        break;

                }

                txt.SetDatabaseDefaults(db);
                txt.Height = Plugin.nCurheight; //<==change to your default height
                //txt.Rotation = ang;
                txt.Width = Plugin.nCurwidth;
                txt.TextStyleId = mtStyleid;
                txt.TextHeight = Plugin.nCurDepth;
                txt.Attachment = AttachmentPoint.MiddleCenter;
                txt.Location = new Point3d(leftpt.X + basewidth / 10 + Plugin.nCurwidth / 2, upperpt.Y, 0);
                acBlkTblRec.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                tr.Commit();
            }
        }
        public static Point3d Getleft(Polyline pl)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Point3d leftpt = pl.GetPoint3dAt(0);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                int cnt1 = pl.NumberOfVertices;
                for (int i = 0; i < cnt1; i++)
                {
                    Point3d curpt = pl.GetPoint3dAt(i);
                    if (curpt.X < leftpt.X)
                        leftpt = curpt;
                }
                tr.Commit();
            }
            return leftpt;
        }
        public static Point3d Getright(Polyline pl)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Point3d rightpt = pl.GetPoint3dAt(0);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                int cnt1 = pl.NumberOfVertices;
                for (int i = 0; i < cnt1; i++)
                {
                    Point3d curpt = pl.GetPoint3dAt(i);
                    if (curpt.X > rightpt.X)
                        rightpt = curpt;
                }
                tr.Commit();
            }
            return rightpt;
        }
        public static Point3d Gettop(Polyline pl)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Point3d toppt = pl.GetPoint3dAt(0);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                int cnt1 = pl.NumberOfVertices;
                for (int i = 0; i < cnt1; i++)
                {
                    Point3d curpt = pl.GetPoint3dAt(i);
                    if (curpt.Y < toppt.Y)
                        toppt = curpt;
                }
                tr.Commit();
            }
            return toppt;
        }
        public static Point3d Getbottom(Polyline pl)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Point3d bottompt = pl.GetPoint3dAt(0);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                int cnt1 = pl.NumberOfVertices;
                for (int i = 0; i < cnt1; i++)
                {
                    Point3d curpt = pl.GetPoint3dAt(i);
                    if (curpt.Y > bottompt.Y)
                        bottompt = curpt;
                }
                tr.Commit();
            }
            return bottompt;
        }
        public static bool IsOverlapped(Polyline pl)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            bool boverlap = false;
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    var PlineCls = RXObject.GetClass(typeof(Polyline));
                    foreach (ObjectId id in btr)
                    {
                        if (id.ObjectClass == PlineCls)
                        {
                            var curve = (Curve)tr.GetObject(id, OpenMode.ForRead);
                            var points = new Point3dCollection();
                            pl.IntersectWith(curve, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
                            if (points.Count > 0)
                                boverlap = true;
                        }
                    }
                }
            }
            return boverlap;
        }
        public static string GetMTextContent(MText txt)
        {
            string content = "";
            content = txt.Contents;
            return content;
        }
        public static double GetRoadWidth(string str)
        {
            double result = 0;
            int pos = str.IndexOf("M") - 1;
            string strtmp = str.Substring(0, pos);
            result = Convert.ToDouble(strtmp);
            return result;
        }
        private static bool IsincludedinList(string str, List<string> strlist)
        {
            bool bresult = false;
            foreach (string strinst in strlist)
            {
                if (str == strinst)
                {
                    bresult = true;
                    return bresult;
                }
            }
            return bresult;
        }
        public static void CallImplement()
        {
            ImplementANBnP(Plugin.ANBNPpl1, Plugin.ANBNPpl2);
        }
        public static void ImplementANBnP(Polyline pln1, Polyline pln2)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction acCurrTrans = db.TransactionManager.StartTransaction())
                {
                    TextStyleTable ts = (TextStyleTable)acCurrTrans.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    ObjectId mtStyleid = db.Textstyle;
                    if (ts.Has("Romans"))
                    {
                        mtStyleid = ts["Romans"];
                    }
                    if (pln1 != null && pln2 != null)
                    {
                        Point3d leftpt = pln1.GetPoint3dAt(0);
                        Point3d upperpt = pln1.GetPoint3dAt(0);
                        Point3d rightpt = pln1.GetPoint3dAt(0);
                        Point3d bottompt = pln1.GetPoint3dAt(0);

                        int cnt1 = pln1.NumberOfVertices;
                        for (int i = 0; i < cnt1; i++)
                        {
                            Point3d curpt = pln1.GetPoint3dAt(i);
                            if (curpt.X < leftpt.X)
                                leftpt = curpt;
                            if (curpt.Y < upperpt.Y)
                                upperpt = curpt;
                            if (curpt.X > rightpt.X)
                                rightpt = curpt;
                            if (curpt.Y > bottompt.Y)
                                bottompt = curpt;
                        }
                        BlockTableRecord btr = (BlockTableRecord)acCurrTrans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        DBText txt = new DBText();
                        txt.TextString = Plugin.ANBNPwing + Plugin.ANBNPbuilding;
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 0.5; //<==change to your default height
                                          //txt.Height = HeightRectPolyLine(upperpt, bottompt) / 8.0; //<==change to your default height
                                          //txt.Width = WidthRectPolyLine(leftpt, rightpt);
                        txt.TextStyleId = mtStyleid;
                        //txt.Height = txt.Height / 3.0;

                        //txt.AlignmentPoint = TextHorizontalMode.TextCenter;
                        txt.Layer = pln1.Layer;
                        txt.Position = new Point3d(leftpt.X + WidthRectPolyLine(leftpt, rightpt) / 2, upperpt.Y + HeightRectPolyLine(upperpt, bottompt) / 2, 0);
                        btr.AppendEntity(txt);
                        acCurrTrans.AddNewlyCreatedDBObject(txt, true);

                        int cnt2 = pln2.NumberOfVertices;
                        leftpt = pln2.GetPoint3dAt(0);
                        upperpt = pln2.GetPoint3dAt(0);
                        rightpt = pln2.GetPoint3dAt(0);
                        bottompt = pln2.GetPoint3dAt(0);
                        for (int i = 0; i < cnt2; i++)
                        {
                            Point3d curpt = pln2.GetPoint3dAt(i);
                            if (curpt.X < leftpt.X)
                                leftpt = curpt;
                            if (curpt.Y < upperpt.Y)
                                upperpt = curpt;
                            if (curpt.X > rightpt.X)
                                rightpt = curpt;
                            if (curpt.Y > bottompt.Y)
                                bottompt = curpt;
                        }

                        BlockTableRecord btr1 = (BlockTableRecord)acCurrTrans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        DBText txt1 = new DBText();
                        txt1.TextString = Plugin.ANBNPwing + Plugin.ANBNPbuilding;
                        txt1.SetDatabaseDefaults(db);
                        txt1.Height = 0.5; //<==change to your default height
                                           //txt1.Height = HeightRectPolyLine(upperpt, bottompt) / 8.0; //<==change to your default height
                        txt1.TextStyleId = mtStyleid;
                        //txt1.Attachment = AttachmentPoint.TopCenter;
                        txt1.Layer = pln2.Layer;
                        txt1.Position = new Point3d(leftpt.X, upperpt.Y, 0);
                        btr1.AppendEntity(txt1);
                        acCurrTrans.AddNewlyCreatedDBObject(txt1, true);
                    }
                    acCurrTrans.Commit();
                }
            }
        }
        [CommandMethod("crtlyrs")]
        public static void makingLayers()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            ObjectId layerId = ObjectId.Null;
            readExcel();
            using (DocumentLock docLock = currentDocument.LockDocument())
            {
                using (Transaction acTrans = database.TransactionManager.StartTransaction())
                {
                    // Open the Layer table for read
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(database.LayerTableId,
                                                 OpenMode.ForRead) as LayerTable;

                    for (int i = 0; i < Plugin.lyrName.Count; i++)
                    {
                        if (acLyrTbl.Has(Plugin.lyrName[i]) == false)
                        {
                            LayerTableRecord acLyrTblRec = new LayerTableRecord();
                            acLyrTblRec.Name = Plugin.lyrName[i];
                            acLyrTblRec.IsOff = getbool(Plugin.lyrOn[i]);
                            acLyrTblRec.IsFrozen = getbool(Plugin.lyrFreeze[i]);
                            acLyrTblRec.IsLocked = getbool(Plugin.lyrLock[i]);
                            acLyrTblRec.Color = Getcolor(Plugin.lyrColor[i]);

                            LinetypeTable acLinTbl;
                            acLinTbl = acTrans.GetObject(database.LinetypeTableId,
                                                               OpenMode.ForRead) as LinetypeTable;
                            if (acLinTbl.Has(Plugin.lyrLinetype[i]) == true)
                            {
                                acLyrTblRec.LinetypeObjectId = acLinTbl[Plugin.lyrLinetype[i]];
                            }
                            acLyrTblRec.LineWeight = GetLwgt(Plugin.lyrLineweight[i]);

                            using (PlaceHolder hldr = new PlaceHolder())
                            {
                                DictionaryWithDefaultDictionary d =
                                  (DictionaryWithDefaultDictionary)acTrans.GetObject
                                      (database.PlotStyleNameDictionaryId, OpenMode.ForWrite);
                                d.SetAt(Plugin.lyrPlotstyle[i], hldr);
                            }
                            acLyrTblRec.IsPlottable = getbool(Plugin.lyrPlot[i]);
                            acLyrTblRec.ViewportVisibilityDefault = getbool(Plugin.lyrNewVp[i]);
                            // Upgrade the Layer table for write
                            acLyrTbl.UpgradeOpen();

                            // Append the new layer to the Layer table and the transaction
                            acLyrTbl.Add(acLyrTblRec);
                            acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                        }
                    }
                    acTrans.Commit();
                }
                for (int i = 0; i < Plugin.lyrName.Count; i++)
                {
                    SetLayerTransparency(Plugin.lyrName[i], (byte)Convert.ToInt32(Plugin.lyrTrans[i]));
                }
            }
            SetLayerCurrent("0");
        }

        [CommandMethod("lyrsh")]
        public void LayersShowHide()
        {
            //    string strpath = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Autodesk\\ApplicationPlugins\\Preval.bundle");
            //    strpath = strpath.Replace(" (x86)", "");
            //    strpath = strpath + "\\" + "layer details.xlsx";
            //    MessageBox.Show(strpath);
            if (!Plugin.blyrsh)
            {
                Plugin.blyrsh = true;
                var frm = new LayersShowHideForm();
                frm.Show();
            }
        }
        [CommandMethod("rnlyrs")]
        public void RenameNotincludedlayers()
        {
            Plugin.b_renamelyr = false;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            List<string> tmplst = LayersToList(db);
            var list1 = tmplst.Except(Plugin.lyrName);
            foreach (string str in list1)
            {
                Plugin.differentlyrs.Add(str);
            }
            var frm = new LayerRenameForm();
            frm.Show();
        }

        [CommandMethod("calcarea")]
        public void CalculateArea()
        {
            SelectObjectsCalcArea();
        }
        [CommandMethod("chkcls")]
        public void CheckAllClosed()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            foreach (string lyrnm in Plugin.lyrName)
            {
                GetPolylineEntitiesOnLayer(acCurDb, lyrnm);
            }
            //if(Plugin.blnclosed)
            var frm = new PlineCloseFrm();
            frm.Show();
            frm.Show_LineCloseResult();
        }

        [CommandMethod("viewchk")]
        public void ViewCheck()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Ask the user for the layer name, allowing
            using (var view = ed.GetCurrentView())
            {
                ed.WriteMessage($"\nCurrent View: {GetViewName(view.ViewDirection)}");
            }
        }
        [CommandMethod("nbcrule")]
        public void NBCRULECheck()
        {
            //var frm = new NBCruleCheck();
            //frm.Show();
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.CurrentDocument;
            var database = currentDocument.Database;
            Plugin.allLayers = Commands.LayersToList(database);
            NBCrelate.Rulecheck();
            var frm = new RuleCheckForm();
            frm.Show();
        }
        [CommandMethod("openpro")]
        public void OpenProj()
        {
            bNewproj = false;
            string sourceFileName = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Drawing Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dwg",
                Filter = "Drawing files (*.dwg)|*.dwg",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceFileName = openFileDialog1.FileName;
            }
            DocumentCollection acDocMgr = Application.DocumentManager;

            if (File.Exists(sourceFileName))
            {
                Document curdoc = acDocMgr.Open(sourceFileName, false);
                acDocMgr.MdiActiveDocument = curdoc;
                curdoc.SendStringToExecute("Application" + "\n", false, false, false);
                bLpmhs = true;
                curdoc.SendStringToExecute(
                  "LAYERCLOSE" + "\n",
                  false, false, false);
                //SignDraw(curdoc);
            }
            else
            {
                acDocMgr.MdiActiveDocument.Editor.WriteMessage("File " + sourceFileName +
                                                                     " does not exist.");
            }
        }

        [CommandMethod("mmg")]
        public void MarkMargin()
        {
            var frm = new NBCLayers.MarginForm();
            frm.Show();
        }

        [CommandMethod("nbcsave")]
        public void NbcSave()
        {
            var documentManager = Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            var ed = currentDocument.Editor;

            SaveFileDialog svdlg = new SaveFileDialog();
            svdlg.Filter = "Drawing files (*.dwg)|*.dwg|All files (*.*)|*.*";
            svdlg.FilterIndex = 2;
            svdlg.RestoreDirectory = true;
            string str = "";
            if (svdlg.ShowDialog() == DialogResult.OK)
            {
                str = svdlg.FileName;
            }
            if (str != "")
                database.SaveAs(str, DwgVersion.Current);
        }
        [CommandMethod("hsl")]
        public void hideshowLPM()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            object oldCmdEcho = Application.GetSystemVariable("CMDECHO");
            if (bLpmhs)
            {
                doc.SendStringToExecute(
              "LAYER" + "\n",
              false, false, false);
                bLpmhs = !bLpmhs;
            }
            else
            {
                doc.SendStringToExecute(
              "LAYERCLOSE" + "\n",
              false, false, false);
                bLpmhs = !bLpmhs;
            }
        }
        [CommandMethod("newpro")]
        public void NewProj()
        {
            var authrFrm = new projectinForm();
            authrFrm.Show();
            //makingLayers();
        }

        [CommandMethod("Application")]
        public void MonitorCommandEvents_Method()
        {
            SubscribeToDoc(Application.DocumentManager.MdiActiveDocument);
        }

        [CommandMethod("RoomRename")]               ///////Assign Name Module   /////////////////
        public void RenameRoom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Plugin.bARoom = true;
            SetLayerCurrent("_Room");
            var frm = new RoomNameForm();
            frm.Show();
            //PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            //options.SetRejectMessage("\nSelected object is no a Polyline.");
            //options.AddAllowedClass(typeof(Polyline), true);
            //PromptEntityResult result = ed.GetEntity(options);
            var resultSet = PromptForPolyLineSSet("Select the PolyLines to name Room");
            ObjectId[] oids = resultSet.GetObjectIds();
            if ((string)Application.GetSystemVariable("clayer") == "_Room")
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    int i = 0;
                    foreach (SelectedObject obj in resultSet)
                    {
                        Polyline poly = tr.GetObject(obj.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
                        {
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmproomName + "\n" + Math.Round(Convert.ToDouble(WidthRectPolyLine(min, max)), 1).ToString() + " X " + Math.Round(Convert.ToDouble(HeightRectPolyLine(min, max)), 1).ToString(); //<==change to your default string value
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                                                 //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                    }

                    i++;
                    tr.Commit();
                }
                Commands.brmnamechanged = false;
            }
        }

        [CommandMethod("assignroad")]
        public void assignroad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            var frm = new NBCLayers.RoadAName();
            frm.Show();
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Plugin.ANexistRdwidth + " " + Plugin.ANpropRdwidth + "ROAD";
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                            double rdwidth = GetRoadWidth(txt.Contents);
                        }
                        tr.Commit();
                    }
                }
            }
            Plugin.bANRd = false;

        }

        [CommandMethod("assignpassage")]
        public void assignpassage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Passage");
            var frm = new NBCLayers.ANPassage();
            frm.Show();
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Passage")
            {
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Plugin.ANPgewidth + " " + Plugin.ANPgeitem;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                    Commands.brmnamechanged = false;
                }
            }
            Plugin.bANPge = false;
        }

        [CommandMethod("assignbuilding")]
        public void assignbuilding()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Plugin.bANBNP = true;
            Polyline pln1, pln2;
            TypedValue[] acTypedValueArray = new TypedValue[2];
            acTypedValueArray.SetValue(new TypedValue(0 /*(int)DxfCode.Start*/, "LWPOLYLINE"), 0);
            SelectionFilter acSelectionFilter = new SelectionFilter(acTypedValueArray);
            MessageBox.Show("First Select a _BuildingName Layer Polyline.");
            PromptSelectionOptions acPoptions = new PromptSelectionOptions
            {
                SingleOnly = false,
                SinglePickInSpace = false

            };
            using (Transaction acCurrTrans = db.TransactionManager.StartTransaction())
            {
                PromptEntityOptions options = new PromptEntityOptions("\nSelect a BuildingName Polyline: ");
                options.SetRejectMessage("\nSelected object is no a Polyline.");
                options.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult result = ed.GetEntity(options);
                SetLayerCurrent("_BuildingName");
                if (result.Status == PromptStatus.OK)
                {
                    pln1 = acCurrTrans.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    Plugin.ANBNPpl1 = pln1;
                    MessageBox.Show("_BuildingName Polyline OK, Select a _ProposedWork Polyline.");
                    SetLayerCurrent("_ProposedWork");
                    PromptEntityOptions options1 = new PromptEntityOptions("\nSelect a ProposedWork Polyline: ");
                    options1.SetRejectMessage("\nSelected object is no a Polyline.");
                    options1.AddAllowedClass(typeof(Polyline), true);
                    PromptEntityResult result1 = ed.GetEntity(options1);
                    if (result1.Status == PromptStatus.OK)
                    {
                        pln2 = acCurrTrans.GetObject(result1.ObjectId, OpenMode.ForRead, false) as Polyline;
                        Plugin.ANBNPpl2 = pln2;
                        Plugin.ANBnPTrans = acCurrTrans;
                        var frm = new ANBnPropWork();
                        frm.Show();
                        ImplementANBnP(Plugin.ANBNPpl1, Plugin.ANBNPpl2);
                    }
                }
                acCurrTrans.Commit();
            }
        }

        [CommandMethod("AssignFloorName")]
        public void AssignFloorNames()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
        repeat: var frm = new NBCLayers.FloorNameForm();
            frm.Show();
            SetLayerCurrent("_FloorInSection");
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityOptions options = new PromptEntityOptions("\nSelect a FloorInSection Polyline: ");
                options.SetRejectMessage("\nSelected object is no a Polyline.");
                options.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult result = ed.GetEntity(options);
                if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
                {
                    if (result.Status == PromptStatus.OK)
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpfloorsectionName;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.BottomLeft;
                            txt.Location = new Point3d(ptleft.X, pttop.Y, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }

                    }
                }
                SetLayerCurrent("_Floor");
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
                        if (poly1 != null)
                        {
                            TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                            ObjectId mtStyleid = db.Textstyle;
                            var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            if (ts.Has("Romans"))
                            {
                                mtStyleid = ts["Romans"];
                            }
                            Point3d pickPoint = result.PickedPoint;
                            Point3d oPoint = poly1.GetClosestPointTo(pickPoint, ed.GetCurrentView().ViewDirection, false);
                            double param = 0;
                            param = poly1.GetParameterAtPoint(oPoint);
                            double sparam = 0, eparam = 0;
                            sparam = (int)param;
                            eparam = sparam + 1;
                            Point3d sp = poly1.GetPointAtParameter(sparam);
                            Point3d ep = poly1.GetPointAtParameter(eparam);
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly1.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpfloorName;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly1);
                            Point3d ptright = Getright(poly1);
                            Point3d pttop = Gettop(poly1);
                            Point3d ptbottom = Getbottom(poly1);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.BottomLeft;
                            txt.Location = new Point3d(ptleft.X, pttop.Y, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                    }
                }
                tr.Commit();
            }
            DialogResult dresult = MessageBox.Show("Do you want to assign more floor name?", "AutoCAD", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dresult == DialogResult.Yes)
                goto repeat;
        }

        [CommandMethod("assignramp")]
        public void assignramp()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Ramp");
            var frm = new NBCLayers.ANRamp();
            frm.Show();
            if (frm.DialogResult != DialogResult.OK)
                return;
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Ramp")
            {
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Plugin.ANRmpwidth + " " + Plugin.ANRmplngh + " " + Plugin.ANRmphght + " " + Plugin.ANrmpitem;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        /// <summary>
        /// Assign Name Module End//////////////
        /// </summary>
        [CommandMethod("InstWindow")]               ////////////Insert Menu Module
        public void InsertWindow()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Window");
            var frm = new NBCLayers.WindowSizeFrm();
            frm.Show();
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Window")
            {
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    if (result.Status == PromptStatus.OK)
                    {
                        // at this point we know an entity have been selected and it is a Polyline
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                            if (poly != null)
                            {
                                TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                                ObjectId mtStyleid = db.Textstyle;
                                AddLightweightPolyline(poly);
                            }
                            tr.Commit();
                        }
                    }
                }
            }
        }
        [CommandMethod("InstDoor")]
        public void InsertDoor()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Door");
            var frm = new ProsoftAcPlugin.DoorSizeFrm();
            frm.Show();

            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);

            if ((string)Application.GetSystemVariable("clayer") == "_Door")
            {
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
                        {
                            TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                            ObjectId mtStyleid = db.Textstyle;
                            AddLightweightPolyline(poly);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("InstBuTem")]
        public void InstBuTem()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_BuildingName");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_BuildingName")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }
                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                        txt.Width = 10;
                        txt.Contents = "Building Template";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("InstRefCir")]
        public void InstRefCir()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Floor");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nEnter the center point of the line: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Autodesk.AutoCAD.DatabaseServices.Group tmpgrp = new Autodesk.AutoCAD.DatabaseServices.Group("", true);
                    if ((string)Application.GetSystemVariable("clayer") == "_Floor")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        // Create a circle that is at 2,3 with a radius of 4.25
                        Circle acCirc = new Circle();
                        acCirc.SetDatabaseDefaults();
                        acCirc.Center = ptCenter;
                        acCirc.Radius = 0.075;
                        acBlkTblRec.AppendEntity(acCirc);
                        tr.AddNewlyCreatedDBObject(acCirc, true);
                        //tmpgrp.Append(acCirc.ObjectId);

                        Circle acCirc1 = new Circle();
                        acCirc1.SetDatabaseDefaults();
                        acCirc1.Center = ptCenter;
                        acCirc1.Radius = 0.15;

                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(acCirc1);
                        tr.AddNewlyCreatedDBObject(acCirc1, true);
                        //tmpgrp.Append(acCirc1.ObjectId);

                        Polyline horPoly = new Polyline();
                        horPoly.SetDatabaseDefaults();
                        horPoly.AddVertexAt(0, new Point2d(ptCenter.X - acCirc1.Radius, ptCenter.Y), 0, 0, 0);
                        horPoly.AddVertexAt(1, new Point2d(ptCenter.X + acCirc1.Radius, ptCenter.Y), 0, 0, 0);
                        horPoly.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(horPoly);
                        tr.AddNewlyCreatedDBObject(horPoly, true);
                        //tmpgrp.Append(horPoly.ObjectId);

                        Polyline verrPoly = new Polyline();
                        verrPoly.SetDatabaseDefaults();
                        verrPoly.AddVertexAt(0, new Point2d(ptCenter.X, ptCenter.Y - acCirc1.Radius), 0, 0, 0);
                        verrPoly.AddVertexAt(1, new Point2d(ptCenter.X, ptCenter.Y + acCirc1.Radius), 0, 0, 0);
                        verrPoly.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(verrPoly);
                        tr.AddNewlyCreatedDBObject(verrPoly, true);
                        ObjectIdCollection ids = new ObjectIdCollection();
                        ids.Add(acCirc.Id);
                        ids.Add(acCirc1.Id);
                        ids.Add(verrPoly.Id);
                        ids.Add(horPoly.Id);
                        tmpgrp.InsertAt(0, ids);
                        //tmpgrp.Append(verrPoly.ObjectId);
                    }
                    SetLayerCurrent("_ResiBUAOutline");
                    if ((string)Application.GetSystemVariable("clayer") == "_ResiBUAOutline")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        Circle acCirc1 = new Circle();
                        acCirc1.SetDatabaseDefaults();
                        acCirc1.Center = new Point3d(ptCenter.X + 1, ptCenter.Y, 0);
                        acCirc1.Radius = 0.15;

                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(acCirc1);
                        tr.AddNewlyCreatedDBObject(acCirc1, true);
                        tmpgrp.Append(acCirc1.ObjectId);

                        Polyline horPoly = new Polyline();
                        horPoly.SetDatabaseDefaults();
                        horPoly.AddVertexAt(0, new Point2d(ptCenter.X + 1 - acCirc1.Radius, ptCenter.Y), 0, 0, 0);
                        horPoly.AddVertexAt(1, new Point2d(ptCenter.X + 1 + acCirc1.Radius, ptCenter.Y), 0, 0, 0);
                        horPoly.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(horPoly);
                        tr.AddNewlyCreatedDBObject(horPoly, true);
                        tmpgrp.Append(horPoly.ObjectId);

                        Polyline verrPoly = new Polyline();
                        verrPoly.SetDatabaseDefaults();
                        verrPoly.AddVertexAt(0, new Point2d(ptCenter.X + 1, ptCenter.Y - acCirc1.Radius), 0, 0, 0);
                        verrPoly.AddVertexAt(1, new Point2d(ptCenter.X + 1, ptCenter.Y + acCirc1.Radius), 0, 0, 0);
                        verrPoly.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(verrPoly);
                        tr.AddNewlyCreatedDBObject(verrPoly, true);
                        tmpgrp.Append(verrPoly.ObjectId);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("InstNDir")]
        public void InstNDir()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nEnter the center point of the north Point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Autodesk.AutoCAD.DatabaseServices.Group tmpgrp = new Autodesk.AutoCAD.DatabaseServices.Group("", true);
                    if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        // Create a circle that is at 2,3 with a radius of 4.25
                        Circle acCirc = new Circle();
                        acCirc.SetDatabaseDefaults();
                        acCirc.Center = ptCenter;
                        acCirc.Radius = 2;
                        acBlkTblRec.AppendEntity(acCirc);
                        tr.AddNewlyCreatedDBObject(acCirc, true);
                        tmpgrp.Append(acCirc.ObjectId);

                        Circle acCirc1 = new Circle();
                        acCirc1.SetDatabaseDefaults();
                        acCirc1.Center = ptCenter;
                        acCirc1.Radius = 3;

                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(acCirc1);
                        tr.AddNewlyCreatedDBObject(acCirc1, true);
                        tmpgrp.Append(acCirc1.ObjectId);

                        Circle acCirc2 = new Circle();
                        acCirc2.SetDatabaseDefaults();
                        acCirc2.Center = ptCenter;
                        acCirc2.Radius = 6;

                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(acCirc2);
                        tr.AddNewlyCreatedDBObject(acCirc2, true);
                        tmpgrp.Append(acCirc2.ObjectId);

                        Polyline horPolyL = new Polyline();
                        horPolyL.SetDatabaseDefaults();
                        horPolyL.AddVertexAt(0, new Point2d(ptCenter.X - acCirc.Radius, ptCenter.Y), 0, 0, 0);
                        horPolyL.AddVertexAt(1, new Point2d(ptCenter.X - acCirc2.Radius, ptCenter.Y), 0, 0, 0);
                        horPolyL.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(horPolyL);
                        tr.AddNewlyCreatedDBObject(horPolyL, true);
                        tmpgrp.Append(horPolyL.ObjectId);

                        Polyline horPolyR = new Polyline();
                        horPolyR.SetDatabaseDefaults();
                        horPolyR.AddVertexAt(0, new Point2d(ptCenter.X + acCirc.Radius, ptCenter.Y), 0, 0, 0);
                        horPolyR.AddVertexAt(1, new Point2d(ptCenter.X + acCirc2.Radius, ptCenter.Y), 0, 0, 0);
                        horPolyR.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(horPolyR);
                        tr.AddNewlyCreatedDBObject(horPolyR, true);
                        tmpgrp.Append(horPolyR.ObjectId);

                        Polyline verrPolyL = new Polyline();
                        verrPolyL.SetDatabaseDefaults();
                        verrPolyL.AddVertexAt(0, new Point2d(ptCenter.X, ptCenter.Y - acCirc.Radius), 0, 0, 0);
                        verrPolyL.AddVertexAt(1, new Point2d(ptCenter.X, ptCenter.Y - acCirc2.Radius), 0, 0, 0);
                        verrPolyL.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(verrPolyL);
                        tr.AddNewlyCreatedDBObject(verrPolyL, true);
                        tmpgrp.Append(verrPolyL.ObjectId);

                        Polyline verrPolyR = new Polyline();
                        verrPolyR.SetDatabaseDefaults();
                        verrPolyR.AddVertexAt(0, new Point2d(ptCenter.X, ptCenter.Y + acCirc.Radius), 0, 0, 0);
                        verrPolyR.AddVertexAt(1, new Point2d(ptCenter.X, ptCenter.Y + acCirc2.Radius), 0, 0, 0);
                        verrPolyR.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(verrPolyR);
                        tr.AddNewlyCreatedDBObject(verrPolyR, true);
                        tmpgrp.Append(verrPolyR.ObjectId);

                        Polyline hypo = new Polyline();
                        hypo.SetDatabaseDefaults();
                        hypo.AddVertexAt(0, new Point2d(ptCenter.X - acCirc2.Radius, ptCenter.Y), 0, 0, 0);
                        hypo.AddVertexAt(1, new Point2d(ptCenter.X, ptCenter.Y + acCirc2.Radius), 0, 0, 0);
                        hypo.AddVertexAt(2, new Point2d(ptCenter.X + acCirc2.Radius, ptCenter.Y), 0, 0, 0);
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(hypo);
                        tr.AddNewlyCreatedDBObject(hypo, true);
                        tmpgrp.Append(hypo.ObjectId);

                        Polyline Npl = new Polyline();
                        Npl.SetDatabaseDefaults();
                        Npl.AddVertexAt(0, new Point2d(ptCenter.X - acCirc.Radius / 2, ptCenter.Y - acCirc.Radius * Math.Asin(0.5)), 0, 0, 0);
                        Npl.AddVertexAt(1, new Point2d(ptCenter.X - acCirc.Radius / 2, ptCenter.Y + acCirc.Radius * Math.Asin(0.5)), 0, 0, 0);
                        Npl.AddVertexAt(2, new Point2d(ptCenter.X + acCirc.Radius / 2, ptCenter.Y - acCirc.Radius * Math.Asin(0.5)), 0, 0, 0);
                        Npl.AddVertexAt(3, new Point2d(ptCenter.X + acCirc.Radius / 2, ptCenter.Y + acCirc.Radius * Math.Asin(0.5)), 0, 0, 0);
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(Npl);
                        tr.AddNewlyCreatedDBObject(Npl, true);
                        tmpgrp.Append(Npl.ObjectId);

                        ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                        acObjIdColl.Add(acCirc1.ObjectId);
                        acObjIdColl.Add(hypo.ObjectId);

                        // Create the hatch object and append it to the block table record
                        Hatch acHatch = new Hatch();
                        acHatch.SetDatabaseDefaults();
                        acBlkTblRec.AppendEntity(acHatch);
                        tr.AddNewlyCreatedDBObject(acHatch, true);
                        acHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                        acHatch.Associative = true;
                        //acHatch.AppendLoop(HatchLoopTypes.Outermost, acObjIdColl);
                        //acHatch.EvaluateHatch(true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("insertsection")]
        public void insertsection()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SectionLine");
            var frm = new SecLineFrm();
            frm.Show();
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nEnter the Start point of the Section line: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptstart = pPtRes.Value;
            if ((string)Application.GetSystemVariable("clayer") == "_SectionLine")
            {
                if (pPtRes.Status == PromptStatus.OK)
                {
                    PromptPointResult pPtRes1;
                    PromptPointOptions pPtOpts1 = new PromptPointOptions("");
                    // Prompt for the start point
                    pPtOpts1.Message = "\nEnter the End point of the Section line: ";
                    pPtRes1 = ed.GetPoint(pPtOpts);
                    Point3d ptend = pPtRes1.Value;
                    if (pPtRes1.Status == PromptStatus.OK)
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            BlockTable acBlkTbl;
                            acBlkTbl = tr.GetObject(db.BlockTableId,
                                                         OpenMode.ForRead) as BlockTable;

                            // Open the Block table record Model space for write
                            BlockTableRecord acBlkTblRec;
                            acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                            OpenMode.ForWrite) as BlockTableRecord;

                            Polyline secline = new Polyline();
                            secline.SetDatabaseDefaults();
                            secline.AddVertexAt(0, new Point2d(ptstart.X, ptstart.Y), 0, 0, 0);
                            secline.AddVertexAt(1, new Point2d(ptend.X, ptstart.Y), 0, 0, 0);
                            secline.Closed = true;
                            // Add the new object to the block table record and the transaction
                            acBlkTblRec.AppendEntity(secline);
                            tr.AddNewlyCreatedDBObject(secline, true);

                            TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                            ObjectId mtStyleid = db.Textstyle;

                            if (ts.Has("Romans"))
                            {
                                mtStyleid = ts["Romans"];
                            }

                            MText txt = new MText();
                            txt.SetDatabaseDefaults(db);
                            txt.Height = 5; //<==change to your default height
                                            //txt.Rotation = ang;
                            txt.Width = 10;
                            txt.Contents = "Section Line";
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptstart.X + (ptend.X - ptstart.X) / 2, ptstart.Y + 2, 0);
                            acBlkTblRec.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);

                            tr.Commit();

                        }
                    }
                }
            }

        }

        [CommandMethod("inserttitle")]
        public void inserttitle()               ///////////This needs fixing./////////////////////
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            var frm = new NBCLayers.Projtittle();
            frm.Show();
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point of the Tittle: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        Polyline rectangle = new Polyline();
                        rectangle.SetDatabaseDefaults();
                        rectangle.AddVertexAt(0, new Point2d(ptCenter.X, ptCenter.Y), 0, 0, 0);
                        rectangle.AddVertexAt(1, new Point2d(ptCenter.X + 60, ptCenter.Y), 0, 0, 0);
                        rectangle.AddVertexAt(2, new Point2d(ptCenter.X + 60, ptCenter.Y + 20), 0, 0, 0);
                        rectangle.AddVertexAt(3, new Point2d(ptCenter.X, ptCenter.Y + 20), 0, 0, 0);
                        //rectangle.AddVertexAt(1, new Point2d(ptCenter.X , ptCenter.Y+10), 0, 0, 0);
                        rectangle.Closed = true;
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(rectangle);
                        tr.AddNewlyCreatedDBObject(rectangle, true);

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 20; //<==change to your default height
                                         //txt.Rotation = ang;
                        txt.Width = 30;
                        txt.Contents = Commands.InsProjstr;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X + 30, ptCenter.Y + 15, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }
        [CommandMethod("InstTree")]
        public void InstTree()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Tree");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point of the Tree: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Tree")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Tree";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("scooter")]
        public void scooter()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Scooter";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("car")]
        public void car()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Car";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("transportvechicle")]
        public void transportvechicle()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Transport Vechicle";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("busparking")]
        public void busparking()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Bus Parking";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("cycle")]
        public void cycle()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Cycle";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("physicallyhandicapped")]
        public void physicallyhandicapped()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Parking")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Physically Handicapped";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("ablution")]
        public void ablution()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Ablution Tap";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("kitchensink")]
        public void kitchensink()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Kitchen Sink";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("washbasin")]
        public void washbasin()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Wash Basin";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("washingsink")]
        public void washingsink()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Washing Sink";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("cleanersink")]
        public void cleanersink()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Cleaner Sink";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("urinal")]
        public void urinal()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Urinal";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("drinkingwater")]
        public void drinkingwater()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Drinking Water";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }
        [CommandMethod("washingtap")]
        public void washingtap()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Sanitation");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_Sanitation")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;

                        TextStyleTable ts = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                        ObjectId mtStyleid = db.Textstyle;

                        if (ts.Has("Romans"))
                        {
                            mtStyleid = ts["Romans"];
                        }

                        MText txt = new MText();
                        txt.SetDatabaseDefaults(db);
                        txt.Height = 5; //<==change to your default height
                                        //txt.Rotation = ang;
                        txt.Width = 10;
                        txt.Contents = "Washing Tap";
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptCenter.X, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("stairup")]
        public void stairup()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point of the Tree: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;
                        Leader leader = new Leader();
                        leader.SetDatabaseDefaults();
                        leader.AppendVertex(ptCenter);
                        leader.AppendVertex(new Point3d(ptCenter.X + 2, ptCenter.Y, 0));
                        leader.HasArrowHead = true;
                        // Add the new object to Model space and the transaction
                        acBlkTblRec.AppendEntity(leader);
                        tr.AddNewlyCreatedDBObject(leader, true);
                        MText mText = new MText();

                        mText.SetDatabaseDefaults();
                        mText.Width = 20;
                        mText.Height = 10;
                        mText.SetContentsRtf("Up");
                        mText.Location = new Point3d(ptCenter.X + 2, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(mText);
                        tr.AddNewlyCreatedDBObject(mText, true);
                        //leader.AddContext((ObjectContext)mText);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("stairdown")]
        public void stairdown()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            // Prompt for the start point
            pPtOpts.Message = "\nPick the point of the Tree: ";
            pPtRes = ed.GetPoint(pPtOpts);
            Point3d ptCenter = pPtRes.Value;
            SetLayerCurrent("_StairCase");
            if (pPtRes.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
                    {
                        BlockTable acBlkTbl;
                        acBlkTbl = tr.GetObject(db.BlockTableId,
                                                     OpenMode.ForRead) as BlockTable;

                        BlockTableRecord acBlkTblRec;
                        acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                        OpenMode.ForWrite) as BlockTableRecord;
                        Leader leader = new Leader();
                        leader.SetDatabaseDefaults();
                        leader.AppendVertex(ptCenter);
                        leader.AppendVertex(new Point3d(ptCenter.X - 2, ptCenter.Y, 0));
                        leader.HasArrowHead = true;
                        // Add the new object to Model space and the transaction
                        acBlkTblRec.AppendEntity(leader);
                        tr.AddNewlyCreatedDBObject(leader, true);
                        MText mText = new MText();

                        mText.SetDatabaseDefaults();
                        mText.Width = 20;
                        mText.Height = 10;
                        mText.SetContentsRtf("Down");
                        mText.Location = new Point3d(ptCenter.X - 2, ptCenter.Y, 0);
                        acBlkTblRec.AppendEntity(mText);
                        tr.AddNewlyCreatedDBObject(mText, true);
                        //leader.AddContext((ObjectContext)mText);
                    }
                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// Insert Menu End/////////////////////////////
        /// </summary>
        [CommandMethod("MarkAccessRoad")]           ///////////////Mark Menu Module
        public void MarkAccessRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                Commands.tmpmarkstring = "AccessRoad";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MarkMainRoad")]
        public void MarkMainRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                Commands.tmpmarkstring = "MainRoad";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MarkOrrRadialRoad")]
        public void MarkOrrRadialRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                Commands.tmpmarkstring = "ORR Radial Road";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MarkOrrServiceRoad")]
        public void MarkOrrServiceRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                Commands.tmpmarkstring = "ORR Service Road";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MarkCommercialRoad")]
        public void MarkCommercialRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MainRoad")
            {
                Commands.tmpmarkstring = "Commercial Road";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("Amenityfree")]
        public void Amenityfree()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Amenity");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Amenity")
            {
                Commands.tmpmarkstring = "Area to be given free of cost for disposal for Resi or Comm Use";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("AmenitySocial")]
        public void AmenitySocial()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Amenity");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Amenity")
            {
                Commands.tmpmarkstring = "Area for Social Infrastructure";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("AmenitySpecific")]
        public void AmenitySpecific()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Amenity");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Amenity")
            {
                Commands.tmpmarkstring = "Area for Social Infrastructure";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("OpenGreenStrip")]
        public void OpenGreenStrip()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_OrganizedOpenSpace");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_OrganizedOpenSpace")
            {
                Commands.tmpmarkstring = "Green Strip";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("OpenGreenBelt")]
        public void OpenGreenBelt()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_OrganizedOpenSpace");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_OrganizedOpenSpace")
            {
                Commands.tmpmarkstring = "Green Belt";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("OpenTotlot")]
        public void OpenTotlot()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_OrganizedOpenSpace");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            SetLayerCurrent("_OrganizedOpenSpace");
            if ((string)Application.GetSystemVariable("clayer") == "_OrganizedOpenSpace")
            {
                Commands.tmpmarkstring = "Tot lot";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MortgageBuilt")]
        public void MortgageBuilt()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MortgageArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MortgageArea")
            {
                Commands.tmpmarkstring = "Built Up Area to Mortgage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("MortgageDwelling")]
        public void MortgageDwelling()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MortgageArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MortgageArea")
            {
                Commands.tmpmarkstring = "Dwelling Unit to Mortgage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MortgageExtra")]
        public void MortgageExtra()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MortgageArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MortgageArea")
            {
                Commands.tmpmarkstring = "Extra Installment Mortgage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("MortgageLand")]
        public void MortgageLand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MortgageArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MortgageArea")
            {
                Commands.tmpmarkstring = "Land Area to Mortgage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("naMG")]
        public void NalaMortgage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MortgageArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_MortgageArea")
            {
                Commands.tmpmarkstring = "Nala Area to Mortgage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ProposedCentrally")]
        public void ProposedCentrally()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ProposedWork");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ProposedWork")
            {
                Commands.tmpmarkstring = "Centrally AC Bldg";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("udamen")]
        public void UndomarkingDefaultAmenity()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Amenity");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udprop")]
        public void UndomarkingDefaultProposed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ProposedWork");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udrm")]
        public void UndomarkingDefaultRoom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Room");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udprk")]
        public void UndomarkingDefaultParking()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udstair")]
        public void UndomarkingDefaultStaircase()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udlift")]
        public void UndomarkingDefaultLift()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Lift");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udbua")]
        public void UndomarkingDefaultBUA()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ResiBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udcarpt")]
        public void UndomarkingDefaultCarpet()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_CarpetArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udb")]
        public void UndomarkingDefaultBalcony()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Balcony");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udrd")]
        public void UndomarkingDefaultMainRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_MainRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udrw")]
        public void UndomarkingDefaultWidening()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_RoadWidening");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("udnw")]
        public void UndomarkingDefaultNalaRoad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_NalaRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSwitch to correct Active Layer and Select a Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            string CurLayer = (string)Application.GetSystemVariable("clayer");
            string markstr = CurLayer.Remove(0, 1);
            Commands.tmpmarkstring = markstr;
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                    if (poly != null)
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
                        double ang = Angle(sp, ep);
                        Extents3d ext = poly.GeometricExtents;
                        Point3d min = ext.MinPoint;
                        Point3d max = ext.MaxPoint;
                        Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        MText txt = new MText();
                        txt.Contents = Commands.tmpmarkstring;
                        txt.SetDatabaseDefaults(db);
                        Point3d ptleft = Getleft(poly);
                        Point3d ptright = Getright(poly);
                        Point3d pttop = Gettop(poly);
                        Point3d ptbottom = Getbottom(poly);
                        double width = ptright.X - ptleft.X;
                        double height = pttop.Y - ptbottom.Y;
                        txt.Height = height; //<==change to your default height
                                             //txt.Rotation = ang;
                        txt.Width = width;
                        txt.TextStyleId = mtStyleid;
                        txt.Attachment = AttachmentPoint.MiddleCenter;
                        txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("RoomAC")]
        public void RoomAC()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Room");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Room")
            {
                Commands.tmpmarkstring = "AC Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ParkingVisitors")]
        public void RooParkingVisitors()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Parking")
            {
                Commands.tmpmarkstring = "Vistors Parking";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("twostackParking")]
        public void twostackParking()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Parking")
            {
                Commands.tmpmarkstring = "Two Stack Parking";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("threestackParking")]
        public void threestackParking()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Parking")
            {
                Commands.tmpmarkstring = "Three Stack Parking";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("fourstackparking")]
        public void fourstackparking()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Parking");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Parking")
            {
                Commands.tmpmarkstring = "Four Stack Parking";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("oneway")]
        public void oneway()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Driveway");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Driveway")
            {
                Commands.tmpmarkstring = "One way";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("twoway")]
        public void twoway()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Driveway");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Driveway")
            {
                Commands.tmpmarkstring = "Two way";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("tdrfloor")]
        public void tdrfloor()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_FloorInSection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
            {
                Commands.tmpmarkstring = "TDR Floor";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("roadwidening")]
        public void roadwidening()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_FloorInSection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
            {
                Commands.tmpmarkstring = "Road Widening";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("sectiondemolished")]
        public void sectiondemolished()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_FloorInSection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
            {
                Commands.tmpmarkstring = "Floor in Section to be demolished";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("acroomfloor")]
        public void acroomfloor()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_FloorInSection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
            {
                Commands.tmpmarkstring = "AC Room Floor";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("defaultfloorsection")]
        public void defaultfloorsection()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_FloorInSection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_FloorInSection")
            {
                Commands.tmpmarkstring = "Floor in Section";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("acduct")]
        public void acduct()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SectionalItem");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SectionalItem")
            {
                Commands.tmpmarkstring = "AC Duct";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("beam")]
        public void beam()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            SetLayerCurrent("_SectionalItem");
            if ((string)Application.GetSystemVariable("clayer") == "_SectionalItem")
            {
                Commands.tmpmarkstring = "Beam";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("slab")]
        public void slab()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SectionalItem");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SectionalItem")
            {
                Commands.tmpmarkstring = "Slab";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("sunkslab")]
        public void sunkslab()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SectionalItem");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SectionalItem")
            {
                Commands.tmpmarkstring = "Sunk Slab";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("escalator")]
        public void escalator()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Escalator";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("openstaircase")]
        public void openstaircase()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Open StairCase";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("fireescStair")]
        public void fireescStair()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Fire Escape StairCase";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("spiralstair")]
        public void spiralstair()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Spiral/Fabricated StairCase";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("threestair")]
        public void threestair()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Three Flight StairCase";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("fourstair")]
        public void fourstair()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Four Flight StairCase";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        [CommandMethod("interlanding")]
        public void interlanding()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Intermediate Landing";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("flightwidth")]
        public void flightwidth()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Flight Width";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("floorlanding")]
        public void floorlanding()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_StairCase");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_StairCase")
            {
                Commands.tmpmarkstring = "Floor Landing";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("machineroom")]
        public void machineroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Lift");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Lift")
            {
                Commands.tmpmarkstring = "Lift Machine Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("firelift")]
        public void firelift()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Lift");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Lift")
            {
                Commands.tmpmarkstring = "Fire Escape Lift";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("hydrauliclift")]
        public void hydrauliclift()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Lift");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Lift")
            {
                Commands.tmpmarkstring = "Hydraulic Lift";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("carlift")]
        public void carlift()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Lift");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Lift")
            {
                Commands.tmpmarkstring = "Car Lift";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("openpassage")]
        public void openpassage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Passage");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Passage")
            {
                Commands.tmpmarkstring = "Open Passage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("eduinstitution")]
        public void eduinstitution()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Educational/Institutional";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("publicutility")]
        public void publicutility()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Public Utility";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("transcommu")]
        public void transcommu()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Transportation and Communication";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("medical")]
        public void medical()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Medical";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("unclassified")]
        public void unclassified()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Unclassified";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("assembly")]
        public void assembly()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Assembly";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("mixed")]
        public void mixed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Mixed";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("offbusiness")]
        public void offbusiness()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Office/Business";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("storage")]
        public void storage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Storage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("hazardous")]
        public void hazardous()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Hazardous";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("nanotech")]
        public void nanotech()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "NanoTechnology Building";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("biotech")]
        public void biotech()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "BioTechnology Building";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("inftech")]
        public void inftech()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Information Technology Building";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("wholesale")]
        public void wholesale()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Wholesale establishment";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("floriculture")]
        public void floriculture()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "FloriCulture Land-use";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("hotel")]
        public void hotel()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Hotel";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("constructed")]
        public void constructed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline" || (string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "Existing constructed as per rules";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("approved")]
        public void approved()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "Existing approved but not constructed";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("existingbua")]
        public void existingbua()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "ExistingBUA outline";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("tobedemo")]
        public void tobedemo()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_SpecialUseBUAOutline");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_SpecialUseBUAOutline")
            {
                Commands.tmpmarkstring = "To be Demolished";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("splittedtene")]
        public void splittedtene()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_CarpetArea");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_CarpetArea")
            {
                Commands.tmpmarkstring = "splitted Tenement";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("enclosedbal")]
        public void enclosedbal()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Balcony");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Balcony")
            {
                Commands.tmpmarkstring = "Enclosed Balcony";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("serviceba")]
        public void serviceba()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_Balcony");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_Balcony")
            {
                Commands.tmpmarkstring = "Service Balcony";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("cornice")]                  //Projections   
        public void cornice()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Cornice";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("chajja")]
        public void chajja()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Chhajja";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("weathershed")]
        public void weathershed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Weather shed";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("canopy")]
        public void canopy()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Canopy";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("porch")]
        public void porch()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Porch";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("loft")]
        public void loft()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Loft";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("steps")]
        public void steps()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Steps";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("otta")]
        public void otta()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "OTTA";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("verandah")]
        public void verandah()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Open Verandah";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("glassfacade")]
        public void glassfacade()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ArchProjection");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ArchProjection")
            {
                Commands.tmpmarkstring = "Glass Facade";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("wideningfreecost")]
        public void wideningfreecost()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_RoadWidening");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_RoadWidening")
            {
                Commands.tmpmarkstring = "Surrendered Free of Cost";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("periphery")]
        public void periphery()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_RoadWidening");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_RoadWidening")
            {
                Commands.tmpmarkstring = "Periphery";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("nalafreecost")]
        public void nalafreecost()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_NalaRoad");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_NalaRoad")
            {
                Commands.tmpmarkstring = "Surrendered Free of Cost";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("sturucturedemolished")]
        public void sturucturedemolished()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ExistingStructure");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "To be Demolished";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            txt.Height = HeightRectPolyLine(min, max) / 3.0; //<==change to your default height
                            txt.Rotation = ang;
                            txt.Width = WidthRectPolyLine(min, max) / 2.0;
                            txt.TextStyleId = mtStyleid;
                            txt.TextHeight = HeightRectPolyLine(min, max) / 20.0;
                            txt.Attachment = AttachmentPoint.TopCenter;
                            txt.Location = new Point3d(min.X + WidthRectPolyLine(min, max) / 2, min.Y + HeightRectPolyLine(min, max) * 2 / 3, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("retained")]
        public void retained()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ExistingStructure");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "To be retained";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("appnotconstructed")]
        public void appnotconstructed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ExistingStructure");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "Existing approved but not onstructed:";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("notconstructed")]
        public void notconstructed()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ExistingStructure");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "Existing not constructed as per the Rule";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("notconstructed1")]
        public void notconstructed1()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ExistingStructure");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ExistingStructure")
            {
                Commands.tmpmarkstring = "Existing not constructed as per the Rule:";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("chemicalplant")]
        public void chemicalplant()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Open Chemical Plant";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("solarlightening")]
        public void solarlightening()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Solar Lightening system";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("rainwater")]
        public void rainwater()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Rain Water Harvesting";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("solarheating")]
        public void solarheating()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Solar Heating System";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("distributiontrans")]
        public void distributiontrans()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Distribution transformer";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("securityguard")]
        public void securityguard()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Security Guard Booth";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("garage")]
        public void garage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Garage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("explossivestorage")]
        public void explossivestorage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Explossive Storage";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("pumproom")]
        public void pumproom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Pump House/Moter Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("electrictrans")]
        public void electrictrans()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Electric Transformer";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterrecycle")]
        public void waterrecycle()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Waste water recyling";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("acplantroom")]
        public void acplantroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "AC Plant Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("gategroomty")]
        public void gategroomty()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Gate Groomty";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("septictank")]
        public void septictank()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Septic Tank";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("toiletblock")]
        public void toiletblock()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Toilet/Sanitary Block";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("bore")]
        public void bore()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Bore";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("sump")]
        public void sump()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Sump";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("boilerroom")]
        public void boilerroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Boiler Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("servantquarter")]
        public void servantquarter()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Servant Quarter";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("well")]
        public void well()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Well";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("parkingsheds")]
        public void parkingsheds()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Parking Sheds";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("garbagepit")]
        public void garbagepit()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Garbage Pit";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("undergroundwantertank")]
        public void undergroundwantertank()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Unerground water Tank";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("sewagetreatment")]
        public void sewagetreatment()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Sewage Treatment Plant";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("overheadwatertank")]
        public void overheadwatertank()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Overhead water Tank";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("swimmingpool")]
        public void swimmingpool()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Swimming pool";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("fitnesscenter")]
        public void fitnesscenter()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Fitness Center";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("percolationwell")]
        public void percolationwell()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Percolation well or Percolation Pit";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ahu")]
        public void ahu()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "AHU";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("garbage")]
        public void garbage()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Garbage Bin";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("meterroom")]
        public void meterroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Meter Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("drypit")]
        public void drypit()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Dry Pit";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("effuentrtreat")]
        public void effuentrtreat()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Effluent Treatment Plant";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("enterancegate")]
        public void enterancegate()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Entrance Gate";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("societyoffice")]
        public void societyoffice()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Society Office";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("clubhouse")]
        public void clubhouse()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Club House";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("weighbridge")]
        public void weighbridge()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Weigh Bridge";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("wetpit")]
        public void wetpit()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Wet Pit";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("helipad")]
        public void helipad()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Helipad";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("dgsetroom")]
        public void dgsetroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "D G Set Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("laundry")]
        public void laundry()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Laundry";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("utility")]
        public void utility()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Utility";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("lobby")]
        public void lobby()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Lobby";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("firecontrol")]
        public void firecontrol()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Fire Control Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("watchman")]
        public void watchman()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Watchman Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("firecommand")]
        public void firecommand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Fire Command Centre";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("storagetank")]
        public void storagetank()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Storage Tank";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("generatorroom")]
        public void generatorroom()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_AccessoryUse");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_AccessoryUse")
            {
                Commands.tmpmarkstring = "Generator Room";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("htensionline")]
        public void htensionline()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ElectricLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ElectricLine")
            {
                Commands.tmpmarkstring = "High Tension Electricity Lines";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            Plugin.elinestate = 1;
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("eltowerline")]
        public void eltowerline()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ElectricLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ElectricLine")
            {
                Commands.tmpmarkstring = "Electricity Tower Lines";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                            Plugin.elinestate = 2;
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ltensionline")]
        public void ltensionline()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_ElectricLine");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_ElectricLine")
            {
                Commands.tmpmarkstring = "Low Tension Electricity Lines";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                            Plugin.elinestate = 3;
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterbodycanal")]
        public void waterbodycanal()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_WaterBodies");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_WaterBodies")
            {
                Commands.tmpmarkstring = "Defined Boundary of Canal, Vagu, Nala, Storm Water Drain of width up to 10m";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterbodyabove")]
        public void waterbodyabove()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_WaterBodies");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_WaterBodies")
            {
                Commands.tmpmarkstring = "FTL Boundary of Lakes/Tanks/Kuntas of area 10Ha and above";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterbodyless")]
        public void waterbodyless()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_WaterBodies");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_WaterBodies")
            {
                Commands.tmpmarkstring = "FTL Boundary of Lakes/Tanks/Kuntas of area less thatn 10Ha/shikam lands";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterbodymore")]
        public void waterbodymore()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_WaterBodies");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_WaterBodies")
            {
                Commands.tmpmarkstring = "Defined Boundary of Canal, Vagu, Nala, Storm Water Drain of width more than 10m";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("waterbodyriver")]
        public void waterbodyriver()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_WaterBodies");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_WaterBodies")
            {
                Commands.tmpmarkstring = "River";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("contour")]
        public void contour()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Contours";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("locationplan")]
        public void locationplan()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Location Plan";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailsection")]
        public void detailsection()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Section";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailmaster")]
        public void detailmaster()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Extract of Master Plan";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailkey")]
        public void detailkey()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Key Plan";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailcertificate")]
        public void detailcertificate()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Certificate";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailelivation2")]
        public void detailelivation2()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Elevation_2";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailtitle")]
        public void detailtitle()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Project Title";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailstructal")]
        public void detailstructal()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Structural Detail";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("detailelevation1")]
        public void detailelevation1()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            SetLayerCurrent("_PrintAdditionalDetail");
            PromptEntityOptions options = new PromptEntityOptions("\nSelect Polyline: ");
            options.SetRejectMessage("\nSelected object is no a Polyline.");
            options.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(options);
            if ((string)Application.GetSystemVariable("clayer") == "_PrintAdditionalDetail")
            {
                Commands.tmpmarkstring = "Elevation_1";
                if (result.Status == PromptStatus.OK)
                {
                    // at this point we know an entity have been selected and it is a Polyline
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Polyline poly = tr.GetObject(result.ObjectId, OpenMode.ForRead, false) as Polyline;
                        if (poly != null)
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
                            double ang = Angle(sp, ep);
                            Extents3d ext = poly.GeometricExtents;
                            Point3d min = ext.MinPoint;
                            Point3d max = ext.MaxPoint;
                            Point3d geoCtr = Polar(min, Angle(min, max), Distance(min, max) / 2.0);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            MText txt = new MText();
                            txt.Contents = Commands.tmpmarkstring;
                            txt.SetDatabaseDefaults(db);
                            Point3d ptleft = Getleft(poly);
                            Point3d ptright = Getright(poly);
                            Point3d pttop = Gettop(poly);
                            Point3d ptbottom = Getbottom(poly);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            txt.Height = height; //<==change to your default height
                            //txt.Rotation = ang;
                            txt.Width = width;
                            txt.TextStyleId = mtStyleid;
                            txt.Attachment = AttachmentPoint.MiddleCenter;
                            txt.Location = new Point3d(ptleft.X + width / 2, pttop.Y - height / 2, 0);
                            btr.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }
                        tr.Commit();
                    }
                }
            }
        }
        /////////////Mark Module End//////////////////////////

        ////////////Tool Submenu Function////////////////////
        [CommandMethod("shla")]
        public static void ShowAllLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            Plugin.allLayers = Commands.LayersToList(db);
            List<string> offlayers = new List<string>();
            offlayers.Clear();
            TurnOnLayers(Plugin.allLayers, offlayers);
        }

        [CommandMethod("shld")]
        public static void ShowOnlyDcrLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            Plugin.allLayers = Commands.LayersToList(db);
            List<string> onlayers = new List<string>();
            List<string> offlayers = new List<string>();
            foreach (string str in Plugin.allLayers)
            {
                if (str.Contains("BP_"))
                    onlayers.Add(str);
                else
                    offlayers.Add(str);
            }
            TurnOnLayers(onlayers, offlayers);
        }

        [CommandMethod("shlo")]
        public static void ShowOtherLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            Plugin.allLayers = Commands.LayersToList(db);
            List<string> onlayers = new List<string>();
            List<string> offlayers = new List<string>();
            foreach (string str in Plugin.allLayers)
            {
                if (str.Contains("_"))
                    offlayers.Add(str);
                else
                    onlayers.Add(str);
            }
            TurnOnLayers(onlayers, offlayers);
        }

        [CommandMethod("shlap")]
        public static void AllPreDcrLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            Plugin.allLayers = Commands.LayersToList(db);
            List<string> onlayers = new List<string>();
            List<string> offlayers = new List<string>();
            foreach (string str in Plugin.allLayers)
            {
                if (str[0] == '_')
                    onlayers.Add(str);
                else
                    offlayers.Add(str);
            }
            TurnOnLayers(onlayers, offlayers);
        }

        [CommandMethod("shllp")]
        public static void ShowPreDcrLayoutLevelLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            string strpath = System.IO.Directory.GetCurrentDirectory() + "\\" + "llevel.tdb";
            List<string> Layerlevellayers = new List<string>();
            List<string> offlayers = new List<string>();
            List<string> onlayers = new List<string>();
            if (File.Exists(strpath))
            {
                foreach (string strln in File.ReadLines(strpath))
                {
                    Layerlevellayers.Add(strln);
                }
            }
            foreach (string str in Plugin.allLayers)
            {
                if (IsincludedinList(str, Layerlevellayers))
                    onlayers.Add(str);
                else
                    offlayers.Add(str);
            }
            TurnOnLayers(onlayers, offlayers);
        }

        [CommandMethod("shlbp")]
        public static void ShowPreDcrBuildingLevelLayers()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            string strpath = System.IO.Directory.GetCurrentDirectory() + "\\" + "blevel.tdb";
            List<string> Buildinglevellayers = new List<string>();
            List<string> offlayers = new List<string>();
            List<string> onlayers = new List<string>();
            if (File.Exists(strpath))
            {
                foreach (string strln in File.ReadLines(strpath))
                {
                    Buildinglevellayers.Add(strln);
                }
            }
            foreach (string str in Plugin.allLayers)
            {
                if (IsincludedinList(str, Buildinglevellayers))
                    onlayers.Add(str);
                else
                    offlayers.Add(str);
            }
            TurnOnLayers(onlayers, offlayers);
        }
        //////////Tool Submenu Function End///////////////////////////
    }
}
