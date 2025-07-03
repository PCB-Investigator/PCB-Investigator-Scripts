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
        /// Example method to count the number of single boards in a panel by using the PCB-Investigator API.
        /// </summary>
        private static string Example_CountSingleBoardsInPanel(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            int count = 0;
            // Check if the current step is a single board
            if (step.IsRootStep)
            {
                return "The current step is a single board.";
            }
            // Get the step-and-repeat structure of the current step
            Dictionary<IStep, List<MatrixD>> subStepInfo = step.GetMatrixListforAllStepAndRepeatChilds(pcbi);
            // Iterate through the sub-steps to count the single boards
            foreach (var kvp in subStepInfo)
            {
                if (kvp.Key.IsRootStep)
                {
                    count += kvp.Value.Count;
                }
            }
            return "There are " + count + " single boards in the panel.";
        }

        /// <summary>
        /// Example method to count single boards in a panel by using the PCB-Investigator API
        /// </summary>
        private static string Example_CountSingleBoardsInPanel(IPCBIWindow pcbi, IStep step)
        {
            // Check if a job is loaded
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
        
            // Check if the current step is a single board
            if (step.IsRootStep)
            {
                return "This is a single board step, not a panel.";
            }
        
            // Get the step-and-repeat structure of the current step
            var subStepInfo = step.GetMatrixListforAllStepAndRepeatChilds(pcbi);
            int boardCount = 0;
        
            // Iterate through the sub-steps to count the single boards
            foreach (var kvp in subStepInfo)
            {
                if (kvp.Key.IsRootStep)
                {
                    boardCount += kvp.Value.Count;
                }
            }
        
            // Return the count of single boards in the panel
            return boardCount > 0
                ? "This panel contains " + boardCount + " single board(s)."
                : "No sub-boards were found in this panel.";
        }
        
}
}
