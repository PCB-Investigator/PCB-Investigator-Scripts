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
        /// Example method to change the color of a specified component to green by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ChangeComponentColorToGreen(IPCBIWindow pcbi, IStep step, string componentReference)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Check if the specified component exists in the current step
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentReference, out ICMPObject cmp))
            {
                // Change the color of the component to green
                cmp.ObjectColor = Color.Green;
                // Update the view
                pcbi.UpdateView(NeedFullRedraw: true);
                return $"The color of the component {componentReference} has been changed to green.";
            }
            else
            {
                return $"The component {componentReference} is not found in the current step.";
            }
        }
        
    }
}
