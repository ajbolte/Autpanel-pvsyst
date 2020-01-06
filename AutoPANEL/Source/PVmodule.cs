// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.PVmodule
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Diagnostics;

namespace WpfApp1.Source
{
  public class PVmodule
  {
    public double PanelWide { get; set; }

    public int PanelIndex { get; set; }

    public int OrientationIndex { get; set; }

    public double PanelLong { get; set; }

    public double PanelWidth { get; set; }

    public double PanelWidthReal { get; set; }

    public double PanelLength { get; set; }

    public double PanelWatts { get; set; }

    public bool PanelOrientation { get; set; }

    public string Name { get; set; }

    public PVmodule()
    {
      this.PanelIndex = 0;
      this.OrientationIndex = 0;
      this.PanelLong = 1650.0;
      this.PanelWidthReal = 991.0;
      this.PanelWide = 991.0;
      this.PanelWatts = 290.0;
      this.PanelOrientation = false;
      this.ModuleOrientation(this.PanelOrientation, 0.0);
      this.Name = "60 cell 290W 1650mm x 991mm";
    }

    public PVmodule(PVmodule PV)
    {
      this.PanelIndex = PV.PanelIndex;
      this.OrientationIndex = PV.OrientationIndex;
      this.PanelLong = PV.PanelLong;
      this.PanelWidthReal = PV.PanelWidthReal;
      this.PanelWide = PV.PanelWide;
      this.PanelWatts = PV.PanelWatts;
      this.PanelOrientation = PV.PanelOrientation;
      this.ModuleOrientation(this.PanelOrientation, 0.0);
      this.Name = "60 cell 290W 1650mm x 991mm";
    }

    public void SetModule(
      string discription,
      double length,
      double width,
      double watts,
      bool orientation)
    {
      this.PanelLong = length;
      this.PanelWide = width;
      this.PanelWatts = watts;
      this.Name = discription;
      this.ModuleOrientation(orientation, 0.0);
    }

    public void ModuleOrientation(bool orientation, double tilt)
    {
      this.PanelOrientation = orientation;
      if (orientation)
      {
        this.OrientationIndex = 1;
        this.PanelLength = this.PanelLong;
        this.PanelWidthReal = this.PanelWide;
        this.PanelWidth = this.PanelWide * Math.Cos(tilt / 180.0 * Math.PI);
      }
      else
      {
        this.OrientationIndex = 0;
        this.PanelLength = this.PanelWide;
        this.PanelWidthReal = this.PanelLong;
        this.PanelWidth = this.PanelLong * Math.Cos(tilt / 180.0 * Math.PI);
      }
    }

    public override string ToString()
    {
      return this.Name;
    }

    private void DebugPrint()
    {
      Debug.WriteLine("");
      Debug.WriteLine("");
      Debug.WriteLine("***** PV Module Settings *****");
      Debug.WriteLine("PV Module width: " + this.PanelLength.ToString());
      Debug.WriteLine("PV Module width: " + this.PanelWidth.ToString());
      Debug.WriteLine("PV Module watts: " + this.PanelWatts.ToString());
      if (!this.PanelOrientation)
        Debug.WriteLine("PV Orientation: Portait");
      else
        Debug.WriteLine("PV Orientation: Landscape");
      Debug.WriteLine("******************************");
      Debug.WriteLine("");
      Debug.WriteLine("");
    }
  }
}
