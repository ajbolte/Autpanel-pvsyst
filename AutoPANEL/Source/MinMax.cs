// Decompiled with JetBrains decompiler
// Type: AutoPANEL.Source.MinMax
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Windows;
using System.Windows.Media;
using WpfApp1.Source;

namespace AutoPANEL.Source
{
  public class MinMax
  {
    public int panelsLeft = 0;
    public int panelsRight = 0;
    public int panelsUp = 0;
    public int panelsDown = 0;
    public Point xMin;
    public Point xMax;
    public Point yMin;
    public Point yMax;
    private Point beforeOrigin;
    private Point afterOrigin;

    public MinMax()
    {
    }

    public MinMax(PointCollection Correctedboundary, PVmodule pVmodule, double scaleMM)
    {
      this.xMax = MinMax.Xmaximum(Correctedboundary);
      this.xMin = MinMax.Xminimum(Correctedboundary);
      this.yMax = MinMax.Ymaximum(Correctedboundary);
      this.yMin = MinMax.Yminimum(Correctedboundary);
      this.beforeOrigin = new Point(0.0 - this.xMin.X, 0.0 - this.yMin.Y);
      if (this.beforeOrigin.X < 0.0)
        this.beforeOrigin.X = 0.0;
      if (this.beforeOrigin.Y < 0.0)
        this.beforeOrigin.Y = 0.0;
      this.afterOrigin = new Point(this.xMax.X, this.yMax.Y);
      if (this.afterOrigin.X < 0.0)
        this.afterOrigin.X = 0.0;
      if (this.afterOrigin.Y < 0.0)
        this.afterOrigin.Y = 0.0;
      this.panelsLeft = Convert.ToInt32(Math.Ceiling(this.beforeOrigin.X * scaleMM / pVmodule.PanelLength));
      this.panelsRight = Convert.ToInt32(Math.Ceiling(this.afterOrigin.X * scaleMM / pVmodule.PanelLength));
      this.panelsDown = Convert.ToInt32(Math.Ceiling(this.beforeOrigin.Y * scaleMM / pVmodule.PanelWidth));
      this.panelsUp = Convert.ToInt32(Math.Ceiling(this.afterOrigin.Y * scaleMM / pVmodule.PanelWidth));
    }

    public static Point Xminimum(PointCollection pc)
    {
      Point point1;
      ref Point local = ref point1;
      Point point2 = pc[0];
      double x = point2.X;
      point2 = pc[0];
      double y = point2.Y;
      local = new Point(x, y);
      foreach (Point point3 in pc)
      {
        if (point3.X < point1.X)
        {
          point1.X = point3.X;
          point1.Y = point3.Y;
        }
      }
      return point1;
    }

    public static Point Yminimum(PointCollection pc)
    {
      Point point1;
      ref Point local = ref point1;
      Point point2 = pc[0];
      double x = point2.X;
      point2 = pc[0];
      double y = point2.Y;
      local = new Point(x, y);
      foreach (Point point3 in pc)
      {
        if (point3.Y < point1.Y)
        {
          point1.X = point3.X;
          point1.Y = point3.Y;
        }
      }
      return point1;
    }

    public static Point Xmaximum(PointCollection pc)
    {
      Point point1;
      ref Point local = ref point1;
      Point point2 = pc[0];
      double x = point2.X;
      point2 = pc[0];
      double y = point2.Y;
      local = new Point(x, y);
      foreach (Point point3 in pc)
      {
        if (point3.X > point1.X)
        {
          point1.X = point3.X;
          point1.Y = point3.Y;
        }
      }
      return point1;
    }

    public static Point Ymaximum(PointCollection pc)
    {
      Point point1;
      ref Point local = ref point1;
      Point point2 = pc[0];
      double x = point2.X;
      point2 = pc[0];
      double y = point2.Y;
      local = new Point(x, y);
      foreach (Point point3 in pc)
      {
        if (point3.Y > point1.Y)
        {
          point1.X = point3.X;
          point1.Y = point3.Y;
        }
      }
      return point1;
    }
  }
}
