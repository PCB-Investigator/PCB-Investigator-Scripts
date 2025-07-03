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
        /// Example method to move a specified layer above the top signal layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_MoveLayerAboveTopSignalLayer(IPCBIWindow pcbi, IStep step, string layerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Get the layer with the specified name
            ILayer layer = step.GetLayer(layerName);
            if (layer == null)
            {
                return "The layer '" + layerName + "' is not found in the design.";
            }
            
            IMatrix matrix = pcbi.GetMatrix();
            List<string> existingLayers = matrix.GetAllLayerNames();
            
            // Move the layer above the top signal layer
            List<string> newLayerOrder = new List<string>();
            bool changed = false;
            string topSignalLayer = matrix.GetTopSignalLayer();
            
            foreach (string existingLayer in existingLayers)
            {
                // Skip the layer that should be moved
                if (string.Compare(existingLayer, layerName, true) == 0)
                {
                    continue;
                }
                
                // Add the layer above the top signal layer
                if (!changed && string.Compare(existingLayer, topSignalLayer, true) == 0)
                {
                    newLayerOrder.Add(layerName);
                    changed = true;
                }
                
                newLayerOrder.Add(existingLayer);
            }
            
            if (changed)
            {
                // Update the matrix order
                matrix.SetMatrixOrder(LayernamesInCorrectOrder: newLayerOrder, fireEvent: false);
                
                // Update the matrix
                matrix.UpdateDataAndList();
                return "The layer '" + layerName + "' is moved above the top signal layer.";
            }
            else
            {
                return "The layer '" + layerName + "' was not moved.";
            }
        }
        
    }
}
