using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using PCBI.Automation;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class EMCAnalyzer : IPCBIScript
    {
        public void Execute(IPCBIWindow parent)
        {
            if (parent == null) return;

            // Zeige den Eingabedialog zur Festlegung der Regeln
            using (var inputForm = new InputForm())
            {
                if (inputForm.ShowDialog() != DialogResult.OK)
                {
                    return; // Abbrechen, wenn der Benutzer den Dialog schließt
                }
            }

            // Werte aus dem Eingabedialog abrufen
            double maxTraceLength = InputForm.MaxTraceLength;
            double minTraceWidth = InputForm.MinTraceWidth;

            IStep step = parent.GetCurrentStep();
            List<EMCIssue> emcIssues = new List<EMCIssue>();
            List<IODBObject> highlightList = new List<IODBObject>();
            List<INet> nets = step.GetNets();

            PCB_Investigator.PCBIWindows.PCBIWorkingDialog wdlg = new PCB_Investigator.PCBIWindows.PCBIWorkingDialog();
            wdlg.SetAnimationStatus(false);
            wdlg.SetStatusPercent(0);
            wdlg.SetStatusText("Analyzing...");
            wdlg.CanCancel(true);
            wdlg.ShowWorkingDlgAsThread();

            double value = 0;
            double valueStep = 100.0 / nets.Count;

            foreach (INet net in nets)
            {
                wdlg.SetStatusText("Analyzing net " + net.NetName + "...");
                value += valueStep;
                wdlg.SetStatusPercent((int)value);

                List<IODBObject> netItems = net.GetAllNetObjects(parent);
                List<IODBObject> traces = netItems.FindAll(item => item.Type == IObjectType.Line);

                foreach (IODBObject traceObj in traces)
                {
                    ILineSpecificsD trace = (ILineSpecificsD)traceObj.GetSpecificsD();
                    if (trace != null)
                    {
                        double length = IMath.DistancePointToPoint(trace.Start, trace.End);

                        if (length > maxTraceLength)
                        {
                            emcIssues.Add(new EMCIssue
                            {
                                Issue = "Long Trace",
                                Position = $"({trace.Start.X:F3}, {trace.Start.Y:F3})",
                                Layer = traceObj.GetParentLayerName(),
                                Description = $"Trace length {length:F2} mm exceeds {maxTraceLength} mm"
                            });
                            highlightList.Add(traceObj);
                        }

                        bool startConnected = IsPointConnected(trace.Start, netItems, traceObj);
                        bool endConnected = IsPointConnected(trace.End, netItems, traceObj);

                        if (!startConnected || !endConnected)
                        {
                            emcIssues.Add(new EMCIssue
                            {
                                Issue = "Unconnected Stub",
                                Position = $"({trace.Start.X:F3}, {trace.Start.Y:F3})",
                                Layer = traceObj.GetParentLayerName(),
                                Description = "Trace end is not connected"
                            });
                            highlightList.Add(traceObj);
                        }

                        double width = trace.Diameter;
                        if (width < minTraceWidth)
                        {
                            emcIssues.Add(new EMCIssue
                            {
                                Issue = "Small Trace Width",
                                Position = $"({trace.Start.X:F3}, {trace.Start.Y:F3})",
                                Layer = traceObj.GetParentLayerName(),
                                Description = $"Trace width {width:F3} mm is below {minTraceWidth} mm"
                            });
                            highlightList.Add(traceObj);
                        }
                    }
                }
            }

            wdlg.Dispose();
            parent.UpdateView();

            using (var resultsForm = new EMCResultsForm(emcIssues))
            {
                resultsForm.ShowDialog();
            }
        }

        private bool IsPointConnected(PointD point, List<IODBObject> netItems, IODBObject excludeObj)
        {
            double tolerance = 0.01;
            foreach (IObject obj in netItems)
            {
                if (obj == excludeObj) continue;

                if (obj.Type == IObjectType.Pad)
                {
                    IPadSpecificsD pad = (IPadSpecificsD)obj.GetSpecificsD();
                    if (pad != null)
                    {
                        RectangleD padRect = obj.GetBoundsD();
                        padRect.Inflate(tolerance, tolerance);
                        if (padRect.Contains(point)) return true;
                    }
                }
                else if (obj.Type == IObjectType.Line)
                {
                    ILineSpecificsD trace = obj.GetSpecificsD() as ILineSpecificsD;
                    if (trace != null)
                    {
                        if (IMath.DistancePointToPoint(point, trace.Start) <= tolerance ||
                            IMath.DistancePointToPoint(point, trace.End) <= tolerance)
                            return true;
                    }
                }
            }
            return false;
        }
    }

    public class EMCIssue
    {
        public string Issue { get; set; }
        public string Position { get; set; }
        public string Layer { get; set; }
        public string Description { get; set; }
    }

    public class EMCResultsForm : Form
    {
        public EMCResultsForm(List<EMCIssue> emcIssues)
        {
            InitializeComponent(emcIssues);
        }

        private void InitializeComponent(List<EMCIssue> emcIssues)
        {
            this.Text = "EMC Analysis Results";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            ListView resultsListView = new ListView();
            resultsListView.View = View.Details;
            resultsListView.FullRowSelect = true;
            resultsListView.GridLines = true;
            resultsListView.Dock = DockStyle.Fill;

            resultsListView.Columns.Add("Issue", 150);
            resultsListView.Columns.Add("Position", 150);
            resultsListView.Columns.Add("Layer", 100);
            resultsListView.Columns.Add("Description", 350);

            foreach (var issue in emcIssues)
            {
                var item = new ListViewItem(new[] { issue.Issue, issue.Position, issue.Layer, issue.Description });
                resultsListView.Items.Add(item);
            }

            this.Controls.Add(resultsListView);
        }
    }

    public class InputForm : Form
    {
        public static double MaxTraceLength { get; private set; }
        public static double MinTraceWidth { get; private set; }

        private TextBox maxTraceLengthTextBox;
        private TextBox minTraceWidthTextBox;

        public InputForm()
        {
            this.Text = "Set EMC Analysis Rules";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;

            Label maxTraceLengthLabel = new Label() { Text = "Max Trace Length (mm):", Left = 10, Top = 20, Width = 150 };
            maxTraceLengthTextBox = new TextBox() { Left = 170, Top = 20, Width = 100, Text = "100.0" };

            Label minTraceWidthLabel = new Label() { Text = "Min Trace Width (mm):", Left = 10, Top = 60, Width = 150 };
            minTraceWidthTextBox = new TextBox() { Left = 170, Top = 60, Width = 100, Text = "0.2" };

            Button okButton = new Button() { Text = "OK", Left = 100, Width = 80, Top = 100 };
            okButton.Click += (sender, e) =>
            {
                if (double.TryParse(maxTraceLengthTextBox.Text, out double maxTrace) &&
                    double.TryParse(minTraceWidthTextBox.Text, out double minWidth))
                {
                    MaxTraceLength = maxTrace;
                    MinTraceWidth = minWidth;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid numeric values.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.Add(maxTraceLengthLabel);
            this.Controls.Add(maxTraceLengthTextBox);
            this.Controls.Add(minTraceWidthLabel);
            this.Controls.Add(minTraceWidthTextBox);
            this.Controls.Add(okButton);
        }
    }
}
