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
        /// Example method to select all unpopulated components in a PCB design step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectUnpopulatedComponents(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Clear the selection
            step.ClearSelection();
            // Get all components in the step
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
                // Check if the component is populated
                bool isPopulated = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_ignore) == null && IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.no_pop) == null;
                // Select the unpopulated component
                if (!isPopulated)
                {
                    cmp.Select(true);
                }
            }
            // Zoom to the selected objects
            pcbi.ZoomToSelection();
            return "All unpopulated components are selected.";
        }
        
    }
}
