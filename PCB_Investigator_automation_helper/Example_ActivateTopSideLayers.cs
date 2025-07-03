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
        /// Example method to activate all top side layers in the current step by using the PCB-Investigator API
        /// </summary>
        private static string Example_ActivateTopSideLayers(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Turn off all layers in the current step
            step.TurnOffAllLayer();
        
            // Get the names of the top side layers
            string topSigLayer = matrix.GetTopSignalLayer();
            string topMaskLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Solder_mask, TopSide: true, context: MatrixLayerContext.Board);
            string topPasteLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Solder_paste, TopSide: true, context: MatrixLayerContext.Board);
            string topSilkLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Silk_screen, TopSide: true, context: MatrixLayerContext.Board);
            string topCompLayer = matrix.GetTopComponentLayer();
        
            // Enable the top side layers
            step.GetLayer(topSigLayer)?.EnableLayer(activate: true);
            step.GetLayer(topMaskLayer)?.EnableLayer(activate: true);
            step.GetLayer(topPasteLayer)?.EnableLayer(activate: true);
            step.GetLayer(topSilkLayer)?.EnableLayer(activate: true);
            step.GetLayer(topCompLayer)?.EnableLayer(activate: true);
            matrix.GetAllDrillLayersForThisLayer(topSigLayer).ForEach(x => step.GetLayer(x)?.EnableLayer(true));
        
            return "All top side layers have been activated in the current step.";
        }
        
    }
}
