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
        /// Example method to select all resistors in the current step by using the PCB-Investigator API
        /// </summary>
        private static string Example_SelectAllResistorsInCurrentStep(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            List<ICMPObject> cmpList = new List<ICMPObject>();
            // Iterate through all components to find resistors
            foreach (ICMPObject c in step.GetAllCMPObjects())
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested) return "Operation was cancelled.";
        
                string description = IAttribute.GetProperty(c, "DESCRIPTION")?.VALUE_STRING?.ToUpperInvariant() ?? "";
        
                // Check if the optional PART_CATEGORY property exists and contains 'resistor'
                if (IAttribute.GetProperty(c, "PART_CATEGORY")?.VALUE_STRING?.ToLowerInvariant().Contains("resistor") ?? false)
                {
                    cmpList.Add(c);
                }
                // Alternativ, check if the optional DESCRIPTION property contains 'RES' or starts with 'RES'
                else if (description.Contains(" RES ") || description.StartsWith("RES "))
                {
                    cmpList.Add(c);
                }
                // Alternativ, check if the reference starts with 'R' followed by a digit, has more than 1 pin and an even pin count
                else if (c.Ref.StartsWith("R") && c.Ref.Length > 1 && char.IsDigit(c.Ref[1]) && c.GetPinCount() > 1 && (c.GetPinCount() % 2 == 0))
                {
                    cmpList.Add(c);
                }
                // Alternativ, check if the reference starts with 'RN' followed by a digit, has more than 2 pins and an even pin count (Resistor Array)
                else if (c.Ref.StartsWith("RN") && c.Ref.Length > 2 && char.IsDigit(c.Ref[2]) && c.GetPinCount() > 2 && (c.GetPinCount() % 2 == 0))
                {
                    cmpList.Add(c);
                }
            }
            if (cmpList.Count > 0)
            {
                bool setSelected = true;
                // Select all found resistors
                foreach (ICMPObject cmp in cmpList)
                {
                    cmp.Select(setSelected);
                }
                // Update the selection and view
                pcbi.UpdateSelection();
                pcbi.UpdateView(NeedFullRedraw: true);
                return "All resistors have been selected in the current step.";
            }
            else
            {
                return "There are no resistors in the current step.";
            }
        }
        
    }
}
