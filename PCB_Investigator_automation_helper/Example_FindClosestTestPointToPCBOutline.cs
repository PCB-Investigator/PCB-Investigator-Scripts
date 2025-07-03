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
        /// Example method to find the closest test point to the PCB outline by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FindClosestTestPointToPCBOutline(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get all components in the current step
            var allComponents = step.GetAllCMPObjects();
            // Get the PCB outline polygon
            IPolyClass boardOutline = step.GetPCBOutlinePoly();
            if (boardOutline == null)
            {
                return "The board outline is not found in the current step.";
            }
        
            double minDistanceMils = double.MaxValue;
            PointD fromMils = PointD.InfPoint, toMils = PointD.InfPoint;
            string closestTestPoint = "None";
        
            // Iterate through all components to find the closest test point to the PCB outline
            foreach (var cmp in allComponents)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (cmp.Ref.StartsWith("TP") || cmp.Ref.StartsWith("MP") || cmp.Ref.StartsWith("P"))
                {
                    // Get the polygon outline of the test point
                    IPolyClass p1 = cmp.GetPolygonOutline(IncludePins: false);
                    // Calculate the distance between the test point and the PCB outline
                    double distanceMils = p1.DistanceTo(boardOutline, ref fromMils, ref toMils);  //always in mils
                    if (distanceMils < minDistanceMils)
                    {
                        minDistanceMils = distanceMils;
                        closestTestPoint = cmp.Ref;
                    }
                }
            }
        
            if (minDistanceMils == double.MaxValue)
            {
                return "No test points found in the current step.";
            }
        
            // Get the unit the user wants to see in the UI (metric or imperial)
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            return "The shortest distance from test point " + closestTestPoint + " to the board outline is " +
                   (showMetricUnit ? IMath.Mils2MM(minDistanceMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                           : minDistanceMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils");
        }
        
    }
}
