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
        /// Example method to retrieve and list all board layers and their types by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ListBoardLayersAndTypes(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get all board layers and their types
            List<string> boardLayers = matrix.GetAllBoardLayerNames(ToLower: false);
            if (boardLayers.Count == 0)
            {
                return "No board layers are found in the current job.";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var layerName in boardLayers)
            {
                sb.AppendLine("Layer: " + layerName + ", Type: " + matrix.GetMatrixLayerType(layerName).ToString());
            }
            return sb.ToString();
        }
        
    }
}
