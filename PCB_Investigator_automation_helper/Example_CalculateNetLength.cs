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
        /// Example method to calculate the total length of a specified net by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CalculateNetLength(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the unit the user wants to see in the UI (metric or imperial)
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            // Get the net named by the provided netName
            INet net = step.GetNet(netName);
            if (net != null)
            {
                double totalNetLengthMils = 0;
                // Get the matrix of the current job
                IMatrix matrix = pcbi.GetMatrix();
                Dictionary<string, double> depthPerDrillLayer = new Dictionary<string, double>();
        
                // Iterate through all used layers of the net to calculate the total length
                foreach (string layername in net.GetAllUsedLayers())
                {
                    double drillDepthMils = 0;
                    MatrixLayerType relType = matrix.GetMatrixLayerType(layername);
                    if (relType == MatrixLayerType.Drill)
                    {
                        // Calculate the depth of the drill
                        int startIndex = matrix.GetStartDrillLayer(layername);
                        int endIndex = matrix.GetEndDrillLayer(layername);
        
                        drillDepthMils = matrix.GetDistanceBetweenLayers(startIndex, endIndex);  //always in mils
                        depthPerDrillLayer[layername] = drillDepthMils;
                    }
                }
        
                // Iterate through all net objects to calculate the total length
                foreach (IODBObject obj in net.GetAllNetObjects(pcbi))
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    IObjectSpecificsD lineSpec = obj.GetSpecificsD();
                    if (lineSpec.GetType() == typeof(ILineSpecificsD))
                    {
                        double distanceMils = IMath.DistancePointToPoint(((ILineSpecificsD)lineSpec).Start, ((ILineSpecificsD)lineSpec).End);  //always in mils
                        totalNetLengthMils += distanceMils;
                    }
                    else if (lineSpec is IArcSpecificsD)
                    {
                        IArcSpecificsD arc = (IArcSpecificsD)lineSpec;
        
                        double distanceMils = IMath.DistanceOnArc(IMath.GetAngle(arc.Start, arc.End, arc.Center, arc.ClockWise), IMath.DistancePointToPoint(arc.Start, arc.Center)); //always in mils
                        totalNetLengthMils += distanceMils;
                    }
                    else if (lineSpec is IPadSpecificsD && obj.GetSymbol()?.Type == PCBI.Symbol_Type.r && depthPerDrillLayer.TryGetValue(obj.GetParentLayerName().ToLowerInvariant(), out double drillDepthMils))
                    {
                        totalNetLengthMils += drillDepthMils;
                    }
                }
        
                if (showMetricUnit)
                {
                    return "The length of the net named '" + netName + "' is " + IMath.Mils2MM(totalNetLengthMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.";
                }
                else
                {
                    return "The length of the net named '" + netName + "' is " + totalNetLengthMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils.";
                }
            }
            else
            {
                return "The net named '" + netName + "' is not found in the current step.";
            }
        }
        
    }
}
