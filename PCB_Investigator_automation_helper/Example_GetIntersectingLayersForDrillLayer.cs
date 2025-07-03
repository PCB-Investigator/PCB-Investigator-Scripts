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
        /// Example method to retrieve layers intersecting a specified drill layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetIntersectingLayersForDrillLayer(IPCBIWindow pcbi, IStep step, string drillLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get the drill layer specified
            MatrixLayerType layerType = matrix.GetMatrixLayerType(drillLayer);
            if (layerType == MatrixLayerType.Unknown)
            {
                return "The drill layer '" + drillLayer + "' is not found in the current job.";
            }
            else
            {
                // Get the layers that intersect the drill layer
                List<string> layers = matrix.GetAllLayerWithThisDrills(drillLayer);
                return "The layers where the drill layer '" + drillLayer + "' passes through are: " + string.Join(", ", layers);
            }
        }
        
    }
}
