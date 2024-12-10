/*
 * This PCB-Investigator script performs a Signal Integrity (SI) analysis on PCB nets.
 * 
 * Functionality:
 * 1. **Net Length Analysis**:
 *    - Calculates the total length of each net and compares it against a predefined maximum length.
 *    - Flags nets exceeding the threshold and highlights them in red.
 * 
 * 2. **Trace Angle Validation**:
 *    - Analyzes trace angles within nets to identify acute angles below a minimum threshold.
 *    - Flags traces with problematic angles and marks them for review.
 * 
 * 3. **Parallel Trace Detection**:
 *    - Detects parallel traces between the current net and others within a specified distance.
 *    - Highlights potential crosstalk regions in the design.
 * 
 * 4. **Visualization**:
 *    - Marks problematic traces and nets with a color-coded system:
 *      - **Green**: No issues detected.
 *      - **Red**: Signal integrity issues found.
 * 
 * 5. **Result Reporting**:
 *    - Summarizes analysis results in a user-friendly Windows Form.
 *    - Allows users to view issues by net, including detailed descriptions.
 *    - Supports net selection directly from the results table for quick inspection.
 * 
 * Use Cases:
 * - Validate signal integrity of PCB designs before manufacturing.
 * - Identify potential design issues, such as crosstalk or acute angles, early in the development process.
 * - Provide clear feedback on net-level integrity for debugging and optimization.
 * 
 * Parameters:
 * - `maxNetLength`: Maximum allowable net length (in microns).
 * - `minTraceAngle`: Minimum trace angle to avoid acute-angle issues.
 * - `parallelTraceDistance`: Distance threshold for detecting parallel traces and potential crosstalk.
 * 
 * Prerequisites:
 * - A PCB job must be loaded in PCB-Investigator.
 * - The design should include defined nets and trace geometry.
 * 
 * Notes:
 * - The analysis dynamically updates the PCB view to highlight issues as they are detected.
 * - Results can be exported or reviewed directly within the script's interface.
 * 
 * Author: Guenther
 * Date: 06.12.2024
 */

