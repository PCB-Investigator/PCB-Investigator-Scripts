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
        /// Example method to select nets without test points in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetsWithoutTestPoints(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
            // Initialize the count of nets without test points
            int count = 0;
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
                // Select the net if it does not have a test point
                if (!hasTestPoint)
                {
                    net.SelectNet(onlyTheseTypesOrNull: null, fireSelectionChangedEvent: false);
                    count++;
                }
            }
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            // Check if any nets were selected
            if (count > 0)
            {
                return "All " + count + " nets that do not have a test point have been selected in the current step.";
            }
            else
            {
                return "There are no nets that do not have a test point in the current step.";
            }
        }
        
    }
}
