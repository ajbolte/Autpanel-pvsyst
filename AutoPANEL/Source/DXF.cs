// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.DXF
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using netDxf;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace WpfApp1.Source
{
  public class DXF
  {
    public static void imageOut(string file, DxfDocument dxf, double scale)
    {
      ImageDefinition imageDefinition = new ImageDefinition(file);
      netDxf.Entities.Image image = new netDxf.Entities.Image(imageDefinition, new Vector2(0.0, 0.0), new Vector2((double) imageDefinition.Width * scale, (double) imageDefinition.Height * scale));
      dxf.AddEntity((EntityObject) image);
    }

    public DXF()
    {
      DxfDocument dxfDocument = new DxfDocument();
    }

    public static DxfDocument DrawPanels(PanelTest pt, DxfDocument doc, double scale)
    {
      List<AciColor> aciColorList = new List<AciColor>()
      {
        new AciColor(Color.Blue),
        new AciColor(Color.Green),
        new AciColor(Color.Yellow),
        new AciColor(Color.Black),
        new AciColor(Color.Red)
      };
      foreach (PanelPoint panelPoint in (Collection<PanelPoint>) pt.PanelLayoutPoints[pt.SelectedIndex].set)
      {
        Polyline polyline = new Polyline();
        polyline.Vertexes.Add(new PolylineVertex((panelPoint.point.X + pt.testPoints.panelCorners[0].X) * scale, (panelPoint.point.Y + pt.testPoints.panelCorners[0].Y) * scale, 0.0));
        polyline.Vertexes.Add(new PolylineVertex((panelPoint.point.X + pt.testPoints.panelCorners[1].X) * scale, (panelPoint.point.Y + pt.testPoints.panelCorners[1].Y) * scale, 0.0));
        polyline.Vertexes.Add(new PolylineVertex((panelPoint.point.X + pt.testPoints.panelCorners[2].X) * scale, (panelPoint.point.Y + pt.testPoints.panelCorners[2].Y) * scale, 0.0));
        polyline.Vertexes.Add(new PolylineVertex((panelPoint.point.X + pt.testPoints.panelCorners[3].X) * scale, (panelPoint.point.Y + pt.testPoints.panelCorners[3].Y) * scale, 0.0));
        polyline.Vertexes.Add(new PolylineVertex((panelPoint.point.X + pt.testPoints.panelCorners[0].X) * scale, (panelPoint.point.Y + pt.testPoints.panelCorners[0].Y) * scale, 0.0));
        Layer layer = new Layer(pt.arraySettings.name);
        doc.Layers.Add(layer);
        polyline.Layer = layer;
        polyline.Color = aciColorList[pt.PanelColor];
        doc.AddEntity((EntityObject) polyline);
      }
      return doc;
    }
  }
}
