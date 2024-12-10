using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using PCBI.Automation;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        private PCB_Investigator.PCBIWindows.PCBIWorkingDialog wdlg;

        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            wdlg = new PCB_Investigator.PCBIWindows.PCBIWorkingDialog();
            wdlg.SetAnimationStatus(false);
            wdlg.SetStatusPercent(0);
            wdlg.SetStatusText("Working");
            wdlg.CanCancel(true);

            IStep curStep = parent.GetCurrentStep();

            if (curStep == null) return;

            Dictionary<string, double> smallestDiameterList = new Dictionary<string, double>();
            wdlg.ShowWorkingDlgAsThread();

            List<string> netNames = curStep.GetAllNetNames();
            double value = 0;
            double valueStep = (100.0 / netNames.Count);

            foreach (string netName in curStep.GetAllNetNames())
            {
                INet net = curStep.GetNet(netName);

                wdlg.SetStatusText("Working on " + netName + "...");
                value += valueStep;
                wdlg.SetStatusPercent((int)(value));

                List<IODBObject> allNetElements = net.GetAllNetObjects(parent);
                if (allNetElements.Count == 0) continue;

                double smallestDiameter = allNetElements[0].GetDiameter();
                foreach (IODBObject netElement in allNetElements)
                {
                    double currentDiameter = netElement.GetDiameter();
                    if (currentDiameter < 0) continue; // Skip elements without diameter

                    if (currentDiameter < smallestDiameter)
                    {
                        smallestDiameter = currentDiameter;
                    }
                }
                if (!parent.GetUnit())
                {
                    smallestDiameterList.Add(netName, smallestDiameter);
                }
                else
                {
                    smallestDiameter = IMath.Mils2MM(smallestDiameter);
                    smallestDiameterList.Add(netName, smallestDiameter);
                }
            }
            wdlg.Dispose();

            // Display results in a WinForm dialog
            using (var resultForm = new NetDiameterResultForm(smallestDiameterList, parent.GetUnit()))
            {
                resultForm.ShowDialog();
            }
        }
    }

    public class NetDiameterResultForm : Form
    {
        private ListView resultListView;
        private bool sortAscending = true;

        public NetDiameterResultForm(Dictionary<string, double> smallestDiameterList, bool isMetric)
        {
            InitializeComponent(smallestDiameterList, isMetric);
        }

        private void InitializeComponent(Dictionary<string, double> smallestDiameterList, bool isMetric)
        {
            this.Text = "Smallest Net Diameters";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            resultListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Sorting = SortOrder.None,
                Dock = DockStyle.Fill
            };

            resultListView.Columns.Add("Net Name", 200);
            resultListView.Columns.Add("Smallest Diameter", 150);

            foreach (var kvp in smallestDiameterList)
            {
                string diameter = isMetric ?
                    $"{kvp.Value:F3} mm" :
                    $"{kvp.Value:F3} mils";

                var item = new ListViewItem(new[] { kvp.Key, diameter });
                resultListView.Items.Add(item);
            }

            resultListView.ColumnClick += ResultListView_ColumnClick; // Add event handler for column click

            this.Controls.Add(resultListView);
        }

        // Event handler to sort columns
        private void ResultListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Toggle sorting direction
            sortAscending = !sortAscending;

            // Perform the sorting
            resultListView.ListViewItemSorter = new ListViewItemComparer(e.Column, sortAscending);
            resultListView.Sort();
        }
    }

    // Custom comparer for sorting ListView items
    public class ListViewItemComparer : System.Collections.IComparer
    {
        private int columnIndex;
        private bool ascending;

        public ListViewItemComparer(int columnIndex, bool ascending)
        {
            this.columnIndex = columnIndex;
            this.ascending = ascending;
        }

        public int Compare(object x, object y)
        {
            var itemX = x as ListViewItem;
            var itemY = y as ListViewItem;

            if (itemX == null || itemY == null)
                return 0;

            // Try to parse the values as numbers for numeric comparison
            double valueX, valueY;
            bool isNumberX = double.TryParse(itemX.SubItems[columnIndex].Text.Replace(" mm", "").Replace(" mils", ""), out valueX);
            bool isNumberY = double.TryParse(itemY.SubItems[columnIndex].Text.Replace(" mm", "").Replace(" mils", ""), out valueY);

            int result;
            if (isNumberX && isNumberY)
            {
                result = valueX.CompareTo(valueY);
            }
            else
            {
                result = string.Compare(itemX.SubItems[columnIndex].Text, itemY.SubItems[columnIndex].Text);
            }

            return ascending ? result : -result;
        }
    }
}
