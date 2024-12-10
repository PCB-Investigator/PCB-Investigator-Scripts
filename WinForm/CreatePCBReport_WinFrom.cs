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
using PCBI.MathUtils;
using System.Linq;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        public PScript()
        {
        }

        private StringBuilder report;

        public void Execute(IPCBIWindow parent)
        {
            IStep curStep = parent.GetCurrentStep();
            if (curStep == null) return;

            report = new StringBuilder();
            IMatrix matrix = parent.GetMatrix();
            if (matrix == null) return;

            int SignalLayerCount = GetLayerCount(matrix);
            double width = IMath.Mils2MM(curStep.GetPCBOutlineAsODBObject().Bounds.Width);
            double height = IMath.Mils2MM(curStep.GetPCBOutlineAsODBObject().Bounds.Height);

            report.AppendLine("PCB width  : " + width.ToString());
            report.AppendLine("PCB height : " + height.ToString());

            FindSmalestLine(curStep, matrix);
            GetNDKList(curStep, matrix);

            ShowResultsDialog();

            string rep = report.ToString();
            File.WriteAllText(@"D:\TMP\PCB_REP.TXT", rep);

            parent.UpdateView();
        }

   private int GetLayerCount(IMatrix matrix)
        {
            int count = 0;
            foreach (string signalLayerName in matrix.GetAllSignalLayerNames())
            {
                count++;
            }
            return count;
        }

        private Dictionary<string, double> FindSmalestLine(IStep step, IMatrix matrix)
        {
            Dictionary<string, double> smLineList = new Dictionary<string, double>();
            foreach (string layerName in matrix.GetAllSignalLayerNames())
            {
                double smDiameter = double.MaxValue;
                IODBLayer layer = (IODBLayer)step.GetLayer(layerName);
                bool linesFound = false;
                foreach (IODBObject obj in layer.GetAllLayerObjects())
                {
                    if (obj.Type == IObjectType.Line)
                    {
                        ILineSpecificsD line = (ILineSpecificsD)obj.GetSpecificsD();
                        double dia = line.Diameter;
                        if (dia < smDiameter)
                        {
                            smDiameter = dia;
                            linesFound = true;
                        }
                    }
                }
                if(linesFound)
                {
                   smLineList[layerName] = smDiameter;
                 report.AppendLine("Smallest Line: " + IMath.Mils2MM(smDiameter).ToString() + "  " + layerName);
                } else
                {
                smLineList[layerName] = 0;
                  report.AppendLine("No Line found: " + layerName);
                }
            }
            return smLineList;
        }

        private Dictionary<string, double> GetNDKList(IStep step, IMatrix matrix)
        {
            double SmalestDrill = double.MaxValue;
            Dictionary<string, double> AnualRingperLayer = new Dictionary<string, double>();

            foreach (string drillLayer in matrix.GetAllDrillLayerNames(true))
            {
                double MinAnnualRing = double.MaxValue;
                foreach (IODBObject drill in step.GetLayer(drillLayer).GetAllLayerObjects())
                {
                    if (drill.GetDiameter() < SmalestDrill)
                    {
                        SmalestDrill = drill.GetDiameter();
                    }

                    foreach (string layerName in matrix.GetAllSignalLayerNames())
                    {
                        bool PadFound = false;
                        IODBLayer layer = (IODBLayer)step.GetLayer(layerName);
                        List<IObject> hits = layer.GetAllObjectsOnPosition(drill.GetBoundsD().GetMidPoint());
                        if (hits != null && hits.Count > 0)
                        {
                            foreach (IObject obj in hits)
                            {
                                IODBObject odbObj = (IODBObject)obj;
                                if (odbObj.Type == IObjectType.Pad)
                                {
                                    if (drill.GetDiameter() < odbObj.GetDiameter())
                                    {
                                        PadFound = true;
                                        if (MinAnnualRing > odbObj.GetDiameter() - drill.GetDiameter())
                                        {
                                            MinAnnualRing = odbObj.GetDiameter() - drill.GetDiameter();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        AnualRingperLayer[layerName] = MinAnnualRing;
                    }
                }
                report.AppendLine("Smallest Drill: " + IMath.Mils2MM(SmalestDrill).ToString());
            }

            foreach (var kvp in AnualRingperLayer)
            {
                report.AppendLine("Min Annual Ring: " + IMath.Mils2MM(kvp.Value).ToString() + "  " + kvp.Key);
            }

            return AnualRingperLayer;
        }

        private void ShowResultsDialog()
        {
            Form resultForm = new Form
            {
                Text = "PCB Report",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            ListView listView = new ListView
            {
                View = View.Details,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true
            };

            listView.Columns.Add("Category", 150);
            listView.Columns.Add("Value", 150);
            listView.Columns.Add("Layer/Details", 200);

            string[] reportLines = report.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in reportLines)
            {
                string[] parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    string category = parts[0].Trim();
                    string[] valueAndLayer = parts[1].Trim().Split(new[] { ' ' }, 2);
                    string value = valueAndLayer[0].Trim();
                    string layer = valueAndLayer.Length > 1 ? valueAndLayer[1].Trim() : "";

                    listView.Items.Add(new ListViewItem(new[] { category, value, layer }));
                }
            }

            resultForm.Controls.Add(listView);
            
            resultForm.ShowDialog();
        }
    }
}