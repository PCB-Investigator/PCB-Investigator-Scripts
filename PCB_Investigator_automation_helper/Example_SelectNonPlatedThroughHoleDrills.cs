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
        /// Example method to select all non-plated through-hole (NPTH) drills in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNonPlatedThroughHoleDrills(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Initialize the count of NPTH drills
            int count = 0;
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Iterate through all drill layers
            foreach (string drillLayer in matrix.GetAllDrillLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (matrix.IsSBUDrill(drillLayer)) continue; // Skip laser drill layers
                IODBLayer layer = step.GetLayer(drillLayer) as IODBLayer;
                if (layer == null) continue;
                // Iterate through all objects in the layer
                foreach (IODBObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject drillObj && drillObj.Type == IObjectType.Pad)
                    {
                        if (drillObj.GetSymbol()?.Type == PCBI.Symbol_Type.r)
                        {
                            // Check if the drill is non-plated
                            IAttributeElement drillTypeAttr = IAttribute.GetStandardAttribute(drillObj, PCBI.FeatureAttributeEnum.drill);
                            if (drillTypeAttr != null && drillTypeAttr.Value?.ToString() == "non_plated")
                            {
                                // Select the drill
                                drillObj.Select(select: true);
                                count++;
                            }
                        }
                    }
                }
            }
            // Check if any NPTH drills were selected
            if (count > 0)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " NDK drills have been selected in the current design.";
            }
            else
            {
                return "There are no NDK drills in the current design.";
            }
        }
        
    }
}
