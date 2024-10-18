using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Windows.Data;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProsoftAcPlugin
{
    public class windowrule
    {
        public Polyline pl;
        public float width;
        public float height;
        public float depth;
        public ObjectId objid;
        public Handle hnd;
        public string kind;
    }
    public class doorrule
    {
        public Polyline pl;
        public float width;
        public float height;
        public float depth;
        public ObjectId objid;
        public Handle hnd;
        public string kind;
    }
    public class Gaterule
    {
        public Polyline pl;
        public float width;
        public float height;
        public float depth;
        public ObjectId objid;
        public Handle hnd;
        public string kind;
    }
    public class roomrule
    {
        public Polyline pl;
        public double width;
        public double height;
        public ObjectId objid;
        public Handle hnd;
    }
    public class mroadrule
    {
        public Polyline pl;
        public float width;
        public float height;
        public ObjectId objid;
        public Handle hnd;
    }
    public class intRoadrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class plotrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class indvSubPlotrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class openspacerule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class amenityrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aMortrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aSplayrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class abufferzonerule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class awaterbodyrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class awaterlinerule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aelectriclinerule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aLeftownersrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aSurroundtoAuthorityrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aCmpWallrule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aelinerule
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aFlorInSec
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aStair
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class aPassage
    {
        public Polyline pl;
        public float width;
        public float height;
    }
    public class ruleError
    {
        public string lyrname;
        public int errorCnt;
        public string errcause;
        public List<ObjectId> objIdlist = null;
    }
    public class JsonItems
    {
        public string layer { get; set; }
        public string OId { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double depth { get; set; }
        public string hndle { get; set; }
        public string projtype { get; set; }
        public string bpass { get; set; }
        public string kind { get; set; }
        public ResultBuffer ToResultBuffer()
        {
            return new ResultBuffer(
                new TypedValue(1, layer),
                new TypedValue(5, OId),
                new TypedValue(40, width),
                new TypedValue(40, height),
                new TypedValue(40, depth),
                new TypedValue(5, hndle),
                new TypedValue(5, projtype),
                new TypedValue(5, bpass),
                new TypedValue(5, kind));
        }
    }
}
