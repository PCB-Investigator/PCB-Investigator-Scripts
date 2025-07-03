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
        /// Example method to count fiducials on signal layers by using the PCB-Investigator API
        /// </summary>
        private static string Example_CountFiducialsOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
            List<string> sigLayers = new List<string>() { matrix.GetTopSignalLayer(), matrix.GetBotSignalLayer() };
            int countTotal = 0;
            StringBuilder ret = new StringBuilder();
        
            // Iterate through all signal layers to find fiducials
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                int localCount = 0;
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (attr != null && (attr.Value?.ToString().ToLowerInvariant() == "g_fiducial"
                                             || attr.Value?.ToString().ToLowerInvariant() == "l_fiducial"))
                        {
                            localCount++;
                        }
                    }
                }
                if (localCount > 0)
                {
                    ret.AppendLine("There are " + localCount + " fiducials on the "
                                   + (sigLayer == matrix.GetTopSignalLayer() ? "top" : "bot") + " layer.");
                    countTotal += localCount;
                }
            }
            if (countTotal > 0)
            {
                return ret.ToString();
            }
            else
            {
                return "There are no recognized fiducials in the current step.";
            }
        }

        /// <summary>
        /// Example method to count fiducials on signal layers by using the PCB-Investigator API
        /// </summary>
        private static string Example_CountFiducialsOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string topSignalLayer, string botSignalLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
            List<string> sigLayers = new List<string>() { topSignalLayer, botSignalLayer };
            int countTotal = 0;
            StringBuilder ret = new StringBuilder();
        
            // Iterate through all signal layers to find fiducials
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                int localCount = 0;
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.pad_usage);
                        if (attr != null && (attr.Value?.ToString().ToLowerInvariant() == "g_fiducial"
                                             || attr.Value?.ToString().ToLowerInvariant() == "l_fiducial"))
                        {
                            localCount++;
                        }
                    }
                }
                if (localCount > 0)
                {
                    ret.AppendLine("There are " + localCount + " fiducials on the "
                                   + (sigLayer == topSignalLayer ? "top" : "bot") + " layer.");
                    countTotal += localCount;
                }
            }
            if (countTotal > 0)
            {
                return ret.ToString();
            }
            else
            {
                return "There are no recognized fiducials in the current step.";
            }
        }
        
}
}
