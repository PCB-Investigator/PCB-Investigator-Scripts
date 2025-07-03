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
        /// Example method to retrieve the net name of a specific pin on a component by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetPinNetName(IPCBIWindow pcbi, IStep step, string componentName, string pinNumber)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Get the component with the specified name
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentName, out ICMPObject cmp))
            {
                IPin pin = cmp.GetPinList().Where(pin => pin.PinNumber == pinNumber).FirstOrDefault();
                if (pin != null)
                {
                    return "The pin " + pinNumber + " of the component '" + componentName + "' belongs to the net '" + pin.GetNetNameOnIPin(Parent: cmp) + "'.";
                }
                else
                {
                    return "The pin " + pinNumber + " is not found in the component '" + componentName + "'.";
                }
            }
            else
            {
                return "The component '" + componentName + "' is not found in the current step.";
            }
        }
        
    }
}
