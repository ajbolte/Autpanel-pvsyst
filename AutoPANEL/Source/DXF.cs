using netDxf;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Color = System.Drawing.Color;
namespace WpfApp1.Source
{
   
    public class DXF
    {

        public static void imageOut(string file,DxfDocument dxf,Double scale)
        {
            ImageDefinition picture = new ImageDefinition(file);
            netDxf.Entities.Image piccy = new netDxf.Entities.Image(picture, new netDxf.Vector2(0, 0), new netDxf.Vector2(picture.Width* scale, picture.Height*scale));
            dxf.AddEntity(piccy);
        }

        public DXF()
        {
            DxfDocument dxf = new DxfDocument();
        }


        #region dxf



            public static DxfDocument DrawPanels(PanelTest pt,DxfDocument doc,Double scale)
            {
                List<AciColor> cadColor = new List<AciColor>
                {
                    new AciColor(Color.Blue),
                    new AciColor(Color.Green),
                    new AciColor(Color.Yellow),
                    new AciColor(Color.Black),
                    new AciColor(Color.Red)
                };
      
                foreach (System.Windows.Point p in pt.panelList[pt.SelectedIndex].points)
                {

                    Polyline pGon = new Polyline();
                    pGon.Vertexes.Add(new PolylineVertex((p.X + pt.testSettings.panelCorners[0].X)*scale, (p.Y + pt.testSettings.panelCorners[0].Y) * scale, 0));
                    pGon.Vertexes.Add(new PolylineVertex((p.X  + pt.testSettings.panelCorners[1].X ) * scale, (p.Y  + pt.testSettings.panelCorners[1].Y) * scale, 0));
                    pGon.Vertexes.Add(new PolylineVertex((p.X  + pt.testSettings.panelCorners[2].X) * scale, (p.Y + pt.testSettings.panelCorners[2].Y) * scale, 0));
                    pGon.Vertexes.Add(new PolylineVertex((p.X  + pt.testSettings.panelCorners[3].X) * scale, (p.Y +pt.testSettings.panelCorners[3].Y) * scale, 0));
                    pGon.Vertexes.Add(new PolylineVertex((p.X + pt.testSettings.panelCorners[0].X) * scale, (p.Y + pt.testSettings.panelCorners[0].Y) * scale, 0));
                    Layer layer = new Layer(pt.arraySettings.name);
                    doc.Layers.Add(layer);
                    pGon.Layer = layer;
                    pGon.Color = cadColor[pt.PanelColor];
                    doc.AddEntity(pGon);


                }
            return doc;
            }

         /*   public DxfDocument DrawBoundary(DxfDocument dxf)
            {
                Polyline pGon = new Polyline();
                pGon.Vertexes.Add(new PolylineVertex(boundary[0].X * scale, boundary[0].Y * scale, 0));
                for (int i = 1;i< boundary.Count; i++)
                {
                    pGon.Vertexes.Add(new PolylineVertex(boundary[i].X * scale, boundary[i].Y * scale, 0));

                }
                pGon.Vertexes.Add(new PolylineVertex(boundary[0].X * scale, boundary[0].Y * scale, 0));
                Layer layer = new Layer("Panels");


                dxf.Layers.Add(layer);
                dxf.AddEntity(pGon);
                return dxf;
            }*/
            
        #endregion
    }
}
