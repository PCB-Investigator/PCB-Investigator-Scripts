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
        /// Example method to retrieve and display detailed information about a specific component by using the PCB-Investigator API.
        /// </summary>
        private static string Example_GetComponentDetails(IPCBIWindow pcbi, IStep step, string componentReference)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the component exists in the current step
            if (step.GetAllCMPObjectsByReferenceDictionary().TryGetValue(componentReference, out ICMPObject cmp))
            {
                // Get the unit the user wants to see in the UI (metric or imperial)
                bool showMetricUnit = pcbi.GetUnit();  //this is the unit, the user wants to see in the UI (true=metric, false=imperial)
                // Check if the component is populated
                bool isPopulated = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_ignore) == null && IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.no_pop) == null;
                // Get the mount type of the component
                string compType = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_mount_type)?.Value?.ToString() ?? "Unknown";
        
                // Get the bounds of the component
                RectangleD bodyBoundsMils = cmp.GetBodyBoundsD(); //always in mils
                // Get the position of the component
                PointD positionMils = cmp.GetPosition(); //always in mils
                // Get the height of the component
                double heightMils = cmp.CompHEIGHT;
        
                // Create a string builder to store the component details
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Reference: " + cmp.Ref);
                sb.AppendLine("Package: " + cmp.UsedPackageName);
                sb.AppendLine("Part Number: " + cmp.PartName);
                sb.AppendLine("X Position: " + (showMetricUnit ? IMath.Mils2MM(positionMils.X).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                                                            : (positionMils.X / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch"));
                sb.AppendLine("Y Position: " + (showMetricUnit ? IMath.Mils2MM(positionMils.Y).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                                                            : (positionMils.Y / 1000).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " inch"));
                sb.AppendLine("Body X Size: " + (showMetricUnit ? IMath.Mils2MM(bodyBoundsMils.Width).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                                                             : bodyBoundsMils.Width.ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mils"));
                sb.AppendLine("Body Y Size: " + (showMetricUnit ? IMath.Mils2MM(bodyBoundsMils.Height).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                                                             : bodyBoundsMils.Height.ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mils"));
                sb.AppendLine("Height: " + (showMetricUnit ? IMath.Mils2MM(heightMils).ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " mm"
                                                        : heightMils.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " mils"));
                sb.AppendLine("Rotation: " + cmp.Rotation.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "°");
                sb.AppendLine("Populated: " + (isPopulated ? "Yes" : "No"));
                sb.AppendLine("Pin Count: " + cmp.GetPinCount());
                sb.AppendLine("Side: " + (cmp.PlacedTop ? "Top" : "Bottom"));
                sb.AppendLine("Polarity Pin: " + (cmp.PolarityMarker <= 0 ? "No Polarity" : cmp.PolarityMarker));
                sb.AppendLine("Mount Type: " + compType);
                sb.AppendLine("------Dynamic Attributed------");
                // Add all dynamic attributes of the component
                foreach (IAttributeElement attr in IAttribute.GetAllAttributes(cmp, pcbi))
                {
                    sb.AppendLine(attr.DisplayName + ": " + attr.Value?.ToString() ?? "");
                }
                sb.AppendLine("------Dynamic Properties------");
                // Add all dynamic properties of the component
                foreach (KeyValuePair<string, IEDA_PRP> prpKvP in IAttribute.GetProperties(cmp))
                {
                    sb.AppendLine(prpKvP.Value.NAME + ": " + prpKvP.Value.VALUE_STRING);
                }
        
                return sb.ToString();
            }
            else
            {
                return $"The component {componentReference} is not found in the current step.";
            }
        }
        
    }
}
