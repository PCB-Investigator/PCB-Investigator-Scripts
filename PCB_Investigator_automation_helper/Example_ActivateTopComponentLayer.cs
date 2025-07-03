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
        /// Example method to activate the top component layer in a PCB job by using the PCB-Investigator API
        /// </summary>
        private static string Example_ActivateTopComponentLayer(IPCBIWindow pcbi, IStep step, string topComponentLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IStep panelStep = step;
            // Check if the current step is a root step
            if (panelStep.IsRootStep)
            {
                // Iterate through all steps to find the panel step
                foreach (IStep stepEntry in pcbi.GetStepList())
                {
                    if (!stepEntry.IsRootStep)
                    {
                        // Activate the panel step
                        stepEntry.ActivateStep();
                        panelStep = stepEntry;
                        break;
                    }
                }
            }
        
            IMatrix matrix = pcbi.GetMatrix();
            if (topComponentLayer != null)
            {
                // Get the top component layer in the panel step
                ILayer layer = panelStep.GetLayer(topComponentLayer);
                if (layer != null)
                {
                    // Turn off all layers first
                    panelStep.TurnOffAllLayer();
                    // Enable the top component layer
                    layer.EnableLayer(activate: true);
                    return "The top component layer '" + topComponentLayer + "' is displayed and activated.";
                }
                else
                {
                    return "The top component layer '" + topComponentLayer + "' is not found in the current step.";
                }
            }
            else
            {
                return "The top component layer is not defined in the current job.";
            }
        }

        /// <summary>
        /// Example method to activate the top component layer by using the PCB-Investigator API
        /// </summary>
        private static string Example_ActivateTopComponentLayer(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IStep panelStep = step;
            // Check if the current step is a root step
            if (panelStep.IsRootStep)
            {
                // Iterate through all steps to find the panel step
                foreach (IStep stepEntry in pcbi.GetStepList())
                {
                    if (!stepEntry.IsRootStep)
                    {
                        // Activate the panel step
                        stepEntry.ActivateStep();
                        panelStep = stepEntry;
                        break;
                    }
                }
            }
        
            IMatrix matrix = pcbi.GetMatrix();
            string topComponentLayer = matrix.GetTopComponentLayer();
            if (topComponentLayer != null)
            {
                // Get the top component layer in the panel step
                ILayer layer = panelStep.GetLayer(topComponentLayer);
                if (layer != null)
                {
                    // Turn off all layers first
                    panelStep.TurnOffAllLayer();
                    // Enable the top component layer
                    layer.EnableLayer(activate: true);
                    return "The top component layer '" + topComponentLayer + "' is displayed and activated.";
                }
                else
                {
                    return "The top component layer '" + topComponentLayer + "' is not found in the current step.";
                }
            }
            else
            {
                return "The top component layer is not defined in the current job.";
            }
        }
        
}
}
