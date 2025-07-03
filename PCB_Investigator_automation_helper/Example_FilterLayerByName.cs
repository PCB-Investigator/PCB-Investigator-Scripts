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
        /// Example method to filter for a specific layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FilterLayerByName(IPCBIWindow pcbi, IStep step, string layerName)
        {
            if (pcbi == null)
                return "No IPCBIWindow available.";
        
            if (step == null)
                return "No step available.";
        
            if (string.IsNullOrEmpty(layerName))
                return "Layer name is not specified.";
        
            var layerList = step.GetAllLayerNames();
            if (!layerList.Contains(layerName))
                return $"Layer '{layerName}' not found.";
        
            var layer = step.GetLayer(layerName);
            if (layer == null)
                return $"Layer '{layerName}' could not be retrieved.";
        
            // Perform operations on the layer
            // ...
        
            return $"Layer '{layerName}' processed successfully.";
        }
        
    }
}
