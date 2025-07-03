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
        /// Example method to select and highlight ground nets in the current design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SelectAndHighlightGroundNets(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            bool anySelected = false;
            List<string> foundNets = new List<string>();
            // Define the layer filter for selecting nets
            List<MatrixLayerType> layerFilter = new List<MatrixLayerType>() { MatrixLayerType.Signal, MatrixLayerType.Power_ground, MatrixLayerType.Mixed, MatrixLayerType.Drill };
            // Iterate through all nets to find ground nets
            foreach (INet net in step.GetNets())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                string netNameLower = net.NetName.ToLowerInvariant();
                if (netNameLower.Contains("gnd") || netNameLower.Contains("ground"))
                {
                    // Select the net
                    net.SelectNet(onlyTheseTypesOrNull: layerFilter, fireSelectionChangedEvent: false);
                    anySelected = true;
                    foundNets.Add(net.NetName);
                }
            }
            if (anySelected)
            {
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "Following ground nets have been selected in the current design: " + string.Join(", ", foundNets);
            }
            else
            {
                return "There are no recognized ground nets in the current step.";
            }
        }
        
    }
}
