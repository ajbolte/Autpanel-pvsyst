using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1.Source
{
    public class MinMax
    {
        public Point xMin;
        public Point xMax;
        public Point yMin;
        public Point yMax;
        public Double xLength;
        public Double yLength;
        public Double m1 = 0;
        public Double m2 = 0;
        public Double cos = 0;
        public Double sin = 0;
        public Double angle = 0;
        Double b1 = 0;
        Double b2 = 0;
        public Point intercept;
        // PointCollection pc = null;

        public MinMax()
        {
            Point p = new Point(0,0);
            xMin = p;
            yMin = p;
            p = new Point(1, 1);
            xMax = p;
            yMax = p;
        }
        public MinMax(PointCollection pc)
        {
            
            
                m1 = (pc[1].Y - pc[0].Y) / (pc[1].X - pc[0].X);
                angle = Math.Atan(m1);
                if (pc[1].X < pc[0].X)
                {
                    angle += Math.PI;
                }

                m2 = -1 / m1;

                xMin = pc.First();
                xMax = pc.First();
                yMin = pc.First();
                yMax = pc.First();






                foreach (Point p in pc)
                {


                    if (p.X < xMin.X)
                    {
                        xMin = p;
                    }

                    if (p.Y < yMin.Y)
                    {
                        yMin = p;
                    }

                    if (p.X > xMax.X)
                    {
                        xMax = p;
                    }

                    if (p.Y > yMax.Y)
                    {
                        yMax = p;
                    }

                }




            if ((angle >= Math.PI / 2) && (angle < Math.PI))
            {
                Point temp = yMin;

                yMin = xMax;
                Point temp2 = xMin;
                xMin = temp;

                temp = yMax;
                yMax = temp2;
                xMax = temp;
            }
            else if ((angle >= Math.PI))
            {
                Point temp = yMin;
                yMin = yMax;
                yMax = temp;
                temp = xMin;
                xMin = xMax;
                xMax = temp;

            }


            /*
            if (angle < 0)
            {
                angle = angle + Math.PI;
            }

            Debug.WriteLine("Angle: " + (Math.Round(angle/Math.PI*180,2)).ToString());
            if ((angle >=  Math.PI/4 ) && (angle < 0.75 * Math.PI)) // 45 < ang < 135
            {
                Point yMinTemp = yMin;
                Point yMaxTemp = yMax;
                Point xMinTemp = xMin;
                Point xMaxTemp = xMax;

                yMin = xMinTemp;        
                xMax = yMinTemp; 
                yMax = xMaxTemp;
                xMin = yMaxTemp;
            }
            else if ((angle >=0.75 * Math.PI ) && (angle < 1.25 * Math.PI)) // 135 < ang < 225
            {
                Point yMinTemp = yMin;
                Point yMaxTemp = yMax;
                Point xMinTemp = xMin;
                Point xMaxTemp = xMax;

                yMin = xMinTemp;
                xMax = yMinTemp;
                yMax = xMaxTemp;
                xMin = yMaxTemp;

            }

            else if ((angle >= 1.25 * Math.PI) && (angle < 1.75 * Math.PI)) // 225 < ang < 315
            {
                Point yMinTemp = yMin;
                Point yMaxTemp = yMax;
                Point xMinTemp = xMin;
                Point xMaxTemp = xMax;

                yMin = xMaxTemp;
                xMax = yMaxTemp;
                yMax = xMinTemp;
                xMin = yMinTemp;

            }
            */
            xLength = xMax.X - xMin.X;
            yLength = yMax.Y - yMin.Y;

            if (angle > 0 && angle < (Math.PI / 2))
            {
                b1 = yMin.Y - m1 * yMin.X;
                b2 = xMin.Y - m2 * xMin.X;

                intercept.X = (b2 - b1) / (m1 - m2);
                intercept.Y = m1 * intercept.X + b1;


            }

            else if ((angle >= 1.25 * Math.PI) && (angle < 1.75 * Math.PI ))
                {
                intercept.X = pc[0].X;
                intercept.Y = pc[0].Y;

                 }
            else if ((angle >= -0.75 * Math.PI) && (angle < -0.25 * Math.PI))
            {
                intercept.X = pc[0].X;
                intercept.Y = pc[0].Y;

            }
            else //if (angle >= 0 && angle < (Math.PI / 2))
            {
                b1 = yMin.Y - m1 * yMin.X;
                b2 = xMin.Y - m2 * xMin.X;

                intercept.X = (b2 - b1) / (m1 - m2) + 0.0005;
                intercept.Y = m1 * intercept.X + b1 + 0.0005;


            }

            if (Double.IsNaN(intercept.X))
            {
                intercept.X = pc[0].X;
                intercept.Y = pc[0].Y;
            }

            //   MessageBox.Show(yMin.X.ToString());
            //   MessageBox.Show(yMin.Y.ToString());
            //   MessageBox.Show("m1: " + m1.ToString());
            //   MessageBox.Show("m2: " + m2.ToString());
            //   MessageBox.Show("b1: " + b1.ToString());
            //   MessageBox.Show("b2: " + b2.ToString());
            //   MessageBox.Show(intercept.X.ToString());
            //   MessageBox.Show(intercept.Y.ToString());
        }

        public double AdjLengthX(Double angle)
        {
            double x = (xLength * Math.Cos(angle)) * (xLength * Math.Cos(angle)) + (yLength * Math.Sin(angle)) * (yLength * Math.Sin(angle));
            return Math.Sqrt(x);
        }

        public double AdjLengthY(Double angle)
        {
            double y = (yLength * Math.Cos(angle)) * (yLength * Math.Cos(angle)) + (xLength * Math.Sin(angle)) * (xLength * Math.Sin(angle));
            return Math.Sqrt(y);
        }
    }
     
}
