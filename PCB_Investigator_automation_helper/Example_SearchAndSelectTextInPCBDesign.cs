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
        /// Example method to search and select objects with specific text in a PCB design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_SearchAndSelectTextInPCBDesign(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string searchKeyword)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            //Get the matrix
            IMatrix matrix = pcbi.GetMatrix();
            bool selectionCleared = false;
            foreach (string boardLayerName in matrix.GetAllBoardLayerNames(ToLower: true))
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                ILayer boardLayer = step.GetLayer(boardLayerName);
                if (boardLayer == null) continue;
                foreach (IODBObject obj in boardLayer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    IObjectSpecificsD specs = obj.GetSpecificsD();
                    // Check if the object is a text with the text 'author'
                    if (specs is ITextSpecificsD textSpecs && textSpecs.Text.ToLowerInvariant() == searchKeyword)
                    {
                        if (!selectionCleared) { step.ClearSelection(); selectionCleared = true; }
                        obj.Select(true);
                    }
                    // Check if the object is a line that belongs to a text with the string 'author'
                    else if (specs is ILineSpecificsD lineSpecs &&
                        (IAttribute.GetStandardAttribute(obj, PCBI.FeatureAttributeEnum.text)?.Value?.ToString().ToLowerInvariant() == searchKeyword ||
                         IAttribute.GetStandardAttribute(obj, PCBI.FeatureAttributeEnum._string)?.Value?.ToString().ToLowerInvariant() == searchKeyword))
                    {
                        if (!selectionCleared) { step.ClearSelection(); selectionCleared = true; }
                        obj.Select(true);
                    }
                }
            }
        
            // Zoom to the selected objects
            if (selectionCleared)
                pcbi.ZoomToSelection();
        
            return selectionCleared ? "The text '" + searchKeyword + "' is found and selected in the design." : "The text '" + searchKeyword + "' is not found in the design.";
        }
        
    }
}
