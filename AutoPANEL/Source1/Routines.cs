using Aspose.ThreeD;
using Aspose.ThreeD.Utilities;
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Source;
using Point = System.Windows.Point;

namespace WpfApp1.Source
{
    class Routines
    {
        public static double GetAngle(Point p1, Point p2)
        {
            Double angle;
            angle = (p2.Y - p1.Y) / (p2.X-p1.X);
            angle = Math.Atan(angle);
            if (p2.X < p1.X)
            {
                angle += Math.PI;
            }
            return angle;
        }






    }
}
