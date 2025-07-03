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
        /// Example method to select components with vias underneath them by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentsWithVias(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Dictionary to store component polygons
            Dictionary<ICMPObject, IPolyClass> componentPolygons = new Dictionary<ICMPObject, IPolyClass>();
            // List to store components to be selected
            List<ICMPObject> componentsToSelect = new List<ICMPObject>();
        
            // Cache component polygons to avoid redundant calculations
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                IPolyClass cmpOutline = cmp.GetPolygonOutline(IncludePins: false);
                if (cmpOutline != null)
                {
                    componentPolygons[cmp] = cmpOutline;
                }
            }
        
            // Iterate over all drill layers to find vias
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
                                // Get the bounds and polygon outline of the via
                                RectangleD viaBoundsMils = drillObj.GetBoundsD();  //always in mils
                                IPolyClass viaPoly = drillObj.GetPolygonOutline();
        
                                // Check against cached component polygons
                                foreach (var cmpEntry in componentPolygons)
                                {
                                    if (cmpEntry.Key.GetBoundsD().IntersectsWith(viaBoundsMils) &&
                                        (cmpEntry.Value.DoesIntersect(viaPoly) || cmpEntry.Value.IsAnyPointOfSecondObjectIncluded(viaPoly))) // Check for overlap or if drill is situated inside component polygon
                                    {
                                        componentsToSelect.Add(cmpEntry.Key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        
            if (componentsToSelect.Count > 0)
            {
                // Clear the current selection
                step.ClearSelection(FireEvents: false);
                // Select the components with vias underneath them
                foreach (var cmp in componentsToSelect)
                {
                    cmp.Select(select: true);
                }
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "Marked " + componentsToSelect.Count + " components that have vias underneath them.";
            }
            else
            {
                return "No components were found with vias underneath them.";
            }
        }
        
    }
}
