using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Exception = System.Exception;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;

namespace ProsoftAcPlugin
{
    public class NBCrelate
    {        
        public void Initialize()
        {
            
        }
        public void Terminate()
        {
        }
        public static void Rulecheck()
        {
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            RuleInit();
            foreach (string layername in Plugin.allLayers)
            {
                GetNeededEntitiesOnLayer(database, layername);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
            }
            foreach (string layername in Plugin.allLayers)
            {
                if (Plugin.projtypestate == 3 || Plugin.projtypestate == 4 || Plugin.projtypestate == 5)
                    LayerRuleCheck_Layout( layername);
                else if (Plugin.projtypestate == 0)
                    LayerRuleCheck_BldgPermiss( layername);
            }
        }
        public static void RuleInit()
        {
            Commands.errlist.Clear();
            Plugin.awindowpline.Clear();
            Plugin.aroompline.Clear();
            Plugin.adoorpline.Clear();
            Plugin.aplotpline.Clear();
            Plugin.amroadpline.Clear();
            Plugin.aindvSubPltpline.Clear();
            Plugin.ainterroadpline.Clear();
            Plugin.aopenspacepline.Clear();
            Plugin.aAmenitypline.Clear();
            Plugin.aMortgageAreapline.Clear();
            Plugin.asplaypline.Clear();
            Plugin.aBufferpline.Clear();
            Plugin.aWaterBodypline.Clear();
            Plugin.aWaterlinepline.Clear();
            Plugin.aLeftownerspline.Clear();
            Plugin.aSurAuthpline.Clear();
            Plugin.aCompndwllpline.Clear();
            Plugin.aElectricpline.Clear();
            Plugin.aGllvlpline.Clear();
            Plugin.aFlrinSecpline.Clear();
            Plugin.aPropWrkpline = null;
            Plugin.aParkingpline.Clear();
            Plugin.aDrivewaypline.Clear();
            Plugin.arampline.Clear();
            Plugin.aFloorpline.Clear();
            Plugin.aVoidpline.Clear();
            Plugin.aAccusepline.Clear();
            Plugin.aNalapline.Clear();
            Plugin.aStairpline.Clear();
            Plugin.aStairpline.Clear();
            Plugin.aVenShaftpline.Clear();

            Plugin.awindowNmTxt.Clear();
            Plugin.awindowNmTxt.Clear();
            Plugin.aroomNmTxt.Clear();
            Plugin.adoorNmTxt.Clear();
            Plugin.aplotNmTxt.Clear();
            Plugin.aindvsubPltTxt.Clear();
            Plugin.ainterroadTxt.Clear();
            Plugin.aopenspaceTxt.Clear();
            Plugin.aAmenityTxt.Clear();
            Plugin.aMortgageAreaTxt.Clear();
            Plugin.asplayTxt.Clear();
            Plugin.aBufferTxt.Clear();
            Plugin.aWaterBodyTxt.Clear();
            Plugin.aWaterlineTxt.Clear();
            Plugin.aLeftOwnersTxt.Clear();
            Plugin.aSurAuthTxt.Clear();
            Plugin.aCmpWallTxt.Clear();
            Plugin.aElineTxt.Clear();
            Plugin.aGllvlTxt.Clear();
            Plugin.aFlrinSecTxt.Clear();
            Plugin.aPropWrkTxt = null;
            Plugin.aParkingTxt.Clear();
            Plugin.aDrivewayTxt.Clear();
            Plugin.arampTxt.Clear();
            Plugin.aFloorTxt.Clear();
            Plugin.aVoidTxt.Clear();
            Plugin.aAccuseTxt.Clear();
            Plugin.aNalaTxt.Clear();
            Plugin.aStairTxt.Clear();
            Plugin.aPassageTxt.Clear();
            Plugin.aVenShaftTxt.Clear();
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
            Plugin.aWaterBodyTxt.Clear();
            Plugin.aWaterlineTxt.Clear();
            Plugin.aLeftOwnersTxt.Clear();
            Plugin.aSurAuthTxt.Clear();
            Plugin.aCmpWallTxt.Clear();
            Plugin.aElineTxt.Clear();
            Plugin.aGllvlTxt.Clear();
            Plugin.aFlrinSecTxt.Clear();
            Plugin.aPropWrkTxt = null;
            Plugin.aParkingTxt.Clear();
            Plugin.aDrivewayTxt.Clear();
            Plugin.arampTxt.Clear();
            Plugin.aVoidTxt.Clear();
            Plugin.aAccuseTxt.Clear();
            Plugin.aNalaTxt.Clear();
            Plugin.aStairTxt.Clear();
            Plugin.aPassageTxt.Clear();
            Plugin.aVenShaftTxt.Clear();
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
                    var MTxtCls= RXObject.GetClass(typeof(MText));
                    var TxtCls = RXObject.GetClass(typeof(DBText));
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity subent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            if(subent.Layer==layerName)
                            {
                                if (id.ObjectClass == PlineCls)
                                {
                                    var pline = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                                    if (pline.Layer.Equals(layerName, System.StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        switch (layerName)
                                        {
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
                                                Plugin.aPropWrkpline = pline;
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
                                                if(pline.Closed)
                                                    Plugin.aStairpline.Add(pline);
                                                break;
                                            case "_Passage":
                                                Plugin.aStairpline.Add(pline);
                                                break;
                                            case "_VentilationShaft":
                                                Plugin.aVenShaftpline.Add(pline);
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
                                            Plugin.aPropWrkTxt = pObj;
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
                                    }
                                }
                                if(id.ObjectClass == TxtCls)
                                {
                                    var pObj1 = (DBText)tr.GetObject(id, OpenMode.ForRead);
                                    switch (layerName)
                                    {
                                        case "_Floor":
                                            Plugin.aFloorTxt.Add(pObj1);
                                            break;
                                    }
                                }
                            }                            
                        }
                    }
                }
            }
        }
        public static void LayerRuleCheck_Layout( string slayer)
        {
            var documentManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var currentDocument = documentManager.MdiActiveDocument;
            var database = currentDocument.Database;
            switch (slayer)
            {
                case "_Window":
                    {
                        int windowerrcnt = 0;
                        string winerrcause = "";
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            double rarea = pln.Area;
                            foreach (Polyline wpl in Plugin.awindowpline)
                            {
                                for (int i = 0; i < wpl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = wpl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        double warea = 0;
                                        foreach (ProsoftAcPlugin.windowrule rule in ProsoftAcPlugin.Commands.awindowrule)
                                        {
                                            if (rule.pl == wpl)
                                            {
                                                warea = rule.width * rule.height;
                                                MessageBox.Show("Window Area: " + warea.ToString());
                                            }
                                        }

                                        if (warea < (rarea * 0.1))
                                        {
                                            windowerrcnt++;
                                            Commands.windowerrcause.Add("This Window is little than 10 % area of room");
                                            winerrcause = winerrcause + "-" + "This Window is little than 10 % area of room";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = windowerrcnt;
                        err.lyrname = "_Window";
                        err.errcause = winerrcause;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Room":
                    {
                        int windowerrcntrm = 0;
                        string roomerrcause = "";
                        foreach (Polyline pln in Plugin.aroompline)
                        {
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
                                            Commands.roomerrcause.Add("This room does not satisfy ventilation requirement.");
                                            roomerrcause = roomerrcause + "-" + "This room does not satisfy ventilation requirement.";
                                            break;
                                        }
                                    }
                                }
                            }
                            bool bIntext = false;
                            foreach (MText inst in Plugin.aroomNmTxt)
                            {
                                bIntext = IsInPolyLine(pln, inst.GeometricExtents.MinPoint, inst.GeometricExtents.MaxPoint);
                                if (bIntext)
                                    break;
                                else
                                {
                                    windowerrcntrm++;
                                    Commands.roomerrcause.Add("This room does not contain roomname.");
                                    roomerrcause = roomerrcause + "-" + "This room does not contain roomname.";
                                }
                            }
                        }
                        ruleError errrm = new ruleError();
                        errrm.errorCnt = windowerrcntrm;
                        errrm.lyrname = "_Room";
                        errrm.errcause = roomerrcause;
                        Commands.errlist.Add(errrm);
                        break;
                    }
                case "_Door":
                    {
                        int doorerrcnt = 0;
                        string doorerrcause = "";
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            double rarea = pln.Area;
                            foreach (Polyline dpl in Plugin.adoorpline)
                            {
                                for (int i = 0; i < dpl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = dpl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        double darea = 0;
                                        foreach (ProsoftAcPlugin.doorrule rule in ProsoftAcPlugin.Commands.adoorrule)
                                        {
                                            if (rule.pl == dpl)
                                            {
                                                darea = rule.width * rule.height;
                                                MessageBox.Show("Door Area: " + darea.ToString());
                                            }
                                        }

                                        if (darea < (rarea * 0.1))
                                        {
                                            doorerrcnt++;
                                            doorerrcause +="-This Window is little than 10 % area of room";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = doorerrcnt;
                        err.lyrname = "_Door";
                        err.errcause = doorerrcause;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Plot":
                    {
                        int ploterrcnt = 0;
                        bool istch = false;
                        string ploterrcaues = "";
                        foreach (Polyline pl1 in Plugin.amroadpline)
                        {
                            foreach (Polyline pl2 in Plugin.aplotpline)
                                istch = checkTwoPlineTouch(pl1, pl2);
                        }
                        if (!istch)
                        {
                            ploterrcaues = ploterrcaues + "-" + "Mainroad and PLot does not touch";
                            ploterrcnt++;
                        }
                        ruleError errplot = new ruleError();
                        errplot.errorCnt = ploterrcnt;
                        errplot.lyrname = "_Plot";
                        errplot.errcause = ploterrcaues;
                        Commands.errlist.Add(errplot);
                        break;
                    }
                case "_MainRoad":
                    {
                        string mroaderrcause = "";
                        int mroaderrcnt = 0;
                        bool istchmroad = false;
                        foreach (Polyline pl in Plugin.amroadpline)
                        {
                            if (!istchmroad)
                            {
                                foreach (Polyline plinternal in Plugin.aplotpline)
                                {
                                    istchmroad = checkTwoPlineTouch(pl, plinternal);
                                    if (istchmroad)
                                    {
                                        Point3d ptleft = Commands.Getleft(plinternal);
                                        Point3d ptright = Commands.Getright(plinternal);
                                        Point3d pttop = Commands.Gettop(plinternal);
                                        Point3d ptbottom = Commands.Getbottom(plinternal);
                                        double width = ptright.X - ptleft.X;
                                        double height = pttop.Y - ptbottom.Y;
                                        if (width >= 30 || height >= 30)
                                            break;
                                        else
                                            istchmroad = false;
                                    }
                                }
                            }
                        }
                        if (!istchmroad)
                        {
                            mroaderrcause += "This layer has no entity is closed to Plot or width is less than 30mts.";
                            mroaderrcnt++;
                        }
                        ruleError errmroad = new ruleError();
                        errmroad.errorCnt = mroaderrcnt;
                        errmroad.lyrname = "_MainRoad";
                        errmroad.errcause = mroaderrcause;
                        Commands.errlist.Add(errmroad);
                        break;
                    }
                case "_IndivSubPlot":
                    {
                        double totalAreaSubPlt = 0, PlotArea = 0;
                        string errcause = "";
                        bool istch1 = false;
                        int inderrcnt = 0;
                        foreach (Polyline pl in Plugin.aindvSubPltpline)
                        {
                            totalAreaSubPlt += pl.Area; istch1 = false;
                            foreach (Polyline plintrd in Plugin.ainterroadpline)
                            {
                                istch1 = checkTwoPlineTouch(pl, plintrd);
                                if (Commands.IsOverlapped(pl))
                                {
                                    inderrcnt++;
                                    errcause += "-some plottings are overlapped.";
                                }
                                if (istch1)
                                    break;
                            }
                            if (!istch1)
                            {
                                inderrcnt++;
                                errcause += "-Polyline does not touch with Internal Road layer.";
                            }
                        }

                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        if (totalAreaSubPlt > PlotArea * 0.6)
                        {
                            errcause += "-Total Individual Sub Plot Area is more than 60%";
                            inderrcnt++;
                        }
                        ruleError errIndsub = new ruleError();
                        errIndsub.errorCnt = inderrcnt;
                        errIndsub.lyrname = "_IndivSubPlot";
                        errIndsub.errcause = errcause;
                        Commands.errlist.Add(errIndsub);
                        break;
                    }
                case "_InternalRoad":
                    {
                        bool istchmrd = false, istchindv = false, istchamenity = false, istchopensp = false;
                        string intrderrcause = "";
                        int intrderrcnt = 0;
                        foreach (Polyline plinternal in Plugin.ainterroadpline)
                        {
                            Point3d ptleft = Commands.Getleft(plinternal);
                            Point3d ptright = Commands.Getright(plinternal);
                            Point3d pttop = Commands.Gettop(plinternal);
                            Point3d ptbottom = Commands.Getbottom(plinternal);
                            double width = ptright.X - ptleft.X;
                            double height = pttop.Y - ptbottom.Y;
                            if (width <= 9.0 || height <= 0.0)
                            {
                                intrderrcause += "some Internal road width is smaller than 9.0 mts.";
                                intrderrcnt++;
                            }
                            istchindv = false;
                            if (!istchmrd)
                                foreach (Polyline plmrd in Plugin.amroadpline)
                                {
                                    if (checkTwoPlineTouch(plinternal, plmrd))
                                        break;
                                }
                            if (!istchamenity)
                                foreach (Polyline plamen in Plugin.aAmenitypline)
                                {
                                    if (checkTwoPlineTouch(plinternal, plamen))
                                        break;
                                }
                            if (!istchopensp)
                                foreach (Polyline plopen in Plugin.aopenspacepline)
                                {
                                    if (checkTwoPlineTouch(plinternal, plopen))
                                        break;
                                }
                            foreach (Polyline plindv in Plugin.aindvSubPltpline)
                            {
                                if (istchindv = checkTwoPlineTouch(plinternal, plindv))
                                    break;
                            }
                            if (!istchindv)
                            {
                                intrderrcause += "some Internal roads are not closed with individual sub plots layer.";
                                intrderrcnt++;
                            }
                        }
                        if (!istchmrd)
                        {
                            intrderrcause += "Every Internal roads are not closed with MainRoad layer.";
                            intrderrcnt++;
                        }
                        if (!istchamenity)
                        {
                            intrderrcause += "Every Internal roads are not closed with Socialinfrastructure layer.";
                            intrderrcnt++;
                        }
                        if (!istchopensp)
                        {
                            intrderrcause += "Every Internal roads are not closed with organization open space layer.";
                            intrderrcnt++;
                        }
                        ruleError errIntrd = new ruleError();
                        errIntrd.errorCnt = intrderrcnt;
                        errIntrd.lyrname = "_InternalRoad";
                        errIntrd.errcause = intrderrcause;
                        Commands.errlist.Add(errIntrd);
                        break;
                    }
                case "_OrganizedOpenSpace":
                    {
                        double totalopenspacearea = 0, PlotArea1 = 0;
                        string openerrcause = "";
                        int openerrcnt = 0;
                        bool istchopen = false;
                        foreach (Polyline pl in Plugin.aopenspacepline)
                        {
                            totalopenspacearea += pl.Area;
                            if (!istchopen)
                            {
                                foreach (Polyline plinternal in Plugin.ainterroadpline)
                                {
                                    istchopen = checkTwoPlineTouch(pl, plinternal);
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
                        }
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea1 += pl2.Area;
                        if (totalopenspacearea > PlotArea1 * 0.075)
                        {
                            openerrcause += "-Total Organized OpenSpace Area is more than 7.5%";
                            openerrcnt++;
                        }
                        if (!istchopen)
                        {
                            openerrcause += "This layer has no entity is closed to Internal road of which width is above 9mts.";
                            openerrcnt++;
                        }
                        ruleError erropenspace = new ruleError();
                        erropenspace.errorCnt = openerrcnt;
                        erropenspace.lyrname = "_OrganizedOpenSpace";
                        erropenspace.errcause = openerrcause;
                        Commands.errlist.Add(erropenspace);
                        break;
                    }
                case "_Amenity":
                    {
                        double socialarea = 0, PlotArea_social = 0;
                        string socialerrcause = "";
                        int socialerrcnt = 0;
                        bool istchsocial = false;
                        foreach (Polyline pl in Plugin.aAmenitypline)
                        {
                            socialarea += pl.Area;
                            if (!istchsocial)
                            {
                                foreach (Polyline plinternal in Plugin.ainterroadpline)
                                {
                                    istchsocial = checkTwoPlineTouch(pl, plinternal);
                                    if (istchsocial)
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
                                            istchsocial = false;
                                    }
                                }
                            }
                        }
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea_social += pl2.Area;
                        if (socialarea > PlotArea_social * 0.025)
                        {
                            socialerrcause += "-Total Organized OpenSpace Area is more than 7.5%";
                            socialerrcnt++;
                        }
                        if (!istchsocial)
                        {
                            socialerrcause += "This layer has no entity is closed to Internal road of which width is above 9mts.";
                            socialerrcnt++;
                        }
                        ruleError errsocial = new ruleError();
                        errsocial.errorCnt = socialerrcnt;
                        errsocial.lyrname = "_Amenity";
                        errsocial.errcause = socialerrcause;
                        Commands.errlist.Add(errsocial);
                        break;
                    }
                case "_MortgageArea":
                    {
                        double mortarea = 0, PlotArea_mort = 0;
                        string morterrcause = "";
                        int morterrcnt = 0;
                        bool istchmort_indv = false, istchmort_plt;
                        foreach (Polyline pl in Plugin.aMortgageAreapline)
                        {
                            mortarea += pl.Area;
                            istchmort_indv = false; istchmort_plt = false; ;
                            if (!istchmort_indv)
                            {
                                foreach (Polyline plindv in Plugin.aindvSubPltpline)
                                {
                                    istchmort_indv = checkTwoPlineTouch(pl, plindv);
                                    if (istchmort_indv)
                                        break;
                                }
                                if (!istchmort_indv)
                                {
                                    morterrcause += "- Some Mortgage Area is not closed to Individual SubPlot Layer.";
                                    morterrcnt++;
                                }
                            }
                            if (!istchmort_plt)
                            {
                                foreach (Polyline plplt in Plugin.aplotpline)
                                {
                                    istchmort_plt = checkTwoPlineTouch(pl, plplt);
                                    if (istchmort_plt)
                                        break;
                                }
                                if (!istchmort_plt)
                                {
                                    morterrcause += "- Some Mortgage Area is not closed to Individual SubPlot Layer.";
                                    morterrcnt++;
                                }
                            }
                        }
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea_mort += pl2.Area;
                        if (mortarea < PlotArea_mort * 0.15)
                        {
                            morterrcause += "-Total Organized OpenSpace Area is more than 7.5%";
                            morterrcnt++;
                        }
                        ruleError errmortArea = new ruleError();
                        errmortArea.errorCnt = morterrcnt;
                        errmortArea.lyrname = "_MortgageArea";
                        errmortArea.errcause = morterrcause;
                        Commands.errlist.Add(errmortArea);
                        break;
                    }
                case "_Splay":
                    {
                        bool splayinplot = false, istchsplay_indiv, istchsplay_mrd = false, istchsplay_intrd;
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        foreach (Polyline pl in Plugin.asplaypline)
                        {
                            splayinplot = false;
                            istchsplay_indiv = false;
                            istchsplay_mrd = false;
                            istchsplay_intrd = false;
                            Point3d ptleft = Commands.Getleft(pl);
                            Point3d ptright = Commands.Getright(pl);
                            Point3d pttop = Commands.Gettop(pl);
                            Point3d ptbottom = Commands.Getbottom(pl);
                            foreach (Polyline plplot in Plugin.aplotpline)
                            {
                                Point3d Upperleft = new Point3d(ptleft.X, pttop.Y, 0);
                                Point3d Bottomright = new Point3d(ptright.X, ptbottom.Y, 0);
                                splayinplot = IsInPolyLine(plplot, Upperleft, Bottomright);
                            }
                            if (!splayinplot)
                            {
                                splayerrcnt++;
                                splayerrcause += "-Some Splay entities are not in Plot Area.";
                            }
                            foreach (Polyline plindiv in Plugin.aindvSubPltpline)
                            {
                                istchsplay_indiv = checkTwoPlineTouch(plindiv, pl);
                            }
                            if (!istchsplay_indiv)
                            {
                                splayerrcause += "-Some Splay entites are not touched with Individual Sub Plot Layer.";
                                splayerrcnt++;
                            }
                            foreach (Polyline plmrd in Plugin.amroadpline)
                            {
                                istchsplay_mrd = checkTwoPlineTouch(plmrd, pl);
                            }
                            if (!istchsplay_mrd)
                            {
                                splayerrcause += "-Some Splay entites are not touched with Mainroad Layer.";
                                splayerrcnt++;
                            }
                            foreach (Polyline plintrd in Plugin.ainterroadpline)
                            {
                                istchsplay_intrd = checkTwoPlineTouch(plintrd, pl);
                            }
                            if (!istchsplay_intrd)
                            {
                                splayerrcause += "-Some Splay entites are not touched with Internal road Layer.";
                                splayerrcnt++;
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Splay";
                        errsplay.errcause = splayerrcause;
                        Commands.errlist.Add(errsplay);
                        break;
                    }
                case "_BufferZone":
                    {
                        bool istchbuffer = false;
                        string buferrcause = "";
                        int buferrcnt = 0;
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
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "BufferZone is not closed with WaterBodies.";
                                    buferrcnt++;
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
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "BufferZone is not closed with ElectricLine.";
                                    buferrcnt++;
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
                                        }
                                        break;
                                    }
                                }
                                if (!istchbuffer)
                                {
                                    buferrcause += "BufferZone is not closed with ElectricLine.";
                                    buferrcnt++;
                                }
                            }
                        }
                        ruleError errbuf = new ruleError();
                        errbuf.errorCnt = buferrcnt;
                        errbuf.lyrname = "_BufferZone";
                        errbuf.errcause = buferrcause;
                        Commands.errlist.Add(errbuf);
                        break;
                    }
                case "_LeftoverOwnersLand":
                    {
                        foreach (Polyline pl in Plugin.aLeftownerspline)
                        {
                            Plugin.LeftOwnerArea += pl.Area;
                        }
                        break;
                    }
                case "_SurrenderToAuthority":
                    {
                        foreach (Polyline pl in Plugin.aSurAuthpline)
                        {
                            Plugin.SurroundtoAuthorityArea += pl.Area;
                        }
                        break;
                    }
                case "_CompoundWall":
                    {
                        bool istchcmp_plt = false;
                        string cmpwallerrcause = "";
                        int cmpwlerrcauseCnt = 0;
                        foreach (Polyline pl in Plugin.aCompndwllpline)
                        {
                            if (!istchcmp_plt)
                            {
                                foreach (Polyline plplt in Plugin.aplotpline)
                                {
                                    istchcmp_plt = checkTwoPlineTouch(plplt, pl);
                                    if (istchcmp_plt)
                                        break;
                                }
                            }
                        }
                        if (!istchcmp_plt)
                        {
                            cmpwallerrcause += "-Some CompoundWall entities are not closed with Plot.";
                            cmpwlerrcauseCnt++;
                        }
                        ruleError errcmpwl = new ruleError();
                        errcmpwl.errorCnt = cmpwlerrcauseCnt;
                        errcmpwl.lyrname = "_CompoundWall";
                        errcmpwl.errcause = cmpwallerrcause;
                        Commands.errlist.Add(errcmpwl);
                        break;
                    }
                case "_ElectricLine":
                    {
                        List<Point3d> elineptlist = new List<Point3d>();
                        List<Point3d> Pltlineptlist = new List<Point3d>();
                        string elineerrcause = "";
                        int elineerrcnt = 0;
                        foreach (Polyline plplt in Plugin.aplotpline)
                        {
                            for (int i = 0; i < plplt.NumberOfVertices; i++)
                            {
                                Pltlineptlist.Add(plplt.GetPoint3dAt(i));
                            }
                        }
                        switch (Plugin.elinestate)
                        {
                            case 1:
                                {
                                    foreach (Polyline pl in Plugin.aElectricpline)
                                    {
                                        elineptlist.Clear();

                                        for (int i = 0; i < pl.NumberOfVertices; i++)
                                        {
                                            elineptlist.Add(pl.GetPoint3dAt(i));
                                        }
                                        foreach (Point3d pt in Pltlineptlist)
                                        {
                                            int cntx = 0, cnty = 0;
                                            foreach (Point3d pt1 in elineptlist)
                                            {
                                                if (pt.X == pt1.X + 3 || pt.X == pt1.X - 3)
                                                {
                                                    cntx++;
                                                    if (cntx == 2)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (pt.Y == pt1.Y + 3 || pt.Y == pt1.Y - 3)
                                                {
                                                    cnty++;
                                                    if (cnty == 2)
                                                    {
                                                        break;
                                                    }
                                                }

                                            }
                                            if (cntx < 2 || cnty < 2)
                                            {
                                                elineerrcause += "-Some Electric Line entities are not maintained safety distance.";
                                                elineerrcnt++;
                                            }
                                        }
                                    }
                                    break;
                                }

                            case 2:
                                {
                                    foreach (Polyline pl in Plugin.aElectricpline)
                                    {
                                        elineptlist.Clear();

                                        for (int i = 0; i < pl.NumberOfVertices; i++)
                                        {
                                            elineptlist.Add(pl.GetPoint3dAt(i));
                                        }
                                        foreach (Point3d pt in Pltlineptlist)
                                        {
                                            int cntx = 0, cnty = 0;
                                            foreach (Point3d pt1 in elineptlist)
                                            {
                                                if (pt.X == pt1.X + 1.5 || pt.X == pt1.X - 1.5)
                                                {
                                                    cntx++;
                                                    if (cntx == 2)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (pt.Y == pt1.Y + 1.5 || pt.Y == pt1.Y - 1.5)
                                                {
                                                    cnty++;
                                                    if (cnty == 2)
                                                    {
                                                        break;
                                                    }
                                                }

                                            }
                                            if (cntx < 2 || cnty < 2)
                                            {
                                                elineerrcause += "-Some Electric Line entities are not maintained safety distance.";
                                                elineerrcnt++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 3:
                                break;
                        }
                        break;
                    }
            }
        }
        public static void LayerRuleCheck_BldgPermiss( string slayer)
        {
            switch (slayer)
            {
                case "_Window":    
                    {
                        int windowerrcnt = 0;
                        string winerrcause = "";
                        List<ObjectId> objidlist = new List<ObjectId>();
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            double rarea = pln.Area;
                            foreach (Polyline wpl in Plugin.awindowpline)
                            {
                                for (int i = 0; i < wpl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = wpl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        double warea = 0;
                                        foreach (ProsoftAcPlugin.windowrule rule in ProsoftAcPlugin.Commands.awindowrule)
                                        {
                                            if (rule.pl == wpl)
                                            {
                                                warea = rule.width * rule.height;
                                                MessageBox.Show("Window Area: "+warea.ToString());
                                            }
                                        }
                                        
                                        if (warea < (rarea * 0.1))
                                        {
                                            windowerrcnt++;
                                            Commands.windowerrcause.Add("This Window is little than 10 % area of room");
                                            winerrcause = winerrcause + "-" + "This Window is little than 10 % area of room";
                                            objidlist.Add(wpl.ObjectId);
                                            break;
                                        }
                                    }
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
                case "_Plot":
                    {
                        double buildinghght = 0;
                        Point3d topPt = new Point3d(0, 0, 0);
                        Point3d bottomPt = new Point3d(0, 0, 0);
                        double PlotArea = 0;
                        string plterrcause = "";
                        int plterrcnt = 0;
                        double roadwidth = 0;
                        bottomPt = Plugin.aFlrinSecpline.First().GetPoint3dAt(0);
                        List<ObjectId> objlst = new List<ObjectId>();
                        Polyline PlotLine = Plugin.aplotpline.First();
                        Polyline PrpWkline = Plugin.aPropWrkpline;
                        Point3d ptleftplt = Commands.Getleft(PlotLine);
                        Point3d ptrightplt = Commands.Getright(PlotLine);
                        Point3d pttopplt = Commands.Gettop(PlotLine);
                        Point3d ptbottomplt = Commands.Getbottom(PlotLine);

                        Point3d ptleftprp = Commands.Getleft(PrpWkline);
                        Point3d ptrightprp = Commands.Getright(PrpWkline);
                        Point3d pttopprp = Commands.Gettop(PrpWkline);
                        Point3d ptbottomprp = Commands.Getbottom(PrpWkline);

                        double lsetback, rsetback, fsetback, rearsetback;
                        lsetback = Math.Abs(ptleftplt.X - ptleftprp.X);
                        rsetback = Math.Abs(ptrightplt.X - ptrightprp.X);
                        fsetback = Math.Abs(pttopplt.Y - pttopprp.Y);
                        rearsetback = Math.Abs(ptbottomplt.Y - ptbottomprp.Y);
                        if (fsetback < rearsetback)
                        {
                            Swap(fsetback, rearsetback);
                        }
                        if (Plugin.amroadNmTxt.Count != 0)
                        {
                            string widthtxt = Commands.GetMTextContent(Plugin.amroadNmTxt.ElementAt(0));
                            roadwidth = Commands.GetRoadWidth(widthtxt);
                        }
                        MText terracetxt=null;
                        foreach(MText txt in Plugin.aFlrinSecTxt)
                        {
                            string str = txt.Contents.ToUpper();
                            MessageBox.Show(str);
                            if (str.Contains("TERRACE"))
                            {
                                terracetxt = txt;
                                MessageBox.Show("Terrace: " + terracetxt.ObjectId.ToString());
                            }
                        }
                        
                        foreach (Polyline pl in Plugin.aFlrinSecpline)
                        {
                            if(terracetxt!=null)
                            {
                                if (IsInPolyLine(pl, terracetxt.GeometricExtents.MinPoint,terracetxt.GeometricExtents.MaxPoint))
                                {
                                    for (int i = 0; i < pl.NumberOfVertices; i++)
                                    {
                                        if (topPt.Y < pl.GetPoint3dAt(i).Y)
                                            topPt = pl.GetPoint3dAt(i);
                                    }
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
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        buildinghght = topPt.Y - bottomPt.Y;
                        MessageBox.Show("Building Height: "+buildinghght.ToString());
                        if (PlotArea <= 50)
                        {
                            if (buildinghght > 7)
                            {
                                plterrcause += "-Current Plot Area is less than 50. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            else
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (50 <= PlotArea && PlotArea <= 100)
                        {
                            if (buildinghght < 7 || buildinghght > 10)
                            {
                                plterrcause += "-Current Plot Area is less than 100. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            else if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            else
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 1.5 || rsetback != 0.5 || lsetback != 0.5 || rearsetback != 0.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 1.5 || rsetback != 0.5 || lsetback != 0.5 || rearsetback != 0.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3 || rsetback != 0.5 || lsetback != 0.5 || rearsetback != 0.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 3 || rsetback != 0.5 || lsetback != 0.5 || rearsetback != 0.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 3 || rsetback != 0.5 || lsetback != 0.5 || rearsetback != 0.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (100 <= PlotArea && PlotArea <= 200)
                        {
                            if (buildinghght > 10)
                            {
                                plterrcause += "-Current Plot Area is less than 200. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            else
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 1.5 || rsetback != 1.0 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 1.5 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 3 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 3 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (200 <= PlotArea && PlotArea <= 300)
                        {
                            if (buildinghght < 7 || buildinghght > 10)
                            {
                                plterrcause += "-Current Plot Area is less than 300. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            else if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 2 || rsetback != 1.0 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 3 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 4 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 5 || rsetback != 1 || lsetback != 1 || rearsetback != 1)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 || buildinghght <= 10)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 2 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 3 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 3 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 5 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 6 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (300 <= PlotArea && PlotArea <= 400)
                        {
                            if (buildinghght < 7 || buildinghght > 12)
                            {
                                plterrcause += "-Current Plot Area is less than 400. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 1.5 || lsetback != 1.5 || rearsetback != 1.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 12)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (400 <= PlotArea && PlotArea <= 500)
                        {
                            if (buildinghght < 7 || buildinghght > 12)
                            {
                                plterrcause += "-Current Plot Area is less than 500. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 2 || lsetback != 2 || rearsetback != 2)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 12)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        if (500 <= PlotArea && PlotArea <= 750)
                        {
                            if (buildinghght < 7 || buildinghght > 15)
                            {
                                plterrcause += "-Current Plot Area is less than 750. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 2.5 || lsetback != 2.5 || rearsetback != 2.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 12)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 12 && buildinghght <= 15)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }
                        if (750 <= PlotArea && PlotArea <= 1000)
                        {
                            if (buildinghght < 7 || buildinghght > 15)
                            {
                                plterrcause += "-Current Plot Area is less than 1000. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 3 || lsetback != 3 || rearsetback != 3)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 12)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 12 && buildinghght <= 15)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }
                        if (1000 <= PlotArea && PlotArea <= 1500)
                        {
                            if (buildinghght < 7 || buildinghght > 18)
                            {
                                plterrcause += "-Current Plot Area is less than 1500. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 3.5 || lsetback != 3.5 || rearsetback != 3.5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 12)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 12 && buildinghght <= 15)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 15 && buildinghght <= 18)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }
                        if (1500 <= PlotArea && PlotArea <= 2500)
                        {
                            if (buildinghght < 7 || buildinghght > 18)
                            {
                                plterrcause += "-Current Plot Area is less than 2500. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 4 || lsetback != 4 || rearsetback != 4)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 15)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 15 && buildinghght <= 18)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }
                        if (2500 <= PlotArea)
                        {
                            if (buildinghght < 7 || buildinghght > 18)
                            {
                                plterrcause += "-Current Plot Area is more than 2500. Building Height is out of rule";
                                plterrcnt++;
                                objlst.Add(Plugin.aplotpline[0].ObjectId);
                            }
                            if (buildinghght == 7)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 5 || lsetback != 5 || rearsetback != 5)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 7 && buildinghght <= 15)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 6 || lsetback != 6 || rearsetback != 6)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                            if (buildinghght > 15 && buildinghght <= 18)
                            {
                                if (roadwidth > 0 && roadwidth <= 12)
                                {
                                    if (fsetback != 3 || rsetback != 7 || lsetback != 7 || rearsetback != 7)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 12 && roadwidth <= 18)
                                {
                                    if (fsetback != 4 || rsetback != 7 || lsetback != 7 || rearsetback != 7)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 18 && roadwidth <= 24)
                                {
                                    if (fsetback != 5 || rsetback != 7 || lsetback != 7 || rearsetback != 7)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth >= 24 && roadwidth <= 30)
                                {
                                    if (fsetback != 6 || rsetback != 7 || lsetback != 7 || rearsetback != 7)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                                if (roadwidth > 30)
                                {
                                    if (fsetback != 7.5 || rsetback != 7 || lsetback != 7 || rearsetback != 7)
                                    {
                                        plterrcause += "-Building Setback is out of rule.";
                                        plterrcnt++;
                                        objlst.Add(Plugin.aplotpline[0].ObjectId);
                                    }
                                }
                            }
                        }

                        ruleError err_plt = new ruleError();
                        err_plt.errorCnt = plterrcnt;
                        err_plt.lyrname = "_Plot";
                        err_plt.errcause = plterrcause;
                        err_plt.objIdlist = objlst;
                        Commands.errlist.Add(err_plt);
                        break;
                    }
                case "_OrganizedOpenSpace":
                    {
                        double PlotArea = 0, greenArea = 0, openspaceArea = 0;
                        double openspacewidth = 0, length = 0;
                        string openerrcause = "";
                        int openerrcnt = 0;
                        List<ObjectId> objidlst=new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        greenArea = Plugin.aopenspacepline.ElementAt(0).Area;
                        length = Plugin.aopenspacepline.ElementAt(0).Length;
                        foreach (Polyline pl in Plugin.aopenspacepline)
                        {
                            openspaceArea += pl.Area;
                            objidlst.Add(pl.ObjectId);
                        }
                        string openEntname = Plugin.aopenspaceTxt.ElementAt(0).Text;
                        if (openEntname.Contains("Green Strip"))
                            openEntname = "Green Strip";
                        if (openEntname.Contains("Green Belt"))
                            openEntname = "Green Belt";
                        if (openEntname.Contains("Tot lot"))
                            openEntname = "Tot lot";
                        if (openEntname.Contains("OPEN SPACE"))
                            openEntname = "OPEN SPACE";
                        if (openEntname == "Green Strip" || openEntname == "Green Belt")
                            if (300 <= PlotArea || PlotArea <= 4000)
                            {
                                //(length/2-width)*width=area
                                openspacewidth = (length / 2 - Math.Sqrt(Math.Pow(length / 2, 2) - 4 * greenArea)) / 2;
                                if (openspacewidth < 1 || openspacewidth > 2)
                                {
                                    openerrcause += "-Green Strip or Green Belt is out of rule.";
                                    openerrcnt++;
                                }
                            }
                        if (openEntname == "Tot lot" || openEntname == "OPEN SPACE")
                        {
                            if (750 <= PlotArea || PlotArea <= 4000)
                            {
                                if (openspaceArea > PlotArea * 0.05)
                                {
                                    openerrcause += "-Open space or Tot lot is out of rule.";
                                    openerrcnt++;
                                }
                            }
                            if (PlotArea > 4000)
                            {
                                if (openspaceArea > PlotArea * 0.1)
                                {
                                    openerrcause += "-Open space or Tot lot is out of rule.";
                                    openerrcnt++;
                                }
                            }
                            if (PlotArea > 40000 && Plugin.projtypestate == 3)
                            {
                                if (openspaceArea > PlotArea * 0.075)
                                {
                                    openerrcause += "-Open space or Tot lot is out of rule.";
                                    openerrcnt++;
                                }
                            }
                        }
                        ruleError err_orgopen = new ruleError();
                        err_orgopen.errorCnt = openerrcnt;
                        err_orgopen.lyrname = "_OrganizedOpenSpace";
                        err_orgopen.errcause = openerrcause;
                        err_orgopen.objIdlist = objidlst;
                        Commands.errlist.Add(err_orgopen);
                        break;
                    }
                case "_Parking":
                    {
                        double allParkArea = 0, ruleParkarea = 0, netBua = 0, Parkflrarea = 0;
                        double allVshaftarea = 0, allVoidarea = 0, allAccusearea = 0;
                        Polyline rangePl = null;
                        Polyline ParkFlrPl = null;
                        foreach (Polyline pl in Plugin.aParkingpline)
                        {
                            if (allParkArea < pl.Area)
                            {
                                rangePl = pl;
                                allParkArea = pl.Area;
                            }
                        }
                        foreach (Polyline pl in Plugin.aParkingpline)
                        {
                            Point3d ptleftplt = Commands.Getleft(pl);
                            Point3d ptrightplt = Commands.Getright(pl);
                            Point3d pttopplt = Commands.Gettop(pl);
                            Point3d ptbottomplt = Commands.Getbottom(pl);
                            Point3d ptUpperLeft = new Point3d(ptleftplt.X, pttopplt.Y, 0);
                            Point3d ptBottomRight = new Point3d(ptrightplt.X, ptbottomplt.Y, 0);
                            if (!IsInPolyLine(rangePl, ptUpperLeft, ptBottomRight))
                            {
                                allParkArea += pl.Area;
                            }
                        }
                        foreach (Polyline pl in Plugin.aFloorpline)
                        {
                            Point3d ptleftplt = Commands.Getleft(rangePl);
                            Point3d ptrightplt = Commands.Getright(rangePl);
                            Point3d pttopplt = Commands.Gettop(rangePl);
                            Point3d ptbottomplt = Commands.Getbottom(rangePl);
                            Point3d ptUpperLeft = new Point3d(ptleftplt.X, pttopplt.Y, 0);
                            Point3d ptBottomRight = new Point3d(ptrightplt.X, ptbottomplt.Y, 0);
                            if (IsInPolyLine(pl, ptUpperLeft, ptBottomRight))
                                ParkFlrPl = pl;
                        }
                        Parkflrarea = ParkFlrPl.Area;
                        foreach (Polyline pl in Plugin.aVShaftpline)
                        {
                            allVshaftarea += pl.Area;
                        }
                        foreach (Polyline pl in Plugin.aVoidpline)
                        {
                            allVoidarea += pl.Area;
                        }
                        foreach (Polyline pl in Plugin.aAccusepline)
                        {
                            allAccusearea += pl.Area;
                        }
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
                            if(mroadwidth==0)
                            {
                                strerrcause += "-MainRoad Width is worng.";
                                errcnt++;
                                objidlst.Add(Plugin.amroadNmTxt[0].ObjectId);
                            }                                
                        }
                        else
                        {
                            strerrcause += "-No Text in the MainRoad Layer.";
                            errcnt++;
                            objidlst.Add(Plugin.amroadpline[0].ObjectId);
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
                        double rmplength = 0, rmpwidth = 0, rmpht = 0, plinth = 0;
                        double glY = Plugin.aGllvlpline.ElementAt(0).GetPoint3dAt(0).Y;
                        Polyline firstcellarpl = null, basefirstfloorpl = null;
                        DBText basefirsttxt = null;
                        Point3d tmpTop = new Point3d(0, 0, 0);
                        List<Polyline> plcellarlist = new List<Polyline>();
                        List<ObjectId> objidlst = new List<ObjectId>();
                        string rmperrcause = "";
                        int rmperrcnt = 0;
                        foreach (DBText txt in Plugin.aFloorTxt)
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
                            if (IsInPolyLine(pl, basefirsttxt.Position,
                                new Point3d(basefirsttxt.Position.X + basefirsttxt.Bounds.Value.MaxPoint.X - basefirsttxt.Bounds.Value.MinPoint.X
                                , basefirsttxt.Position.Y + basefirsttxt.Height, 0)))
                            {
                                basefirstfloorpl = pl;
                                break;
                            }
                        }
                        MessageBox.Show("Ramp Step");
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
                        foreach (MText txt in Plugin.arampTxt)
                        {
                            Point3d ptstart = txt.Location;
                            Point3d ptend = new Point3d(txt.Location.X + txt.Width, txt.Location.Y + txt.Height, 0);
                            string str = txt.Contents;
                            str = str.ToLower();
                            int lengthpos = str.IndexOf(" mt. long ");
                            string strlength = str.Substring(0, lengthpos);
                            rmplength = Convert.ToDouble(strlength);
                            str = str.Remove(0, lengthpos + 10);
                            int widehpos = str.IndexOf(" mt. high ");
                            string strwide = str.Substring(0, widehpos);
                            rmpht = Convert.ToDouble(strwide);
                            str = str.Remove(0, widehpos + 10);
                            int htpos = str.IndexOf(" mt. wide ");
                            string strht = str.Substring(0, htpos);
                            rmpwidth = Convert.ToDouble(strht);
                            if (IsInPolyLine(basefirstfloorpl, ptstart, ptend))
                            {
                                double cellarht = Commands.Gettop(firstFlrINSeccellarpl).Y - Commands.Getbottom(firstFlrINSeccellarpl).Y;
                                if (rmplength >= (cellarht - plinth) * 8)
                                {
                                    rmperrcause += "-Ramp Length is out of rule.";
                                    rmperrcnt++;
                                    objidlst.Add(txt.ObjectId);
                                }
                            }
                            else
                            {
                                double cellarht = Commands.Gettop(firstFlrINSeccellarpl).Y - Commands.Getbottom(firstFlrINSeccellarpl).Y;
                                if (rmplength >= (cellarht) * 8)
                                {
                                    rmperrcause += "-Ramp is out of rule.";
                                    rmperrcnt++;
                                    objidlst.Add(txt.ObjectId);
                                }
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
                        double PlotArea = 0;
                        string stairerrcause = "";
                        int errcnt = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        foreach (Polyline pl in Plugin.aStairpline)
                        {
                            Point3d ptleftplt = Commands.Getleft(pl);
                            Point3d ptrightplt = Commands.Getright(pl);
                            Point3d pttopplt = Commands.Gettop(pl);
                            Point3d ptbottomplt = Commands.Getbottom(pl);
                            double width1 = Math.Abs(ptleftplt.X - ptrightplt.X);
                            double width2 = Math.Abs(pttopplt.Y - ptbottomplt.Y);
                            if(pl.Closed)
                            {
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
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl2 in Plugin.aplotpline)
                            PlotArea += pl2.Area;
                        foreach (Polyline pl in Plugin.aPassagepline)
                        {
                            Point3d ptleftplt = Commands.Getleft(pl);
                            Point3d ptrightplt = Commands.Getright(pl);
                            Point3d pttopplt = Commands.Gettop(pl);
                            Point3d ptbottomplt = Commands.Getbottom(pl);
                            double width1 = Math.Abs(ptleftplt.X - ptrightplt.X);
                            double width2 = Math.Abs(pttopplt.Y - ptbottomplt.Y);
                            if (PlotArea <= 300)
                            {
                                if (width1 != 2 && width2 != 2)
                                {
                                    Passageerrcause += " -Passage Width is out of rule";
                                    errcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (300 <= PlotArea || PlotArea <= 4000)
                            {
                                if (width1 != 2.5 && width2 != 2.5)
                                {
                                    Passageerrcause += " -Passage Width is out of rule";
                                    errcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (PlotArea >= 4000)
                            {
                                if (width1 != 3 && width2 != 3)
                                {
                                    Passageerrcause += " -Passage Width is out of rule";
                                    errcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError err_passage = new ruleError();
                        err_passage.errorCnt = errcnt;
                        err_passage.lyrname = "_Passage";
                        err_passage.errcause = Passageerrcause;
                        err_passage.objIdlist=objidlst;
                        Commands.errlist.Add(err_passage);
                        break;
                    }
                case "_Room":
                    {
                        int windowerrcntrm = 0;
                        string roomerrcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pln in Plugin.aroompline)
                        {
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
                                            Commands.roomerrcause.Add("This room does not satisfy ventilation requirement.");
                                            roomerrcause = roomerrcause + "-" + "This room does not satisfy ventilation requirement.";
                                            objidlst.Add(pln.ObjectId);
                                            break;
                                        }
                                    }
                                }
                            }
                            bool bIntext = false;
                            foreach (MText inst in Plugin.aroomNmTxt)
                            {
                                bIntext = IsInPolyLine(pln, inst.GeometricExtents.MinPoint, inst.GeometricExtents.MaxPoint);
                                if (bIntext)
                                    break;
                            }
                            if(!bIntext)
                            {
                                windowerrcntrm++;
                                Commands.roomerrcause.Add("This room does not contain roomname.");
                                roomerrcause = roomerrcause + "-" + "This room does not contain roomname or roomname Text range is out of RoomPolyline.";
                                objidlst.Add(pln.ObjectId);
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
                        foreach (Polyline pln in Plugin.aroompline)
                        {
                            double rarea = pln.Area;
                            foreach (Polyline dpl in Plugin.adoorpline)
                            {
                                for (int i = 0; i < dpl.NumberOfVertices; i++)
                                {
                                    Point3d pt3 = dpl.GetPoint3dAt(i);
                                    if (IsPointOnPolyline(pln, pt3))
                                    {
                                        double darea = 0;
                                        foreach (ProsoftAcPlugin.doorrule rule in ProsoftAcPlugin.Commands.adoorrule)
                                        {
                                            if (rule.pl == dpl)
                                            {
                                                darea = rule.width * rule.height;
                                            }
                                        }

                                        if (darea < (rarea * 0.1))
                                        {
                                            doorerrcnt++;
                                            doorerrcause += "-This door is little than 10 % area of room";
                                            objidlst.Add(dpl.ObjectId);
                                            break;
                                        }
                                    }
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
                        int splayerrcnt = 0;
                        string splayerrcause = "";
                        double mroadwidth = 0;
                        List<ObjectId> objidlst = new List<ObjectId>();
                        if (Plugin.amroadNmTxt.Count != 0)
                        {
                            string widthtxt = Commands.GetMTextContent(Plugin.amroadNmTxt.ElementAt(0));
                            mroadwidth = Commands.GetRoadWidth(widthtxt);
                        }
                        foreach (Polyline pl in Plugin.asplaypline)
                        {
                            Point3d ptleft = Commands.Getleft(pl);
                            Point3d ptright = Commands.Getright(pl);
                            Point3d pttop = Commands.Gettop(pl);
                            Point3d ptbottom = Commands.Getbottom(pl);
                            double width = ptleft.X - ptright.X;
                            if (mroadwidth < 12)
                            {
                                if (width != 3)
                                {
                                    splayerrcause += "-splay provision should be 3mX3m";
                                    splayerrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (mroadwidth >= 12 && mroadwidth < 24)
                            {
                                if (width != 4.5)
                                {
                                    splayerrcause += "-splay provision should be 4.5mX4.5m";
                                    splayerrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                            if (mroadwidth > 24)
                            {
                                if (width != 6)
                                {
                                    splayerrcause += "-splay provision should be 6mX6m";
                                    splayerrcnt++;
                                    objidlst.Add(pl.ObjectId);
                                }
                            }
                        }
                        ruleError errsplay = new ruleError();
                        errsplay.errorCnt = splayerrcnt;
                        errsplay.lyrname = "_Splay";
                        errsplay.errcause = splayerrcause;
                        errsplay.objIdlist = objidlst;
                        Commands.errlist.Add(errsplay);
                        break;

                    }
                case "_VentilationShaft":
                    {
                        double width1 = 0, width2 = 0;
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        foreach (Polyline pl in Plugin.aVenShaftpline)
                        {
                            Point3d ptleft = Commands.Getleft(pl);
                            Point3d ptright = Commands.Getright(pl);
                            Point3d pttop = Commands.Gettop(pl);
                            Point3d ptbottom = Commands.Getbottom(pl);
                            width1 = Math.Abs(ptleft.X - ptright.X);
                            width2 = Math.Abs(pttop.Y - ptbottom.Y);
                            if (pl.Area < 9)
                                if (width1 < 2 || width2 < 2)
                                {
                                    errcnt++;
                                    errcause += "-Ventilation side width is smaller than 2.";
                                    objidlst.Add(pl.ObjectId);
                                }
                            if (pl.Area > 9 && pl.Area < 25)
                                if (width1 < 3 || width2 < 3)
                                {
                                    errcnt++;
                                    errcause += "-Ventilation side width is smaller than 3.";
                                    objidlst.Add(pl.ObjectId);
                                }    
                        }
                        ruleError err = new ruleError();
                        err.errorCnt = errcnt;
                        err.lyrname = "_VentilationShaft";
                        err.errcause = errcause;
                        err.objIdlist=  objidlst;
                        Commands.errlist.Add(err);
                        break;
                    }
                case "_Amenity":
                    {
                        double amenityarea = 0, plotarea = 0;
                        int errcnt = 0;
                        string errcause = "";
                        List<ObjectId> objidlst = new List<ObjectId>();
                        if(Plugin.aAmenitypline.Count!=0)
                        {
                            foreach (Polyline pl in Plugin.aplotpline)
                                plotarea += pl.Area;
                            foreach (Polyline pl in Plugin.aAmenitypline)
                            {
                                amenityarea += pl.Area;
                                objidlst.Add(pl.ObjectId);
                            }
                            if (plotarea < 20235 && 0 < plotarea)
                                if (amenityarea < plotarea * 0.03)
                                {
                                    errcnt++;
                                    errcause += "-Amenity Area is too small";
                                }
                            if (plotarea >= 20235)
                                if (amenityarea < plotarea * 0.05)
                                {
                                    errcnt++;
                                    errcause += "-Amenity Area is too small";
                                }
                            ruleError err = new ruleError();
                            err.errorCnt = errcnt;
                            err.lyrname = "_Amenity";
                            err.errcause = errcause;
                            err.objIdlist = objidlst;
                            Commands.errlist.Add(err);
                        }                        
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
                                    buferrcause += "BufferZone is not closed with WaterBodies.";
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
                                    buferrcause += "BufferZone is not closed with ElectricLine.";
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
                                    buferrcause += "BufferZone is not closed with ElectricLine.";
                                    buferrcnt++;
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
                if (seg != null)
                {
                    isOn = seg.IsOn(pt);
                    if (isOn)
                        break;
                }
            }
            return isOn;
        }
        public static bool IsInPolyLine(Polyline pl, Point3d ptbegin, Point3d ptend)        //this returns rect that includes ptbegin and ptend is in polyline or not.
        {
            bool bresult = false;
            if (pl.GeometricExtents.MinPoint.X <= ptbegin.X && pl.GeometricExtents.MaxPoint.X >= ptend.X && pl.GeometricExtents.MinPoint.Y <= ptbegin.Y && pl.GeometricExtents.MaxPoint.Y >= ptend.Y)
            {
                bresult = true;
            }
            else
                bresult = false;
            return bresult;            
        }
        public static bool checkTwoPlineTouch(Polyline pl1, Polyline pl2)            // this checks two polyline is close.
        {
            bool btch=false;

            int tchcnt = 0;
            for(int i=0;i<pl2.NumberOfVertices;i++)
            {
                if (IsPointOnPolyline(pl1, pl2.GetPoint3dAt(i)))
                {
                    tchcnt++;
                }
            }
            if (tchcnt >= 2)
                btch = true;
            return btch;
        }              
        public static void Swap(double first, double second)
        {
            double temp;
            temp = first;
            first = second;
            second = temp;
        }
    }
}
