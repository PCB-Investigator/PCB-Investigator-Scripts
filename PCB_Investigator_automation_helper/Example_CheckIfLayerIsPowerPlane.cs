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
        /// Example method to check if a specified layer is a power plane layer by using the PCB-Investigator API
        /// </summary>
        private static string Example_CheckIfLayerIsPowerPlane(IPCBIWindow pcbi, IStep step, string layerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            MatrixLayerType layerType = matrix.GetMatrixLayerType(layerName);
            if (layerType == MatrixLayerType.Unknown)
            {
                return "The layer '" + layerName + "' is not found in the current job.";
            }
            else
            {
                return layerType == MatrixLayerType.Power_ground ? "Yes, the layer '" + layerName + "' is a power plane layer." : "No, the layer '" + layerName + "' is not a power plane layer.";
            }
        }
        
    }
}
