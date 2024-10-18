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
using System.Windows.Input;
using ProsoftAcPlugin;

namespace NBCLayers
{
    public class RuleCheckControlVariables
    {
        public static TreeView trv;
        public static TextBox tb;
        public static void AddingErrors()
        {
            int allErrCnt = 0;
            foreach (ruleError re in ProsoftAcPlugin.Commands.errlist)
            {
                if (re.errorCnt != 0)
                {
                    TreeNode node = new TreeNode(re.lyrname);
                    trv.Nodes.Add(node);
                    for (int i = 0; i < re.errorCnt; i++)
                    {
                        TreeNode childnode = new TreeNode(re.lyrname + "--" + i.ToString());
                        node.Nodes.Add(childnode);
                    }
                    allErrCnt++;
                }
            }
            if (allErrCnt == 0)
                tb.Text = "There are no Errors in this drawing.";
        }
    }
}
