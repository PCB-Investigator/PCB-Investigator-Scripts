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
        /// Example method to select a specific net by name in the current design using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetByName(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            bool anySelected = false;
            // Define the layer filter for selecting nets
            List<MatrixLayerType> layerFilter = new List<MatrixLayerType>() { MatrixLayerType.Signal, MatrixLayerType.Power_ground, MatrixLayerType.Mixed, MatrixLayerType.Drill };
        
            // Iterate through all nets to find the specified net
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant() == netName.ToLowerInvariant())
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: layerFilter, fireSelectionChangedEvent: false);
                    anySelected = true;
                    break;
                }
            }
            if (anySelected)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return $"The net named '{netName}' has been selected in the current design.";
            }
            else
            {
                return $"The net named '{netName}' is not found in the current design.";
            }
        }

        /// <summary>
        /// Example method to select a specific net by name in the current design using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetByName(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string targetNetName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            bool anySelected = false;
            // Define the layer filter for selecting nets
            List<MatrixLayerType> layerFilter = new List<MatrixLayerType>() { MatrixLayerType.Signal, MatrixLayerType.Power_ground, MatrixLayerType.Mixed, MatrixLayerType.Drill };
        
            // Iterate through all nets to find the net named as specified by targetNetName
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant() == targetNetName.ToLowerInvariant())
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: layerFilter, fireSelectionChangedEvent: false);
                    anySelected = true;
                    break;
                }
            }
            if (anySelected)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return $"The net named '{targetNetName}' has been selected in the current design.";
            }
            else
            {
                return $"The net named '{targetNetName}' is not found in the current design.";
            }
        }
        
}
}
