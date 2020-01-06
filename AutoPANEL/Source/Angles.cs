// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.Angles
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Windows;

namespace WpfApp1.Source
{
  public class Angles
  {
    public double m1 = 0.0;
    public double m2 = 0.0;
    public double cos = 0.0;
    public double sin = 0.0;
    public double angle = 0.0;

    public Angles()
    {
    }

    public Angles(Point p1, Point p2)
    {
      this.m1 = (p2.Y - p1.Y) / (p2.X - p1.X);
      this.angle = Math.Atan(this.m1);
      if (p2.X < p1.X)
        this.angle += Math.PI;
      this.m2 = -1.0 / this.m1;
      this.cos = Math.Cos(this.angle);
      this.sin = Math.Sin(this.angle);
    }

    public void changeAngle(double newAngle)
    {
      this.angle = newAngle;
      this.cos = Math.Cos(this.angle);
      this.sin = Math.Sin(this.angle);
    }
  }
}
