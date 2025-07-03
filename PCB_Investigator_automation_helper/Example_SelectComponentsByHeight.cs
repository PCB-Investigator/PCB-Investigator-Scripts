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
        /// Example method to select components with a height less than the specified threshold by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentsByHeight(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, double heightThresholdMils)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            int count = 0;
        
            // Iterate through all components to find those taller than the specified threshold
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                double heightMils = cmp.CompHEIGHT;  //always in mils
                if (heightMils < heightThresholdMils)
                {
                    // Select the component
                    cmp.Select(select: true);
                    count++;
                }
            }
        
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (count > 0)
            {
                return "All " + count + " components with a height greater than " + heightThresholdMils + "mm have been selected in the current step.";
            }
            else
            {
                return "There are no components with a height greater than " + heightThresholdMils + "mm in the current step.";
            }
        }

        /// <summary>
        /// Example method to select components with a height less than a specified threshold by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentsByHeight(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, double heightThresholdMils)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            int count = 0;
        
            // Iterate through all components to find those taller than the specified threshold
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                double heightMils = cmp.CompHEIGHT;  //always in mils
                if (heightMils < heightThresholdMils)
                {
                    // Select the component
                    cmp.Select(select: true);
                    count++;
                }
            }
        
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (count > 0)
            {
                return "All " + count + " components with a height greater than " + (heightThresholdMils / 1000) + "mm have been selected in the current step.";
            }
            else
            {
                return "There are no components with a height greater than " + (heightThresholdMils / 1000) + "mm in the current step.";
            }
        }

        /// <summary>
        /// Example method to select components with a height less than a specified threshold by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentsByHeight(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, double heightThresholdMils)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            int count = 0;
        
            // Iterate through all components to find those shorter than the specified height
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                double heightMils = cmp.CompHEIGHT;  //always in mils
                if (heightMils < heightThresholdMils)
                {
                    // Select the component
                    cmp.Select(select: true);
                    count++;
                }
            }
        
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (count > 0)
            {
                return "All " + count + " components with a height less than " + heightThresholdMils + " mils have been selected in the current step.";
            }
            else
            {
                return "There are no components with a height less than " + heightThresholdMils + " mils in the current step.";
            }
        }
        
}
}
