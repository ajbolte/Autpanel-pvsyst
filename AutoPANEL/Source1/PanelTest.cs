using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using netDxf.Tables;

using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using System.Diagnostics;

namespace WpfApp1.Source
{
     [Serializable] public class PanelTest
    {
        Point startPoint = new Point(0, 0);
        public MinMax minmax;
        public Angles angles;
        public Point panelSize = new Point();
        public Int16 xIt = 0;
        public Int16 yIt = 0;
        public PointCollection boundary = new PointCollection();
        public int panelCountX = 30;
        public int panelCountY = 30;
        public Double yGap = 0.06;
        public Double xGap = 0.06;
        public Double ySpace = 1.6;
        public Double xSpace = 1.6;
        public Int16 yRows = 4;
        public Int16 xRows = 30;
        public Int32 xSect = 4;
        public Int32 ySect = 30;
        public Int32 num = 0;
        public Int32 lastpanelSet = 0;
        public int roofPitch = 0;
        public int panelTilt = 0;
        public int panelWatts = 290;
        public int panelColor = 0;
        public int outlineColor = 0;
        public int shadingColor = 0;
        public string name = "Untitled";
        public string map = "";
     
        public double f;
        
        //public PointCollection boundaryPoints = new PointCollection();
        public List<PointCollection> points = new List<PointCollection>();
        public List<PointCollection> shading = new List<PointCollection>();
        public PointCollection outline = new PointCollection();

        public Double scale = 1;
        public Double dxfScale = 1;
    
        PointCollection testPoints = new PointCollection();
        public PointCollection panelPoints = new PointCollection();
     
        DxfDocument loaded = null;
        public PointCollection panelCorners = new PointCollection();
        Point offsetX = new Point();
        Point offsetY = new Point();


        public PanelTest()
        { }

       

        public void setPanelSize(double panelWidth,double panelLength)
        {
            panelSize.X = panelLength / scale;
            panelSize.Y = panelWidth / scale;

            PanelTestSetup();


        }
        public void newPanelSize(double newScale)
        {
            panelSize.X = panelSize.X * scale;
            panelSize.Y = panelSize.Y * scale;
            panelSize.X = panelSize.X / newScale;
            panelSize.Y = panelSize.Y / newScale;

           


        }
        public void setPanelGap(double panelgapX, double panelGapY)
        {
            yGap = panelGapY / scale;
            xGap = panelgapX / scale;
        }


        public void setRows(short YRows, short XRows)
        {
            yRows = YRows;
            xRows = XRows;
        }

        public void setSections(short YSect, short XSect)
        {
            ySect = YSect;
            xSect = XSect;
        }
        public void setWalks(short yWalk, short xWalk)
        {
            yRows = yWalk;
            xRows = xWalk;
        }

        public void setWalksDist(double yWalkDist, double xWalkDist)
        {
            ySpace = yWalkDist/ scale;
            xSpace = xWalkDist/ scale;
        }

        public void reset()
        {
            setWalksDist(ySpace, xSpace);
           setPanelGap(xGap, yGap);
           
            PanelTestSetup();

        }

        public void PanelTestSetup()
        {
             minmax = new MinMax(boundary);


            angles = new Angles(boundary[0], boundary[1]);
            
            if (angles.angle == 0)
            {
                angles.angle = 0.00001;
            }
            startPoint = minmax.intercept;
            {
            
                panelCorners = new PointCollection();
                testPoints = new PointCollection();
                Point p = new Point(0, 0);
                panelCorners.Add(p);
                testPoints.Add(p);
                p = new Point(panelSize.X * angles.cos, panelSize.X * angles.sin);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(panelSize.X * angles.cos - panelSize.Y * angles.sin, panelSize.X * angles.sin + panelSize.Y * angles.cos);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(-panelSize.Y * angles.sin, panelSize.Y * angles.cos);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(panelSize.X * angles.cos / 3, panelSize.X * angles.sin / 3);
                testPoints.Add(p);
                p = new Point(2 * panelSize.X * angles.cos / 3, 2 * panelSize.X * angles.sin / 3);
                testPoints.Add(p);
                p = new Point(-panelSize.Y * angles.sin / 2, panelSize.Y * angles.cos / 2);
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos / 2) - (panelSize.Y * angles.sin / 2), (panelSize.X * angles.sin / 2) + (panelSize.Y * angles.cos / 2));
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos) - (panelSize.Y * angles.sin / 2), (panelSize.X * angles.sin) + (panelSize.Y * angles.cos / 2));
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos / 3) - (panelSize.Y * angles.sin), (panelSize.X * angles.sin / 3) + (panelSize.Y * angles.cos));
                testPoints.Add(p);
                p = new Point((2 * panelSize.X * angles.cos / 3) - (panelSize.Y * angles.sin), (2 * panelSize.X * angles.sin / 3) + (panelSize.Y * angles.cos));
                testPoints.Add(p);

                offsetX = new Point(panelSize.X * angles.cos / xSect, panelSize.X * angles.sin / xSect);
                offsetY = new Point(-panelSize.Y * angles.sin / ySect, panelSize.Y * angles.cos / ySect);
                Debug.WriteLine("    Xoff: " + offsetX.ToString() + "   Y off: " + offsetY.ToString());
            }
            //Debug.Print(("      X length: " + minmax.xLength.ToString() + "  y length: " + minmax.yLength.ToString() + "  angle: " + angles.angle.ToString()));
            panelCountX = Convert.ToInt32(+Math.Abs((minmax.xLength * Math.Cos(angles.angle)) + Math.Abs(minmax.yLength * Math.Sin(angles.angle))) / panelSize.X) + 17;
            panelCountY = Convert.ToInt32(+Math.Abs((minmax.yLength * Math.Cos(angles.angle)) + Math.Abs(minmax.xLength * Math.Sin(angles.angle))) / panelSize.Y) + 17;
           // Debug.Print((" x count: " + panelCountX.ToString() + " y count: " + panelCountY.ToString()));
           // Debug.Print((" x size: " + panelSize.X.ToString() + " y size: " + panelSize.Y.ToString()));

        }

        public void ChangeDirection(Double angle)
        {
         
            angles.changeAngle(angle);
            {
            
                panelCorners = new PointCollection();
                testPoints = new PointCollection();
                Point p = new Point(0, 0);
                panelCorners.Add(p);
                testPoints.Add(p);
                p = new Point(panelSize.X * angles.cos, panelSize.X * angles.sin);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(panelSize.X * angles.cos - panelSize.Y * angles.sin, panelSize.X * angles.sin + panelSize.Y * angles.cos);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(-panelSize.Y * angles.sin, panelSize.Y * angles.cos);
                testPoints.Add(p);
                panelCorners.Add(p);
                p = new Point(panelSize.X * angles.cos / 3, panelSize.X * angles.sin / 3);
                testPoints.Add(p);
                p = new Point(2 * panelSize.X * angles.cos / 3, 2 * panelSize.X * angles.sin / 3);
                testPoints.Add(p);
                p = new Point(-panelSize.Y * angles.sin / 2, panelSize.Y * angles.cos / 2);
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos / 2) - (panelSize.Y * angles.sin / 2), (panelSize.X * angles.sin / 2) + (panelSize.Y * angles.cos / 2));
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos) - (panelSize.Y * angles.sin / 2), (panelSize.X * angles.sin) + (panelSize.Y * angles.cos / 2));
                testPoints.Add(p);
                p = new Point((panelSize.X * angles.cos / 3) - (panelSize.Y * angles.sin), (panelSize.X * angles.sin / 3) + (panelSize.Y * angles.cos));
                testPoints.Add(p);
                p = new Point((2 * panelSize.X * angles.cos / 3) - (panelSize.Y * angles.sin), (2 * panelSize.X * angles.sin / 3) + (panelSize.Y * angles.cos));
                testPoints.Add(p);
            }
            //  offsetX = new Point(panelSize.X * angles.cos / 10, panelSize.X * angles.sin / 10);
            //  offsetY = new Point(-panelSize.Y * angles.sin / 3, panelSize.Y * angles.cos / 3);
                offsetX = new Point(panelSize.X * angles.cos / xSect, panelSize.X * angles.sin / xSect);
                offsetY = new Point(-panelSize.Y * angles.sin / ySect, panelSize.Y * angles.cos / ySect);


        }
        public PointCollection GoTest(int xOffset, int yOffset)
        {
            Debug.WriteLine("    Xoff: " + offsetX.ToString() + "   Y off: " + offsetY.ToString());
            Point origin = new Point(startPoint.X + offsetX.X * (-xOffset) + offsetY.X * (-yOffset), startPoint.Y + offsetX.Y * (-xOffset) + offsetY.Y * (-yOffset));
          //  MessageBox.Show("start: " + startPoint.X.ToString() + " " + startPoint.Y.ToString());
        //    MessageBox.Show("Origin: " + origin.X.ToString() + " " + origin.Y.ToString());

            Point[][] pArray = new Point[panelCountX][];
            for (int i = 0; i < panelCountX; i++)
            {
                pArray[i] = new Point[panelCountY];
            }

            for (int i = 0; i < panelCountY; i++)
            {
                Double rowCalcY = i / yRows;
                int j = Convert.ToInt32(Math.Truncate(rowCalcY));
                rowCalcY = j * (ySpace-yGap);

                pArray[0][i] = new Point(origin.X - (i * (panelSize.Y + yGap) + rowCalcY) * angles.sin, origin.Y + (i * (panelSize.Y + yGap) + rowCalcY) * angles.cos);
                for (int k = 1; k < panelCountX; k++)
                {
                    Double rowCalcX = k / xRows;
                    int l = Convert.ToInt32(Math.Truncate(rowCalcX));
                    rowCalcX = l * (xSpace-xGap);
                    pArray[k][i] = new Point(pArray[0][i].X + (k * (panelSize.X + xGap) + rowCalcX) * angles.cos, pArray[0][i].Y + (k * (panelSize.X + xGap) + rowCalcX) * angles.sin);
                }
            }
           
            panelPoints = new PointCollection();
          //  MessageBox.Show("1");
            panelPoints = Test(pArray);
          //  MessageBox.Show("2");
            return panelPoints;



        }

        public DxfDocument startDXF()
        {
            loaded = new DxfDocument();
            return loaded;
        }
        public void saveDXF(string s)
        {
            loaded.Save(s);

        }

        public DxfDocument DrawPanels(DxfDocument dxf,PointCollection pc)
        {
            // System.Windows.Point p = new System.Windows.Point();
            // Point p = new Point;
            // PolylineVertex v = new PolylineVertex();
            //  Parallel.ForEach(pc, line =>
            // MessageBox.Show(scale.ToString());
            List<AciColor> cadColor = new List<AciColor>
            {
                new AciColor(Color.Yellow),
                new AciColor(Color.Blue),
                new AciColor(Color.Green),
                new AciColor(Color.Black)
            };

            foreach (Point p in pc)
            {

                Polyline pGon = new Polyline();
                pGon.Vertexes.Add(new PolylineVertex(p.X* scale, p.Y * scale, 0));
                pGon.Vertexes.Add(new PolylineVertex((p.X  + panelCorners[1].X) * scale, (p.Y  + panelCorners[1].Y) * scale, 0));
                pGon.Vertexes.Add(new PolylineVertex((p.X  + panelCorners[2].X) * scale, (p.Y + panelCorners[2].Y ) * scale, 0));
                pGon.Vertexes.Add(new PolylineVertex((p.X  + panelCorners[3].X) * scale, (p.Y + panelCorners[3].Y) * scale, 0));
                pGon.Vertexes.Add(new PolylineVertex((p.X) * scale, (p.Y) * scale, 0));
                Layer layer = new Layer(name);
                dxf.Layers.Add(layer);
                pGon.Layer = layer;
                pGon.Color = cadColor[panelColor];
                dxf.AddEntity(pGon);
                
                
            }
            return dxf;
        }

        public DxfDocument DrawBoundary(DxfDocument dxf)
        {
            Polyline pGon = new Polyline();
            pGon.Vertexes.Add(new PolylineVertex(boundary[0].X * scale, boundary[0].Y * scale, 0));
            for (int i = 1;i< boundary.Count; i++)
            {
                pGon.Vertexes.Add(new PolylineVertex(boundary[i].X * scale, boundary[i].Y * scale, 0));
   
            }
            pGon.Vertexes.Add(new PolylineVertex(boundary[0].X * scale, boundary[0].Y * scale, 0));
            Layer layer = new Layer("Panels");
            
              
            dxf.Layers.Add(layer);
            dxf.AddEntity(pGon);
            return dxf;
        }

        
        public PointCollection Test(Point[][] panelpoints)
        {
            Boolean valid = false;
            int y = panelCountX;
            int x = panelCountY;
            PointCollection pc = new PointCollection();
          //  Debug.Write("        k:" + y.ToString() + " j:" + x.ToString());
            for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < x; k++)
                    {
                 
                    //  MessageBox.Show(panelpoints[j][k].X.ToString() + " " + panelpoints[j][k].Y.ToString());
                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary,testPoints[0]);
                        if (valid == true)
                        {
                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[1]);
                            if (valid == true)
                            {
                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[2]);
                                if (valid == true)
                                {
                                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[3]);
                                    if (valid == true)
                                    {
                                        valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[4]);
                                        if (valid == true)
                                        {
                                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[5]);
                                            if (valid == true)
                                            {
                                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[6]);
                                                if (valid == true)
                                                {
                                                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[7]);
                                                    if (valid == true)
                                                    {
                                                        valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[8]);
                                                        if (valid == true)
                                                        {
                                                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[9]);
                                                            if (valid == true)
                                                            {
                                                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testPoints[10]);
                                                                if (valid == true)
                                                                {
                                                                //   pc.Add(panelpoints[j][k]);

                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[0]);
                                                                    if (valid == true)
                                                                    {
                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[1]);
                                                                        if (valid == true)
                                                                        {
                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[2]);
                                                                            if (valid == true)
                                                                            {
                                                                                valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[3]);
                                                                                if (valid == true)
                                                                                {
                                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[4]);
                                                                                    if (valid == true)
                                                                                    {
                                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[5]);
                                                                                        if (valid == true)
                                                                                        {
                                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[6]);
                                                                                            if (valid == true)
                                                                                            {
                                                                                                valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[7]);
                                                                                                if (valid == true)
                                                                                                {
                                                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[8]);
                                                                                                    if (valid == true)
                                                                                                    {
                                                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[9]);
                                                                                                        if (valid == true)
                                                                                                        {
                                                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testPoints[10]);
                                                                                                            if (valid == true)
                                                                                                            {
                                                                                                                  pc.Add(panelpoints[j][k]);
                                                                                                         }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
           // MessageBox.Show(pc.Count.ToString());
            return pc;
        }
    }
}
      
        
      