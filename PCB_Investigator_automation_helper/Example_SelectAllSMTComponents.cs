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
        /// Example method to select all SMT components in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectAllSMTComponents(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            int count = 0;
            // Iterate through all components in the current step
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Check if the component is an SMT component
                IAttributeElement mountTypeAttr = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_mount_type);
                if (mountTypeAttr?.Value?.ToString().ToLowerInvariant() == "smt")
                {
                    // Select the SMT component
                    cmp.Select(select: true);
                    count++;
                }
            }
            if (count > 0)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " SMT components have been selected in the current step.";
            }
            else
            {
                return "There are no SMT components in the current step.";
            }
        }
        
    }
}
