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
        /// Example method to check if a specified drill layer is a laser drill layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckIfLayerIsLaserDrill(IPCBIWindow pcbi, IStep step, string drillLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            MatrixLayerType layerType = matrix.GetMatrixLayerType(drillLayer);
            if (layerType == MatrixLayerType.Unknown)
            {
                return "The drill layer '" + drillLayer + "' is not found in the current job.";
            }
            else
            {
                bool isLaserDrill = matrix.IsSBUDrill(drillLayer);
                return isLaserDrill ? "Yes, the layer '" + drillLayer + "' is a laser drill layer." : "No, the layer '" + drillLayer + "' is not a laser drill layer.";
            }
        }
        
    }
}
