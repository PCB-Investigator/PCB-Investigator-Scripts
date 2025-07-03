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
        /// Example method to select all traces with a specified width in the top signal layer by using the PCB-Investigator API
        /// </summary>
        private static string Example_SelectTracesByWidthInTopSignalLayer(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, double traceWidthMicron)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            string topSignalLayer = matrix.GetTopSignalLayer();
            if (topSignalLayer == null)
            {
                return "The top signal layer is not defined in the current job.";
            }
            // Get the top signal layer
            ILayer layer = step.GetLayer(topSignalLayer);
            if (layer == null)
            {
                return "The top signal layer '" + topSignalLayer + "' is not found in the current step.";
            }
            // Clear the selection
            step.ClearSelection();
            // Get the trace width in mils
            double traceWidthMils = IMath.Micron2Mils(traceWidthMicron);
            bool anyFound = false;
            // Select all traces with the specified width in the top signal layer
            foreach (IODBObject obj in layer.GetAllLayerObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (obj is IODBObject traceObj && traceObj.Type == IObjectType.Line)
                {
                    // Check if the trace width is equal to the specified width (±0.001 mils tolerance)
                    if (Math.Abs(traceObj.GetDiameter() - traceWidthMils) < 0.001) //always in mils
                    {
                        traceObj.Select(true);
                        anyFound = true;
                    }
                }
            }
            if (!anyFound)
            {
                return "No " + traceWidthMicron + "µm traces are found in the top signal layer.";
            }
            // Zoom to the selected objects
            pcbi.ZoomToSelection();
            return "All " + traceWidthMicron + "µm traces in the top signal layer are selected.";
        }
        
    }
}
