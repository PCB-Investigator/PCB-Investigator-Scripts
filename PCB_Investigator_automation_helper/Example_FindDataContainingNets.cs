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
        /// Example method to find and list all nets containing 'data' in their names by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FindDataContainingNets(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the list of all nets in the current step
            var allNets = step.GetNets();
            List<string> dataNets = new List<string>();
        
            // Iterate through all nets to find those containing 'data'
            foreach (var net in allNets)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant().Contains("data"))
                {
                    dataNets.Add(net.NetName);
                }
            }
        
            // Return the list of nets containing 'data' or a message if no nets were found
            return dataNets.Count > 0
                ? "Nets containing 'data': " + string.Join(", ", dataNets)
                : "No nets with 'data' found.";
        }
        
    }
}
