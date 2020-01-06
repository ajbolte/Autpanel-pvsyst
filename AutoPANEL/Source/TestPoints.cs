// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.TestPoints
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1.Source
{
  [Serializable]
  public class TestPoints
  {
    public PointCollection panelCorners = new PointCollection();
    public PointCollection testPoints = new PointCollection();

    public void Modify(double scale, PVmodule PV, double Azumith)
    {
      this.CreatePanelTestPoints(PV, scale, Azumith);
    }

    public void CreatePanelTestPoints(PVmodule pv, double scale, double azumith)
    {
      this.panelCorners = new PointCollection();
      this.testPoints = new PointCollection();
      Point point = new Point(0.0, 0.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale, 0.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale, pv.PanelWidth / scale);
      this.testPoints.Add(point);
      point = new Point(0.0, pv.PanelWidth / scale);
      this.testPoints.Add(point);
      this.panelCorners = this.RotatePointCollection(this.testPoints, azumith);
      point = new Point(pv.PanelLength / scale / 3.0, 0.0);
      this.testPoints.Add(point);
      point = new Point(2.0 * pv.PanelLength / scale / 3.0, 0.0);
      this.testPoints.Add(point);
      point = new Point(0.0, pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale / 3.0, pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(2.0 * pv.PanelLength / scale / 3.0, pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale, pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(0.0, 2.0 * pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale / 3.0, 2.0 * pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(2.0 * pv.PanelLength / scale / 3.0, 2.0 * pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale, 2.0 * pv.PanelWidth / scale / 3.0);
      this.testPoints.Add(point);
      point = new Point(pv.PanelLength / scale / 3.0, pv.PanelWidth / scale);
      this.testPoints.Add(point);
      point = new Point(2.0 * pv.PanelLength / scale / 3.0, pv.PanelWidth / scale);
      this.testPoints.Add(point);
    }

    private PointCollection RotatePointCollection(
      PointCollection pointCollectionToRotate,
      double angleInRadians)
    {
      PointCollection pointCollection = new PointCollection();
      foreach (Point point1 in pointCollectionToRotate)
      {
        double num1 = Math.Cos(angleInRadians);
        double num2 = Math.Sin(angleInRadians);
        Point point2 = new Point()
        {
          X = num1 * point1.X - num2 * point1.Y,
          Y = num2 * point1.X + num1 * point1.Y
        };
        pointCollection.Add(point2);
      }
      return pointCollection;
    }

    private void DebugPrintCorners(PVmodule pv)
    {
      Debug.WriteLine("+++++++ Panel Corners +++++++");
      double num1 = pv.PanelLength;
      string str1 = num1.ToString();
      num1 = pv.PanelWidth;
      string str2 = num1.ToString();
      Debug.WriteLine("Panel length: " + str1 + " Panel width: " + str2);
      foreach (Point panelCorner in this.panelCorners)
      {
        double num2 = panelCorner.X;
        string str3 = num2.ToString();
        num2 = panelCorner.Y;
        string str4 = num2.ToString();
        Debug.WriteLine("Point x: " + str3 + " y: " + str4);
      }
      Debug.WriteLine("+++++++++++++++++++++++++++++");
    }
  }
}
