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
        /// Example method to analyze the thinnest trace width of specified nets by using the PCB-Investigator API.
        /// </summary>
        private static string Example_AnalyzeThinnestTraceWidth(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, List<string> netNames)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the unit the user wants to see in the UI (metric or imperial)
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            // StringBuilder to store the results
            StringBuilder sb = new StringBuilder();
        
            // Iterate through each net name
            foreach (string netName in netNames)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Get the net by name
                INet net = step.GetNet(netName);
                if (net != null)
                {
                    // Initialize the smallest line width to a large value
                    double smallestLineWidthMils = double.MaxValue;
                    // Iterate through all net objects
                    foreach (IODBObject obj in net.GetAllNetObjects(pcbi))
                    {
                        if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                        // Check if the object is a line
                        if (obj is IODBObject odbObj && odbObj.Type == IObjectType.Line)
                        {
                            // Get the line width
                            double lineWidthMils = odbObj.GetDiameter(); //always in mils
                            // Update the smallest line width if necessary
                            if (lineWidthMils < smallestLineWidthMils)
                            {
                                smallestLineWidthMils = lineWidthMils;
                            }
                        }
                    }
                    // Check if a valid line width was found
                    if (smallestLineWidthMils < double.MaxValue)
                    {
                        // Convert the line width to the appropriate unit and add to the result
                        if (showMetricUnit)
                        {
                            sb.AppendLine("The thinnest trace width of the net " + net.NetName + " is " + IMath.Mils2MM(smallestLineWidthMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.");
                        }
                        else
                        {
                            sb.AppendLine("The thinnest trace width of the net " + net.NetName + " is " + smallestLineWidthMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils.");
                        }
                    }
                    else
                    {
                        sb.AppendLine("There are no line objects in the net named '" + net.NetName + "'.");
                    }
                }
                else sb.AppendLine("The net named '" + netName + "' is not found in the current step.");
            }
            // Return the result
            return sb.ToString();
        }
        
    }
}
