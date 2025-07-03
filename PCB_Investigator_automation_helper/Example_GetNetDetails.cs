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
        /// Example method to retrieve and display details of a specified net by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetNetDetails(IPCBIWindow pcbi, IStep step, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the net named by the parameter
            INet net = step.GetNet(netName);
            if (net != null)
            {
                // Create a string builder to store the net details
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Net Name: " + net.NetName);
                sb.AppendLine("Connected Pins: " + net.ComponentList.Count);
                sb.AppendLine("Used Layers: " + string.Join(", ", net.GetAllUsedLayers()));
        
                // Add all attributes of the net
                IAttribute.GetAllAttributes(net, pcbi).ForEach(attr =>
                {
                    sb.AppendLine(attr.DisplayName + ": " + attr.Value?.ToString() ?? "");
                });
        
                return sb.ToString();
            }
            else
            {
                return $"The net named '{netName}' is not found in the current design.";
            }
        }
        
    }
}
