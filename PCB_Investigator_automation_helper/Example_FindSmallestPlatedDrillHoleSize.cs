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
        /// Example method to find the smallest plated drill hole size in the design by using the PCB-Investigator API.
        /// </summary>
        private static string Example_FindSmallestPlatedDrillHoleSize(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            double minPlatedDrillSizeMils = double.MaxValue;
        
            IMatrix matrix = pcbi.GetMatrix();
            foreach (string drillLayerName in matrix.GetAllDrillLayerNames())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                ILayer drillLayer = step.GetLayer(drillLayerName);
                if (drillLayer == null) continue;
                foreach (IODBObject obj in drillLayer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject drillObj && drillObj.Type == IObjectType.Pad)
                    {
                        // Check if the object is a round drill
                        if (drillObj.GetSymbol()?.Type == PCBI.Symbol_Type.r)
                        {
                            // Check if the drill is plated
                            IAttributeElement drillTypeAttr = IAttribute.GetStandardAttribute(drillObj, PCBI.FeatureAttributeEnum.drill);
                            if (drillTypeAttr != null && drillTypeAttr.Value?.ToString() == "plated")
                            {
                                double drillDiameterMils = drillObj.GetDiameter(); //always in mils
                                if (drillDiameterMils < minPlatedDrillSizeMils)
                                {
                                    minPlatedDrillSizeMils = drillDiameterMils;
                                }
                            }
                        }
                    }
                }
            }
        
            bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
        
            if (showMetricUnit)
            {
                return "The smallest plated drill hole size in the design is " + IMath.Mils2MM(minPlatedDrillSizeMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm.";
            }
            else
            {
                return "The smallest plated drill hole size in the design is " + minPlatedDrillSizeMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils.";
            }
        }
        
    }
}