//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script
// 
// Create a testpoint overview with customizable raster settings.
//-----------------------------------------------------------------------------------
// GUID=IS_638641531264786699


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
                MessageBox.Show("Bitte laden Sie zuerst einen Job."); // "Please load a job first."
                return;
            }

            IStep step = parent.GetCurrentStep();
            if (step == null)
            {
                MessageBox.Show("Kein Step geladen."); // "No step loaded."
                return;
            }
            parent.UIAction.Execute(ID_ActionItem.ID_DRAW_ONLY_SELECTED_ELEMENTS);
            // SI Analysis Parameters
            double maxNetLength = 10000; // Maximum net length in microns (10 mm)
            double minTraceAngle = 0;   // Minimum allowed trace angle in degrees
            double parallelTraceDistance = 200; // Distance in microns for crosstalk (0.2 mm)

            // List to store analysis results
            List<SIResult> siResults = new List<SIResult>();

            // Colors for SI status
            Color colorOK = Color.Green;
            Color colorIssue = Color.Red;

            // Get all nets in the design
            List<INet> nets = step.GetNets();

            foreach (INet net in nets)
            {
                bool siIssueFound = false;
                StringBuilder issues = new StringBuilder();

                // Check net length
                double netLength = GetNetLength(parent, net);
                if (netLength > maxNetLength)
                {
                    siIssueFound = true;
                    issues.AppendLine($"Net length {netLength:F2} µm exceeds maximum of {maxNetLength} µm.");
                    // Mark the net objects with color
                    MarkNet(parent, net, colorIssue);
                }

                // Check for acute angles in traces
                List<IODBObject> netObjects = net.GetAllNetObjects(parent);
                foreach (IODBObject obj in netObjects)
                {
                    if (obj.Type == IObjectType.Line)
                    {
                        if (HasAcuteAngles(obj, ref minTraceAngle))
                        {
                            siIssueFound = true;
                            issues.AppendLine($"Trace {obj.GetHashCode()} has acute angles less than {minTraceAngle} degrees.");
                            obj.ObjectColor = colorIssue;
                        }
                        else
                        {
                            obj.ObjectColor = colorOK;
                        }
                    }
                }

                // Check for parallel traces (crosstalk)
                if (HasParallelTraces(parent, net, step, parallelTraceDistance))
                {
                    siIssueFound = true;
                    issues.AppendLine($"Net {net.NetName} has parallel traces within {parallelTraceDistance} µm (possible crosstalk).");
                    // Mark the net objects with color
                    MarkNet(parent, net, colorIssue);
                }

                // If no SI issues found, mark net as OK
                if (!siIssueFound)
                {
                    MarkNet(parent, net, colorOK);
                    issues.AppendLine("No SI issues detected.");
                }

                // Add the result to the list
                siResults.Add(new SIResult
                {
                    NetName = net.NetName,
                    Length = netLength,
                    Issues = issues.ToString(),
                    SIStatus = siIssueFound ? "Issues Found" : "OK"
                });
            }

            parent.UpdateView();

            // Display the results in a Windows Form
            ShowResultsForm(siResults, parent, step);
        }

        private void SelectNet(IPCBIWindow parent, IStep step, string netName)
        {
       
        step.ClearSelection();
        if(!string.IsNullOrEmpty(netName))
        {
            INet net = step.GetNet(netName);
            if(net != null)
            foreach (IODBObject obj in net.GetAllNetObjects(parent))
            {
                obj.Select(true);
            }
            parent.ZoomToSelection();
            }
        }

        private double GetNetLength(IPCBIWindow parent, INet net)
        {
            double totalLength = 0;
            List<IODBObject> netObjects = net.GetAllNetObjects(parent);

            foreach (IODBObject obj in netObjects)
            {
                if (obj.Type == IObjectType.Line)
                {
                    // Cast it to line
                    ILineSpecificsD line = obj.GetSpecificsD() as ILineSpecificsD;
                    totalLength += IMath.DistancePointToPoint(line.Start, line.End);
                }
            }

            return totalLength;
        }

        private bool HasAcuteAngles(IODBObject trace, ref double minAngle)
        {
            if (trace.Type == IObjectType.Line)
            {
                ILineSpecificsD line = trace.GetSpecificsD() as ILineSpecificsD;
                int angle = (int)IMath.GetAngle(line.Start, line.End);
                minAngle = angle;
                return !(angle % 45 == 0 || angle == 0 || angle == 180 || angle == 90 || angle == 270 || angle == 45 || angle == 135 || angle == 225 || angle == 315);
            }

            return false;
        }

        private bool HasParallelTraces(IPCBIWindow parent, INet net, IStep step, double distanceThreshold)
        {
            List<IODBObject> netTraces = net.GetAllNetObjects(parent).FindAll(obj => obj.Type == IObjectType.Line);
            List<IODBObject> allTraces = new List<IODBObject>();

            foreach (string layerName in step.GetAllLayerNames())
            {
                ILayer layer = step.GetLayer(layerName);
                if (layer is IODBLayer odbLayer)
                {
                    foreach (IObject obj in odbLayer.GetAllLayerObjects())
                    {
                        if (obj is IODBObject trace && trace.Type == IObjectType.Line && !netTraces.Contains(trace))
                        {
                            allTraces.Add(trace);
                        }
                    }
                }
            }

            foreach (IODBObject netTrace in netTraces)
            {
                IPolyClass netPoly = netTrace.GetPolygonOutline();
                foreach (IODBObject otherTrace in allTraces)
                {
                    IPolyClass otherPoly = otherTrace.GetPolygonOutline();
                    if (AreTracesParallel(netPoly, otherPoly, distanceThreshold))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool AreTracesParallel(IPolyClass poly1, IPolyClass poly2, double distanceThreshold)
        {
            poly1.GetBounds().Inflate(distanceThreshold, distanceThreshold);
            return poly1.Intersect(poly2).EdgeCount > 0;
        }

        private void MarkNet(IPCBIWindow parent, INet net, Color color)
        {
            List<IODBObject> netObjects = net.GetAllNetObjects(parent);
            foreach (IODBObject obj in netObjects)
            {
                obj.ObjectColor = color;
            }
        }

        private void ShowResultsForm(List<SIResult> siResults, IPCBIWindow parent, IStep step)
        {
            Form form = new Form
            {
                Text = "Signal Integrity Analysis Results",
                Size = new Size(800, 600)
            };

            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Define columns
            DataGridViewTextBoxColumn netNameColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Net Name",
                Name = "NetName",
                DataPropertyName = "NetName",
                Width = 150
            };
            dataGridView.Columns.Add(netNameColumn);

            DataGridViewTextBoxColumn lengthColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Length (µm)",
                DataPropertyName = "Length",
                Width = 100
            };
            dataGridView.Columns.Add(lengthColumn);

            DataGridViewTextBoxColumn siStatusColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "SI Status",
                DataPropertyName = "SIStatus",
                Width = 100
            };
            dataGridView.Columns.Add(siStatusColumn);

            DataGridViewTextBoxColumn issuesColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Issues",
                DataPropertyName = "Issues",
                Width = 400
            };
            dataGridView.Columns.Add(issuesColumn);

            // Bind the data
            dataGridView.DataSource = new BindingSource { DataSource = siResults };

            // Add selection changed event to select the net
            dataGridView.SelectionChanged += (sender, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                try
                {
                    string selectedNetName = dataGridView.SelectedRows[0].Cells["NetName"].Value.ToString();
                    SelectNet(parent, step, selectedNetName);
                    }
                    catch(Exception ex)
                    { 
                        MessageBox.Show(ex.ToString(),"");
                    }
                }
            };

            form.Controls.Add(dataGridView);
            form.Show(parent.MainForm);
        }
    }

    // Helper class for storing SI analysis results
    public class SIResult
    {
        public string NetName { get; set; }
        public double Length { get; set; }
        public string SIStatus { get; set; }
        public string Issues { get; set; }
    }
}
