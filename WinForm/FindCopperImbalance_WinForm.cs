// Dieses PCB-Investigator-Skript analysiert die Kupferverteilung in einem geladenen PCB-Job. 
// Nach dem Start des Skriptes wird das aktuelle Step herangezogen, um die Kupferlagen (Signal-Layer) zu untersuchen. 
// Dabei wird die PCB-Kontur in 9 Teilbereiche (3x3 Raster) unterteilt und pro Kupfer-Lage die Teilflächen berechnet. 
// Anschließend werden die Flächen paarweise gegenübergestellt, um symmetrische Unterschiede in der Kupferverteilung 
// zu erkennen. Der relative Unterschied wird in Prozent ermittelt. Liegen diese Unterschiede über einem definierten 
// Schwellwert (lowerTreshhold), weist das Skript entsprechend auf mögliche Kupferimbalancen hin.
//
// Nach der Berechnung öffnet sich ein Ergebnisfenster (Form), in dem die Resultate pro Kupfer-Layer visuell dargestellt 
// werden. Es werden dazu Bitmap-Ansichten der einzelnen Layer erzeugt, in denen die 9 Teilbereiche farblich markiert 
// sind. Bereiche, die innerhalb des Toleranzbereichs liegen, werden beispielsweise in Grün, deutlich abweichende 
// Teilbereiche in Rot dargestellt. Zusätzlich wird der angewandte Schwellwert angezeigt. 
//
// Dieses Skript ermöglicht es somit, potenzielle Kupferimbalancen schnell zu erkennen und unterstützt bei der 
// Bewertung der Kupferverteilung im PCB-Layout.

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
// GUID=CopperBalance_638641531264786697
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

namespace PCBIScript
{
    public class PScript : IPCBIScript
    {
        double lowerTreshhold = 0.15;

        public PScript()
        {
        }

        public void Execute(IPCBIWindow parent)
        {
            if (!parent.JobIsLoaded) return;
            AnalyseCopperDistrubution(parent);
            parent.UpdateView();
        }

        public void AnalyseCopperDistrubution(IPCBIWindow Parent)
        {
            IMatrix matrix = Parent.GetMatrix();
            IStep step = Parent.GetCurrentStep();
            List<string> allCopperLayers = matrix.GetAllSignalLayerNames();
            ISurfaceSpecificsD specs = step.GetPCBOutlineAsSurfaceD();
            Parent.SaveJob();

            if (specs == null)
            {
                MessageBox.Show("Failed to get the outline of the step", "Error", MessageBoxButtons.OK);
                return;
            }

            RectangleD bounds = specs.GetBounds();
            List<List<double>> areaMatrix = new List<List<double>>();

            foreach (string layerName in allCopperLayers)
            {
                List<double> areasByRectangle = AnalyseLayer(Parent, matrix, step, bounds, layerName);
                List<double> areaDifferncePercent = GetAreaByPercentDifference(areasByRectangle);
                areaMatrix.Add(areaDifferncePercent);
            }

            ResultsForm form = new ResultsForm(Parent, areaMatrix, lowerTreshhold);
            form.GenerateImages();
            form.ShowDialog();
        }

        private List<double> AnalyseLayer(IPCBIWindow parent, IMatrix matrix, IStep step, RectangleD bounds, string layerName)
        {
            ILayer layer = step.GetLayer(layerName);
            if (layer == null || !(layer is IODBLayer))
            {
                return new List<double>();
            }

            List<double> allAreas = new List<double>();
            List<RectangleD> dividedRectangles = GetDividedRectangles(bounds);
            IODBLayer iODB = (IODBLayer)layer;

            foreach (RectangleD rect in dividedRectangles)
            {
                List<IObject> objs = iODB.GetAllObjectInRectangle(rect);
                List<IPolyClass> polyList = new List<IPolyClass>();
                foreach (IObject obj in objs)
                {
                    polyList.Add(obj.GetPolygonOutline());
                }
                IPolyClass poly = IPolyClass.Union(polyList);
                poly = poly.Intersect(IPolyClass.FromRectangle(rect));
                double area = poly.GetArea(true);
                allAreas.Add(area);
            }

            return allAreas;
        }

