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
        /// Example method to select THT copper pads on the bottom signal layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectTHTCopperPadsOnBottomLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string signalLayerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            IMatrix matrix = pcbi.GetMatrix();
            string sigLayer = signalLayerName;
            int count = 0;
        
            // Iterate through the bottom signal layer to find THT copper pads
            IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
            if (layer != null)
            {
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement smdAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.smd);
                        IAttributeElement padAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (padAttr != null && padAttr.Value?.ToString().ToLowerInvariant() == "toeprint"
                            && (smdAttr == null || smdAttr.Value?.ToString().ToLowerInvariant() != "true"))
                        {
                            // Select the THT copper pad
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
                return "All " + count + " copper pads of THT components on the bot layer have been selected in the current step.";
            }
            else
            {
                return "There are no copper pads of THT components on the bot layer in the current step.";
            }
        }

        /// <summary>
        /// Example method to select all THT copper pads on the bottom signal layer in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectTHTCopperPadsOnBottomLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string sigLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            IMatrix matrix = pcbi.GetMatrix();
            int count = 0;
        
            // Iterate through the bottom signal layer to find THT copper pads
            IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
            if (layer != null)
            {
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement smdAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.smd);
                        IAttributeElement padAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (padAttr != null && padAttr.Value?.ToString().ToLowerInvariant() == "toeprint"
                            && (smdAttr == null || smdAttr.Value?.ToString().ToLowerInvariant() != "true"))
                        {
                            // Select the THT copper pad
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
                return "All " + count + " copper pads of THT components on the bot layer have been selected in the current step.";
            }
            else
            {
                return "There are no copper pads of THT components on the bot layer in the current step.";
            }
        }

        /// <summary>
        /// Example method to select all THT copper pads on the bottom signal layer in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectTHTCopperPadsOnBottomLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string signalLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            IMatrix matrix = pcbi.GetMatrix();
            int count = 0;
        
            // Iterate through the bottom signal layer to find THT copper pads
            IODBLayer layer = step.GetLayer(signalLayer) as IODBLayer;
            if (layer != null)
            {
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement smdAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.smd);
                        IAttributeElement padAttr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (padAttr != null && padAttr.Value?.ToString().ToLowerInvariant() == "toeprint"
                            && (smdAttr == null || smdAttr.Value?.ToString().ToLowerInvariant() != "true"))
                        {
                            // Select the THT copper pad
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
                return "All " + count + " copper pads of THT components on the bot layer have been selected in the current step.";
            }
            else
            {
                return "There are no copper pads of THT components on the bot layer in the current step.";
            }
        }
        
}
}
