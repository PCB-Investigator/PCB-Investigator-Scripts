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
        /// Example method to create and add a new drill layer to the design by using the PCB-Investigator API
        /// </summary>
        private static string Example_CreateAndAddDrillLayer(IPCBIWindow pcbi, IStep step, string newLayerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the layer already exists
            if (step.GetLayer(newLayerName) != null)
            {
                return "The layer '" + newLayerName + "' already exists in the design.";
            }
            IFilter filter = new IFilter(pcbi);
            IMatrix matrix = pcbi.GetMatrix();
            List<string> existingLayers = matrix.GetAllLayerNames();
            // Create a new layer
            ILayer newLayer = filter.CreateEmptyODBLayer(LayerName: newLayerName, StepName: step.Name, AddUndo: true, raiseUpdateEvent: false);
            if (newLayer != null)
            {
                // Set the layer parameters
                matrix.SetMatrixLayerParameter(LayerName: newLayer.GetLayerName(), Context: MatrixLayerContext.Board, Polarity: MatrixLayerPolarity.Positive, Type: MatrixLayerType.Drill, StartLayer: -1, EndLayer: -1, fireEvent: false);
                // Move the drill layer in front of the bottom component layer if no drill layer exists
                List<string> newLayerOrder = new List<string>();
                bool added = false;
                string botComponentLayer = matrix.GetBotComponentLayer();
                foreach (string layer in existingLayers)
                {
                    if (!added && string.Compare(layer, botComponentLayer, true) == 0)
                    {
                        newLayerOrder.Add(newLayerName);
                        added = true;
                    }
                    newLayerOrder.Add(layer);
                }
                if (!added)
                {
                    newLayerOrder.Add(newLayerName);
                }
                // Update the matrix order
                matrix.SetMatrixOrder(LayernamesInCorrectOrder: newLayerOrder, fireEvent: false);
                // Update the matrix
                matrix.UpdateDataAndList();
                // Activate the new layer
                newLayer.EnableLayer(activate: true);
                return "The new drill layer '" + newLayerName + "' is added to the design.";
            }
            else
            {
                return "Failed to create the new drill layer '" + newLayerName + "'.";
            }
        }
        
    }
}