        private List<double> GetAreaByPercentDifference(List<double> areasByRectangle)
        {
            List<double> returnList = new List<double>();
            if (areasByRectangle.Count != 9) return new List<double>();

            for (int i = 0; i < 4; i++)
            {
                double smallerArea = areasByRectangle[areasByRectangle.Count - 1 - i] > areasByRectangle[i] ? areasByRectangle[i] : areasByRectangle[areasByRectangle.Count - 1 - i];
                double biggerArea = areasByRectangle[areasByRectangle.Count - 1 - i] > areasByRectangle[i] ? areasByRectangle[areasByRectangle.Count - 1 - i] : areasByRectangle[i];
                double difference = smallerArea - biggerArea;
                double percentage = difference / biggerArea;
                returnList.Add(percentage);
            }

            return returnList;
        }

        private List<RectangleD> GetDividedRectangles(RectangleD bounds)
        {
            RectangleD originalRectangle = bounds;
            List<RectangleD> dividedRectangles = new List<RectangleD>();
            double smallWidth = originalRectangle.Width / 3;
            double smallHeight = originalRectangle.Height / 3;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    double smallX = originalRectangle.X + col * smallWidth;
                    double smallY = originalRectangle.Y + row * smallHeight;
                    RectangleD smallRectangle = new RectangleD(smallX, smallY, smallWidth, smallHeight);
                    dividedRectangles.Add(smallRectangle);
                }
            }

