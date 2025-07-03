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
        /// Example method to add missing prepreg layers to the design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_AddMissingPrepregLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
        
            // Initialize filter and other variables
            IFilter filter = new IFilter(pcbi);
            List<string> newLayerOrder = new List<string>();
            bool nextMustBePrepreg = false;
            HashSet<string> existingLayersLower = new HashSet<string>(step.GetAllLayerNames(toLower: true));
        
            int addedLayers = 0;
            int prePregIndex = 1;
        
            // Iterate through all layers in the matrix
            foreach (string layer in matrix.GetAllLayerNames())
            {
                // Check if the operation is cancelled
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                // Check if the layer is a signal layer
                if (matrix.IsSignalLayer(layer))
                {
                    if (nextMustBePrepreg)
                    {
                        // Generate a unique prepreg layer name
                        string prepregLayerName = "prepreg" + prePregIndex++;
                        while (existingLayersLower.Contains(prepregLayerName.ToLowerInvariant()))
                        {
                            prepregLayerName = "prepreg" + prePregIndex++;
                        }
                        // Create the prepreg layer and set its parameters
                        filter.CreateEmptyODBLayer(LayerName: prepregLayerName, StepName: step.Name, AddUndo: true, raiseUpdateEvent: false);
                        matrix.SetMatrixLayerParameter(LayerName: prepregLayerName, Context: MatrixLayerContext.Board, Polarity: MatrixLayerPolarity.Positive, Type: MatrixLayerType.Dielectric, StartLayer: -1, EndLayer: -1, fireEvent: false);
                        newLayerOrder.Add(prepregLayerName);
                        existingLayersLower.Add(prepregLayerName);
                        addedLayers++;
                    }
                    nextMustBePrepreg = true;
                }
                // Check if the layer is a prepreg/dielectric layer
                else if (matrix.GetMatrixLayerType(layer) == MatrixLayerType.Dielectric)
                {
                    nextMustBePrepreg = false;
                }
        
                newLayerOrder.Add(layer);
            }
        
            // Update the matrix order if new layers were added
            if (addedLayers > 0)
            {
                matrix.SetMatrixOrder(LayernamesInCorrectOrder: newLayerOrder, fireEvent: false);
                matrix.UpdateDataAndList();
                return "All missing prepreg layers are added to the design.";
            }
            else
            {
                return "All needed prepreg layers are already present in the design.";
            }
        }
        
    }
}
