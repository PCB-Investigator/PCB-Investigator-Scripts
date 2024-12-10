//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script
// Created on 18.01.2017
// Modified on [Current Date]
// Author: Fabio.Gruber
// Modified by: [Your Name]
// 
// Create a testpoint overview with customizable raster settings.
//-----------------------------------------------------------------------------------
// GUID=Raster_638641531264786699

/*
 * This PCB-Investigator script generates a customizable raster and testpoint overview for a PCB design.
 * 
 * Functionality:
 * 1. Displays a user-friendly dialog for configuring raster parameters, including:
 *    - Package name filter
 *    - Grid size and subline spacing
 *    - Text size for labeling
 *    - Selection of the top or bottom PCB side
 * 2. Creates a raster layer based on the user-defined settings, adding:
 *    - Horizontal and vertical grid lines
 *    - Labels for rows (Y) and columns (X)
 * 3. Identifies and marks testpoints based on their attributes, such as drill type and component pin overlap.
 * 4. Automatically handles label positioning to prevent overlap with other elements on the PCB.
 * 5. Supports optional flattening of the step to avoid multi-threading conflicts during raster creation.
 * 
 * Use Cases:
 * - Quickly generate a visual testpoint overview for testing and verification.
 * - Create a detailed raster layout for analysis or documentation purposes.
 * - Simplify preparation for ICT (In-Circuit Testing) by visualizing testpoints and grid positions.
 * 
 * Prerequisites:
 * - A PCB job must be loaded in PCB-Investigator.
 * - Ensure appropriate layer settings are configured for testpoint detection and raster placement.
 * 
 * Notes:
 * - The script dynamically calculates grid lines and labels based on the PCB dimensions and user inputs.
 * - Adjustments to raster settings can be made through the Raster Setup dialog.
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
            // Display the raster setup dialog
            RasterSetupForm setupForm = new RasterSetupForm();
            DialogResult result = setupForm.ShowDialog();

            if (result != DialogResult.OK)
            {
                MessageBox.Show("Operation cancelled by user.");
                return;
            }

            // Retrieve the settings from the dialog
            string PackageName = setupForm.PackageName;
            bool topSide = setupForm.TopSide;
            bool useFlattenStep = true; // This can also be added to the dialog if needed

            int gridSize = setupForm.GridSize;
            int numberOfSublines = setupForm.NumberOfSublines;
            int sublineSpacing = setupForm.SublineSpacing;
            SizeF textSize = setupForm.TextSize;

            if (!parent.JobIsLoaded) return;
            IFilter filter = new IFilter(parent);
            IStep step = parent.GetCurrentStep();
            IMatrix matrix = parent.GetMatrix();
            if (matrix == null) return;

            step.TurnOffAllLayer();
            step.ClearSelection();
            // Not for flatten step, as it's too slow due to threads and may delete new data!
            if (!useFlattenStep)
            {
                matrix.DeleteLayer("testpoint_locations_top", false);
                matrix.DeleteLayer("testpoint_locations_bot", false);
            }
            if (topSide)
                CreateForOneSide(true, step, filter, PackageName, matrix.GetAllDrillLayersForThisLayer(matrix.GetTopSignalLayer()));
            else
                CreateForOneSide(false, step, filter, PackageName, matrix.GetAllDrillLayersForThisLayer(matrix.GetBotSignalLayer()));

            // Create raster layer
            IODBLayer rasterLayer = filter.CreateEmptyODBLayer("raster", step.Name);
            int shapeIndex = IFilter.AddToolDefinitionRound(rasterLayer, 10);
            RectangleF boundsStep = step.GetBounds();

            int countYLines = (int)(boundsStep.Height / gridSize) + 1;
            int countXLines = (int)(boundsStep.Width / gridSize) + 1;

            for (int y = 0; y <= countYLines; y++)
            {
                for (int x = 0; x <= countXLines; x++)
                {
                    for (int i = 0; i < numberOfSublines; i++) // Subline loop
                    {
                        int yVal = (int)(y * gridSize);
                        int xVal = (int)(x * gridSize);
                        if (x < countXLines)
                            CreateHorLine(filter, rasterLayer, shapeIndex, yVal, xVal + i * sublineSpacing + sublineSpacing / 2);
                        if (y < countYLines)
                            CreateVertLine(filter, rasterLayer, shapeIndex, yVal + i * sublineSpacing + sublineSpacing / 2, xVal);
                    }
                }
            }

            for (int y = 0; y < countYLines; y++)
            {
                #region draw text
                // Left side labels
                IODBObject textForPad = filter.CreateText(rasterLayer);
                ITextSpecificsD text = (ITextSpecificsD)textForPad.GetSpecificsD();

                text.Text = "Y" + (y + 1);
                text.TextSize = textSize;
                text.Location = new PointD(-120, y * gridSize + 200);
                text.WidthFactor = 1;
                textForPad.SetSpecifics(text);

                // Right side labels
                IODBObject textForPad2 = filter.CreateText(rasterLayer);
                ITextSpecificsD text2 = (ITextSpecificsD)textForPad2.GetSpecificsD();

                text2.Text = text.Text;
                text2.TextSize = textSize;
                text2.Location = new PointD(countXLines * gridSize + 120, y * gridSize + 200);
                text2.WidthFactor = 1;
                textForPad2.SetSpecifics(text2);
                #endregion
            }
            for (int x = 0; x < countXLines; x++)
            {
                #region draw text
                // Bottom labels
                IODBObject textForPad = filter.CreateText(rasterLayer);
                ITextSpecificsD text = (ITextSpecificsD)textForPad.GetSpecificsD();

                text.Text = "X" + (x + 1);
                text.TextSize = textSize;
                text.Location = new PointD(x * gridSize + 200, -100);
                text.Rotation = -90;
                text.WidthFactor = 1;
                textForPad.SetSpecifics(text);

                // Top labels
                IODBObject textForPad2 = filter.CreateText(rasterLayer);
                ITextSpecificsD text2 = (ITextSpecificsD)textForPad2.GetSpecificsD();

                text2.Text = text.Text;
                text2.TextSize = textSize;
                text2.Location = new PointD(x * gridSize + 300, countYLines * gridSize + 100);
                text2.Rotation = 90;
                text2.WidthFactor = 1;
                textForPad2.SetSpecifics(text2);
                #endregion
            }

            foreach (string drillName in matrix.GetAllDrillLayerNames())
            {
                IODBLayer drillLayer = (IODBLayer)step.GetLayer(drillName);

                foreach (IODBObject drill in drillLayer.GetAllLayerObjects())
                {
                    Dictionary<PCBI.FeatureAttributeEnum, string> attribs = drill.GetAttributesDictionary();

                    if (drill.Type == IObjectType.Pad && attribs.ContainsKey(PCBI.FeatureAttributeEnum.drill) && attribs.ContainsKey(PCBI.FeatureAttributeEnum.geometry))
                    {
                        if (attribs[PCBI.FeatureAttributeEnum.drill].ToUpperInvariant() == "NON_PLATED" && attribs[PCBI.FeatureAttributeEnum.geometry].ToUpperInvariant() == "ICTAUFNAHME")
                        {
                            IODBObject drillMarker = filter.CreatePad(rasterLayer);
                            IPadSpecificsD drillSpec = (IPadSpecificsD)drill.GetSpecificsD();

                            int shapeIndexNew = IFilter.AddToolDefinitionRound(rasterLayer, (float)drillSpec.Diameter);
                            drillSpec.ShapeIndex = shapeIndexNew;
                            drillMarker.SetSpecifics(drillSpec);
                        }
                    }
                }
            }

            matrix.UpdateDataAndList();

            if (!useFlattenStep)
                parent.UIAction.Execute(ID_ActionItem.ID_PRINT_PREVIEW);
        }
        private static void CreateHorLine(IFilter filter, IODBLayer rasterLayer, int shapeIndex, int y, int x)
        {
            IODBObject rasterLine = filter.CreateLine(rasterLayer);
            ILineSpecificsD line = (ILineSpecificsD)rasterLine.GetSpecificsD();

            line.Start = new PointD(x, y);
            line.End = new PointD(x + 50, y);
            line.ShapeIndex = shapeIndex;

            rasterLine.SetSpecifics(line);
        }
        private static void CreateVertLine(IFilter filter, IODBLayer rasterLayer, int shapeIndex, int y, int x)
        {
            IODBObject rasterLine = filter.CreateLine(rasterLayer);
            ILineSpecificsD line = (ILineSpecificsD)rasterLine.GetSpecificsD();

            line.Start = new PointD(x, y);
            line.End = new PointD(x, y + 50);
            line.ShapeIndex = shapeIndex;

            rasterLine.SetSpecifics(line);
        }
        void CreateForOneSide(bool top, IStep step, IFilter filter, string PackageName, List<string> DrillLayers)
        {
            ICMPLayer compLayer = step.GetCMPLayer(top);
            if (compLayer == null) return;

            IODBLayer newLayer = filter.CreateEmptyODBLayer("testpoint_locations_" + (top ? "top" : "bot"), step.Name);
            if (newLayer == null) return;

            List<IODBLayer> allDrillLayers = new List<IODBLayer>();
            foreach (string drillName in DrillLayers)
                allDrillLayers.Add((IODBLayer)step.GetLayer(drillName));

            int shapeIndex = IFilter.AddToolDefinitionRound(newLayer, 75);
            int shapeIndexConnection = IFilter.AddToolDefinitionRound(newLayer, 1);

            foreach (ICMPObject cmp in compLayer.GetAllLayerObjects())
            {
                if (!cmp.UsedPackageName.Contains(PackageName)) continue;

                IODBObject markerPad = filter.CreatePad(newLayer);
                IPadSpecificsD pad = (IPadSpecificsD)markerPad.GetSpecificsD();

                pad.Location = new PointD(cmp.Position);
                pad.ShapeIndex = shapeIndex;

                markerPad.SetSpecifics(pad);
                markerPad.ObjectColor = Color.Green;
                markerPad.SetAttribute("Steel needle <BST> (Testpoint)");
                bool special = false;
                foreach (IODBLayer drillLayer in allDrillLayers)
                {
                    #region check drills
                    foreach (IODBObject drill in drillLayer.GetAllObjectsOnPosition(cmp.Position))
                    {
                        Dictionary<PCBI.FeatureAttributeEnum, string> attribs = drill.GetAttributesDictionary();

                        if (attribs.ContainsKey(PCBI.FeatureAttributeEnum.drill))
                        {
                            if (attribs[PCBI.FeatureAttributeEnum.drill].ToUpperInvariant() == "VIA")
                            {
                                markerPad.ObjectColor = Color.Blue;
                                markerPad.SetAttribute("Pyramid <H> (Via)");
                                special = true;
                                break;
                            }
                        }
                    }
                    if (special) break;
                    #endregion
                }
                if (!special)
                {
                    // Check for component pin
                    foreach (ICMPObject comp in compLayer.GetAllObjectsOnPosition(cmp.Position))
                    {
                        if (comp == cmp) continue; // Ignore the testpoint itself

                        foreach (IPin pin in comp.GetPinList())
                        {
                            IPolyClass cmpPoly = pin.GetPolygonOutline(comp);
                            if (cmpPoly.PointInPolygon(pad.Location))
                            {
                                markerPad.ObjectColor = Color.Red;
                                markerPad.SetAttribute("Serrated <C>"); // Optionally check if pin or body overlaps
                                special = true;                            // or "Serrated with overlapping plastic <CS>"
                                break;
                            }
                        }
                        if (special) break;
                    }
                }
            }
            foreach (ICMPObject cmp in compLayer.GetAllLayerObjects()) // New loop to place all pads first
            {
                if (!cmp.UsedPackageName.Contains(PackageName)) continue;

                IODBObject textForPad = filter.CreateText(newLayer);
                ITextSpecificsD text = (ITextSpecificsD)textForPad.GetSpecificsD();

                text.Text = cmp.Ref.Remove(0, 2); // Assumption that all refs start with TP
                text.TextSize = new SizeF(25, 50);
                text.Location = new PointD(cmp.Position.X + 50, cmp.Position.Y - 10);
                text.WidthFactor = 0.6;
                textForPad.SetSpecifics(text);
                textForPad.ObjectColor = Color.DarkGray;

                // Ensure text location does not intersect
                List<IObject> otherObjectsOnSameLocation = newLayer.GetAllObjectInRectangle(textForPad.GetBoundsD());
                int offset = 50;
                bool horChecked = false;
                while (otherObjectsOnSameLocation.Count > 1)
                {
                    // Move text
                    if (horChecked)
                        text.Location = new PointD(cmp.Position.X, cmp.Position.Y + offset);
                    else
                        text.Location = new PointD(cmp.Position.X - offset - textForPad.GetBoundsD().Width, cmp.Position.Y - 10);
                    offset += 50;
                    horChecked = true;
                    textForPad.SetSpecifics(text);
                    otherObjectsOnSameLocation = newLayer.GetAllObjectInRectangle(textForPad.GetBoundsD());
                }

                IODBObject connectionLine = filter.CreateLine(newLayer);
                ILineSpecificsD line = (ILineSpecificsD)connectionLine.GetSpecificsD();

                line.ShapeIndex = shapeIndexConnection;
                line.Start = new PointD(cmp.Position);
                line.End = new PointD(text.Location.X, text.Location.Y + 25);
                connectionLine.SetSpecifics(line);
                connectionLine.ObjectColor = Color.LightGray;
            }
        }
    }

    // Raster setup dialog class
    public class RasterSetupForm : Form
    {
        public string PackageName { get; private set; }
        public bool TopSide { get; private set; }
        public int GridSize { get; private set; }
        public int NumberOfSublines { get; private set; }
        public int SublineSpacing { get; private set; }
        public SizeF TextSize { get; private set; }

        private TextBox txtPackageName;
        private CheckBox chkTopSide;
        private NumericUpDown numGridSize;
        private NumericUpDown numNumberOfSublines;
        private NumericUpDown numSublineSpacing;
        private NumericUpDown numTextWidth;
        private NumericUpDown numTextHeight;
        private Button btnOK;
        private Button btnCancel;

        public RasterSetupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Raster Setup";
            this.Size = new Size(300, 300);

            Label lblPackageName = new Label() { Text = "Package Name:", Left = 10, Top = 20, Width = 100 };
            txtPackageName = new TextBox() { Left = 120, Top = 20, Width = 150 };
            txtPackageName.Text = "TESTPUNKT";

            chkTopSide = new CheckBox() { Text = "Top Side", Left = 120, Top = 50, Checked = true };

            Label lblGridSize = new Label() { Text = "Grid Size:", Left = 10, Top = 80, Width = 100 };
            numGridSize = new NumericUpDown() { Left = 120, Top = 80, Width = 150, Minimum = 100, Maximum = 10000, Value = 500, Increment = 100 };

            Label lblNumberOfSublines = new Label() { Text = "Number of Sublines:", Left = 10, Top = 110, Width = 120 };
            numNumberOfSublines = new NumericUpDown() { Left = 140, Top = 110, Width = 130, Minimum = 1, Maximum = 20, Value = 5 };

            Label lblSublineSpacing = new Label() { Text = "Subline Spacing:", Left = 10, Top = 140, Width = 100 };
            numSublineSpacing = new NumericUpDown() { Left = 120, Top = 140, Width = 150, Minimum = 10, Maximum = 1000, Value = 100, Increment = 10 };

            Label lblTextSize = new Label() { Text = "Text Size (W x H):", Left = 10, Top = 170, Width = 100 };
            numTextWidth = new NumericUpDown() { Left = 120, Top = 170, Width = 70, Minimum = 10, Maximum = 500, Value = 50, Increment = 5 };
            numTextHeight = new NumericUpDown() { Left = 200, Top = 170, Width = 70, Minimum = 10, Maximum = 500, Value = 80, Increment = 5 };

            btnOK = new Button() { Text = "OK", Left = 50, Width = 80, Top = 210, DialogResult = DialogResult.OK };
            btnCancel = new Button() { Text = "Cancel", Left = 150, Width = 80, Top = 210, DialogResult = DialogResult.Cancel };

            btnOK.Click += new EventHandler(btnOK_Click);

            this.Controls.Add(lblPackageName);
            this.Controls.Add(txtPackageName);
            this.Controls.Add(chkTopSide);
            this.Controls.Add(lblGridSize);
            this.Controls.Add(numGridSize);
            this.Controls.Add(lblNumberOfSublines);
            this.Controls.Add(numNumberOfSublines);
            this.Controls.Add(lblSublineSpacing);
            this.Controls.Add(numSublineSpacing);
            this.Controls.Add(lblTextSize);
            this.Controls.Add(numTextWidth);
            this.Controls.Add(numTextHeight);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PackageName = txtPackageName.Text;
            TopSide = chkTopSide.Checked;
            GridSize = (int)numGridSize.Value;
            NumberOfSublines = (int)numNumberOfSublines.Value;
            SublineSpacing = (int)numSublineSpacing.Value;
            TextSize = new SizeF((float)numTextWidth.Value, (float)numTextHeight.Value);
        }
    }
}
