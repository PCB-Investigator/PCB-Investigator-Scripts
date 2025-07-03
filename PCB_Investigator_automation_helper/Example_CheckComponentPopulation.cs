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
        /// Example method to check if a specific component is populated by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckComponentPopulation(IPCBIWindow pcbi, IStep step, string componentName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the component with the specified name
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentName, out ICMPObject cmp))
            {
                // Check if the component is populated
                bool isPopulated = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_ignore) == null && IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.no_pop) == null;
                return isPopulated ? "Yes, the component '" + componentName + "' is populated." : "No, the component '" + componentName + "' is not populated.";
            }
            else
            {
                return "The component '" + componentName + "' is not found in the current step.";
            }
        }
        
    }
}
