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
        /// Example method to count components connected to a specified net by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountComponentsConnectedToNet(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the net with the specified name
            INet net = step.GetNet(netName);
            if (net != null)
            {
                HashSet<string> connectedComponentRefs = new HashSet<string>();
                foreach (INetObject pinInfo in net.ComponentList)
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    connectedComponentRefs.Add(pinInfo.ICMP.Ref);
                }
                return "The total number of components connected to the net '" + netName + "' is " + connectedComponentRefs.Count + ".";
            }
            else
            {
                return "The net '" + netName + "' is not found in the current step.";
            }
        }
        
    }
}
