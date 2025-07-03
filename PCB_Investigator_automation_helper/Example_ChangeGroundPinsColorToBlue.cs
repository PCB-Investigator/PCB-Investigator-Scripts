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
        /// Example method to change the color of all ground pins to blue by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ChangeGroundPinsColorToBlue(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, Color pinColor)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            int count = 0;
        
            // Iterate through all nets to find ground nets
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                string netNameLower = net.NetName.ToLowerInvariant();
                if (netNameLower.Contains("gnd") || netNameLower.Contains("ground"))
                {
                    // Iterate through all pins in the ground net and color them blue
                    foreach (INetObject pinInfo in net.ComponentList)
                    {
                        IPin pin = pinInfo.GetIPin();
                        if (pin != null)
                        {
                            pin.SetPinColor(pinColor: pinColor, Parent: pinInfo.ICMP);
                            count++;
                        }
                    }
                }
            }
        
            if (count > 0)
            {
                // Update the view
                pcbi.UpdateView(NeedFullRedraw: true);
                return "The color of all " + count + " ground pins has been changed to blue.";
            }
            else
            {
                return "There are no ground pins in the current step.";
            }
        }
        
    }
}
