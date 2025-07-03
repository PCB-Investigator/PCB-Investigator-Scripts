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
        /// Example method to retrieve unique design actions from the history log by using the PCB-Investigator API.
        /// </summary>
        private static async Task<string> Example_RetrieveUniqueDesignActions(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            StringBuilder sb = new StringBuilder();
            List<DesignLogEntry> designLogEntries = await pcbi.History.GetAllLogs();
            HashSet<string> uniqueActions = new HashSet<string>();
        
            // Iterate through all design log entries
            foreach (DesignLogEntry log in designLogEntries)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                string logText = log.Category + " - " + log.Task;
                // Add unique actions to the set and string builder
                if (uniqueActions.Add(logText))
                {
                    sb.AppendLine("-> " + logText);
                }
            }
        
            // Return the unique actions or a message if no actions were found
            if (sb.Length == 0)
            {
                return "There are no actions performed on the current design according to its history.";
            }
            return sb.ToString();
        }
        
    }
}
