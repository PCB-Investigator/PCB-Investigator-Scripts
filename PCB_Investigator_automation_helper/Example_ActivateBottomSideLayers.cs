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
        /// Example method to activate all bottom side layers in the current step by using the PCB-Investigator API.
        /// </summary>
        private static string Example_ActivateBottomSideLayers(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            // Turn off all layers in the current step
            step.TurnOffAllLayer();
        
            // Get the names of the bottom side layers
            string botSigLayer = matrix.GetBotSignalLayer();
            string botMaskLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Solder_mask, TopSide: false, context: MatrixLayerContext.Board);
            string botPasteLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Solder_paste, TopSide: false, context: MatrixLayerContext.Board);
            string botSilkLayer = matrix.FindSideLayerName(relType: MatrixLayerType.Silk_screen, TopSide: false, context: MatrixLayerContext.Board);
            string botCompLayer = matrix.GetBotComponentLayer();
        
            // Enable the bottom side layers
            step.GetLayer(botSigLayer)?.EnableLayer(activate: true);
            step.GetLayer(botMaskLayer)?.EnableLayer(activate: true);
            step.GetLayer(botPasteLayer)?.EnableLayer(activate: true);
            step.GetLayer(botSilkLayer)?.EnableLayer(activate: true);
            step.GetLayer(botCompLayer)?.EnableLayer(activate: true);
            matrix.GetAllDrillLayersForThisLayer(botSigLayer).ForEach(x => step.GetLayer(x)?.EnableLayer(true));
        
            return "All bot side layers have been activated in the current step.";
        }
        
    }
}
