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
        /// Example method to calculate the shortest distance between two specified components by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CalculateComponentDistance(IPCBIWindow pcbi, IStep step, string componentRef1, string componentRef2)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Check if the components exist in the current step
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentRef1, out ICMPObject cmp1)
                && step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentRef2, out ICMPObject cmp2))
            {
                // Check if the components are on the same side of the PCB
                if (cmp1.PlacedTop != cmp2.PlacedTop)
                {
                    return $"The components {componentRef1} and {componentRef2} are not on the same side of the PCB.";
                }
        
                // Get the polygon outlines of the components
                IPolyClass p1 = cmp1.GetPolygonOutline(IncludePins: false);
                IPolyClass p2 = cmp2.GetPolygonOutline(IncludePins: false);
        
                // Calculate the distance between the components
                PointD fromMils = PointD.InfPoint, toMils = PointD.InfPoint;
                double distanceMils = p1.DistanceTo(p2, ref fromMils, ref toMils); //always in mils
        
                // Get the unit the user wants to see in the UI (metric or imperial)
                bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
                return $"The shortest distance between component {componentRef1} and component {componentRef2} is " +
                       (showMetricUnit ? IMath.Mils2MM(distanceMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                               : distanceMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils");
            }
            else
            {
                return $"At least one of the components {componentRef1} or {componentRef2} is not found in the current step.";
            }
        }
        
    }
}
