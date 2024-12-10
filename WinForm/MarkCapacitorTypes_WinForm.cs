// Dieses PCB-Investigator-Skript dient dazu, im geladenen Board-Layout alle vorhandenen Kondensatoren zu ermitteln, 
// zu klassifizieren und übersichtlich darzustellen. Zunächst werden alle Komponenten des aktuellen Steps durchsucht 
// und diejenigen Bauteile als Kondensatoren identifiziert, deren Referenz oder Wert dies nahelegen (z. B. "C..." oder 
// Kapazitätsangabe in Farad). Anschließend wird anhand der Netz-Topologie (Anschlüsse an Power-, Ground- oder 
// Signalleitungen) der spezifische Kondensatortyp bestimmt: Entkoppel-, Kopplungs-, Filterkondensator oder ein 
// sonstiger Typ.
//
// Die identifizierten Kondensatoren werden farblich im Layout hervorgehoben und in einer zusätzlichen Tabelle (Form) 
// aufgelistet. Diese Tabelle zeigt Referenz, Wert, Kondensatortyp sowie deren Position auf dem Board an. Durch Auswahl 
// eines Eintrags in der Tabelle kann der entsprechende Kondensator im Layout fokussiert und näher betrachtet werden. 
// Auf diese Weise unterstützt das Skript bei der Analyse und Überprüfung der Kondensatoren innerhalb der PCB-Designs.


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
// GUID=MarkCapType_638641531264786697
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

            // Dictionary to map capacitor types to colors for visualization
            Dictionary<string, Color> capacitorTypesColors = new Dictionary<string, Color>
            {
                { "Decoupling Capacitor", Color.Green },
                { "Coupling Capacitor", Color.Blue },
                { "Filter Capacitor", Color.Yellow },
                { "Other Capacitor", Color.Orange }
            };

            // Create and configure the DataGridView
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Define columns for the DataGridView
            DataGridViewTextBoxColumn refColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Reference",
                Name = "Reference"
            };
            dataGridView.Columns.Add(refColumn);

            DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Value",
                Name = "Value"
            };
            dataGridView.Columns.Add(valueColumn);

            DataGridViewTextBoxColumn typeColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Capacitor Type",
                Name = "CapacitorType"
            };
            dataGridView.Columns.Add(typeColumn);

            DataGridViewTextBoxColumn locColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Location",
                Name = "Location"
            };
            dataGridView.Columns.Add(locColumn);

            // Populate DataGridView with capacitor information
            foreach (ICMPObject component in step.GetAllCMPObjects())
            {
                if (IsCapacitor(component))
                {
                    string capacitorType = DetermineCapacitorType(step, component);
                    Color color = capacitorTypesColors.ContainsKey(capacitorType) ? capacitorTypesColors[capacitorType] : Color.Orange;
                    component.ObjectColor = color;
                    component.AddComponentAttribute("CapacitorType", capacitorType);

                    // Collect data
                    string reference = component.Ref;
                    string value = component.Value ?? "";
                    string location = $"({component.Position.X:F3}, {component.Position.Y:F3})";

                    // Add row to DataGridView
                    dataGridView.Rows.Add(reference, value, capacitorType, location);
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
                Text = "Capacitor Information",
                Size = new Size(600, 400)
            };
            form.Controls.Add(dataGridView);
            form.Show(parent.MainForm);
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

        private bool IsCapacitor(ICMPObject component)
        {
            string partName = component.PartName?.ToLower() ?? "";
            string value = component.Value?.ToLower() ?? "";
            return partName.StartsWith("c") || value.Contains("f");
        }

        private string DetermineCapacitorType(IStep step, ICMPObject capacitor)
        {
            if (IsDecouplingCapacitor(capacitor))
                return "Decoupling Capacitor";
            else if (IsCouplingCapacitor(step, capacitor))
                return "Coupling Capacitor";
            else if (IsFilterCapacitor(capacitor))
                return "Filter Capacitor";
            else
                return "Other Capacitor";
        }

        private bool IsDecouplingCapacitor(ICMPObject capacitor)
        {
            List<IPin> pins = capacitor.GetPinList();
            if (pins.Count != 2) return false;

            bool connectedToPower = false;
            bool connectedToGround = false;
            foreach (IPin pin in pins)
            {
                string netName = pin.GetNetNameOnIPin(capacitor);
                if (netName != null)
                {
                    if (IsPowerNet(netName)) connectedToPower = true;
                    if (IsGroundNet(netName)) connectedToGround = true;
                }
            }
            return connectedToPower && connectedToGround;
        }

        private bool IsCouplingCapacitor(IStep step, ICMPObject capacitor)
        {
            List<IPin> pins = capacitor.GetPinList();
            if (pins.Count != 2) return false;

            INet net1 = step.GetNet(pins[0].GetNetNameOnIPin(capacitor));
            INet net2 = step.GetNet(pins[1].GetNetNameOnIPin(capacitor));
            if (net1 == null || net2 == null || net1 == net2) return false;

            return IsSignalNet(net1) && IsSignalNet(net2);
        }

        private bool IsFilterCapacitor(ICMPObject capacitor)
        {
            List<IPin> pins = capacitor.GetPinList();
            if (pins.Count != 2) return false;

            foreach (IPin pin in pins)
            {
                string netName = pin.GetNetNameOnIPin(capacitor);
                if (netName != null && IsGroundNet(netName))
                {
                    IPin otherPin = (pin == pins[0]) ? pins[1] : pins[0];
                    string otherNetName = otherPin.GetNetNameOnIPin(capacitor);
                    if (otherNetName != null && IsSignalNet(otherNetName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsGroundNet(string netName)
        {
            string[] groundNames = { "gnd", "ground", "vss", "0v", "0" };
            foreach (string ground in groundNames)
            {
                if (netName.ToLower().Contains(ground))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPowerNet(string netName)
        {
            string[] powerNames = { "vcc", "vdd", "v+", "v-", "3v3", "5v", "12v", "1v8", "vpp", "avcc", "dvcc", "vbat" };
            foreach (string power in powerNames)
            {
                if (netName.ToLower().Contains(power))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsSignalNet(string netName)
        {
            return !IsPowerNet(netName) && !IsGroundNet(netName);
        }

        private bool IsSignalNet(INet net)
        {
            return IsSignalNet(net.NetName);
        }
    }
}
