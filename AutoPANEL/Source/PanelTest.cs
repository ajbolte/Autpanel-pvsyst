// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.PanelTest
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using AutoPANEL;
using AutoPANEL.Source;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApp1.Source
{
  [Serializable]
  public class PanelTest
  {
    public double azumith = 0.0;
    public bool? SpacingCheck = new bool?(false);
    public PointCollection Correctedboundary = new PointCollection();
    public PointCollection boundary = new PointCollection();
    public int stepAcross = 300;
    public int stepDown = 300;
    public ObservableCollection<PanelPointSet> PanelLayoutPoints = new ObservableCollection<PanelPointSet>();
    public int num = 0;
    public int SelectedIndex = 0;
    private double scaleMM = 1000.0;
    public int PanelColor = 0;
    public int OutlineColor = 1;
    public int ExclusionColor = 2;
    public List<PointCollection> shading = new List<PointCollection>();
    public List<PointCollection> Correctedshading = new List<PointCollection>();
    public PointCollection outline = new PointCollection();
    public PVmodule pVmodule;
    public ArraySettings arraySettings;
    public TestPoints testPoints;
    public MinMax minmax;
    public Point origin;

    public PanelTest()
    {
    }

    public PanelTest(MasterSettings ms, PVmodule pv)
    {
      this.scaleMM = ms.scaleMM;
      this.pVmodule = new PVmodule(pv);
      this.arraySettings = new ArraySettings();
    }

    public void changeScale(double scale)
    {
      this.scaleMM = scale;
    }

    public override string ToString()
    {
      return this.arraySettings.name;
    }

    public void ChangePanel(PVmodule pv, bool orientation)
    {
      this.pVmodule = new PVmodule(pv);
    }

    public void SetBoundary(PointCollection pc)
    {
            this.boundary = pc;
      this.Correctedboundary = new PointCollection();
      this.azumith = PanelTestMethods.GetAzumith(pc);
      this.origin.X = pc[0].X;
      this.origin.Y = pc[0].Y;
      foreach (Point point in pc)
        this.Correctedboundary.Add(PanelTestMethods.RotatePoint(new Point(point.X - this.origin.X, point.Y - this.origin.Y), -this.azumith));
    }

    public void AddShading(PointCollection pc)
    {
      PointCollection pointCollection = new PointCollection();
      foreach (Point point1 in pc)
      {
        Point point2 = PanelTestMethods.RotatePoint(new Point(point1.X - this.origin.X, point1.Y - this.origin.Y), -this.azumith);
        pointCollection.Add(point2);
      }
      this.shading.Add(pc);
      this.Correctedshading.Add(pointCollection);
    }

    public void SetupTest()
    {
      this.pVmodule.ModuleOrientation(this.pVmodule.PanelOrientation, this.arraySettings.ArrayPitch);
      this.minmax = new MinMax(this.Correctedboundary, this.pVmodule, this.scaleMM);
      this.testPoints = new TestPoints();
      this.testPoints.Modify(this.scaleMM, this.pVmodule, this.azumith);
    }

    public void RunTest()
    {
      this.SetupTest();
      this.PanelLayoutPoints = new ObservableCollection<PanelPointSet>();
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
      mainWindow.percentBox.Visibility = Visibility.Visible;
      mainWindow.go.Visibility = Visibility.Hidden;
      double num1 = this.arraySettings.TestsDown * this.arraySettings.TestsAcross;
      int num2 = 0;
      for (int index1 = 0; (double) index1 < this.arraySettings.TestsAcross; ++index1)
      {
        for (int index2 = 0; (double) index2 < this.arraySettings.TestsDown; ++index2)
        {
          PanelPointSet panelPointSet = new PanelPointSet();
          PanelPointSet pps = this.DoTest(new Point(10.0 / this.scaleMM + this.arraySettings.StepAcross * (double) index1 / this.scaleMM - this.arraySettings.StartPoint.X / this.scaleMM, 10.0 / this.scaleMM + this.arraySettings.StepDown * (double) index2 / this.scaleMM - this.arraySettings.StartPoint.Y / this.scaleMM));
          this.PanelLayoutPoints.Add(pps);
          ++num2;
          PanelTestMethods.TestBoundary(pps, this.Correctedboundary, this.Correctedshading, this.testPoints.testPoints);
          mainWindow.percentBox.Text = Math.Round((double) num2 / num1 * 100.0).ToString() + "%";
          PanelTest.DoEvents();
        }
      }
      stopwatch.Stop();
      long num3 = 0;
      int num4 = 0;
      this.SelectedIndex = -1;
      foreach (PanelPointSet panelLayoutPoint in (Collection<PanelPointSet>) this.PanelLayoutPoints)
      {
        if (panelLayoutPoint.Count() > num3)
        {
          num3 = panelLayoutPoint.Count();
          this.SelectedIndex = num4;
        }
        ++num4;
      }
      mainWindow.percentBox.Visibility = Visibility.Hidden;
      mainWindow.go.Visibility = Visibility.Visible;
    }

    public static void DoEvents()
    {

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

    public PanelPointSet DoTest(Point xyOffset)
    {
      PanelPointSet panelPointSet = new PanelPointSet();
      double num1 = Math.Cos(this.arraySettings.RoofPitch / 180.0 * Math.PI);
      double num2 = this.pVmodule.PanelLength / this.scaleMM;
      double num3 = this.pVmodule.PanelWidth / this.scaleMM;
      double num4 = this.arraySettings.GapBetweenPanelsAcross / this.scaleMM;
      double num5 = this.arraySettings.GapBetweenPanelsDown / this.scaleMM * num1;
      double num6 = this.arraySettings.GapBetweenPanelsAcross / this.scaleMM;
      double num7 = this.arraySettings.GapBetweenPanelsDown / this.scaleMM;
      double num8 = this.arraySettings.WalkwayWidthAcross / this.scaleMM;
      double num9 = this.arraySettings.WalkwayWidthDown / this.scaleMM * num1;
      double num10 = num8 > num4 ? num8 - num4 : 0.0;
      double num11 = num9 > num5 ? num9 - num5 : 0.0;
      for (int index1 = 0; index1 < this.minmax.panelsRight; ++index1)
      {
        for (int index2 = 0; index2 < this.minmax.panelsUp; ++index2)
        {
          double num12 = Math.Floor(Convert.ToDouble(index1) / (double) this.arraySettings.ConsecutivePanelsAcross);
          double num13 = Math.Floor(Convert.ToDouble(index2) / (double) this.arraySettings.ConsecutivePanelsDown);
          PanelPoint panelPoint = new PanelPoint(xyOffset.X + (double) index1 * num2 + (double) index1 * num4 + num12 * num10, xyOffset.Y + (double) index2 * num3 + (double) index2 * num5 + num13 * num11);
          panelPointSet.set.Add(panelPoint);
        }
      }
      for (int index1 = 0; index1 < this.minmax.panelsLeft; ++index1)
      {
        for (int index2 = 0; index2 < this.minmax.panelsUp; ++index2)
        {
          double num12 = Math.Floor(Convert.ToDouble(index1) / (double) this.arraySettings.ConsecutivePanelsAcross) + 1.0;
          double num13 = Math.Floor(Convert.ToDouble(index2) / (double) this.arraySettings.ConsecutivePanelsDown);
          PanelPoint panelPoint = new PanelPoint(xyOffset.X - ((double) index1 * num2 + (double) index1 * num4 + num12 * num10 + num2 + num4), xyOffset.Y + (double) index2 * num3 + (double) index2 * num5 + num13 * num11);
          panelPointSet.set.Add(panelPoint);
        }
      }
      for (int index1 = 0; index1 < this.minmax.panelsRight; ++index1)
      {
        for (int index2 = 0; index2 < this.minmax.panelsDown; ++index2)
        {
          double num12 = Math.Floor(Convert.ToDouble(index1) / (double) this.arraySettings.ConsecutivePanelsAcross);
          double num13 = Math.Floor(Convert.ToDouble(index2) / (double) this.arraySettings.ConsecutivePanelsDown) + 1.0;
          PanelPoint panelPoint = new PanelPoint(xyOffset.X + (double) index1 * num2 + (double) index1 * num4 + num12 * num10, xyOffset.Y - ((double) index2 * num3 + (double) index2 * num5 + num13 * num11 + num3 + num5));
          panelPointSet.set.Add(panelPoint);
        }
      }
      for (int index1 = 0; index1 < this.minmax.panelsLeft; ++index1)
      {
        for (int index2 = 0; index2 < this.minmax.panelsDown; ++index2)
        {
          double num12 = Math.Floor(Convert.ToDouble(index1) / (double) this.arraySettings.ConsecutivePanelsAcross) + 1.0;
          double num13 = Math.Floor(Convert.ToDouble(index2) / (double) this.arraySettings.ConsecutivePanelsDown) + 1.0;
          PanelPoint panelPoint = new PanelPoint(xyOffset.X - ((double) index1 * num2 + (double) index1 * num4 + num12 * num10 + num2 + num4), xyOffset.Y - ((double) index2 * num3 + (double) index2 * num5 + num13 * num11 + num3 + num5));
          panelPointSet.set.Add(panelPoint);
        }
      }
      return panelPointSet;
    }
  }
}
