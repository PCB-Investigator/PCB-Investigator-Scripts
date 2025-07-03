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
        /// Example method to activate a specified layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ActivateLayerByName(IPCBIWindow pcbi, IStep step, string layerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Get the layer with the specified name
            ILayer layer = step.GetLayer(layerName);
            if (layer != null)
            {
                layer.EnableLayer(activate: true);
                return "The layer '" + layerName + "' is displayed and activated.";
            }
            else
            {
                return "The layer '" + layerName + "' is not found in the current step.";
            }
        }
        
    }
}
