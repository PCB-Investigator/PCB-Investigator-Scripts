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
        /// Example method to retrieve panel building task logs from the design history by using the PCB-Investigator API.
        /// </summary>
        private static async Task<string> Example_RetrievePanelBuildingLogs(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Create a filter to search for panel building tasks in the design history
            DBFilterInfo dBFilterInfo = new DBFilterInfo();
            dBFilterInfo.And = true;
            dBFilterInfo.FilterParameters.Add(
                new DBFilterInfo.DBFilterParameter
                {
                    Key = "Category",
                    Operation = DBFilterInfo.DBFilterParameter.Operations.Equals,
                    Value = TaskCategory.PanelBuilding
                });
        
            // StringBuilder to store the results
            StringBuilder sb = new StringBuilder();
            // Get the design log entries that match the filter
            List<DesignLogEntry> designLogEntries = await pcbi.History.GetLogs(dBFilterInfo);
        
            // Iterate through all design log entries and append them to the StringBuilder
            foreach (DesignLogEntry log in designLogEntries)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                sb.AppendLine("-> " + log.LogTimeUTC.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss") + ": " + log.Category + " - " + log.Task + " - " + log.Description);
            }
        
            // Return the panel build logs or a message if no logs were found
            if (sb.Length > 0)
            {
                return "There has been done following panel build according to the design history:\n" + sb.ToString();
            }
            else
            {
                return "There has been no panel build according to the design history.";
            }
        }

        /// <summary>
        /// Example method to retrieve panel building task logs from the design history by using the PCB-Investigator API.
        /// </summary>
        private static async Task<string> Example_RetrievePanelBuildingLogs(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, TaskCategory taskCategory)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Create a filter to search for panel building tasks in the design history
            DBFilterInfo dBFilterInfo = new DBFilterInfo();
            dBFilterInfo.And = true;
            dBFilterInfo.FilterParameters.Add(
                new DBFilterInfo.DBFilterParameter
                {
                    Key = "Category",
                    Operation = DBFilterInfo.DBFilterParameter.Operations.Equals,
                    Value = taskCategory
                });
        
            // StringBuilder to store the results
            StringBuilder sb = new StringBuilder();
            // Get the design log entries that match the filter
            List<DesignLogEntry> designLogEntries = await pcbi.History.GetLogs(dBFilterInfo);
        
            // Iterate through all design log entries and append them to the StringBuilder
            foreach (DesignLogEntry log in designLogEntries)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                sb.AppendLine("-> " + log.LogTimeUTC.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss") + ": " + log.Category + " - " + log.Task + " - " + log.Description);
            }
        
            // Return the panel build logs or a message if no logs were found
            if (sb.Length > 0)
            {
                return "There has been done following panel build according to the design history:\n" + sb.ToString();
            }
            else
            {
                return "There has been no panel build according to the design history.";
            }
        }
        
}
}
