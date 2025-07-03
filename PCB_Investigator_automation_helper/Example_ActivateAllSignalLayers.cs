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
        /// Example method to activate all signal layers in a given step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ActivateAllSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, IMatrix matrix)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Turn off all layers first
            step.TurnOffAllLayer();
            
            foreach (string signalLayer in matrix.GetAllSignalLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                ILayer layer = step.GetLayer(signalLayer);
                if (layer != null)
                {
                    layer.EnableLayer(activate: true);
                }
            }
            return "All signal layers are displayed and activated.";
        }

        /// <summary>
        /// Example method to activate all signal layers by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ActivateAllSignalLayers(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            // Turn off all layers first
            step.TurnOffAllLayer();
            foreach (string signalLayer in matrix.GetAllSignalLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                ILayer layer = step.GetLayer(signalLayer);
                if (layer != null)
                {
                    layer.EnableLayer(activate: true);
                }
            }
            return "All signal layers are displayed and activated.";
        }
        
}
}
