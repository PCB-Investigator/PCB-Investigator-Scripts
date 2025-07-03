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
        /// Example method to activate the panel step if it exists, by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ActivatePanelStepIfExists(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the current step is already the panel step
            if (!step.IsRootStep)
            {
                return "The current step " + step.Name + " is already the panel step.";
            }
            else
            {
                // Iterate through all steps to find the panel step
                foreach (IStep stepEntry in pcbi.GetStepList())
                {
                    if (!stepEntry.IsRootStep)
                    {
                        // Activate the panel step
                        stepEntry.ActivateStep();
                        return "The panel step " + stepEntry.Name + " has been activated.";
                    }
                }
                return "There is no panel step in the current step.";
            }
        }
        
    }
}
