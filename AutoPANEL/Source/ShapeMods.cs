using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using WpfApp1.Source;

namespace WpfApp1.Source
{

    public class ShapeMods
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
        public static void changePlineThickessByID(Double weight, Canvas canvas, string ID)
        {

            IEnumerable<Polyline> query1 = canvas.Children.OfType<Polyline>();

            foreach (Polyline fruit in query1)
            {
                string str = fruit.Uid as string;
                if (str == ID)
                    fruit.StrokeThickness = weight;
            }

        }

        public static void changePlineColorByID(SolidColorBrush color, Canvas canvas, string ID)
        {

            IEnumerable<Polyline> query1 = canvas.Children.OfType<Polyline>();

            foreach (Polyline fruit in query1)
            {
                string str = fruit.Uid as string;
                if (str == ID)
                    fruit.Stroke = color;
            }

        }
        public static void changePgonColorByID(SolidColorBrush color, Canvas canvas, string ID)
        {

            IEnumerable<Polygon> query1 = canvas.Children.OfType<Polygon>();

            foreach (Polygon fruit in query1)
            {
                string str = fruit.Uid as string;
                if (str == ID)
                    fruit.Stroke = color;
            }

        }


        public static bool changePgonThickess(Double weight, Canvas canvas)
        {
            //        Dispatcher.BeginInvoke(
            //    new ThreadStart(() => EmployeesDataGrid.DataContext = employeesView));
            //        Polygon obj;
            //       List<Polygon> query1 = new List<Polygon>();// canvas.Children.OfType<Polygon>();
            ////       query1.AddRange(canvas.Children.OfType<Polygon>());


            //      Parallel.For(0, query1.Count() - 1, i =>
            //     {
            //      canvas.Dispatcher.BeginInvoke(
            //       new ThreadStart()


            /////        obj = query1[i];
            //    obj.StrokeThickness = weight;
            //    });
            //   });
          //  try
         //   {
  
                // Method signature: Parallel.ForEach(IEnumerable<TSource> source, Action<TSource> body)
             
        // try
         //   {
                IEnumerable<Polygon> query1 = canvas.Children.OfType<Polygon>();
                int i = 0;
                foreach (Polygon fruit in query1)
                {
                    i++;
                    fruit.StrokeThickness = weight;
                    if (i > 100)
                    {

                        DoEvents();
                        i = 0;
                    }
                }
            return false;
            }
          //  catch { }

