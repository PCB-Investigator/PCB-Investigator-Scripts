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
        /// Example method to count the number of pins connected to a specific net for a given component by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountPinsConnectedToNet(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string componentName, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the component with the specified name
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentName, out ICMPObject cmp))
            {
                int connectedPinCount = 0;
                foreach (IPin pin in cmp.GetPinList())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (pin.GetNetNameOnIPin(Parent: cmp).ToLowerInvariant() == netName.ToLowerInvariant())
                    {
                        connectedPinCount++;
                    }
                }
                return "The total number of pins of the component '" + componentName + "' connected to the net '" + netName + "' is " + connectedPinCount + ".";
            }
            else
            {
                return "The component '" + componentName + "' is not found in the current step.";
            }
        }
        
    }
}
