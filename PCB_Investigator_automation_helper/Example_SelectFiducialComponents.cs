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
        /// Example method to select fiducial components in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectFiducialComponents(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            bool anySelected = false;
            List<string> foundRefs = new List<string>();
        
            // Iterate through all components to find fiducials
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (cmp.Ref.StartsWith("FID"))
                {
                    // Select the fiducial
                    cmp.Select(select: true);
                    anySelected = true;
                    foundRefs.Add(cmp.Ref);
                }
            }
            if (anySelected)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "Following components have been selected in the current design: " + string.Join(", ", foundRefs);
            }
            else
            {
                return "There are no components with reference designators starting with FID in the current step.";
            }
        }
        
    }
}
