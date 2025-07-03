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
        /// Example method to zoom into a specified component by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ZoomIntoComponent(IPCBIWindow pcbi, IStep step, string componentReference)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the specified component exists in the current step
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentReference, out ICMPObject cmp))
            {
                // Enable the layer of the component
                step.GetLayer(cmp.LayerName)?.EnableLayer(activate: true);
        
                // Get the bounds of the component and inflate the rectangle for better visibility
                bool reduceClientToVisibleRect = true;
                RectangleD rectMils = cmp.GetBoundsD(); //always in mils
                double inflate = Math.Min(rectMils.Width, rectMils.Height) * 0.5;
                rectMils.Inflate(inflate, inflate);
                // Zoom into the component
                pcbi.ZoomRect(rectMils, reduceClientToVisibleRect);
        
                return $"The view has been zoomed into the component {componentReference}.";
            }
            else
            {
                return $"The component {componentReference} is not found in the current step.";
            }
        }
        
    }
}
