//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script
// 
// Create a testpoint overview with customizable raster settings.
//-----------------------------------------------------------------------------------
// GUID=THT_SMD_638641531264786699
/*
 * This PCB-Investigator script performs a testpoint proximity check, ensuring connectors maintain 
 * adequate distance from other components or connectors on both top and bottom layers.
 *
 * Functionality:
 * 1. **Connector Distance Validation**:
 *    - Verifies the distance between connectors and other components (e.g., SMD components).
 *    - Ensures a minimum clearance of:
 *      - 4 mm between connectors and SMD components.
 *      - 0.8 mm between connectors.
 * 2. **Analysis Scope**:
 *    - Analyzes both top and bottom component layers.
 *    - Flags violations for review and stores them in a detailed results list.
 * 3. **Visualization**:
 *    - Highlights connectors and related components involved in distance violations.
 *    - Displays a detailed overview of testpoint distances in a user-friendly DataGridView.
 * 4. **Interactive Results**:
 *    - Allows users to select and zoom to flagged components directly from the results table.
 *
 * Parameters:
 * - `MinDistanceConnector2SMD_CMP`: Minimum allowed distance between connectors and SMD components (default: 4 mm).
 * - `MinDistanceConnector2Connector`: Minimum allowed distance between connectors (default: 0.8 mm).
 *
 * Use Cases:
 * - Ensure testpoint placement complies with clearance rules for manufacturing and testing.
 * - Validate connector and component proximity to minimize potential signal integrity or physical assembly issues.
 *
 * Notes:
 * - The script dynamically calculates distances using precise geometric analysis.
 * - Results include connector references, nearby components, measured distances, and the layer information.
 *
 * Author: Guenther
 * Date: 06.12.2024
 */



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
        private const string ConnectorReferencePrefix = "S";
        private double MinDistanceConnector2SMD_CMP = PCBI.MathUtils.IMath.MM2Mils(4);
        private double MinDistanceConnector2Connector = PCBI.MathUtils.IMath.MM2Mils(0.8);
        private int tpCountTotal = 0;
        private List<TestpointResult> results = new List<TestpointResult>();

        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            if (parent == null || !parent.JobIsLoaded)
            {
                MessageBox.Show("No job loaded.");
                return;
            }

            IStep step = parent.GetCurrentStep();
            IMatrix matrix = parent.GetMatrix();

            if (matrix == null || step == null)
            {
                MessageBox.Show("No job loaded.");
                return;
            }

            // Check Top Component Layer
            tpCountTotal = 0;
            string topComponentLayerName = matrix.GetTopComponentLayer();
            if (!string.IsNullOrWhiteSpace(topComponentLayerName))
            {
                ILayer dataLayer = step.GetLayer(topComponentLayerName);
                if (dataLayer != null && dataLayer is ICMPLayer)
                {
                    CheckTestpoints((ICMPLayer)dataLayer);
                }
            }

            // Check Bot Component Layer
            string botComponentLayerName = matrix.GetBotComponentLayer();
            if (!string.IsNullOrWhiteSpace(botComponentLayerName))
            {
                ILayer dataLayer = step.GetLayer(botComponentLayerName);
                if (dataLayer != null && dataLayer is ICMPLayer)
                {
                    CheckTestpoints((ICMPLayer)dataLayer);
                }
            }

            parent.UpdateView();
            ShowResultsDialog(parent, step);
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

        private void CheckTestpoints(ICMPLayer layer)
        {
            if (layer == null)
            {
                MessageBox.Show("Layer is null referenced.");
                return;
            }

            PointD fromPoint = PointD.Empty, toPoint = PointD.Empty;
            List<IObject> allComponents = layer.GetAllLayerObjects();
            double maxDistance = Math.Max(MinDistanceConnector2Connector, MinDistanceConnector2SMD_CMP) * 1.01;

            foreach (IObject element in allComponents)
            {
                if (element is ICMPObject)
                {
                    ICMPObject tpCmp = (ICMPObject)element;

                    if (tpCmp.Ref.StartsWith(ConnectorReferencePrefix))
                    {
                        tpCountTotal++;
                        RectangleD checkRect = tpCmp.GetBoundsD();
                        checkRect.Inflate(maxDistance, maxDistance);

                        IPolyClass tpPoly = new IPolyClass();
                        foreach (IPin pin in tpCmp.GetPinList())
                        {
                            tpPoly.AddPolygon(pin.GetPolygonOutline(tpCmp));
                        }

                        foreach (IObject nearElement in layer.GetAllObjectInRectangle(checkRect))
                        {
                            if (nearElement is ICMPObject)
                            {
                                ICMPObject nearCmp = (ICMPObject)nearElement;
                                if (tpCmp.Ref == nearCmp.Ref) continue;

                                IPolyClass nearCmpPoly = nearCmp.GetPolygonOutline(true);
                                if (tpPoly != null && nearCmpPoly != null)
                                {
                                    double distance = tpPoly.DistanceTo(nearCmpPoly, ref fromPoint, ref toPoint);
                                    if (nearCmp.Ref.StartsWith(ConnectorReferencePrefix))
                                    {
                                        if (distance < MinDistanceConnector2Connector)
                                        {
                                            results.Add(new TestpointResult(tpCmp.Ref, nearCmp.Ref, PCBI.MathUtils.IMath.Mils2MM(distance), "Connector", layer.GetLayerName()));
                                        }
                                    }
                                    else
                                    {
                                        if (distance < MinDistanceConnector2SMD_CMP)
                                        {
                                            results.Add(new TestpointResult(tpCmp.Ref, nearCmp.Ref, PCBI.MathUtils.IMath.Mils2MM(distance), "SMD", layer.GetLayerName()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShowResultsDialog(IPCBIWindow parent, IStep step)
        {
            Form resultForm = new Form
            {
                Text = "Testpoint Check Results",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Connector", HeaderText = "Connector", DataPropertyName = "Connector" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "NearComponent", HeaderText = "Near Component", DataPropertyName = "NearComponent" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Distance", HeaderText = "Distance (mm)", DataPropertyName = "Distance" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", DataPropertyName = "Type" });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Layer", HeaderText = "Layer", DataPropertyName = "Layer" });

            dataGridView.DataSource = results;

            // Handle row selection in the DataGridView
            dataGridView.SelectionChanged += (sender, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    string selectedRef = dataGridView.SelectedRows[0].Cells["Connector"].Value.ToString();
                    SelectComponent(parent, step, selectedRef);
                }
            };

            resultForm.Controls.Add(dataGridView);
            resultForm.Show(parent.MainForm);
        }
    }

    public class TestpointResult
    {
        public string Connector { get; set; }
        public string NearComponent { get; set; }
        public double Distance { get; set; }
        public string Type { get; set; }
        public string Layer { get; set; }

        public TestpointResult(string connector, string nearComponent, double distance, string type, string layer)
        {
            Connector = connector;
            NearComponent = nearComponent;
            Distance = Math.Round(distance, 3);
            Type = type;
            Layer = layer;
        }
    }
}
