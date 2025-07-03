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
        /// Example method to check the polarity of a specified layer in the current job by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckLayerPolarity(IPCBIWindow pcbi, IStep step, string layerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get the polarity of the specified layer
            MatrixLayerPolarity polarity = matrix.GetMatrixLayerPolarity(layerName);
            if (polarity == MatrixLayerPolarity.Unknown)
            {
                return "The layer '" + layerName + "' is not found in the current job.";
            }
            else
            {
                return "The polarity of the layer '" + layerName + "' is '" + polarity.ToString() + "'.";
            }
        }
        
    }
}
