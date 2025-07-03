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
        /// Example method to check for negative objects on signal layers of the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CheckForNegativeObjectsOnSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Initialize the flag for negative objects
            bool hasNegativeObjects = false;
            // Iterate through all signal layers
            foreach (string sigLayerNames in matrix.GetAllSignalLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                IODBLayer layer = step.GetLayer(sigLayerNames) as IODBLayer;
                if (layer == null) continue;
                // Iterate through all objects in the layer
                foreach (IObject obj in layer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject odbObj)
                    {
                        // Check if the object is negative
                        if (!odbObj.Positive)
                        {
                            hasNegativeObjects = true;
                            break;
                        }
                    }
                }
                if (hasNegativeObjects) break;
            }
            // Return the result
            if (hasNegativeObjects)
            {
                return "There are negative objects on signal layers of the current step.";
            }
            else
            {
                return "There are no negative objects on signal layers of the current step.";
            }
        }
        
    }
}
