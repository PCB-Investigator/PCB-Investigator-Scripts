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
        /// Example method to count board markers on signal layers by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountBoardMarkersOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string topSignalLayer, string botSignalLayer)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
        
            // Get the names of the top and bottom signal layers
            List<string> sigLayers = new List<string>() { topSignalLayer, botSignalLayer };
            int countTotal = 0;
            StringBuilder ret = new StringBuilder();
            // Iterate through the signal layers to find board markers
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                int localCount = 0;
                // Iterate through all objects in the layer to find board markers
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.board_mark);
                        if (attr != null && (attr.Value?.ToString().ToLowerInvariant() == "bbm"
                                             || attr.Value?.ToString().ToLowerInvariant() == "gpm"))
                        {
                            localCount++;
                        }
                    }
                }
                if (localCount > 0)
                {
                    ret.AppendLine("There are " + localCount + " board marker on the "
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
                return "There are no recognized board marker in the current step.";
            }
        }

        /// <summary>
        /// Example method to count board markers on signal layers by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountBoardMarkersOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string topSignalLayerName, string botSignalLayerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
        
            // Get the names of the top and bottom signal layers
            List<string> sigLayers = new List<string>() { topSignalLayerName, botSignalLayerName };
            int countTotal = 0;
            StringBuilder ret = new StringBuilder();
            // Iterate through the signal layers to find board markers
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                int localCount = 0;
                // Iterate through all objects in the layer to find board markers
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.board_mark);
                        if (attr != null && (attr.Value?.ToString().ToLowerInvariant() == "bbm"
                                             || attr.Value?.ToString().ToLowerInvariant() == "gpm"))
                        {
                            localCount++;
                        }
                    }
                }
                if (localCount > 0)
                {
                    ret.AppendLine("There are " + localCount + " board marker on the "
                                   + (sigLayer == topSignalLayerName ? "top" : "bot") + " layer.");
                    countTotal += localCount;
                }
            }
            if (countTotal > 0)
            {
                return ret.ToString();
            }
            else
            {
                return "There are no recognized board marker in the current step.";
            }
        }

        /// <summary>
        /// Example method to count board markers on signal layers by using the PCB-Investigator API
        /// </summary>
        private static string Example_CountBoardMarkersOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            IMatrix matrix = pcbi.GetMatrix();
        
            // Get the names of the top and bottom signal layers
            List<string> sigLayers = new List<string>() { matrix.GetTopSignalLayer(), matrix.GetBotSignalLayer() };
            int countTotal = 0;
            StringBuilder ret = new StringBuilder();
            // Iterate through the signal layers to find board markers
            foreach (string sigLayer in sigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (string.IsNullOrEmpty(sigLayer)) continue;
                IODBLayer layer = step.GetLayer(sigLayer) as IODBLayer;
                if (layer == null) continue;
        
                int localCount = 0;
                // Iterate through all objects in the layer to find board markers
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        IAttributeElement attr = IAttribute.GetStandardAttribute(odbObj, PCBI.FeatureAttributeEnum.board_mark);
                        if (attr != null && (attr.Value?.ToString().ToLowerInvariant() == "bbm"
                                             || attr.Value?.ToString().ToLowerInvariant() == "gpm"))
                        {
                            localCount++;
                        }
                    }
                }
                if (localCount > 0)
                {
                    ret.AppendLine("There are " + localCount + " board marker on the "
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
                return "There are no recognized board marker in the current step.";
            }
        }
        
}
}
