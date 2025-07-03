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
        /// Example method to retrieve and display original data import logs by using the PCB-Investigator API.
        /// </summary>
        private static async Task<string> Example_RetrieveOriginalDataImportLogs(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, TaskCategory category)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            DBFilterInfo dBFilterInfo = new DBFilterInfo();
            dBFilterInfo.And = true;
            dBFilterInfo.FilterParameters.Add(
                new DBFilterInfo.DBFilterParameter
                {
                    Key = "Category",
                    Operation = DBFilterInfo.DBFilterParameter.Operations.Equals,
                    Value = category
                });
        
            StringBuilder sb = new StringBuilder();
            List<DesignLogEntry> designLogEntries = await pcbi.History.GetLogs(dBFilterInfo);
        
            // Iterate through all design log entries
            foreach (DesignLogEntry log in designLogEntries)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                sb.AppendLine("-> " + log.LogTimeUTC.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss") + ": " + log.Category + " - " + log.Task + " - " + log.Description);
            }
        
            // Return the original data import logs or a message if no logs were found
            if (sb.Length > 0)
            {
                return "There has been done following original data import according to the design history:\n" + sb.ToString();
            }
            else
            {
                return "There has been no original data import according to the design history.";
            }
        }

        /// <summary>
        /// Example method to retrieve and display original data import logs by using the PCB-Investigator API.
        /// </summary>
        private static async Task<string> Example_RetrieveOriginalDataImportLogs(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, TaskCategory taskCategory)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            DBFilterInfo dBFilterInfo = new DBFilterInfo();
            dBFilterInfo.And = true;
            dBFilterInfo.FilterParameters.Add(
                new DBFilterInfo.DBFilterParameter
                {
                    Key = "Category",
                    Operation = DBFilterInfo.DBFilterParameter.Operations.Equals,
                    Value = taskCategory
                });
        
            StringBuilder sb = new StringBuilder();
            List<DesignLogEntry> designLogEntries = await pcbi.History.GetLogs(dBFilterInfo);
        
            // Iterate through all design log entries
            foreach (DesignLogEntry log in designLogEntries)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                sb.AppendLine("-> " + log.LogTimeUTC.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss") + ": " + log.Category + " - " + log.Task + " - " + log.Description);
            }
        
            // Return the original data import logs or a message if no logs were found
            if (sb.Length > 0)
            {
                return "There has been done following original data import according to the design history:\n" + sb.ToString();
            }
            else
            {
                return "There has been no original data import according to the design history.";
            }
        }
        
}
}
