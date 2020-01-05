using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPANEL.Source
{
    class Latitude
    {
        static double a = 6378137;
        static double f = 1/298.2572236;
        static double drad = Math.PI / 180;
        static double k0   = 0.9996;                      // scale on central meridian
        static double b    = a * (1 - f);                   // polar axis
        static double e    = Math.Sqrt(1 - (b / a) * (b / a));  // eccentricity
        static double e0   = e / Math.Sqrt(1 - e * e);      // called e' in reference
        static double esq  = (1 - (b / a) * (b / a));         // e² for use in expansions
        static double e0sq = e * e / (1 - e * e);             // e0² — always even powers



         public static double utmToLatLon(double x,double y,double utmz,bool north)
        {

            // First some validation:
            if (x < 160000 || x > 840000)
            {
              //  alert("Outside permissible range of easting values.");
              //  return;
            }
            if (y < 0)
            {
                //alert("Negative values are not allowed for northing.");
               // return;
            }
            if (y > 10000000)
            {
              ////  alert("Northing may not exceed 10,000,000.");
             //   return;
            }

            // Now the actual calculation:
             double zcm = 3 + 6 * (utmz - 1) - 180;  // central meridian of zone
            double e1  = (1 - Math.Sqrt(1 - e * e)) / (1 + Math.Sqrt(1 - e * e));  // called e₁ in USGS PP 1395
            double M0  = 0;  // in case origin other than zero lat - not needed for standard UTM

            double M = 0;  // arc length along standard meridian
            if (north)
            {
                M = M0 + y / k0;
            }
            else
            {  // southern hemisphere
                M = M0 + (y - 10000000) / k0;
            }
            double mu = M / (a * (1 - esq * (1 / 4 + esq * (3 / 64 + 5 * esq / 256))));
            double phi1 = mu + e1 * (3 / 2 - 27 * e1 * e1 / 32) * Math.Sin(2 * mu) + e1 * e1 * (21 / 16 - 55 * e1 * e1 / 32) * Math.Sin(4 * mu);  // footprint Latitude
            phi1 = phi1 + e1 * e1 * e1 * (Math.Sin(6 * mu) * 151 / 96 + e1 * Math.Sin(8 * mu) * 1097 / 512);
            double C1 = e0sq * Math.Pow(Math.Cos(phi1), 2);
            double T1 = Math.Pow(Math.Tan(phi1), 2);
            double N1 = a / Math.Sqrt(1 - Math.Pow(e * Math.Sin(phi1), 2));
            double R1 = N1 * (1 - e * e) / (1 - Math.Pow(e * Math.Sin(phi1), 2));
            double D  = (x - 500000) / (N1 * k0);
            double phi = (D * D) * (1 / 2 - D * D * (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * e0sq) / 24);
            phi = phi + Math.Pow(D, 6) * (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * e0sq - 3 * C1 * C1) / 720;
            phi = phi1 - (N1 * Math.Tan(phi1) / R1) * phi;

            // Output Latitude:
            double outLat = Math.Floor(1000000 * phi / drad) / 1000000;

            double lng = D * (1 + D * D * ((-1 - 2 * T1 - C1) / 6 + D * D * (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * e0sq + 24 * T1 * T1) / 120)) / Math.Cos(phi1);
            double lngd = zcm + lng / drad;

            // Output Longitude:
            double outLon = Math.Floor(1000000 * lngd) / 1000000;

            return outLat;
        }

        public static double Spacing(double PanelLength,double RoofPitch, double ArrayTilt, double Angle, double latitude)
        {
            double[] hour = new double[] { -0.785398163, -0.523598776, -0.261799388,0, 0.261799388, 0.523598776, 0.785398163 };

            double[] q = new double[] { 0.7071, 0.866, 0.9659, 1, 0.9659,0.866 , 0.7071 };

            double PanelTilt = ArrayTilt - RoofPitch;
            double vertical = Math.Sin(PanelTilt / 180 * Math.PI) * PanelLength;
          //  Debug.WriteLine("vertical: " + vertical.ToString());
            double p18 = Math.Sin(Math.Abs(latitude) / 180 * Math.PI);
         //   Debug.WriteLine("p18: " + p18.ToString());
            double q18 = Math.Cos(Math.Abs(latitude) / 180 * Math.PI);
         //   Debug.WriteLine("q18: " + q18.ToString());
            double p19 = Math.Sin(-23.45 / 180 * Math.PI);
         //   Debug.WriteLine("p19: " + p19.ToString());
            double q19 = Math.Cos(-23.45 / 180 * Math.PI);
         //   Debug.WriteLine("q19: " + q19.ToString());
            double t = p18 * p19 + q18 * q19;
 
            double v = Math.Asin(t);

            double y = vertical / Math.Tan(v);

            double[] z = new double[7];
            for(int i = 0; i < 7; i++)
            {
                t = p18 * p19 + q18 * q19 * q[i];
             //   Debug.WriteLine("t: " + t.ToString());
                v = Math.Asin(t);
              //  Debug.WriteLine("v: " + v.ToString());
                y = vertical / Math.Tan(v);
              //  Debug.WriteLine("y: " + y.ToString());
              //  Debug.WriteLine("hout[i]: " + hour[i].ToString());
               // Debug.WriteLine("Angle: " + Angle.ToString());
                z[i] =  Math.Cos((hour[i] - Angle) );
              //  Debug.WriteLine("z: " + z[i].ToString());
                z[i] = y * Math.Cos((hour[i] - Angle) );
              //  Debug.WriteLine("hour: " +( 9+i*1).ToString() + " Space: " + z[i].ToString() );
            }
            double returnVal = Math.Round(z.Max());
            if (returnVal < 26)
            {
                returnVal = 26;
            }
            else
            {
                returnVal = returnVal / 50;
                returnVal = Math.Ceiling(returnVal);
                returnVal = returnVal * 50;
            }
            return returnVal;
        }
    }
}
