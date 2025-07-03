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
        /// Example method to select components with datasheet URLs in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectComponentsWithDatasheetURLs(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            int count = 0;
        
            // Iterate through all components
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Check if the component has a datasheet URL
                IEDA_PRP prp = IAttribute.GetProperty(cmp, "DATASHEET_URL");
                if (prp != null && !string.IsNullOrWhiteSpace(prp.VALUE_STRING))
                {
                    // Select the component
                    cmp.Select(select: true);
                    count++;
                }
            }
        
            // Update the selection and view if any components with datasheet URLs were found
            if (count > 0)
            {
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " components that have a datasheet link have been selected in the current step.";
            }
            else
            {
                return "There are no components that have a datasheet link in the current step.";
            }
        }
        
    }
}
