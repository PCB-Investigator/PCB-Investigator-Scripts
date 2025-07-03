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
        /// Example method to check for test points in nets by using the PCB-Investigator API
        /// </summary>
        private static string Example_CheckForTestPointsInNets(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // StringBuilder to store the results
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Net Name\tHas Test Point");
            // Iterate through all nets
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                bool hasTestPoint = false;
                // Check for test point components
                foreach (INetObject pinInfo in net.ComponentList)
                {
                    string refDes = pinInfo.ICMP?.Ref;
                    if (refDes != null && (refDes.StartsWith("TP") || refDes.StartsWith("P") || refDes.StartsWith("MP")))
                    {
                        hasTestPoint = true;
                        break;
                    }
                }
                // Check for test point pads if no test point component was found
                if (!hasTestPoint)
                {
                    foreach (IODBObject obj in net.GetAllNetObjects(pcbi))
                    {
                        if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                        if (obj is IODBObject odbObj)
                        {
                            IAttributeElement testPointAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.test_point);
                            if (testPointAttr != null && testPointAttr.Value?.ToString().ToLowerInvariant() == "true")
                            {
                                hasTestPoint = true;
                                break;
                            }
                        }
                    }
                }
                // Add the net name and test point presence to the result
                sb.AppendLine(net.NetName + "\t" + (hasTestPoint ? "Yes" : "No"));
            }
            // Check if any nets were found
            if (sb.Length == 0)
            {
                return "There are no nets in the current step.";
            }
            return sb.ToString();
        }
        
    }
}
