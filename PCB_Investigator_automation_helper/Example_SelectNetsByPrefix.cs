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
        /// Example method to select all nets starting with a specified prefix in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetsByPrefix(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netPrefix)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            List<string> foundNets = new List<string>();
            // Iterate through all nets to find those starting with the specified prefix
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant().StartsWith(netPrefix.ToLowerInvariant()))
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: null, fireSelectionChangedEvent: false);
                    foundNets.Add(net.NetName);
                }
            }
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (foundNets.Count > 0)
            {
                return $"All nets starting with '{netPrefix}' have been selected in the current design: " + string.Join(", ", foundNets.OrderBy(aa => aa));
            }
            else
            {
                return $"There are no nets starting with '{netPrefix}' in the current design.";
            }
        }

        /// <summary>
        /// Example method to select all nets starting with a specified prefix in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetsByPrefix(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netPrefix)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            List<string> foundNets = new List<string>();
            // Iterate through all nets to find those starting with the specified prefix
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant().StartsWith(netPrefix.ToLowerInvariant()))
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: null, fireSelectionChangedEvent: false);
                    foundNets.Add(net.NetName);
                }
            }
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (foundNets.Count > 0)
            {
                return "All nets starting with '" + netPrefix + "' have been selected in the current design: " + string.Join(", ", foundNets.OrderBy(aa => aa));
            }
            else
            {
                return "There are no nets starting with '" + netPrefix + "' in the current design.";
            }
        }
        
}
}
