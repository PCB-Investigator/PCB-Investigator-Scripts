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
        /// Example method to count components with more than a specified number of pins by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountComponentsWithMoreThanSpecifiedPins(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, int pinThreshold)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Initialize the count of components with more than the specified number of pins
            int count = 0;
        
            // Iterate through all components in the current step
            foreach (var cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (cmp.GetPinCount() > pinThreshold)
                {
                    count++;
                }
            }
        
            // Return the count of components with more than the specified number of pins or a message if no components were found
            return count > 0
                ? "There are " + count + " components with more than " + pinThreshold + " pins."
                : "No components have more than " + pinThreshold + " pins.";
        }
        
    }
}
