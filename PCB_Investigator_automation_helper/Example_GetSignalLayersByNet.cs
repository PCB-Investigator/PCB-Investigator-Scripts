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
        /// Example method to retrieve signal layers used by a specified net by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetSignalLayersByNet(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string netName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Get the net with the specified name
            INet net = step.GetNet(netName);
            if (net != null)
            {
                IMatrix matrix = pcbi.GetMatrix();
                HashSet<string> signalLayers = new HashSet<string>();
                foreach (string netLayerName in net.GetAllUsedLayers())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (matrix.IsSignalLayer(netLayerName))
                    {
                        signalLayers.Add(netLayerName);
                    }
                }
                return "The signal layers used by the net '" + netName + "' are: " + string.Join(", ", signalLayers);
            }
            else
            {
                return "The net '" + netName + "' is not found in the current step.";
            }
        }
        
    }
}
