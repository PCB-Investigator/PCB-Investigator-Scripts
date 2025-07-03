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
        /// Example method to check the existence of a specific net in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckNetExistence(IPCBIWindow pcbi, IStep step, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the net named 'netName' exists in the current step
            return step.GetNet(netName) != null
                ? $"Yes, the net '{netName}' exists in this step."
                : $"No, the net '{netName}' is not found in this step.";
        }
        
    }
}
