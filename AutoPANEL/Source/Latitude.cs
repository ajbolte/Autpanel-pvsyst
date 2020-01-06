// Decompiled with JetBrains decompiler
// Type: AutoPANEL.Source.Latitude
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPANEL.Source
{
  internal class Latitude
  {
    private static double a = 6378137.0;
    private static double f = 0.00335281066433155;
    private static double drad = Math.PI / 180.0;
    private static double k0 = 0.9996;
    private static double b = Latitude.a * (1.0 - Latitude.f);
    private static double e = Math.Sqrt(1.0 - Latitude.b / Latitude.a * (Latitude.b / Latitude.a));
    private static double e0 = Latitude.e / Math.Sqrt(1.0 - Latitude.e * Latitude.e);
    private static double esq = 1.0 - Latitude.b / Latitude.a * (Latitude.b / Latitude.a);
    private static double e0sq = Latitude.e * Latitude.e / (1.0 - Latitude.e * Latitude.e);

    public static double utmToLatLon(double x, double y, double utmz, bool north)
    {
      if (x >= 160000.0 && x <= 840000.0)
        ;
      if (y >= 0.0)
        ;
      if (y <= 10000000.0)
        ;
      double num1 = 3.0 + 6.0 * (utmz - 1.0) - 180.0;
      double num2 = (1.0 - Math.Sqrt(1.0 - Latitude.e * Latitude.e)) / (1.0 + Math.Sqrt(1.0 - Latitude.e * Latitude.e));
      double num3 = 0.0;
      double num4 = (!north ? num3 + (y - 10000000.0) / Latitude.k0 : num3 + y / Latitude.k0) / (Latitude.a * (1.0 - Latitude.esq * (0.0 + Latitude.esq * (0.0 + 5.0 * Latitude.esq / 256.0))));
      double num5 = num4 + num2 * (1.0 - 27.0 * num2 * num2 / 32.0) * Math.Sin(2.0 * num4) + num2 * num2 * (1.0 - 55.0 * num2 * num2 / 32.0) * Math.Sin(4.0 * num4) + num2 * num2 * num2 * (Math.Sin(6.0 * num4) * 151.0 / 96.0 + num2 * Math.Sin(8.0 * num4) * 1097.0 / 512.0);
      double num6 = Latitude.e0sq * Math.Pow(Math.Cos(num5), 2.0);
      double num7 = Math.Pow(Math.Tan(num5), 2.0);
      double num8 = Latitude.a / Math.Sqrt(1.0 - Math.Pow(Latitude.e * Math.Sin(num5), 2.0));
      double num9 = num8 * (1.0 - Latitude.e * Latitude.e) / (1.0 - Math.Pow(Latitude.e * Math.Sin(num5), 2.0));
      double x1 = (x - 500000.0) / (num8 * Latitude.k0);
      double num10 = x1 * x1 * (0.0 - x1 * x1 * (5.0 + 3.0 * num7 + 10.0 * num6 - 4.0 * num6 * num6 - 9.0 * Latitude.e0sq) / 24.0) + Math.Pow(x1, 6.0) * (61.0 + 90.0 * num7 + 298.0 * num6 + 45.0 * num7 * num7 - 252.0 * Latitude.e0sq - 3.0 * num6 * num6) / 720.0;
      double num11 = Math.Floor(1000000.0 * (num5 - num8 * Math.Tan(num5) / num9 * num10) / Latitude.drad) / 1000000.0;
      double num12 = x1 * (1.0 + x1 * x1 * ((-1.0 - 2.0 * num7 - num6) / 6.0 + x1 * x1 * (5.0 - 2.0 * num6 + 28.0 * num7 - 3.0 * num6 * num6 + 8.0 * Latitude.e0sq + 24.0 * num7 * num7) / 120.0)) / Math.Cos(num5);
      double num13 = Math.Floor(1000000.0 * (num1 + num12 / Latitude.drad)) / 1000000.0;
      return num11;
    }

    public static double Spacing(
      double PanelLength,
      double RoofPitch,
      double ArrayTilt,
      double Angle,
      double latitude)
    {
      double[] numArray1 = new double[7]
      {
        -0.785398163,
        -0.523598776,
        -1.0 * Math.PI / 12.0,
        0.0,
        Math.PI / 12.0,
        0.523598776,
        0.785398163
      };
      double[] numArray2 = new double[7]
      {
        0.7071,
        0.866,
        0.9659,
        1.0,
        0.9659,
        0.866,
        0.7071
      };
      double num1 = Math.Sin((ArrayTilt - RoofPitch) / 180.0 * Math.PI) * PanelLength;
      double num2 = Math.Sin(Math.Abs(latitude) / 180.0 * Math.PI);
      double num3 = Math.Cos(Math.Abs(latitude) / 180.0 * Math.PI);
      double num4 = Math.Sin(-0.40927970959267);
      double num5 = Math.Cos(-0.40927970959267);
      double a1 = Math.Asin(num2 * num4 + num3 * num5);
      double num6 = num1 / Math.Tan(a1);
      double[] numArray3 = new double[7];
      for (int index = 0; index < 7; ++index)
      {
        double a2 = Math.Asin(num2 * num4 + num3 * num5 * numArray2[index]);
        double num7 = num1 / Math.Tan(a2);
        numArray3[index] = Math.Cos(numArray1[index] - Angle);
        numArray3[index] = num7 * Math.Cos(numArray1[index] - Angle);
      }
      double num8 = Math.Round(((IEnumerable<double>) numArray3).Max());
      return num8 >= 26.0 ? Math.Ceiling(num8 / 50.0) * 50.0 : 26.0;
    }
  }
}
