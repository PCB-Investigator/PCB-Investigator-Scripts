/*
 * This PCB-Investigator script is designed to identify, classify, and visualize resistors on a PCB. 
 * It provides the following functionalities:
 *
 * 1. Verifies that a job and step are loaded in PCB-Investigator before proceeding.
 * 2. Classifies resistors into categories based on their functionality:
 *    - Pull Down
 *    - Pull Up
 *    - In Line
 *    - Other Resistor
 * 3. Assigns colors to each resistor category for clear visual representation.
 * 4. Displays detailed resistor information in an interactive table (DataGridView), including:
 *    - Reference
 *    - Value
 *    - Resistor Type
 *    - Location
 * 5. Enables selection of resistors from the table to highlight them in the PCB design.
 * 
 * This script is intended to enhance the analysis and design process by providing a 
 * structured overview of resistors and their connections within a PCB layout.
 *
 * Prerequisites:
 * - A valid PCB job must be loaded in PCB-Investigator.
 * - The step data must be active and accessible.
 *
 * Notes:
 * - Resistor classification logic includes power/ground nets for pull-up and pull-down identification.
 * - The script uses a combination of net analysis and pin connections to determine resistor functionality.
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
// GUID=MarkRType_638641531264786697
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

            Dictionary<string, Color> functionColors = new Dictionary<string, Color>
            {
                { "Pull Down", Color.Green },
                { "Pull Up", Color.Blue },
                { "In Line", Color.Yellow },
                { "Other Resistor", Color.Orange }
            };

            // Create and configure the DataGridView
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Define the columns
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reference", Name = "Reference" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", Name = "Value" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Resistor Type", Name = "ResistorType" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Location", Name = "Location" });

            // Populate the DataGridView
            foreach (ICMPObject component in step.GetAllCMPObjects())
            {
                if (IsResistor(component))
                {
                    string function = DetermineResistorFunction(step, component);
                    Color color = functionColors.ContainsKey(function) ? functionColors[function] : Color.Orange;
                    component.ObjectColor = color;
                    component.AddComponentAttribute("Function", function);

                    string reference = component.Ref;
                    string value = component.Value ?? "";
                    string location = $"({component.Position.X:F3}, {component.Position.Y:F3})";

                    dataGridView.Rows.Add(reference, value, function, location);
                }
            }

            parent.UpdateView();

            // Handle row selection in the DataGridView
            dataGridView.SelectionChanged += (sender, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    string selectedRef = dataGridView.SelectedRows[0].Cells["Reference"].Value.ToString();
                    SelectComponent(parent, step, selectedRef);
                    parent.UpdateView();
                }
            };

            // Create and show the form with the DataGridView
            Form form = new Form
            {
                Text = "Resistor Information",
                Size = new Size(600, 400)
            };
            form.Controls.Add(dataGridView);
            form.ShowDialog();
        }

        // Method to select the component in the PCB design
        private void SelectComponent(IPCBIWindow parent, IStep step, string reference)
        {
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                cmp.Select(cmp.Ref.Equals(reference, StringComparison.OrdinalIgnoreCase));
            }
             parent.ZoomToSelection();
        }

        private bool IsResistor(ICMPObject component)
        {
            string partName = component.Ref?.ToLower() ?? "";
            string value = component.Value?.ToLower() ?? "";
            return partName.StartsWith("r") || value.Contains("ohm");
        }

        private string DetermineResistorFunction(IStep step, ICMPObject resistor)
        {
            if (IsPullDown(resistor))
                return "Pull Down";
            else if (IsPullUp(resistor))
                return "Pull Up";
            else if (IsInLine(step, resistor))
                return "In Line";
            else
                return "Other Resistor";
        }

        private bool IsGroundNet(string netName)
        {
            string[] groundNames = { "gnd", "ground", "vss", "agnd", "dgnd", "pgnd", "0v", "0" };
            return Array.Exists(groundNames, ground => netName.ToLower().Contains(ground));
        }

        private bool IsPowerNet(string netName)
        {
            string[] powerNames = { "vcc", "vdd", "v+", "v-", "3v3", "5v", "12v", "1v8", "vpp", "avcc", "dvcc", "vbat" };
            return Array.Exists(powerNames, power => netName.ToLower().Contains(power));
        }

        private bool IsPullDown(ICMPObject resistor)
        {
            List<IPin> pins = resistor.GetPinList();
            if (pins.Count != 2) return false;

            foreach (IPin pin in pins)
            {
                string net = pin.GetNetNameOnIPin(resistor);
                if (net != null && IsGroundNet(net))
                {
                    IPin otherPin = (pin == pins[0]) ? pins[1] : pins[0];
                    string otherNet = otherPin.GetNetNameOnIPin(resistor);
                    if (otherNet != null && !IsPowerNet(otherNet))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPullUp(ICMPObject resistor)
        {
            List<IPin> pins = resistor.GetPinList();
            if (pins.Count != 2) return false;

            foreach (IPin pin in pins)
            {
                string net = pin.GetNetNameOnIPin(resistor);
                if (net != null && IsPowerNet(net))
                {
                    IPin otherPin = (pin == pins[0]) ? pins[1] : pins[0];
                    string otherNet = otherPin.GetNetNameOnIPin(resistor);
                    if (otherNet != null && !IsGroundNet(otherNet))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInLine(IStep step, ICMPObject resistor)
        {
            List<IPin> pins = resistor.GetPinList();
            if (pins.Count != 2) return false;

            INet net1 = step.GetNet(pins[0].GetNetNameOnIPin(resistor));
            INet net2 = step.GetNet(pins[1].GetNetNameOnIPin(resistor));
            if (net1 == null || net2 == null || net1 == net2) return false;

            return IsSignalNet(net1) && IsSignalNet(net2) && AreNetsConnectedOnlyThroughResistor(step, net1, net2, resistor);
        }

        private bool IsSignalNet(INet net)
        {
            return !IsPowerNet(net.NetName) && !IsGroundNet(net.NetName);
        }

        private bool AreNetsConnectedOnlyThroughResistor(IStep step, INet net1, INet net2, ICMPObject resistor)
        {
            int netID = net1.GetNetNumber();
            List<ICMPObject> componentsOnNet1 = step.GetAllCMPsWithNetConnectionTo(netID);
            foreach (ICMPObject component in componentsOnNet1)
            {
                if (component != resistor && IsComponentConnectedToNet(component, net2))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsComponentConnectedToNet(ICMPObject component, INet net)
        {
            foreach (IPin pin in component.GetPinList())
            {
                if (pin.GetNetNameOnIPin(component) == net.NetName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
