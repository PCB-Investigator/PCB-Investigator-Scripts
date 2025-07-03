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
        /// Example method to retrieve and format layer thickness details by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetLayerThicknessDetails(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, bool showMetricUnit)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Create a string builder to store the layer thickness details
            StringBuilder sb = new StringBuilder();
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Iterate through all layers to get their thickness
            foreach (string layerName in step.GetAllLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (matrix.GetMatrixLayerContext(layerName) != MatrixLayerContext.Board) continue;
                if (matrix.IsSignalLayer(layerName) || matrix.GetMatrixLayerType(layerName) == MatrixLayerType.Dielectric)
                {
                    double heightMils = step.GetHeightOfLayer(layerName);
        
                    if (showMetricUnit)
                    {
                        sb.AppendLine("Layer '" + layerName + "': " + IMath.Mils2Micron(heightMils).ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + " µm");
                    }
                    else
                    {
                        sb.AppendLine("Layer '" + layerName + "': " + heightMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils");
                    }
                }
            }
            if (sb.Length == 0)
            {
                return "There are signal or prepreg layers in the current design.";
            }
            return sb.ToString();
        }

        /// <summary>
        /// Example method to retrieve and format the thickness of layers in a PCB design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetLayerThicknessDetails(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            // Create a string builder to store the layer thickness details
            StringBuilder sb = new StringBuilder();
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Get the unit the user wants to see in the UI (metric or imperial)
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            // Iterate through all layers to get their thickness
            foreach (string layerName in step.GetAllLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (matrix.GetMatrixLayerContext(layerName) != MatrixLayerContext.Board) continue;
                if (matrix.IsSignalLayer(layerName) || matrix.GetMatrixLayerType(layerName) == MatrixLayerType.Dielectric)
                {
                    double heightMils = step.GetHeightOfLayer(layerName);
        
                    if (showMetricUnit)
                    {
                        sb.AppendLine("Layer '" + layerName + "': " + IMath.Mils2Micron(heightMils).ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + " µm");
                    }
                    else
                    {
                        sb.AppendLine("Layer '" + layerName + "': " + heightMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils");
                    }
                }
            }
            if (sb.Length == 0)
            {
                return "There are signal or prepreg layers in the current design.";
            }
            return sb.ToString();
        }
        
}
}
