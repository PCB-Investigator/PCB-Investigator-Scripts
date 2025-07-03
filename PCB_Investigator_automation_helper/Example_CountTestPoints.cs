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
        /// Example method to count test point components and pads by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountTestPoints(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            int totalTestPointsCMP = 0;
            int totalTestPointsPad = 0;
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                if (cmp.Ref.StartsWith("TP") || cmp.Ref.StartsWith("MP") || cmp.Ref.StartsWith("P"))
                {
                    totalTestPointsCMP++;
                }
            }
            IMatrix matrix = pcbi.GetMatrix();
            List<string> outerSigLayers = new List<string>() { matrix.GetTopSignalLayer(), matrix.GetBotSignalLayer() };
            foreach (string layer in outerSigLayers)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                IODBLayer sigLayer = step.GetLayer(layer) as IODBLayer;
                if (sigLayer == null) continue;
                foreach (IODBObject obj in sigLayer.GetAllLayerObjects())
                {
                    if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                    if (obj is IODBObject padObj && padObj.Type == IObjectType.Pad)
                    {
                        IAttributeElement testPointAttr = IAttribute.GetStandardAttribute(padObj, PCBI.FeatureAttributeEnum.test_point);
                        if (testPointAttr != null && testPointAttr.Value?.ToString().ToLowerInvariant() == "true")
                        {
                            totalTestPointsPad++;
                        }
                    }
                }
            }
            return "The total number of test point components is " + totalTestPointsCMP + " and the total number of test point pads is " + totalTestPointsPad + ".";
        }
        
    }
}
