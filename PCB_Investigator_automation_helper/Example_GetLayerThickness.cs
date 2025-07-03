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
        /// Example method to retrieve the thickness of a specified layer by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetLayerThickness(IPCBIWindow pcbi, IStep step, string layerName)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            
            // Get the matrix of the current job
            IMatrix matrix = pcbi.GetMatrix();
            
            // Get the layer type of the specified layer
            MatrixLayerType layerType = matrix.GetMatrixLayerType(layerName);
            if (layerType == MatrixLayerType.Unknown)
            {
                return "The layer '" + layerName + "' is not found in the current job.";
            }
            
            // Get the thickness of the specified layer
            double thicknessMils = step.GetHeightOfLayer(layerName);
            
            // Get the unit of measurement (mm or inch)
            bool showMetricUnit = pcbi.GetUnit();  // this is the unit, the user wants to see in the UI (true=metric, false=imperial)
            
            return "The thickness of the layer '" + layerName + "' is " +
                   (showMetricUnit ? IMath.Mils2Micron(thicknessMils).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + " µm"
                                   : thicknessMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils");
        }
        
    }
}
