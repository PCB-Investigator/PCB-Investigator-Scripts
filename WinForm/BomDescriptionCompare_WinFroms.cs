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
// GUID=BonDescriptionCompare_638641531264786697
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
using PCBIScript.DebugHelp;
using System.IO;
using System.Linq;
using System.Drawing.Drawing2D;
using PCBI.MathUtils;
using TextCompare;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        private IStep curStep;
        private IPCBIWindow iPCBIWindow;
        public bool Canceled = false;
        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            iPCBIWindow = parent;
            curStep = iPCBIWindow.GetCurrentStep();
            if (curStep == null) return;

            var propertySelector = new PropertySelectorForm(parent);
            if (propertySelector.ShowDialog() == DialogResult.OK)
            {
                // Fortschrittsdialog erstellen
                PCB_Investigator.PCBIWindows.PCBIWorkingDialog progressDialog = new PCB_Investigator.PCBIWindows.PCBIWorkingDialog();
                progressDialog.SetAnimationStatus(false);
                progressDialog.SetStatusText("Comparing Properties...");
                progressDialog.CanCancel(true);
                progressDialog.ShowWorkingDlgAsThread();

                progressDialog.CancelPressed += () => progressDialog.Dispose(); // Fortschrittsdialog schließen, wenn abgebrochen

                var compare = new TextCompare.FormCompareList();
                compare.SelectCMPEvent += SelectInPCBI;
                
                progressDialog.CancelPressed += () =>
                {
                Canceled = true;
                progressDialog.Dispose();
                };
                // Anzahl der Komponenten für Fortschrittsanzeige
                double totalComponents = curStep.GetAllCMPObjects().Count;
                double progressStep = 100.0 / totalComponents;
                double progressValue = 0;

                foreach (var cmp in curStep.GetAllCMPObjects())
                {
                    progressValue += progressStep;
                    progressDialog.SetStatusPercent((int)progressValue);

                    if (Canceled) // Prüfen, ob der Fortschrittsdialog abgebrochen wurde
                    {
                        break;
                    }
                }

                GetPropertiesFromStep(parent, compare, propertySelector.SelectedProperties);
                compare.Show(parent.MainForm);

                progressDialog.Dispose(); // Fortschrittsdialog schließen
            }

            parent.UpdateView();
        }

        private void SelectInPCBI(string Reference)
        {
            foreach (ICMPObject cmp in curStep.GetAllCMPObjects())
            {
                cmp.Select(cmp.PartName.ToLower().Trim() == Reference.ToLower().Trim());
            }
            iPCBIWindow.UpdateSelection();
            iPCBIWindow.ZoomToSelection();
        }

        public void GetPropertiesFromStep(IPCBIWindow iPCBIWindow, FormCompareList formCompare, List<string> selectedProperties)
        {
            formCompare.AddHeader(selectedProperties[0], selectedProperties[1], selectedProperties[2], Color.DarkBlue, Color.YellowGreen);

            IStep step = iPCBIWindow.GetCurrentStep();
            if (step == null) return;

            Dictionary<string, List<ICMPObject>> parts = new Dictionary<string, List<ICMPObject>>();

            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                var attribs = cmp.GetComponentAttributeClasses();
                string key = GetPropertyValue(cmp, attribs, selectedProperties[0]);
                if (!parts.ContainsKey(key))
                {
                    parts[key] = new List<ICMPObject>();
                }
                parts[key].Add(cmp);
            }

            foreach (var partGroup in parts.Values)
            {
                if (partGroup.Count > 0)
                {
                    var cmp = partGroup[0];
                    var attribs = cmp.GetComponentAttributeClasses();

                    string textLeft = GetPropertyValue(cmp, attribs, selectedProperties[1]);
                    string textRight = GetPropertyValue(cmp, attribs, selectedProperties[2]);
                    string key = GetPropertyValue(cmp, attribs, selectedProperties[0]);
                    formCompare.AddLine(key, textLeft, textRight, Color.Empty, Color.Empty, true);
                }
            }
        }

        private string GetPropertyValue(ICMPObject cmp, Dictionary<string, IEDA_PRP> attribs, string propertyName)
        {
            switch (propertyName)
            {
                case "Ref":
                    return cmp.Ref;
                case "PartName":
                    return cmp.PartName;
                case "UsedPackageName":
                    return cmp.UsedPackageName;
                default:
                    return attribs.ContainsKey(propertyName) ? attribs[propertyName].ToValueString() : "";
            }
        }
    }

    public class PropertySelectorForm : Form
    {
        public List<string> SelectedProperties { get; private set; }
        private ComboBox[] propertySelectors;
        private IPCBIWindow parent = null;

        public PropertySelectorForm(IPCBIWindow Parent)
        {
            this.parent = Parent;
            InitializeComponent();
        }

        public List<string> ComponentProperties(IStep step)
        {
            var properties = new List<string>();

            // Finde alle verfügbaren Eigenschafts-Schlüssel für Komponenten
            foreach (ICMPObject cmp in step.GetAllCMPObjects())
            {
                var attribs = cmp.GetComponentAttributeClasses();
                foreach (var attrib in attribs)
                {
                    if (!properties.Contains(attrib.Key))
                    {
                        properties.Add(attrib.Key);
                    }
                }
            }
            properties.Add("Ref");
            properties.Add("PartName");
            properties.Add("UsedPackageName");
            return properties;
        }

        private void InitializeComponent()
        {
            this.Text = "Select Properties for Comparison";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            IStep step = parent.GetCurrentStep();
            var properties = ComponentProperties(step);

            propertySelectors = new ComboBox[3];
            string[] labels = { "Key Property:", "Left Column:", "Right Column:" };

            for (int i = 0; i < 3; i++)
            {
                var label = new Label { Text = labels[i], Left = 10, Top = 20 + i * 30, Width = 100 };
                propertySelectors[i] = new ComboBox
                {
                    Left = 120,
                    Top = 20 + i * 30,
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                propertySelectors[i].Items.AddRange(properties.ToArray());
                propertySelectors[i].SelectedIndex = i;

                this.Controls.Add(label);
                this.Controls.Add(propertySelectors[i]);
            }
            int RefIndex = propertySelectors[0].FindString("Ref");
            propertySelectors[0].SelectedIndex = RefIndex;
            var okButton = new Button { Text = "OK", Left = 120, Top = 120, Width = 75 };
            okButton.Click += (sender, e) =>
            {
                SelectedProperties = propertySelectors.Select(cb => cb.SelectedItem.ToString()).ToList();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(okButton);

            var cancelButton = new Button { Text = "Cancel", Left = 210, Top = 120, Width = 75 };
            cancelButton.Click += (sender, e) => this.Close();
            this.Controls.Add(cancelButton);
        }
    }
}

//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script (Synchronous)
// Created on: 09.12.2024
// Author:Guenther
// SDK online reference: http://manual.pcb-investigator.com/InterfaceDocumentation/index.php
// SDK: http://www.pcb-investigator.com/sdk-participate
//
// Description:
// This script allows users to select three component-related properties from the current PCB design
// and compare them side-by-side in a dedicated comparison form. By choosing a "Key Property" as well
// as a "Left" and "Right" property, the user can quickly verify differences or similarities across
// multiple components.
//
// The script performs the following steps:
// 1. Presents an input dialog that lets the user pick three properties: one key property (e.g., Ref)
//    and two additional properties to be displayed as left and right columns.
// 2. Once properties are selected, the script gathers all components from the current PCB step and
//    retrieves their chosen property values.
// 3. It then compiles these values into a comparison table using the TextCompare functionality,
//    enabling the user to easily review property differences or correlations between components.
// 4. During data preparation, a progress dialog is displayed, providing visual feedback and offering
//    the option to cancel the process if needed.
// 5. After the data is collected, the comparison dialog is shown. Selecting entries within this
//    dialog can highlight corresponding components in PCB-Investigator and zoom to them for better
//    inspection.
//
// Requirements:
// - A design must be loaded.
// - The TextCompare plugin and any required references must be available in the environment.
//
// Note:
// This script is meant to be integrated into PCB-Investigator as an automation script. Modify the
// GUID, name, or other integration parameters as needed to place it in the ribbon menu or run it
// directly from the automation interface.
//
//-----------------------------------------------------------------------------------

