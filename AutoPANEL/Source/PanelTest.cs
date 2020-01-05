using netDxf;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Diagnostics;

using System.Windows;
using System.Collections.ObjectModel;
using static WpfApp1.Source.ShapeMods;

namespace WpfApp1.Source
{
    [Serializable] public class PanelTest
    {
        public bool? SpacingCheck = false;
        public MinMax minmax;
        public Angles angles;
        public PVmodule pVmodule;
        public arraySettings arraySettings;
        public MasterSettings masterSettings;
        public TestSettings testSettings;
        public Int16 xIt = 0;
        public Int16 yIt = 0;
        public PointCollection boundary = new PointCollection();
        public ObservableCollection<PanelList> panelList = new ObservableCollection<PanelList>();
        public Int32 num = 0;
        public Int32 SelectedIndex = 0;
        public Int32 lastpanelSet = 0;
        public int ySect = 3;
        public int xSect = 3;
        public int stepAcross = 100;
        public int stepDown = 100;
        public int ExtraPanels = 20;
        public string map = "";
     
        public double f;
        public int PanelColor = 0;
        public int OutlineColor = 1;
        public int ExclusionColor = 2;
        public List<PointCollection> points = new List<PointCollection>();
        public List<PointCollection> shading = new List<PointCollection>();
        public PointCollection outline = new PointCollection();
        public PointCollection panelPoints = new PointCollection();   

        public PanelTest()
        {

        }
        public PanelTest(MasterSettings ms)
        {
            masterSettings = ms;
            pVmodule = new PVmodule();
            arraySettings = new arraySettings();
       
        {

            }
            //  testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles, minmax);
        }
        public override string ToString()
        {
            return arraySettings.name;
            //return num.ToString();
        }
        public void ChangePanel(PVmodule pv,bool orientation)
        {
            pVmodule = pv;
            pVmodule.ModuleOrientation(orientation, arraySettings.ArrayPitch);
            MessageBox.Show(pv.PanelOrientation.ToString());
            testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles, minmax, stepAcross,stepDown, ExtraPanels);
        }
        public void PanelTestSetup()
        {
            minmax = new MinMax(boundary);
            angles = new Angles(boundary[0], boundary[1]);
            if (angles.angle == 0)
            {
                angles.angle = 0.00001;
            }

      
            pVmodule.ModuleOrientation(pVmodule.PanelOrientation,arraySettings.ArrayPitch);
            testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles,minmax, stepAcross, stepDown, ExtraPanels);
        }
        public void AzimuthChanged(Double newAngle)
        {
            angles.changeAngle(newAngle);
            if (angles.angle == 0)
            {
                angles.angle = 0.00001;
            }

            
            testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles, minmax, stepAcross, stepDown, ExtraPanels);
        }
        public void UpdateTest()
        {
            
            testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles, minmax, stepAcross, stepDown, ExtraPanels);
            
        }
        public void UpdateTest(int extraPanels)
        {
            ExtraPanels = extraPanels;
            testSettings = new TestSettings(masterSettings, pVmodule, arraySettings, angles, minmax, stepAcross, stepDown, extraPanels);

        }
        public PointCollection GoTest(int xOffset, int yOffset)
        {
            // Get test start point
            Point origin = DetermineOrigin(xOffset, yOffset);
            
            Point[][] pArray = new Point[testSettings.PanelsAcross][];
            for (int i = 0; i < testSettings.PanelsAcross; i++)
            {
                pArray[i] = new Point[testSettings.PanelsDown];
            }
            for (int i = 0; i < testSettings.PanelsDown; i++)
            {
                Double rowCalcY = i / arraySettings.ConsecutivePanelsDown;
                int j = Convert.ToInt32(Math.Truncate(rowCalcY));
                if (arraySettings.WalkwayWidthDown > arraySettings.GapBetweenPanelsDown)
                {
                    rowCalcY = j * (arraySettings.WalkwayWidthDown - arraySettings.GapBetweenPanelsDown) / masterSettings.scaleMM;
                }
                else
                {
                    rowCalcY = 0;
                }
                pArray[0][i] = new Point(origin.X - (i * (pVmodule.PanelWidth + arraySettings.GapBetweenPanelsDown) / masterSettings.scaleMM + rowCalcY) * angles.sin, origin.Y + (i * (pVmodule.PanelWidth + arraySettings.GapBetweenPanelsDown) / masterSettings.scaleMM + rowCalcY) * angles.cos);
                for (int k = 1; k < testSettings.PanelsAcross; k++)
                {
                
                    Double rowCalcX = k / arraySettings.ConsecutivePanelsAcross;
                    int l = Convert.ToInt32(Math.Truncate(rowCalcX));
                    if (arraySettings.WalkwayWidthAcross > arraySettings.GapBetweenPanelsAcross)
                    {
                        rowCalcX = l * (arraySettings.WalkwayWidthAcross - arraySettings.GapBetweenPanelsAcross) / masterSettings.scaleMM;
                    }
                    else
                    {
                        rowCalcX = 0;
                    }
                    pArray[k][i] = new Point(pArray[0][i].X + (k * (pVmodule.PanelLength + arraySettings.GapBetweenPanelsAcross) / masterSettings.scaleMM + rowCalcX) * angles.cos, pArray[0][i].Y + (k * (pVmodule.PanelLength + arraySettings.GapBetweenPanelsAcross) / masterSettings.scaleMM + rowCalcX) * angles.sin);
               //     Debug.WriteLine("Point x: " +pArray[k][i].X.ToString() + " y: " + pArray[k][i].Y.ToString());
               //   Debug.WriteLine("Length: " + pVmodule.PanelLength);
                }
            }
         
            panelPoints = new PointCollection();
         
            panelPoints = Test(pArray);
         
            return panelPoints;
        }
        // Returns the point at which the panel test originates. x and y are multiplers which shift the origin point by the test setting step across and step down values
        private Point DetermineOrigin(int x,int y)
        {
           // Debug.WriteLine("***** Determine Origin *****");
            Point p = new Point();
           // Debug.WriteLine("Start Point X:" + (testSettings.StartPoint.X * masterSettings.scale).ToString() + " Y:" + (testSettings.StartPoint.Y * masterSettings.scale).ToString());
           try
           {
             
                p = new Point(testSettings.StartPoint.X + testSettings.StepAcrossPoint.X * (-x) + testSettings.StepDownPoint.X * (-y), testSettings.StartPoint.Y + testSettings.StepAcrossPoint.Y * (-x) + testSettings.StepDownPoint.Y * (-y));
            }
            catch
            {
                MessageBox.Show("Array Start point could not be calculated, array testing starting at 0,0");
                p = new Point(0, 0);
          }
          //  Debug.WriteLine("Origin Point X:" + (p.X * masterSettings.scale).ToString() + " Y:" + (p.Y * masterSettings.scale).ToString());

           // Debug.WriteLine("****************************");
            return p;
        }
        public PointCollection Test(Point[][] panelpoints)
        {


            Boolean valid = false;
            int y = testSettings.PanelsAcross;
          //  MessageBox.Show(y.ToString());
            int x = testSettings.PanelsDown;
          //  Debug.WriteLine("Panel Point differences x: " + ((panelpoints[1][1].X- panelpoints[0][0].X)*masterSettings.scaleMM).ToString() + " y: " + ((panelpoints[1][1].Y - panelpoints[0][0].Y) * masterSettings.scaleMM).ToString());
            PointCollection pc = new PointCollection();
            for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < x; k++)
                    {
                    
                 
                  
                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary,testSettings.testPoints[0]);
                        if (valid == true)
                        {
                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[1]);
                            if (valid == true)
                            {
                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[2]);
                                if (valid == true)
                                {
                                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[3]);
                                    if (valid == true)
                                    {
                                        valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[4]);
                                        if (valid == true)
                                        {
                                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[5]);
                                            if (valid == true)
                                            {
                                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[6]);
                                                if (valid == true)
                                                {
                                                    valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[7]);
                                                    if (valid == true)
                                                    {
                                                        valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[8]);
                                                        if (valid == true)
                                                        {
                                                            valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[9]);
                                                            if (valid == true)
                                                            {
                                                                valid = ShapeMods.get_line_intersection(panelpoints[j][k], boundary, testSettings.testPoints[10]);
                                                                if (valid == true)
                                                                {
                                                                //   pc.Add(panelpoints[j][k]);

                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[0]);
                                                                    if (valid == true)
                                                                    {
                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[1]);
                                                                        if (valid == true)
                                                                        {
                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[2]);
                                                                            if (valid == true)
                                                                            {
                                                                                valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[3]);
                                                                                if (valid == true)
                                                                                {
                                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[4]);
                                                                                    if (valid == true)
                                                                                    {
                                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[5]);
                                                                                        if (valid == true)
                                                                                        {
                                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[6]);
                                                                                            if (valid == true)
                                                                                            {
                                                                                                valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[7]);
                                                                                                if (valid == true)
                                                                                                {
                                                                                                    valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[8]);
                                                                                                    if (valid == true)
                                                                                                    {
                                                                                                        valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[9]);
                                                                                                        if (valid == true)
                                                                                                        {
                                                                                                            valid = ShapeMods.get_line_shading(panelpoints[j][k], shading, testSettings.testPoints[10]);
                                                                                                            if (valid == true)
                                                                                                            {
                                                                                                             pc.Add(panelpoints[j][k]);
                                                                                                         }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                  // pc.Add(panelpoints[j][k]);
                }
                }
           
            return pc;
        }
    }
    public class PVmodule
    {
        public double PanelWide { get; set; }
        public int PanelIndex { get; set; }
        public int OrientationIndex { get; set; }
        public double PanelLong { get; set; }
        public double PanelWidth { get; set; }
        public double PanelWidthReal { get; set; }
        public double PanelLength { get; set; }
        public double PanelWatts { get; set; }
        public bool PanelOrientation { get; set; }
        public string Name { get; set; }

        public PVmodule()
        {
            PanelIndex = 0;
            OrientationIndex = 0;
            PanelLong = 1650;
            PanelWidthReal = 991;
            PanelWide = 991;
            PanelWatts = 290;
            PanelOrientation = Orientation.Portait;
            ModuleOrientation(PanelOrientation,0);
            Name = "60 cell 290W 1650mm x 991mm";
          //  DebugPrint();
        }

        public void SetModule(string discription,double length, double width, double watts,bool orientation)
        {
            PanelLong = length;
            PanelWide = width;
            PanelWatts = watts;
            Name = discription;
            ModuleOrientation(orientation,0);

        }

        public void ModuleOrientation(bool orientation,double tilt)
        {
            if (orientation == Orientation.Landscape)
            {
                PanelLength = PanelLong;
                PanelWidthReal = PanelWide;
                PanelWidth = PanelWide * Math.Cos(tilt / 180 * Math.PI);
            }
            else
            {
                PanelLength = PanelWide;
                PanelWidthReal = PanelLong;
                PanelWidth = PanelLong * Math.Cos(tilt / 180 * Math.PI);
            }
        }
        public override string ToString()
        {
            return Name;
        }
        private void DebugPrint()
        {
            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("***** PV Module Settings *****");
            Debug.WriteLine("PV Module width: " + PanelLength.ToString());
            Debug.WriteLine("PV Module width: " + PanelWidth.ToString());
            Debug.WriteLine("PV Module watts: " + PanelWatts.ToString());
            if (PanelOrientation == Orientation.Portait)
            {
                Debug.WriteLine("PV Orientation: Portait");
            }
            else
            {
                Debug.WriteLine("PV Orientation: Landscape");
            }
            Debug.WriteLine("******************************");
            Debug.WriteLine("");
            Debug.WriteLine("");
        }
    }
   [Serializable] public class arraySettings
    {
        public int ConsecutivePanelsAcross { get; set; }
        public int ConsecutivePanelsDown { get; set; }
        public double RoofPitch { get; set; }
        public double ArrayPitch { get; set; }
        public double GapBetweenPanelsAcross { get; set; }
        public double GapBetweenPanelsDown { get; set; }
        public double WalkwayWidthDown { get; set; }
        public double WalkwayWidthAcross { get; set; }
        public double Azumuth { get; set; }
        public int panelColor { get; set; }
        public int outlineColor { get; set; }
        public int shadingColor { get; set; }
        public string name { get; set; }

        private double panelTilt;  
        public double PanelTilt
        {
            get
            {
                return panelTilt;
            }
            set
            {
                panelTilt = ArrayPitch - RoofPitch;
   
            }
        }
        public arraySettings()
        {
            ConsecutivePanelsAcross = 4;
            ConsecutivePanelsDown = 20;
            GapBetweenPanelsAcross = 26;
            GapBetweenPanelsDown = 26;
            WalkwayWidthDown = 800;
            WalkwayWidthAcross = 800;
            ArrayPitch =0;
            RoofPitch = 10;
            panelTilt = PanelTilt;
            Azumuth = 0.00001;
            panelColor = 0;
            outlineColor = 0;
            shadingColor = 0;
            name = "untitled";
           // DebugPrint();
        }

        private void DebugPrint()
        {
            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("******* Array Settings *******");
            Debug.WriteLine("Consecutive Panels Across: " + ConsecutivePanelsAcross.ToString());
            Debug.WriteLine("Consecutive Panels Down: " + ConsecutivePanelsDown.ToString());
            Debug.WriteLine("Gap Between Panels Across: " + GapBetweenPanelsAcross.ToString());
            Debug.WriteLine("Gap Between Panels Down: " + GapBetweenPanelsDown.ToString());
            Debug.WriteLine("Walkway Width Across: " + WalkwayWidthAcross.ToString());
            Debug.WriteLine("Walkway Width Down: " + WalkwayWidthDown.ToString());
            Debug.WriteLine("Roof Pitch: " + RoofPitch.ToString());
            Debug.WriteLine("Array Pitch: " + ArrayPitch.ToString());
            Debug.WriteLine("Panel Tilt: " + panelTilt.ToString());
            Debug.WriteLine("Direction of Array: " + Math.Round(Azumuth,0).ToString());
            Debug.WriteLine("******************************");
            Debug.WriteLine("");
            Debug.WriteLine("");
        }
    }
    [Serializable] public static class Orientation
    {
        public const bool Portait = false;
        public const bool Landscape = true;
    }
    [Serializable] public class TestSettings
    {
        public double StepAcross { get; set; }
        public double StepDown { get; set; }
        public Point StepAcrossPoint { get; set; }
        public Point StepDownPoint { get; set; }
        public double TestsAcross { get; set; }
        public double TestsDown { get; set; }
        public int PanelsAcross { get; set; }
        public int PanelsDown { get; set; }
        public int ExtraPanels = 20;
        public Point StartPoint { get; set; }

        public PointCollection panelCorners = new PointCollection();
        public PointCollection testPoints = new PointCollection();

        public TestSettings()
        {

        }



        
        public TestSettings(MasterSettings ms, PVmodule pv, arraySettings arraySettings, Angles angles, MinMax minmax, int across, int down, int xtraPanels)
        {

            ExtraPanels = xtraPanels;
            arraySettings.Azumuth = angles.angle;
            StepAcross = across;
            StepDown = down;
            StepAcrossPoint = new Point(StepAcross * angles.cos / ms.scaleMM, StepAcross * angles.sin / ms.scaleMM);
            StepDownPoint = new Point(-StepDown * angles.sin / ms.scaleMM, StepDown * angles.cos / ms.scaleMM);
            TestsAcross = 3;
            TestsDown = 3;
            StartPoint = minmax.intercept;
            try
            {
                PanelsAcross = Convert.ToInt32(Math.Abs((minmax.xLength * Math.Cos(angles.angle)) + Math.Abs(minmax.yLength * Math.Sin(angles.angle))) / (pv.PanelLength / ms.scaleMM)) + ExtraPanels;
            }
            catch
            {
                PanelsAcross = 200;
            }
            try
            {
                PanelsDown = Convert.ToInt32(Math.Abs((minmax.yLength * Math.Cos(angles.angle)) + Math.Abs(minmax.xLength * Math.Sin(angles.angle))) / (pv.PanelWidth / ms.scaleMM)) + ExtraPanels;
            }
            catch
            {
                PanelsDown = 200;
            }
            CreatePanelTestPoints(ms, pv, arraySettings, angles);
            // DebugPrint(ms);



        }
        private void DebugPrint(MasterSettings ms)
        {
            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("******* Test Settings ********");
            Debug.WriteLine("Step Across: " + StepAcross.ToString());
            Debug.WriteLine("Step Down: " + StepDown.ToString());
            Debug.WriteLine("Step Across Point X:" + StepAcrossPoint.X.ToString() + " Y:" + StepAcrossPoint.Y.ToString());
            Debug.WriteLine("Step Down Point X:" + StepAcrossPoint.X.ToString() + " Y:" + StepAcrossPoint.Y.ToString());
            Debug.WriteLine("Tests Across: " + TestsAcross.ToString());
            Debug.WriteLine("Tests Down: " + TestsDown.ToString());
            Debug.WriteLine("Panels Across: " + PanelsAcross.ToString());
            Debug.WriteLine("Panels Downh: " + PanelsDown.ToString());
            Debug.WriteLine("Start Point X:" + (StartPoint.X * ms.scale).ToString() + " Y:" + (StartPoint.Y * ms.scale).ToString());
            Debug.WriteLine("******************************");
            Debug.WriteLine("");
            Debug.WriteLine("");
        }
        public void CreatePanelTestPoints(MasterSettings ms, PVmodule pv, arraySettings arraySettings, Angles angles)
        {
            panelCorners = new PointCollection();
            testPoints = new PointCollection();
            Point p = new Point(0, 0);
            panelCorners.Add(p);
            testPoints.Add(p);
            p = new Point(pv.PanelLength * angles.cos / ms.scaleMM, pv.PanelLength * angles.sin / ms.scaleMM);
            testPoints.Add(p);
            panelCorners.Add(p);
            p = new Point(pv.PanelLength / ms.scaleMM * angles.cos - pv.PanelWidth / ms.scaleMM * angles.sin, pv.PanelLength / ms.scaleMM * angles.sin + pv.PanelWidth / ms.scaleMM * angles.cos);
            testPoints.Add(p);
            panelCorners.Add(p);
            p = new Point(-pv.PanelWidth / ms.scaleMM * angles.sin, pv.PanelWidth / ms.scaleMM * angles.cos);
            testPoints.Add(p);
            panelCorners.Add(p);
            p = new Point(pv.PanelLength / ms.scaleMM * angles.cos / 3, pv.PanelLength / ms.scaleMM * angles.sin / 3);
            testPoints.Add(p);
            p = new Point(2 * pv.PanelLength / ms.scaleMM * angles.cos / 3, 2 * pv.PanelLength / ms.scaleMM * angles.sin / 3);
            testPoints.Add(p);
            p = new Point(-pv.PanelWidth / ms.scaleMM * angles.sin / 2, pv.PanelWidth / ms.scaleMM * angles.cos / 2);
            testPoints.Add(p);
            p = new Point((pv.PanelLength / ms.scaleMM * angles.cos / 2) - (pv.PanelWidth / ms.scaleMM * angles.sin / 2), (pv.PanelLength / ms.scaleMM * angles.sin / 2) + (pv.PanelWidth / ms.scaleMM * angles.cos / 2));
            testPoints.Add(p);
            p = new Point((pv.PanelLength / ms.scaleMM * angles.cos) - (pv.PanelWidth / ms.scaleMM * angles.sin / 2), (pv.PanelLength / ms.scaleMM * angles.sin) + (pv.PanelWidth / ms.scaleMM * angles.cos / 2));
            testPoints.Add(p);
            p = new Point((pv.PanelLength / ms.scaleMM * angles.cos / 3) - (pv.PanelWidth / ms.scaleMM * angles.sin), (pv.PanelLength / ms.scaleMM * angles.sin / 3) + (pv.PanelWidth / ms.scaleMM * angles.cos));
            testPoints.Add(p);
            p = new Point((2 * pv.PanelLength / ms.scaleMM * angles.cos / 3) - (pv.PanelWidth / ms.scaleMM * angles.sin), (2 * pv.PanelLength / ms.scaleMM * angles.sin / 3) + (pv.PanelWidth / ms.scaleMM * angles.cos));
            testPoints.Add(p);
           // DebugPrintCorners(pv);
        }
        private void DebugPrintCorners(PVmodule pv)
        {
            Debug.WriteLine("+++++++ Panel Corners +++++++");
            Debug.WriteLine("Panel length: " + pv.PanelLength.ToString() + " Panel width: " + pv.PanelWidth.ToString());
            foreach (Point p in panelCorners)
            {
                Debug.WriteLine("Point x: " + p.X.ToString() + " y: " + p.Y.ToString());
            }
            Debug.WriteLine("+++++++++++++++++++++++++++++");
        }
    }

}
      
        
      