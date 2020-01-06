// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.PanelPointSet
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Collections.ObjectModel;

namespace WpfApp1.Source
{
  [Serializable]
  public class PanelPointSet
  {
    public ObservableCollection<PanelPoint> set;

    public PanelPointSet()
    {
      this.set = new ObservableCollection<PanelPoint>();
    }

    public override string ToString()
    {
      long num = 0;
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) this.set)
      {
        if (panelPoint.active)
          ++num;
      }
      return num.ToString();
    }

    public long Count()
    {
      long num = 0;
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) this.set)
      {
        if (panelPoint.active)
          ++num;
      }
      return num;
    }
  }
}
