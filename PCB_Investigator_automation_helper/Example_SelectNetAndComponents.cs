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
        /// Example method to select a specific net and its components in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectNetAndComponents(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string targetNetName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            bool anySelected = false;
        
            // Clear the current selection
            step.ClearSelection(FireEvents: false);
        
            // Define the layer filter for selecting nets
            List<MatrixLayerType> layerFilter = new List<MatrixLayerType>()
                { MatrixLayerType.Signal, MatrixLayerType.Power_ground, MatrixLayerType.Mixed, MatrixLayerType.Drill };
        
            // Iterate through all nets to find the net named "OUT1"
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (net.NetName.ToLowerInvariant() == targetNetName.ToLowerInvariant())
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: layerFilter, fireSelectionChangedEvent: false);
                    // Select the associated components
                    net.ComponentList?.DistinctBy(ni => ni.ICMP?.Ref)?.ToList()
                        ?.ForEach(ni => ni.ICMP?.Select(select: true));
                    anySelected = true;
                    break;
                }
            }
            pcbi.UpdateSelection();
            if (anySelected)
            {
                // Update the view to show only the selected components and elements
                pcbi.IColorsetting.DrawOnlySelectedCMPs = true;
                pcbi.IColorsetting.DrawOnlySelectedElements = true;
                pcbi.ZoomToSelection();
                return $"The net named '{targetNetName}' has been selected in the current design.";
            }
            else
            {
                pcbi.UpdateView(NeedFullRedraw: true);
                return $"The net named '{targetNetName}' is not found in the current design.";
            }
        }
        
    }
}
