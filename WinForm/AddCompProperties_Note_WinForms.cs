//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script (Synchronous)
// Created on 10.10.2024
// Author guent
// SDK online reference http://manual.pcb-investigator.com/InterfaceDocumentation/index.php
// SDK http://www.pcb-investigator.com/sdk-participate
// Updated: 10.10.2024
// Description: Enter your description here
//-----------------------------------------------------------------------------------
// Favorite scripts can be placed in the ribbon menu. Therefore a unique GUID and a ButtonEnabled state is mandatory:
// GUID=AddCompNote_fromProperty_638641531264786697
// ButtonEnabled=1   (Button enabled if: 1=Design is loaded, 2=Always, 4=Design contains components, 8=Loaded step is a panel, 16=Element is selected, 32=Component is selected)
// AutoStart=false


// DLLImport $(Application.StartupPath)\PlugIn\PCBI_Dimensioning.dll;

using System;
using System.Collections;
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
        public PScript() { }
        public bool Canceled = false;

        public void Execute(IPCBIWindow parent)
        {
            IStep curStep = parent.GetCurrentStep();
            if (curStep == null) return;

            // Zeige den Eingabedialog zur Auswahl der Eigenschaft und Farbe
            using (var inputForm = new InputForm())
            {
                if (inputForm.ShowDialog() != DialogResult.OK)
                {
                    return; // Abbrechen, wenn der Benutzer den Dialog schließt
                }
            }

            // Werte aus dem Eingabedialog abrufen
            string selectedProperty = InputForm.SelectedProperty;
            Color foundTextColor = InputForm.SelectedTextColor;
            Color notFoundTextColor = InputForm.NotFoundTextColor;

            PCBI_Dimensioning.PlugInToolBarConnection dimensionPlugin = parent.GetPlugInInstance(typeof(PCBI_Dimensioning.PlugInToolBarConnection)) as PCBI_Dimensioning.PlugInToolBarConnection;
            if (dimensionPlugin == null)
            {
                IAutomation.AddToErrorLog("Cannot load dimension plugin!");
                return;
            }

            ICMPLayer cmpTop = curStep.GetCMPLayer(true);
            ICMPLayer cmpBot = curStep.GetCMPLayer(false);
            List<RectangleD> occupiedAreas = new List<RectangleD>();

            // Sammle vorhandene Komponentenumrisse
            foreach (ICMPObject cmp in curStep.GetAllCMPObjects())
            {
                occupiedAreas.Add(cmp.Bounds);
            }

            // Fortschrittsdialog erstellen
            PCB_Investigator.PCBIWindows.PCBIWorkingDialog progressDialog = new PCB_Investigator.PCBIWindows.PCBIWorkingDialog();
            progressDialog.SetAnimationStatus(false);
            progressDialog.SetStatusText("Adding Notes to Components...");
            progressDialog.CanCancel(true);
            progressDialog.ShowWorkingDlgAsThread();

            progressDialog.CancelPressed += () =>
            {
                Canceled = true;
                progressDialog.Dispose();
            };

            double totalComponents = curStep.GetAllCMPObjects().Count;
            double progressStep = 100.0 / totalComponents;
            double progressValue = 0;

            foreach (ICMPObject cmp in curStep.GetAllCMPObjects())
            {
                progressDialog.SetStatusText($"Processing component: {cmp.Ref}");
                progressValue += progressStep;
                progressDialog.SetStatusPercent((int)progressValue);

                string MyValue = cmp.GetType().GetProperty(selectedProperty)?.GetValue(cmp, null)?.ToString();
                Color fontColor = !string.IsNullOrEmpty(MyValue) ? foundTextColor : notFoundTextColor;

                if (Canceled)
                {
                    break; // Schleife abbrechen, wenn der Benutzer den Fortschrittsdialog abbricht
                }

                if (string.IsNullOrEmpty(MyValue))
                {
                    continue; // Überspringe, wenn der Wert leer ist
                }

                float fontSize = 20;
                string fontName = "Arial";
                string layerName = cmp.PlacedTop ? cmpTop.GetLayerName() : cmpBot.GetLayerName();

                double noteWidth = MyValue.Length * fontSize * 0.6;
                double noteHeight = fontSize;
                double offset = 10.0;

                List<PointD> candidatePositions = new List<PointD>
                {
                    new PointD(cmp.Bounds.Left - noteWidth / 2 - offset, cmp.GetPosition().Y), // Links
                    new PointD(cmp.Bounds.Right + noteWidth / 2 + offset, cmp.GetPosition().Y), // Rechts
                    new PointD(cmp.GetPosition().X, cmp.Bounds.Top - noteHeight / 2 - offset), // Oben
                    new PointD(cmp.GetPosition().X, cmp.Bounds.Bottom + noteHeight / 2 + offset) // Unten
                };

                bool placed = false;
                RectangleD noteBounds = RectangleD.Empty;

                foreach (PointD TextPos in candidatePositions)
                {
                    noteBounds = new RectangleD(TextPos.X - noteWidth / 2, TextPos.Y - noteHeight / 2, noteWidth, noteHeight);
                    bool overlaps = false;

                    foreach (RectangleD occupied in occupiedAreas)
                    {
                        if (noteBounds.IntersectsWith(occupied))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        dimensionPlugin.AddNoteComment(cmp.GetPosition(), TextPos, MyValue, layerName, fontSize, fontName, fontColor);
                        occupiedAreas.Add(noteBounds);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    fontColor = notFoundTextColor;
                    dimensionPlugin.AddNoteComment(cmp.GetPosition(), cmp.GetPosition(), MyValue, layerName, fontSize, fontName, fontColor);
                    occupiedAreas.Add(noteBounds);
                }
            }

            progressDialog.Dispose();
            parent.UpdateView();
        }
    }

    public class InputForm : Form
    {
        public static string SelectedProperty { get; private set; }
        public static Color SelectedTextColor { get; private set; }
        public static Color NotFoundTextColor { get; private set; }

        private ComboBox propertyComboBox;
        private Button textColorButton;
        private Button notFoundColorButton;
        private ColorDialog colorDialog;

        public InputForm()
        {
            this.Text = "Set Note Properties";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;

            Label propertyLabel = new Label() { Text = "Choose Property:", Left = 10, Top = 20, Width = 100 };
            propertyComboBox = new ComboBox() { Left = 120, Top = 20, Width = 250 };
            propertyComboBox.Items.AddRange(new string[] { "PartName", "Value", "Package" });
            propertyComboBox.SelectedIndex = 0;

            Label textColorLabel = new Label() { Text = "Text Color:", Left = 10, Top = 60, Width = 100 };
            textColorButton = new Button() { Text = "Choose", Left = 120, Top = 60, Width = 100 };
            textColorButton.Click += (sender, e) =>
            {
                colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedTextColor = colorDialog.Color;
                    textColorButton.BackColor = SelectedTextColor;
                }
            };

            Label notFoundColorLabel = new Label() { Text = "Not Found Color:", Left = 10, Top = 100, Width = 100 };
            notFoundColorButton = new Button() { Text = "Choose", Left = 120, Top = 100, Width = 100 };
            notFoundColorButton.Click += (sender, e) =>
            {
                colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    NotFoundTextColor = colorDialog.Color;
                    notFoundColorButton.BackColor = NotFoundTextColor;
                }
            };

            Button okButton = new Button() { Text = "OK", Left = 150, Top = 150, Width = 80 };
            okButton.Click += (sender, e) =>
            {
                SelectedProperty = propertyComboBox.SelectedItem.ToString();
                if (SelectedTextColor == default) SelectedTextColor = Color.White;
                if (NotFoundTextColor == default) NotFoundTextColor = Color.Red;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(propertyLabel);
            this.Controls.Add(propertyComboBox);
            this.Controls.Add(textColorLabel);
            this.Controls.Add(textColorButton);
            this.Controls.Add(notFoundColorLabel);
            this.Controls.Add(notFoundColorButton);
            this.Controls.Add(okButton);
        }
    }
}
