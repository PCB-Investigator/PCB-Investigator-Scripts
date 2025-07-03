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
        /// Example method to retrieve the name of the bottom component layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetBottomComponentLayerName(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            string botComponentLayer = matrix.GetBotComponentLayer();
            if (string.IsNullOrWhiteSpace(botComponentLayer))
            {
                return "The bottom component layer is not found in the current job.";
            }
            return "The name of the bottom component layer is '" + botComponentLayer + "'.";
        }
        
    }
}
