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
public class EMCAnalyzer : IPCBIScript
{
    public void Execute(IPCBIWindow parent)
    {
        // Get the current PCB data
        if (parent == null) return;

        IStep step = parent.GetCurrentStep();

        List<string> emcIssues = new List<string>();

        // Get the highlight list to highlight problematic objects
        List<IODBObject> highlightList = new List<IODBObject>();

        // Analyze nets for EMC issues
        List<INet> nets = step.GetNets();

        double maxTraceLength = 100.0; // Maximum acceptable trace length in mm
        double minTraceWidth = 0.2; // Minimum acceptable trace width in mm

        // Dictionary to store the smallest trace width per net
        Dictionary<string, double> smallestDiameterList = new Dictionary<string, double>();

        // Working dialog
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

            // Get all net items (pads, traces, vias)
            List<IODBObject> netItems = net.GetAllNetObjects(parent);

            // Get all traces in the net
            List<IODBObject> traces = netItems.FindAll(item => item.Type == IObjectType.Line);

            double smallestDiameter = double.MaxValue;

            // Analyze traces
            foreach (IODBObject traceObj in traces)
            {
                ILineSpecificsD trace = (ILineSpecificsD)traceObj.GetSpecificsD();
                if (trace != null)
                {
                    // Calculate trace length
                    double length = IMath.DistancePointToPoint(trace.Start, trace.End);

                    if (length > maxTraceLength)
                    {
                        // Long trace found
                        emcIssues.Add($"Long trace in net {net.NetName} at trace {traceObj.NetName}, length: {length:F2} mm");

                        // Highlight the trace in the design
                        highlightList.Add(traceObj);
                    }

                    // Check for unconnected stubs
                    bool startConnected = IsPointConnected(trace.Start, netItems, traceObj);
                    bool endConnected = IsPointConnected(trace.End, netItems, traceObj);

                    if (!startConnected || !endConnected)
                    {
                        // Unconnected stub found
                        emcIssues.Add($"Unconnected stub in net {net.NetName} at trace {traceObj.NetName}");

                        // Highlight the trace in the design
                        highlightList.Add(traceObj);
                    }

                    // Get trace width (diameter)
                    double width = trace.Diameter; // Assuming trace.Width gives width in mm

                    // Keep track of the smallest diameter per net
                    if (width < smallestDiameter)
                    {
                        smallestDiameter = width;
                    }

                    // Check for traces with small widths
                    if (width < minTraceWidth)
                    {
                        emcIssues.Add($"Trace with small width in net {net.NetName} at trace {traceObj.NetName}, width: {width:F3} mm");

                        // Highlight the trace in the design
                        highlightList.Add(traceObj);
                    }
                }
            }

            if (smallestDiameter != double.MaxValue)
            {
                smallestDiameterList.Add(net.NetName, smallestDiameter);
            }
        }

        wdlg.Dispose();

        // Analyze components for connection issues
        List<ICMPObject> components = step.GetAllCMPObjects();

        foreach (ICMPObject component in components)
        {
            List<string> connectedNets = new List<string>();

            // Get pads of the component

            List<IPin> pads = component.GetPinList();

            foreach (IPin pad in pads)
            {
                string netName = pad.GetNetNameOnIPin(component);
                if (!string.IsNullOrEmpty(netName) && !connectedNets.Contains(netName))
                {
                    connectedNets.Add(netName);
                }
            }

            if (connectedNets.Count < 2)
            {
                // Component with only one net connected
                emcIssues.Add($"Component {component.Ref} is connected to only {connectedNets.Count} net(s)");

                // Highlight the component in the design
                component.ObjectColor = Color.Red;
                //highlightList.Add(component);
            }
        }

        // Refresh the display to show highlights
        parent.UpdateView();

        // Display the results
        using (var resultsForm = new EMCResultsForm(emcIssues, smallestDiameterList, parent.GetUnit()))
        {
            resultsForm.ShowDialog();
        }
    }

    private bool IsPointConnected(PointD point, List<IODBObject> netItems, IODBObject excludeObj)
    {
        double tolerance = 0.01; // Adjust as necessary
        foreach (IObject obj in netItems)
        {
            if (obj == excludeObj)
                continue;

            if (obj.Type == IObjectType.Pad)
            {
                IPadSpecificsD pad = (IPadSpecificsD)obj.GetSpecificsD();
                if (pad != null)
                {
                    RectangleD padRect = obj.GetBoundsD();
                    padRect.Inflate(tolerance, tolerance);
                    if (padRect.Contains(point))
                        return true;
                }
            }
            else if (obj.Type == IObjectType.Line)
            {
                ILineSpecificsD trace = obj.GetSpecificsD() as ILineSpecificsD;
                if (trace != null)
                {
                    // use IMath
                    if ((IMath.DistancePointToPoint(point, trace.Start) <= tolerance) || (IMath.DistancePointToPoint(point, trace.End)) <= tolerance)
                        return true;
                }
            }
        }
        return false;
    }
}

public class EMCResultsForm : Form
{
    public EMCResultsForm(List<string> emcIssues, Dictionary<string, double> smallestDiameterList, bool isMetric)
    {
        InitializeComponent(emcIssues, smallestDiameterList, isMetric);
    }

    private void InitializeComponent(List<string> emcIssues, Dictionary<string, double> smallestDiameterList, bool isMetric)
    {
        this.Text = "EMC Analysis Results";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        TabControl tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;

        // EMC Issues Tab
        TabPage emcIssuesTab = new TabPage("EMC Issues");
        ListBox emcIssuesListBox = new ListBox();
        emcIssuesListBox.Dock = DockStyle.Fill;
        emcIssuesListBox.Items.AddRange(emcIssues.ToArray());
        emcIssuesTab.Controls.Add(emcIssuesListBox);

        // Smallest Diameters Tab
        TabPage diametersTab = new TabPage("Smallest Trace Widths");
        ListView diametersListView = new ListView();
        diametersListView.View = View.Details;
        diametersListView.FullRowSelect = true;
        diametersListView.GridLines = true;
        diametersListView.Sorting = SortOrder.Ascending;
        diametersListView.Dock = DockStyle.Fill;

        diametersListView.Columns.Add("Net Name", 200);
        diametersListView.Columns.Add("Smallest Trace Width", 200);

        foreach (var kvp in smallestDiameterList)
        {
            string width = isMetric ? $"{kvp.Value:F3} mm" : $"{IMath.MM2Mils(kvp.Value):F3} mils";
            var item = new ListViewItem(new[] { kvp.Key, width });
            diametersListView.Items.Add(item);
        }

        diametersTab.Controls.Add(diametersListView);

        tabControl.TabPages.Add(emcIssuesTab);
        tabControl.TabPages.Add(diametersTab);

        this.Controls.Add(tabControl);
    }
}
}
