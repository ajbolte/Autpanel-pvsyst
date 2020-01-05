using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Source
{
    class DXF
    {

        public static void imageOut(string file,netDxf.DxfDocument dxf,Double scale)
        {
            ImageDefinition picture = new ImageDefinition(file);
            //MessageBox.Show(file);
           // MessageBox.Show(picture.Height.ToString());
            MessageBox.Show(picture.Width.ToString());
            MessageBox.Show(scale.ToString());
            netDxf.Entities.Image piccy = new netDxf.Entities.Image(picture, new netDxf.Vector2(0, 0), new netDxf.Vector2(picture.Width* scale*1000, picture.Height*scale*1000));
            dxf.AddEntity(piccy);
        }
    }
}
