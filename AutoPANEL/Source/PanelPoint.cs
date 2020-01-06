// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.PanelPoint
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Windows;

namespace WpfApp1.Source
{
  [Serializable]
  public class PanelPoint
  {
    public bool active = false;
    public Point point;

    public PanelPoint()
    {
      this.point = new Point();
    }

    public PanelPoint(Point p)
    {
      this.point = p;
    }

    public PanelPoint(double x, double y)
    {
      this.point = new Point(x, y);
    }

    public PanelPoint(Point p, bool Activate)
    {
      this.active = Activate;
      this.point = p;
    }

    public PanelPoint(double x, double y, bool Activate)
    {
      this.active = Activate;
      this.point = new Point(x, y);
    }
  }
}
