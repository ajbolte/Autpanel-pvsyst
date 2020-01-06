// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.MasterSettings
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System.Collections.ObjectModel;

namespace WpfApp1.Source
{
  public class MasterSettings
  {
    public ArraySettings aset = new ArraySettings();
    public ObservableCollection<PVmodule> moduleTypes = new ObservableCollection<PVmodule>();

    public string image { get; set; }

    public string path { get; set; }

    public string imageEx { get; set; }

    public string title { get; set; }

    public double scale { get; set; }

    public double scaleMM { get; set; }

    public double dxfScale { get; set; }

    public double latitude { get; set; }

    public double longitude { get; set; }

    public double lineScale { get; set; }

    public double drawingThickness { get; set; }

    public double SetLineScale(double zoom)
    {
      this.lineScale = 1.0 / (zoom / this.drawingThickness * 5.0);
      return this.lineScale;
    }

    public MasterSettings()
    {
      this.scale = 1.0;
      this.drawingThickness = 10.0;
      this.lineScale = 4.0;
    }
  }
}
