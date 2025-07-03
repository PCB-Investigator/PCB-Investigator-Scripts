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
        /// Example method to select all plated through-hole (PTH) drills in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectPlatedThroughHoleDrills(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            int count = 0;
            IMatrix matrix = pcbi.GetMatrix();
        
            // Iterate through all drill layers
            foreach (string drillLayer in matrix.GetAllDrillLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Skip laser drill layers
                if (matrix.IsSBUDrill(drillLayer)) continue;
        
                IODBLayer layer = step.GetLayer(drillLayer) as IODBLayer;
                if (layer == null) continue;
        
                // Iterate through all objects in the layer
                foreach (IODBObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject drillObj && drillObj.Type == IObjectType.Pad)
                    {
                        // Check if the object is a round drill
                        if (drillObj.GetSymbol()?.Type == PCBI.Symbol_Type.r)
                        {
                            // Check if the drill is plated
                            IAttributeElement drillTypeAttr = IAttribute.GetStandardAttribute(drillObj, PCBI.FeatureAttributeEnum.drill);
                            if (drillTypeAttr != null && drillTypeAttr.Value?.ToString() == "plated")
                            {
                                // Select the drill
                                drillObj.Select(select: true);
                                count++;
                            }
                        }
                    }
                }
            }
        
            // Update the selection and view if any PTH drills were found
            if (count > 0)
            {
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " PTH drills have been selected in the current design.";
            }
            else
            {
                return "There are no PTH drills in the current design.";
            }
        }
        
    }
}
