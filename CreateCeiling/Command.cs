using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CreateCeiling
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,ref string message,ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            XYZ p1 = new XYZ(0, 0, 0)/304.8;
            XYZ p2 = new XYZ(1000, 0, 0)/304.8;
            XYZ p3 = new XYZ(1000, 1000, 0)/304.8;

            IList<CurveLoop> ppp = new List<CurveLoop>();

            CurveLoop lp = new CurveLoop();
            Line line = Line.CreateBound(p1, p2);
            Line line1 = Line.CreateBound(p2, p3);
            Line line2 = Line.CreateBound(p3, p1);

            lp.Append(line);
            lp.Append(line1);
            lp.Append(line2);

            CurveLoop pp = CurveLoop.CreateViaOffset(lp, 500 / 304.8, XYZ.BasisZ);

            ppp.Add(lp);
            ppp.Add(pp);

            CeilingType ct = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Ceilings)
                .OfClass(typeof(CeilingType)).FirstElement() as CeilingType;
            
            using(Transaction trans = new Transaction(doc, "fe"))
            {
                trans.Start("fo");
                Ceiling createcl = Ceiling.Create(doc, ppp, ct.Id, doc.ActiveView.GenLevel.Id);
                trans.Commit();
            }
            

            return Result.Succeeded;
        }
    }
}
