// Decompiled with JetBrains decompiler
// Type: AutoPANEL.Source.TestMethods
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfApp1.Source;

namespace AutoPANEL.Source
{
  internal class TestMethods
  {
    public static void TestBoundary(
      PanelPointSet pps,
      PointCollection Correctedboundary,
      List<PointCollection> shading,
      PointCollection testPoints)
    {
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) pps.set)
      {
        if (TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[0]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[1]) && (TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[2]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[3])) && (TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[4]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[5]) && (TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[6]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[7]))) && (TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[8]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[9]) && TestMethods.get_line_intersection(panelPoint.point, Correctedboundary, testPoints[10])))
        {
          if (shading.Count > 0)
          {
            if (TestMethods.get_line_shading(panelPoint.point, shading, testPoints[0]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[1]) && (TestMethods.get_line_shading(panelPoint.point, shading, testPoints[2]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[3])) && (TestMethods.get_line_shading(panelPoint.point, shading, testPoints[4]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[5]) && (TestMethods.get_line_shading(panelPoint.point, shading, testPoints[6]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[7]))) && (TestMethods.get_line_shading(panelPoint.point, shading, testPoints[8]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[9]) && TestMethods.get_line_shading(panelPoint.point, shading, testPoints[10])))
              panelPoint.active = true;
          }
          else
            panelPoint.active = true;
        }
      }
    }

    public static bool get_line_intersection(Point p, PointCollection pc, Point offset)
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

    public static bool get_line_shading(Point p, List<PointCollection> pc1, Point offset)
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
  }
}
