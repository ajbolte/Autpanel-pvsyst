using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1.Source
{

    class ShapeMods
    {
        public static void changeLineThickess(Double weight, Canvas canvas)
        {
            IEnumerable<Line> query1 = canvas.Children.OfType<Line>();

            foreach (Line fruit in query1)
            {
                fruit.StrokeThickness = weight;
            }

        }

        public static void changePlineThickess(Double weight, Canvas canvas)
        {

            IEnumerable<Polyline> query1 = canvas.Children.OfType<Polyline>();

            foreach (Polyline fruit in query1)
            {
                fruit.StrokeThickness = weight;
            }

        }

        public static void changePgonThickess(Double weight, Canvas canvas)
        {

            IEnumerable<Polygon> query1 = canvas.Children.OfType<Polygon>();

            foreach (Polygon fruit in query1)
            {
                fruit.StrokeThickness = weight;
            }

        }

        public static void resetScale(ScrollViewer scrollView)
        {
            // if (scrollView.ZoomFactor != 1)
            //{
            //      scrollView.ZoomToFactor(1);
            //  }
        }


        public static Line lineStart(Double x, Double y, SolidColorBrush solidColorBrush, Double thickness, Canvas canvas)
        {
            Line line = new Line
            {
                X1 = x,
                Y1 = (y),
                X2 = x,
                Y2 = (y),
                StrokeThickness = thickness,
                Stroke = solidColorBrush
            };
            canvas.Children.Add(line);
            return line;
        }
        public static void InsertPanel(Point p,PointCollection corners, Int32 colorIndex, Double thickness, Canvas canvas,String name)
        {
            IList<SolidColorBrush> colors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Black)
            };
            PointCollection points = new PointCollection();
            Polygon pg = new Polygon();
            for (int i = 0; i < 4; i++) {
               // MessageBox.Show((corners[i].X + p.X).ToString() + " " + (corners[i].Y + p.Y).ToString());
                points.Add(new Point(corners[i].X + p.X, corners[i].Y + p.Y));
            }
            pg.Points=points;

            pg.StrokeThickness = thickness;
            pg.Stroke = colors[colorIndex];
            //MessageBox.Show(pg.Uid);
            pg.Uid = name;
            canvas.Children.Add(pg);
        
        }

        public static Polyline PlineCreate(PointCollection points, SolidColorBrush solidColorBrush, Double thickness, Canvas canvas)
        {
            Polyline pLine = new Polyline();
            IEnumerable<Point> query1 = points.OfType<Point>();
            foreach (Point p in query1)
            {
                pLine.Points.Add(p);
            }
            pLine.StrokeThickness = thickness;
            pLine.Stroke = solidColorBrush;
            canvas.Children.Add(pLine);
            return pLine;
        }

        public static Boolean get_line_intersection(Point p, PointCollection pc, Point offset)
        {
            Point p1 = new Point(50000, 50000);
            Point p0 = new Point(p.X + offset.X, p.Y + offset.Y);
            int count = 0;
            Double[] pY = new Double[pc.Count];
            Double[] pX = new Double[pc.Count];
            for (int z = 0; z < pc.Count; z++)
            {
                pY[z] = pc[z].Y;
                pX[z] = pc[z].X;
            }

            Parallel.For(0, pc.Count - 1, i =>
            {
                PointCollection panels = new PointCollection();
                panels = pc;
                Double p2_y = pY[i];
                Double p2_x = pX[i];
                Double p3_y = pY[i + 1];
                Double p3_x = pX[i + 1];



                Double s1_x, s1_y, s2_x, s2_y;
                s1_x = p1.X - p0.X; s1_y = p1.Y - p0.Y;
                s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

                Double s, t;
                s = (-s1_y * (p0.X - p2_x) + s1_x * (p0.Y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
                t = (s2_x * (p0.Y - p2_y) - s2_y * (p0.X - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

                if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
                {
                    count++;

                }

            });
            if (count % 2 == 0)
            {
                return false;
            }
            else
            {

                return true;
            }
        }


        public static Boolean get_line_shading(Point p, List<PointCollection> pc1, Point offset)
        {

            if (pc1.Count() < 1)
            {
                return true;
            }
            foreach (PointCollection pc in pc1)// need null protection
            {
                Point p1 = new Point(50000, 50000);
                Point p0 = new Point(p.X + offset.X, p.Y + offset.Y);
                int count = 0;
                if (pc == null)
                {
                    return true;
                }
                Double[] pY = new Double[pc.Count];
                Double[] pX = new Double[pc.Count];
                for (int z = 0; z < pc.Count; z++)
                {
                    pY[z] = pc[z].Y;
                    pX[z] = pc[z].X;
                }

                Parallel.For(0, pc.Count - 1, i =>
                {
                    PointCollection panels = new PointCollection();
                    panels = pc;
                    Double p2_y = pY[i];
                    Double p2_x = pX[i];
                    Double p3_y = pY[i + 1];
                    Double p3_x = pX[i + 1];



                    Double s1_x, s1_y, s2_x, s2_y;
                    s1_x = p1.X - p0.X; s1_y = p1.Y - p0.Y;
                    s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

                    Double s, t;
                    s = (-s1_y * (p0.X - p2_x) + s1_x * (p0.Y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
                    t = (s2_x * (p0.Y - p2_y) - s2_y * (p0.X - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

                    if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
                    {
                        count++;

                    }

                });
                if (count % 2 != 0)
                {
                    return false;
                }
            }
                return true;
            
        }

        public static void RemoveLines(Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Line line = new Line();
                line = canvas.Children[i] as Line;
                if (line != null)
                {
                    canvas.Children.Remove(line);
                }
            }
        }
        public static void RemovePgons(Canvas canvas,String name)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Polygon polygon = new Polygon();
                polygon = canvas.Children[i] as Polygon;
                if (polygon != null)
                {
                    if (polygon.Uid == name)
                    {
                        canvas.Children.Remove(polygon);
                    }
                }
            }
        }





    }
}