       // }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
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
        public static void InsertPanel(Point p, PointCollection corners, SolidColorBrush color, Double thickness, Canvas canvas, String name,  Double scale, PVmodule pVmodule, double angle, int renderOption)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            
            if (renderOption > 0)
            {
                BitmapImage image = new BitmapImage();
                // Open image
                try
                {
                    image.BeginInit();
                }
                catch
                {

                }
                ScaleTransform st = new ScaleTransform();
                if (pVmodule.Name == ("GROUND MOUNT 14400W"))
                {
                    Stream myStream = myAssembly.GetManifestResourceStream("AutoPANEL.Resources.Ground Mount.png");
                    //  MessageBox.Show(myStream.Length.ToString());
                    image.StreamSource = myStream;
                    // image.UriSource = new Uri(@"Resources\test1.png");
                    // image.UriSource = new Uri(@"C:\Users\ajbol\Documents\test1.png");
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.EndInit();


                    st.ScaleX = pVmodule.PanelLength / image.Width / scale;
                    st.ScaleY = pVmodule.PanelWidth / image.Height / scale;
                }

                else
                {
                    if (pVmodule.PanelOrientation == true)
                    {
                        Stream myStream = myAssembly.GetManifestResourceStream("AutoPANEL.Resources.Panel Landscape.png");

                        image.StreamSource = myStream;
                        //  image.UriSource = new Uri(@"Resources\test1a.png");
                        // image.UriSource = new Uri(@"C:\Users\ajbol\Documents\test1a.png");
                        image.CacheOption = BitmapCacheOption.OnLoad;

                        image.EndInit(); ;


                        st.ScaleX = pVmodule.PanelLength / image.Width / scale;
                        st.ScaleY = pVmodule.PanelWidth / image.Height / scale;
                    }
                    else
                    {
                        Stream myStream = myAssembly.GetManifestResourceStream("AutoPANEL.Resources.Panel Portrait.png");
                        //  MessageBox.Show(myStream.Length.ToString());
                        image.StreamSource = myStream;
                        // image.UriSource = new Uri(@"Resources\test1.png");
                        // image.UriSource = new Uri(@"C:\Users\ajbol\Documents\test1.png");
                        image.CacheOption = BitmapCacheOption.OnLoad;

                        image.EndInit();


                        st.ScaleX = pVmodule.PanelLength / image.Width / scale;
                        st.ScaleY = pVmodule.PanelWidth / image.Height / scale;
                    }
                }
                TransformedBitmap tb = new TransformedBitmap();
                //Debug.WriteLine(image.Width.ToString());
                //Debug.WriteLine(scale.ToString());
                //st.ScaleX = Math.Abs(corners[1].X - corners[0].X) / image.Width;
                //st.ScaleY = Math.Abs(corners[3].Y - corners[0].Y) / image.Height;
                // BitmapSource objects like TransformedBitmap can only have their properties
                // changed within a BeginInit/EndInit block.
                tb.BeginInit();

                // Use the BitmapSource object defined above as the source for this BitmapSource.
                // This creates a "chain" of BitmapSource objects which essentially inherit from each other.
                tb.Source = image;

                // Flip the source 90 degrees.
                tb.Transform = st;
                tb.EndInit();

                Image im = new Image();
                im.Source = tb;
                Canvas.SetLeft(im, p.X);
                Canvas.SetTop(im, p.Y);
                TransformGroup tg = new TransformGroup();
                RotateTransform rt = new RotateTransform();
                SkewTransform sk = new SkewTransform();
                sk.AngleX = 5;
                // Debug.WriteLine( (corners[1].X - corners[0].X));

                //rt.Angle = Math.Atan((corners[1].Y - corners[0].Y) / (corners[1].X - corners[0].X)) / Math.PI * 180;
                rt.Angle = angle / Math.PI * 180;
                im.Uid = name;
                //tg.Children.Add(sk);
                tg.Children.Add(rt);
                im.RenderTransform = tg;
                canvas.Children.Add(im);
            }
            if (renderOption != 1)
            {
                IList<SolidColorBrush> colors = new List<SolidColorBrush>
            {
                 new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Black)
            };
                PointCollection points = new PointCollection();
                Polygon pg = new Polygon();
                for (int i = 0; i < 4; i++)
                {
                    // MessageBox.Show((corners[i].X + p.X).ToString() + " " + (corners[i].Y + p.Y).ToString());
                    points.Add(new Point(corners[i].X + p.X, corners[i].Y + p.Y));
                }
                pg.Points = points;

                pg.StrokeThickness = thickness;
                pg.Stroke = color;
                //MessageBox.Show(pg.Uid);
                pg.Uid = name;
            
