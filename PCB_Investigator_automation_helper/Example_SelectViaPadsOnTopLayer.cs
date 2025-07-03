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
        /// Example method to select all VIA pads on the top signal layer in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectViaPadsOnTopLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string sigLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            IMatrix matrix = pcbi.GetMatrix();
            int count = 0;
        
            // Iterate through the top signal layer to find VIA pads
            IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
            if (layer != null)
            {
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (attr != null && attr.Value?.ToString().ToLowerInvariant() == "via")
                        {
                            // Select the VIA pad
                            odbObj.Select(select: true);
                            count++;
                        }
                    }
                }
            }
        
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (count > 0)
            {
                return "All " + count + " VIA pads on the top layer have been selected in the current step.";
            }
            else
            {
                return "There are no SMT pads on the top layer in the current step.";
            }
        }

        /// <summary>
        /// Example method to select all VIA pads on the top signal layer in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectViaPadsOnTopLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            IMatrix matrix = pcbi.GetMatrix();
            string sigLayer = matrix.GetTopSignalLayer();
            int count = 0;
        
            // Iterate through the top signal layer to find VIA pads
            IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
            if (layer != null)
            {
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (attr != null && attr.Value?.ToString().ToLowerInvariant() == "via")
                        {
                            // Select the VIA pad
                            odbObj.Select(select: true);
                            count++;
                        }
                    }
                }
            }
        
            // Update the selection and view
            pcbi.UpdateSelection();
            pcbi.UpdateView(NeedFullRedraw: true);
            if (count > 0)
            {
                return "All " + count + " VIA pads on the top layer have been selected in the current step.";
            }
            else
            {
                return "There are no SMT pads on the top layer in the current step.";
            }
        }
        
}
}
