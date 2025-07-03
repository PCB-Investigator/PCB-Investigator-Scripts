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
        /// Example method to select all SMT pads in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectAllSMTPadsInCurrentStep(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
            List<string> sigLayers = new List<string>() { matrix.GetTopSignalLayer(), matrix.GetBotSignalLayer() };
            int count = 0;
        
            // Iterate through all signal layers to find SMT pads
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.smd);
                        if (attr != null && attr.Value?.ToString().ToLowerInvariant() == "true")
                        {
                            // Select the SMT pad
                            odbObj.Select(select: true);
                            count++;
                        }
                    }
                }
            }
            if (count > 0)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " SMT pads have been selected in the current step.";
            }
            else
            {
                return "There are no SMT pads in the current step.";
            }
        }

        /// <summary>
        /// Example method to select all SMT pads in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectAllSMTPadsInCurrentStep(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, List<string> signalLayers)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
            List<string> sigLayers = signalLayers ?? new List<string>() { matrix.GetTopSignalLayer(), matrix.GetBotSignalLayer() };
            int count = 0;
        
            // Iterate through all signal layers to find SMT pads
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.smd);
                        if (attr != null && attr.Value?.ToString().ToLowerInvariant() == "true")
                        {
                            // Select the SMT pad
                            odbObj.Select(select: true);
                            count++;
                        }
                    }
                }
            }
            if (count > 0)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All " + count + " SMT pads have been selected in the current step.";
            }
            else
            {
                return "There are no SMT pads in the current step.";
            }
        }
        
}
}
