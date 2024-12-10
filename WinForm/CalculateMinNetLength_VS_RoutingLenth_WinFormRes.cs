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
// GUID=CalculateNetLength_638641531264786697
// ButtonEnabled=1   (Button enabled if: 1=Design is loaded, 2=Always, 4=Design contains components, 8=Loaded step is a panel, 16=Element is selected, 32=Component is selected)
// AutoStart=false

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using PCBI.Plugin;
using PCBI.Plugin.Interfaces;
using PCBI.Automation;
using PCBI.MathUtils;

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        public PScript() { }

        public void Execute(IPCBIWindow parent)
        {
            IStep curStep = parent.GetCurrentStep();
            if (curStep == null) return;

            Dictionary<string, double> netMinimalLengths = CalcMinimalNetLengths(parent);
            List<NetInfo> netInfos = new List<NetInfo>();

            foreach (var kvp in netMinimalLengths.OrderBy(x => x.Key))
            {
                double lengthOfAllLines = 0;
                foreach (IODBObject obj in curStep.GetNet(kvp.Key).GetAllNetObjects(parent))
                {
                    if (obj.Type == IObjectType.Arc)
                    {
                        IArcSpecificsD arcSpec = (IArcSpecificsD)obj.GetSpecificsD();
                        double angleOfArc = IMath.GetAngle(arcSpec.Start, arcSpec.End, arcSpec.Center, arcSpec.ClockWise);
                        double lengthOfArc = IMath.DistanceOnArc(angleOfArc, arcSpec.Diameter / 2);
                        lengthOfAllLines += lengthOfArc;
                    }
                    if (obj.Type == IObjectType.Line)
                    {
                        ILineSpecificsD lineSpec = (ILineSpecificsD)obj.GetSpecificsD();
                        double lengthOfLine = IMath.DistancePointToPoint(lineSpec.Start, lineSpec.End);
                        lengthOfAllLines += lengthOfLine;
                    }
                }
                lengthOfAllLines = IMath.Mils2MM(lengthOfAllLines);
                netInfos.Add(new NetInfo { NetName = kvp.Key, MinNetLength = kvp.Value, RoutedLength = lengthOfAllLines });
            }

            ShowResultsForm(netInfos);
            parent.UpdateView();
        }

        public Dictionary<string, double> CalcMinimalNetLengths(IPCBIWindow parent)
        {
            IStep currentStep = parent.GetCurrentStep();
            if (currentStep == null) return new Dictionary<string, double>();

            Dictionary<string, double> netMinimalLengths = new Dictionary<string, double>();

            foreach (string netName in currentStep.GetAllNetNames())
            {
                List<ICMPObject> CompsOnNet = currentStep.GetAllCMPsWithNetConnectionTo(currentStep.GetNetNrFromNetName(netName));
                if (CompsOnNet.Count < 2) continue;

                List<PointD> pinsOnNet = new List<PointD>();
                foreach (ICMPObject compOnNet in CompsOnNet)
                {
                    foreach (IPin pin in compOnNet.GetPinList())
                    {
                        if (pin.GetNetNameOnIPin(compOnNet).ToLower() == netName.ToLower())
                            pinsOnNet.Add(pin.GetIPinPositionD(compOnNet));
                    }
                }

                double minimalLength = CalculateMinimalSpanningTreeLength(pinsOnNet);
                netMinimalLengths[netName] = minimalLength;
            }
            return netMinimalLengths;
        }

        private double CalculateMinimalSpanningTreeLength(List<PointD> points)
        {
            if (points.Count < 2) return 0;

            List<int> mstSet = new List<int> { 0 };
            double totalLength = 0;

            while (mstSet.Count < points.Count)
            {
                double minDistance = double.MaxValue;
                int nextPoint = -1;

                for (int i = 0; i < points.Count; i++)
                {
                    if (mstSet.Contains(i)) continue;

                    foreach (int j in mstSet)
                    {
                        double distance = IMath.DistancePointToPoint(points[i], points[j]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nextPoint = i;
                        }
                    }
                }

                if (nextPoint != -1)
                {
                    mstSet.Add(nextPoint);
                    totalLength += IMath.Mils2MM(minDistance);
                }
            }

            return totalLength;
        }

        private void ShowResultsForm(List<NetInfo> netInfos)
        {
            Form resultsForm = new Form
            {
                Text = "Net Lengths",
                Width = 800,
                Height = 600
            };

            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = netInfos
            };

            resultsForm.Controls.Add(dataGridView);
            resultsForm.ShowDialog();
        }

        private class NetInfo
        {
            public string NetName { get; set; }
            public double MinNetLength { get; set; }
            public double RoutedLength { get; set; }
        }
    }
}
//-----------------------------------------------------------------------------------
// PCB-Investigator Automation Script (Synchronous)
// Created on: 09.12.2024
// Author: Guenther
// SDK online reference: http://manual.pcb-investigator.com/InterfaceDocumentation/index.php
// SDK: http://www.pcb-investigator.com/sdk-participate
//
// Description:
// This script evaluates the routing efficiency of each net in the currently loaded PCB design
// by comparing the minimal spanning tree (MST) length between all pins on that net with the 
// actual routed length of its traces (including lines and arcs).
//
// Steps performed by the script:
// 1. For each net in the design, gather all component pins belonging to that net.
// 2. Calculate the minimal spanning tree length of these pins (the minimal total distance 
//    connecting all pins). This represents the theoretical shortest possible routing length.
// 3. Determine the actual routed length by summing the physical lengths of all line and arc 
//    segments belonging to that net.
// 4. Convert all lengths to millimeters for consistency.
// 5. Present a comparison table showing the net name, minimal net length, and the actual 
//    routed length in a dialog window.
//
// This allows designers or engineers to quickly identify which nets are not optimally routed 
// and could potentially be improved.
//
// Requirements:
// - A loaded design in PCB-Investigator.
//
// Note:
// Modify the script as needed to integrate it into your workflow, or adapt it for more 
// advanced routing assessments.
//
//-----------------------------------------------------------------------------------

