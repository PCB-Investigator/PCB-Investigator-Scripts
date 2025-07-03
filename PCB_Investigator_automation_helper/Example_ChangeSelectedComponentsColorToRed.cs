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
        /// Example method to change the color of selected components to red by using the PCB-Investigator API
        /// </summary>
        private static string Example_ChangeSelectedComponentsColorToRed(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            int count = 0;
            // Iterate through all selected components and change their color to red
            foreach (ICMPObject cmp in step.GetSelectedCMPs())
            {
                cmp.ObjectColor = Color.Red;
                count++;
            }
            if (count > 0)
            {
                // Update the view
                pcbi.UpdateView(NeedFullRedraw: true);
                return "The color of all " + count + " selected components has been changed to red.";
            }
            else
            {
                return "There are no selected components in the current step.";
            }
        }
        
    }
}
