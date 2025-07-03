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
        /// Example method to check for the presence of laser drills in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckForLaserDrills(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
        
            // Iterate through all drill layers to check for laser drills
            foreach (string drillLayer in matrix.GetAllDrillLayerNames())
            {
                if (matrix.IsSBUDrill(drillLayer))
                {
                    return "There are laser drills used in the current design.";
                }
            }
            return "There are no laser drills used in the current design.";
        }
        
    }
}
