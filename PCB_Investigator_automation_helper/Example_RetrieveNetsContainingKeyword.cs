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
        /// Example method to retrieve nets containing a specific keyword by using the PCB-Investigator API
        /// </summary>
        private static string Example_RetrieveNetsContainingKeyword(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string keyword)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the list of all nets in the current step
            var allNets = step.GetNets();
            List<string> dataNets = new List<string>();
        
            // Iterate through all nets to find those containing the specified keyword
            foreach (var net in allNets)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                {
                    dataNets.Add(net.NetName);
                }
            }
        
            // Return the list of nets containing the specified keyword or a message if no nets were found
            return dataNets.Count > 0
                ? $"Nets containing '{keyword}': " + string.Join(", ", dataNets)
                : $"No nets with '{keyword}' found.";
        }
        
    }
}
