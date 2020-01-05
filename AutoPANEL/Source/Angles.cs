using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Source
{
    public class Angles
    {
        public Double m1 = 0;
        public Double m2 = 0;
        public Double cos = 0;
        public Double sin = 0;
        public Double angle = 0;

        public Angles()
        {

        }
        public Angles(Point p1, Point p2)
        {
            m1 = (p2.Y - p1.Y) / (p2.X - p1.X);
            angle = Math.Atan(m1);

            if (p2.X < p1.X)
            {
                angle += Math.PI;
            }

            m2 = -1 / m1;
            cos = Math.Cos(angle);
            sin = Math.Sin(angle);

        }

        public void changeAngle(Double newAngle)
        {
            angle = newAngle;
            cos = Math.Cos(angle);
            sin = Math.Sin(angle);
        }
    }
}
