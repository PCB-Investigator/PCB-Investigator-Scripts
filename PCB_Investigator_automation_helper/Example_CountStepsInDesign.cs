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
        /// Example method to count the number of steps in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountStepsInDesign(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the list of all steps in the current job
            var allSteps = pcbi.GetStepList();
        
            // Return the count of steps
            return "There are " + allSteps.Count + " step(s) in this design.";
        }

        /// <summary>
        /// Example method to count the number of steps in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountStepsInDesign(IPCBIWindow pcbi)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the list of all steps in the current job
            var allSteps = pcbi.GetStepList();
        
            // Return the count of steps
            return "There are " + allSteps.Count + " step(s) in this design.";
        }
        
}
}
