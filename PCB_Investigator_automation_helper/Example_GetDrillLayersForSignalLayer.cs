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
        /// Example method to retrieve drill layers intersecting a specified signal layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetDrillLayersForSignalLayer(IPCBIWindow pcbi, IStep step, string signalLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get the signal layer with the specified name
            MatrixLayerType layerType = matrix.GetMatrixLayerType(signalLayer);
            if (layerType == MatrixLayerType.Unknown)
            {
                return "The signal layer '" + signalLayer + "' is not found in the current job.";
            }
            else
            {
                // Get the drill layers that intersect the signal layer
                List<string> drillLayers = matrix.GetAllDrillLayersForThisLayer(signalLayer);
                return "The drill layers that go through the signal layer '" + signalLayer + "' are: " + string.Join(", ", drillLayers);
            }
        }
        
    }
}
