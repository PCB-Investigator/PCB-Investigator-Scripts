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
        /// Example method to count components on the top and bottom sides of a PCB in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountComponentsOnSides(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            int countTop = 0, countBot = 0;
            // Iterate through all components in the current step
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Check if the component is placed on the top or bottom side of the PCB
                if (cmp.PlacedTop)
                {
                    countTop++;
                }
                else
                {
                    countBot++;
                }
            }
            return "There are " + countTop + " components on the top side and " + countBot + " components on the bot side of the PCB in the current step.";
        }
        
    }
}
