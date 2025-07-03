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
        /// Example method to calculate and return the size of the current PCB in the loaded step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CalculatePCBSize(IPCBIWindow pcbi, IStep step, bool showMetricUnit)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the PCB outline polygon
            IPolyClass profilePoly = step.GetPCBOutlinePoly();
            if (profilePoly != null)
            {
                // Get the bounds of the PCB outline
                RectangleD boundsMils = profilePoly.GetBounds(); //always in mils
        
                if (showMetricUnit)
                {
                    return "The size of the current PCB in the loaded step is "
                           + IMath.Mils2MM(boundsMils.Width).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm x "
                           + IMath.Mils2MM(boundsMils.Height).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.";
                }
                else
                {
                    return "The size of the current PCB in the loaded step is "
                           + (boundsMils.Width / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch x "
                           + (boundsMils.Height / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch.";
                }
            }
            else
            {
                // If no PCB outline is defined, get the bounds of the current step
                RectangleD boundsMils = step.GetBoundsD(); //always in mils
                if (showMetricUnit)
                {
                    return "There is no PCB contour defined, but the size of the current PCB data of the loaded step is "
                           + IMath.Mils2MM(boundsMils.Width).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm x "
                           + IMath.Mils2MM(boundsMils.Height).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.";
                }
                else
                {
                    return "There is no PCB contour defined, but the size of the current PCB data of the loaded step is "
                           + (boundsMils.Width / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch x "
                           + (boundsMils.Height / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch.";
                }
            }
        }
        
    }
}
