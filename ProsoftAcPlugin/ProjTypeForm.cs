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
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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

namespace ProsoftAcPlugin
{
    public partial class ProjTypeForm : Form
    {
        public ProjTypeForm()
        {
            InitializeComponent();
        }

        private void Btn_ok_Click(object sender, EventArgs e)
        {
            Plugin.projtypestate = (uint)cmb_projtype.SelectedIndex;
            WritetoNODProjType();
            this.Close();
        }
        public static void WritetoNODProjType()
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
                        trans.Commit();
                    }
                }
            }catch(Exception e)
            {
                //Application.ShowAlertDialog(e.ToString());
            }
                            
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmb_projtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plugin.projtypestate = (uint)cmb_projtype.SelectedIndex;
        }
    }
}
