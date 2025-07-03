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
        /// Example method to find the smallest drill size in a specified layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FindSmallestDrillSizeInLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string drillLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            IMatrix matrix = pcbi.GetMatrix();
        
            double smallestDrillMils = double.MaxValue;
        
            // Get the specified 'drill' layer
            IODBLayer layer = step.GetLayer(drillLayer) as IODBLayer;
            if (layer != null)
            {
                // Iterate through all objects in the layer to find the smallest drill size
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject drillObj)
                    {
                        if (drillObj.GetSymbol()?.Type == PCBI.Symbol_Type.r)
                        {
                            double drillSizeMils = drillObj.GetDiameter(); //always in mils
                            if (drillSizeMils < smallestDrillMils)
                            {
                                smallestDrillMils = drillSizeMils;
                            }
                        }
                    }
                }
            }
            else return $"The layer '{drillLayer}' is not found in the current step.";
        
            if (smallestDrillMils < double.MaxValue)
            {
                if (showMetricUnit)
                {
                    return "The smallest drilling size in the layer '" + drillLayer + "' is "
                           + IMath.Mils2MM(smallestDrillMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.";
                }
                else
                {
                    return "The smallest drilling size in the layer '" + drillLayer + "' is "
                           + smallestDrillMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils.";
                }
            }
            else
            {
                return "There are no recognized drill objects in the layer '" + drillLayer + "'.";
            }
        }
        
    }
}
