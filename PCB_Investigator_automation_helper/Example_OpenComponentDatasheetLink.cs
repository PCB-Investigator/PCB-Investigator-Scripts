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
        /// Example method to open the datasheet link of a selected component by using the PCB-Investigator API
        /// </summary>
        private static string Example_OpenComponentDatasheetLink(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            List<ICMPObject> selectedCMPs = step.GetSelectedCMPs();
        
            // Check if no components or more than one component is selected
            if (selectedCMPs.Count == 0)
            {
                return "No components are selected in the current step.";
            }
            else if (selectedCMPs.Count > 1)
            {
                return "Please select only one component to open the datasheet link.";
            }
            else
            {
                // Get the datasheet URL of the selected component
                IEDA_PRP prp = IAttribute.GetProperty(selectedCMPs[0], "DATASHEET_URL");
                if (prp != null && !string.IsNullOrWhiteSpace(prp.VALUE_STRING))
                {
                    // Open the datasheet URL
                    PCBI.Automation.ProcessWithShellExecute.Start(prp.VALUE_STRING);
                    return "The datasheet link for the selected component has been opened.";
                }
                else
                {
                    return "The selected component does not have a datasheet link.";
                }
            }
        }

        /// <summary>
        /// Example method to open the datasheet link of a selected component by using the PCB-Investigator API
        /// </summary>
        private static string Example_OpenComponentDatasheetLink(IPCBIWindow pcbi, IStep step, string propertyName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            List<ICMPObject> selectedCMPs = step.GetSelectedCMPs();
        
            // Check if no components or more than one component is selected
            if (selectedCMPs.Count == 0)
            {
                return "No components are selected in the current step.";
            }
            else if (selectedCMPs.Count > 1)
            {
                return "Please select only one component to open the datasheet link.";
            }
            else
            {
                // Get the datasheet URL of the selected component
                IEDA_PRP prp = IAttribute.GetProperty(selectedCMPs[0], propertyName);
                if (prp != null && !string.IsNullOrWhiteSpace(prp.VALUE_STRING))
                {
                    // Open the datasheet URL
                    PCBI.Automation.ProcessWithShellExecute.Start(prp.VALUE_STRING);
                    return "The datasheet link for the selected component has been opened.";
                }
                else
                {
                    return "The selected component does not have a datasheet link.";
                }
            }
        }
        
}
}