            canvas.Children.Add(pg);
             }
            

               
           
        }

        public static void InsertPanel1(Point p, PointCollection corners, SolidColorBrush color, Double thickness, Canvas canvas, String name)
        {

            // if ((p.X != 0) && (p.Y != 0))
            // {
            //    Debug.WriteLine("Point x: " + p.X.ToString() + " Point y: " + p.Y.ToString());
            //   ShapeMods.InsertPanel(p, pt.testSettings.panelCorners, Color[pt.PanelColor], ms.lineScale, TheCanvas,pt.num.ToString());
            BitmapImage ObjBitmapImage = new BitmapImage();
            ObjBitmapImage.BeginInit();
            ObjBitmapImage.UriSource = new Uri(@"C:\\Users\\ajbol\\Documents\\test.png"); 
            ObjBitmapImage.EndInit();
            Image image = new Image();

            image.Source = ObjBitmapImage;
            IList<SolidColorBrush> colors = new List<SolidColorBrush>
            {
                 new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Black)
            };
            PointCollection points = new PointCollection();
            Polygon pg = new Polygon();
            for (int i = 0; i < 4; i++)
            {
                // MessageBox.Show((corners[i].X + p.X).ToString() + " " + (corners[i].Y + p.Y).ToString());
                points.Add(new Point(corners[i].X + p.X, corners[i].Y + p.Y));
            }
            pg.Points = points;

            pg.StrokeThickness = thickness;
            pg.Stroke = color;
            //MessageBox.Show(pg.Uid);
            pg.Uid = name;
            canvas.Children.Add(image);
            // }




        }

        public static Polyline PlineCreate(PointCollection points, SolidColorBrush solidColorBrush, Double thickness, Canvas canvas)
        {
            Polyline pLine = new Polyline();
         ////   Debug.WriteLine("/////// Drawing Outine ///////");

            IEnumerable<Point> query1 = points.OfType<Point>();
          //  Debug.WriteLine("Points in outline: " + query1.Count().ToString());
            foreach (Point p in query1)
            {
          //      Debug.WriteLine("Point: X-" + p.X.ToString() + " Y-" + p.Y.ToString());
                pLine.Points.Add(p);
            }
            pLine.StrokeThickness = thickness;
            pLine.Stroke = solidColorBrush;
            canvas.Children.Add(pLine);
            return pLine;
        //    Debug.WriteLine("///// Drawing Outine End /////");
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

            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Image image = new Image();
                image = canvas.Children[i] as Image;
                if (image != null)
                {
                    if (image.Uid == name)
                    {
                        canvas.Children.Remove(image);
                    }
                }
            }

        }

        public static void RemovePlines(Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Polyline polyline = new Polyline();
                polyline = canvas.Children[i] as Polyline;
                if (polyline != null)
                {

                        canvas.Children.Remove(polyline);
                    
                }
            }
        }
        public static void RemoveAllPgons(Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Polygon polygon = new Polygon();
                polygon = canvas.Children[i] as Polygon;
                if (polygon != null)
                {

                    canvas.Children.Remove(polygon);

                }
            }

            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Image image = new Image();
                image = canvas.Children[i] as Image;
                if (image != null)
                {
                    if (image.Uid != "")
                    {
                        Debug.WriteLine(image.Uid);
                          canvas.Children.Remove(image);
                        Debug.WriteLine("HELLO");
                    }
                }
            }
        }
            public static void RemovePlineByTag(Canvas canvas,string UId)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Polyline polyline = new Polyline();
                polyline = canvas.Children[i] as Polyline;
                if (polyline != null)
                {
                    string str = polyline.Uid as string; 
                    if(str==UId)
                    canvas.Children.Remove(polyline);

                }
            }
        }

        public static void pLineVisibility(Canvas canvas, Visibility v)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                Polyline polyline = new Polyline();
                polyline = canvas.Children[i] as Polyline;
                if (polyline != null)
                {
                    polyline.Visibility = v;
                }
            }
        }
        public static void removeZeroPoints(PointCollection pc)
        {
            for (int i = pc.Count - 1; i >= 0; i--)
            {
                if((pc[i].X== 0)&& (pc[i].Y== 0))
                {
                    pc.Remove(pc[i]);
         
                }
           
            }
        }
        [Serializable] public class ShapeColor
        {
            public SolidColorBrush color =new SolidColorBrush(Colors.Blue);
            public string colorName = "";

            public ShapeColor()
            {

            }
            public override string ToString()
            {
                return colorName;
            }
        }
        [Serializable]
        public class PanelList
        {
            public PointCollection points;

            public PanelList()
            {
                Point p = new Point(0, 0);
                PointCollection pc = new PointCollection();
                pc.Add(p);
                points = pc;
            }
            public override string ToString()
            {
                return points.Count.ToString();
            }
        }

    }
}

