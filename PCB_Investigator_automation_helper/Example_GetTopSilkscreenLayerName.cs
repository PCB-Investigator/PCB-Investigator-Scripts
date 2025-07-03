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
        /// Example method to retrieve the name of the top silkscreen layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetTopSilkscreenLayerName(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            string topSilkScreenLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Silk_screen, TopSide: true, context: MatrixLayerContext.Board);
            if (string.IsNullOrWhiteSpace(topSilkScreenLayer))
            {
                return "The top silkscreen layer is not found in the current job.";
            }
            else
            {
                return "The name of the top silkscreen layer is '" + topSilkScreenLayer + "'.";
            }
        }

        /// <summary>
        /// Example method to retrieve the name of the top silkscreen layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetTopSilkscreenLayerName(IPCBIWindow pcbi, IStep step, MatrixLayerType relType, bool TopSide, MatrixLayerContext context)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            string topSilkScreenLayer = matrix.FindSideLayerName(relType: relType, TopSide: TopSide, context: context);
            if (string.IsNullOrWhiteSpace(topSilkScreenLayer))
            {
                return "The top silkscreen layer is not found in the current job.";
            }
            else
            {
                return "The name of the top silkscreen layer is '" + topSilkScreenLayer + "'.";
            }
        }

        /// <summary>
        /// Example method to retrieve the name of the top silkscreen layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetTopSilkScreenLayerName(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            IMatrix matrix = pcbi.GetMatrix();
            string topSilkScreenLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Silk_screen, TopSide: true, context: MatrixLayerContext.Board);
            if (string.IsNullOrWhiteSpace(topSilkScreenLayer))
            {
                return "The top silkscreen layer is not found in the current job.";
            }
            else
            {
                return "The name of the top silkscreen layer is '" + topSilkScreenLayer + "'.";
            }
        }
        
}
}
