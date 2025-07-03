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
        /// Example method to select a specific component by reference in the current step using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentByReference(IPCBIWindow pcbi, IStep step, string componentReference)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the component exists in the current step
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentReference, out ICMPObject cmp))
            {
                // Select the component
                cmp.Select(select: true);
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return $"The component {componentReference} has been selected in the current step.";
            }
            else
            {
                return $"The component {componentReference} is not found in the current step.";
            }
        }
        
    }
}
