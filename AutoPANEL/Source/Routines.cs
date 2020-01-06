// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.Routines
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Globalization;
using System.Net;
using System.Windows;

namespace WpfApp1.Source
{
  internal class Routines
  {
    public static double GetAngle(Point p1, Point p2)
    {
      double num = Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
      if (p2.X < p1.X)
        num += Math.PI;
      return num;
    }

    public static int GetTime(DateTime date1)
    {
      try
      {
        DateTime exact = DateTime.ParseExact(WebRequest.Create("http://www.microsoft.com").GetResponse().Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", (IFormatProvider) CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);
        return (date1 - exact).Days;
      }
      catch
      {
        return -99;
      }
    }
  }
}
