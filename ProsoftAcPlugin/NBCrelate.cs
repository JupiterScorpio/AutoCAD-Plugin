using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using System.Collections.Specialized;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Exception = System.Exception;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;
//using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using static ProsoftAcPlugin.Plugin;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Autodesk.Forge.Model;
using System.Threading;
using System.Timers;
using System.Web.Configuration;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;


namespace ProsoftAcPlugin
{
    public class NBCrelate
    {
        public static Thread MyThread;
        public static int rulecheckprogress;
        //public static NBCLayers.Rulecheckprogress frm = new NBCLayers.Rulecheckprogress();
        public static double cur_progress_pos;
        public static Thread secondThread;
        public void Initialize()
        {

        }
        public void Terminate()
        {
        }

        public static void MakeJson()
        {
            if (ProsoftAcPlugin.Commands.amroadrule.Count == 0)
            {
                getonlyMroadentity();
            }
            List<JsonItems> jsonlists = new List<JsonItems>();
            IEnumerable<windowrule> windrulelst = ProsoftAcPlugin.Commands.awindowrule.Distinct();
            IEnumerable<doorrule> doorrulelst = ProsoftAcPlugin.Commands.adoorrule.Distinct();
            IEnumerable<roomrule> rmrulelst = ProsoftAcPlugin.Commands.aroomrule.Distinct();
            IEnumerable<mroadrule> mroadlst = ProsoftAcPlugin.Commands.amroadrule.Distinct();

            foreach (mroadrule wrule in mroadlst)
            {
                jsonlists.Add(new JsonItems()
                {
                    layer = "_MainRoad",
                    OId = wrule.objid.ToString(),
                    width = wrule.width,
                    height = wrule.height,
                    hndle = wrule.hnd.ToString(),
                    projtype = Commands.ProjecttypeTostring(Plugin.projtypestate)
                });
            }
            foreach (windowrule wrule in windrulelst)
            {
                jsonlists.Add(new JsonItems()
                {
                    layer = "_MainRoad",
                    OId = wrule.objid.ToString(),
                    width = wrule.width,
                    height = wrule.height,
                    hndle = wrule.hnd.ToString(),
                    projtype = Commands.ProjecttypeTostring(Plugin.projtypestate)
                });
            }
            foreach (doorrule wrule in doorrulelst)
            {
                jsonlists.Add(new JsonItems()
                {
                    layer = "_MainRoad",
                    OId = wrule.objid.ToString(),
                    width = wrule.width,
                    height = wrule.height,
                    hndle = wrule.hnd.ToString(),
                    projtype = Commands.ProjecttypeTostring(Plugin.projtypestate)
                });
            }
            foreach (roomrule wrule in rmrulelst)
            {
                jsonlists.Add(new JsonItems()
                {
                    layer = "_MainRoad",
                    OId = wrule.objid.ToString(),
                    width = wrule.width,
                    height = wrule.height,
                    hndle = wrule.hnd.ToString(),
                    projtype = Commands.ProjecttypeTostring(Plugin.projtypestate)
                });
            }
            if (Plugin.strCurJsonPath == "")
            {
                Application.ShowAlertDialog("Please select a json file name equals to drawing file.");
                string sourceFileName = "";
                System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog
                {
                    InitialDirectory = @"D:\",
                    Title = "Browse Drawing Files",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = "dwg",
                    Filter = "Json files (*.dwg)|*.dwg",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };


                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sourceFileName = openFileDialog1.FileName;
                    Plugin.strCurDocPath = sourceFileName;
                    Plugin.strCurJsonPath = Path.ChangeExtension(Plugin.strCurDocPath, "json");
                    var Json1 = JsonConvert.SerializeObject(jsonlists);
                    File.WriteAllText(Plugin.strCurJsonPath, Json1);
                }
            }
            else
            {
                var Json = JsonConvert.SerializeObject(jsonlists);
                File.WriteAllText(Plugin.strCurJsonPath, Json);
            }
        }
        public static void GettingEntities()
        {
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;            
            int progresscnt = 0;
            try
            {
                NBCLayers.Rulecheckprogress frm = new NBCLayers.Rulecheckprogress();
                frm.Show();
                foreach (string layername in Plugin.allLayers)
                {                    
                    rulecheckprogress = progresscnt / Plugin.allLayers.Count * 100;
                    double f = ((double)progresscnt / (double)(2 * Plugin.allLayers.Count)) * 100;
                    cur_progress_pos = f;
                    frm.ReportProgress((int)f, (" Now getting objects of " + layername + " Layer... "));
                    GetNeededEntitiesOnLayer(database, layername);
                    progresscnt++;
                }
                frm.ReportProgress(50, "Entities Check Finished");
                foreach (string layername in Plugin.allLayers)
                {
                    double f = ((double)progresscnt / (double)(2 * Plugin.allLayers.Count)) * 100;
                    cur_progress_pos = f;
                    frm.ReportProgress((int)f, (" Now checking " + layername + " Layer rule... "));
                    if (Plugin.projtypestate == 3 || Plugin.projtypestate == 4 || Plugin.projtypestate == 5)
                        LayerRuleCheck_Layout(layername);
                    else if (Plugin.projtypestate == 0)
                        LayerRuleCheck_BldgPermiss(layername);
                    progresscnt++;
                }
                Commands.brulefinished = true;
                frm.ReportProgress(100, "Rule Check finished");
                //frm.Close();
                Commands.CheckingValidEntity();
                //RuleResultshow();
            }
            catch (Exception e)
            {
                
                return;
            }            
        }
        public static void deletingEntities(Database db, string layerName)
        {
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForWrite);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForWrite);                    
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity subent = tr.GetObject(id, OpenMode.ForWrite) as Entity;  //Temporarily modified
                            if (subent.Layer == layerName)
                            {
                                subent.Erase();
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }
        public static void Rulecheck()
        {
            cur_progress_pos = 0;
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            Commands.brulefinished = false;
            //deletingEntities(database, "_BuildingName");
            RuleInit();
            ReadFromNodProjecttypeANDPlotUse();
            GettingEntities();
            //foreach (string layername in Plugin.allLayers)
            //{
            //    GetNeededEntitiesOnLayer(database, layername);
            //}
            //foreach (string layername in Plugin.allLayers)
            //{
            //    if (Plugin.projtypestate == 3 || Plugin.projtypestate == 4 || Plugin.projtypestate == 5)
            //        LayerRuleCheck_Layout(layername);
            //    else
            //        LayerRuleCheck_BldgPermiss(layername);
            //}
            //List<string> emptylist = new List<string>();          //empty layer showing
            //emptylist = FindEmptylayerList();                     //
            //if(emptylist.Count!=0)
            //{
            //    string strresult = "";

            //    foreach(string layer in emptylist)
            //    {
            //        strresult = strresult + " " + layer;
            //    }
            //    Application.ShowAlertDialog(strresult+"layers are missing...");
            //}
            //MakeWind_DoorText(MakingWind_DoorList());
            RuleResultshow();
        }
        public static void ReadFromNodProjecttypeANDPlotUse()
        {
            string projtype = "";
            string plotuse = "";
            var db = HostApplicationServices.WorkingDatabase;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var Nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead);
                if (Nod.Contains("PrevalProjectType"))
                {
                    try
                    {
                        var prevaldict = (DBDictionary)tr.GetObject(Nod.GetAt("PrevalProjectType"), OpenMode.ForRead);
                        if(prevaldict.Contains("ProjectType"))
                        {
                            var xrec = (Xrecord)tr.GetObject(prevaldict.GetAt("ProjectType"), OpenMode.ForRead);
                            ResultBuffer data = xrec.Data;
                            var tmp = data.AsArray();
                            projtype = tmp[0].Value.ToString();
                            Plugin.projtypestate = Commands.ProjecttypetoUint(projtype);
                        }
                        

                        //Application.ShowAlertDialog(projtype + "--" + Plugin.projtypestate.ToString());
                        if(prevaldict.Contains("PlotUse"))
                        {
                            var xrec1 = (Xrecord)tr.GetObject(prevaldict.GetAt("PlotUse"), OpenMode.ForRead);
                            ResultBuffer data1 = xrec1.Data;
                            var tmp1 = data1.AsArray();
                            plotuse = tmp1[0].Value.ToString();
                            Plugin.usestate = Commands.PlotusetoUint(plotuse);
                        }      
                    }
                    catch 
                    {
                        throw;
                    }
                }
            }
        }
        public static List<string> FindEmptylayerList()
        {
            List<string> emptylayerlist = new List<string>();
            List<string> presentlayerlist = new List<string>();
            if (Plugin.projtypestate == 3 || Plugin.projtypestate == 4 || Plugin.projtypestate == 5)
            {
                presentlayerlist.Add("_Plot");
                presentlayerlist.Add("_Amenity");
                presentlayerlist.Add("_IndivSubPlot");
                presentlayerlist.Add("_MainRoad");
                presentlayerlist.Add("_InternalRoad");
                presentlayerlist.Add("_OrganizedOpenSpace");
                presentlayerlist.Add("_MortgageArea");
                presentlayerlist.Add("_Amenity");
                presentlayerlist.Add("_Splay");
                //presentlayerlist.Add("_CompoundWall");
            }
            else
            {
                presentlayerlist.Add("_Plot");
                presentlayerlist.Add("_AccessoryUse");
                presentlayerlist.Add("_BuildingName");
                presentlayerlist.Add("_ProposedWork");
                presentlayerlist.Add("_MortgageArea");
                presentlayerlist.Add("_Parking");
                presentlayerlist.Add("_ResiBUAOutline");
                presentlayerlist.Add("_SpecialUseBUAOutline");
                presentlayerlist.Add("_CommBUAOutline");
                presentlayerlist.Add("_IndBUAOutline");
                presentlayerlist.Add("_Room");
                presentlayerlist.Add("_CarpetArea");
                presentlayerlist.Add("_Passage");
                presentlayerlist.Add("_Door");
                presentlayerlist.Add("_Window");
                presentlayerlist.Add("_FloorInSection");
                presentlayerlist.Add("_Section");
                presentlayerlist.Add("_GroundLevel");
                presentlayerlist.Add("_Floor");
                presentlayerlist.Add("_PrintAdditionalDetail");
                presentlayerlist.Add("_StairCase");
                presentlayerlist.Add("_Lift");
                presentlayerlist.Add("_Terrace");
            }
            
            foreach(string layer in presentlayerlist)
            {
                switch(layer)
                {
                    case "_Plot":
                        {
                            if (Plugin.aplotpline.Count == 0)
                                emptylayerlist.Add("_Plot");
                            break;
                        }
                    case "_Amenity":
                        {
                            if (Plugin.aAmenitypline.Count == 0)
                                emptylayerlist.Add("_Amenity");
                            break;
                        }
                    case "_IndivSubPlot":
                        {
                            if (Plugin.aindvSubPltpline.Count == 0)
                                emptylayerlist.Add("_IndivSubPlot");
                            break;
                        }
                    case "_MainRoad":
                        {
                            if (Plugin.amroadpline.Count == 0)
                                emptylayerlist.Add("_MainRoad");
                            break;
                        }
                    case "_InternalRoad":
                        {
                            if (Plugin.ainterroadpline.Count == 0)
                                emptylayerlist.Add("_InternalRoad");
                            break;
                        }
                    case "_OrganizedOpenSpace":
                        {                            
                            if (Plugin.aopenspacepline.Count == 0)
                                emptylayerlist.Add("_OrganizedOpenSpace");
                            break;
                        }
                    case "_MortgageArea":
                        {
                            if (Plugin.aMortgageAreapline.Count == 0)
                                emptylayerlist.Add("_MortgageArea");
                            break;
                        }
                    case "_Splay":
                        {
                            if (Plugin.asplaypline.Count == 0)
                                emptylayerlist.Add("_Splay");
                            break;
                        }
                    case "_CompoundWall":
                        {
                            if (Plugin.aCompndwllpline.Count == 0)
                                emptylayerlist.Add("_CompoundWall");
                            break;
                        }

                    /////////////////////////////in Building Propasal///////////////////////
                    case "_AccessoryUse":
                        {
                            if (Plugin.aAccusepline.Count == 0)
                                emptylayerlist.Add("_AccessoryUse");
                            break;
                        }
                    case "_BuildingName":
                        {
                            if (Plugin.abuildingNmpline.Count == 0)
                                emptylayerlist.Add("_BuildingName");
                            break;
                        }
                    case "_ProposedWork":
                        {
                            if (Plugin.aprpwrkpline.Count == 0)
                                emptylayerlist.Add("_ProposedWork");
                            break;
                        }
                    case "_Parking":
                        {
                            if (Plugin.aParkingpline.Count == 0)
                                emptylayerlist.Add("_Parking");
                            break;
                        }
                    case "_ResiBUAOutline":
                        {
                            if (Plugin.aResiBUApline.Count == 0)
                                emptylayerlist.Add("_ResiBUAOutline");
                            break;
                        }
                    case "_SpecialUseBUAOutline":
                        {
                            if (Plugin.aSpecBUApline.Count == 0)
                                emptylayerlist.Add("_SpecialUseBUAOutline");
                            break;
                        }
                    case "_CommBUAOutline":
                        {
                            if (Plugin.aComBUApline.Count == 0)
                                emptylayerlist.Add("_CommBUAOutline");
                            break;
                        }
                    case "_IndBUAOutline":
                        {
                            if (Plugin.aIndBUApline.Count == 0)
                                emptylayerlist.Add("_IndBUAOutline");
                            break;
                        }
                    case "_Room":
                        {
                            if (Plugin.aroompline.Count == 0)
                                emptylayerlist.Add("_Room");
                            break;
                        }
                    case "_CarpetArea":
                        {
                            if (Plugin.aCarpetpline.Count == 0)
                                emptylayerlist.Add("_CarpetArea");
                            break;
                        }
                    case "_Passage":
                        {
                            if (Plugin.aPassagepline.Count == 0&&Plugin.usestate!=1)
                                emptylayerlist.Add("_Passage");
                            break;
                        }
                    case "_Door":
                        {
                            if (Plugin.adoorpline.Count == 0)
                                emptylayerlist.Add("_Door");
                            break;
                        }
                    case "_Window":
                        {
                            if (Plugin.awindowpline.Count == 0)
                                emptylayerlist.Add("_Window");
                            break;
                        }
                    case "_FloorInSection":
                        {
                            if (Plugin.aFlrinSecpline.Count == 0)
                                emptylayerlist.Add("_FloorInSection");
                            break;
                        }
                    case "_Section":
                        {
                            if (Plugin.aSectionpline.Count == 0)
                                emptylayerlist.Add("_Section");
                            break;
                        }
                    case "_GroundLevel":
                        {
                            if (Plugin.aGllvlpline.Count == 0)
                                emptylayerlist.Add("_GroundLevel");
                            break;
                        }
                    case "_Floor":
                        {
                            if (Plugin.aFloorpline.Count == 0)
                                emptylayerlist.Add("_Floor");
                            break;
                        }
                    case "_PrintAdditionalDetail":
                        {
                            if (Plugin.aprintaddpline.Count == 0)
                                emptylayerlist.Add("_PrintAdditionalDetail");
                            break;
                        }
                    case "_StairCase":
                        {
                            if (Plugin.aStairpline.Count == 0)
                                emptylayerlist.Add("_StairCase");
                            break;
                        }
                    case "_Lift":
                        {
                            if (Plugin.aLiftpline.Count == 0)
                                emptylayerlist.Add("_Lift");
                            break;
                        }
                    case "_Terrace":
                        {
                            if (Plugin.aTerracepline.Count == 0)
                                emptylayerlist.Add("_Terrace");
                            break;
                        }
                }
            }
            if(Plugin.aResiBUApline.Count != 0|| Plugin.aSpecBUApline.Count != 0|| Plugin.aComBUApline.Count != 0|| Plugin.aIndBUApline.Count != 0)
            {
                emptylayerlist.Remove("_ResiBUAOutline");
                emptylayerlist.Remove("_SpecialUseBUAOutline");
                emptylayerlist.Remove("_CommBUAOutline");
                emptylayerlist.Remove("_IndBUAOutline");
            }
            return emptylayerlist;
        }
        
        public static void RuleResultshow()
        {
            var frm = new RuleCheckForm();
            frm.Show();
        }
        public static void RuleInit()
        {
            Commands.errlist.Clear();
            Plugin.awindowpline.Clear();
            Plugin.adoorpline.Clear();
            Plugin.aroompline.Clear();
            Plugin.aplotpline.Clear();
            Plugin.amroadpline.Clear();
            Plugin.aindvSubPltpline.Clear();
            Plugin.ainterroadpline.Clear();
            Plugin.aopenspacepline.Clear();
            Plugin.aAmenitypline.Clear();
            Plugin.aMortgageAreapline.Clear();
            Plugin.asplaypline.Clear();
            Plugin.aBufferpline.Clear();
            Plugin.aElectricpline.Clear();
            Plugin.aWaterBodypline.Clear();
            Plugin.aWaterlinepline.Clear();
            Plugin.aLeftownerspline.Clear();
            Plugin.aSurAuthpline.Clear();
            Plugin.aCompndwllpline.Clear();
            Plugin.aElinepline.Clear();
            Plugin.aGllvlpline.Clear();
            Plugin.aFlrinSecpline.Clear();
            Plugin.aPropWrkpline =null;
            Plugin.aParkingpline.Clear();
            Plugin.aDrivewaypline.Clear();
            Plugin.arampline.Clear();
            Plugin.aFloorpline.Clear();
            Plugin.aVShaftpline.Clear();
            Plugin.aVoidpline.Clear();
            Plugin.aAccusepline.Clear();
            Plugin.aNalapline.Clear();
            Plugin.aStairpline.Clear();
            Plugin.aPassagepline.Clear();
            Plugin.aVenShaftpline.Clear();
            Plugin.aRdWidepline.Clear();
            Plugin.aSectionpline.Clear();
            Plugin.aMargineline.Clear();
            Plugin.azeropline.Clear();
            Plugin.aprintaddpline.Clear();
            Plugin.abuildingNmpline.Clear();
            Plugin.aprpwrkpline.Clear();
            Plugin.aResiBUApline.Clear();
            Plugin.aSpecBUApline.Clear();
            Plugin.aComBUApline.Clear();
            Plugin.aIndBUApline.Clear();
            Plugin.aCarpetpline.Clear();
            Plugin.aLiftpline.Clear();
            Plugin.aTerracepline.Clear();
            Plugin.abuildingNmpline.Clear();
            Plugin.aSitePlanplilne.Clear();
            Plugin.aBalconypline.Clear();

            Plugin.awindowNmTxt.Clear();
            Plugin.aroomNmTxt.Clear();
            Plugin.adoorNmTxt.Clear();
            Plugin.aplotNmTxt.Clear();
            Plugin.amroadNmTxt.Clear();
            Plugin.aindvsubPltTxt.Clear();
            Plugin.ainterroadTxt.Clear();
            Plugin.aopenspaceTxt.Clear();
            Plugin.aAmenityTxt.Clear();
            Plugin.aMortgageAreaTxt.Clear();
            Plugin.asplayTxt.Clear();
            Plugin.aBufferTxt.Clear();
            Plugin.aElectricTxt.Clear();
            Plugin.aWaterBodyTxt.Clear();
            Plugin.aWaterlineTxt.Clear();
            Plugin.aLeftOwnersTxt.Clear();
            Plugin.aSurAuthTxt.Clear();
            Plugin.aCmpWallTxt.Clear();
            Plugin.aElineTxt.Clear();
            Plugin.aGllvlTxt.Clear();
            Plugin.aFlrinSecTxt.Clear();
            Plugin.aPropWrkTxt =null;
            Plugin.aParkingTxt.Clear();
            Plugin.aDrivewayTxt.Clear();
            Plugin.arampTxt.Clear();
            Plugin.aFloorTxt.Clear();
            Plugin.aVShafttxt.Clear();
            Plugin.aVoidTxt.Clear();
            Plugin.aAccuseTxt.Clear();
            Plugin.aNalaTxt.Clear();
            Plugin.aStairTxt.Clear();
            Plugin.aPassageTxt.Clear();
            Plugin.aVenShaftTxt.Clear();
            Plugin.aRdWideTxt.Clear();
            Plugin.aZeromTxt.Clear();
            Plugin.marginlinelist.Clear();
            Plugin.aprintaddTxt.Clear();
            Plugin.abldingNmTxt.Clear();
            Plugin.aprpWrkTxt.Clear();
            Plugin.aResiBUATxt.Clear();
            Plugin.aSpecBUATxt.Clear();
            Plugin.aComBUATxt.Clear();
            Plugin.aIndBUATxt.Clear();
            Plugin.aCarpetTxt.Clear();
            Plugin.aSecTxt.Clear();
            Plugin.aLiftTxt.Clear();
            Plugin.aTerraceTxt.Clear();
            Plugin.aSitePlanpTxt.Clear();       //10.09 added
            Plugin.aBalconyTxt.Clear();

            Plugin.aFlrinSecSTxt.Clear();
            Plugin.aFloorsTxt.Clear();
            Plugin.aZeroTxt.Clear();
            Plugin.asplyDBTxt.Clear();
            Plugin.ainterloadDBTxt.Clear();
            Plugin.aindvsubDBTxt.Clear();
            Plugin.apltDBTxt.Clear();
            Plugin.acmpndWallDBTxt.Clear();
            Plugin.aTerraceDBTxt.Clear();   ///09.02 added
            Plugin.aLiftDBTxt.Clear();   ///09.02 added
            Plugin.aVenShaftDBTxt.Clear();      //09.23 added
            Plugin.aWindDBTxt.Clear();
            Plugin.aRoomDBTxt.Clear();
            Plugin.aDoorDBTxt.Clear();
            Plugin.aMroadDBTxt.Clear();
            Plugin.aOrgOpenDBTxt.Clear();
            Plugin.aAmenDBTxt.Clear();
            Plugin.aMortgageDBTxt.Clear();
            Plugin.aBufferDBTxt.Clear();
            Plugin.aWaterBodDBTxt.Clear();
            Plugin.aWaterLnDBTxt.Clear();
            Plugin.aLeftOverDBTxt.Clear();
            Plugin.aSurrenderDBTxt.Clear();
            Plugin.aElectricDBTxt.Clear();
            Plugin.aGroundDBTxt.Clear();
            Plugin.aPrpWrkDBTxt.Clear();
            Plugin.aParkDBTxt.Clear();
            Plugin.aDrvwayDBTxt.Clear();
            Plugin.aRampDBTxt.Clear();
            Plugin.aSlabcutoutDBTxt.Clear();
            Plugin.aAccesUseDBTxt.Clear();
            Plugin.aNalaDBText.Clear();
            Plugin.aStairDBText.Clear();
            Plugin.aPassageDBTxt.Clear();
            Plugin.aRdWideDBTxt.Clear();
            Plugin.aPrintaddtionDBTxt.Clear();
            Plugin.aBuildNameDBTxt.Clear();
            Plugin.aResiBuaDBTxt.Clear();
            Plugin.aspecialuseBUaDBTxt.Clear();
            Plugin.aCommBUADBTxt.Clear();
            Plugin.aIndBUADBTxt.Clear();
            Plugin.aCarpetDBTxt.Clear();
            Plugin.aSectionDBTxt.Clear();           //09.23 added
            Plugin.aSitePlanpDBTxt.Clear();       //10.09 added
            Plugin.aBalconyDBTxt.Clear();
        }
        public static void getonlyMroadentity()
        {
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var db = currentDocument.Database;
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
                            if (subent.Layer == "_MainRoad")
                            {
                                if (id.ObjectClass == PlineCls)
                                {
                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                    if (pline.Layer.Equals("_MainRoad", System.StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        mroadrule mrule = new mroadrule();
                                        mrule.pl = pline;
                                        mrule.objid = pline.ObjectId;
                                        mrule.hnd = pline.ObjectId.Handle;
                                        ProsoftAcPlugin.Commands.amroadrule.Add(mrule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void GetNeededEntitiesOnLayer(Database db, string layerName)        //this function gets all entities in one layer
        {
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    var PlineCls = RXObject.GetClass(typeof(Polyline));
                    var lineCls = RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Line));
                    var MTxtCls = RXObject.GetClass(typeof(MText));
                    var TxtCls = RXObject.GetClass(typeof(DBText));
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;  //Temporarily modified
                            if (subent.Layer == layerName)
                            {
                                if (id.ObjectClass == PlineCls)
                                {
                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                    if (pline.Layer.Equals(layerName, System.StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        switch (layerName)
                                        {
                                            case "0":
                                                Plugin.azeropline.Add(pline);
                                                break;
                                            case "_Window":
                                                Plugin.awindowpline.Add(pline);
                                                break;
                                            case "_Room":
                                                Plugin.aroompline.Add(pline);
                                                break;
                                            case "_Door":
                                                Plugin.adoorpline.Add(pline);
                                                break;
                                            case "_Plot":
                                                Plugin.aplotpline.Add(pline);
                                                break;
                                            case "_MainRoad":
                                                Plugin.amroadpline.Add(pline);
                                                break;
                                            case "_IndivSubPlot":
                                                Plugin.aindvSubPltpline.Add(pline);
                                                break;
                                            case "_InternalRoad":
                                                Plugin.ainterroadpline.Add(pline);
                                                break;
                                            case "_OrganizedOpenSpace":
                                                Plugin.aopenspacepline.Add(pline);
                                                break;
                                            case "_Amenity":
                                                Plugin.aAmenitypline.Add(pline);
                                                break;
                                            case "_MortgageArea":
                                                Plugin.aMortgageAreapline.Add(pline);
                                                break;
                                            case "_Splay":
                                                Plugin.asplaypline.Add(pline);
                                                break;
                                            case "_BufferZone":
                                                Plugin.aBufferpline.Add(pline);
                                                break;
                                            case "_WaterBodies":
                                                Plugin.aWaterBodypline.Add(pline);
                                                break;
                                            case "_WaterLine":
                                                Plugin.aWaterlinepline.Add(pline);
                                                break;
                                            case "_LeftoverOwnersLand":
                                                Plugin.aLeftownerspline.Add(pline);
                                                break;
                                            case "_SurrenderToAuthority":
                                                Plugin.aSurAuthpline.Add(pline);
                                                break;
                                            case "_CompoundWall":
                                                Plugin.aCompndwllpline.Add(pline);
                                                break;
                                            case "_ElectricLine":
                                                Plugin.aElectricpline.Add(pline);
                                                break;
                                            case "_GroundLevel":
                                                Plugin.aGllvlpline.Add(pline);
                                                break;
                                            case "_FloorInSection":
                                                Plugin.aFlrinSecpline.Add(pline);
                                                break;
                                            case "_ProposedWork":
                                                //Plugin.aPropWrkpline = pline;
                                                Plugin.aprpwrkpline.Add(pline);
                                                break;
                                            case "_Parking":
                                                Plugin.aParkingpline.Add(pline);
                                                break;
                                            case "_Driveway":
                                                Plugin.aDrivewaypline.Add(pline);
                                                break;
                                            case "_Ramp":
                                                Plugin.arampline.Add(pline);
                                                break;
                                            case "_Floor":
                                                Plugin.aFloorpline.Add(pline);
                                                break;
                                            case "_SlabCutoutVoid":
                                                Plugin.aVoidpline.Add(pline);
                                                break;
                                            case "_AccessoryUse":
                                                Plugin.aAccusepline.Add(pline);
                                                break;
                                            case "_NalaRoad":
                                                Plugin.aNalapline.Add(pline);
                                                break;
                                            case "_StairCase":
                                                if (pline.Closed)
                                                    Plugin.aStairpline.Add(pline);
                                                break;
                                            case "_Passage":
                                                Plugin.aPassagepline.Add(pline);
                                                break;
                                            case "_VentilationShaft":
                                                Plugin.aVenShaftpline.Add(pline);
                                                break;
                                            case "_RoadWidening":
                                                Plugin.aRdWidepline.Add(pline);
                                                break;
                                            case "_Section":
                                                Plugin.aSectionpline.Add(pline);
                                                break;
                                            case "_PrintAdditionalDetail":
                                                Plugin.aprintaddpline.Add(pline);
                                                break;
                                            case "_BuildingName":
                                                Plugin.abuildingNmpline.Add(pline);
                                                break;
                                            case "_ResiBUAOutline":
                                                Plugin.aResiBUApline.Add(pline);
                                                break;
                                            case "_SpecialUseBUAOutline":
                                                Plugin.aSpecBUApline.Add(pline);
                                                break;
                                            case "_CommBUAOutline":
                                                Plugin.aComBUApline.Add(pline);
                                                break;
                                            case "_IndBUAOutline":
                                                Plugin.aIndBUApline.Add(pline);
                                                break;
                                            case "_CarpetArea":
                                                Plugin.aCarpetpline.Add(pline);
                                                break;
                                            case "_Lift":
                                                Plugin.aLiftpline.Add(pline);
                                                break;
                                            case "_Terrace":
                                                Plugin.aTerracepline.Add(pline);
                                                break;
                                            case "_SitePlan":
                                                Plugin.aSitePlanplilne.Add(pline);
                                                break;
                                            case "_NetPlot":
                                                Plugin.anetpltpline.Add(pline);
                                                break;
                                            case "_Balcony":
                                                Plugin.aBalconypline.Add(pline);
                                                break;
                                        }
                                    }
                                }
                                if (id.ObjectClass == MTxtCls)
                                {
                                    var pObj = (MText)tr.GetObject(id, OpenMode.ForRead);
                                    switch (layerName)
                                    {
                                        case "_Window":
                                            Plugin.awindowNmTxt.Add(pObj);
                                            break;
                                        case "_Room":
                                            Plugin.aroomNmTxt.Add(pObj);
                                            break;
                                        case "_Door":
                                            Plugin.adoorNmTxt.Add(pObj);
                                            break;
                                        case "_Plot":
                                            Plugin.aplotNmTxt.Add(pObj);
                                            break;
                                        case "_MainRoad":
                                            Plugin.amroadNmTxt.Add(pObj);
                                            break;
                                        case "_IndivSubPlot":
                                            Plugin.aindvsubPltTxt.Add(pObj);
                                            break;
                                        case "_InternalRoad":
                                            Plugin.ainterroadTxt.Add(pObj);
                                            break;
                                        case "_OrganizedOpenSpace":
                                            Plugin.aopenspaceTxt.Add(pObj);
                                            break;
                                        case "_Amenity":
                                            Plugin.aAmenityTxt.Add(pObj);
                                            break;
                                        case "_MortgageArea":
                                            Plugin.aMortgageAreaTxt.Add(pObj);
                                            break;
                                        case "_Splay":
                                            Plugin.asplayTxt.Add(pObj);
                                            break;
                                        case "_BufferZone":
                                            Plugin.aBufferTxt.Add(pObj);
                                            break;
                                        case "_WaterBodies":
                                            Plugin.aWaterBodyTxt.Add(pObj);
                                            break;
                                        case "_WaterLine":
                                            Plugin.aWaterlineTxt.Add(pObj);
                                            break;
                                        case "_LeftoverOwnersLand":
                                            Plugin.aLeftOwnersTxt.Add(pObj);
                                            break;
                                        case "_SurrenderToAuthority":
                                            Plugin.aSurAuthTxt.Add(pObj);
                                            break;
                                        case "_CompoundWall":
                                            Plugin.aCmpWallTxt.Add(pObj);
                                            break;
                                        case "_ElectricLine":
                                            Plugin.aElineTxt.Add(pObj);
                                            break;
                                        case "_GroundLevel":
                                            Plugin.aGllvlTxt.Add(pObj);
                                            break;
                                        case "_FloorInSection":
                                            Plugin.aFlrinSecTxt.Add(pObj);
                                            break;
                                        case "_ProposedWork":
                                            //Plugin.aPropWrkTxt = pObj;
                                            Plugin.aprpWrkTxt.Add(pObj);
                                            break;
                                        case "_Parking":
                                            Plugin.aParkingTxt.Add(pObj);
                                            break;
                                        case "_Driveway":
                                            Plugin.aDrivewayTxt.Add(pObj);
                                            break;
                                        case "_Ramp":
                                            Plugin.arampTxt.Add(pObj);
                                            break;
                                        case "_SlabCutoutVoid":
                                            Plugin.aVoidTxt.Add(pObj);
                                            break;
                                        case "_AccessoryUse":
                                            Plugin.aAccuseTxt.Add(pObj);
                                            break;
                                        case "_NalaRoad":
                                            Plugin.aNalaTxt.Add(pObj);
                                            break;
                                        case "_StairCase":
                                            Plugin.aStairTxt.Add(pObj);
                                            break;
                                        case "_Passage":
                                            Plugin.aPassageTxt.Add(pObj);
                                            break;
                                        case "_VentilationShaft":
                                            Plugin.aVenShaftTxt.Add(pObj);
                                            break;
                                        case "_RoadWidening":
                                            Plugin.aRdWideTxt.Add(pObj);
                                            break;
                                        case "_Floor":
                                            Plugin.aFloorTxt.Add(pObj);
                                            break;
                                        case "0":
                                            Plugin.aZeromTxt.Add(pObj);
                                            break;
                                        case "_PrintAdditionalDetail":
                                            Plugin.aprintaddTxt.Add(pObj);
                                            break;
                                        case "_BuildingName":
                                            Plugin.abldingNmTxt.Add(pObj);
                                            break;
                                        case "_ResiBUAOutline":
                                            Plugin.aResiBUATxt.Add(pObj);
                                            break;
                                        case "_SpecialUseBUAOutline":
                                            Plugin.aSpecBUATxt.Add(pObj);
                                            break;
                                        case "_CommBUAOutline":
                                            Plugin.aComBUATxt.Add(pObj);
                                            break;
                                        case "_IndBUAOutline":
                                            Plugin.aIndBUATxt.Add(pObj);
                                            break;
                                        case "_CarpetArea":
                                            Plugin.aCarpetTxt.Add(pObj);
                                            break;
                                        case "_Terrace":
                                            Plugin.aTerraceTxt.Add(pObj);
                                            break;
                                        case "_Lift":
                                            Plugin.aLiftTxt.Add(pObj);
                                            break;
                                        case "_Section":
                                            Plugin.aSecTxt.Add(pObj);
                                            break;
                                        case "_SitePlan":
                                            Plugin.aSitePlanpTxt.Add(pObj);
                                            break;
                                        case "_NetPlot":
                                            Plugin.anetpltTxt.Add(pObj); break;
                                            break;
                                        case "_Balcony":
                                            Plugin.aBalconyTxt.Add(pObj);
                                            break;
                                    }
                                }
                                if (id.ObjectClass == TxtCls)
                                {
                                    var pObj1 = (DBText)tr.GetObject(id, OpenMode.ForRead); //Temporarily modified
                                    switch (layerName)
                                    {                                        
                                        case "_Floor":
                                            Plugin.aFloorsTxt.Add(pObj1);
                                            break;
                                        case "_FloorInSection":
                                            Plugin.aFlrinSecSTxt.Add(pObj1);
                                            break;
                                        case "0":
                                            Plugin.aZeroTxt.Add(pObj1);
                                            break;
                                        case "_Splay":
                                            Plugin.asplyDBTxt.Add(pObj1);
                                            break;
                                        case "_InternalRoad":
                                            Plugin.ainterloadDBTxt.Add(pObj1);
                                            break;
                                        case "_IndivSubPlot":
                                            Plugin.aindvsubDBTxt.Add(pObj1);
                                            break;
                                        case "_Plot":
                                            Plugin.apltDBTxt.Add(pObj1);
                                            break;
                                        case "_CompoundWall":
                                            Plugin.acmpndWallDBTxt.Add(pObj1);
                                            break;
                                        case "_Terrace":
                                            Plugin.aTerraceDBTxt.Add(pObj1);
                                            break;
                                        case "_Window":
                                            Plugin.aWindDBTxt.Add(pObj1);
                                            break;
                                        case "_VentilationShaft":
                                            Plugin.aVenShaftDBTxt.Add(pObj1);
                                            break;
                                        case "_Room":
                                            Plugin.aRoomDBTxt.Add(pObj1);
                                            break;
                                        case "_Door":
                                            Plugin.aDoorDBTxt.Add(pObj1);
                                            break;
                                        case "_MainRoad":
                                            Plugin.aMroadDBTxt.Add(pObj1);
                                            break;
                                        case "_OrganizedOpenSpace":
                                            Plugin.aOrgOpenDBTxt.Add(pObj1);
                                            break;
                                        case "_Amenity":
                                            Plugin.aAmenDBTxt.Add(pObj1);
                                            break;
                                        case "_MortgageArea":
                                            Plugin.aMortgageDBTxt.Add(pObj1);
                                            break;
                                        case "_BufferZone":
                                            Plugin.aBufferDBTxt.Add(pObj1);
                                            break;
                                        case "_WaterBodies":
                                            Plugin.aWaterBodDBTxt.Add(pObj1);
                                            break;
                                        case "_WaterLine":
                                            Plugin.aWaterLnDBTxt.Add(pObj1);
                                            break;
                                        case "_LeftoverOwnersLand":
                                            Plugin.aLeftOverDBTxt.Add(pObj1);
                                            break;
                                        case "_SurrenderToAuthority":
                                            Plugin.aSurrenderDBTxt.Add(pObj1);
                                            break;
                                        case "_ElectricLine":
                                            Plugin.aElectricDBTxt.Add(pObj1);
                                            break;
                                        case "_GroundLevel":
                                            Plugin.aGroundDBTxt.Add(pObj1);
                                            break;
                                        case "_ProposedWork":
                                            //subent.Erase();
                                            Plugin.aPrpWrkDBTxt.Add(pObj1);
                                            break;
                                        case "_Parking":
                                            Plugin.aParkDBTxt.Add(pObj1);
                                            break;
                                        case "_Driveway":
                                            Plugin.aDrvwayDBTxt.Add(pObj1);
                                            break;
                                        case "_Ramp":
                                            Plugin.aRampDBTxt.Add(pObj1);
                                            break;
                                        case "_SlabCutoutVoid":
                                            Plugin.aSlabcutoutDBTxt.Add(pObj1);
                                            break;
                                        case "_AccessoryUse":
                                            Plugin.aAccesUseDBTxt.Add(pObj1);
                                            break;
                                        case "_NalaRoad":
                                            Plugin.aNalaDBText.Add(pObj1);
                                            break;
                                        case "_StairCase":
                                            Plugin.aStairDBText.Add(pObj1);
                                            break;
                                        case "_Passage":
                                            Plugin.aPassageDBTxt.Add(pObj1);
                                            break;
                                        case "_RoadWidening":
                                            Plugin.aRdWideDBTxt.Add(pObj1);
                                            break;
                                        case "_PrintAdditionalDetail":
                                            Plugin.aPrintaddtionDBTxt.Add(pObj1);
                                            break;
                                        case "_BuildingName":
                                            Plugin.aBuildNameDBTxt.Add(pObj1);
                                            break;
                                        case "_ResiBUAOutline":
                                            Plugin.aResiBuaDBTxt.Add(pObj1);
                                            break;
                                        case "_SpecialUseBUAOutline":
                                            Plugin.aspecialuseBUaDBTxt.Add(pObj1);
                                            break;
                                        case "_CommBUAOutline":
                                            Plugin.aCommBUADBTxt.Add(pObj1);
                                            break;
                                        case "_IndBUAOutline":
                                            Plugin.aIndBUADBTxt.Add(pObj1);
                                            break;
                                        case "_CarpetArea":
                                            Plugin.aCarpetDBTxt.Add(pObj1);
                                            break;
                                        case "_Lift":
                                            Plugin.aLiftDBTxt.Add(pObj1);
                                            break;
                                        case "_Section":
                                            Plugin.aSectionDBTxt.Add(pObj1);
                                            break;
                                        case "_SitePlan":
                                            Plugin.aSitePlanpDBTxt.Add(pObj1);
                                            break;
                                        case "_NetPlot":
                                            Plugin.anetpltDBTxt.Add(pObj1);
                                            break;
                                        case "_Balcony":
                                            Plugin.aBalconyDBTxt.Add(pObj1);
                                            break;
                                    }
                                }
                                if(id.ObjectClass==lineCls)
                                {
                                    var line = (Line)tr.GetObject(id, OpenMode.ForRead);
                                    switch (layerName)
                                    {
                                        case "_MarginLine":
                                            Plugin.aMargineline.Add(line);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void LayerRuleCheck_Layout(string slayer)
        {
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            //System.IO.File.WriteAllText(@"D:\splayCompareCross.txt", slayer);
            //Editor ed1 = Application.DocumentManager.MdiActiveDocument.Editor;
            //ed1.WriteMessage(" Now checking " + slayer + "Layer rule" + "\n");
            //Application.ShowAlertDialog(slayer);
            switch (slayer)
            {
                case "_Window":
                    {
                        int windowerrcnt = 0;
                        string winerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        //foreach (Polyline pln in Plugin.aroompline)
                        //{
                        //    double rarea = pln.Area;
                        //    foreach (Polyline wpl in Plugin.awindowpline)
                        //    {
                        //        for (int i = 0; i < wpl.NumberOfVertices; i++)
                        //        {
                        //            Point3d pt3 = wpl.GetPoint3dAt(i);
                        //            if (IsPointOnPolyline(pln, pt3))
                        //            {
                        //                double warea = 0;
                        //                foreach (ProsoftAcPlugin.windowrule rule in ProsoftAcPlugin.Commands.awindowrule)
                        //                {
                        //                    if (rule.objid == wpl.ObjectId)
                        //                    {
                        //                        warea = rule.width * rule.height;
                        //                        MessageBox.Show("Window Area: " + warea.ToString());
                        //                    }
                        //                }

                        //                if (warea < (rarea * 0.1))
                        //                {
                        //                    windowerrcnt++;
                        //                    Commands.windowerrcause.Add("This Window is little than 10 % area of room");
                        //                    winerrcause = winerrcause + "-" + "This Window is little than 10 % area of room";
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        foreach (Polyline pl in Plugin.awindowpline)
                        {
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.awindowNmTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectangleIsInPolyline(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch(Exception e)
                                    {
                                        
                                    }
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aWindDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    windowerrcnt++;
                                    winerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }                            
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = windowerrcnt;
                        err.lyrname = "_Window";
                        err.errcause = winerrcause;
                        err.objIdlist = objidlist;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Room":
                    {
                        int windowerrcntrm = 0;
                        string roomerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            //foreach (windowrule wr in Commands.awindowrule)
                            //{
                            //    Polyline wpl = new Polyline();
                            //    foreach (Polyline pl in Plugin.awindowpline)
                            //    {
                            //        if (pl.ObjectId == wr.objid)
                            //        {
                            //            wpl = pl;
                            //            break;
                            //        }
                            //    }
                            //    for (int i = 0; i < wpl.NumberOfVertices; i++)
                            //    {
                            //        Point3d pt3 = wpl.GetPoint3dAt(i);
                            //        if (IsPointOnPolyline(pln, pt3))
                            //        {
                            //            double area = pln.Area;
                            //            if (area > (wr.width * wr.height * 10))
                            //            {
                            //                windowerrcntrm++;
                            //                Commands.roomerrcause.Add("This room does not satisfy ventilation requirement.");
                            //                roomerrcause = roomerrcause + "-" + "This room does not satisfy ventilation requirement.";
                            //                break;
                            //            }
                            //        }
                            //    }
                            //}
                            if(pln.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aroomNmTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pln);
                                        //binTxt = RectangleIsInPolyline(pln, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch(Exception e)
                                    {
                                        
                                    }
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aRoomDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pln);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    windowerrcntrm++;
                                    roomerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pln.ObjectId);
                                }
                            }
                        }
                        ruleError errrm = new ruleError();
                        errrm.errorCnt = windowerrcntrm;
                        errrm.lyrname = "_Room";
                        errrm.errcause = roomerrcause;
                        errrm.objIdlist = objidlist;
                        Commands.errlist.Add(errrm);
                        break;
                    }
                case "_Door":
                    {
                        int doorerrcnt = 0;
                        string doorerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        //foreach (Polyline pln in Plugin.aroompline)
                        //{
                        //    double rarea = pln.Area;
                        //    foreach (Polyline dpl in Plugin.adoorpline)
                        //    {
                        //        for (int i = 0; i < dpl.NumberOfVertices; i++)
                        //        {
                        //            Point3d pt3 = dpl.GetPoint3dAt(i);
                        //            if (IsPointOnPolyline(pln, pt3))
                        //            {
                        //                double darea = 0;
                        //                foreach (ProsoftAcPlugin.doorrule rule in ProsoftAcPlugin.Commands.adoorrule)
                        //                {
                        //                    if (rule.objid == dpl.ObjectId)
                        //                    {
                        //                        darea = rule.width * rule.height;
                        //                        MessageBox.Show("Door Area: " + darea.ToString());
                        //                    }
                        //                }

                        //                if (darea < (rarea * 0.1))
                        //                {
                        //                    doorerrcnt++;
                        //                    doorerrcause +="-This Window is little than 10 % area of room";
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        foreach (Polyline pl in Plugin.adoorpline)
                        {
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.adoorNmTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectangleIsInPolyline(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch (Exception e) { }
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aDoorDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    doorerrcnt++;
                                    doorerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = doorerrcnt;
                        err.lyrname = "_Door";
                        err.errcause = doorerrcause;
                        err.objIdlist = objidlist;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Plot":
                    {
                        int ploterrcnt = 0;
                        bool istch = false;
                        bool istchinter = false;
                        string ploterrcaues = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        List<Polyline> plinterclosedlist = new List<Polyline>();
                        List<Polyline> plintercenterlist = new List<Polyline>();
                        foreach (Polyline pl1 in Plugin.amroadpline)
                        {
                            foreach (Polyline pl2 in Plugin.aplotpline)
                                istch = checkTwoPlineTouch(pl2, pl1);
                        }
                        if (!istch)
                        {
                            ploterrcaues = ploterrcaues + "-" + "Mainroad and PLot does not touch";
                            ploterrcnt += Plugin.amroadpline.Count;
                            objidlist.Add(Plugin.aplotpline[0].ObjectId);
                        }
                        foreach (Polyline pl in Plugin.ainterroadpline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plintercenterlist.Add(pl);
                            }
                            if (pl.Closed && pl.Linetype != "CENTER")
                                plinterclosedlist.Add(pl);
                        }
                        foreach (Polyline pl in Plugin.aplotpline)
                        {
                            if (pl.Closed)
                            {
                                istchinter = false;
                                foreach(Polyline plinter in plinterclosedlist)
                                {
                                    istchinter=checkTwoPlineTouch(pl, plinter);
                                    if (istchinter)
                                        break;
                                }
                                if(!istch&&!istchinter)
                                {
                                    ploterrcnt++;
                                    ploterrcaues += "-This polyline does not touch InternalRoad and MainRoad.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        foreach (Polyline pl in Plugin.aplotpline)
                        {
                            if (pl.Closed)
                            {
                                bool bdbintxt = false;
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aplotNmTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.apltDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    ploterrcnt++;
                                    ploterrcaues += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errplot = new ruleError();
                        errplot.errorCnt = ploterrcnt;
                        errplot.lyrname = "_Plot";
                        errplot.errcause = ploterrcaues;
                        errplot.objIdlist = objidlist;
                        Commands.errlist.Add(errplot);
                        break;
                    }
                case "_MainRoad":
                    {
                        string mroaderrcause = "";
                        int mroaderrcnt = 0;
                        bool istchmroad = false;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.amroadpline)
                        {
                            if (!istchmroad)
                            {
                                foreach (Polyline plinternal in Plugin.aplotpline)
                                {
                                    istchmroad = checkTwoPlineTouch(plinternal, pl);
                                    //if (istchmroad)
                                    //{
                                    //    Point3d ptleft = Commands.Getleft(plinternal);
                                    //    Point3d ptright = Commands.Getright(plinternal);
                                    //    Point3d pttop = Commands.Gettop(plinternal);
                                    //    Point3d ptbottom = Commands.Getbottom(plinternal);
                                    //    double width = ptright.X - ptleft.X;
                                    //    double height = pttop.Y - ptbottom.Y;
                                    //    if (width >= 30 || height >= 30)
                                    //        break;
                                    //    else
                                    //        istchmroad = false;
                                    //}
                                }
                            }
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.amroadNmTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch(Exception e)
                                    {   }
                                    
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aMroadDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&bdbintxt)
                                {
                                    mroaderrcnt++;
                                    mroaderrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        if (!istchmroad)
                        {
                            foreach (Polyline pl in Plugin.amroadpline)
                            {
                                foreach (Polyline plinternal in Plugin.aplotpline)
                                {
                                    istchmroad = checkTwoPlineTouch(pl, plinternal);
                                }
                            }
                        }
                        if (!istchmroad)
                        {
                            mroaderrcause += "-This layer has no entity is closed to Plot.";
                            mroaderrcnt += Plugin.amroadpline.Count;
                            foreach (Polyline pl1 in Plugin.amroadpline)
                                objidlist.Add(pl1.ObjectId);
                        }
                        if(amroadNmTxt.Count!=0)
                        {
                            string strmRdtxt = amroadNmTxt[0].Contents.ToLower();
                            string strexist = "mt wide existing";
                            string strprop = "mt wide proposed";
                            double exwidth = -1;
                            double propwidth = -1;
                            int posex = strmRdtxt.IndexOf(strexist);                            
                            int posprop = strmRdtxt.IndexOf(strprop);
                            if (posex < posprop)
                            {
                                if (posex != -1)
                                {
                                    int pos = strmRdtxt.IndexOf("m") - 1;
                                    string strtmp = strmRdtxt.Substring(0, pos);
                                    exwidth = Convert.ToDouble(strtmp);
                                    int pos1 = pos + 17;
                                    strmRdtxt=strmRdtxt.Remove(0, pos1+1);
                                    pos= strmRdtxt.IndexOf("m") - 1;
                                    strtmp= strmRdtxt.Substring(0, pos);
                                    propwidth= Convert.ToDouble(strtmp);
                                }
                            }
                            else
                            {
                                if (posprop != -1)
                                {
                                    int pos = strmRdtxt.IndexOf("m") - 1;
                                    string strtmp = strmRdtxt.Substring(0, pos);
                                    propwidth = Convert.ToDouble(strtmp);
                                    int pos1 = pos + 17;
                                    strmRdtxt = strmRdtxt.Remove(0, pos1 + 1);
                                    pos = strmRdtxt.IndexOf("m") - 1;
                                    strtmp = strmRdtxt.Substring(0, pos);
                                    exwidth = Convert.ToDouble(strtmp);
                                }
                            }
                            if(exwidth!=propwidth&&exwidth>0&&propwidth>0)
                            {
                                if(Plugin.aRdWidepline.Count==0)
                                {
                                    mroaderrcnt++;
                                    mroaderrcause += "-Existing Road Width does not equals to Proposed Road width. It requires RoadWidening.";
                                    objidlist.Add(amroadpline[0].ObjectId);
                                }
                            }
                            //else if(exwidth < 0 || propwidth < 0)
                            //if (aRdWidepline.Count == 0)
                            //{
                            //    mroaderrcnt++;
                            //    mroaderrcause += "-Can not get values of Existing Road Width. ";
                            //    objidlist.Add(amroadpline[0].ObjectId);
                            //}
                        }
                        ruleError errmroad = new ruleError();
                        errmroad.errorCnt = mroaderrcnt;
                        errmroad.lyrname = "_MainRoad";
                        errmroad.errcause = mroaderrcause;
                        errmroad.objIdlist = objidlist;
                        Commands.errlist.Add(errmroad);
                        break;
                    }
                case "_IndivSubPlot":
                    {
                        double totalAreaSubPlt = 0;
                        string errcause = "";
                        bool istch1 = false;
                        bool istchmrd = false;
                        bool istchplt = false;
                        int inderrcnt = 0;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aindvSubPltpline)
                        {
                            totalAreaSubPlt += pl.Area; istch1 = false;
                            if(pl.Closed)
                            {
                                foreach (Polyline plintrd in Plugin.ainterroadpline)
                                {
                                    if(plintrd.Closed)
                                    {
                                        istch1 = checkTwoPlineTouch(plintrd, pl);
                                        //if(!istch1)
                                        //    istch1 = checkTwoPlineTouch(pl, plintrd);
                                        if (istch1)
                                            break;
                                    }
                                }
                                istchmrd = false;
                                foreach (Polyline plintrd in Plugin.amroadpline)
                                {
                                    if (plintrd.Closed)
                                    {
                                        istchmrd = checkTwoPlineTouch(plintrd, pl);
                                        //if(!istch1)
                                        //    istch1 = checkTwoPlineTouch(pl, plintrd);
                                        if (istchmrd)
                                            break;
                                    }
                                }
                                istchplt = false;
                                foreach (Polyline plintrd in Plugin.aplotpline)
                                {
                                    if (plintrd.Closed)
                                    {
                                        istchplt = checkTwoPlineTouch(plintrd, pl);
                                        //if(!istch1)
                                        //    istch1 = checkTwoPlineTouch(pl, plintrd);
                                        if (istchplt)
                                            break;
                                    }
                                }
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aindvsubPltTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aindvsubDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, pl);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                
                                if (!binTxt&&!binDBtxt)
                                {
                                    inderrcnt++;
                                    errcause += "-This polyline does not have MText.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                            if(Plugin.projtypestate!=5)
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                        && (pline.Layer != pl.Layer) && (pline.Layer != "_Splay") && (pline.Layer != "_ProposedWork")
                                                        && (pline.Layer != "_CompoundWall")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        inderrcnt++;
                                                        errcause += str;
                                                        objidlist.Add(pl.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                            
                            if (!istch1&&!istchmrd&&!istchplt)
                            {
                                inderrcnt++;
                                errcause += "-IndivSubPlot Polyline does not touch with anyone of Internal Road, MainRoad and Plot layer.";
                                objidlist.Add(pl.ObjectId);
                            }
                        }

                        ruleError errIndsub = new ruleError();
                        errIndsub.errorCnt = inderrcnt;
                        errIndsub.lyrname = "_IndivSubPlot";
                        errIndsub.errcause = errcause;
                        errIndsub.objIdlist = objidlist;
                        Commands.errlist.Add(errIndsub);
                        //RuleCheckForm.addin
                        break;
                    }
                case "_InternalRoad":
                    {
                        bool istchmrd = false, istchindv = false, istchamenity = false, istchopensp = false;
                        string intrderrcause = "";
                        int intrderrcnt = 0;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        List<Polyline> plclosedlist = new List<Polyline>();
                        List<Polyline> plcenterlist = new List<Polyline>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.ainterroadpline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plcenterlist.Add(pl);
                            }
                            if (pl.Closed)
                                plclosedlist.Add(pl);
                        }

                        foreach (Polyline plclo in plclosedlist)
                        {
                            bool bcenter = false;
                            foreach (Polyline plcen in plcenterlist)
                            {
                                if (PolyIsInPolyLine(plclo, plcen))
                                {
                                    bcenter = true;
                                }
                            }
                            if (!bcenter)
                            {
                                intrderrcause += "-This polyline has no centerline.";
                                intrderrcnt++;
                                objidlist.Add(plclo.ObjectId);
                            }
                        }
                        foreach (Polyline plinternal in Plugin.ainterroadpline)
                        {
                            if (plinternal.Closed)
                            {
                                Point3d ptleft = Commands.Getleft(plinternal);
                                Point3d ptright = Commands.Getright(plinternal);
                                Point3d pttop = Commands.Gettop(plinternal);
                                Point3d ptbottom = Commands.Getbottom(plinternal);
                                double width = ptright.X - ptleft.X;
                                double height = pttop.Y - ptbottom.Y;
                                if (width < 9 || height <= 0.0)
                                {
                                    intrderrcause += "-some Internal road width is smaller than 9.0 mts.";
                                    intrderrcnt++;
                                    objidlist.Add(plinternal.ObjectId);
                                }
                                istchindv = false;
                                if (!istchmrd)
                                    foreach (Polyline plmrd in Plugin.amroadpline)
                                    {
                                        if (istchmrd = checkTwoPlineTouch(plmrd, plinternal))
                                            break;
                                    }
                                if (!istchmrd)
                                    foreach (Polyline plmrd in Plugin.amroadpline)
                                    {
                                        if (istchmrd = checkTwoPlineTouch(plinternal, plmrd))
                                            break;
                                    }
                                if (!istchamenity)
                                    foreach (Polyline plamen in Plugin.aAmenitypline)
                                    {
                                        if (istchamenity = checkTwoPlineTouch(plinternal, plamen))
                                            break;
                                    }
                                if (!istchamenity)
                                    foreach (Polyline plamen in Plugin.aAmenitypline)
                                    {
                                        if (istchamenity = checkTwoPlineTouch(plamen, plinternal))
                                            break;
                                    }
                                if (!istchopensp)
                                    foreach (Polyline plopen in Plugin.aopenspacepline)
                                    {
                                        if (istchopensp = checkTwoPlineTouch(plinternal, plopen))
                                            break;
                                    }
                                if (!istchopensp)
                                    foreach (Polyline plopen in Plugin.aopenspacepline)
                                    {
                                        if (istchopensp = checkTwoPlineTouch(plopen, plinternal))
                                            break;
                                    }
                                foreach (Polyline plindv in Plugin.aindvSubPltpline)
                                {
                                    if (istchindv = checkTwoPlineTouch(plinternal, plindv))
                                        break;
                                }
                                if (!istchindv)
                                {
                                    intrderrcause += "-some Internal roads are not closed with individual sub plots layer.";
                                    intrderrcnt++;
                                    objidlist.Add(plinternal.ObjectId);
                                }
                                bool binTxt = false;
                                foreach (MText txt in Plugin.ainterroadTxt)
                                {
                                    //binTxt = RectangleIsInPolyline(plinternal, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    binTxt = IsPointInside(txt.Location, plinternal);
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.ainterloadDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plinternal);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    intrderrcnt++;
                                    intrderrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plinternal.ObjectId);
                                }
                            }
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
                                            //Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead) as Polyline;
                                                //Application.ShowAlertDialog(pline.ObjectId.ToString());
                                                if (pline.Layer == "0")
                                                    continue;
                                                if ((pline.ObjectId == plinternal.ObjectId))
                                                    continue;
                                                if (PolyIsInPolyLine(plinternal, pline) && (pline.Layer != plinternal.Layer) && (pline.Closed == true) && (plinternal.Closed == true)
                                                        && (!pline.Layer.Contains("_Fire")))
                                                {
                                                    string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                    intrderrcnt++;
                                                    intrderrcause += str;
                                                    objidlist.Add(plinternal.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        if (!istchmrd)
                        {
                            bool istchmroad = false;
                            foreach (Polyline pl in Plugin.amroadpline)
                            {
                                if (!istchmroad)
                                {
                                    foreach (Polyline plinternal in Plugin.aplotpline)
                                    {
                                        istchmroad = checkTwoPlineTouch(pl, plinternal);
                                    }
                                }
                            }
                            if (!istchmroad)
                            {
                                intrderrcause += "-Any Internal roads are not closed with MainRoad layer.";
                                intrderrcnt++;
                                objidlist.Add(Plugin.amroadpline[0].ObjectId);
                            }

                        }
                        //if (!istchamenity)
                        //{
                        //    intrderrcause += "-Every Internal roads are not closed with Socialinfrastructure layer.";
                        //    intrderrcnt++;
                        //    Application.ShowAlertDialog("socialinfra");
                        //    //foreach(Polyline pl in Plugin.asoc)
                        //}
                        if (!istchopensp)
                        {
                            intrderrcnt++;
                            foreach (Polyline pl in Plugin.aopenspacepline)
                            {
                                intrderrcause += "-Any Internal roads are not closed with organization open space layer.";
                                objidlist.Add(pl.ObjectId);
                            }
                        }
                        ruleError errIntrd = new ruleError();
                        errIntrd.errorCnt = intrderrcnt;
                        errIntrd.lyrname = "_InternalRoad";
                        errIntrd.errcause = intrderrcause;
                        errIntrd.objIdlist = objidlist;
                        Commands.errlist.Add(errIntrd);
                        break;
                    }
                case "_OrganizedOpenSpace":
                    {
                        double totalopenspacearea = 0, PlotArea1 = 0;
                        string openerrcause = "";
                        int openerrcnt = 0;
                        bool istchopen = false;
                        bool istchmrd = false;
                        bool istchinter = false;
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        List<Polyline> plinterclosedlist = new List<Polyline>();
                        List<Polyline> plintercenterlist = new List<Polyline>();
                        foreach (Polyline pl in Plugin.ainterroadpline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plintercenterlist.Add(pl);
                            }
                            if (pl.Closed && pl.Linetype != "CENTER")
                                plinterclosedlist.Add(pl);
                        }
                        foreach (Polyline pl in Plugin.aopenspacepline)
                        {
                            if (pl.Closed)
                            {
                                istchinter = false;
                                foreach (Polyline plinter in plinterclosedlist)
                                {
                                    istchinter = checkTwoPlineTouch(pl, plinter);
                                    if (istchinter)
                                        break;
                                }
                                istchmrd = false;
                                foreach(Polyline plrd in Plugin.amroadpline)
                                {
                                    istchmrd = checkTwoPlineTouch(pl, plrd);
                                    if (istchmrd)
                                        break;
                                }
                                if (!istchmrd && !istchinter)
                                {
                                    openerrcnt++;
                                    openerrcause += "-This polyline does not touch InternalRoad and MainRoad.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        foreach (Polyline pl in Plugin.aopenspacepline)
                        {
                            totalopenspacearea += pl.Area;
                            if (!istchopen)
                            {
                                foreach (Polyline plinternal in Plugin.ainterroadpline)
                                {
                                    istchopen = checkTwoPlineTouch(plinternal, pl);
                                    if (istchopen)
                                    {
                                        Point3d ptleft = Commands.Getleft(plinternal);
                                        Point3d ptright = Commands.Getright(plinternal);
                                        Point3d pttop = Commands.Gettop(plinternal);
                                        Point3d ptbottom = Commands.Getbottom(plinternal);
                                        double width = ptright.X - ptleft.X;
                                        double height = pttop.Y - ptbottom.Y;
                                        if (width >= 9.0 || height >= 0.0)
                                            break;
                                        else
                                            istchopen = false;
                                    }
                                }
                            }
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aopenspaceTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aOrgOpenDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    openerrcnt++;
                                    openerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                            //using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                            //{
                            //    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                            //    foreach (ObjectId btrId in blockTable)
                            //    {
                            //        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                            //        var PlineCls = RXObject.GetClass(typeof(Polyline));
                            //        if (btr.IsLayout)
                            //        {
                            //            foreach (ObjectId id in btr)
                            //            {
                            //                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            //                if (id.ObjectClass == PlineCls)
                            //                {
                            //                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                            //                    if (pline.Layer == "0")
                            //                        continue;
                            //                    if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                            //                        && (pline.Layer != pl.Layer))
                            //                    {
                            //                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                            //                        openerrcnt++;
                            //                        openerrcause += str;
                            //                        objidlist.Add(pline.ObjectId);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    tr.Commit();
                            //}
                        }
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea1 += pl2.Area;
                        
                        if (!istchopen)
                        {
                            openerrcause += "-This layer has no entity is closed to Internal road of which width is above 9mts.";
                            openerrcnt++;
                            foreach (Polyline pl in Plugin.aopenspacepline)
                                objidlist.Add(pl.ObjectId);
                        }
                        ruleError erropenspace = new ruleError();
                        erropenspace.errorCnt = openerrcnt;
                        erropenspace.lyrname = "_OrganizedOpenSpace";
                        erropenspace.errcause = openerrcause;
                        erropenspace.objIdlist = objidlist;
                        Commands.errlist.Add(erropenspace);
                        break;
                    }
                case "_AccessoryUse":
                    {
                        string openerrcause = "";
                        int openerrcnt = 0;
                        bool istchmrd = false;
                        bool istchinter = false;
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        List<Polyline> plinterclosedlist = new List<Polyline>();
                        List<Polyline> plintercenterlist = new List<Polyline>();
                        foreach (Polyline pl in Plugin.ainterroadpline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plintercenterlist.Add(pl);
                            }
                            if (pl.Closed && pl.Linetype != "CENTER")
                                plinterclosedlist.Add(pl);
                        }
                        foreach (Polyline pl in Plugin.aAccusepline)
                        {
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aAccuseTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aAccesUseDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    openerrcnt++;
                                    openerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                                istchinter = false;
                                foreach (Polyline plinter in plinterclosedlist)
                                {
                                    istchinter = checkTwoPlineTouch(pl, plinter);
                                    if (istchinter)
                                        break;
                                }
                                istchmrd = false;
                                foreach (Polyline plrd in Plugin.amroadpline)
                                {
                                    istchmrd = checkTwoPlineTouch(pl, plrd);
                                    if (istchmrd)
                                        break;
                                }

                                if (!istchmrd && !istchinter)
                                {
                                    openerrcnt++;
                                    openerrcause += "-This polyline does not touch InternalRoad and MainRoad.";
                                    objidlist.Add(pl.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                        && (pline.Layer != pl.Layer)
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                        openerrcnt++;
                                                        openerrcause += str;
                                                        objidlist.Add(pl.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        ruleError erropenspace = new ruleError();
                        erropenspace.errorCnt = openerrcnt;
                        erropenspace.lyrname = "_AccessoryUse";
                        erropenspace.errcause = openerrcause;
                        erropenspace.objIdlist = objidlist;
                        Commands.errlist.Add(erropenspace);
                        break;
                    }
                case "_Amenity":
                    {
                        string socialerrcause = "";
                        int socialerrcnt = 0;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aAmenitypline)
                        {
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aAmenityTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aAmenDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    socialerrcnt++;
                                    socialerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if((pline.ObjectId == pl.ObjectId))
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline)
                                                    && (pline.Layer != pl.Layer) && (pline.Closed == true)
                                                        && (!pline.Layer.Contains("_Fire")))
                                                {
                                                    string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                    socialerrcnt++;
                                                    socialerrcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        ruleError errsocial = new ruleError();
                        errsocial.errorCnt = socialerrcnt;
                        errsocial.lyrname = "_Amenity";
                        errsocial.errcause = socialerrcause;
                        errsocial.objIdlist = objidlist;
                        Commands.errlist.Add(errsocial);
                        break;
                    }
                case "_MortgageArea":
                    {
                        if (Plugin.subuse == "Petrol Pump")
                            break;
                        double mortarea = 0, PlotArea_mort = 0;
                        string morterrcause = "";
                        int morterrcnt = 0;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea_mort += pl2.Area;
                        if (mortarea > PlotArea_mort * 0.15)
                        {
                            morterrcnt += Plugin.aMortgageAreapline.Count;
                            foreach (Polyline pl in Plugin.aMortgageAreapline)
                            {
                                morterrcause += "-Total Organized OpenSpace Area is more than 15%";
                                objidlist.Add(pl.ObjectId);
                            }
                        }
                        foreach (Polyline pl in Plugin.aMortgageAreapline)
                        {
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aMortgageAreaTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aMortgageDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    morterrcnt++;
                                    morterrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                            //using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                            //{
                            //    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                            //    foreach (ObjectId btrId in blockTable)
                            //    {
                            //        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                            //        var PlineCls = RXObject.GetClass(typeof(Polyline));
                            //        if (btr.IsLayout)
                            //        {
                            //            foreach (ObjectId id in btr)
                            //            {
                            //                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            //                if (id.ObjectClass == PlineCls)
                            //                {
                            //                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                            //                    if (pline.Layer == "0")
                            //                        continue;
                            //                    if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                            //                        && (pline.Layer != pl.Layer))
                            //                    {
                            //                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                            //                        morterrcnt++;
                            //                        morterrcause += str;
                            //                        objidlist.Add(pline.ObjectId);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    tr.Commit();
                            //}
                        }
                        ruleError errmortArea = new ruleError();
                        errmortArea.errorCnt = morterrcnt;
                        errmortArea.lyrname = "_MortgageArea";
                        errmortArea.errcause = morterrcause;
                        errmortArea.objIdlist = objidlist;
                        Commands.errlist.Add(errmortArea);
                        break;
                    }
                case "_Splay":
                    {
                        List<ObjectId> objidlist = new List<ObjectId>();
                        bool splayonplot = false;
                        //bool isplotinter = false, isinterinter = false;
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<Point3d> interinterPtlst = new List<Point3d>();
                        foreach (Polyline plinter1 in Plugin.ainterroadpline)
                        {
                            if (plinter1.Closed)
                            {
                                foreach (Polyline plin in Plugin.ainterroadpline)
                                {
                                    //isinterinter = false;
                                    if (plin.Closed && plinter1 != plin&&plinter1.Closed)
                                    {
                                        for (int i = 0; i < plin.NumberOfVertices; i++)
                                        {
                                            Point3d ptin = plin.GetPoint3dAt(i);
                                            if (IsPointOnPolyline(plinter1, ptin))
                                            {
                                                //bool isonplinter1 = false;
                                                //for (int j = 0; j < plinter1.NumberOfVertices; j++)
                                                //{
                                                //    Point3d pttmp = plinter1.GetPoint3dAt(j);
                                                //    if (pttmp == ptin)
                                                //    {
                                                //        isonplinter1 = true;
                                                //        break;
                                                //    }
                                                //}
                                                //if (!isonplinter1)
                                                    interinterPtlst.Add(ptin);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        List<Point3d> interinterPtlst1 = new List<Point3d>(interinterPtlst);
                        foreach (Polyline plsply in Plugin.asplaypline)
                        {
                            for (int i = 0; i < plsply.NumberOfVertices; i++)
                            {
                                Point3d ptsptmp = plsply.GetPoint3dAt(i);
                                foreach (Point3d ptint in interinterPtlst)
                                {                                    
                                    if (IsSamePoint(ptsptmp, ptint))
                                    {
                                        interinterPtlst1.Remove(ptint);
                                    }                                        
                                }
                            }
                            if(plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.asplayTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.asplyDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }                            
                        }

                        //System.IO.File.WriteAllText(@"D:\splayCompareCross.txt", str);
                        //string tmpstr = "splay count: "+Plugin.asplaypline.Count.ToString()+" cross Point Count: "+
                        //    interinterPtlst1.Count.ToString() + " " + System.DateTime.Now.ToString() + Environment.NewLine;
                        //foreach (Point3d pt in interinterPtlst1)
                        //{
                        //    tmpstr += pt.ToString();
                        //    tmpstr += Environment.NewLine;
                        //}
                        //System.IO.File.WriteAllText(@"D:\splay.txt", tmpstr);

                        foreach (Point3d pt in interinterPtlst)
                        {
                            foreach(Polyline pl in Plugin.aopenspacepline)
                            {
                                if (IsPointOnPolyline(pl, pt))
                                    interinterPtlst1.Remove(pt);
                            }
                            foreach(Polyline pl in Plugin.aAmenitypline)
                                if (IsPointOnPolyline(pl, pt))
                                    interinterPtlst1.Remove(pt);
                        }
                        foreach (Point3d pttmp in interinterPtlst1)
                        {
                            foreach (Polyline pl in Plugin.ainterroadpline)
                            {
                                if (IsPointOnPolyline(pl, pttmp))
                                {
                                    bool isalready = false;
                                    foreach (ObjectId id in objidlist)
                                    {
                                        if (id == pl.ObjectId)
                                        {
                                            isalready = true;
                                            break;
                                        }
                                    }
                                    if (!isalready)
                                    {
                                        splayerrcnt++;
                                        splayerrcause += "-This Road should has Splay.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }
                        if(Plugin.aplotpline.Count==0)
                        {
                            splayerrcause += "This drawing has no entity in Plot layer.";
                            splayerrcnt++;
                            objidlist.Add(Plugin.asplaypline[0].ObjectId);
                        }
                        else
                        {
                            foreach (Polyline pl in Plugin.asplaypline)
                            {
                                Polyline plplt = Plugin.aplotpline[0];
                                splayonplot = false;
                                //istchsplay_mrd = false;
                                //istchsplay_intrd = false;
                                Point3d ptleft = Commands.Getleft(pl);
                                Point3d ptright = Commands.Getright(pl);
                                Point3d pttop = Commands.Gettop(pl);
                                Point3d ptbottom = Commands.Getbottom(pl);
                                splayonplot = PolyIsInPolyLine(plplt, pl);
                                if (!splayonplot)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-Some Splay entities are not in Plot Area.";
                                    objidlist.Add(pl.ObjectId);
                                }

                                double width = ptleft.X - ptright.X;
                                //if (mroadwidth < 12)
                                //{
                                //    if (width != 3)
                                //    {
                                //        splayerrcause += "-splay provision should be 3mX3m";
                                //        splayerrcnt++;
                                //        objidlst.Add(pl.ObjectId);
                                //    }
                                //}
                                //if (mroadwidth >= 12 && mroadwidth < 24)
                                //{
                                //    if (width != 4.5)
                                //    {
                                //        splayerrcause += "-splay provision should be 4.5mX4.5m";
                                //        splayerrcnt++;
                                //        objidlst.Add(pl.ObjectId);
                                //    }
                                //}
                                //if (mroadwidth > 24)
                                //{
                                //    if (width != 6)
                                //    {
                                //        splayerrcause += "-splay provision should be 6mX6m";
                                //        splayerrcnt++;
                                //        objidlst.Add(pl.ObjectId);
                                //    }
                                //}
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Splay";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_BufferZone":
                    {
                        //bool istchbuffer = false;
                        string buferrcause = "";
                        int buferrcnt = 0;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aBufferpline)
                        {
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aBufferTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aBufferDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    buferrcnt++;
                                    buferrcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                    && (pline.Layer != pl.Layer)
                                                        && (!pline.Layer.Contains("_Fire")))
                                                {
                                                    string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                    buferrcnt++;
                                                    buferrcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        //foreach (Polyline pl in Plugin.aBufferpline)
                        //{
                        //    if (Plugin.aWaterBodypline.Count != 0)
                        //    {
                        //        istchbuffer = false;
                        //        foreach (Polyline plwbody in Plugin.aWaterBodypline)
                        //        {
                        //            istchbuffer = checkTwoPlineTouch(plwbody, pl);
                        //            if (istchbuffer)
                        //            {
                        //                Point3d ptleft = Commands.Getleft(pl);
                        //                Point3d ptright = Commands.Getright(pl);
                        //                Point3d pttop = Commands.Gettop(pl);
                        //                Point3d ptbottom = Commands.Getbottom(pl);
                        //                double width = ptright.X - ptleft.X;
                        //                double height = pttop.Y - ptbottom.Y;
                        //                if (height != 2)
                        //                {
                        //                    buferrcause += "-BufferZone thickness is not 2.0mts.";
                        //                    buferrcnt++;
                        //                }
                        //                break;
                        //            }
                        //        }
                        //        if (!istchbuffer)
                        //        {
                        //            buferrcause += "-BufferZone is not closed with WaterBodies.";
                        //            buferrcnt++;
                        //        }
                        //    }
                        //    if (Plugin.aElectricpline.Count != 0)
                        //    {
                        //        istchbuffer = false;
                        //        foreach (Polyline pleline in Plugin.aElectricpline)
                        //        {
                        //            istchbuffer = checkTwoPlineTouch(pleline, pl);
                        //            if (istchbuffer)
                        //            {
                        //                Point3d ptleft = Commands.Getleft(pl);
                        //                Point3d ptright = Commands.Getright(pl);
                        //                Point3d pttop = Commands.Gettop(pl);
                        //                Point3d ptbottom = Commands.Getbottom(pl);
                        //                double width = ptright.X - ptleft.X;
                        //                double height = pttop.Y - ptbottom.Y;
                        //                if (height != 10)
                        //                {
                        //                    buferrcause += "-BufferZone thickness is not 10.0mts.";
                        //                    buferrcnt++;
                        //                }
                        //                break;
                        //            }
                        //        }
                        //        if (!istchbuffer)
                        //        {
                        //            buferrcause += "-BufferZone is not closed with ElectricLine.";
                        //            buferrcnt++;
                        //        }
                        //    }
                        //    if (Plugin.aWaterlinepline.Count != 0)
                        //    {
                        //        istchbuffer = false;
                        //        foreach (Polyline pwline in Plugin.aWaterlinepline)
                        //        {
                        //            istchbuffer = checkTwoPlineTouch(pwline, pl);
                        //            if (istchbuffer)
                        //            {
                        //                Point3d ptleft = Commands.Getleft(pl);
                        //                Point3d ptright = Commands.Getright(pl);
                        //                Point3d pttop = Commands.Gettop(pl);
                        //                Point3d ptbottom = Commands.Getbottom(pl);
                        //                double width = ptright.X - ptleft.X;
                        //                double height = pttop.Y - ptbottom.Y;
                        //                if (height != 10)
                        //                {
                        //                    buferrcause += "-BufferZone thickness is not 2.0mts.";
                        //                    buferrcnt++;
                        //                }
                        //                break;
                        //            }
                        //        }
                        //        if (!istchbuffer)
                        //        {
                        //            buferrcause += "-BufferZone is not closed with ElectricLine.";
                        //            buferrcnt++;
                        //        }
                        //    }
                        //}
                        //foreach (ObjectId id in objidlist)
                        //    SetViewCenterToObject(id);
                        ruleError errbuf = new ruleError();
                        errbuf.errorCnt = buferrcnt;
                        errbuf.lyrname = "_BufferZone";
                        errbuf.errcause = buferrcause;
                        Commands.errlist.Add(errbuf);
                        break;
                    }
                case "_LeftoverOwnersLand":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aLeftownerspline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aLeftOwnersTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aLeftOverDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                    && (pline.Layer != pl.Layer)
                                                        && (!pline.Layer.Contains("_Fire")))
                                                {
                                                    string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                    errcnt++;
                                                    errcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        ruleError errleft = new ruleError();
                        errleft.errorCnt = errcnt;
                        errleft.lyrname = "_LeftoverOwnersLand";
                        errleft.errcause = errcause;
                        errleft.objIdlist = objidlist;
                        Commands.errlist.Add(errleft);
                        break;
                    }
                case "_SurrenderToAuthority":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aSurAuthpline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aSurAuthTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aSurrenderDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errleft = new ruleError();
                        errleft.errorCnt = errcnt;
                        errleft.lyrname = "_SurrenderToAuthority";
                        errleft.errcause = errcause;
                        errleft.objIdlist = objidlist;
                        Commands.errlist.Add(errleft);
                        break;
                    }
                case "_CompoundWall":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aCompndwllpline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aCmpWallTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.acmpndWallDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errleft = new ruleError();
                        errleft.errorCnt = errcnt;
                        errleft.lyrname = "_CompoundWall";
                        errleft.errcause = errcause;
                        errleft.objIdlist = objidlist;
                        Commands.errlist.Add(errleft);
                        break;
                    }
                case "_ElectricLine":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aElectricpline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aElectricTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aElectricDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errelec = new ruleError();
                        errelec.errorCnt = errcnt;
                        errelec.lyrname = "_ElectricLine";
                        errelec.errcause = errcause;
                        errelec.objIdlist = objidlist;
                        Commands.errlist.Add(errelec);
                        break;
                    }
                case "_PrintAdditionalDetail":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        string wdfinfstr = Commands.MakingWind_DoorList();
                        Point3d ptdetail = new Point3d(Commands.Getright(Plugin.aprintaddpline[0]).X,
                            Commands.Getbottom(Plugin.aprintaddpline[0]).Y, 0);
                        Commands.MakeWind_DoorText(wdfinfstr, ptdetail);
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aprintaddpline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aprintaddTxt)
                                {
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    binTxt= IsPointInside(txt.Location, pl);
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aPrintaddtionDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
                        }
                        
                        ruleError errelec = new ruleError();
                        errelec.errorCnt = errcnt;
                        errelec.lyrname = "_PrintAdditionalDetail";
                        errelec.errcause = errcause;
                        errelec.objIdlist = objidlist;
                        Commands.errlist.Add(errelec);
                        break;
                    }
                case "_RoadWidening":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aRdWidepline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aRdWideTxt)
                                {
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    binTxt = IsPointInside(txt.Location, pl);
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aRdWideDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                    && (pline.Layer != pl.Layer))
                                                {
                                                    string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                    errcnt++;
                                                    errcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        ruleError errelec = new ruleError();
                        errelec.errorCnt = errcnt;
                        errelec.lyrname = "_RoadWidening";
                        errelec.errcause = errcause;
                        errelec.objIdlist = objidlist;
                        Commands.errlist.Add(errelec);
                        break;
                    }
                case "_Lift":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aLiftpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aLiftTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aLiftDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Lift";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_Terrace":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aTerracepline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aTerraceTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aTerraceDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Terrace";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_SitePlan":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aSitePlanplilne)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aSitePlanpTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aSitePlanpDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position,plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_SitePlan";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_BuildingName":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.abuildingNmpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.abldingNmTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aBuildNameDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_BuildingName";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_ProposedWork":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aprpwrkpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aprpWrkTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aPrpWrkDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        foreach (Polyline plflr in Plugin.aprpwrkpline)
                        {
                            bool northblock = false;
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                // open the block table which contains all the BlockTableRecords (block definitions and spaces)
                                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                                // open the model space BlockTableRecord
                                var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                                // iterate through the model space 
                                foreach (ObjectId id in modelSpace)
                                {
                                    // check if the current ObjectId is a block reference one
                                    if (id.ObjectClass.DxfName == "INSERT")
                                    {
                                        // open the block reference
                                        var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                        // print the block name to the command line
                                        ed.WriteMessage("\n" + blockReference.Name);
                                        if (blockReference.Name.Contains("DirectionRef_PreVal"))
                                        {
                                            double widthref = blockReference.GeometricExtents.MaxPoint.X - blockReference.GeometricExtents.MinPoint.X;
                                            double heightref = blockReference.GeometricExtents.MaxPoint.Y - blockReference.GeometricExtents.MinPoint.Y;
                                            if (RectangleIsInPolyline(plflr, blockReference.Position, new Point3d(blockReference.Position.X + widthref,
                                                blockReference.Position.Y + heightref, 0)))
                                            {
                                                northblock = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                            if (!northblock)
                            {
                                splayerrcnt++;
                                splayerrcause += "-This Polyline has no Preval made DirectionRef_PreVal Block reference.";
                                objidlist.Add(plflr.ObjectId);
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_ProposedWork";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_CarpetArea":
                    {
                        int ploterrcnt = 0;
                        string ploterrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        if (Plugin.aCarpetpline.Count != 0)
                        {
                            foreach (Polyline pl in Plugin.aCarpetpline)
                            {
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.aCarpetTxt)
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.aCarpetDBTxt)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt && !bdbintxt)
                                    {
                                        ploterrcnt++;
                                        ploterrcause += "-This polyline does not have Label.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    ploterrcnt++;
                        //    ploterrcause += "-This Project has no CarpetArea Polyline.";
                        //}
                        ruleError err = new ruleError();
                        err.errorCnt = ploterrcnt;
                        err.lyrname = "_CarpetArea";
                        err.errcause = ploterrcause;
                        err.objIdlist = objidlist;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_WaterBodies":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aWaterBodypline)
                        {
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aWaterBodyTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aWaterBodDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                    && (pline.Layer != pl.Layer))
                                                {
                                                    string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                    errcnt++;
                                                    errcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        ruleError errelec = new ruleError();
                        errelec.errorCnt = errcnt;
                        errelec.lyrname = "_WaterBodies";
                        errelec.errcause = errcause;
                        errelec.objIdlist = objidlist;
                        Commands.errlist.Add(errelec);
                        break;
                    }
                case "_NetPlot":
                    {
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                        {
                            foreach (Polyline pl in Plugin.anetpltpline)
                            {
                                ObjectId oid = pl.ObjectId;
                                Entity subent = tr.GetObject(oid, OpenMode.ForWrite) as Entity;
                                subent.Erase(true);
                            }
                            BlockTable acBlkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                            BlockTableRecord acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                            if (Plugin.aRdWidepline.Count == 0)
                            {
                                Entity plnet = Plugin.aplotpline[0].Clone() as Entity;
                                plnet.Layer = "_NetPlot";
                                var id = acBlkTblRec.AppendEntity(plnet);
                                tr.AddNewlyCreatedDBObject(plnet, true);
                                ed.UpdateScreen();
                            }
                            else
                            {
                                Point3dCollection ptcol = new Point3dCollection();
                                Point3dCollection ptremainplt = new Point3dCollection();
                                Point2dCollection ptnetall = new Point2dCollection();
                                Point3dCollection ptrdcol = new Point3dCollection();
                                for (int j = 0; j < Plugin.aRdWidepline[0].NumberOfVertices; j++)
                                {
                                    Point3d ptrdwd = Plugin.aRdWidepline[0].GetPoint3dAt(j);
                                    ptrdcol.Add(ptrdwd);
                                }
                                for (int j = 0; j < Plugin.aplotpline[0].NumberOfVertices; j++)
                                {
                                    Point3d ptplt = Plugin.aplotpline[0].GetPoint3dAt(j);
                                    ptremainplt.Add(ptplt);
                                }
                                foreach (Point3d ptrdwd in ptrdcol)
                                {
                                    bool bsamept = false;
                                    for (int j = 0; j < Plugin.aplotpline[0].NumberOfVertices; j++)
                                    {
                                        Point3d ptplt = Plugin.aplotpline[0].GetPoint3dAt(j);
                                        if (!IsSamePoint(ptrdwd, ptplt))
                                        {
                                            bsamept = false;
                                        }
                                        else
                                        {
                                            ptremainplt.Remove(ptplt);
                                            bsamept = true;
                                            break;
                                        }
                                    }
                                    if (!bsamept)
                                        ptcol.Add(ptrdwd);
                                }
                                Polyline plnet = new Polyline();
                                for (int i = 0; i < ptremainplt.Count; i++)
                                {
                                    ptnetall.Add(new Point2d(ptremainplt[i].X, ptremainplt[i].Y));
                                    //plnet.AddVertexAt(i,new Point2d())
                                }
                                for (int j = 0; j < ptcol.Count; j++)
                                    ptnetall.Add(new Point2d(ptcol[j].X, ptcol[j].Y));
                                for (int k = 0; k < ptnetall.Count; k++)
                                    plnet.AddVertexAt(k, ptnetall[k], 0, 0, 0);
                                plnet.Closed = true;
                                plnet.Layer = "_NetPlot";
                                acBlkTblRec.AppendEntity(plnet);
                                tr.AddNewlyCreatedDBObject(plnet, true);
                                ed.UpdateScreen();
                            }
                            tr.Commit();
                        }
                        break;
                    }
            }
        }
        public static void LayerRuleCheck_BldgPermiss(string slayer)
        {
            //System.IO.File.WriteAllText(@"D:\layer.txt", slayer);
            //Application.ShowAlertDialog(slayer);
            //Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //ed.WriteMessage("Now checking " + slayer + "Layer rule");
            switch (slayer)
            {
                case "_Window":
                    {
                        int windowerrcnt = 0;
                        string winerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        bool iswindow = false;
                        List<Polyline> fdwindlst = new List<Polyline>();
                        if(Plugin.awindowpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.adoorpline)
                            {
                                foreach (MText txt in Plugin.adoorNmTxt)
                                {
                                    if (RectangleIsInPolyline(pl, txt.GeometricExtents.MinPoint, txt.GeometricExtents.MaxPoint))
                                    {
                                        string strtxt = txt.Text;
                                        if (strtxt.Contains("FD"))
                                        {
                                            fdwindlst.Add(pl);
                                        }
                                    }
                                }
                            }
                            foreach (Polyline pln in Plugin.aroompline)
                            {
                                iswindow = false;
                                bool b_chkWind = false;
                                foreach (MText inst in Plugin.aroomNmTxt)
                                {
                                    if (RectangleIsInPolyline(pln, inst.GeometricExtents.MinPoint, inst.GeometricExtents.MaxPoint))
                                    {
                                        string strtxt = inst.Text;
                                        if (strtxt.Contains("Utility") || strtxt.Contains("Dress Room") || strtxt.Contains("Foyer")
                                            || strtxt.Contains("Wash") || strtxt.Contains("Dinning") || strtxt.Contains("F.D"))
                                        {
                                            b_chkWind = true;
                                            break;
                                        }
                                    }
                                }
                                foreach (MText inst in Plugin.aZeromTxt)
                                {
                                    if (RectangleIsInPolyline(pln, inst.GeometricExtents.MinPoint, inst.GeometricExtents.MaxPoint))
                                    {
                                        string strtxt = inst.Text;
                                        if (strtxt.Contains("F.D"))
                                        {
                                            b_chkWind = true;
                                            break;
                                        }
                                    }
                                }
                                foreach (Polyline plfd in fdwindlst)
                                {
                                    if (checkTwoPlineTouch(plfd, pln))
                                    {
                                        b_chkWind = true;
                                        break;
                                    }
                                }
                                if (!b_chkWind)
                                {
                                    foreach (Polyline dpl in Plugin.awindowpline)
                                    {
                                        for (int i = 0; i < dpl.NumberOfVertices; i++)
                                        {
                                            Point3d pt3 = dpl.GetPoint3dAt(i);
                                            if (IsPointOnPolyline(pln, pt3))
                                            {
                                                iswindow = true;
                                                break;
                                            }
                                        }
                                        if (iswindow)
                                            break;
                                    }
                                    if (!iswindow)
                                    {
                                        windowerrcnt++;
                                        winerrcause += "-This Room does not have a Window.";
                                        objidlist.Add(pln.ObjectId);
                                    }
                                }
                            }

                            foreach (Polyline pl in Plugin.awindowpline)
                            {
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.awindowNmTxt)
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectangleIsInPolyline(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.aWindDBTxt)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt&& !bdbintxt)
                                    {
                                        windowerrcnt++;
                                        winerrcause += "-This window does not have Label.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                            ruleError err = new ruleError();
                            err.errorCnt = windowerrcnt;
                            err.lyrname = "_Window";
                            err.errcause = winerrcause;
                            err.objIdlist = objidlist;
                            Commands.errlist.Add(err);
                        }
                        else
                        {
                            windowerrcnt++;
                            winerrcause += "-This Project has no Window layer Polyline.";
                            ruleError err = new ruleError();
                            err.errorCnt = windowerrcnt;
                            err.lyrname = "_Window";
                            err.errcause = winerrcause;
                            err.objIdlist = objidlist;
                            Commands.errlist.Add(err);
                        }
                        break;
                    }
                case "_Floor":
                    {
                        string flrStr = "";
                        int flrerrCnt = 0;
                        double PlotArea = 0;
                        List<ObjectId> objlst = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        if (Plugin.aFloorTxt.Count == 0 && Plugin.aFloorsTxt.Count == 0)
                        {
                            flrStr += "-Current Floor Layer has no FloorName MTEXT or TEXT.Please reassign FloorName.";
                            flrerrCnt++;
                            foreach (Polyline pl in Plugin.aFloorpline)
                            {
                                objlst.Add(pl.ObjectId);
                            }
                        }
                        if(Plugin.aFloorpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.aFloorpline)
                            {
                                bool bhastxt = false;
                                foreach (MText txt in Plugin.aFloorTxt)
                                {
                                    bhastxt = IsPointInside(txt.Location, pl);
                                    //bhastxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        bhastxt = false;
                                    if (bhastxt)
                                        break;
                                }
                                if (!bhastxt)
                                {
                                    foreach (DBText stxt in Plugin.aFloorsTxt)
                                    {
                                        bhastxt = IsPointInside(stxt.Position, pl);
                                        if (stxt.TextString == "")
                                            bhastxt = false;
                                        if (bhastxt)
                                            break;
                                    }
                                }
                                bool bdbintxt = false;
                                if (!bhastxt)
                                {
                                    foreach (DBText dBText in Plugin.aFloorsTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!bhastxt && !bdbintxt)
                                {
                                    flrerrCnt++;
                                    flrStr += "-This Polyline has no MText or DBText.";
                                    objlst.Add(pl.ObjectId);
                                }
                            }
                            foreach (Polyline pl2 in Plugin.aplotpline)
                                PlotArea += pl2.Area;
                            bool bstilt = false;
                            List<Polyline> stiltpllist = new List<Polyline>();
                            foreach (MText flrtxt in Plugin.aFlrinSecTxt)
                            {
                                if (flrtxt.Text.Contains("Stilt"))
                                {
                                    bstilt = true;
                                    foreach (Polyline plstlt in Plugin.aFloorpline)
                                    {
                                        if (RectIsInPolyLine(plstlt, flrtxt.Location, new Point3d(flrtxt.Location.X + flrtxt.Width / 2, flrtxt.Location.Y + flrtxt.Height / 2, 0)))
                                        {
                                            stiltpllist.Add(plstlt);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            foreach (Polyline plrm in Plugin.aroompline)
                            {
                                foreach (Polyline plst in stiltpllist)
                                {
                                    if (PlotArea < 750 && bstilt)
                                    {
                                        if (PolyIsInPolyLine(plst, plrm))
                                        {
                                            flrerrCnt++;
                                            flrStr += "-This Stilt is not allowed to have rooms.";
                                            objlst.Add(plst.ObjectId);
                                        }
                                    }
                                }
                            }
                            if (PlotArea < 200 && bstilt)
                            {
                                flrerrCnt++;
                                flrStr += "-This project does not require a Stilt.";
                            }

                            foreach (Polyline plflr in Plugin.aFloorpline)
                            {
                                bool northblock = false;
                                using (Transaction tr = db.TransactionManager.StartTransaction())
                                {
                                    // open the block table which contains all the BlockTableRecords (block definitions and spaces)
                                    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                                    // open the model space BlockTableRecord
                                    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                                    // iterate through the model space 
                                    foreach (ObjectId id in modelSpace)
                                    {
                                        // check if the current ObjectId is a block reference one
                                        if (id.ObjectClass.DxfName == "INSERT")
                                        {
                                            // open the block reference
                                            var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                            // print the block name to the command line
                                            ed.WriteMessage("\n" + blockReference.Name);
                                            if (blockReference.Name.Contains("DirectionRef_PreVal"))
                                            {
                                                double widthref = blockReference.GeometricExtents.MaxPoint.X - blockReference.GeometricExtents.MinPoint.X;
                                                double heightref = blockReference.GeometricExtents.MaxPoint.Y - blockReference.GeometricExtents.MinPoint.Y;
                                                if (RectangleIsInPolyline(plflr, blockReference.Position, new Point3d(blockReference.Position.X + widthref,
                                                    blockReference.Position.Y + heightref, 0)))
                                                {
                                                    northblock = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                                if (!northblock)
                                {
                                    flrerrCnt++;
                                    flrStr += "-This layer has no Preval made DirectionRef_PreVal Block reference.";
                                    objlst.Add(plflr.ObjectId);
                                }
                            }
                            ruleError err_flrinsec = new ruleError();
                            err_flrinsec.errorCnt = flrerrCnt;
                            err_flrinsec.lyrname = "_Floor";
                            err_flrinsec.errcause = flrStr;
                            err_flrinsec.objIdlist = objlst;
                            Commands.errlist.Add(err_flrinsec);
                        }
                        else
                        {
                            flrerrCnt++;
                            flrStr += "-This Project has no Floor layer Polyline.";
                            ruleError err_flrinsec = new ruleError();
                            err_flrinsec.errorCnt = flrerrCnt;
                            err_flrinsec.lyrname = "_Floor";
                            err_flrinsec.errcause = flrStr;
                            err_flrinsec.objIdlist = objlst;
                            Commands.errlist.Add(err_flrinsec);
                        }
                        break;
                    }
                case "_FloorInSection":
                    {
                        string flrinsecStr = "";
                        int flrinsecerrCnt = 0;
                        List<ObjectId> objlst = new List<ObjectId>();
                        if (Plugin.aFlrinSecTxt.Count == 0 && Plugin.aFlrinSecSTxt.Count == 0)
                        {
                            flrinsecStr += "-Current FloorInSection Layer has no FloorName MTEXT or TEXT.Please reassign FloorName.";
                            flrinsecerrCnt++;
                            foreach (Polyline pl in Plugin.aFlrinSecpline)
                            {
                                objlst.Add(pl.ObjectId);
                            }
                        }
                        if(Plugin.aFlrinSecpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.aFlrinSecpline)
                            {
                                bool binsection = false;
                                bool bhastxt = false;
                                foreach (Polyline plsec in Plugin.aSectionpline)
                                {
                                    binsection = RectIsInPolyLine(plsec, new Point3d(Commands.Getleft(pl).X, Commands.Getbottom(pl).Y, 0),
                                        new Point3d(Commands.Getright(pl).X, Commands.Gettop(pl).Y, 0));
                                    if (binsection)
                                        break;
                                }
                                if (!binsection)
                                {
                                    flrinsecerrCnt++;
                                    flrinsecStr += "-This Polyline is not in Section layer.";
                                    objlst.Add(pl.ObjectId);
                                }
                                foreach (MText txt in Plugin.aFlrinSecTxt)
                                {
                                    if (pl.Closed)
                                    {
                                        bhastxt = IsPointInside(txt.Location, pl);
                                        //bhastxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            bhastxt = false;
                                        if (bhastxt)
                                            break;
                                    }
                                }
                                if (!bhastxt)
                                {
                                    foreach (DBText stxt in Plugin.aFlrinSecSTxt)
                                    {
                                        bhastxt = IsPointInside(stxt.Position, pl);
                                        if (stxt.TextString == "")
                                            bhastxt = false;
                                        if (bhastxt)
                                            break;
                                    }
                                }
                                if (!bhastxt)
                                {
                                    flrinsecerrCnt++;
                                    flrinsecStr += "-This Polyline has no MText or DBText. Or Text is out of Polyline range.";
                                    objlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        else
                        {
                            flrinsecerrCnt++;
                            flrinsecStr += "-This Project has no FloorinSection Polyline.";
                        }
                        
                        ruleError err_flrinsec = new ruleError();
                        err_flrinsec.errorCnt = flrinsecerrCnt;
                        err_flrinsec.lyrname = "_FloorInSection";
                        err_flrinsec.errcause = flrinsecStr;
                        err_flrinsec.objIdlist = objlst;
                        Commands.errlist.Add(err_flrinsec);
                        break;
                    }
                case "_Plot":
                    {
                        int ploterrcnt = 0;
                        string ploterrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        bool bmargin = false; 
                        if(Plugin.aplotpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.aplotpline)
                            {
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.aplotNmTxt)
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.apltDBTxt)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt && !bdbintxt)
                                    {
                                        ploterrcnt++;
                                        ploterrcause += "-This polyline does not have Label.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                                var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                                foreach (ObjectId id in modelSpace)
                                {
                                    if (id.ObjectClass.DxfName == "INSERT")
                                    {
                                        var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                        ed.WriteMessage("\n" + blockReference.Name);
                                        if (blockReference.Name.Contains("Margin_PreVal"))
                                        {
                                            bmargin = true;
                                            break;
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                            if (!bmargin)
                            {
                                ploterrcnt++;
                                ploterrcause += "-This layer has no Preval made Margin_PreVal Block reference.";
                                objidlist.Add(Plugin.aplotpline[0].ObjectId);
                            }
                        }
                        else
                        {
                            ploterrcnt++;
                            ploterrcause += "-This Project has no Plot Polyline.";
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = ploterrcnt;
                        err.lyrname = "_Plot";
                        err.errcause = ploterrcause;
                        err.objIdlist = objidlist;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_OrganizedOpenSpace":
                    {
                        double PlotArea = 0, greenArea = 0, openspaceArea = 0;
                        double length = 0;
                        string openerrcause = "";
                        int openerrcnt = 0;
                        double bldght = 0;
                        double maxht=0, minht=0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        bool btotlot = false;
                        foreach(MText mtxt in Plugin.aopenspaceTxt)
                        {
                            if(mtxt.Text.Contains("Tot lot"))
                            {
                                btotlot = true;
                                break;
                            }
                        }
                        if(!btotlot)
                        {
                            foreach(DBText dbtxt in Plugin.aOrgOpenDBTxt)
                            {
                                if(dbtxt.TextString.Contains("Tot lot"))
                                {
                                    btotlot = true;
                                    break;
                                }
                            }
                        }
                        if(Plugin.aopenspacepline.Count!=0)
                        {
                            foreach (Polyline pl2 in Plugin.aplotpline)
                                PlotArea += pl2.Area;
                            greenArea = Plugin.aopenspacepline.ElementAt(0).Area;
                            length = Plugin.aopenspacepline.ElementAt(0).Length;
                            foreach (Polyline pl in Plugin.aopenspacepline)
                            {
                                openspaceArea += pl.Area;
                                objidlst.Add(pl.ObjectId);
                            }
                            if (Plugin.aopenspaceTxt.Count != 0)
                            {
                                string openEntname = Plugin.aopenspaceTxt.ElementAt(0).Text;
                                if (openEntname.Contains("Green Strip"))
                                    openEntname = "Green Strip";
                                if (openEntname.Contains("Green Belt"))
                                    openEntname = "Green Belt";
                                if (openEntname.Contains("Tot lot"))
                                    openEntname = "Tot lot";
                                if (openEntname.Contains("OPEN SPACE"))
                                    openEntname = "OPEN SPACE";
                            }

                            foreach (Polyline pl in Plugin.aopenspacepline)
                            {
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.aopenspaceTxt)
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.aOrgOpenDBTxt)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt&&!bdbintxt)
                                    {
                                        openerrcnt++;
                                        openerrcause += "-This polyline does not have Label.";
                                        objidlst.Add(pl.ObjectId);
                                    }
                                }
                            }
                            if (Plugin.usestate == 1)
                            {
                                for (int i = 0; i < Plugin.aFlrinSecpline.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (Plugin.aFlrinSecpline.Count != 0)
                                        {
                                            maxht = Commands.Gettop(Plugin.aFlrinSecpline[i]).Y;
                                            minht = Commands.Getbottom(Plugin.aFlrinSecpline[i]).Y;
                                        }
                                    }
                                    else
                                    {
                                        if (Plugin.aFlrinSecpline.Count != 0)
                                        {
                                            if (maxht < Commands.Gettop(Plugin.aFlrinSecpline[i]).Y)
                                                maxht = Commands.Gettop(Plugin.aFlrinSecpline[i]).Y;
                                            if (minht > Commands.Gettop(Plugin.aFlrinSecpline[i]).Y)
                                                minht = Commands.Gettop(Plugin.aFlrinSecpline[i]).Y;
                                        }
                                    }
                                }
                                if (Plugin.aGllvlpline.Count != 0)
                                    minht = Commands.Gettop(Plugin.aGllvlpline[0]).Y;
                                bldght = maxht - minht;
                            }
                            if (Plugin.usestate == 0 && PlotArea > 750)
                            {
                                if(!btotlot)
                                {
                                    openerrcnt++;
                                    openerrcause += "-This project requires a " + "Tot lot.";
                                }
                            }
                            ruleError err_orgopen = new ruleError();
                            err_orgopen.errorCnt = openerrcnt;
                            err_orgopen.lyrname = "_OrganizedOpenSpace";
                            err_orgopen.errcause = openerrcause;
                            err_orgopen.objIdlist = objidlst;
                            Commands.errlist.Add(err_orgopen);
                        }
                        else
                        {
                            openerrcnt++;
                            openerrcause += "-This project has no _OrganizedOpenSpace layer Polyline.";
                            ruleError err_orgopen = new ruleError();
                            err_orgopen.errorCnt = openerrcnt;
                            err_orgopen.lyrname = "_OrganizedOpenSpace";
                            err_orgopen.errcause = openerrcause;
                            err_orgopen.objIdlist = objidlst;
                            Commands.errlist.Add(err_orgopen);
                        }
                        break;
                    }
                case "_Parking":
                    {
                        if (Plugin.subuse == "Petrol Pump")
                            break;
                        double allParkArea = 0, netBua = 0;
                        double allVshaftarea = 0, allVoidarea = 0, allAccusearea = 0;
                        //Polyline rangePl = null;
                        //Polyline ParkFlrPl = null;
                        string errcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        double PlotArea = 0;
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if (Plugin.usestate==0&&PlotArea>750)
                        {
                            if(Plugin.aParkingpline.Count==0)
                            {
                                errcause += "-Parking layer must have at least one entity.";
                                errcnt++;
                            }
                        }
                        if(Plugin.usestate==1)
                        {
                            if(Plugin.aComBUApline.Count==0&&Plugin.aSpecBUApline.Count==0&&Plugin.aIndBUApline.Count==0)
                            {
                                if (Plugin.aParkingpline.Count == 0)
                                {
                                    errcause += "-Parking layer must have at least one entity.";
                                    errcnt++;
                                }
                            }
                        }
                        foreach (Polyline pl in Plugin.aParkingpline)
                        {
                            if (pl.Closed)
                            {
                                bool bdbintxt = false;
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aParkingTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aParkDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                        && (pline.Layer != pl.Layer) && (pline.Layer != "_BuildingName")
                                                        && (pline.Layer != "_Lift") && (pline.Layer != "_StairCase") && (pline.Layer != "_AccessoryUse")
                                                        && (pline.Layer != "_SlabCutoutVoid") && (pline.Layer != "_VentilationShaft")
                                                        && (pline.Layer != "_Ramp") && (pline.Layer != "_Driveway") && (pline.Layer != "_Column")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        errcnt++;
                                                        errcause += str;
                                                        objidlst.Add(pl.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        //foreach (Polyline pl in Plugin.aParkingpline)
                        //{
                        //    if (allParkArea < pl.Area)
                        //    {
                        //        rangePl = pl;
                        //        allParkArea = pl.Area;
                        //    }
                        //}
                        //foreach (Polyline pl in Plugin.aParkingpline)
                        //{
                        //    Point3d ptleftplt = Commands.Getleft(pl);
                        //    Point3d ptrightplt = Commands.Getright(pl);
                        //    Point3d pttopplt = Commands.Gettop(pl);
                        //    Point3d ptbottomplt = Commands.Getbottom(pl);
                        //    Point3d ptUpperLeft = new Point3d(ptleftplt.X, pttopplt.Y, 0);
                        //    Point3d ptBottomRight = new Point3d(ptrightplt.X, ptbottomplt.Y, 0);
                        //    if (!IsInPolyLine(rangePl, ptUpperLeft, ptBottomRight))
                        //    {
                        //        allParkArea += pl.Area;
                        //    }
                        //}
                        //foreach (Polyline pl in Plugin.aFloorpline)
                        //{
                        //    Point3d ptleftplt = Commands.Getleft(rangePl);
                        //    Point3d ptrightplt = Commands.Getright(rangePl);
                        //    Point3d pttopplt = Commands.Gettop(rangePl);
                        //    Point3d ptbottomplt = Commands.Getbottom(rangePl);
                        //    Point3d ptUpperLeft = new Point3d(ptleftplt.X, pttopplt.Y, 0);
                        //    Point3d ptBottomRight = new Point3d(ptrightplt.X, ptbottomplt.Y, 0);
                        //    if (IsInPolyLine(pl, ptUpperLeft, ptBottomRight))
                        //        ParkFlrPl = pl;
                        //}
                        //Parkflrarea = ParkFlrPl.Area;
                        //foreach (Polyline pl in Plugin.aVShaftpline)
                        //{
                        //    allVshaftarea += pl.Area;
                        //}
                        //foreach (Polyline pl in Plugin.aVoidpline)
                        //{
                        //    allVoidarea += pl.Area;
                        //}
                        //foreach (Polyline pl in Plugin.aAccusepline)
                        //{
                        //    allAccusearea += pl.Area;
                        //}
                        netBua = allParkArea - allVshaftarea - allVoidarea + allAccusearea;
                        switch (Plugin.subuse)
                        {
                            case "MultiPlex":
                                {
                                    break;
                                }   //////////////////until 1//////////////////////////////////////////
                            case "Shopping Mall":
                            case "Information-technology IT/ITES":
                                {
                                    break;
                                }//////////////////////////////////until 2/////////////////////////////
                            case "Shop":
                            case "Store":
                            case "Retail Shop":
                            case "Bank":
                            case "Safe Deposit Vault":
                            case "Shopping Centre/mall":
                            case "Showroom":
                            case "Commercial Bldg":
                            case "Market":
                            case "Departmental Store":
                            case "Shopping Malls with Multiplexes":
                            case "SuperMarkets":
                            case "Convenience Markets":
                            case "Resicomm Bldg":
                            case "Office":
                            case "Shop and Office":
                            case "Professional Office":
                            case "Corporate Office":
                            case "Business Office":
                            case "IT Office":
                            case "Bio-Technology(BT) Office":
                            case "Corporate Commercial":
                            case "Restaurant":
                            case "Holiday Resort":
                            case "Service orRepair establishments":
                            case "Clinic":
                            case "Kiosk":
                            case "Service Station":
                            case "Pathological Lab":
                            case "Booth":
                            case "Parlor":
                            case "Bakery":
                            case "Training Institue":
                            case "Public Library":
                            case "Court House":
                            case "Call Centers":
                            case "Junk Yard":
                            case "Godowns":
                            case "Ware House":
                            case "Good Storage":
                            case "Cold Storage":
                            case "Petrol Pump":
                            case "Petrol Filling Station(With Service Bay)":
                            case "Petrol Filling Station(Without Service Bay)":
                            case "Parking Complex(Parking Lot)":
                            case "Gas Godown":
                            case "Wholesale Commercial Market":
                            case "Other Commercial Building":
                            case "Hotel":
                            case "3 Star Hotel":
                            case "4 Star Hotel":
                            case "5 Star Hotel":
                            case "Lodging":
                            case "Cinema":
                            case "Conference Hall":
                            case "Assembly Hall":
                            case "Drama Hall":
                            case "City Hall":
                            case "Town Hall":
                            case "Dance Hall":
                            case "Meeting Hall":
                            case "Lecture Hall":
                            case "Banquet Hall":
                            case "Marriage Hall":
                            case "Community Hall":
                            case "Mangal Karyalaya":
                            case "Post Office":
                            case "EB Office":
                            case "Telegraph Office":
                                {
                                    break;
                                }                   //////////////////until 3//////////////////////
                            case "Residental Bldg":
                            case "Bungalow":
                            case "Semidetached":
                            case "Row House":
                            case "Low income group and EWS Housing":
                            case "Group Housing":
                            case "Farm House":
                            case "Hostel":
                            case "Dormitory":
                            case "Boarding":
                            case "Dharamshala":
                            case "Guest House":
                            case "Staff Quarters":
                            case "Old Age HOme":
                            case "Orphanages":
                            case "Other Residental Building":
                            case "Individual Row House":
                            case "Nursing Home":
                            case "Dispensary":
                            case "Lab":
                            case "Indoor Patients Wards":
                            case "Hospital":
                            case "Special Hospital":
                            case "Private Hospital":
                            case "Govt-Semi Govt. Hospital":
                            case "Research and Training Center":
                            case "Rehabilitation Center":
                            case "Govt. Dispensary":
                            case "Maternity Home":
                            case "Health Centre":
                            case "Medical Building":
                            case "Sanatoria":
                            case "Forensic Science Laboratory":
                            case "Other Medical Building":
                            case "Educational Building":
                            case "School":
                            case "Primary School":
                            case "Nursery School":
                            case "High School":
                            case "Secondary-Higher Secondary School":
                            case "College":
                            case "Research Institution":
                            case "Educational Institution":
                            case "Railway Station":
                            case "Library":
                            case "Technical School":
                            case "Coaching Class":
                            case "Middle School":
                            case "Tutorial Centre":
                            case "Research and Development":
                            case "Other Educational Building":
                            case "Industrial Building":
                            case "Service Industry":
                            case "HouseHold Industry":
                            case "Light Industry":
                            case "Medium Industry":
                            case "Heavy Industry":
                            case "Workshop":
                            case "Industrial Laboratory":
                            case "Power Plant":
                            case "Assembly Plant":
                            case "Refinery":
                            case "Gas Plant":
                            case "Mill":
                            case "Factory":
                            case "Dairy":
                            case "Godown":
                            case "Small Scale Industries":
                            case "Other Industrial Building":
                                {
                                    break;
                                }
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = errcnt;
                        err.lyrname = "_Parking";
                        err.errcause = errcause;
                        err.objIdlist = objidlst;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_MainRoad":
                    {
                        double mroadwidth = 0, PlotArea = 0;
                        string strerrcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if (Plugin.amroadNmTxt.Count != 0)
                        {
                            string widthtxt = Commands.GetMTextContent(Plugin.amroadNmTxt.ElementAt(0));
                            mroadwidth = Commands.GetRoadWidth(widthtxt);
                            if (mroadwidth == 0)
                            {
                                strerrcause += "-MainRoad Width is worng.";
                                errcnt++;
                                objidlst.Add(Plugin.amroadNmTxt[0].ObjectId);
                            }
                        }
                        else
                        {
                            if(Plugin.amroadpline.Count!=0)
                            {
                                foreach (Polyline pl in Plugin.amroadpline)
                                {
                                    if (pl.Closed)
                                    {
                                        bool bdbintxt = false;
                                        bool binTxt = false;
                                        foreach (MText txt in Plugin.amroadNmTxt)
                                        {
                                            try
                                            {
                                                binTxt = IsPointInside(txt.Location, pl);
                                                //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                                if (txt.Contents == "")
                                                    binTxt = false;
                                                if (binTxt)
                                                    break;
                                            }
                                            catch (Exception e) { }
                                        }
                                        if (!binTxt)
                                        {
                                            foreach (DBText dBText in Plugin.aMroadDBTxt)
                                            {
                                                bdbintxt = IsPointInside(dBText.Position, pl);
                                                if (dBText.TextString == "")
                                                    bdbintxt = false;
                                                if (bdbintxt)
                                                    break;
                                            }
                                        }
                                        if (!binTxt&&!bdbintxt)
                                        {
                                            errcnt++;
                                            strerrcause += "-This polyline does not have Label.";
                                            objidlst.Add(pl.ObjectId);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                strerrcause += "-This Project has no Mainroad layer Polyline.";
                                errcnt++;
                            }
                        }
                        ruleError err_mroad = new ruleError();
                        err_mroad.errorCnt = errcnt;
                        err_mroad.lyrname = "_MainRoad";
                        err_mroad.errcause = strerrcause;
                        err_mroad.objIdlist = objidlst;
                        Commands.errlist.Add(err_mroad);
                        break;
                    }
                case "_InternalRoad":
                    {
                        double inroadwidth = 0;
                        string inrderrcause = "";
                        int inrderrcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        if (Plugin.ainterroadTxt.Count != 0)
                        {
                            string widthtxt = Commands.GetMTextContent(Plugin.amroadNmTxt.ElementAt(0));
                            inroadwidth = Commands.GetRoadWidth(widthtxt);
                            if (inroadwidth < 9.14)
                            {
                                inrderrcause += "-Internal road width is less than rule(9.14).";
                                inrderrcnt++;
                                objidlst.Add(Plugin.amroadNmTxt[0].ObjectId);
                            }
                        }
                        if(Plugin.ainterroadpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.ainterroadpline)
                            {
                                if (pl.Closed)
                                {
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.ainterroadTxt)
                                    {
                                        try
                                        {
                                            binTxt = IsPointInside(txt.Location, pl);
                                            //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                            if (txt.Contents == "")
                                                binTxt = false;
                                            if (binTxt)
                                                break;
                                        }
                                        catch (Exception e) { }
                                    }
                                    bool binDBtxt = false;
                                    if (!binTxt)
                                    {
                                        foreach (DBText txt1 in Plugin.ainterloadDBTxt)
                                        {
                                            try
                                            {
                                                binDBtxt = IsPointInside(txt1.Position, pl);
                                                if (txt1.TextString == "")
                                                    binDBtxt = false;
                                                if (binDBtxt)
                                                    break;
                                            }
                                            catch (Exception e) { }
                                        }
                                    }
                                    if (!binTxt && !binDBtxt)
                                    {
                                        inrderrcnt++;
                                        inrderrcause += "-This polyline does not have Label.";
                                        objidlst.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }                        
                        ruleError err_inrd = new ruleError();
                        err_inrd.errorCnt = inrderrcnt;
                        err_inrd.lyrname = "_InternalRoad";
                        err_inrd.errcause = inrderrcause;
                        err_inrd.objIdlist = objidlst;
                        Commands.errlist.Add(err_inrd);
                        break;
                    }
                case "_Driveway":
                    {
                        double drivewidth = 0, PlotArea = 0;
                        string driveerrcause = "";
                        int driveerrcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if (Plugin.ainterroadTxt.Count != 0)
                        {
                            string widthtxt = Commands.GetMTextContent(Plugin.amroadNmTxt.ElementAt(0));
                            drivewidth = Commands.GetRoadWidth(widthtxt);
                            if (PlotArea > 4000)
                                if (drivewidth < 4.5)
                                {
                                    driveerrcause += "-Drive way width is less than rule(4.5).";
                                    driveerrcnt++;
                                    objidlst.Add(Plugin.ainterroadTxt[0].ObjectId);
                                }
                                else
                                if (drivewidth < 3.6)
                                {
                                    driveerrcause += "-Drive way width is less than rule(3.6).";
                                    driveerrcnt++;
                                    objidlst.Add(Plugin.ainterroadTxt[0].ObjectId);
                                }
                        }
                        ruleError err_inrd = new ruleError();
                        err_inrd.errorCnt = driveerrcnt;
                        err_inrd.lyrname = "_Driveway";
                        err_inrd.errcause = driveerrcause;
                        err_inrd.objIdlist = objidlst;
                        Commands.errlist.Add(err_inrd);
                        break;
                    }
                case "_Ramp":
                    {
                        double  plinth = 0;
                        double glY = 0; string rmperrcause = "";
                        int rmperrcnt = 0;
                        Polyline  basefirstfloorpl = null;
                        DBText basefirsttxt = null;
                        Point3d tmpTop = new Point3d(0, 0, 0);
                        List<Polyline> plcellarlist = new List<Polyline>();
                        List<ObjectId> objidlst = new List<ObjectId>();
                        if (Plugin.aGllvlpline.Count != 0)
                        {
                            glY = Plugin.aGllvlpline.ElementAt(0).GetPoint3dAt(0).Y;
                        }
                        else
                        {
                            rmperrcause += "_GroundLevel layer has no entity.";
                            rmperrcnt++;
                            objidlst.Add(Plugin.azeropline[0].ObjectId);
                        }


                        foreach (DBText txt in Plugin.aFloorsTxt)
                        {
                            string strtmp = txt.TextString;
                            if (strtmp.Contains("BASEMENT FIRST") || strtmp.Contains("CELLAR"))
                            {
                                basefirsttxt = txt;
                            }
                        }
                        if (basefirsttxt == null)
                            return;
                        foreach (Polyline pl in Plugin.aFloorpline)
                        {
                            if (RectIsInPolyLine(pl, basefirsttxt.Position,
                                new Point3d(basefirsttxt.Position.X + basefirsttxt.Bounds.Value.MaxPoint.X - basefirsttxt.Bounds.Value.MinPoint.X
                                , basefirsttxt.Position.Y + basefirsttxt.Height, 0)))
                            {
                                basefirstfloorpl = pl;
                                break;
                            }
                        }
                        Point3d ptfirsttop = Commands.Gettop(basefirstfloorpl);
                        foreach (Polyline pl in Plugin.aFlrinSecpline)
                        {
                            Point3d ptbottom = Commands.Getbottom(pl);
                            if (glY > ptbottom.Y)
                                plcellarlist.Add(pl);
                        }
                        Polyline firstFlrINSeccellarpl = plcellarlist[0];
                        foreach (Polyline pl in plcellarlist)
                        {
                            if (Commands.Gettop(pl).Y > Commands.Gettop(firstFlrINSeccellarpl).Y)
                                firstFlrINSeccellarpl = pl;
                        }
                        plinth = Commands.Gettop(firstFlrINSeccellarpl).Y - glY;                        
                        //foreach (MText txt in Plugin.arampTxt)
                        //{
                        //    Point3d ptstart = txt.Location;
                        //    Point3d ptend = new Point3d(txt.Location.X + txt.Width , txt.Location.Y + txt.Height , 0);
                        //    string str = txt.Contents;
                        //    str = str.ToLower();
                        //    int lengthpos = str.IndexOf(" mt. l ");
                        //    string strlength = str.Substring(0, lengthpos);
                        //    rmplength = Convert.ToDouble(strlength);
                        //    str = str.Remove(0, lengthpos + 10);
                        //    int widehpos = str.IndexOf(" mt. h ");
                        //    string strwide = str.Substring(0, widehpos);
                        //    rmpht = Convert.ToDouble(strwide);
                        //    str = str.Remove(0, widehpos + 10);
                        //    int htpos = str.IndexOf(" mt. w ");
                        //    string strht = str.Substring(0, htpos);
                        //    rmpwidth = Convert.ToDouble(strht);
                        //    if (RectIsInPolyLine(basefirstfloorpl, ptstart, ptend))
                        //    {
                        //        double cellarht = Commands.Gettop(firstFlrINSeccellarpl).Y - Commands.Getbottom(firstFlrINSeccellarpl).Y;
                        //        if (rmplength >= (cellarht - plinth) * 8)
                        //        {
                        //            rmperrcause += "-Ramp Length is out of rule.";
                        //            rmperrcnt++;
                        //            objidlst.Add(txt.ObjectId);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        double cellarht = Commands.Gettop(firstFlrINSeccellarpl).Y - Commands.Getbottom(firstFlrINSeccellarpl).Y;
                        //        if (rmplength >= (cellarht) * 8)
                        //        {
                        //            rmperrcause += "-Ramp is out of rule.";
                        //            rmperrcnt++;
                        //            objidlst.Add(txt.ObjectId);
                        //        }
                        //    }
                        //}
                        foreach (Polyline pl in Plugin.arampline)
                        {
                            if(pl.Closed)
                            {
                                bool bdbintxt = false;
                                bool binTxt = false;
                                foreach (MText txt in Plugin.arampTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aRampDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    rmperrcnt++;
                                    rmperrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }                            
                        }
                        List<Polyline> plclosedlist = new List<Polyline>();
                        List<Polyline> plcenterlist = new List<Polyline>();
                        foreach (Polyline pl in Plugin.arampline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plcenterlist.Add(pl);
                            }
                            if (pl.Closed)
                                plclosedlist.Add(pl);
                        }

                        foreach (Polyline plclo in plclosedlist)
                        {
                            bool bcenter = false;
                            foreach (Polyline plcen in plcenterlist)
                            {
                                if (PolyIsInPolyLine(plclo, plcen))
                                {
                                    bcenter = true;
                                }
                            }
                            if (!bcenter)
                            {
                                rmperrcause += "-This polyline has no centerline.";
                                rmperrcnt++;
                                objidlst.Add(plclo.ObjectId);
                            }
                        }
                        ruleError err_inrd = new ruleError();
                        err_inrd.errorCnt = rmperrcnt;
                        err_inrd.lyrname = "_Ramp";
                        err_inrd.errcause = rmperrcause;
                        err_inrd.objIdlist = objidlst;
                        Commands.errlist.Add(err_inrd);
                        break;
                    }
                case "_StairCase":
                    {
                        if (Plugin.subuse == "Petrol Pump")
                            break;
                        if (Plugin.usestate == 2)
                            break;
                        double PlotArea = 0;
                        string stairerrcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if(Plugin.aStairpline.Count==0)
                        {
                            errcnt++;
                            stairerrcause += "-Staircase must be required.";
                        }else
                        {
                            foreach (Polyline pl in Plugin.aStairpline)
                            {
                                Point3d ptleftplt = Commands.Getleft(pl);
                                Point3d ptrightplt = Commands.Getright(pl);
                                Point3d pttopplt = Commands.Gettop(pl);
                                Point3d ptbottomplt = Commands.Getbottom(pl);
                                double width1 = Math.Abs(ptleftplt.X - ptrightplt.X);
                                double width2 = Math.Abs(pttopplt.Y - ptbottomplt.Y);
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    if (PlotArea < 300)
                                    {
                                        if (width1 < 2 && width2 < 2)
                                        {
                                            stairerrcause += " -Staircase Width is out of rule, will be 2";
                                            errcnt++;
                                            objidlst.Add(pl.ObjectId);
                                        }
                                    }
                                    if (300 <= PlotArea && PlotArea <= 4000)
                                    {
                                        if (width1 < 2.5 && width2 < 2.5)
                                        {
                                            stairerrcause += " -Staircase Width is out of rule, will be 2.5";
                                            errcnt++;
                                            objidlst.Add(pl.ObjectId);
                                        }
                                    }
                                    if (PlotArea > 4000)
                                    {
                                        if (width1 < 3 && width2 < 3)
                                        {
                                            stairerrcause += " -Staircase Width is out of rule, will be 3";
                                            errcnt++;
                                            objidlst.Add(pl.ObjectId);
                                        }
                                    }
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.aStairTxt)
                                    {
                                        try
                                        {
                                            binTxt = IsPointInside(txt.Location, pl);
                                            //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                            if (txt.Contents == "")
                                                binTxt = false;
                                            if (binTxt)
                                                break;
                                        }
                                        catch (Exception e) {  }
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.aStairDBText)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt && !bdbintxt)
                                    {
                                        errcnt++;
                                        stairerrcause += "-This polyline does not have Label.";
                                        objidlst.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }
                        ruleError err_stair = new ruleError();
                        err_stair.errorCnt = errcnt;
                        err_stair.lyrname = "_StairCase";
                        err_stair.errcause = stairerrcause;
                        err_stair.objIdlist = objidlst;
                        Commands.errlist.Add(err_stair);
                        break;
                    }
                case "_Passage":
                    {
                        double PlotArea = 0;
                        string Passageerrcause = "";
                        int errcnt = 0;

                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        //foreach (Polyline pl in Plugin.aPassagepline)
                        //{                            
                        //    if(pl.Closed)
                        //    {
                        //        Point3d ptleftplt = Commands.Getleft(pl);
                        //        Point3d ptrightplt = Commands.Getright(pl);
                        //        Point3d pttopplt = Commands.Gettop(pl);
                        //        Point3d ptbottomplt = Commands.Getbottom(pl);
                        //        double width1 = Math.Abs(ptleftplt.X - ptrightplt.X);
                        //        double width2 = Math.Abs(pttopplt.Y - ptbottomplt.Y);
                        //        if (PlotArea <= 300)
                        //        {
                        //            if (width1 != 2 && width2 != 2)
                        //            {
                        //                Passageerrcause += "-Passage Width is out of rule";
                        //                errcnt++;
                        //                objidlst.Add(pl.ObjectId);
                        //            }
                        //        }else if (300 <= PlotArea || PlotArea <= 4000)
                        //        {
                        //            if (width1 != 2.5 && width2 != 2.5)
                        //            {
                        //                Passageerrcause += "-Passage Width is out of rule";
                        //                errcnt++;
                        //                objidlst.Add(pl.ObjectId);
                        //            }
                        //        }else if (PlotArea >= 4000)
                        //        {
                        //            if (width1 != 3 && width2 != 3)
                        //            {
                        //                Passageerrcause += "-Passage Width is out of rule";
                        //                errcnt++;
                        //                objidlst.Add(pl.ObjectId);
                        //            }
                        //        }
                        //    }                            
                        //}
                        foreach (Polyline pl1 in Plugin.aPassagepline)
                        {
                            if (pl1.Closed)
                            {
                                bool bdbintxt = false;
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aPassageTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl1);
                                        //binTxt = RectIsInPolyLine(pl1, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch(Exception e) {  }
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aPassageDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl1);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    Passageerrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl1.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pl1, pline) && (pline.ObjectId != pl1.ObjectId)
                                                        && (pline.Layer != pl1.Layer) && (pline.Layer != "_Door") && (pline.Layer != "_StairCase")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        errcnt++;
                                                        Passageerrcause += str;
                                                        objidlst.Add(pl1.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        List<Polyline> plclosedlist = new List<Polyline>();
                        List<Polyline> plcenterlist = new List<Polyline>();
                        foreach (Polyline pl2 in Plugin.aPassagepline)
                        {
                            if (!pl2.Closed && pl2.Linetype == "CENTER")
                            {
                                plcenterlist.Add(pl2);
                            }
                            if (pl2.Closed)
                                plclosedlist.Add(pl2);
                        }

                        foreach (Polyline plclo in plclosedlist)
                        {
                            bool bcenter = false;
                            foreach (Polyline plcen in plcenterlist)
                            {
                                if (PolyIsInPolyLine(plclo, plcen))
                                {
                                    bcenter = true;
                                }
                            }
                            if (!bcenter)
                            {
                                Passageerrcause += "-This polyline has no centerline.";
                                errcnt++;
                                objidlst.Add(plclo.ObjectId);
                            }
                        }
                        ruleError err_passage = new ruleError();
                        err_passage.errorCnt = errcnt;
                        err_passage.lyrname = "_Passage";
                        err_passage.errcause = Passageerrcause;
                        err_passage.objIdlist = objidlst;
                        Commands.errlist.Add(err_passage);
                        break;
                    }
                case "_CompoundWall":
                    {
                        string Passageerrcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach(Polyline pl in Plugin.aCompndwllpline)
                        {
                            bool bdbintxt = false;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aCmpWallTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.acmpndWallDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    Passageerrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }

                        List<Polyline> plclosedlist = new List<Polyline>();
                        List<Polyline> plcenterlist = new List<Polyline>();
                        foreach (Polyline pl in Plugin.aCompndwllpline)
                        {
                            if (!pl.Closed && pl.Linetype == "CENTER")
                            {
                                plcenterlist.Add(pl);
                            }
                            if (pl.Closed)
                                plclosedlist.Add(pl);
                        }

                        foreach (Polyline plclo in plclosedlist)
                        {
                            bool bcenter = false;
                            foreach (Polyline plcen in plcenterlist)
                            {
                                if (PolyIsInPolyLine(plclo, plcen))
                                {
                                    bcenter = true;
                                }
                            }
                            if (!bcenter)
                            {
                                Passageerrcause += "-This polyline has no centerline.";
                                errcnt++;
                                objidlst.Add(plclo.ObjectId);
                            }
                        }
                        ruleError err_passage = new ruleError();
                        err_passage.errorCnt = errcnt;
                        err_passage.lyrname = "_CompoundWall";
                        err_passage.errcause = Passageerrcause;
                        err_passage.objIdlist = objidlst;
                        Commands.errlist.Add(err_passage);
                        break;
                    }
                case "_Room":
                    {
                        int windowerrcntrm = 0;
                        string roomerrcause = "";
                        Point3d topPt = new Point3d(0, Double.MinValue, 0);
                        Point3d bottomPt = new Point3d(0, 0, 0);
                        List<ObjectId> objidlst = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            bool bopen = false;
                            foreach (windowrule wr in Commands.awindowrule)
                            {
                                for (int i = 0; i < wr.pl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = wr.pl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        double area = pln.Area;
                                        if (area > (wr.width * wr.height * 10))
                                        {
                                            windowerrcntrm++;
                                            Commands.roomerrcause.Add("-This room does not satisfy ventilation requirement.");
                                            roomerrcause = roomerrcause + "-" + "This room does not satisfy ventilation requirement.";
                                            objidlst.Add(pln.ObjectId);
                                            break;
                                        }
                                    }
                                }
                                foreach (Polyline pl in Plugin.adoorpline)
                                {
                                    for (int i = 0; i < pl.NumberOfVertices; i++)
                                    {
                                        Point3d pt3 = pl.GetPoint3dAt(i);
                                        if (IsPointOnPolyline(pln, pt3))
                                        {
                                            bopen = true;
                                            break;
                                        }
                                    }
                                }
                                if (!bopen)
                                {
                                    bool bintxt = false;
                                    bool bfoyer = false;
                                    foreach (MText inst in Plugin.aroomNmTxt)
                                    {
                                        bintxt = IsPointInside(inst.Location, pln);
                                        //bintxt = RectangleIsInPolyline(pln, inst.Location, new Point3d(inst.Location.X+inst.Width,inst.Location.Y+inst.Height,0));

                                        if (bintxt)
                                        {
                                            string strtmp = inst.Contents;
                                            if (strtmp.Contains("Foyer"))
                                            {
                                                bfoyer = true;
                                                break;
                                            }
                                        }
                                            
                                    }
                                    if(!bfoyer)
                                    {
                                        windowerrcntrm++;
                                        roomerrcause += "-This room is not open with door.";
                                        objidlst.Add(pln.ObjectId);
                                    }
                                }
                            }
                            if(pln.Closed)
                            {
                                bool bIntext = false;
                                bool bdbintxt = false;
                                foreach (MText inst in Plugin.aroomNmTxt)
                                {
                                    bIntext = RectangleIsInPolyline(pln, inst.GeometricExtents.MinPoint, inst.GeometricExtents.MaxPoint);
                                    if (inst.Contents == "")
                                        bIntext = false;
                                    if (bIntext)
                                        break;
                                }
                                if (!bIntext)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aRoomDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pln);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!bIntext&&!bdbintxt)
                                {
                                    windowerrcntrm++;
                                    roomerrcause += "-This room does not contain roomname or roomname Text range is out of RoomPolyline.";
                                    objidlst.Add(pln.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pln, pline) && (pline.ObjectId != pln.ObjectId)
                                                        && (pline.Layer != pln.Layer) && (pline.Layer != "_Door")
                                                        && (pline.Layer != "_Window") && (pline.Layer != "_MortgageArea")
                                                         && (pline.Layer != "_CarpetArea")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        windowerrcntrm++;
                                                        roomerrcause += str;
                                                        objidlst.Add(pln.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        MText terracetxt = null;
                        DBText terractdbtxt = null;
                        foreach (MText txt in Plugin.aFlrinSecTxt)
                        {
                            string str = txt.Contents.ToUpper();
                            if (str.Contains("TERRACE"))
                            {
                                terracetxt = txt;
                            }
                        }
                        if (terracetxt == null)
                        {
                            foreach (DBText txt in Plugin.aFlrinSecSTxt)
                            {
                                string str = txt.TextString.ToUpper();
                                if (str.Contains("TERRACE"))
                                {
                                    terractdbtxt = txt;
                                }
                            }
                        }
                        //if (Plugin.aFlrinSecTxt.Count == 0 && Plugin.aFlrinSecSTxt.Count == 0)
                        //{
                        //    roomerrcause += "-Current FloorInSection Layer has no FloorName MTEXT or TEXT.Please reassign FloorName.";
                        //    windowerrcntrm++;
                        //    objidlst.Add(Plugin.aplotpline[0].ObjectId);
                        //}
                        foreach (Polyline pl in Plugin.aFlrinSecpline)
                        {
                            if (terracetxt != null)
                            {
                                if (RectangleIsInPolyline(pl, terracetxt.GeometricExtents.MinPoint, terracetxt.GeometricExtents.MaxPoint))
                                {
                                    topPt = Commands.Getbottom(pl);
                                    //for (int i = 0; i < pl.NumberOfVertices; i++)
                                    //{
                                    //    if (topPt.Y < pl.GetPoint3dAt(i).Y)
                                    //        topPt = pl.GetPoint3dAt(i);
                                    //}
                                    break;
                                }
                            }
                            else if(terractdbtxt!=null)
                            {
                                if (RectangleIsInPolyline(pl, terractdbtxt.GeometricExtents.MinPoint, terractdbtxt.GeometricExtents.MaxPoint))
                                {
                                    topPt = Commands.Getbottom(pl);
                                    break;
                                }
                            }
                        }
                        foreach (Polyline pl in Plugin.aGllvlpline)
                        {
                            for (int i = 0; i < pl.NumberOfVertices; i++)
                            {
                                if (bottomPt.Y > pl.GetPoint3dAt(i).Y)
                                    bottomPt = pl.GetPoint3dAt(i);
                            }
                        }
                        foreach (Polyline pl in Plugin.aFlrinSecpline)
                        {
                            if (Commands.Getbottom(pl).Y >= bottomPt.Y && Commands.Gettop(pl).Y <= topPt.Y)
                            {
                                //if(Plugin.usestate==0)
                                //{
                                //    if((Commands.Gettop(pl).Y- Commands.Getbottom(pl).Y)<2.75)
                                //    {
                                //        objidlst.Add(pl.ObjectId);
                                //        roomerrcause = roomerrcause + "-" + "This floor does not satisfy Height requirement(2.75m).";
                                //        windowerrcntrm++;
                                //    }
                                //}
                                //if (Plugin.usestate == 6)
                                //{
                                //    if ((Commands.Gettop(pl).Y - Commands.Getbottom(pl).Y) < 3.6)
                                //    {
                                //        objidlst.Add(pl.ObjectId);
                                //        roomerrcause = roomerrcause + "-" + "This floor does not satisfy Height requirement(3.6m).";
                                //        windowerrcntrm++;
                                //    }
                                //}
                            }
                        }
                        ruleError errrm = new ruleError();
                        errrm.errorCnt = windowerrcntrm;
                        errrm.lyrname = "_Room";
                        errrm.errcause = roomerrcause;
                        errrm.objIdlist = objidlst;
                        Commands.errlist.Add(errrm);
                        break;
                    }
                case "_Door":
                    {
                        int doorerrcnt = 0;
                        string doorerrcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        bool isdoor = false;
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            isdoor = false;
                            foreach (Polyline dpl in Plugin.adoorpline)
                            {
                                for (int i = 0; i < dpl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = dpl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        isdoor = true;
                                        break;
                                    }
                                }
                                if (isdoor)
                                    break;
                            }
                            if (!isdoor)
                            {
                                doorerrcnt++;
                                doorerrcause += "-This Room does not have a Door.";
                                objidlst.Add(pln.ObjectId);
                            }
                        }
                        foreach(Polyline pldr in Plugin.adoorpline)
                        {
                            if (pldr.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.adoorNmTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pldr);
                                    //binTxt = RectangleIsInPolyline(pldr, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aDoorDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pldr);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    doorerrcnt++;
                                    doorerrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pldr.ObjectId);
                                }
                            }
                        }                   
                        ruleError err = new ruleError();
                        err.errorCnt = doorerrcnt;
                        err.lyrname = "_Door";
                        err.errcause = doorerrcause;
                        err.objIdlist = objidlst;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Splay":
                    {
                        List<ObjectId> objidlist = new List<ObjectId>();
                        bool splayonplot = false;
                        //bool isplotinter = false, isinterinter = false;
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<Point3d> interinterPtlst = new List<Point3d>();
                        foreach (Polyline plinter1 in Plugin.ainterroadpline)
                        {
                            if (plinter1.Closed)
                            {
                                foreach (Polyline plin in Plugin.ainterroadpline)
                                {
                                    //isinterinter = false;
                                    if (plin.Closed && plinter1 != plin && plinter1.Closed)
                                    {
                                        for (int i = 0; i < plin.NumberOfVertices; i++)
                                        {
                                            Point3d ptin = plin.GetPoint3dAt(i);
                                            if (IsPointOnPolyline(plinter1, ptin))
                                            {
                                                interinterPtlst.Add(ptin);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        List<Point3d> interinterPtlst1 = new List<Point3d>(interinterPtlst);
                        foreach (Polyline plsply in Plugin.asplaypline)
                        {
                            for (int i = 0; i < plsply.NumberOfVertices; i++)
                            {
                                Point3d ptsptmp = plsply.GetPoint3dAt(i);
                                foreach (Point3d ptint in interinterPtlst)
                                {
                                    if (IsSamePoint(ptsptmp, ptint))
                                        interinterPtlst1.Remove(ptint);
                                }
                            }
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.asplayTxt)
                                {   
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, plsply);
                                        //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch (Exception e) {  }
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    double width1 = 0;
                                    double height1 = 0;
                                    foreach (DBText txt1 in Plugin.asplyDBTxt)
                                    {
                                        width1 = txt1.GeometricExtents.MaxPoint.X - txt1.GeometricExtents.MinPoint.X;
                                        height1 = txt1.GeometricExtents.MaxPoint.Y - txt1.GeometricExtents.MinPoint.Y;
                                        try
                                        {
                                            binDBtxt = IsPointInside(txt1.Position, plsply);
                                            if (txt1.TextString == "")
                                                binDBtxt = false;
                                            if (binDBtxt)
                                                break;
                                        }catch(Exception e) { }
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }

                        foreach (Point3d pt in interinterPtlst)
                        {
                            foreach (Polyline pl in Plugin.aopenspacepline)
                            {
                                if (IsPointOnPolyline(pl, pt))
                                    interinterPtlst1.Remove(pt);
                            }
                            foreach (Polyline pl in Plugin.aAmenitypline)
                                if (IsPointOnPolyline(pl, pt))
                                    interinterPtlst1.Remove(pt);
                        }
                        foreach (Point3d pttmp in interinterPtlst1)
                        {
                            foreach (Polyline pl in Plugin.ainterroadpline)
                            {
                                if (IsPointOnPolyline(pl, pttmp))
                                {
                                    bool isalready = false;
                                    foreach (ObjectId id in objidlist)
                                    {
                                        if (id == pl.ObjectId)
                                        {
                                            isalready = true;
                                            break;
                                        }
                                    }
                                    if (!isalready)
                                    {
                                        splayerrcnt++;
                                        splayerrcause += "-This Road should has Splay.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }
                        if (Plugin.aplotpline.Count == 0)
                        {
                            splayerrcause += "This drawing has no entity in Plot layer.";
                            splayerrcnt++;
                            objidlist.Add(Plugin.asplaypline[0].ObjectId);
                        }
                        else
                        {
                            foreach (Polyline pl in Plugin.asplaypline)
                            {
                                Polyline plplt = Plugin.aplotpline[0];
                                splayonplot = false;
                                //istchsplay_indiv = false;
                                //istchsplay_mrd = false;
                                //istchsplay_intrd = false;
                                Point3d ptleft = Commands.Getleft(pl);
                                Point3d ptright = Commands.Getright(pl);
                                Point3d pttop = Commands.Gettop(pl);
                                Point3d ptbottom = Commands.Getbottom(pl);
                                splayonplot = PolyIsInPolyLine(plplt, pl);
                                if (!splayonplot)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-Some Splay entities are not in Plot Area.";
                                    objidlist.Add(pl.ObjectId);
                                }
                                double width = ptleft.X - ptright.X;
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Splay";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_VentilationShaft":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aVenShaftpline)
                        {
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aVenShaftTxt)
                                {try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch(Exception e) { }
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aVenShaftDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        //double width1 = 0, width2 = 0;
                        //
                        //foreach (Polyline pl in Plugin.aVenShaftpline)
                        //{
                        //    Point3d ptleft = Commands.Getleft(pl);
                        //    Point3d ptright = Commands.Getright(pl);
                        //    Point3d pttop = Commands.Gettop(pl);
                        //    Point3d ptbottom = Commands.Getbottom(pl);
                        //    width1 = Math.Abs(ptleft.X - ptright.X);
                        //    width2 = Math.Abs(pttop.Y - ptbottom.Y);
                        //    if (pl.Area < 9)
                        //        if (width1 < 2 || width2 < 2)
                        //        {
                        //            errcnt++;
                        //            errcause += "-Ventilation side width is smaller than 2.";
                        //            objidlst.Add(pl.ObjectId);
                        //        }
                        //    if (pl.Area > 9 && pl.Area < 25)
                        //        if (width1 < 3 || width2 < 3)
                        //        {
                        //            errcnt++;
                        //            errcause += "-Ventilation side width is smaller than 3.";
                        //            objidlst.Add(pl.ObjectId);
                        //        }    
                        //}
                        //foreach (ObjectId id in objidlst)
                        //    SetViewCenterToObject(id);
                        ruleError err = new ruleError();
                        err.errorCnt = errcnt;
                        err.lyrname = "_VentilationShaft";
                        err.errcause = errcause;
                        err.objIdlist = objidlst;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Amenity":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aAmenitypline)
                        {
                            if(pl.Closed)
                            {
                                string startx="", starty="", endx="", endy="";
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aAmenityTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width/2, txt.Location.Y + txt.Height/2, 0));
                                    startx = Math.Round(txt.Location.X, 0).ToString();
                                    starty = Math.Round(txt.Location.Y, 0).ToString();
                                    endx = Math.Round(txt.Location.X + txt.Width, 0).ToString();
                                    endy = Math.Round(txt.Location.Y + txt.Height, 0).ToString();
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aAmenDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        //if (Plugin.aAmenitypline.Count != 0)
                        //{
                        //    foreach (Polyline pl in Plugin.aplotpline)
                        //        plotarea += pl.Area;
                        //    foreach (Polyline pl in Plugin.aAmenitypline)
                        //    {
                        //        amenityarea += pl.Area;
                        //        objidlst.Add(pl.ObjectId);
                        //    }
                        //    if (plotarea < 20235 && 0 < plotarea)
                        //        if (amenityarea < plotarea * 0.03)
                        //        {
                        //            errcnt++;
                        //            errcause += "-Amenity Area is too small";
                        //        }
                        //    if (plotarea >= 20235)
                        //        if (amenityarea < plotarea * 0.05)
                        //        {
                        //            errcnt++;
                        //            errcause += "-Amenity Area is too small";
                        //        }
                        ruleError err = new ruleError();
                        err.errorCnt = errcnt;
                        err.lyrname = "_Amenity";
                        err.errcause = errcause;
                        err.objIdlist = objidlst;
                        Commands.errlist.Add(err);
                        //}
                        break;
                    }
                case "_BufferZone":
                    {
                        bool istchbuffer = false;
                        string buferrcause = "";
                        int buferrcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aBufferpline)
                        {
                            if (Plugin.aWaterBodypline.Count != 0)
                            {
                                istchbuffer = false;
                                foreach (Polyline plwbody in Plugin.aWaterBodypline)
                                {
                                    istchbuffer = checkTwoPlineTouch(plwbody, pl);
                                    if (istchbuffer)
                                    {
                                        Point3d ptleft = Commands.Getleft(pl);
                                        Point3d ptright = Commands.Getright(pl);
                                        Point3d pttop = Commands.Gettop(pl);
                                        Point3d ptbottom = Commands.Getbottom(pl);
                                        double width = ptright.X - ptleft.X;
                                        double height = pttop.Y - ptbottom.Y;
                                        if (height != 2)
                                        {
                                            buferrcause += "-BufferZone thickness is not 2.0mts.";
                                            buferrcnt++;
                                            objidlst.Add(plwbody.ObjectId);
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "-BufferZone is not closed with WaterBodies.";
                                    buferrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (Plugin.aElectricpline.Count != 0)
                            {
                                istchbuffer = false;
                                foreach (Polyline pleline in Plugin.aElectricpline)
                                {
                                    istchbuffer = checkTwoPlineTouch(pleline, pl);
                                    if (istchbuffer)
                                    {
                                        Point3d ptleft = Commands.Getleft(pl);
                                        Point3d ptright = Commands.Getright(pl);
                                        Point3d pttop = Commands.Gettop(pl);
                                        Point3d ptbottom = Commands.Getbottom(pl);
                                        double width = ptright.X - ptleft.X;
                                        double height = pttop.Y - ptbottom.Y;
                                        if (height != 10)
                                        {
                                            buferrcause += "-BufferZone thickness is not 10.0mts.";
                                            buferrcnt++;
                                            objidlst.Add(pleline.ObjectId);
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "-BufferZone is not closed with ElectricLine.";
                                    buferrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (Plugin.aWaterlinepline.Count != 0)
                            {
                                istchbuffer = false;
                                foreach (Polyline pwline in Plugin.aWaterlinepline)
                                {
                                    istchbuffer = checkTwoPlineTouch(pwline, pl);
                                    if (istchbuffer)
                                    {
                                        Point3d ptleft = Commands.Getleft(pl);
                                        Point3d ptright = Commands.Getright(pl);
                                        Point3d pttop = Commands.Gettop(pl);
                                        Point3d ptbottom = Commands.Getbottom(pl);
                                        double width = ptright.X - ptleft.X;
                                        double height = pttop.Y - ptbottom.Y;
                                        if (height != 10)
                                        {
                                            buferrcause += "-BufferZone thickness is not 2.0mts.";
                                            buferrcnt++;
                                            objidlst.Add(pwline.ObjectId);
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "-BufferZone is not closed with ElectricLine.";
                                    buferrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if(pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aBufferTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aBufferDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt&&!bdbintxt)
                                {
                                    buferrcnt++;
                                    buferrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errbuf = new ruleError();
                        errbuf.errorCnt = buferrcnt;
                        errbuf.lyrname = "_BufferZone";
                        errbuf.errcause = buferrcause;
                        errbuf.objIdlist = objidlst;
                        Commands.errlist.Add(errbuf);
                        break;
                    }
                case "_MortgageArea":
                    {
                        if (Plugin.subuse == "Petrol Pump")
                            break;
                        string errcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        double PlotArea = 0;
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if (Plugin.usestate==1)
                        {
                            if(Plugin.aMortgageAreapline.Count==0)
                            {
                                errcause += "-MortgageArea must have at least one entity.";
                                errcnt++;
                            }
                        }
                        if (Plugin.usestate == 0)
                        {
                            if (Plugin.aMortgageAreapline.Count == 0&&PlotArea<300)
                            {
                                errcause += "-MortgageArea must have at least one entity.";
                                errcnt++;
                            }
                        }
                        foreach (Polyline pl in Plugin.aMortgageAreapline)
                        {
                            if (pl.Closed)
                            {
                                string startx = "", starty = "", endx = "", endy = "";
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aMortgageAreaTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, pl);
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    startx = Math.Round(txt.Location.X, 0).ToString();
                                    starty = Math.Round(txt.Location.Y, 0).ToString();
                                    endx = Math.Round(txt.Location.X + txt.Width, 0).ToString();
                                    endy = Math.Round(txt.Location.Y + txt.Height, 0).ToString();
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    double width1 = 0, height1 = 0;
                                    foreach (DBText dBText in Plugin.aMortgageDBTxt)
                                    {
                                        width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                        height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = errcnt;
                        err.lyrname = "_MortgageArea";
                        err.errcause = errcause;
                        err.objIdlist = objidlst;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_PrintAdditionalDetail":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        string wdfinfstr = Commands.MakingWind_DoorList();
                        Point3d ptdetail = new Point3d(Commands.Getright(Plugin.aprintaddpline[0]).X,
                            Commands.Getbottom(Plugin.aprintaddpline[0]).Y, 0);
                        Commands.MakeWind_DoorText(wdfinfstr, ptdetail);
                        List<ObjectId> objidlist = new List<ObjectId>();
                        if(Plugin.aprintaddpline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.aprintaddpline)
                            {
                                Plugin.LeftOwnerArea += pl.Area;
                                if (pl.Closed)
                                {
                                    bool binTxt = false;
                                    bool bdbintxt = false;
                                    foreach (MText txt in Plugin.aprintaddTxt)
                                    {
                                        try
                                        {
                                            binTxt = IsPointInside(txt.Location, pl);
                                            //binTxt = RectangleIsInPolyline(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                            if (txt.Contents == "")
                                                binTxt = false;
                                            if (binTxt)
                                                break;
                                        }
                                        catch (Exception e) { }
                                    }
                                    if (!binTxt)
                                    {
                                        double width1 = 0, height1 = 0;
                                        foreach (DBText dBText in Plugin.aPrintaddtionDBTxt)
                                        {
                                            width1 = dBText.GeometricExtents.MaxPoint.X - dBText.GeometricExtents.MinPoint.X;
                                            height1 = dBText.GeometricExtents.MaxPoint.Y - dBText.GeometricExtents.MinPoint.Y;
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt&&!bdbintxt)
                                    {
                                        errcnt++;
                                        errcause += "-This polyline does not have Label.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                            //bool northblock = false;
                            //Document doc = Application.DocumentManager.MdiActiveDocument;
                            //Database db = doc.Database;
                            //Editor ed1 = doc.Editor;
                            //using (Transaction tr = db.TransactionManager.StartTransaction())
                            //{
                            //    // open the block table which contains all the BlockTableRecords (block definitions and spaces)
                            //    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                            //    // open the model space BlockTableRecord
                            //    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                            //    // iterate through the model space 
                            //    foreach (ObjectId id in modelSpace)
                            //    {
                            //        // check if the current ObjectId is a block reference one
                            //        if (id.ObjectClass.DxfName == "INSERT")
                            //        {
                            //            // open the block reference
                            //            var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);

                            //            // print the block name to the command line
                            //            ed1.WriteMessage("\n" + blockReference.Name);
                            //            if (blockReference.Name == "NorthDirection_PreVal")
                            //            {
                            //                northblock = true;
                            //                break;
                            //            }
                            //            else
                            //            {
                            //                if (blockReference.Name == "North_PreDCR")
                            //                {
                            //                    objidlist.Add(blockReference.ObjectId);
                            //                    errcnt++;
                            //                    errcause += "-This block reference is not Preval made North Direction Block reference.";
                            //                    northblock = true;
                            //                }
                            //            }
                            //        }
                            //    }
                            //    tr.Commit();
                            //}
                            //if (!northblock)
                            //{
                            //    errcnt++;
                            //    errcause += "-This layer has no Preval made North Direction Block reference.";
                            //    objidlist.Add(Plugin.aprintaddpline[0].ObjectId);
                            //}
                            ruleError errelec = new ruleError();
                            errelec.errorCnt = errcnt;
                            errelec.lyrname = "_PrintAdditionalDetail";
                            errelec.errcause = errcause;
                            errelec.objIdlist = objidlist;
                            Commands.errlist.Add(errelec);
                        }
                        else
                        {
                            errcnt++;
                            errcause += "-This Project has no _PrintAdditionalDetail layer polyline.";
                            ruleError errelec = new ruleError();
                            errelec.errorCnt = errcnt;
                            errelec.lyrname = "_PrintAdditionalDetail";
                            errelec.errcause = errcause;
                            errelec.objIdlist = objidlist;
                            Commands.errlist.Add(errelec);
                        }
                        break;
                    }
                case "_RoadWidening":
                    {
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline pl in Plugin.aRdWidepline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                            if (pl.Closed)
                            {
                                bool binTxt = false;
                                bool bdbintxt = false;
                                foreach (MText txt in Plugin.aRdWideTxt)
                                {
                                    //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    binTxt = IsPointInside(txt.Location, pl);
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aRdWideDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    errcause += "-This polyline does not have Label.";
                                    objidlist.Add(pl.ObjectId);
                                }
                            }
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
                                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                            if (id.ObjectClass == PlineCls)
                                            {
                                                var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                if (pline.Layer == "0")
                                                    continue;
                                                if (PolyIsInPolyLine(pl, pline) && (pline.ObjectId != pl.ObjectId)
                                                    && (pline.Layer != pl.Layer))
                                                {
                                                    string str = "-This polyline has " + pl.Layer.ToString() + " layer  Object.";
                                                    errcnt++;
                                                    errcause += str;
                                                    objidlist.Add(pl.ObjectId);
                                                }
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                        }
                        ruleError errelec = new ruleError();
                        errelec.errorCnt = errcnt;
                        errelec.lyrname = "_RoadWidening";
                        errelec.errcause = errcause;
                        errelec.objIdlist = objidlist;
                        Commands.errlist.Add(errelec);
                        break;
                    }
                case "_Lift":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aLiftpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aLiftTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, plsply);
                                        //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }catch(Exception e) { }
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    double width1 = 0;
                                    double height1 = 0;
                                    foreach (DBText txt1 in Plugin.aLiftDBTxt)
                                    {
                                        width1 = txt1.GeometricExtents.MaxPoint.X - txt1.GeometricExtents.MinPoint.X;
                                        height1 = txt1.GeometricExtents.MaxPoint.Y - txt1.GeometricExtents.MinPoint.Y;
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Lift";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_Terrace":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        foreach (Polyline plsply in Plugin.aTerracepline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aTerraceTxt)
                                {try
                                    {
                                        binTxt = IsPointInside(txt.Location, plsply);
                                        //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch (Exception e) { }
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    double width1 = 0;
                                    double height1 = 0;
                                    foreach (DBText txt1 in Plugin.aTerraceDBTxt)
                                    {
                                        width1 = txt1.GeometricExtents.MaxPoint.X - txt1.GeometricExtents.MinPoint.X;
                                        height1 = txt1.GeometricExtents.MaxPoint.Y - txt1.GeometricExtents.MinPoint.Y;
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(plsply, pline) && (pline.ObjectId != plsply.ObjectId)
                                                        && (pline.Layer != plsply.Layer) && (pline.Layer != "_StairCase")
                                                        && (pline.Layer != "_Lift") && (pline.Layer != "_AccessoryUse") && (pline.Layer != "_SlabCutoutVoid")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        splayerrcnt++;
                                                        splayerrcause += str;
                                                        objidlist.Add(plsply.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Terrace";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_SitePlan":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aSitePlanplilne)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aSitePlanpTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aSitePlanpDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_SitePlan";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_BuildingName":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.abuildingNmpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.abldingNmTxt)
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aBuildNameDBTxt)
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_BuildingName";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_ProposedWork":
                    {
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline plsply in Plugin.aprpwrkpline)
                        {
                            if (plsply.Closed)
                            {
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aprpWrkTxt) 
                                {
                                    binTxt = IsPointInside(txt.Location, plsply);
                                    //binTxt = RectIsInPolyLine(plsply, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                    if (txt.Contents == "")
                                        binTxt = false;
                                    if (binTxt)
                                        break;
                                }
                                bool binDBtxt = false;
                                if (!binTxt)
                                {
                                    foreach (DBText txt1 in Plugin.aPrpWrkDBTxt)    
                                    {
                                        binDBtxt = IsPointInside(txt1.Position, plsply);
                                        if (txt1.TextString == "")
                                            binDBtxt = false;
                                        if (binDBtxt)
                                            break;
                                    }
                                }
                                //Application.ShowAlertDialog(binTxt + ":" + binDBtxt);
                                if (!binTxt && !binDBtxt)
                                {
                                    splayerrcnt++;
                                    splayerrcause += "-This polyline does not have Label.";
                                    objidlist.Add(plsply.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(plsply, pline) && (pline.ObjectId != plsply.ObjectId)
                                                        && (pline.Layer != plsply.Layer) && (pline.Layer != "_BuildingName")
                                                        && (pline.Layer != "_Lift") && (pline.Layer != "_StairCase")
                                                        &&(!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        splayerrcnt++;
                                                        splayerrcause += str;
                                                        objidlist.Add(plsply.ObjectId);
                                                        //Application.ShowAlertDialog(pline.Layer.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        foreach (Polyline plflr in Plugin.aprpwrkpline)
                        {
                            bool northblock = false;
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                // open the block table which contains all the BlockTableRecords (block definitions and spaces)
                                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                                // open the model space BlockTableRecord
                                var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                                // iterate through the model space 
                                foreach (ObjectId id in modelSpace)
                                {
                                    // check if the current ObjectId is a block reference one
                                    if (id.ObjectClass.DxfName == "INSERT")
                                    {
                                        // open the block reference
                                        var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                        if (blockReference.Name.Contains("DirectionRef_PreVal"))
                                        {
                                            double widthref = blockReference.GeometricExtents.MaxPoint.X - blockReference.GeometricExtents.MinPoint.X;
                                            double heightref = blockReference.GeometricExtents.MaxPoint.Y - blockReference.GeometricExtents.MinPoint.Y;
                                            if (RectangleIsInPolyline(plflr, blockReference.Position, new Point3d(blockReference.Position.X + widthref/2,
                                                blockReference.Position.Y + heightref/2, 0)))
                                            {
                                                northblock = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                            if (!northblock)
                            {
                                splayerrcnt++;
                                splayerrcause += "-This Polyline has no Preval made DirectionRef_PreVal Block reference.";
                                objidlist.Add(plflr.ObjectId);
                            }
                        }
                        
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_ProposedWork";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlist;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_CarpetArea":
                    {
                        int ploterrcnt = 0;
                        string ploterrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        if (Plugin.aCarpetpline.Count != 0)
                        {
                            foreach (Polyline pl in Plugin.aCarpetpline)
                            {
                                if (pl.Closed)
                                {
                                    bool bdbintxt = false;
                                    bool binTxt = false;
                                    foreach (MText txt in Plugin.aCarpetTxt)
                                    {
                                        binTxt = IsPointInside(txt.Location, pl);
                                        //binTxt = RectIsInPolyLine(pl, txt.Location, new Point3d(txt.Location.X + txt.Width / 2, txt.Location.Y + txt.Height / 2, 0));
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    if (!binTxt)
                                    {
                                        foreach (DBText dBText in Plugin.aCarpetDBTxt)
                                        {
                                            bdbintxt = IsPointInside(dBText.Position, pl);
                                            if (dBText.TextString == "")
                                                bdbintxt = false;
                                            if (bdbintxt)
                                                break;
                                        }
                                    }
                                    if (!binTxt && !bdbintxt)
                                    {
                                        ploterrcnt++;
                                        ploterrcause += "-This polyline does not have Label.";
                                        objidlist.Add(pl.ObjectId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ploterrcnt++;
                            ploterrcause += "-This Project has no CarpetArea Polyline.";
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = ploterrcnt;
                        err.lyrname = "_CarpetArea";
                        err.errcause = ploterrcause;
                        err.objIdlist = objidlist;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Balcony":
                    {
                        string Passageerrcause = "";
                        int errcnt = 0;
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl1 in Plugin.aBalconypline)
                        {
                            if (pl1.Closed)
                            {
                                bool bdbintxt = false;
                                bool binTxt = false;
                                foreach (MText txt in Plugin.aBalconyTxt)
                                {
                                    try
                                    {
                                        binTxt = IsPointInside(txt.Location, pl1);
                                        if (txt.Contents == "")
                                            binTxt = false;
                                        if (binTxt)
                                            break;
                                    }
                                    catch (Exception e) { }
                                }
                                if (!binTxt)
                                {
                                    foreach (DBText dBText in Plugin.aBalconyDBTxt)
                                    {
                                        bdbintxt = IsPointInside(dBText.Position, pl1);
                                        if (dBText.TextString == "")
                                            bdbintxt = false;
                                        if (bdbintxt)
                                            break;
                                    }
                                }
                                if (!binTxt && !bdbintxt)
                                {
                                    errcnt++;
                                    Passageerrcause += "-This polyline does not have Label.";
                                    objidlst.Add(pl1.ObjectId);
                                }
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
                                                Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                                if (id.ObjectClass == PlineCls)
                                                {
                                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                                    if (pline.Layer == "0")
                                                        continue;
                                                    if (PolyIsInPolyLine(pl1, pline) && (pline.ObjectId != pl1.ObjectId)
                                                        && (pline.Layer != pl1.Layer) && (pline.Layer != "_Door")
                                                        && (pline.Layer != "_Window")
                                                        && (!pline.Layer.Contains("_Fire")))
                                                    {
                                                        string str = "-*This polyline has " + pline.Layer.ToString() + " layer  Object.*";
                                                        errcnt++;
                                                        Passageerrcause += str;
                                                        objidlst.Add(pl1.ObjectId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tr.Commit();
                                }
                            }
                        }
                        //List<Polyline> plclosedlist = new List<Polyline>();
                        //List<Polyline> plcenterlist = new List<Polyline>();
                        //foreach (Polyline pl2 in Plugin.aPassagepline)
                        //{
                        //    if (!pl2.Closed && pl2.Linetype == "CENTER")
                        //    {
                        //        plcenterlist.Add(pl2);
                        //    }
                        //    if (pl2.Closed)
                        //        plclosedlist.Add(pl2);
                        //}

                        //foreach (Polyline plclo in plclosedlist)
                        //{
                        //    bool bcenter = false;
                        //    foreach (Polyline plcen in plcenterlist)
                        //    {
                        //        if (PolyIsInPolyLine(plclo, plcen))
                        //        {
                        //            bcenter = true;
                        //        }
                        //    }
                        //    if (!bcenter)
                        //    {
                        //        Passageerrcause += "-This polyline has no centerline.";
                        //        errcnt++;
                        //        objidlst.Add(plclo.ObjectId);
                        //    }
                        //}
                        ruleError err_passage = new ruleError();
                        err_passage.errorCnt = errcnt;
                        err_passage.lyrname = "_Balcony";
                        err_passage.errcause = Passageerrcause;
                        err_passage.objIdlist = objidlst;
                        Commands.errlist.Add(err_passage);
                        break;
                    }
                case "_NetPlot":
                    {
                        Document doc = Application.DocumentManager.MdiActiveDocument;
                        Database db = doc.Database;
                        Editor ed = doc.Editor;
                        using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                        {
                            foreach (Polyline pl in Plugin.anetpltpline)
                            {
                                ObjectId oid = pl.ObjectId;
                                Entity subent = tr.GetObject(oid, OpenMode.ForWrite) as Entity;
                                subent.Erase(true);
                            }
                            BlockTable acBlkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                            BlockTableRecord acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                            if (Plugin.aRdWidepline.Count == 0)
                            {
                                Entity plnet = Plugin.aplotpline[0].Clone() as Entity;
                                plnet.Layer = "_NetPlot";
                                var id = acBlkTblRec.AppendEntity(plnet);
                                tr.AddNewlyCreatedDBObject(plnet, true);
                                ed.UpdateScreen();
                            }
                            else
                            {
                                Point3dCollection ptcol = new Point3dCollection();
                                Point3dCollection ptremainplt = new Point3dCollection();
                                Point2dCollection ptnetall = new Point2dCollection();
                                Point3dCollection ptrdcol = new Point3dCollection();
                                for (int j = 0; j < Plugin.aRdWidepline[0].NumberOfVertices; j++)
                                {
                                    Point3d ptrdwd = Plugin.aRdWidepline[0].GetPoint3dAt(j);
                                    ptrdcol.Add(ptrdwd);
                                }
                                for (int j = 0; j < Plugin.aplotpline[0].NumberOfVertices; j++)
                                {
                                    Point3d ptplt = Plugin.aplotpline[0].GetPoint3dAt(j);
                                    ptremainplt.Add(ptplt);
                                }
                                foreach (Point3d ptrdwd in ptrdcol)
                                {
                                    bool bsamept = false;
                                    for (int j = 0; j < Plugin.aplotpline[0].NumberOfVertices; j++)
                                    {
                                        Point3d ptplt = Plugin.aplotpline[0].GetPoint3dAt(j);
                                        if (!IsSamePoint(ptrdwd, ptplt))
                                        {
                                            bsamept = false;
                                        }
                                        else
                                        {
                                            ptremainplt.Remove(ptplt);
                                            bsamept = true;
                                            break;
                                        }
                                    }
                                    if (!bsamept)
                                        ptcol.Add(ptrdwd);
                                }
                                Polyline plnet = new Polyline();
                                for (int i = 0; i < ptremainplt.Count; i++)
                                {
                                    ptnetall.Add(new Point2d(ptremainplt[i].X, ptremainplt[i].Y));
                                    //plnet.AddVertexAt(i,new Point2d())
                                }
                                for (int j = 0; j < ptcol.Count; j++)
                                    ptnetall.Add(new Point2d(ptcol[j].X, ptcol[j].Y));
                                for (int k = 0; k < ptnetall.Count; k++)
                                    plnet.AddVertexAt(k, ptnetall[k], 0, 0, 0);
                                plnet.Closed = true;
                                plnet.Layer = "_NetPlot";
                                acBlkTblRec.AppendEntity(plnet);
                                tr.AddNewlyCreatedDBObject(plnet, true);
                                ed.UpdateScreen();
                            }
                            tr.Commit();
                        }
                        break;
                    }
                case "0":
                    {                        
                        break;
                    }
            }
        }
        public static void SetViewCenterToObject(ObjectId id)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                Matrix3d DCS2WCS =
                    Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) *
                    Matrix3d.Displacement(view.Target - Point3d.Origin) *
                    Matrix3d.PlaneToWorld(view.ViewDirection);
                Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                Extents3d ext = ent.GeometricExtents;
                ext.TransformBy(DCS2WCS.Inverse());
                view.CenterPoint = new Point2d(
                    (ext.MaxPoint.X + ext.MinPoint.X) / 2.0,
                    (ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
                ed.SetCurrentView(view);
                tr.Commit();
            }
        }
        public static bool IsPointOnPolyline(Polyline pl, Point3d pt)           // this returns point is on polyline or not.
        {            
            bool isOn = false;
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                Curve3d seg = null;
                SegmentType segType = pl.GetSegmentType(i);
                if (segType == SegmentType.Arc)
                    seg = pl.GetArcSegmentAt(i);
                else
                if (segType == SegmentType.Line)
                    seg = pl.GetLineSegmentAt(i);
                    //seg1 = pl.GetLineSegment2dAt(i);
                if (seg != null)
                {
                    //isOn = seg.IsOn(pt);
                    try
                    { isOn = IsPointOnCurveGDAP(seg, pt); }
                    catch { throw; }
                    if (isOn)
                        break;
                }
            }
            return isOn;
        }
        public static bool IsPointOnPolyline1(Point3d point, Polyline polyline)
        {
            // Iterate through polyline vertices
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point2d vertex = polyline.GetPoint2dAt(i);

                // Check if the point is close to the vertex
                if (point.DistanceTo(new Point3d(vertex.X, vertex.Y, 0)) < Tolerance.Global.EqualPoint)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsPointOnCurveGDAP(Curve3d cv, Point3d pt)
        {
            try
            {
                //return cv.GetDistanceTo(pt) <= Tolerance.Global.EqualPoint;
                return cv.GetDistanceTo(pt) <= 0.1;
                //cv.GetDistAtPoint(pt);
                //return true;
            }
            catch { }
            return false;
        }
        private bool IsPointOnCurveGCP(Curve cv, Point3d pt)
        {
            try
            {
                Point3d p = cv.GetClosestPointTo(pt, false);
                return (p - pt).Length <= Tolerance.Global.EqualVector;
            }
            catch { }
            return false;
        }
        public static bool RectIsInPolyLine(Polyline pl, Point3d ptbegin, Point3d ptend)        //this returns rect that includes ptbegin and ptend is in polyline or not.
        {
            bool bresult = false;
            //if (Commands.Getleft(pl).X <= ptbegin.X && Commands.Getright(pl).X >= ptend.X && Commands.Getbottom(pl).Y <= ptbegin.Y && Commands.Gettop(pl).Y >= ptend.Y)
            //{
            //    bresult = true;
            //}
            //else
            //    bresult = false;
            Point3dCollection ptcol = new Point3dCollection();
            ptcol.Add(new Point3d(ptbegin.X, ptbegin.Y, 0));
            ptcol.Add(new Point3d(ptbegin.X, ptend.Y, 0));
            ptcol.Add(new Point3d(ptend.X, ptend.Y, 0));
            ptcol.Add(new Point3d(ptend.X, ptbegin.Y, 0));
            foreach(Point3d pt in ptcol)
            {
                try
                {
                    bresult = IsPointInside(pt, pl);
                }
                catch
                {
                    break;
                }
                if (!bresult)
                    break;
            }
            return bresult;
        }
        public static bool RectangleIsInPolyline(Polyline pl, Point3d ptbegin, Point3d ptend)
        {
            bool bresult = false;
            if (Commands.Getleft(pl).X <= ptbegin.X && Commands.Getright(pl).X >= ptend.X && Commands.Getbottom(pl).Y <= ptbegin.Y && Commands.Gettop(pl).Y >= ptend.Y)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;
        }
        public static bool PolyIsInPolyLine(Polyline plrect, Polyline plin)        //this returns rect that includes ptbegin and ptend is in polyline or not.
        {
            bool bresult = false;
            //if (Commands.Getleft(pl).X <= ptbegin.X && Commands.Getright(pl).X >= ptend.X && Commands.Getbottom(pl).Y <= ptbegin.Y && Commands.Gettop(pl).Y >= ptend.Y)
            //{
            //    bresult = true;
            //}
            //else
            //    bresult = false;
            Point3dCollection ptcol = new Point3dCollection();
            for(int i=0; i<plin.NumberOfVertices;i++)
            {
                ptcol.Add(plin.GetPoint3dAt(i));
            }
            
            foreach (Point3d pt in ptcol)
            {
                bresult = IsPointInside(pt, plrect);
                if (!bresult)
                    break;
            }
            return bresult;
        }
        public static bool PolyIsInPolyLine1(Polyline plrect, Polyline plin)        //this returns rect that includes ptbegin and ptend is in polyline or not.
        {
            bool bresult = false;
            Point3dCollection ptcol = new Point3dCollection();
            for (int i = 0; i < plin.NumberOfVertices; i++)
            {
                ptcol.Add(plin.GetPoint3dAt(i));
            }

            foreach (Point3d pt in ptcol)
            {
                bresult = IsPointInside1(pt, plrect);
                if (!bresult)
                    break;
            }
            return bresult;
        }
        public static bool checkTwoPlineTouch(Polyline pl1, Polyline pl2)            // this checks two polyline is close.
        {
            bool btch = false;

            int tchcnt = 0;
            for (int i = 0; i < pl2.NumberOfVertices; i++)
            {
                if (IsPointOnPolyline(pl1, pl2.GetPoint3dAt(i)))
                //if (IsPointOnPolyline1(pl2.GetPoint3dAt(i), pl1))
                {
                    tchcnt++;
                }
            }
            if (tchcnt >= 2)
            {
                btch = true;
                return btch;
            }
            tchcnt = 0;
            if (!btch)
            {
                for (int i = 0; i < pl1.NumberOfVertices; i++)
                {
                    if (IsPointOnPolyline(pl2, pl1.GetPoint3dAt(i)))
                    //if (IsPointOnPolyline1(pl1.GetPoint3dAt(i), pl2))
                    {
                        tchcnt++;
                    }
                }
                if (tchcnt >= 2)
                    btch = true;
            }
            return btch;
        }
        public static bool checkTwoPlineTchOrIntersect(Polyline pl1, Polyline pl2)            
        {
            bool btch = false;
            int tchcnt = 0;
            for (int i = 0; i < pl2.NumberOfVertices; i++)
            {
                if (IsPointOnPolyline(pl1, pl2.GetPoint3dAt(i)))
                //if (IsPointOnPolyline1(pl2.GetPoint3dAt(i), pl1))
                {
                    tchcnt++;
                }
            }
            if (tchcnt <= 2)
            {
                btch = true;
                return btch;
            }
            tchcnt = 0;
            if (!btch)
            {
                for (int i = 0; i < pl1.NumberOfVertices; i++)
                {
                    if (IsPointOnPolyline(pl2, pl1.GetPoint3dAt(i)))
                    //if (IsPointOnPolyline1(pl1.GetPoint3dAt(i), pl2))
                    {
                        tchcnt++;
                    }
                }
                if (tchcnt <= 2)
                    btch = true;
            }
            return btch;
        }
        public static void Swap(double first, double second)
        {
            double temp;
            temp = first;
            first = second;
            second = temp;
        }
        public static bool IsSamePoint(Point3d pt1, Point3d pt2)
        {
            bool bresult = false;
            if ((Math.Abs(pt1.X - pt2.X) < 0.1) && (Math.Abs(pt1.Y - pt2.Y) < 0.1))
                bresult = true;
            else
                bresult = false;
            return bresult;
        }

        public static bool IsPointInside(Point3d point, Polyline pline)
        {
            double tolerance = Tolerance.Global.EqualPoint;
            using (MPolygon mpg = new MPolygon())
            {
                try
                {
                    mpg.AppendLoopFromBoundary(pline, true, tolerance);
                }
                catch
                {
                    throw;
                }
                return mpg.IsPointInsideMPolygon(point, tolerance).Count == 1;
            }
        }
        public static bool IsPointInside1(Point3d point, Polyline pline)
        {
            bool bresult = false;
            if (Commands.Getleft(pline).X <= point.X && Commands.Getright(pline).X >= point.X 
                && Commands.Getbottom(pline).Y <= point.Y && Commands.Gettop(pline).Y >= point.Y)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;
        }
        public static bool ISPolyInPoly(Polyline plrect, Polyline plin)       
        {
            bool bresult = false;
            Point3d ptbegin=new Point3d(Commands.Getleft(plin).X,Commands.Getbottom(plin).Y,0);
            Point3d ptend=new Point3d(Commands.Getright(plin).X,Commands.Gettop(plin).Y,0);
            if (Commands.Getleft(plrect).X <= ptbegin.X && Commands.Getright(plrect).X >= ptend.X && Commands.Getbottom(plrect).Y <= ptbegin.Y && Commands.Gettop(plrect).Y >= ptend.Y)
            {
                bresult = true;
            }
            else
                bresult = false;

            return bresult;
        }
        public static bool IsPointInsideRect(Point3d point, Polyline pline)
        {
            bool bresult = false;
            if (Commands.Getleft(pline).X <= point.X &&
                Commands.Getright(pline).X >= point.X && Commands.Getbottom(pline).Y <= point.Y && Commands.Gettop(pline).Y >= point.Y)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;
        }
        public static bool IsDBTxtInsideRect(DBText txt, Polyline pline)
        {
            bool bresult = false;
            Extents3d extents = (Extents3d)txt.Bounds;
            double width = extents.MaxPoint.X - extents.MinPoint.X;
            if (Commands.Getleft(pline).X <= txt.Position.X &&
                Commands.Getright(pline).X >= (txt.Position.X+width )&& Commands.Getbottom(pline).Y <= txt.Position.Y && Commands.Gettop(pline).Y >= txt.Position.Y+txt.Height)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;
        }
        public static bool IsMTxtInsideRect(MText txt, Polyline pline)
        {
            bool bresult = false;
            if (Commands.Getleft(pline).X <= txt.Location.X &&
                Commands.Getright(pline).X >= (txt.Location.X+txt.Width) && Commands.Getbottom(pline).Y <= txt.Location.Y &&
                Commands.Gettop(pline).Y >= txt.Location.Y + txt.Height)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;
        }
    }
}