            return dividedRectangles;
        }
    }

    public class ResultsForm : Form
    {
        private IPCBIWindow parent;
        private List<List<double>> imbalanceMatrix = new List<List<double>>();
        private List<Bitmap> images = new List<Bitmap>();
        private List<string> allLayers = new List<string>();
        private double lowerTresh = 0;
        private TabControl tabControl;

        public ResultsForm(IPCBIWindow Parent, List<List<double>> resultsMatrix, double treshhold)
        {
            parent = Parent;
            imbalanceMatrix = resultsMatrix;
            allLayers = parent.GetMatrix().GetAllSignalLayerNames();
            lowerTresh = treshhold;

            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(ResultsForm_closing);
        }

        private void InitializeComponent()
        {
            this.Text = "Copper Imbalance Results";
            this.Size = new Size(830, 880);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);
        }

        public void GenerateImages()
        {
            IStep step = parent.GetCurrentStep();
            ISurfaceSpecifics specs = step.GetPCBOutlineAsSurface();
            if (specs == null) return;
            RectangleF bounds = specs.GetBounds();
            RectangleF drawRect;

            for (int i = 0; i < allLayers.Count; i++)
            {
                Bitmap map = step.GetBitmap(new List<ILayer>() { step.GetLayer(allLayers[i]) }, bounds, (int)bounds.Width, (int)bounds.Height, out drawRect);
                float fontSize = (float)(map.Width / 3 * 0.085);

                using (Graphics g = Graphics.FromImage(map))
                using (Pen p = new Pen(Color.Yellow, 20f))
                using (Font fontTresh = new Font("Arial", fontSize))
                {
                    List<double> currentMatrix = imbalanceMatrix[i];
                    List<RectangleF> rects = GetDividedRectanglesWithOffset(drawRect);
                    rects = Shuffle(rects);
                    if (rects.Count != 9) continue;

                    RectangleF treshholdRect = rects[7];
                    SizeF stringSize = g.MeasureString("Treshhold: " + Math.Round(lowerTresh * 100, 2).ToString() + "%", fontTresh);
                    g.FillRectangle(Brushes.Yellow, new RectangleF(treshholdRect.X + treshholdRect.Width / 2 - stringSize.Width / 2, treshholdRect.Y + 20, stringSize.Width, stringSize.Height));
                    g.DrawString("Treshhold: " + Math.Round(lowerTresh * 100, 2).ToString() + "%", fontTresh, Brushes.Black, new PointF(treshholdRect.X + treshholdRect.Width / 2 - stringSize.Width / 2, treshholdRect.Y + 20));

                    foreach (RectangleF rect in rects)
                    {
                        g.DrawRectangle(p, rect.Location.X, rect.Location.Y, rect.Width, rect.Height);
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        RectangleF one = rects[j];
                        RectangleF two = rects[rects.Count - 1 - j];
                        double absoluteval = Math.Abs(currentMatrix[j]);
                        Brush b = lowerTresh > absoluteval ? new SolidBrush(Color.FromArgb(155, Color.Green)) : new SolidBrush(Color.FromArgb(155, Color.Red));
                        DrawFilledRectangle(one, two, b, g, j.ToString());
                        using (Font font = new Font("Arial", fontSize))
                        {
                            g.DrawString(Math.Round(absoluteval * 100, 2).ToString() + "%", font, Brushes.Black, new PointF(one.X, one.Y + stringSize.Height * 1.1f));
                            g.DrawString(Math.Round(absoluteval * 100, 2).ToString() + "%", font, Brushes.Black, new PointF(two.X, two.Y + stringSize.Height * 1.1f));
                        }
                    }
                }

                Bitmap mapToAdd = ScaleImageWithBorders(map, new Size(800, 800));
                images.Add(mapToAdd);

                // Create a new tab for each layer
                TabPage tabPage = new TabPage(allLayers[i]);
                PictureBox pictureBox = new PictureBox
                {
                    Image = mapToAdd,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Dock = DockStyle.Fill
                };
                tabPage.Controls.Add(pictureBox);
                tabControl.TabPages.Add(tabPage);
            }
        }

        public Bitmap ScaleImageWithBorders(Image image, Size targetSize)
        {
            int targetWidth = targetSize.Width;
            int targetHeight = targetSize.Height;
            double aspectRatio = (double)image.Width / image.Height;
            double targetAspectRatio = (double)targetWidth / targetHeight;
            int scaledWidth, scaledHeight;
            int xOffset = 0, yOffset = 0;

            if (aspectRatio > targetAspectRatio)
            {
                scaledWidth = targetWidth;
                scaledHeight = (int)(targetWidth / aspectRatio);
                yOffset = (targetHeight - scaledHeight) / 2;
            }
            else
            {
                scaledWidth = (int)(targetHeight * aspectRatio);
                scaledHeight = targetHeight;
                xOffset = (targetWidth - scaledWidth) / 2;
            }

            Bitmap scaledImage = new Bitmap(targetWidth, targetHeight);
            using (Graphics graphics = Graphics.FromImage(scaledImage))
            {
                graphics.FillRectangle(Brushes.Gray, 0, 0, targetWidth, targetHeight);
                graphics.DrawImage(image, xOffset, yOffset, scaledWidth, scaledHeight);
            }
            return scaledImage;
        }

        public void DrawFilledRectangle(RectangleF rectOne, RectangleF rectTwo, Brush brush, Graphics g, string j)
        {
            g.FillRectangle(brush, rectOne.Location.X, rectOne.Location.Y, rectOne.Width, rectOne.Height);
            g.FillRectangle(brush, rectTwo.Location.X, rectTwo.Location.Y, rectTwo.Width, rectTwo.Height);
            float fontSize = (float)(rectOne.Width * 0.12);
            using (Font font = new Font("Arial", fontSize))
            {
                g.DrawString(j, font, Brushes.Black, new PointF(rectOne.X, rectOne.Y));
                g.DrawString(j, font, Brushes.Black, new PointF(rectTwo.X, rectTwo.Y));
            }
        }

        public List<RectangleF> Shuffle(List<RectangleF> rects)
        {
            if (rects.Count != 9) return rects;
            List<RectangleF> one = new List<RectangleF>() { rects[0], rects[1], rects[2] };
            List<RectangleF> two = new List<RectangleF>() { rects[3], rects[4], rects[5] };
            List<RectangleF> three = new List<RectangleF>() { rects[6], rects[7], rects[8] };
            List<RectangleF> returnList = new List<RectangleF>();
            returnList.AddRange(three);
            returnList.AddRange(two);
            returnList.AddRange(one);
            return returnList;
        }

        private List<RectangleF> GetDividedRectanglesWithOffset(RectangleF rect)
        {
            RectangleF originalRectangle = new RectangleF(0, 0, rect.Width, rect.Height);
            List<RectangleF> dividedRectangles = new List<RectangleF>();
            float smallWidth = originalRectangle.Width / 3;
            float smallHeight = originalRectangle.Height / 3;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    float smallX = originalRectangle.X + col * smallWidth;
                    float smallY = originalRectangle.Y + row * smallHeight;
                    RectangleF smallRectangle = new RectangleF(smallX, smallY, smallWidth, smallHeight);
                    dividedRectangles.Add(smallRectangle);
                }
            }

            return dividedRectangles;
        }

        private void ResultsForm_closing(object sender, FormClosingEventArgs e)
        {
            foreach (Bitmap map in images)
            {
                map.Dispose();
            }
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is PictureBox)
                    {
                        ((PictureBox)control).Image.Dispose();
                    }
                    control.Dispose();
                }
                tabPage.Dispose();
            }
            tabControl.Dispose();
        }
    }
}