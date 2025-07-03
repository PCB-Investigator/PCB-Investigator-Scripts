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
        /// Example method to calculate the total number of vias in a PCB design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CalculateTotalVias(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            int totalVias = 0;
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
                                totalVias++;
                            }
                        }
                    }
                }
            }
            return "The total number of vias in the design is " + totalVias + ".";
        }
        
    }
}
