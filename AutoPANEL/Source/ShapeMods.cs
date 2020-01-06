// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.ShapeMods
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1.Source
{
  public class ShapeMods
  {
    public static void changeLineThickess(double weight, Canvas canvas)
    {
      foreach (Shape shape in canvas.Children.OfType<Line>())
        shape.StrokeThickness = weight;
    }

    public static void changePlineThickess(double weight, Canvas canvas)
    {
      foreach (Shape shape in canvas.Children.OfType<Polyline>())
        shape.StrokeThickness = weight;
    }

    public static void changePlineThickessByID(double weight, Canvas canvas, string ID)
    {
      foreach (Polyline polyline in canvas.Children.OfType<Polyline>())
      {
        if (polyline.Uid == ID)
          polyline.StrokeThickness = weight;
      }
    }

    public static void changePlineColorByID(SolidColorBrush color, Canvas canvas, string ID)
    {
      foreach (Polyline polyline in canvas.Children.OfType<Polyline>())
      {
        if (polyline.Uid == ID)
          polyline.Stroke = (Brush) color;
      }
    }

    public static void changePgonColorByID(SolidColorBrush color, Canvas canvas, string ID)
    {
      foreach (Polygon polygon in canvas.Children.OfType<Polygon>())
      {
        if (polygon.Uid == ID)
          polygon.Stroke = (Brush) color;
      }
    }

    public static bool changePgonThickess(double weight, Canvas canvas)
    {
      IEnumerable<Polygon> polygons = canvas.Children.OfType<Polygon>();
      int num = 0;
      foreach (Polygon polygon in polygons)
      {
        ++num;
        polygon.StrokeThickness = weight;
        if (num > 100)
        {
          ShapeMods.DoEvents();
          num = 0;
        }
      }
      return false;
    }

    public static void DoEvents()
    {
   Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

    public static void resetScale(ScrollViewer scrollView)
    {
    }

    public static Line lineStart(
      double x,
      double y,
      SolidColorBrush solidColorBrush,
      double thickness,
      Canvas canvas)
    {
      Line line1 = new Line();
      line1.X1 = x;
      line1.Y1 = y;
      line1.X2 = x;
      line1.Y2 = y;
      line1.StrokeThickness = thickness;
      line1.Stroke = (Brush) solidColorBrush;
      Line line2 = line1;
      canvas.Children.Add((UIElement) line2);
      return line2;
    }

    public static void InsertPanel(
      Point p,
      PointCollection corners,
      SolidColorBrush color,
      double thickness,
      Canvas canvas,
      string name,
      double scale,
      PVmodule pVmodule,
      double angle,
      int renderOption)
    {
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      if (renderOption > 0)
      {
        BitmapImage bitmapImage = new BitmapImage();
        try
        {
          bitmapImage.BeginInit();
        }
        catch
        {
        }
        ScaleTransform scaleTransform = new ScaleTransform();
        if (pVmodule.Name == "GROUND MOUNT 14400W")
        {
          Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("AutoPANEL.Resources.Ground Mount.png");
          bitmapImage.StreamSource = manifestResourceStream;
          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
          bitmapImage.EndInit();
          scaleTransform.ScaleX = pVmodule.PanelLength / bitmapImage.Width / scale;
          scaleTransform.ScaleY = pVmodule.PanelWidth / bitmapImage.Height / scale;
        }
        else if (pVmodule.PanelOrientation)
        {
          Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("AutoPANEL.Resources.Panel Landscape.png");
          bitmapImage.StreamSource = manifestResourceStream;
          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
          bitmapImage.EndInit();
          scaleTransform.ScaleX = pVmodule.PanelLength / bitmapImage.Width / scale;
          scaleTransform.ScaleY = pVmodule.PanelWidth / bitmapImage.Height / scale;
        }
        else
        {
          Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("AutoPANEL.Resources.Panel Portrait.png");
          bitmapImage.StreamSource = manifestResourceStream;
          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
          bitmapImage.EndInit();
          scaleTransform.ScaleX = pVmodule.PanelLength / bitmapImage.Width / scale;
          scaleTransform.ScaleY = pVmodule.PanelWidth / bitmapImage.Height / scale;
        }
        TransformedBitmap transformedBitmap = new TransformedBitmap();
        transformedBitmap.BeginInit();
        transformedBitmap.Source = (BitmapSource) bitmapImage;
        transformedBitmap.Transform = (Transform) scaleTransform;
        transformedBitmap.EndInit();
        Image image = new Image();
        image.Source = (ImageSource) transformedBitmap;
        Canvas.SetLeft((UIElement) image, p.X);
        Canvas.SetTop((UIElement) image, p.Y);
        TransformGroup transformGroup = new TransformGroup();
        RotateTransform rotateTransform = new RotateTransform();
        new SkewTransform().AngleX = 5.0;
        rotateTransform.Angle = angle / Math.PI * 180.0;
        image.Uid = name;
        transformGroup.Children.Add((Transform) rotateTransform);
        image.RenderTransform = (Transform) transformGroup;
        canvas.Children.Add((UIElement) image);
      }
      if (renderOption == 1)
        return;
      List<SolidColorBrush> solidColorBrushList = new List<SolidColorBrush>()
      {
        new SolidColorBrush(Colors.Blue),
        new SolidColorBrush(Colors.Yellow),
        new SolidColorBrush(Colors.Green),
        new SolidColorBrush(Colors.Black)
      };
      PointCollection pointCollection1 = new PointCollection();
      Polygon polygon = new Polygon();
      for (int index = 0; index < 4; ++index)
      {
        PointCollection pointCollection2 = pointCollection1;
        Point corner = corners[index];
        double x = corner.X + p.X;
        corner = corners[index];
        double y = corner.Y + p.Y;
        Point point = new Point(x, y);
        pointCollection2.Add(point);
      }
      polygon.Points = pointCollection1;
      polygon.StrokeThickness = thickness;
      polygon.Stroke = (Brush) color;
      polygon.Uid = name;
      canvas.Children.Add((UIElement) polygon);
    }

    public static void InsertPanelA(PanelTest pt, Canvas canvas)
    {
      Assembly.GetExecutingAssembly();
      IList<SolidColorBrush> solidColorBrushList = (IList<SolidColorBrush>) new List<SolidColorBrush>()
      {
        new SolidColorBrush(Colors.Blue),
        new SolidColorBrush(Colors.Yellow),
        new SolidColorBrush(Colors.Green),
        new SolidColorBrush(Colors.Black)
      };
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) pt.PanelLayoutPoints[pt.SelectedIndex].set)
      {
       
        Point point2 = ShapeMods.RotatePoint(panelPoint.point, pt.azumith);
        if (panelPoint.active)
        {
          PointCollection pointCollection1 = new PointCollection();
          Polygon polygon = new Polygon();
          for (int index = 0; index < 4; ++index)
          {
            PointCollection pointCollection2 = pointCollection1;
            double x1 = pt.origin.X;
            Point panelCorner = pt.testPoints.panelCorners[index];
            double x2 = panelCorner.X;
            double x3 = x1 + x2 + point2.X;
            double y1 = pt.origin.Y;
            panelCorner = pt.testPoints.panelCorners[index];
            double y2 = panelCorner.Y;
            double y3 = y1 + y2 + point2.Y;
            Point point3 = new Point(x3, y3);
            pointCollection2.Add(point3);
          }
          polygon.Points = pointCollection1;
          polygon.StrokeThickness = 1.0;
          polygon.Stroke = (Brush) solidColorBrushList[0];
          polygon.Uid = pt.num.ToString();
          canvas.Children.Add((UIElement) polygon);
        }
      }
    }

    public static void InvertPanelA(PanelTest pt, Point point, double scale)
    {
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) pt.PanelLayoutPoints[pt.SelectedIndex].set)
      {
        Point point1 = new Point();
        point1 = ShapeMods.RotatePoint(panelPoint.point, pt.azumith);
        PointCollection pc = new PointCollection();
        Point panelCorner;
        for (int index = 0; index < 4; ++index)
        {
          PointCollection pointCollection = pc;
          double x1 = pt.origin.X;
          panelCorner = pt.testPoints.panelCorners[index];
          double x2 = panelCorner.X;
          double x3 = x1 + x2 + point1.X;
          double y1 = pt.origin.Y;
          panelCorner = pt.testPoints.panelCorners[index];
          double y2 = panelCorner.Y;
          double y3 = y1 + y2 + point1.Y;
          Point point2 = new Point(x3, y3);
          pointCollection.Add(point2);
        }
        PointCollection pointCollection1 = pc;
        double x4 = pt.origin.X;
        panelCorner = pt.testPoints.panelCorners[0];
        double x5 = panelCorner.X;
        double x6 = x4 + x5 + point1.X;
        double y4 = pt.origin.Y;
        panelCorner = pt.testPoints.panelCorners[0];
        double y5 = panelCorner.Y;
        double y6 = y4 + y5 + point1.Y;
        Point point3 = new Point(x6, y6);
        pointCollection1.Add(point3);
        if (ShapeMods.get_line_intersectionB(point, pc))
        {
          double num = point.X;
          string str1 = num.ToString();
          num = point.Y;
          string str2 = num.ToString();
          Debug.WriteLine("Point x: " + str1 + " Point y: " + str2);
          for (int index = 0; index < 4; ++index)
          {
            string[] strArray = new string[6]
            {
              "Corners ",
              index.ToString(),
              " : ",
              null,
              null,
              null
            };
            double x1 = pt.origin.X;
            panelCorner = pt.testPoints.panelCorners[index];
            double x2 = panelCorner.X;
            num = x1 + x2 + point1.X - point.X;
            strArray[3] = num.ToString();
            strArray[4] = " ";
            double y1 = pt.origin.Y;
            panelCorner = pt.testPoints.panelCorners[index];
            double y2 = panelCorner.Y;
            num = y1 + y2 + point1.Y - point.Y;
            strArray[5] = num.ToString();
            Debug.WriteLine(string.Concat(strArray));
          }
          panelPoint.active = !panelPoint.active;
        }
      }
    }

    private static Point RotatePoint(Point pointToRotate, double angleInRadians)
    {
      double num1 = Math.Cos(angleInRadians);
      double num2 = Math.Sin(angleInRadians);
      return new Point()
      {
        X = num1 * pointToRotate.X - num2 * pointToRotate.Y,
        Y = num2 * pointToRotate.X + num1 * pointToRotate.Y
      };
    }

    public static void InsertPanel1(
      Point p,
      PointCollection corners,
      SolidColorBrush color,
      double thickness,
      Canvas canvas,
      string name)
    {
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      bitmapImage.UriSource = new Uri("C:\\\\Users\\\\ajbol\\\\Documents\\\\test.png");
      bitmapImage.EndInit();
      Image image = new Image();
      image.Source = (ImageSource) bitmapImage;
      List<SolidColorBrush> solidColorBrushList = new List<SolidColorBrush>()
      {
        new SolidColorBrush(Colors.Blue),
        new SolidColorBrush(Colors.Yellow),
        new SolidColorBrush(Colors.Green),
        new SolidColorBrush(Colors.Black)
      };
      PointCollection pointCollection1 = new PointCollection();
      Polygon polygon = new Polygon();
      for (int index = 0; index < 4; ++index)
      {
        PointCollection pointCollection2 = pointCollection1;
        Point corner = corners[index];
        double x = corner.X + p.X;
        corner = corners[index];
        double y = corner.Y + p.Y;
        Point point = new Point(x, y);
        pointCollection2.Add(point);
      }
      polygon.Points = pointCollection1;
      polygon.StrokeThickness = thickness;
      polygon.Stroke = (Brush) color;
      polygon.Uid = name;
      canvas.Children.Add((UIElement) image);
    }

    public static Polyline PlineCreate(
      PointCollection points,
      SolidColorBrush solidColorBrush,
      double thickness,
      Canvas canvas)
    {
      Polyline polyline = new Polyline();
      foreach (Point point in points.OfType<Point>())
        polyline.Points.Add(point);
      polyline.StrokeThickness = thickness;
      polyline.Stroke = (Brush) solidColorBrush;
      canvas.Children.Add((UIElement) polyline);
      return polyline;
    }

    public static bool get_line_intersectionA(Point p, PointCollection pc, Point offset)
    {
      Point p1 = new Point(50000.0, 50000.0);
      Point p0 = new Point(p.X + offset.X, p.Y + offset.Y);
      int count = 0;
      double[] pY = new double[pc.Count];
      double[] pX = new double[pc.Count];
      for (int index = 0; index < pc.Count; ++index)
      {
        pY[index] = pc[index].Y;
        pX[index] = pc[index].X;
      }
      Parallel.For(0, pc.Count - 1, (Action<int>) (i =>
      {
        PointCollection pointCollection = new PointCollection();
        pointCollection = pc;
        double num1 = pY[i];
        double num2 = pX[i];
        double num3 = pY[i + 1];
        double num4 = pX[i + 1];
        double num5 = p1.X - p0.X;
        double num6 = p1.Y - p0.Y;
        double num7 = num4 - num2;
        double num8 = num3 - num1;
        double num9 = (-num6 * (p0.X - num2) + num5 * (p0.Y - num1)) / (-num7 * num6 + num5 * num8);
        double num10 = (num7 * (p0.Y - num1) - num8 * (p0.X - num2)) / (-num7 * num6 + num5 * num8);
        if (num9 < 0.0 || num9 > 1.0 || num10 < 0.0 || num10 > 1.0)
          return;
        ++count;
      }));
      return count % 2 != 0;
    }

    public static bool get_line_intersectionB(Point p, PointCollection pc)
    {
      Point p1 = new Point(50000.0, 50000.0);
      Point p0 = new Point(p.X, p.Y);
      int count = 0;
      double[] pY = new double[pc.Count];
      double[] pX = new double[pc.Count];
      for (int index1 = 0; index1 < pc.Count; ++index1)
      {
        double[] numArray1 = pY;
        int index2 = index1;
        Point point = pc[index1];
        double y = point.Y;
        numArray1[index2] = y;
        double[] numArray2 = pX;
        int index3 = index1;
        point = pc[index1];
        double x = point.X;
        numArray2[index3] = x;
      }
      Parallel.For(0, pc.Count - 1, (Action<int>) (i =>
      {
        PointCollection pointCollection = new PointCollection();
        pointCollection = pc;
        double num1 = pY[i];
        double num2 = pX[i];
        double num3 = pY[i + 1];
        double num4 = pX[i + 1];
        double num5 = p1.X - p0.X;
        double num6 = p1.Y - p0.Y;
        double num7 = num4 - num2;
        double num8 = num3 - num1;
        double num9 = (-num6 * (p0.X - num2) + num5 * (p0.Y - num1)) / (-num7 * num6 + num5 * num8);
        double num10 = (num7 * (p0.Y - num1) - num8 * (p0.X - num2)) / (-num7 * num6 + num5 * num8);
        if (num9 < 0.0 || num9 > 1.0 || num10 < 0.0 || num10 > 1.0)
          return;
        ++count;
      }));
      if (count % 2 == 0)
        return false;
      Debug.WriteLine("intersections: " + count.ToString());
      return true;
    }

    public static bool get_line_shadingA(Point p, List<PointCollection> pc1, Point offset)
    {
      if (pc1.Count<PointCollection>() < 1)
        return true;
      foreach (PointCollection pointCollection1 in pc1)
      {
        PointCollection pc = pointCollection1;
        Point p1 = new Point(50000.0, 50000.0);
        Point p0 = new Point(p.X + offset.X, p.Y + offset.Y);
        int count = 0;
        if (pc == null)
          return true;
        double[] pY = new double[pc.Count];
        double[] pX = new double[pc.Count];
        for (int index1 = 0; index1 < pc.Count; ++index1)
        {
          double[] numArray1 = pY;
          int index2 = index1;
          Point point = pc[index1];
          double y = point.Y;
          numArray1[index2] = y;
          double[] numArray2 = pX;
          int index3 = index1;
          point = pc[index1];
          double x = point.X;
          numArray2[index3] = x;
        }
        Parallel.For(0, pc.Count - 1, (Action<int>) (i =>
        {
          PointCollection pointCollection = new PointCollection();
          pointCollection = pc;
          double num1 = pY[i];
          double num2 = pX[i];
          double num3 = pY[i + 1];
          double num4 = pX[i + 1];
          double num5 = p1.X - p0.X;
          double num6 = p1.Y - p0.Y;
          double num7 = num4 - num2;
          double num8 = num3 - num1;
          double num9 = (-num6 * (p0.X - num2) + num5 * (p0.Y - num1)) / (-num7 * num6 + num5 * num8);
          double num10 = (num7 * (p0.Y - num1) - num8 * (p0.X - num2)) / (-num7 * num6 + num5 * num8);
          if (num9 < 0.0 || num9 > 1.0 || num10 < 0.0 || num10 > 1.0)
            return;
          ++count;
        }));
        if ((uint) (count % 2) > 0U)
          return false;
      }
      return true;
    }

    public static void RemoveLines(Canvas canvas)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Line line = new Line();
        if (canvas.Children[index] is Line child)
          canvas.Children.Remove((UIElement) child);
      }
    }

    public static void RemovePgons(Canvas canvas, string name)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Polygon polygon = new Polygon();
        if (canvas.Children[index] is Polygon child && child.Uid == name)
          canvas.Children.Remove((UIElement) child);
      }
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Image image = new Image();
        if (canvas.Children[index] is Image child && child.Uid == name)
          canvas.Children.Remove((UIElement) child);
      }
    }

    public static void RemovePlines(Canvas canvas)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Polyline polyline = new Polyline();
        if (canvas.Children[index] is Polyline child)
          canvas.Children.Remove((UIElement) child);
      }
    }

    public static void RemoveAllPgons(Canvas canvas)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Polygon polygon = new Polygon();
        if (canvas.Children[index] is Polygon child)
          canvas.Children.Remove((UIElement) child);
      }
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Image image = new Image();
        if (canvas.Children[index] is Image child && child.Uid != "")
        {
          Debug.WriteLine(child.Uid);
          canvas.Children.Remove((UIElement) child);
          Debug.WriteLine("HELLO");
        }
      }
    }

    public static void RemovePlineByTag(Canvas canvas, string UId)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Polyline polyline = new Polyline();
        if (canvas.Children[index] is Polyline child && child.Uid == UId)
          canvas.Children.Remove((UIElement) child);
      }
    }

    public static void pLineVisibility(Canvas canvas, Visibility v)
    {
      for (int index = canvas.Children.Count - 1; index >= 0; --index)
      {
        Polyline polyline = new Polyline();
        if (canvas.Children[index] is Polyline child)
          child.Visibility = v;
      }
    }

    public static void removeZeroPoints(PointCollection pc)
    {
      for (int index = pc.Count - 1; index >= 0; --index)
      {
        Point point = pc[index];
        int num;
        if (point.X == 0.0)
        {
          point = pc[index];
          num = point.Y == 0.0 ? 1 : 0;
        }
        else
          num = 0;
        if (num != 0)
          pc.Remove(pc[index]);
      }
    }

    [Serializable]
    public class ShapeColor
    {
      public SolidColorBrush color = new SolidColorBrush(Colors.Blue);
      public string colorName = "";

      public override string ToString()
      {
        return this.colorName;
      }
    }

    [Serializable]
    public class PanelList
    {
      public PointCollection points;

      public PanelList()
      {
        this.points = new PointCollection()
        {
          new Point(0.0, 0.0)
        };
      }

      public override string ToString()
      {
        return this.points.Count.ToString();
      }
    }
  }
}
