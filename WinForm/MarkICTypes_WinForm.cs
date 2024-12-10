/*
 * This PCB-Investigator script is designed for classifying and visualizing IC components on a PCB. 
 * It provides the following functionalities:
 *
 * 1. Checks if a job is loaded and ensures a step is active.
 * 2. Categorizes ICs based on their type (e.g., Microcontroller, Memory, Analog IC) by analyzing 
 *    component names or package types.
 * 3. Assigns colors to categories for visual distinction.
 * 4. Creates an interactive DataGridView displaying detailed information about the ICs, such as 
 *    reference, part number, package type, pin count, and position.
 * 5. Uses color-coded markings to highlight components in the PCB view.
 * 
 * This script is developed to assist engineers and designers in efficiently identifying and analyzing 
 * ICs on a PCB. It combines automation and visual representation to streamline the design process.
 *
 * Prerequisites:
 * - A job must be loaded in PCB-Investigator.
 * - Step data must be available.
 *
 * Notes:
 * - The script supports common IC package types such as QFP, BGA, SOIC, etc.
 * - The classification logic can be customized to meet specific requirements.
 *
 * Author: Guenther
 * Date: 06.12.2024
 */

//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script (Synchronous)
// Created on 10.10.2024
// Author guenther
// SDK online reference http://manual.pcb-investigator.com/InterfaceDocumentation/index.php
// SDK http://www.pcb-investigator.com/sdk-participate
// Updated: 10.10.2024
// Description: find at the end of code
//-----------------------------------------------------------------------------------
// Favorite scripts can be placed in the ribbon menu. Therefore a unique GUID and a ButtonEnabled state is mandatory:
// GUID=MarkCType_638641531264786697
// ButtonEnabled=1   (Button enabled if: 1=Design is loaded, 2=Always, 4=Design contains components, 8=Loaded step is a panel, 16=Element is selected, 32=Component is selected)
// AutoStart=false

using System;
using System.Collections.Generic;
using System.Text;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using System.Windows.Forms;
using System.Drawing;
using PCBI.Automation;
using System.IO;
using System.Drawing.Drawing2D;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            if (!parent.JobIsLoaded)
            {
                MessageBox.Show("Please load a job first.");
                return;
            }

            IStep step = parent.GetCurrentStep();
            if (step == null)
            {
                MessageBox.Show("No step loaded.");
                return;
            }

            // Define IC categories and their colors for visualization
            Dictionary<string, Color> icCategoriesColors = new Dictionary<string, Color>
            {
                { "Microcontroller", Color.Green },
                { "Memory", Color.Blue },
                { "Power Management", Color.Yellow },
                { "Analog IC", Color.Magenta },
                { "Other IC", Color.Orange }
            };

            // Create and configure the DataGridView
            DataGridView dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AllowUserToAddRows = false;

            // Define the columns
            DataGridViewTextBoxColumn refColumn = new DataGridViewTextBoxColumn();
            refColumn.HeaderText = "Reference";
            refColumn.Name = "Reference";
            dataGridView.Columns.Add(refColumn);

            DataGridViewTextBoxColumn partNumberColumn = new DataGridViewTextBoxColumn();
            partNumberColumn.HeaderText = "Part Number";
            partNumberColumn.Name = "PartNumber";
            dataGridView.Columns.Add(partNumberColumn);

            DataGridViewTextBoxColumn categoryColumn = new DataGridViewTextBoxColumn();
            categoryColumn.HeaderText = "IC Category";
            categoryColumn.Name = "ICCategory";
            dataGridView.Columns.Add(categoryColumn);

            DataGridViewTextBoxColumn packageColumn = new DataGridViewTextBoxColumn();
            packageColumn.HeaderText = "Package Type";
            packageColumn.Name = "PackageType";
            dataGridView.Columns.Add(packageColumn);

            DataGridViewTextBoxColumn pinsColumn = new DataGridViewTextBoxColumn();
            pinsColumn.HeaderText = "Pin Count";
            pinsColumn.Name = "PinCount";
            dataGridView.Columns.Add(pinsColumn);

            DataGridViewTextBoxColumn locColumn = new DataGridViewTextBoxColumn();
            locColumn.HeaderText = "Location";
            locColumn.Name = "Location";
            dataGridView.Columns.Add(locColumn);

            foreach (ICMPObject component in step.GetAllCMPObjects())
            {
                if (IsIC(component))
                {
                    string icCategory = DetermineICCategory(component);
                    Color color = icCategoriesColors.ContainsKey(icCategory) ? icCategoriesColors[icCategory] : Color.Orange;

                    component.ObjectColor = color;
                    component.AddComponentAttribute("ICCategory", icCategory);

                    // Collect data
                    string reference = component.Ref;
                    string partNumber = component.PartName ?? "";
                    string packageType = component.UsedPackageName ?? "";
                    int pinCount = component.GetPinList().Count;
                    string location = $"({component.Position.X:F3}, {component.Position.Y:F3})";

                    // Add row to DataGridView
                    dataGridView.Rows.Add(reference, partNumber, icCategory, packageType, pinCount.ToString(), location);
                }
            }

            parent.UpdateView();

            // Create and show the form with the DataGridView
            Form form = new Form();
            form.Text = "IC Component Classification";
            form.Size = new Size(800, 600);
            form.Controls.Add(dataGridView);

            form.ShowDialog();
        }

        private bool IsIC(ICMPObject component)
        {
            // Determine if a component is an IC based on its package type or other criteria
            string packageType = component.UsedPackageName?.ToLower() ?? "";
            int pinCount = component.GetPinList().Count;

            // Common IC package types
            string[] icPackages = { "qfp", "bga", "soic", "sop", "dip", "lcc", "tsop", "qfn", "lga", "dfn" };

            // Identify as IC if the package type matches common IC packages and pin count is above a threshold
            if (pinCount >= 8)
            {
                foreach (string icPackage in icPackages)
                {
                    if (packageType.Contains(icPackage))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string DetermineICCategory(ICMPObject component)
        {
            string partNumber = component.PartName?.ToLower() ?? "";

            // Simple classification based on keywords in part numbers
            if (partNumber.Contains("mcu") || partNumber.Contains("microcontroller") || partNumber.StartsWith("pic") || partNumber.StartsWith("stm"))
                return "Microcontroller";
            else if (partNumber.Contains("mem") || partNumber.Contains("ram") || partNumber.Contains("rom") || partNumber.Contains("flash"))
                return "Memory";
            else if (partNumber.Contains("pmic") || partNumber.Contains("regulator") || partNumber.Contains("ldr") || partNumber.Contains("dc-dc"))
                return "Power Management";
            else if (partNumber.Contains("op") || partNumber.Contains("amp") || partNumber.Contains("adc") || partNumber.Contains("dac"))
                return "Analog IC";
            else
                return "Other IC";
        }
    }
}
