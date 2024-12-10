/*
 * This PCB-Investigator script calculates and identifies the distances between objects 
 * on solder paste and solder mask layers in a PCB design. 
 * It provides the following functionalities:
 *
 * 1. Automatically identifies and selects solder paste and solder mask layers.
 * 2. Iteratively calculates the distance between objects on these layers using a user-defined threshold.
 * 3. Highlights objects in close proximity and temporarily changes their color for better visualization.
 * 4. Collects distance data (e.g., net number, mask layer, solder paste layer, and distance values).
 * 5. Displays the results in a user-friendly interface, allowing the user to review distances and sort by column.
 * 
 * This script is designed to aid in identifying potential issues with spacing between solder paste 
 * and solder mask elements, ensuring compliance with manufacturing tolerances.
 *
 * Prerequisites:
 * - A valid PCB job must be loaded in PCB-Investigator.
 * - Layers for solder paste and solder mask must exist in the design matrix.
 *
 * Notes:
 * - Results are shown in a dedicated form with sorting capabilities for efficient data analysis.
 * - The maximum distance for proximity checks can be adjusted via the `maxDist` parameter.
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
// GUID=MarkPasteDist_638641531264786697
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
        private PCB_Investigator.PCBIWindows.PCBIWorkingDialog wdlg;

        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            double maxDist = 4; // mils

            wdlg = new PCB_Investigator.PCBIWindows.PCBIWorkingDialog();
            wdlg.SetAnimationStatus(false);
            wdlg.SetStatusPercent(0);
            wdlg.SetStatusText("Working");
            wdlg.CanCancel(true);

            IMatrix matrix = parent.GetMatrix();
            IStep step = parent.GetCurrentStep();

            wdlg.ShowWorkingDlgAsThread();

            List<string> layerNames = step.GetAllLayerNames();
            double value = 0;
            double valueStep = ((100.0 / layerNames.Count));

            List<DistanceResult> distanceResults = new List<DistanceResult>();
            IODBLayer SMTLayer = null;
            IODBLayer SPLayer = null;
            foreach (string layername in step.GetAllLayerNames())
            {
                wdlg.SetStatusText("Working on " + layername + "...");
                value += valueStep;
                wdlg.SetStatusPercent((int)(value));

                if (matrix.GetMatrixLayerType(layername) == MatrixLayerType.Solder_paste && SPLayer == null)
                {
                    SPLayer = (IODBLayer)step.GetLayer(layername);
                }
                else if (matrix.GetMatrixLayerType(layername) == MatrixLayerType.Solder_mask && SMTLayer == null)
                {
                    SMTLayer = (IODBLayer)step.GetLayer(layername);
                }

                if (SMTLayer != null && SPLayer != null)
                {
                    foreach (IODBObject IODBO1 in SPLayer.GetAllLayerObjects())
                    {
                        RectangleD boundsToInflate = IODBO1.GetBoundsD();
                        boundsToInflate.Inflate(maxDist, maxDist);
                        foreach (IODBObject IODBO2 in SMTLayer.GetAllObjectInRectangle(boundsToInflate))
                        {
                            IODBObject.DistanceResultClass distance = IODBO1.DistanceTo(IODBO2);
                            if (distance.Distance >= 0)
                            {
                                distanceResults.Add(new DistanceResult
                                {
                                    NetNumber = IODBO2.NetName.ToString(),
                                    MaskLayer = distance.From.ToString(),
                                    SolderPaste = distance.To.ToString(),
                                    Distance = distance.Distance
                                });
                                IODBO2.ObjectColorTemporary(Color.Gray);
                                IODBO2.Select(true);
                            }
                        }
                    }
                    break;
                }
            }
            wdlg.Dispose();

            if (distanceResults.Count > 0)
            {
                using (var resultForm = new DistanceResultForm(distanceResults))
                {
                    resultForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("No Results found...\nPlease check Layers for Paste- or Mask layers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class DistanceResult
    {
        public string NetNumber { get; set; }
        public string MaskLayer { get; set; }
        public string SolderPaste { get; set; }
        public double Distance { get; set; }
    }

    public class DistanceResultForm : Form
    {
        private ListView resultListView;
        private List<DistanceResult> results;

        public DistanceResultForm(List<DistanceResult> results)
        {
            this.results = results;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Distance Results";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            resultListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Sorting = SortOrder.Ascending,
                Dock = DockStyle.Fill
            };

            resultListView.Columns.Add("Net Number", 100);
            resultListView.Columns.Add("Mask Layer", 150);
            resultListView.Columns.Add("Solder Paste", 150);
            resultListView.Columns.Add("Distance", 100);

            resultListView.ListViewItemSorter = new ListViewItemComparer();
            resultListView.ColumnClick += new ColumnClickEventHandler(ResultListView_ColumnClick);

            PopulateListView();

            this.Controls.Add(resultListView);
        }

        private void PopulateListView()
        {
            resultListView.Items.Clear();
            foreach (var result in results)
            {
                var item = new ListViewItem(new[]
                {
                    result.NetNumber,
                    result.MaskLayer,
                    result.SolderPaste,
                    result.Distance.ToString("F3")
                });
                resultListView.Items.Add(item);
            }
        }

        private void ResultListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == null) return;

            ListViewItemComparer sorter = lv.ListViewItemSorter as ListViewItemComparer;
            if (sorter == null)
            {
                sorter = new ListViewItemComparer();
                lv.ListViewItemSorter = sorter;
            }

            if (e.Column == sorter.SortColumn)
            {
                // Reverse the current sort direction
                if (sorter.Order == SortOrder.Ascending)
                    sorter.Order = SortOrder.Descending;
                else
                    sorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number to sort by
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            lv.Sort();
        }
    }

    public class ListViewItemComparer : System.Collections.IComparer
    {
        public int SortColumn { get; set; }
        public SortOrder Order { get; set; }

        public ListViewItemComparer()
        {
            SortColumn = 0;
            Order = SortOrder.Ascending;
        }

        public int Compare(object x, object y)
        {
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;

            int compareResult;
            if (SortColumn == 3) // Distance column (numeric comparison)
            {
                double distanceX, distanceY;
                if (double.TryParse(listviewX.SubItems[SortColumn].Text, out distanceX) &&
                    double.TryParse(listviewY.SubItems[SortColumn].Text, out distanceY))
                {
                    compareResult = distanceX.CompareTo(distanceY);
                }
                else
                {
                    compareResult = string.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
                }
            }
            else // Other columns (string comparison)
            {
                compareResult = string.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
            }

            if (Order == SortOrder.Descending)
                compareResult *= -1;

            return compareResult;
        }
    }
}