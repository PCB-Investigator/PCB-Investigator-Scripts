//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script (Synchronous)
// Created on 13.03.2025
// Author: guent
// SDK online reference: http://manual.pcb-investigator.com/InterfaceDocumentation/index.php
// SDK: http://www.pcb-investigator.com/sdk-participate
// Updated: 13.03.2025
// Description: IPC-2221 Calculator with option to specify either temperature rise or current
//-----------------------------------------------------------------------------------
// GUID=IPC_2221_Calculator_638774573007653107
// ButtonEnabled=1   (Button enabled if: 1=Design is loaded)
// AutoStart=false

using System;
using System.Collections.Generic;
using System.Text;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using System.Windows.Forms;
using System.Drawing;
using PCBI.Automation;
using PCBIScript.DebugHelp; 
using System.IO;
using System.Linq;
using System.Drawing.Drawing2D;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        public void Execute(IPCBIWindow parent)
        {
            if (parent == null || !parent.JobIsLoaded) return;

            IStep step = parent.GetCurrentStep();
            if (step == null)
            {
                MessageBox.Show("No step loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<IODBObject> selectedElements = step.GetSelectedElements();
            if (selectedElements.Count == 0)
            {
                MessageBox.Show("No elements selected. Please select at least one trace.", 
                              "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create input dialog
            Form dialogForm = new Form
            {
                Text = "IPC-2221 Calculator",
                Width = 750,
                Height = 900,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Input mode selection
            RadioButton currentMode = new RadioButton { Text = "Specify Current (A)", Left = 20, Top = 20, Width = 150, Checked = true };
            RadioButton tempMode = new RadioButton { Text = "Specify Temp Rise (°C)", Left = 180, Top = 20, Width = 150 };
            dialogForm.Controls.Add(currentMode);
            dialogForm.Controls.Add(tempMode);

            // Value input
            Label valueLabel = new Label { Left = 20, Top = 50, Width = 150 };
            TextBox valueTextBox = new TextBox { Left = 180, Top = 50, Width = 100, Text = "1" };
            dialogForm.Controls.Add(valueLabel);
            dialogForm.Controls.Add(valueTextBox);

            currentMode.CheckedChanged += (s, e) => {
                valueLabel.Text = currentMode.Checked ? "Current (A):" : "Temp Rise (°C):";
                valueTextBox.Text = currentMode.Checked ? "1" : "20";
            };
            valueLabel.Text = "Current (A):";

            // Copper thickness input
            Label thicknessLabel = new Label { Text = "Copper Thickness (µm):", Left = 20, Top = 80, Width = 150 };
            TextBox thicknessTextBox = new TextBox { Left = 180, Top = 80, Width = 100, Text = "35" };
            dialogForm.Controls.Add(thicknessLabel);
            dialogForm.Controls.Add(thicknessTextBox);

            // Selected elements list
            Label selectedElementsLabel = new Label { Text = "Selected Elements:", Left = 20, Top = 120, Width = 200 };
            ListView elementListView = new ListView
            {
                Left = 20, Top = 150, Width = 700, Height = 200, View = View.Details, FullRowSelect = true
            };
            elementListView.Columns.Add("Index", 60);
            elementListView.Columns.Add("Layer", 150);
            elementListView.Columns.Add("Type", 100);
            elementListView.Columns.Add("Width (mil)", 100);
            elementListView.Columns.Add("Width (mm)", 100);
            dialogForm.Controls.Add(selectedElementsLabel);
            dialogForm.Controls.Add(elementListView);

            // Results list
            Label resultsListLabel = new Label { Text = "Calculation Results:", Left = 20, Top = 360, Width = 200 };
            ListView resultsListView = new ListView
            {
                Left = 20, Top = 390, Width = 700, Height = 150, View = View.Details, FullRowSelect = true
            };
            resultsListView.Columns.Add("Index", 60);
            resultsListView.Columns.Add("Layer", 150);
            resultsListView.Columns.Add("Width (mm)", 80);
            resultsListView.Columns.Add("Cross Section (mil²)", 100);
            resultsListView.Columns.Add("Cross Section (mm²)", 110);
            resultsListView.Columns.Add("Result", 110);
            dialogForm.Controls.Add(resultsListLabel);
            dialogForm.Controls.Add(resultsListView);

            // Populate trace list
            List<TraceInfo> traces = new List<TraceInfo>();
            foreach (IODBObject obj in selectedElements)
            {
                if (obj.Type == IObjectType.Line)
                {
                    ILineSpecificsD lineSpec = (ILineSpecificsD)obj.GetSpecificsD();
                    double widthMil = lineSpec.Diameter;
                    double widthMm = widthMil * 0.0254;

                    TraceInfo trace = new TraceInfo
                    {
                        ID = obj.GetIndexOnLayer(),
                        LayerName = obj.GetParentLayerName(),
                        WidthMil = widthMil,
                        WidthMm = widthMm,
                        ObjectRef = obj
                    };
                    traces.Add(trace);

                    ListViewItem item = new ListViewItem(trace.ID.ToString());
                    item.SubItems.Add(trace.LayerName);
                    item.SubItems.Add("Trace");
                    item.SubItems.Add(widthMil.ToString("F2"));
                    item.SubItems.Add(widthMm.ToString("F4"));
                    elementListView.Items.Add(item);
                }
            }

            // Summary section
            Label summaryLabel = new Label { Text = "Summary:", Left = 20, Top = 550, Width = 100 };
            TextBox summaryTextBox = new TextBox
            {
                Left = 20, Top = 570, Width = 500, Height = 250, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical
            };
            dialogForm.Controls.Add(summaryLabel);
            dialogForm.Controls.Add(summaryTextBox);

            // Buttons
            Button calculateButton = new Button { Text = "Calculate", Left = 550, Top = 560, Width = 90 };
            Button closeButton = new Button { Text = "Close", Left = 650, Top = 560, Width = 90 };
            dialogForm.Controls.Add(calculateButton);
            dialogForm.Controls.Add(closeButton);

            calculateButton.Click += (sender, e) =>
            {
                if (!double.TryParse(valueTextBox.Text, out double inputValue) ||
                    !double.TryParse(thicknessTextBox.Text, out double thicknessMicron))
                {
                    MessageBox.Show("Invalid input. Please enter numeric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // IPC-2221 constants for external layers
                double k = 0.024;
                double b = 0.44;
                double c = 0.725;

                double thicknessMil = thicknessMicron / 25.4;
                double thicknessMm = thicknessMicron / 1000.0;

                resultsListView.Items.Clear();
                double totalResult = 0;
                double totalCrossSectionMm2 = 0;

                foreach (TraceInfo trace in traces)
                {
                    double crossSectionMil2 = trace.WidthMil * thicknessMil;
                    double crossSectionMm2 = trace.WidthMm * thicknessMm;
                    double result;

                    if (currentMode.Checked)
                    {
                        // Calculate temperature rise given current
                        result = Math.Pow(inputValue / (k * Math.Pow(crossSectionMil2, c)), 1 / b);
                        trace.TemperatureRise = result;
                    }
                    else
                    {
                        // Calculate required current given temperature rise
                        result = k * Math.Pow(crossSectionMil2, c) * Math.Pow(inputValue, b);
                        trace.Current = result;
                    }

                    trace.CrossSectionMil2 = crossSectionMil2;
                    trace.CrossSectionMm2 = crossSectionMm2;

                    ListViewItem item = new ListViewItem(trace.ID.ToString());
                    item.SubItems.Add(trace.LayerName);
                    item.SubItems.Add(trace.WidthMm.ToString("F4"));
                    item.SubItems.Add(crossSectionMil2.ToString("F2"));
                    item.SubItems.Add(crossSectionMm2.ToString("F6"));
                    item.SubItems.Add(currentMode.Checked ? $"{result:F2} °C" : $"{result:F2} A");
                    resultsListView.Items.Add(item);

                    totalResult += result;
                    totalCrossSectionMm2 += crossSectionMm2;
                }

                if (traces.Count > 0)
                {
                    double avgResult = totalResult / traces.Count;
                    double avgCrossSectionMm2 = totalCrossSectionMm2 / traces.Count;

                    summaryTextBox.Text = currentMode.Checked ?
                        $"Current: {inputValue} A\r\n" +
                        $"Copper Thickness: {thicknessMicron} µm ({thicknessMil:F2} mil)\r\n" +
                        $"Number of Traces: {traces.Count}\r\n" +
                        $"Avg. Cross Section: {avgCrossSectionMm2:F6} mm²\r\n" +
                        $"Total Cross Section: {totalCrossSectionMm2:F6} mm²\r\n" +
                        $"Avg. Temp Rise: {avgResult:F2} °C" :
                        $"Target Temp Rise: {inputValue} °C\r\n" +
                        $"Copper Thickness: {thicknessMicron} µm ({thicknessMil:F2} mil)\r\n" +
                        $"Number of Traces: {traces.Count}\r\n" +
                        $"Avg. Cross Section: {avgCrossSectionMm2:F6} mm²\r\n" +
                        $"Total Cross Section: {totalCrossSectionMm2:F6} mm²\r\n" +
                        $"Avg. Current: {avgResult:F2} A";
                }
            };

            closeButton.Click += (sender, e) => dialogForm.Close();
            dialogForm.ShowDialog();
        }

        private class TraceInfo
        {
            public int ID { get; set; }
            public string LayerName { get; set; }
            public double WidthMil { get; set; }
            public double WidthMm { get; set; }
            public double CrossSectionMil2 { get; set; }
            public double CrossSectionMm2 { get; set; }
            public double TemperatureRise { get; set; }
            public double Current { get; set; }
            public IODBObject ObjectRef { get; set; }
        }
    }
}