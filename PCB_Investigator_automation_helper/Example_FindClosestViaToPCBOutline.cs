using PCB_Investigator.Automation.DBFilter;
using PCB_Investigator.Automation.DesignHistory;
using PCB_Investigator.PCBIWindows;
using PCBI.Automation;
using PCBI.MathUtils;
using PCBI.Plugin.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCB_Investigator_API_Examples
{
/// <summary>
/// This is example code to show ways to use the PCB-Investigator API
/// </summary>
    private static partial class PCB_Investigator_API_Example_Class
    {
        /// <summary>
        /// Example method to find the closest via to the PCB outline by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FindClosestViaToPCBOutline(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get the PCB outline polygon
            IPolyClass boardOutline = step.GetPCBOutlinePoly();
            if (boardOutline == null)
            {
                return "The board outline is not found in the current step.";
            }
        
            double minDistanceMils = double.MaxValue;
            PointD fromMils = PointD.InfPoint, toMils = PointD.InfPoint;
            string closestVia = "None";
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
        
            // Iterate through all drill layers to find the closest via to the PCB outline
            foreach (string drillLayer in matrix.GetAllDrillLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (matrix.IsSBUDrill(drillLayer)) continue; // Skip laser drill layers
                IODBLayer layer = step.GetLayer(drillLayer) as IODBLayer;
                if (layer == null) continue;
        
                foreach (IODBObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject drillObj && drillObj.Type == IObjectType.Pad)
                    {
                        if (drillObj.GetSymbol()?.Type == PCBI.Symbol_Type.r) // Ensure it's a round drill
                        {
                            IAttributeElement drillTypeAttr = IAttribute.GetStandardAttribute(drillObj, PCBI.FeatureAttributeEnum.drill);
                            if (drillTypeAttr != null && drillTypeAttr.Value?.ToString().ToLowerInvariant() == "via")
                            {
                                // Get the polygon outline of the via
                                IPolyClass p1 = drillObj.GetPolygonOutline();
                                if (p1 == null) continue;
        
                                // Calculate the distance between the via and the PCB outline
                                double distanceMils = p1.DistanceTo(boardOutline, ref fromMils, ref toMils); //always in mils
                                if (distanceMils < minDistanceMils)
                                {
                                    PointD midPointMils = drillObj.GetBoundsD().GetMidPoint();
                                    minDistanceMils = distanceMils;
                                    if (showMetricUnit)
                                        closestVia = midPointMils.ConvertToMM().ToShortString(3);
                                    else
                                        closestVia = midPointMils.ToShortString(2);
                                }
                            }
                        }
                    }
                }
            }
        
            if (minDistanceMils == double.MaxValue)
            {
                return "No vias found in the current step.";
            }
        
            return "The via '" + closestVia + "' is closest to the board outline with a distance of " +
                   (showMetricUnit ? IMath.Mils2MM(minDistanceMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                           : minDistanceMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils.");
        }
        
    }
}
