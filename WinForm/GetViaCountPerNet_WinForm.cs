using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using PCBI.Automation;
using System.Linq;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            IStep step = parent.GetCurrentStep();
            if (step == null)
            {
                MessageBox.Show("No step loaded. Please load a PCB design first.");
                return;
            }

            int viaCount = 0;
            Dictionary<string, int> viaTypeCount = new Dictionary<string, int>();

            IMatrix matrix = parent.GetMatrix();
            List<string> allLayerNames = matrix.GetAllLayerNames();
            
            foreach (string layerName in allLayerNames)
            {
                ILayer layer = step.GetLayer(layerName);
                if (matrix.GetMatrixLayerType(layerName) == MatrixLayerType.Drill)
                {
                    IODBLayer odbLayer = (IODBLayer)layer;
                    
                    foreach (IODBObject odbObj in odbLayer.GetAllLayerObjects())
                    {
                        if (((IODBObject)odbObj).Type == IObjectType.Pad)
                        {
                            Dictionary<PCBI.FeatureAttributeEnum, string> attributesOfPad = ((IODBObject)odbObj).GetAttributesDictionary();

                            if (attributesOfPad.ContainsKey(PCBI.FeatureAttributeEnum.drill))
                            {
                                if (attributesOfPad[PCBI.FeatureAttributeEnum.drill] == "via")
                                { 
                                    viaCount++;
                                    string diameter = odbObj.GetDiameter().ToString();
                                    if (!viaTypeCount.ContainsKey(diameter))
                                        viaTypeCount[diameter] = 0;
                                    viaTypeCount[diameter]++;
                                }
                            }
                        }
                    }
                }
            }

            // Display the result in a WinForms dialog
            using (var resultForm = new ViaCountResultForm(viaCount, viaTypeCount))
            {
                resultForm.ShowDialog();
            }
        }
    }

    public class ViaCountResultForm : Form
    {
        public ViaCountResultForm(int totalViaCount, Dictionary<string, int> viaTypeCount)
        {
            InitializeComponent(totalViaCount, viaTypeCount);
        }

        private void InitializeComponent(int totalViaCount, Dictionary<string, int> viaTypeCount)
        {
            this.Text = "Via Count Results";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            var resultTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };

            StringBuilder resultText = new StringBuilder();
            resultText.AppendLine($"Total number of vias: {totalViaCount}");
            resultText.AppendLine("\nVia count by diameter:");
            foreach (var kvp in viaTypeCount.OrderBy(x => x.Key))
            {
                resultText.AppendLine($"Diameter {kvp.Key}: {kvp.Value}");
            }

            resultTextBox.Text = resultText.ToString();

            this.Controls.Add(resultTextBox);
        }
    }
}