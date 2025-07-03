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
        /// Example method to color power pins green by using the PCB-Investigator API
        /// </summary>
        private static string Example_ColorPowerPinsGreen(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            int count = 0;
        
            // Reset the color of all pins
            step.GetAllCMPObjects()?.ForEach(cmp => cmp.GetPinList()?.ForEach(pin => pin.ResetPinColor(Parent: cmp)));
        
            // Iterate through all nets to find power nets
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                string netNameLower = net.NetName.ToLowerInvariant();
                if (netNameLower.Contains("power") || netNameLower.Contains("vcc") || netNameLower.Contains("3v")
                    || netNameLower.Contains("5v") || netNameLower.Contains("12v") || netNameLower.Contains("24v")
                    || netNameLower.Contains("36v") || netNameLower.Contains("48v") || netNameLower.Contains("vbat")
                    || netNameLower.Contains("vcore") || netNameLower.Contains("vperi"))
                {
                    // Iterate through all pins in the power net and color them green
                    foreach (INetObject pinInfo in net.ComponentList)
                    {
                        IPin pin = pinInfo.GetIPin();
                        if (pin != null)
                        {
                            pin.SetPinColor(pinColor: Color.Green, Parent: pinInfo.ICMP);
                            count++;
                        }
                    }
                }
            }
        
            if (count > 0)
            {
                // Update the view
                pcbi.UpdateView(NeedFullRedraw: true);
                return "The color of all " + count + " power pins has been changed to green.";
            }
            else
            {
                return "There are no ground pins in the current step.";
            }
        }
        
    }
}
