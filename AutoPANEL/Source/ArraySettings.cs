// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.ArraySettings
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Diagnostics;
using System.Windows;

namespace WpfApp1.Source
{
  [Serializable]
  public class ArraySettings
  {
    public bool useSpacing = false;
    public PVmodule pv;
    private double panelTilt;

    public int ConsecutivePanelsAcross { get; set; }

    public int ConsecutivePanelsDown { get; set; }

    public double RoofPitch { get; set; }

    public double ArrayPitch { get; set; }

    public double GapBetweenPanelsAcross { get; set; }

    public double GapBetweenPanelsDown { get; set; }

    public double WalkwayWidthDown { get; set; }

    public double WalkwayWidthAcross { get; set; }

    public double Azumuth { get; set; }

    public int panelColor { get; set; }

    public int outlineColor { get; set; }

    public int exclusionColor { get; set; }

    public string name { get; set; }

    public double StepAcross { get; set; }

    public double StepDown { get; set; }

    public Point StepAcrossPoint { get; set; }

    public Point StepDownPoint { get; set; }

    public double TestsAcross { get; set; }

    public double TestsDown { get; set; }

    public Point StartPoint { get; set; }

    public double PanelTilt
    {
      get
      {
        return this.panelTilt;
      }
      set
      {
        this.panelTilt = this.ArrayPitch - this.RoofPitch;
      }
    }

    public ArraySettings()
    {
      this.ConsecutivePanelsAcross = 4;
      this.ConsecutivePanelsDown = 20;
      this.GapBetweenPanelsAcross = 26.0;
      this.GapBetweenPanelsDown = 26.0;
      this.WalkwayWidthDown = 800.0;
      this.WalkwayWidthAcross = 800.0;
      this.ArrayPitch = 0.0;
      this.RoofPitch = 10.0;
      this.panelTilt = this.PanelTilt;
      this.Azumuth = 1E-05;
      this.panelColor = 0;
      this.outlineColor = 0;
      this.exclusionColor = 0;
      this.name = "untitled";
      this.StepAcross = 300.0;
      this.StepDown = 300.0;
      this.StepAcrossPoint = new Point(this.StepAcross, 0.0);
      this.StepDownPoint = new Point(0.0, this.StepDown);
      this.TestsAcross = 3.0;
      this.TestsDown = 3.0;
      this.StartPoint = new Point(0.0, 0.0);
      this.pv = new PVmodule();
    }

    private void DebugPrint()
    {
      Debug.WriteLine("");
      Debug.WriteLine("");
      Debug.WriteLine("******* Array Settings *******");
      Debug.WriteLine("Consecutive Panels Across: " + this.ConsecutivePanelsAcross.ToString());
      Debug.WriteLine("Consecutive Panels Down: " + this.ConsecutivePanelsDown.ToString());
      Debug.WriteLine("Gap Between Panels Across: " + this.GapBetweenPanelsAcross.ToString());
      Debug.WriteLine("Gap Between Panels Down: " + this.GapBetweenPanelsDown.ToString());
      Debug.WriteLine("Walkway Width Across: " + this.WalkwayWidthAcross.ToString());
      Debug.WriteLine("Walkway Width Down: " + this.WalkwayWidthDown.ToString());
      Debug.WriteLine("Roof Pitch: " + this.RoofPitch.ToString());
      Debug.WriteLine("Array Pitch: " + this.ArrayPitch.ToString());
      Debug.WriteLine("Panel Tilt: " + this.panelTilt.ToString());
      Debug.WriteLine("Direction of Array: " + Math.Round(this.Azumuth, 0).ToString());
      Debug.WriteLine("******************************");
      Debug.WriteLine("");
      Debug.WriteLine("");
    }
  }
}
