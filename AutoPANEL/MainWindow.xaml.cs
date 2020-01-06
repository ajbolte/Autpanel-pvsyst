using netDxf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.Source;
using Microsoft.VisualBasic;
using Point = System.Windows.Point;
using System.Diagnostics;
using System.Collections.ObjectModel;
using AutoPANEL.Source;
using static WpfApp1.Source.ShapeMods;
using Path = System.IO.Path;
using System.Threading.Tasks;
using System.Threading;
using netDxf.Entities;
using Polyline = System.Windows.Shapes.Polyline;
using Line = System.Windows.Shapes.Line;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Windows.Media.Media3D;
using System.Web.UI.DataVisualization.Charting;
using Point3d = System.Web.UI.DataVisualization.Charting.Point3D;
namespace AutoPANEL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields and collections
        bool SuperUser = true;
        bool DisableKeyRead = false;
        int pvIndex = 0;
        MasterSettings ms = new MasterSettings();
        ObservableCollection<PanelTest> lp = new ObservableCollection<PanelTest>();
        private SaveFile saveFile;
        public bool? pauseAnimation = false;
        IList<System.Windows.Shapes.Polyline> shadeCollection = new List<Polyline>();
        List<List<Point3d>> roofSections = new List<List<Point3d>>();
        IList<Polyline> outlineCollection = new List<Polyline>();
        ObservableCollection<PanelPointSet> SelectedPanelList = new ObservableCollection<PanelPointSet>();
        ObservableCollection<SolidColorBrush> Color = new ObservableCollection<SolidColorBrush>();
        ObservableCollection<string> ColorName = new ObservableCollection<string>();
        PointCollection polygonPoints = new PointCollection();
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        Point distanceStart = new Point(0, 0);
        Point distanceFinish = new Point(0, 0);
        private Polyline pline;
        Boolean drawing = false;
        public Boolean boolo = false;
        Stopwatch watchTime = new System.Diagnostics.Stopwatch();
        private double SnappedAngle = 0;
        public Double x0 = 0;
        public Double y0 = 0;
        public bool updatingLines = false;
        Double savedAngle = 0;
        SolidColorBrush tempColor = new SolidColorBrush(Colors.Green);
        Double dist = 0;
        int bList = 0;
        private string file;
        Boolean lockShift = false;
        private bool AngleSquare = false;
        BitmapImage image = new BitmapImage();
        public Line line = null;
        int panelImageSelection = 0;
        string pvsystPath = "";

        #endregion
        #region Startup
        public MainWindow()
        {
            InitializeComponent();
            loadSettings();
            // Disable advanced features for basic user
            if (SuperUser == false)
            {
                BasicUserSetup();
            }
            // Initial settings
            InitSettings();
            Debug.WriteLine("Program started");
            // Start new project or load existing
            bool loadProjectFlag = StartupMsgbox();
            if (loadProjectFlag == true)
            {
                LoadProject();
            }
            else
            {
                startNewProject();
            }
        }
        public void BasicUserSetup()
        {
            dfxGo.Visibility = Visibility.Hidden;
            button.Visibility = Visibility.Hidden;
            autoCAD.Visibility = Visibility.Hidden;
        }

        public void loadSettings()
        {
            try
            {
                string f = System.Reflection.Assembly.GetExecutingAssembly().Location;
                f = Regex.Replace(f, @"\bautoPanel.exe\b", "", RegexOptions.IgnoreCase);

                XmlDocument doc = new XmlDocument();
                doc.Load(f + "Resources\\settings.xml");


                XmlNode node = doc.DocumentElement.SelectSingleNode("/Settings/PVsystPath");

                pvsystPath = node.InnerText;
            }
            catch
            {
                MessageBox.Show("Could not load settings.xml");
            }


        }
        public void InitSettings()
        {
            var date1 = new DateTime(2020, 1, 30, 23, 59, 59);

            windie.Title = "AutoPANEL V1.01 - Expires " + date1.ToLongDateString();
            var v = Routines.GetTime(date1);
            if (v < 0)
            {
                MessageBox.Show("Licence Expired");
                Environment.Exit(0);
            }
            //  watchTime.Start();
            lockShift = false;
            snapLock.IsChecked = false;
            sBox.Text = "1";
            arrayList.ItemsSource = lp;
            panelList.ItemsSource = SelectedPanelList;
            PanelTypes.ItemsSource = ms.moduleTypes;
            PanelTypesGlobal.ItemsSource = ms.moduleTypes;
            //  pv = new PVmodule();
            //  pv.SetModule("Ground Mount", 4000, 19000, 13110, false);
            //  ms.moduleTypes.Add(pv);

            PVmodule pv = new PVmodule();

            pv.SetModule("72-CELL MONOCRYSTALLINE 400W", 1956, 992, 400, false);
            ms.moduleTypes.Add(pv);
            pv = new PVmodule();
            pv.SetModule("72-CELL MONOCRYSTALLINE 390W SunPower", 2067, 992, 390, false);
            ms.moduleTypes.Add(pv);
            pv = new PVmodule();
            pv.SetModule("72-CELL MONOCRYSTALLINE 440W TRINA TALLMAX", 2102, 1040, 440, false);
            ms.moduleTypes.Add(pv);
            pv = new PVmodule();
            pv.SetModule("60-CELL MONOCRYSTALLINE 335W", 1650, 992, 335, false);
            ms.moduleTypes.Add(pv);
            pv = new PVmodule();
            pv.SetModule("GROUND MOUNT 14400W", 3938, 18298, 14400, false);
            ms.moduleTypes.Add(pv);

            PanelTypes.SelectedIndex = 0;
            PanelTypes.Items.Refresh();
            PanelTypesGlobal.SelectedIndex = 0;
            PanelTypesGlobal.Items.Refresh();
            OrientationBox.Items.Add("Portrait");
            OrientationBox.Items.Add("Landscape");
            OrientationBox.SelectedIndex = 0;
            OrientationBoxGlobal.Items.Add("Portrait");
            OrientationBoxGlobal.Items.Add("Landscape");
            OrientationBoxGlobal.SelectedIndex = 0;
            panelImage.Items.Add("Simple Box");
            panelImage.Items.Add("Panel Image");
            panelImage.Items.Add("Both");
            panelImage.SelectedIndex = 0;
            scrollie.ScrollChanged += OnScrollViewerScrollChanged;
            scrollie.MouseRightButtonUp += OnMouseLeftButtonUp;
            scrollie.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollie.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollie.PreviewMouseRightButtonDown += OnMouseRightButtonDown;
            scrollie.MouseMove += OnMouseMove;
            slider.ValueChanged += OnSliderValueChanged;
            arrayList.SelectedIndex = 0;
            arrayList.SelectedIndex = 0;


            Color = new ObservableCollection<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Black),
                new SolidColorBrush(Colors.Red)
            };
            ColorName = new ObservableCollection<string>
            {
                "Blue",
                "Green",
                "Yellow",
                "Black",
                "Red"
            };
            panelColorList1.ItemsSource = ColorName;
            shadingColorList1.ItemsSource = ColorName;
            outlineColorList1.ItemsSource = ColorName;
            panelColorList1.Items.Refresh();
            shadingColorList1.Items.Refresh();
            panelColorList1.Items.Refresh();
            panelColorList1.SelectedIndex = 0;
            shadingColorList1.SelectedIndex = 2;
            outlineColorList1.SelectedIndex = 1;
            panelColorList1Glob.ItemsSource = ColorName;
            shadingColorList1Glob.ItemsSource = ColorName;
            outlineColorList1Glob.ItemsSource = ColorName;
            panelColorList1Glob.Items.Refresh();
            shadingColorList1Glob.Items.Refresh();
            panelColorList1Glob.Items.Refresh();
            panelColorList1Glob.SelectedIndex = 0;
            shadingColorList1Glob.SelectedIndex = 2;
            outlineColorList1Glob.SelectedIndex = 1;
            Debug.WriteLine("Initiate complete");
        }
        private Boolean StartupMsgbox()
        {

            MsgBoxYesNo msgbox = new MsgBoxYesNo();
            // Starting messagebox. True to load project, False to start new and select image.

            if ((bool)msgbox.ShowDialog())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void startNewProject()
        {

            Source.Latitude latitude = new Source.Latitude();
            // Load image
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = "*.jpg",
                Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|BMP Files (*.bmp)|*.bmp"
            };

            bool? result = dlg.ShowDialog();
            // Clear any existing data
            if (result == true)
            {
                ShapeMods.RemoveAllPgons(TheCanvas);
                ShapeMods.RemovePlines(TheCanvas);
                ms = new MasterSettings();
                lp.Clear();
                // Setup file paths
                ms.image = dlg.FileName;
                ms.path = System.IO.Path.GetDirectoryName(dlg.FileName);
                ms.title = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                ms.imageEx = System.IO.Path.GetExtension(dlg.FileName);
                TitleName.Text = ms.title;
                // Setup image
                SetupImage(ms.image);
                // Setup scale
                SetupScale(ms.image);


            }

        }
        private void LoadProject()
        {

        }
        public void SetupImage(string file)
        {

            image = new BitmapImage();
            // Open image
            try
            {
                image.BeginInit();
            }
            catch
            {

            }
            image.UriSource = new Uri(@file);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            // Setup canvas image height
            TheCanvas.Height = image.Height;
            TheCanvas.Width = image.Width;
            grid.Height = image.Height;
            grid.Width = image.Width;
            ImageViewer1.Height = image.Height;
            ImageViewer1.Width = image.Width;
            ImageViewer1.Source = image;
            // MessageBox.Show();
            if (image.Height > 3000)
            {
                slider.Minimum = 0.2;

            }
            else if (image.Height < 1000)
            {
                slider.Minimum = 1;
            }
            else
            {
                slider.Minimum = 0.5;
            }

            slider.Value = slider.Minimum;
            manualZoom(slider.Value);


        }
        public void SetupScale(string file)
        {
            string xUTM = "0";
            string yUTM = "0";

            // Determine corrisponding world filename (.jgw)
            string filename2 = file.Substring(0, file.Length - 3) + "jgw";

            // try to open world file and read scale and latitude
            try
            {
                StreamReader sr = new StreamReader(filename2);
                // Read scale
                file = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // Read latitude and longitude
                    yUTM = xUTM;
                    xUTM = sr.ReadLine();
                }
            }
            catch
            {
                file = Interaction.InputBox("The wold co-ordinate file (.JGW) was not found in the same directory as the selected image. Please enter the meters per pixel (resolution) of the image for scaling. This value can be found on the  dialog screen when downloading the Nearmaps imagery", "JGW File Not Found", "0.074646", 800, 600);
            }
            // Set scales
            ms.dxfScale = Convert.ToDouble(file);
            ms.latitude = Convert.ToDouble(xUTM);
            ms.longitude = Convert.ToDouble(yUTM);
            if (ms.latitude < 0)
            {
                MessageBox.Show("Error with georeference file (.jgw). Please ensure that GDA94 is selected when downloading georeferenced image and not WGS84. Image scaling and latitude May not be correct");
                ms.latitude = -35;
            }
            else
            {
                ms.latitude = Latitude.utmToLatLon(0, ms.latitude, 54, false);

            }
            latitudeBox.Text = ms.latitude.ToString();
            ms.scale = ms.dxfScale * image.DpiX / 96;
            ms.scaleMM = ms.scale * 1000;
            scaleBox.Text = ms.scale.ToString();


        }

        public ArraySettings ReadGlobalSettings()
        {
            ArraySettings aset = new ArraySettings();
            aset.name = arrayNameGlobal.Text;
            aset.RoofPitch = Convert.ToDouble(RoofPitchGlobal.Text);
            aset.ArrayPitch = Convert.ToDouble(ArrayPitchGlobal.Text);
            aset.ConsecutivePanelsAcross = Convert.ToInt32(PanelsAcrossGlobal.Text);
            aset.ConsecutivePanelsDown = Convert.ToInt32(PanelsDownGlobal.Text);
            aset.GapBetweenPanelsAcross = Convert.ToDouble(GapAcrossGlobal.Text);
            aset.GapBetweenPanelsDown = Convert.ToDouble(GapDownGlobal.Text);
            aset.WalkwayWidthAcross = Convert.ToDouble(WalkwaySizeAcrossGlobal.Text);
            aset.WalkwayWidthDown = Convert.ToDouble(WalkwaySizeDownGlobal.Text);
            aset.useSpacing = (bool)SpacingCheck.IsChecked;


            return aset;

        }

        private void WriteArraySettings(ArraySettings aset)
        {

            if (aset.useSpacing == true)
            {
                SpacingCheck.IsChecked = true;
            }
            else
            {
                SpacingCheck.IsChecked = false;
            }
            azimuth.Text = (Math.Round(aset.Azumuth / Math.PI * 180, 2)).ToString();
            arrayName.Text = aset.ToString();
            PanelsAcross.Text = aset.ConsecutivePanelsAcross.ToString();
            PanelsDown.Text = aset.ConsecutivePanelsDown.ToString();



            GapAcross.Text = aset.GapBetweenPanelsAcross.ToString();
            GapDown.Text = aset.GapBetweenPanelsDown.ToString();
            WalkwaySizeAcross.Text = aset.WalkwayWidthAcross.ToString();
            WalkwaySizeDown.Text = aset.WalkwayWidthDown.ToString();
            PanelTypes.SelectedIndex = aset.pv.PanelIndex;

            OrientationBox.SelectedIndex = aset.pv.OrientationIndex;
            double d = Latitude.Spacing(aset.pv.PanelWidthReal, aset.RoofPitch, aset.ArrayPitch, aset.Azumuth, ms.latitude);
            panelColorList1.SelectedIndex = aset.panelColor;
            outlineColorList1.SelectedIndex = aset.outlineColor;
            shadingColorList1.SelectedIndex = aset.exclusionColor;
            rowSpacing.Text = d.ToString();
            RoofPitch.Text = aset.RoofPitch.ToString();
            ArrayPitch.Text = aset.ArrayPitch.ToString();

            if (SpacingCheck.IsChecked == true)
            {
                GapDown.Text = rowSpacing.Text;
            }
        }
        #endregion
        #region mouse and keyboard clicks
        private void UpdateCursorPosition(Point p)
        {
            xBox.Text = (Math.Round((p.X * ms.scale), 2, MidpointRounding.ToEven)).ToString();
            yBox.Text = (Math.Round((p.Y * ms.scale), 2, MidpointRounding.ToEven)).ToString();
        }
        private void UpdateLineThickness()
        {

            if (pauseAnimation == false)
            {
                //    updatingLines = true;

                ms.lineScale = ms.SetLineScale(scaleTransform.ScaleX);
                ShapeMods.changePlineThickess(ms.lineScale, TheCanvas);
                updatingLines = ShapeMods.changePgonThickess(ms.lineScale, TheCanvas);
                IEnumerable<Polygon> query1 = TheCanvas.Children.OfType<Polygon>();

            }
        }


        private void TheCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point point = e.GetPosition(TheCanvas);
                UpdateCursorPosition(point);
                var x = point.X;
                var y = point.Y;
                distanceFinish.X = point.X;
                distanceFinish.Y = point.Y;
                Double ang = Math.Atan((distanceFinish.Y - distanceStart.Y) / (distanceFinish.X - distanceStart.X));
                dist = Math.Sqrt(Math.Abs((distanceFinish.X - distanceStart.X) * (distanceFinish.X - distanceStart.X)) + Math.Abs((distanceFinish.Y - distanceStart.Y) * (distanceFinish.Y - distanceStart.Y)));
                dist *= ms.scale;

                dist = Math.Round(dist, 2, MidpointRounding.ToEven);
                dBox.Text = dist.ToString();


                if (snapLock.IsChecked == false)
                {


                    if (boolo == true) // drawing in progress
                    {
                        if (distanceFinish.X < distanceStart.X)
                        {
                            ang += Math.PI;

                        }
                        ang = ang / Math.PI * 180;
                        ang = Math.Round(ang, 2);
                        snapBox.Text = ang.ToString();
                        line.X2 = x;
                        line.Y2 = y;
                    }

                }

                else //snap lock on
                {
                    ang = Convert.ToDouble(snapBox.Text);
                    ang = ang / 180 * Math.PI;
                    SnappedAngle = ang;

                    while (ang > Math.PI / 4)
                    {
                        ang -= Math.PI / 2;
                    }
                    while (ang <= -Math.PI / 4)
                    {
                        ang += Math.PI / 2;
                    }
                    snapBox.Text = Math.Round((ang * 180 / Math.PI), 2).ToString();


                    if (line != null)
                    {
                        if (Math.Abs(line.X1 - x) > Math.Abs(line.Y1 - y))// weird error
                        {
                            line.X2 = x;

                            double yyy = line.Y1 + (x - line.X1) * Math.Tan(ang);
                            if (double.IsNaN(yyy) != true)
                            {
                                line.Y2 = yyy;
                            }
                        }
                        else
                        {
                            try
                            {
                                line.Y2 = y;
                                line.X2 = line.X1 - (y - line.Y1) * Math.Tan(ang);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }


        }
        public void TheCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  UpdateLineThickness();
            Point point = e.GetPosition(TheCanvas);
            var x = point.X;
            var y = point.Y;
            if (drawing == true)
            {
                distanceFinish.X = x;
                distanceFinish.Y = y;
                dist = Math.Sqrt(Math.Abs((distanceFinish.X - distanceStart.X) * (distanceFinish.X - distanceStart.X)) + Math.Abs((distanceFinish.Y - distanceStart.Y) * (distanceFinish.Y - distanceStart.Y)));
                dist *= ms.scale;
                dist = Math.Round(dist, 2, MidpointRounding.ToEven);
                dBox.Text = dist.ToString();
            }
            distanceStart.X = x;
            distanceStart.Y = y;
            drawing = true;
            if (boolo == false)
            {
                TempPolylineStart(point);
            }
            else
            {
                if (sqaure.IsChecked == false)
                {
                    TempPolylinePoint(point);
                }
                else
                {
                    TempPolylineSquare(point);
                }
            }
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(scrollie);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                scrollie.ScrollToHorizontalOffset(scrollie.HorizontalOffset - dX);
                scrollie.ScrollToVerticalOffset(scrollie.VerticalOffset - dY);
            }






        }
        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            if (e.Delta > 0)
            {
                slider.Value += 0.1;
            }
            if (e.Delta < 0)
            {
                slider.Value -= 0.1;
            }

            e.Handled = true;
        }
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollie.Cursor = Cursors.Arrow;
            scrollie.ReleaseMouseCapture();
            lastDragPoint = null;
        }
        private void KeyDownRead(object sender, KeyEventArgs e)
        {
            if (DisableKeyRead == false)
            {
                DisableKeyRead = true;
                if ((e.Key == Key.RightCtrl) && (lockShift == false) && (snapLock.IsChecked == true) && (boolo == true))
                {
                    lockShift = true;
                    // MessageBox.Show("HEre");
                    savedAngle = Convert.ToDouble(snapBox.Text);
                    snapLock.IsChecked = false;

                }
                else if (((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt)) && pauseAnimation == false)
                {

                    pauseAnimation = true;
                    animation.IsChecked = pauseAnimation;
                }
                else if (e.Key == Key.Escape)
                {
                    ShapeMods.RemoveLines(TheCanvas);
                    line = new Line();
                    boolo = false;

                }
                else if ((e.Key == Key.LeftShift) || (e.Key == Key.RightShift))
                {

                    if (boolo == true)
                    {
                        snapLock.IsChecked = !snapLock.IsChecked;
                        if (snapLock.IsChecked == true)
                        {
                            sqaure.IsChecked = false;
                        }
                    }
                }

                else if ((e.Key == Key.Space))
                {

                    if (boolo == true)
                    {
                        sqaure.IsChecked = !sqaure.IsChecked;
                    }
                }
            }
        }
        private void KeyUpRead(object sender, KeyEventArgs e)
        {

            if (((e.Key == Key.RightCtrl)) && lockShift == true)
            {

                if (boolo == true)
                {
                    lockShift = false;
                    snapBox.Text = savedAngle.ToString();
                    snapLock.IsChecked = true;


                }
            }
            else if (((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt)) && pauseAnimation == true)
            {

                pauseAnimation = false;
                animation.IsChecked = pauseAnimation;
                UpdateLineThickness();
            }
            DisableKeyRead = false;
        }
        void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Events for right mouse button click

            // Is an object currently being drawn by the user? false if not drawing:
            if (drawing == false)
            {
                //     var mousePos = e.GetPosition( );
                //  Do stuff
            }
            //If user is currently drawing:
            else
            {
                FinishDrawing();
            }
        }
        Point GetMousePos()
        {
            return TheCanvas.PointToScreen(Mouse.GetPosition(TheCanvas));
        }
        #endregion
        #region Drawing
        public void TempPolylineStart(Point polyPoint)
        {

            Debug.WriteLine("Drawing started");
            polygonPoints = new PointCollection
                {
                    polyPoint
                };
            UpdateLineThickness();
            line = ShapeMods.lineStart(polyPoint.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);
            boolo = true;
        }
        public void TempPolylinePoint(Point polyPoint)
        {
            UpdateLineThickness();
            if (snapLock.IsChecked == false)
            {

                line.X2 = polyPoint.X;
                line.Y2 = polyPoint.Y;
                Double ang = (line.Y2 - line.Y1) / (line.X2 - line.X1);
                ang = Math.Atan(ang);
                if (line.X2 < line.X1)
                {
                    ang = ang + Math.PI;
                }
                snapBox.Text = Math.Round((ang / Math.PI * 180), 2).ToString();

                polygonPoints.Add(polyPoint);



                line = ShapeMods.lineStart(polyPoint.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);

            }
            else
            {
                polygonPoints.Add(new Point(line.X2, line.Y2));
                line = ShapeMods.lineStart(line.X2, line.Y2, tempColor, ms.lineScale, TheCanvas);


            }


        }
        public void TempPolylineSquare(Point polyPoint)
        {
            if (AngleSquare == false)
            {
                Point p = new Point(line.X1, line.Y1);
                Point p1 = new Point(polyPoint.X, line.Y1);
                Point p2 = new Point(polyPoint.X, polyPoint.Y);
                Point p3 = new Point(line.X1, polyPoint.Y);
                line.X2 = polyPoint.X;
                line.Y2 = p.Y;
                line = ShapeMods.lineStart(polyPoint.X, p.Y, tempColor, ms.lineScale, TheCanvas);

                // polygonPoints.Add(polyPoint);
                line.X2 = polyPoint.X;
                line.Y2 = polyPoint.Y;

                line = ShapeMods.lineStart(polyPoint.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);

                line.X2 = p.X;
                line.Y2 = polyPoint.Y;

                line = ShapeMods.lineStart(p.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);

                line.X2 = p.X;
                line.Y2 = p.Y;

                line = ShapeMods.lineStart(p.X, p.Y, tempColor, ms.lineScale, TheCanvas);
                polygonPoints.Add(p1);
                polygonPoints.Add(p2);
                polygonPoints.Add(p3);
                polygonPoints.Add(p);

            }
            else
            {
                Debug.WriteLine("****** Draw Angled Square ******");
                snapLock.IsChecked = false;
                int ChangeSign = 1;

                while (SnappedAngle > Math.PI / 4)
                {
                    SnappedAngle -= Math.PI / 2;
                }
                while (SnappedAngle <= -Math.PI / 4)
                {
                    SnappedAngle += Math.PI / 2;
                }
                if (SnappedAngle < 0)
                {
                    //     SnappedAngle = Math.PI/2 + SnappedAngle;
                }
                Debug.WriteLine("Snapped angle: " + (SnappedAngle / Math.PI * 180).ToString());
                double CrossAngle = Math.Atan((polyPoint.Y - line.Y1) / (polyPoint.X - line.X1)) - SnappedAngle;
                double CrossLength = Math.Sqrt(Math.Abs(polyPoint.X - line.X1) * Math.Abs(polyPoint.X - line.X1) + Math.Abs(polyPoint.Y - line.Y1) * Math.Abs(polyPoint.Y - line.Y1));
                if (polyPoint.X < line.X1)
                {
                    CrossAngle = Math.PI + CrossAngle;
                    ChangeSign = 1;
                }
                Debug.WriteLine("Cross angle: " + ((CrossAngle + SnappedAngle) / Math.PI * 180).ToString());
                Debug.WriteLine("Combined angle: " + ((CrossAngle) / Math.PI * 180).ToString());
                Debug.WriteLine("Cross Length: " + (CrossLength).ToString());
                Debug.WriteLine("X start: " + line.X1.ToString() + " X finish: " + polyPoint.X.ToString());
                Debug.WriteLine("y start: " + line.Y1.ToString() + " y finish: " + polyPoint.Y.ToString());
                Debug.WriteLine("********************************");
                double xLength = CrossLength * Math.Cos(CrossAngle);
                double yLength = CrossLength * Math.Sin(CrossAngle);

                Point p = new Point();
                Point p1 = new Point();
                Point p2 = new Point();
                Point p3 = new Point();
                if (((CrossAngle <= 3 * Math.PI / 2) && (CrossAngle > Math.PI)) || ((CrossAngle <= Math.PI / 2) && (CrossAngle > 0)))
                {
                    p = new Point(line.X1, line.Y1);
                    p1 = new Point(line.X1 + xLength * Math.Cos(SnappedAngle) * ChangeSign, line.Y1 + xLength * Math.Sin(SnappedAngle) * ChangeSign);
                    p2 = new Point(polyPoint.X, polyPoint.Y);
                    p3 = new Point(polyPoint.X - xLength * Math.Cos(SnappedAngle) * ChangeSign, polyPoint.Y - xLength * Math.Sin(SnappedAngle) * ChangeSign);
                    Debug.WriteLine("normal");

                }
                else
                {
                    p = new Point(line.X1, line.Y1);
                    p3 = new Point(line.X1 + xLength * Math.Cos(SnappedAngle) * ChangeSign, line.Y1 + xLength * Math.Sin(SnappedAngle) * ChangeSign);
                    p2 = new Point(polyPoint.X, polyPoint.Y);
                    p1 = new Point(polyPoint.X - xLength * Math.Cos(SnappedAngle) * ChangeSign, polyPoint.Y - xLength * Math.Sin(SnappedAngle) * ChangeSign);
                    Debug.WriteLine("reversed");
                }
                Debug.WriteLine("********************************");
                //MessageBox.Show(((CrossAngle)/Math.PI*180 ).ToString());
                line.X2 = polyPoint.X;
                line.Y2 = p.Y;
                line = ShapeMods.lineStart(polyPoint.X, p.Y, tempColor, ms.lineScale, TheCanvas);

                // polygonPoints.Add(polyPoint);
                line.X2 = polyPoint.X;
                line.Y2 = polyPoint.Y;

                line = ShapeMods.lineStart(polyPoint.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);

                line.X2 = p.X;
                line.Y2 = polyPoint.Y;

                line = ShapeMods.lineStart(p.X, polyPoint.Y, tempColor, ms.lineScale, TheCanvas);

                line.X2 = p.X;
                line.Y2 = p.Y;

                line = ShapeMods.lineStart(p.X, p.Y, tempColor, ms.lineScale, TheCanvas);
                polygonPoints.Add(p1);
                polygonPoints.Add(p2);
                polygonPoints.Add(p3);
                polygonPoints.Add(p);
                //  AngleSquare = false;
            }


            FinishDrawing();
        }
        public void TempPolylineFinish()
        {
            Debug.WriteLine("Drawing finished");
            Point point = polygonPoints.First();
            line.X2 = point.X;
            line.Y2 = point.Y;
            polygonPoints.Add(point);
        }
        public void PolyLineAdd(PointCollection polyPoints)
        {
            UpdateLineThickness();

            pline = ShapeMods.PlineCreate(polyPoints, tempColor, ms.lineScale, TheCanvas);

        }
        public void ShadingAdd(PanelTest pt)
        {
            UpdateLineThickness();

            new Polyline();
            pline = ShapeMods.PlineCreate(pt.shading[pt.shading.Count() - 1], Color[pt.ExclusionColor], ms.lineScale, TheCanvas);
            pline.Uid = pt.num.ToString() + "ex";
            shadeCollection.Add(pline);

        }

        public void RoofAdd(PointCollection pc)
        {
            UpdateLineThickness();

            new Polyline();
            pline = ShapeMods.PlineCreate(pc, Color[1], ms.lineScale, TheCanvas);
            pline.Uid = "roof";
            shadeCollection.Add(pline);

        }
        public void ShadingAddAll(PanelTest pt)
        {
            UpdateLineThickness();
            new Polyline();
            foreach (PointCollection pc in pt.shading)
            {

                pline = ShapeMods.PlineCreate(pc, Color[pt.ExclusionColor], ms.lineScale, TheCanvas);
                pline.Uid = pt.num.ToString() + "ex"; ;
                shadeCollection.Add(pline);
            }
        }
        public void ShadingSetAdd(PanelTest pt)
        {
            UpdateLineThickness();
            foreach (PointCollection pc in pt.shading)
            {
                pline = ShapeMods.PlineCreate(pc, Color[pt.ExclusionColor], ms.lineScale, TheCanvas);
                pline.Uid = pt.num.ToString() + "ex";
                shadeCollection.Add(pline);

            }
        }
        public void outLineAdd(PanelTest pt)
        {

            pline = new Polyline();
            pline = ShapeMods.PlineCreate(pt.boundary, Color[pt.OutlineColor], ms.lineScale, TheCanvas);
            pline.Uid = pt.num.ToString();
            outlineCollection.Add(pline);

        }
        void FinishDrawing()
        {
            // Finish drawing object
            TempPolylineFinish();
            ShapeMods.RemoveLines(TheCanvas);
            // If the user was drawing a shade object, add it to the collection of shade objects for the assosiated array


            if (shadingTick_Copy.IsChecked == true)
            {

                List<Point3d> xyz = new List<Point3d>();
                double[] heights = new double[8];
                heights[0] = Convert.ToDouble(p1.Text);
                heights[1] = Convert.ToDouble(p2.Text);
                heights[2] = Convert.ToDouble(p3.Text);
                heights[3] = Convert.ToDouble(p4.Text);
                heights[4] = Convert.ToDouble(p5.Text);
                heights[5] = Convert.ToDouble(p6.Text);
                heights[6] = Convert.ToDouble(p7.Text);
                heights[7] = Convert.ToDouble(p8.Text);
                int z = 0;
                foreach (Point p in polygonPoints)
                {
                    Point3d p3 = new Point3d();
                    p3.X = (float)p.X;
                    p3.Y = (float)p.Y;
                    p3.Z = (float)heights[z];
                    z++;
                    if (z > 7)
                    {
                        z = 7;
                    }
                    xyz.Add(p3);
                }
                xyz.RemoveAt(polygonPoints.Count - 1);
                roofSections.Add(xyz);
                RoofAdd(polygonPoints);


            }



            else if (shadingTick.IsChecked == true)
            {
                int selectedItems = arrayList.SelectedItems.Count;
                if (selectedItems > 0)
                {
                    foreach (PanelTest pt1 in arrayList.SelectedItems)
                    {

                        pt1.shading.Add(polygonPoints);
                        ShadingAdd(pt1);
                        ShapeMods.RemoveLines(TheCanvas);

                    }
                }
            }
            else
            {
                PVmodule pv = (PVmodule)PanelTypesGlobal.SelectedItem;
                PanelTest pt = new PanelTest(ms, pv);



                pt.num = lp.Count();
                pt.SetBoundary(polygonPoints);
                pt.SetupTest();

                ShapeMods.RemoveLines(TheCanvas);

                outLineAdd(pt);

                lp.Add(pt);
                arrayList.Items.Refresh();
                arrayList.SelectedIndex = pt.num;
                foreach (PanelTest pt1 in arrayList.SelectedItems)
                {

                    DoTest(pt1);
                }
            }
            boolo = false;
        }
        #endregion
        #region Events
        void manualZoom(double e)
        {
            try
            {
                scaleTransform.ScaleX = e;
                scaleTransform.ScaleY = e;
                sBox.Text = scaleTransform.ScaleX.ToString();

                UpdateLineThickness();

                var centerOfViewport = new Point(scrollie.ViewportWidth / 2,
                    scrollie.ViewportHeight / 2);
                lastCenterPositionOnTarget = scrollie.TranslatePoint(centerOfViewport, grid);
            }
            catch { }
        }
        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                scaleTransform.ScaleX = e.NewValue;
                scaleTransform.ScaleY = e.NewValue;
                sBox.Text = e.NewValue.ToString();

                UpdateLineThickness();
                var centerOfViewport = new Point(scrollie.ViewportWidth / 2,
                scrollie.ViewportHeight / 2);
                lastCenterPositionOnTarget = scrollie.TranslatePoint(centerOfViewport, grid);
            }
            catch { }
        }
        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {

                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollie.ViewportWidth / 2,
                                                         scrollie.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              scrollie.TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;
                    //  MessageBox.Show(e.ExtentWidth.ToString() + " " + grid.Width.ToString());
                    //  MessageBox.Show(e.ExtentHeight.ToString() + " " + grid.Height.ToString());
                    double newOffsetX = scrollie.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollie.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {

                        return;
                    }

                    scrollie.ScrollToHorizontalOffset(newOffsetX);
                    scrollie.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        #endregion
        #region Button Clicks
        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            string file1;

            saveFile = new SaveFile();
            try
            {
                saveFile.masterSettings = ms;
                saveFile.PanelTests = lp;
                Microsoft.Win32.SaveFileDialog dlg1 = new Microsoft.Win32.SaveFileDialog
                {
                    // Set filter for file extension and default file extension 
                    DefaultExt = "*.apf",
                    Filter = "AutoPanel Files (*.apf)|*.apf",
                    FileName = ms.title + ".apf",
                    RestoreDirectory = true

                };

                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = dlg1.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    file1 = dlg1.FileName;
                    saveImage(file1);
                    System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(SaveFile));
                    FileStream file = System.IO.File.Create(file1);
                    writer.Serialize(file, saveFile);
                    file.Close();
                    MessageBox.Show("FILE SAVED");
                }

            }

            catch
            { MessageBox.Show("ERROR SAVING FILE"); }
        }
        public void Load_Click_1(object sender, RoutedEventArgs e)
        {
            ShapeMods.RemoveAllPgons(TheCanvas);
            LoadProject();

        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Load_Click_1(null, new RoutedEventArgs());
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            SaveButton_Click_1(null, new RoutedEventArgs());
        }
        public void Go_Click(object sender, RoutedEventArgs e)
        {
            List<int> TempCollection = new List<int>();
            int selItemCount = arrayList.SelectedItems.Count;
            if (selItemCount > 0)
            {
                foreach (PanelTest pt in arrayList.SelectedItems)
                {
                    TempCollection.Add(pt.num);

                }
                foreach (int i in TempCollection)
                {
                    arrayList.SelectedIndex = i;
                    PanelTest pt1 = arrayList.SelectedItem as PanelTest;
                    DoTest(pt1);
                }
            }
        }
        private void DeleteArray_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PanelTest pt1 = arrayList.SelectedItem as PanelTest;
                int DeletedItem = arrayList.SelectedIndex;
                ShapeMods.RemovePgons(TheCanvas, pt1.num.ToString());
                ShapeMods.RemovePlineByTag(TheCanvas, pt1.num.ToString());
                ShapeMods.RemovePlineByTag(TheCanvas, pt1.num.ToString() + "ex");
                lp.Remove(pt1);
                arrayList.Items.Refresh();
                // ShapeMods.RemovePlines(TheCanvas);
                foreach (PanelTest pt in arrayList.Items)
                {
                    if (pt.num > DeletedItem)
                    {
                        ShapeMods.RemovePgons(TheCanvas, pt.num.ToString());
                        ShapeMods.RemovePlineByTag(TheCanvas, pt.num.ToString());
                        ShapeMods.RemovePlineByTag(TheCanvas, pt.num.ToString() + "ex");
                        pt.num = pt.num - 1;
                        arrayList.Items.Refresh();
                        outLineAdd(pt);
                        ShadingAddAll(pt);
                        int i = 0;
                        /*   foreach (Point p in pt.panelList[pt.SelectedIndex].points)
                           {
                               i++;
                               ShapeMods.InsertPanel(p, pt.testSettings.panelCorners, Color[pt.PanelColor], ms.lineScale, TheCanvas, pt.num.ToString(), ms.scaleMM, pt.pVmodule, pt.arraySettings.Azumuth, panelImageSelection);
                               if (i > 100)
                               {

                                   DoEvents();
                                   i = 0;
                               }
                           }
                           */
                    }
                    // outLineAdd(pt);
                    //ShadingAdd
                    shadingTick.IsChecked = false;
                    arrayList.SelectedIndex = DeletedItem - 1;
                }
            }
            catch
            {
                MessageBox.Show("No array selected");
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {



        }
        private void DfxGo_Click(object sender, RoutedEventArgs e)
        {

            CreateDXF();
        }
        #endregion
        #region Textbox changes
        private void PercentBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Azimuth_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {

                    }
                }
            }
            catch
            {
                //  MessageBox.Show("Changing array direction angle failed");
            }
        }
        private void ArrayName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {


                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.name = arrayName.Text;
                        arrayList.Items.Refresh();
                    }
                }
            }
            catch
            {
                // MessageBox.Show("Changing array name failed");
            }

        }
        private void GapAcross_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.GapBetweenPanelsAcross = Convert.ToDouble(GapAcross.Text);
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                //     MessageBox.Show("Changing array settings failed");
            }
        }
        private void GapDown_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.GapBetweenPanelsDown = Convert.ToDouble(GapDown.Text);
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                //    MessageBox.Show("Changing array settings failed");
            }
        }
        private void PanelsAcross_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.ConsecutivePanelsAcross = Convert.ToInt32(PanelsAcross.Text);
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                //  MessageBox.Show("Changing array settings failed");
            }
        }
        private void PanelsDown_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.ConsecutivePanelsDown = Convert.ToInt32(PanelsDown.Text);
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }
        }
        private void arrayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            boolo = true;
            try
            {
                arrayList.Items.Refresh();
                SelectedPanelList.Clear();
                CalcPower();
                PanelTest pt = arrayList.SelectedItem as PanelTest;
                foreach (PanelPointSet pps in pt.PanelLayoutPoints)
                {
                    SelectedPanelList.Add(pps);
                }
                panelList.Items.Refresh();
                panelList.SelectedIndex = pt.SelectedIndex;


                UpdateLineThickness();
                ShapeMods.changePlineThickessByID(ms.lineScale * 3, TheCanvas, pt.num.ToString());

            }
            catch { }
            boolo = false;
        }
        private void PanelListChanged(object sender, SelectionChangedEventArgs e)
        {
            // Debug.WriteLine("");
            // Debug.WriteLine("***** Panel List Changed *****");
            try
            {
                PanelTest pt = arrayList.SelectedItem as PanelTest;
                //   Debug.WriteLine("Panel test number: " + pt.num.ToString());
                //   Debug.WriteLine("Panel List Index: " + panelList.SelectedIndex.ToString());
                ShapeMods.RemovePgons(TheCanvas, pt.num.ToString());

                InsertPanelA(pt, TheCanvas);
                pt.SelectedIndex = panelList.SelectedIndex;
                CalcPower();
                //   Debug.WriteLine("Panel List Change Sucseessful");
            }
            catch
            {
                //  Debug.WriteLine("Panel List Change Failed");
            }
            //Debug.WriteLine("*****************************");
        }
        private void PanelTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Sqaure_Checked(object sender, RoutedEventArgs e)
        {
            if ((snapLock.IsChecked == true) && (sqaure.IsChecked == true))
            {
                AngleSquare = true;
                savedAngle = Convert.ToDouble(snapBox.Text);
                SnappedAngle = savedAngle / 180 * Math.PI;
                snapLock.IsChecked = false;
            }
        }
        private void LineThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ms.drawingThickness = Convert.ToDouble(lineThickness.Text);
                UpdateLineThickness();

            }
            catch { }
        }
        private void ExportImage_click(object sender, RoutedEventArgs e)
        {
            ShapeMods.pLineVisibility(TheCanvas, Visibility.Hidden);
            ExportToImage(TheCanvas, TheCanvas);

            ShapeMods.pLineVisibility(TheCanvas, Visibility.Visible);
        }
        private void PrintPDF_Click(object sender, RoutedEventArgs e)
        {
            ShapeMods.pLineVisibility(TheCanvas, Visibility.Hidden);
            PrintPDF();
            ShapeMods.pLineVisibility(TheCanvas, Visibility.Visible);
        }
        private void New_Click_1(object sender, RoutedEventArgs e)
        {
            startNewProject();
        }
        private void ShadingTick_Checked(object sender, RoutedEventArgs e)
        {
            if (lp.Count < 1)
            {
                MessageBox.Show("No array outlines inserted. Please add outline before adding shading");
                shadingTick.IsChecked = false;
            }
        }
        private void Tabby1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void AngleBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            /* Double ang = Convert.ToDouble(angleBox.Text);
             ang = ang / 180 * Math.PI;
             angleModified = true;
             MessageBox.Show("yes");
             */
        }
        private void WalkwaySizeAcross_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.WalkwayWidthAcross = Convert.ToDouble(WalkwaySizeAcross.Text);
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                //  MessageBox.Show("Changing array settings failed");
            }
        }
        private void WalkwaySizeDown_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {

                        pt.arraySettings.WalkwayWidthDown = Convert.ToDouble(WalkwaySizeDown.Text);

                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {
                // MessageBox.Show("Changing array settings failed");
            }
        }
        private void RoofPitch_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.RoofPitch = Convert.ToDouble(RoofPitch.Text);
                        double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                        rowSpacing.Text = d.ToString();


                        if (SpacingCheck.IsChecked == true)
                        {
                            //  pt.arraySettings.GapBetweenPanelsDown = d;
                            GapDown.Text = rowSpacing.Text;
                        }
                        arrayList.Items.Refresh();



                        if (SpacingCheck.IsChecked == true)
                        {
                            //  pt.arraySettings.GapBetweenPanelsDown = d;
                            GapDown.Text = rowSpacing.Text;
                        }
                    }
                }
            }
            catch
            {
                //  MessageBox.Show("Changing array settings failed");
            }
        }
        private void ArrayPitch_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.ArrayPitch = Convert.ToDouble(ArrayPitch.Text);
                        pt.pVmodule.ModuleOrientation(pt.pVmodule.PanelOrientation, pt.arraySettings.ArrayPitch);
                        arrayList.Items.Refresh();


                        double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                        rowSpacing.Text = d.ToString();


                        if (SpacingCheck.IsChecked == true)
                        {
                            //  pt.arraySettings.GapBetweenPanelsDown = d;
                            GapDown.Text = rowSpacing.Text;
                        }
                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }
        }
        private void SpacingCheck_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {

                        double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                        rowSpacing.Text = d.ToString();



                        if (SpacingCheck.IsChecked == true)
                        {
                            pt.SpacingCheck = true;

                            GapDown.Text = rowSpacing.Text;
                            pt.arraySettings.GapBetweenPanelsDown = Convert.ToDouble(rowSpacing.Text);


                        }
                        else
                        {
                            pt.SpacingCheck = false;
                        }




                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }


            if (SpacingCheck.IsChecked == true)
            {
                GapDown.Text = rowSpacing.Text;
            }
        }
        private void PanelColorList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)

                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.PanelColor = panelColorList1.SelectedIndex;
                        ShapeMods.changePgonColorByID(Color[pt.PanelColor], TheCanvas, pt.num.ToString());

                    }

            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }
        }
        private void OutlineColorList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.OutlineColor = outlineColorList1.SelectedIndex;

                        ShapeMods.changePlineColorByID(Color[pt.OutlineColor], TheCanvas, pt.num.ToString());

                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }
        }
        private void ShadingColorList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.ExclusionColor = outlineColorList1.SelectedIndex;
                        ShapeMods.changePlineColorByID(Color[pt.ExclusionColor], TheCanvas, pt.num.ToString() + "ex");

                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }
        }
        private void ExtraPanels_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void XSect_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void XTest_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void YSect_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TitleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ms.title = TitleName.Text;
        }

        private void LatitudeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ms.latitude = Convert.ToDouble(latitudeBox.Text);

                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        pt.arraySettings.RoofPitch = Convert.ToDouble(RoofPitch.Text);
                        double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                        rowSpacing.Text = d.ToString();


                        if (SpacingCheck.IsChecked == true)
                        {
                            //  pt.arraySettings.GapBetweenPanelsDown = d;
                            GapDown.Text = rowSpacing.Text;
                        }
                        arrayList.Items.Refresh();

                    }
                }
            }
            catch
            {

            }
        }

        private void RoofPitchGlobal_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ScaleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ms.dxfScale = Convert.ToDouble(scaleBox.Text);
                ms.scale = ms.dxfScale * image.DpiX / 96;
                ms.scaleMM = ms.scale * 1000;

                if ((arrayList.SelectedItems.Count > 0) && boolo == false)
                {
                    foreach (PanelTest pt in arrayList.Items)
                    {


                    }
                }
            }
            catch
            {
                //   MessageBox.Show("Changing array settings failed");
            }

        }
        #endregion
        #region Do Things
        public void ExportToImage(Canvas canvas, Canvas c)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";
            dlg.DefaultExt = "jpg";
            dlg.FilterIndex = 1;
            dlg.FileName = ms.title + ".jpg";
            dlg.RestoreDirectory = true;

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            string path = dlg.FileName;
            int selectedFilterIndex = dlg.FilterIndex;

            if (result == true)
            {

                try
                {
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                             (int)canvas.ActualWidth, (int)canvas.ActualHeight,
                              96d, 96d, PixelFormats.Pbgra32);
                    // needed otherwise the image output is black
                    canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
                    canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));
                    //   MessageBox.Show("HI");
                    renderBitmap.Render(canvas);




                    BitmapEncoder imageEncoder = new PngBitmapEncoder();


                    if (selectedFilterIndex == 0)
                    {

                    }
                    else if (selectedFilterIndex == 1)
                    {
                        imageEncoder = new JpegBitmapEncoder();
                    }
                    else if (selectedFilterIndex == 2)
                    {
                        imageEncoder = new PngBitmapEncoder();
                    }
                    else if (selectedFilterIndex == 3)
                    {

                        imageEncoder = new GifBitmapEncoder();
                    }


                    imageEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));



                    using (FileStream file = File.Create(path))
                    {
                        imageEncoder.Save(file);

                    }



                }
                catch
                {

                }

            }

        }

        public void ExportToImage1(Viewbox canvas, Canvas c)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";
            dlg.DefaultExt = "jpg";
            dlg.FilterIndex = 1;
            dlg.FileName = ms.title + ".jpg";
            dlg.RestoreDirectory = true;

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            string path = dlg.FileName;
            int selectedFilterIndex = dlg.FilterIndex;

            if (result == true)
            {

                //  try
                //  {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                         (int)canvas.ActualWidth, (int)canvas.ActualHeight,
                          96d, 96d, PixelFormats.Pbgra32);
                // needed otherwise the image output is black
                canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
                canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));
                //   MessageBox.Show("HI");
                renderBitmap.Render(canvas);




                BitmapEncoder imageEncoder = new PngBitmapEncoder();


                if (selectedFilterIndex == 0)
                {

                }
                else if (selectedFilterIndex == 1)
                {
                    imageEncoder = new JpegBitmapEncoder();
                }
                else if (selectedFilterIndex == 2)
                {
                    imageEncoder = new PngBitmapEncoder();
                }
                else if (selectedFilterIndex == 3)
                {

                    imageEncoder = new GifBitmapEncoder();
                }


                imageEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));



                using (FileStream file = File.Create(path))
                {
                    imageEncoder.Save(file);

                }



                //  }
                //  catch
                {

                }

            }

        }


        public BitmapImage LayoutImage(Canvas canvas)
        {

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                     (int)canvas.ActualWidth, (int)canvas.ActualHeight,
                      96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
            canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));

            renderBitmap.Render(canvas);
            BitmapEncoder imageEncoder = new PngBitmapEncoder();

            MemoryStream memoryStream = new MemoryStream();
            imageEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            imageEncoder.Save(memoryStream);
            BitmapImage bImg = new BitmapImage();
            memoryStream.Position = 0;
            bImg.BeginInit();
            // bImg.StreamSource = memoryStream;
            bImg.StreamSource = new MemoryStream(memoryStream.ToArray());
            bImg.EndInit();

            //  memoryStream.Close();

            return bImg;







        }
        private void PrintPDF()
        {
            var v = scrollie.VerticalScrollBarVisibility;
            try
            {
                PrintDialog dialog = new PrintDialog();


                scrollie.VerticalScrollBarVisibility = 0;
                scrollie.HorizontalScrollBarVisibility = 0;
                if (dialog.ShowDialog() != true)
                    return;
                dialog.PrintVisual(scrollie, "IFMS Print Screen");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Screen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            scrollie.VerticalScrollBarVisibility = v;
            scrollie.HorizontalScrollBarVisibility = v;

        }
        public void DoTest(PanelTest pt)
        {
            go.Visibility = Visibility.Hidden;
            arrayList.Visibility = Visibility.Hidden;
            deleteArray_Copy.Visibility = Visibility.Hidden;


            arrayList.Visibility = Visibility.Visible;
            go.Visibility = Visibility.Visible;
            deleteArray_Copy.Visibility = Visibility.Visible;
        }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
        void CreateDXF()
        {
            double viewScale = 0;
            double newScale = 3000;
            DxfDocument dxf = new DxfDocument();
            string f = System.Reflection.Assembly.GetExecutingAssembly().Location;
            f = Regex.Replace(f, @"\bautoPanel.exe\b", "", RegexOptions.IgnoreCase);
            // MessageBox.Show(f + "Resources\\layout.dxf");
            //  try
            //  {

            dxf = netDxf.DxfDocument.Load(f + "Resources\\layout.dxf");
            // dxf = netDxf.DxfDocument.Load("/Resources/layout.dxf");
            DXF.imageOut(ms.image, dxf, ms.scaleMM);
            if ((arrayList.SelectedItems.Count > 0) && boolo == false)
            {

                foreach (PanelTest pt in arrayList.Items)
                {
                    dxf = DXF.DrawPanels(pt, dxf, ms.scaleMM);

                }

            }

            netDxf.Objects.Layout L = dxf.Layouts["PV LAYOUT"];
            List<DxfObject> entities = dxf.Layouts.GetReferences(L);
            dxf.ActiveLayout = "PV LAYOUT";


            foreach (DxfObject o in entities)
            {

                Viewport entity = o as Viewport;

                ////  try
                {
                    if (entity != null)
                    {



                        if ((image.Width / image.Height) > (entity.Width / entity.Height))
                        {
                            viewScale = (image.Height * ms.scaleMM) / entity.Height;
                        }

                        else
                        {
                            viewScale = (image.Width * ms.scaleMM) / entity.Width;
                        }
                        newScale = 5000;
                        while (newScale > viewScale)
                        {
                            if (newScale > 500)
                            {
                                newScale -= 100;
                            }
                            else
                            {
                                newScale -= 50;
                            }
                        }
                        entity.ViewHeight = entity.Height * newScale;






                        Vector2 veccy = new Vector2(image.Width * ms.scaleMM / 2, image.Height * ms.scaleMM / 2);
                        entity.ViewCenter = veccy;
                    }
                }

            }










            foreach (DxfObject o in entities)
            {

                Insert entity = o as Insert;

                ////  try
                {
                    if (entity != null)
                    {
                        if (entity.Block.Name == "VERDIA TITLE BLOCK A3E")
                        {
                            DateTime localDate = DateTime.Now;

                            foreach (var lay in entity.Attributes)
                            {
                                Debug.WriteLine(lay.Tag);
                                switch (lay.Tag)
                                {
                                    case "SCALE":
                                        lay.Value = "1:" + newScale.ToString();
                                        break;
                                    case "CADMAN":
                                        lay.Value = dName.Text.ToUpper();
                                        break;
                                    case "ABY":
                                        lay.Value = dInitials.Text.ToUpper();
                                        break;
                                    case "ENGINEER":
                                        lay.Value = cName.Text.ToUpper();
                                        break;
                                    case "SUPERVISOR":
                                        lay.Value = aName.Text.ToUpper();
                                        break;
                                    case "AAPP":
                                        lay.Value = aInitials.Text.ToUpper();
                                        break;
                                    case "ABY_DATE":

                                        lay.Value = localDate.ToString("dd'/'MM'/'yy");
                                        break;
                                    case "CBY_DATE":

                                        lay.Value = localDate.ToString("dd'/'MM'/'yy");
                                        break;
                                    case "DBY_DATE":

                                        lay.Value = localDate.ToString("dd'/'MM'/'yy");
                                        break;
                                    case "A":

                                        lay.Value = localDate.ToString("dd'/'MM'/'yy");
                                        break;
                                    case "DRAWING":
                                        if (opID.Text == "")
                                        {
                                            lay.Value = "XXXX-XXXX-X-XX-L01";
                                        }
                                        else
                                        {
                                            lay.Value = opID.Text + "-L01";
                                        }
                                        break;
                                    case "JOB":
                                        if (opID.Text == "")
                                        {
                                            lay.Value = "XXXX-XXXX-X-XX";
                                        }
                                        else
                                        {
                                            lay.Value = opID.Text;
                                        }
                                        break;
                                    case "DXF":
                                        if (opID.Text == "")
                                        {
                                            lay.Value = "XXXX-XXXX-X-XX-L01.DXF";
                                        }
                                        else
                                        {
                                            lay.Value = opID.Text + "-L01.dxf";
                                        }
                                        break;
                                    case "TITLE":

                                        lay.Value = "PV SYSTEM LAYOUT - " + TotalPower(0);
                                        break;
                                    case "PROJECT":

                                        lay.Value = client.Text.ToUpper();
                                        break;
                                    case "ADDRESS":

                                        lay.Value = address.Text.ToUpper();
                                        break;
                                }

                            }
                        }
                    }
                }
                //  catch { }
                MText mText = o as MText;

                try
                {


                    Debug.WriteLine(mText.Value);
                    //mText.Value = @"{\LGENERAL NOTES\P\l\P\pxi - 3,l3,t3; 1.REFERENCE PV MODULES ARE 72 - CELL MONOCRYSTALLINE 345W.\P2.TOTAL NUMBER OF PANELS = 4343.\P3.TOTAL PV CAPACITY SHOWN = 191.32 kWp.\P4.ALL PANELS TO BE FLUSH MOUNTED.\P5.FOR INFORMATION ONLY, NOT FOR CONSTRUCTION PURPOSES.}";
                    string s = mText.Value;
                    // MessageBox.Show(s);
                    string power = lp[0].pVmodule.Name + ".";
                    s = Regex.Replace(s, @"\bPV_MODULE\b", power, RegexOptions.IgnoreCase);

                    // MessageBox.Show(s);
                    power = TotalPanels() + ".";
                    s = Regex.Replace(s, @"\bMODULE_COUNT\b", power, RegexOptions.IgnoreCase);

                    power = TotalPower(2);
                    s = Regex.Replace(s, @"\bARRAY_POWER\b", power, RegexOptions.IgnoreCase);


                    power = TiltOrFlush();
                    s = Regex.Replace(s, @"\bMOUNTING\b", power, RegexOptions.IgnoreCase);
                    mText.Value = s;




                }
                catch { }
            }












            L = dxf.Layouts["PRELIM LAYOUT"];
            entities = dxf.Layouts.GetReferences(L);
            dxf.ActiveLayout = "PRELIM LAYOUT";


            foreach (DxfObject o in entities)
            {

                Viewport entity = o as Viewport;

                ////  try
                {
                    if (entity != null)
                    {



                        if ((image.Width / image.Height) > (entity.Width / entity.Height))
                        {
                            viewScale = (image.Height * ms.scaleMM) / entity.Height;
                        }

                        else
                        {
                            viewScale = (image.Width * ms.scaleMM) / entity.Width;
                        }
                        newScale = 5000;
                        while (newScale > viewScale)
                        {
                            if (newScale > 500)
                            {
                                newScale -= 100;
                            }
                            else
                            {
                                newScale -= 50;
                            }
                        }
                        entity.ViewHeight = entity.Height * newScale;






                        Vector2 veccy = new Vector2(-image.Height * ms.scaleMM / 2, image.Width * ms.scaleMM / 2);
                        entity.ViewCenter = veccy;
                    }
                }

            }










            foreach (DxfObject o in entities)
            {


                MText mText = o as MText;

                try
                {


                    Debug.WriteLine(mText.Value);
                    //mText.Value = @"{\LGENERAL NOTES\P\l\P\pxi - 3,l3,t3; 1.REFERENCE PV MODULES ARE 72 - CELL MONOCRYSTALLINE 345W.\P2.TOTAL NUMBER OF PANELS = 4343.\P3.TOTAL PV CAPACITY SHOWN = 191.32 kWp.\P4.ALL PANELS TO BE FLUSH MOUNTED.\P5.FOR INFORMATION ONLY, NOT FOR CONSTRUCTION PURPOSES.}";
                    string s = mText.Value;
                    // MessageBox.Show(s);
                    string power = lp[0].pVmodule.Name + ".";
                    s = Regex.Replace(s, @"\bPV_MODULE\b", power, RegexOptions.IgnoreCase);

                    // MessageBox.Show(s);
                    power = TotalPanels() + ".";
                    s = Regex.Replace(s, @"\bMODULE_COUNT\b", power, RegexOptions.IgnoreCase);

                    power = TotalPower(2);
                    s = Regex.Replace(s, @"\bARRAY_POWER\b", power, RegexOptions.IgnoreCase);


                    power = TiltOrFlush();
                    s = Regex.Replace(s, @"\bMOUNTING\b", power, RegexOptions.IgnoreCase);
                    mText.Value = s;

                    power = address.Text.ToUpper();
                    s = Regex.Replace(s, @"\bSITE_ADDRESS\b", power, RegexOptions.IgnoreCase);
                    mText.Value = s;


                }
                catch { }
            }


















            //   }
            //  catch
            //  {
            //  MessageBox.Show("Changing array settings failed");
            //  }



            //loaded = DxfDocument.Load(template);
            // MessageBox.Show(filename);

            /*
               foreach (PanelTest pt in lp)
               {
                 //  pt.DrawPanels(loaded, pt.points[pt.lastpanelSet]);
               }

               */


            try
            {

                L = dxf.Layouts["SITE LAYOUT"];
                entities = dxf.Layouts.GetReferences(L);
                dxf.ActiveLayout = "SITE LAYOUT";


                foreach (DxfObject o in entities)
                {

                    Viewport entity = o as Viewport;

                    ////  try
                    {
                        if (entity != null)
                        {



                            if ((image.Width / image.Height) > (entity.Width / entity.Height))
                            {
                                viewScale = (image.Height * ms.scaleMM) / entity.Height;
                            }

                            else
                            {
                                viewScale = (image.Width * ms.scaleMM) / entity.Width;
                            }
                            newScale = 5000;
                            while (newScale > viewScale)
                            {


                                newScale -= 50;

                            }
                            entity.ViewHeight = entity.Height * newScale;






                            Vector2 veccy = new Vector2(image.Height * ms.scaleMM / 2, -image.Width * ms.scaleMM / 2);
                            entity.ViewCenter = veccy;
                        }
                    }

                }









                string s;
                if (opID.Text == "")
                {
                    s = TitleName.Text + " - PV LAYOUT.DXF";
                }
                else
                {
                    s = opID.Text + "-L01 PV LAYOUT.dxf";
                }
                Microsoft.Win32.SaveFileDialog dlg1 = new Microsoft.Win32.SaveFileDialog
                {

                    // Set filter for file extension and default file extension 
                    DefaultExt = "*.dxf",
                    Filter = "AutoCAD Files (*.dxf)|*.dxf",

                    FileName = s,
                };


                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = dlg1.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    file = dlg1.FileName;
                    //MessageBox.Show(filename.ToString());
                    dlg1 = null;
                }



                dxf.Save(file);

            }
            catch
            {
                MessageBox.Show("ERROR SAVING FILE");
            }
            dxf = null;
        }
        private void saveImage(string FileName)
        {
            ms.path = System.IO.Path.GetDirectoryName(FileName);
            ms.title = System.IO.Path.GetFileNameWithoutExtension(FileName);
            ms.image = ms.path + "/" + ms.title + ms.imageEx;

            using (FileStream fileStream = new FileStream(ms.image, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));

                encoder.QualityLevel = 100;
                encoder.Save(fileStream);
                TitleName.Text = ms.title;


            }
        }



        private void saveImagePVsyst()
        {

            var v = pvsystPath + @"UserImages\" + ms.title + ms.imageEx;

            using (FileStream fileStream = new FileStream(v, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));

                encoder.QualityLevel = 50;
                encoder.Save(fileStream);



            }
        }
        void CalcPower()
        {
            int selectedItems = arrayList.SelectedItems.Count;
            int count = 0;
            double power = 0;
            try
            {
                if ((selectedItems > 0))
                {
                    foreach (PanelTest pt in arrayList.SelectedItems)
                    {
                        count += pt.PanelLayoutPoints.Count();

                        power += pt.PanelLayoutPoints.Count() * pt.pVmodule.PanelWatts / 1000;
                    }
                }
                if (power < 1000)
                {
                    WattBox.Text = count + " Panels    " + (Math.Round(power, 2)).ToString() + " kWp";
                }
                else
                {

                    WattBox.Text = count + " Panels    " + (Math.Round(power / 1000, 2)).ToString() + " MWp";
                }
            }
            catch
            {

            }
        }

        String TotalPower(int i)
        {


            double power = 0;
            string total = "0 kWp";
            try
            {

                foreach (PanelTest pt in arrayList.Items)
                {

                    power += pt.PanelLayoutPoints.Count() * pt.pVmodule.PanelWatts / 1000;
                }


                total = (Math.Round(power, i)).ToString() + " kWp";

            }
            catch
            {

            }
            return total;
        }

        String TotalPanels()
        {

            int count = 0;

            string total = "0";
            try
            {

                foreach (PanelTest pt in arrayList.Items)
                {
                    count += pt.PanelLayoutPoints.Count();

                }


                total = count.ToString();

            }
            catch
            {

            }
            return total;
        }
        String TiltOrFlush()
        {

            bool tilt = false;
            bool flush = false;

            string total = "0";
            try
            {

                foreach (PanelTest pt in arrayList.Items)
                {
                    if (pt.arraySettings.RoofPitch < pt.arraySettings.ArrayPitch)
                    {
                        tilt = true;
                    }
                    else
                    {
                        flush = true;
                    }

                }

                if ((tilt == true) && (flush == true))
                {
                    total = "PANELS TO BE A COMBINATION OF TILT AND FLUSH MOUNTED.";
                }
                else if (tilt == true)
                {
                    total = "ALL PANELS TO BE TILT MOUNTED.";
                }
                else
                {
                    total = "ALL PANELS TO BE FLUSH MOUNTED.";
                }

            }
            catch
            {

            }
            return total;
        }


        #endregion

        private void Animation_Checked(object sender, RoutedEventArgs e)
        {

            pauseAnimation = animation.IsChecked;

        }
        private void Animation_Unchecked(object sender, RoutedEventArgs e)
        {
            pauseAnimation = animation.IsChecked;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DxfDocument load = DxfDocument.Load(@"C:\Users\ajbol\Documents\dxf.dxf");




            ShapeMods.pLineVisibility(TheCanvas, Visibility.Hidden);
            BitmapImage piccy = LayoutImage(TheCanvas);
            ShapeMods.pLineVisibility(TheCanvas, Visibility.Visible);


            MessageBox.Show(image.Height.ToString());
            MessageBox.Show(piccy.Height.ToString());
            Layout layout = new Layout(TheCanvas, piccy);

            if ((bool)layout.ShowDialog())
            {

            }

            {

            }
        }


        private void OrientationBox_DropDownClosed(object sender, EventArgs e)
        {
            int SelectedCount = arrayList.SelectedItems.Count;
            bool orientation = false;
            if ((SelectedCount > 0) && (boolo == false))
            {
                PVmodule pv = PanelTypes.SelectedItem as PVmodule;
                if (OrientationBox.SelectedIndex == 1)
                {
                    orientation = true;
                    pv.OrientationIndex = 1;
                }
                else
                {
                    pv.OrientationIndex = 0;
                }
                pv.PanelOrientation = orientation;




                foreach (PanelTest pt in arrayList.SelectedItems)
                {

                    pt.ChangePanel(pv, orientation);
                    pt.pVmodule.PanelIndex = PanelTypes.SelectedIndex;

                    double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                    rowSpacing.Text = d.ToString();


                    if (SpacingCheck.IsChecked == true)
                    {
                        pt.arraySettings.GapBetweenPanelsDown = d;
                        // GapDown.Text = rowSpacing.Text;
                    }

                    //  pt.arraySettings.GapBetweenPanelsDown = Convert.ToDouble(GapDown.Text);

                }
                foreach (PanelTest pt in arrayList.SelectedItems)
                {

                    //  MessageBox.Show("NEW VALUE: " + pt.arraySettings.GapBetweenPanelsDown.ToString());
                }

            }

        }

        private void OrientationBox_DropDownClosed_1(object sender, EventArgs e)
        {
            int SelectedCount = arrayList.SelectedItems.Count;
            bool orientation = false;
            if ((SelectedCount > 0) && (boolo == false))
            {
                PVmodule pv = PanelTypes.SelectedItem as PVmodule;
                if (OrientationBox.SelectedIndex == 1)
                {
                    orientation = true;
                    pv.OrientationIndex = 1;
                }
                else
                {
                    pv.OrientationIndex = 0;
                }
                pv.PanelOrientation = orientation;




                foreach (PanelTest pt in arrayList.SelectedItems)
                {

                    pt.ChangePanel(pv, orientation);
                    pt.pVmodule.PanelIndex = PanelTypes.SelectedIndex;

                    double d = Latitude.Spacing(pt.pVmodule.PanelWidthReal, pt.arraySettings.RoofPitch, pt.arraySettings.ArrayPitch, pt.arraySettings.Azumuth, ms.latitude);

                    rowSpacing.Text = d.ToString();


                    if (SpacingCheck.IsChecked == true)
                    {
                        pt.arraySettings.GapBetweenPanelsDown = d;
                        GapDown.Text = rowSpacing.Text;
                    }
                }




            }
        }

        private void RowSpacing_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PanelImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelImageSelection = panelImage.SelectedIndex;
            ShapeMods.RemoveAllPgons(TheCanvas);

            foreach (PanelTest pt in lp)
            {



            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            //  try
            {
                List<string> lines = new List<string>();
                string str = "PVObject_=pvShading";
                string temp;
                int objCount = arrayList.Items.Count + 1;
                int objIndex = 1;
                lines.Add(str);
                str = "Comment=New shading scene";
                lines.Add(str);
                str = "Version=6.78";
                lines.Add(str);
                str = "Flags=$00";
                lines.Add(str);
                str = "NOrient_Shd=0";
                lines.Add(str);
                str = "Precision=1";
                lines.Add(str);
                str = "FracOmbreMod=1.000";
                lines.Add(str);
                str = "FracOmbreThin=0.400";
                lines.Add(str);
                str = "GCROffset=1.0";
                lines.Add(str);
                str = "";
                lines.Add(str);
                str = "ListeObjets, list of=" + objCount + " TShdObject";
                lines.Add(str);


                str = "PVObject_1=pvShdGroundImage" + Environment.NewLine + "Comment = Aerial Image" + Environment.NewLine + "Version = 6.78" + Environment.NewLine + "Flags =$00200000" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "ColorSurf =$C0FF" + Environment.NewLine + "DOrigine = 0.000,0.000,0.000" + Environment.NewLine + "DInclin = 0.0" + Environment.NewLine + "Opacity = 255";
                lines.Add(str);
                temp = ms.title + ms.imageEx;
                str = "Filename=" + temp + Environment.NewLine + "FilenameCopied =" + temp + Environment.NewLine + "TransformOn = True" + Environment.NewLine + "X1 = 0.0,0.0,0.0";
                lines.Add(str);
                temp = (image.Width * ms.scale).ToString();
                str = "X2=" + temp + ",0.0,0.0" + Environment.NewLine + "Distance =" + temp + Environment.NewLine + "ImageBaseWidth =" + temp + Environment.NewLine + "ImageFileExists = True" + Environment.NewLine + "End of PVObject pvShdGroundImage";
                lines.Add(str);

                int x = 2;
                pvIndex = 2;
                foreach (object o in arrayList.Items)
                {
                    PanelTest pt = o as PanelTest;
                    List<string> stringList = new List<string>();
                    stringList = PvsystArray3(x, pt);

                    foreach (string s in stringList)
                    {
                        lines.Add(s);
                    }




                }


                #region plane
                /*
            foreach (List<Point3d> pc in roofSections)
            {
                double X = pc[0].X;
                double Y = pc[0].Y;
                string faces = "Faces = -1,";
                int f = 0;
                double azu = (Math.Atan((pc[1].Y - pc[0].Y) / (pc[1].X - pc[0].X)))/Math.PI*180;
                MessageBox.Show(azu.ToString());
                double inc = Convert.ToDouble(p8.Text);
                Point orig = new Point(Math.Round(pc[0].X*ms.scale,3), Math.Round((pc[0].Y-image.Height) * ms.scale, 3));
                str = "PVObject_1 = pvShdGenObject" + Environment.NewLine + "Comment = Object 1" + Environment.NewLine + "Version = 6.85" + Environment.NewLine + "Flags =$02200002" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "NPElements = 5" + Environment.NewLine + "NElements = 1" + Environment.NewLine + "ColorSurf =$00FFFFFF" + Environment.NewLine + "DOrigine =" + orig.X.ToString() + "," + orig.Y.ToString() + ",5.000";
                lines.Add(str);
                str = "DInclin = "+ inc.ToString() + Environment.NewLine + "DAzimut=" + azu.ToString()+ Environment.NewLine + "Sommets, TPoly3D = " + pc.Count.ToString() + " Point3D";
           lines.Add(str);
                int t = 1;
                double correctionFactor = 1 / Math.Cos(inc / 180 * Math.PI);
                foreach (Point3d p3 in pc)
                {
                    Debug.WriteLine("a: " + (p3.X - pc[0].X).ToString() + " a: " + (p3.Y - pc[0].Y).ToString());


                    Point OldPoint = new Point(p3.X-pc[0].X , p3.Y - pc[0].Y);
                Point newPoint = RotatePoint(OldPoint, -(azu/180*Math.PI) );
                    Debug.WriteLine("a: " + (p3.X - pc[0].X).ToString() + " a: " + (p3.Y - pc[0].Y).ToString());
                    Debug.WriteLine("b: " + (newPoint.X).ToString() + " b: " + (newPoint.Y).ToString());

                    Point3d p3new = new Point3d();
                    p3new.X = (float)newPoint.X;
                    p3new.Y = (float)(newPoint.Y * correctionFactor);
                    p3new.Z = p3.Z;
                    faces += f.ToString() + ",";
                    f++;
                    str = "Point_" + t.ToString() + " = " + p3new.X * ms.scale + "," + (p3new.Y ) * ms.scale + ","+ p3new.Z ;
                    lines.Add(str);
                    t++;
                }

                str = "End of TPoly3D"; 
                         lines.Add(str);
                str = faces;
                     lines.Add(str);
                str = "End of PVObject pvShdGenObject";
                         lines.Add(str);

            }
            */
                #endregion

                #region tilt building
                foreach (List<Point3d> pc in roofSections)
                {
                    double X = pc[0].X;
                    double Y = pc[0].Y;

                    double azu = (Math.Atan((pc[1].Y - pc[0].Y) / (pc[1].X - pc[0].X))) / Math.PI * 180;
                    if (((pc[1].X - pc[0].X) > 0) && ((pc[1].X - pc[0].X) > 0))
                    {
                        azu += 180;
                    }
                    azu -= 90;
                    double inc = Convert.ToDouble(p8.Text);
                    Point orig = new Point(Math.Round(pc[0].X * ms.scale, 3), Math.Round((pc[0].Y - image.Height) * ms.scale, 3));
                    str = "PVObject_1 = pvShdElemObject" + Environment.NewLine + "Comment = asymetric roof" + Environment.NewLine + "Version = 6.85" + Environment.NewLine + "Flags =$02200000" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "ColorSurf =$00B3E4FF" + Environment.NewLine + "ColorBord =$0002409B" + Environment.NewLine + "DOrigine =" + orig.X.ToString() + "," + orig.Y.ToString() + ",0.000";
                    lines.Add(str);
                    str = "DInclin = 0.0" + Environment.NewLine + "DAzimut=" + azu.ToString() + Environment.NewLine + "ObjetElemType=oeAsymHouse";
                    lines.Add(str);
                    int t = 1;
                    double correctionFactor = 1 / Math.Cos(inc / 180 * Math.PI);


                    double length1 = Math.Sqrt(((pc[1].X - pc[0].X) * (pc[1].X - pc[0].X)) + ((pc[1].Y - pc[0].Y) * (pc[1].Y - pc[0].Y)));
                    double length2 = Math.Sqrt(((pc[2].X - pc[1].X) * (pc[2].X - pc[1].X)) + ((pc[2].Y - pc[1].Y) * (pc[2].Y - pc[1].Y)));
                    str = "Params =" + (length2 * ms.scale).ToString() + "," + (-length1 * ms.scale).ToString() + ",5," + p8.Text + ",0.00,1.00,";
                    lines.Add(str);



                    lines.Add(str);
                    str = "End of PVObject pvShdElemObject";
                    lines.Add(str);

                }

                #endregion
                #region House building
                foreach (List<Point3d> pc in roofSections)
                {
                    double X = pc[0].X;
                    double Y = pc[0].Y;

                    double azu = (Math.Atan((pc[1].Y - pc[0].Y) / (pc[1].X - pc[0].X))) / Math.PI * 180;
                    if (((pc[1].X - pc[0].X) > 0) && ((pc[1].X - pc[0].X) > 0))
                    {
                        azu += 180;
                    }
                    azu -= 90;
                    double inc = Convert.ToDouble(p8.Text);
                    Point orig = new Point(Math.Round(pc[0].X * ms.scale, 3), Math.Round((pc[0].Y - image.Height) * ms.scale, 3));
                    str = "PVObject_1 = pvShdElemObject" + Environment.NewLine + "Comment = house roof" + Environment.NewLine + "Version = 6.85" + Environment.NewLine + "Flags =$02200000" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "ColorSurf =$00B3E4FF" + Environment.NewLine + "ColorBord =$0002409B" + Environment.NewLine + "DOrigine =" + orig.X.ToString() + "," + orig.Y.ToString() + ",0.000";
                    lines.Add(str);
                    str = "DInclin = 0.0" + Environment.NewLine + "DAzimut=" + azu.ToString() + Environment.NewLine + "ObjetElemType=oeHouse";
                    lines.Add(str);
                    int t = 1;
                    double correctionFactor = 1 / Math.Cos(inc / 180 * Math.PI);


                    double length1 = Math.Sqrt(((pc[1].X - pc[0].X) * (pc[1].X - pc[0].X)) + ((pc[1].Y - pc[0].Y) * (pc[1].Y - pc[0].Y)));
                    double length2 = Math.Sqrt(((pc[2].X - pc[1].X) * (pc[2].X - pc[1].X)) + ((pc[2].Y - pc[1].Y) * (pc[2].Y - pc[1].Y)));
                    str = "Params =" + (length2 * ms.scale).ToString() + "," + (-length1 * ms.scale).ToString() + ",5," + p8.Text + ",0.00,0.00,";
                    lines.Add(str);



                    lines.Add(str);
                    str = "End of PVObject pvShdElemObject";
                    lines.Add(str);

                }

                #endregion




                str = "End of ListeObjets" + Environment.NewLine + "End of PVObject pvShading";
                lines.Add(str);
                //  MessageBox.Show(pvsystPath);
                System.IO.File.WriteAllLines(@pvsystPath + @"Shadings\" + ms.title + ".SHD", lines);

                saveImagePVsyst();
                // Debug.WriteLine("Start Shade");



                PanelTest pt1 = arrayList.SelectedItem as PanelTest;
                // Debug.WriteLine(pt1.boundary.Count.ToString());
                // foreach (Point p in pt1.boundary)
                {
                    ///     Debug.WriteLine("X: " + (p.X - pt1.boundary[0].X) * ms.scale + " Y: " + (p.Y - pt1.boundary[0].Y) * ms.scale);

                }



            }
            // catch
            {
                //    MessageBox.Show("No array selected");
            }
        }

        /*        private string PvsystArray(int index, PanelTest pt)
                 {
                     Debug.Print("Az angle: " + pt.arraySettings.Azumuth.ToString());
                     double xOffset = pt.boundary[0].X * ms.scale;
                     double yOffset = pt.boundary[0].Y * ms.scale;
                     double slopeCorrection = 1 / (Math.Cos(pt.arraySettings.ArrayPitch / 180 * Math.PI));
                     Point p = new Point();
                     Point pOld = new Point();
                     Double azimuth = 0 + (pt.arraySettings.Azumuth / Math.PI * 180);
                     double yOffset2 = pt.boundary[0].Y * ms.scale - (image.Height * ms.scale);
                     String lines = "PVObject_" + index + "=pvShdArrayPoly" + Environment.NewLine + "Version = 6.78" + Environment.NewLine + "Flags =$02200000" + Environment.NewLine + "NoObjCreated = 2" + Environment.NewLine + "ColorTechn =$00FF0000" + Environment.NewLine + "ColorSurf =$00FF9999" + Environment.NewLine + "ColorBord =$00505050" + Environment.NewLine;
                     lines += "DOrigine=" + xOffset.ToString() + "," + yOffset2.ToString() + ",10.000" + Environment.NewLine;
                     lines += "DInclin=" + (pt.arraySettings.ArrayPitch).ToString() + Environment.NewLine + "DAzimut=" + azimuth.ToString() + Environment.NewLine + "NElemChamps = 1" + Environment.NewLine + "NoOrient = -1" + Environment.NewLine + "CellMargin = 0.150" + Environment.NewLine + "Sommets2D, TPoly2D =" + (pt.boundary.Count - 1).ToString() + " Point2D" + Environment.NewLine;
                     lines += "Point_1=0.00,0.00" + Environment.NewLine;

                     for (int i = 2; i < pt.boundary.Count; i++)
                     {
                         pOld.X = ((pt.boundary[i - 1].X * ms.scale) - xOffset);
                         pOld.Y = ((pt.boundary[i - 1].Y * ms.scale) - yOffset);
                         p = RotatePoint(pOld, -pt.arraySettings.Azumuth + Math.PI);
                         // x = (pt.boundary[i - 1].X * ms.scale)-xOffset)-(pt.boundary[i - 1].X * ms.scale) - xOffset)
                         lines += "Point_" + i.ToString() + "=" + ((p.X)).ToString() + "," + ((p.Y* slopeCorrection)).ToString() + Environment.NewLine;

                         Debug.Print("XXXXX: " + p.X.ToString());
                         Debug.Print("old x: " + ((pt.boundary[i - 1].X * ms.scale) - xOffset).ToString());
                         Debug.Print("old y: " + ((pt.boundary[i - 1].Y * ms.scale) - yOffset).ToString());
                         Debug.Print("new x: " + (p.X * ms.scale).ToString());
                         Debug.Print("new y: " + (p.Y * ms.scale).ToString());
                         //lines += "Point_" + i.ToString() + "=" + ((pt.boundary[i - 1].X * ms.scale) - xOffset).ToString() + "," + ((pt.boundary[i - 1].Y * ms.scale) - yOffset).ToString() + Environment.NewLine;

                     }







                 lines += "End of TPoly2D" + Environment.NewLine + "ModuleList, list of = 1 tModule" + Environment.NewLine + "Module_1=0,1,1,0,0,0,0,0,0,0.013,0.013,$00FFFFFF, False," + Environment.NewLine + "End of List ModuleList" + Environment.NewLine + "End of PVObject pvShdArrayPoly";


                     return lines;





                 }

             */
        private string PvsystArray4(int index, PanelTest pt, List<Point> pc, Point origin)
        {

            double slopeCorrection = 1 / (Math.Cos(pt.arraySettings.ArrayPitch / 180 * Math.PI));

            Double azimuth = 0 + (pt.arraySettings.Azumuth / Math.PI * 180);

            String lines = "";
            double width = pt.pVmodule.PanelWide / 1000;
            double length = pt.pVmodule.PanelLong / 1000;
            string orientation = "moPortrait";
            double tilt = Math.Round(pt.arraySettings.ArrayPitch, 3);

            int pointCount = pc.Count;
            try
            {
                // MessageBox.Show(width.ToString() + " " + length.ToString());

                if (pt.pVmodule.PanelOrientation == true)
                {
                    orientation = "moLandScape";
                    length = pt.pVmodule.PanelWide / 1000;
                    width = pt.pVmodule.PanelLong / 1000;
                }
                double drop = 10 - (pc[0].Y * Math.Tan((pt.arraySettings.ArrayPitch - pt.arraySettings.RoofPitch) / 180 * Math.PI));
                drop = Math.Round(drop, 3);

                double z = 0;



                lines += "PVObject_" + pvIndex.ToString() + "=pvShdArrayRect" + Environment.NewLine + "Version = 6.78" + Environment.NewLine + "Flags =$02200024" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "NPElements=" + (pointCount * 5).ToString() + Environment.NewLine + " NPElemRes = " + (pointCount * 5).ToString() + Environment.NewLine + "NElements =" + pointCount.ToString() + Environment.NewLine + "NElemRes =" + pointCount.ToString() + Environment.NewLine + "ColorTechn =$00FF0000" + Environment.NewLine + "ColorSurf =$00FF9999" + Environment.NewLine + "ColorBord =$00505050" + Environment.NewLine + "DOrigine=";

                lines += (origin.X * ms.scale).ToString() + "," + ((origin.Y - image.Height) * ms.scale).ToString() + "," + drop.ToString() + Environment.NewLine;

                lines += "DInclin=" + tilt.ToString() + Environment.NewLine + "DAzimut=" + azimuth.ToString() + Environment.NewLine + "NElemChamps =" + pointCount.ToString() + Environment.NewLine + "NoOrient = -1" + Environment.NewLine + "PanneauPVName=JAM6-72-325/SI;0.991#1.956" + Environment.NewLine + "ModSpacingX=0.0" + Environment.NewLine + "ModSpacingY=0.0" + Environment.NewLine + "NModInHeight=1" + Environment.NewLine + "NModInWidth=1" + Environment.NewLine + "ModuleOrient=" + orientation + Environment.NewLine;
                string longA = "LongA=";
                string largA = "LargA=";
                string xOrig = "XOrig=";
                string yOrig = "YOrig=";

                foreach (Point point in pc)
                {
                    longA += width.ToString() + ",";
                    largA += length.ToString() + ",";

                    xOrig += point.X.ToString() + ",";

                    yOrig += point.Y.ToString() + ",";
                }

                lines += longA + Environment.NewLine + largA + Environment.NewLine + xOrig + Environment.NewLine + yOrig + Environment.NewLine + "TypeOrigine=1" + Environment.NewLine + "End of PVObject pvShdArrayRect" + Environment.NewLine;
                pvIndex += 1;

            }
            catch { }
            return lines;


        }



        public static List<List<Point>> Split(List<Point> source, int size)
        {
            // TODO: Prepopulate with the right capacity
            List<List<Point>> ret = new List<List<Point>>();
            for (int i = 0; i < source.Count; i += size)
            {
                ret.Add(source.GetRange(i, Math.Min(size, source.Count - i)));
            }
            return ret;
        }

        private List<string> PvsystArray3(int index, PanelTest pt)
        {
            String lines = "";
            List<string> stringList = new List<string>();
            try
            {

                double slopeCorrection = 1 / (Math.Cos(pt.arraySettings.ArrayPitch / 180 * Math.PI));

                Double azimuth = 0 + (pt.arraySettings.Azumuth / Math.PI * 180);
                double yOffset2 = pt.boundary[0].Y * ms.scale - (image.Height * ms.scale);
                PointCollection pc = new PointCollection();
                foreach (PanelPoint p in pt.PanelLayoutPoints[pt.SelectedIndex].set)
                {
                    pc.Add(p.point);
                }
                //MessageBox.Show(pc.Count.ToString());
                Point origin = pc[0];


                int pointCount = pc.Count;

                double width = pt.pVmodule.PanelWide / 1000;
                double length = pt.pVmodule.PanelLong / 1000;



                // MessageBox.Show(width.ToString() + " " + length.ToString());

                if (pt.pVmodule.PanelOrientation == true)
                {

                    length = pt.pVmodule.PanelWide / 1000;
                    width = pt.pVmodule.PanelLong / 1000;
                }
                List<Point> correctedPoints = new List<Point>();
                List<double> Ys = new List<double>();
                foreach (Point pointy in pc)
                {
                    Point Oldpoint = new Point();
                    Oldpoint.X = pointy.X - pc[0].X;
                    Oldpoint.Y = pointy.Y - pc[0].Y;
                    Point newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);
                    double doub = Math.Round((newPoint.X * ms.scale) - width, 3);
                    newPoint.X = doub;
                    doub = Math.Round((newPoint.Y * ms.scale * slopeCorrection) - length, 3);
                    newPoint.Y = doub;
                    correctedPoints.Add(newPoint);
                    Ys.Add(newPoint.Y);
                }




                List<PointCollection> points = new List<PointCollection>();

                List<Point> points1 = new List<Point>();
                IEnumerable<double> referenceYs = correctedPoints.Select(x => x.Y).Distinct();

                foreach (double d in referenceYs)
                {
                    points1 = new List<Point>();


                    foreach (Point pp in correctedPoints)
                    {
                        if (pp.Y == d)
                        {
                            points1.Add(pp);


                        }

                    }


                    if (points1.Count > 35)
                    {
                        List<List<Point>> v = Split(points1, 35);

                        foreach (List<Point> vv in v)
                        {
                            lines = PvsystArray4(index, pt, vv, origin);
                            stringList.Add(lines);
                        }
                    }
                    else
                    {
                        //  points.Add(points1);
                        // MessageBox.Show(points1.Count.ToString());
                        lines = PvsystArray4(index, pt, points1, origin);
                        stringList.Add(lines);
                    }
                }


            }
            catch { }



            return stringList;


        }



        /*

        private string PvsystArray1(int index, PanelTest pt)
        {
            Debug.Print("Az angle: " + pt.arraySettings.Azumuth.ToString());
            double xOffset = pt.boundary[0].X * ms.scale;
            double yOffset = pt.boundary[0].Y * ms.scale;
            double slopeCorrection = 1 / (Math.Cos(pt.arraySettings.ArrayPitch / 180 * Math.PI));
            Point p = new Point();
            Point pOld = new Point();
            Double azimuth = 0 + (pt.arraySettings.Azumuth / Math.PI * 180);
            double yOffset2 = pt.boundary[0].Y * ms.scale - (image.Height * ms.scale);
            String lines = "";
            PointCollection pc = pt.panelList[pt.SelectedIndex].points;
           


                int pointCount = pc.Count;
           





                lines += "PVObject_" + pvIndex.ToString() + "=pvShdArrayRect" + Environment.NewLine + "Version = 6.78" + Environment.NewLine + "Flags =$02200000" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "NPElements="+ (pointCount*5).ToString() + Environment.NewLine + " NPElemRes = "+ (pointCount * 5).ToString() + Environment.NewLine + "NElements ="+ pointCount.ToString() + Environment.NewLine + "NElemRes ="+ pointCount.ToString() + Environment.NewLine + "ColorTechn =$00FF0000" + Environment.NewLine + "ColorSurf =$00FF9999" + Environment.NewLine + "ColorBord =$00505050" + Environment.NewLine + "DOrigine=";
               
                lines +=  (pc[0].X*ms.scale).ToString() + "," + ((pc[0].Y - image.Height)* ms.scale).ToString() + ",10.000" + Environment.NewLine;
                
            lines += "DInclin=" + (pt.arraySettings.ArrayPitch).ToString() + Environment.NewLine + "DAzimut=" + azimuth.ToString() + Environment.NewLine + "NElemChamps =" + pointCount.ToString() + Environment.NewLine + "NoOrient = -1" + Environment.NewLine + "PanneauPVName=JAM6-72-325/SI;0.991#1.956" + Environment.NewLine + "ModSpacingX=0.020" + Environment.NewLine + "ModSpacingY=0.020" + Environment.NewLine + "NModInHeight=1"  + Environment.NewLine + "NModInWidth=1" + Environment.NewLine + "ModuleOrient=moLandScape" + Environment.NewLine;
            string longA = "LongA=";
            string largA = "LargA=";
            string xOrig = "XOrig=";
            string yOrig = "YOrig=";

            foreach (Point point in pc)
            {
                longA += "1.976,";
                largA += "1.011,";
                Point Oldpoint = new Point();
                   Oldpoint.X = point.X -pc[0].X;
                Oldpoint.Y = point.Y - pc[0].Y;
                Point newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);
                double doub = Math.Round((newPoint.X * ms.scale) - 1.976, 3);
                xOrig +=   doub.ToString()+",";
                doub = Math.Round((newPoint.Y * ms.scale) - 1.011, 3);
                yOrig +=   doub.ToString()+",";
            }

            lines += longA + Environment.NewLine + largA + Environment.NewLine + xOrig + Environment.NewLine + yOrig + Environment.NewLine + "TypeOrigine=1" + Environment.NewLine + "End of PVObject pvShdArrayRect" + Environment.NewLine;
                pvIndex += 1;

            return lines;


        }


        private List<string> PvsystArray2(int index, PanelTest pt)
        {
            
            double xOffset = pt.boundary[0].X * ms.scale;
            double yOffset = pt.boundary[0].Y * ms.scale;
            double slopeCorrection = 1 / (Math.Cos(pt.arraySettings.ArrayPitch / 180 * Math.PI));
            Point p = new Point();
            Point pOld = new Point();
            Double azimuth = 0 + (pt.arraySettings.Azumuth / Math.PI * 180);
            double yOffset2 = pt.boundary[0].Y * ms.scale - (image.Height * ms.scale);
            String lines = "";
            PointCollection pc = pt.panelList[pt.SelectedIndex].points;


            List<string> stringList = new List<string>();
            int pointCount = pc.Count;
            int currentModule = 0;
            double previousX = 0;
            double newX = 0;
            int lineIndex = 0;
            int z = 0;
            Point point = pc[0];
            Point Oldpoint = new Point();
            Oldpoint.X = point.X - pc[0].X;
            Oldpoint.Y = point.Y - pc[0].Y;
            Point newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);
            previousX = Math.Round((newPoint.X * ms.scale) - 1.011, 3);
            newX = previousX;


            while (currentModule < pointCount - 1)
            {


                string longA = "LongA=";
                string largA = "LargA=";
                string xOrig = "XOrig=";
                string yOrig = "YOrig=";
                lineIndex = 0;
                Debug.WriteLine("currentModule " + currentModule.ToString());
                Debug.WriteLine("pointCount " + pointCount.ToString());
                point = pc[currentModule];
                Oldpoint.X = point.X - pc[0].X;
                Oldpoint.Y = point.Y - pc[0].Y;
                newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);
                previousX = Math.Round((newPoint.X * ms.scale) - 1.011, 3);
                newX = previousX;



                while (newX == previousX)
                {
                    Debug.WriteLine("Z: " + z.ToString());

                    longA += "1.976,";
                    largA += "1.011,";
                    if (currentModule < pc.Count)
                    {
                        MessageBox.Show("Current: " + currentModule.ToString() + " Count: " + pc.Count.ToString());
                        Debug.WriteLine("Current: " + currentModule.ToString());
                        z = 1;
                        Debug.WriteLine("Z: " + z.ToString());

                        Debug.WriteLine("Count: " + pc.Count.ToString());
                        point = pc[currentModule];
                        Oldpoint.X = point.X - pc[0].X;
                        Oldpoint.Y = point.Y - pc[0].Y;
                        newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);

                        Debug.WriteLine("Z: " + z.ToString());

                        double doub = Math.Round((newPoint.X * ms.scale) - 1.976, 3);
                        newX = Math.Round((newPoint.X * ms.scale) - 1.011, 3);

                        xOrig += doub.ToString() + ",";
                        doub = Math.Round((newPoint.Y * ms.scale) - 1.011, 3);
                        yOrig += doub.ToString() + ",";
                        z = 3;
                        Debug.WriteLine("Z: " + z.ToString());

                        previousX = newX;
                        currentModule++;
                        if (currentModule == pc.Count)
                        {
                            goto Outer;
                        }
                        z = 4;
                        Debug.WriteLine("Z: " + z.ToString());
                        point = pc[currentModule];
                        Oldpoint.X = point.X - pc[0].X;
                        Oldpoint.Y = point.Y - pc[0].Y;
                        newPoint = RotatePoint(Oldpoint, -pt.arraySettings.Azumuth + Math.PI);
                        newX = Math.Round((newPoint.X * ms.scale) - 1.011, 3);
                        lineIndex++;
                        z = 5;
                        Debug.WriteLine("Z: " + z.ToString());
                        Debug.WriteLine(newX.ToString());

                        Debug.WriteLine(previousX.ToString());
                    }

                    else
                    {
                        z = 6;

                        Debug.WriteLine("Zouter: " + z.ToString());
                        goto Outer;


                    }


                }
            // currentModule++;
            Outer:

                // Debug.WriteLine("Out of the loop" + z.ToString());
                if (lineIndex > 0)
                {
                    lines = "PVObject_" + pvIndex.ToString() + "=pvShdArrayRect" + Environment.NewLine + "Version = 6.78" + Environment.NewLine + "Flags =$02200000" + Environment.NewLine + "NoObjCreated = 1" + Environment.NewLine + "NPElements=" + (lineIndex * 5).ToString() + Environment.NewLine + " NPElemRes = " + (lineIndex * 5).ToString() + Environment.NewLine + "NElements =" + lineIndex.ToString() + Environment.NewLine + "NElemRes =" + lineIndex.ToString() + Environment.NewLine + "ColorTechn =$00FF0000" + Environment.NewLine + "ColorSurf =$00FF9999" + Environment.NewLine + "ColorBord =$00505050" + Environment.NewLine + "DOrigine=";


                    lines += (pc[0].X * ms.scale).ToString() + "," + ((pc[0].Y - image.Height) * ms.scale).ToString() + ",10.000" + Environment.NewLine;

                    lines += "DInclin=" + (pt.arraySettings.ArrayPitch).ToString() + Environment.NewLine + "DAzimut=" + azimuth.ToString() + Environment.NewLine + "NElemChamps =" + lineIndex.ToString() + Environment.NewLine + "NoOrient = -1" + Environment.NewLine + "PanneauPVName=JAM6-72-325/SI;0.991#1.956" + Environment.NewLine + "ModSpacingX=0.020" + Environment.NewLine + "ModSpacingY=0.020" + Environment.NewLine + "NModInHeight=1" + Environment.NewLine + "NModInWidth=1" + Environment.NewLine + "ModuleOrient=moLandScape" + Environment.NewLine;
                    pvIndex++;

                    lines += longA + Environment.NewLine + largA + Environment.NewLine + xOrig + Environment.NewLine + yOrig + Environment.NewLine + "TypeOrigine=1" + Environment.NewLine + "End of PVObject pvShdArrayRect" + Environment.NewLine;

                    stringList.Add(lines);
                }




            }









            return stringList;





        }
        */



        Point RotatePoint(Point pointToRotate, double angleInRadians)
        {

            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (double)
                    (cosTheta * (pointToRotate.X) -
                    sinTheta * (pointToRotate.Y)),
                Y =
                    (double)
                    (sinTheta * (pointToRotate.X) +
                    cosTheta * (pointToRotate.Y))
            };

        }

        private void shadingTick_Copy_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (List<Point3d> p3List in roofSections)
                {
                    foreach (Point3d p3 in p3List)
                    {

                        foreach (List<Point3d> p3ListNest in roofSections)
                        {
                            foreach (Point3d p3Nest in p3ListNest)
                            {
                                double px = Math.Abs(p3.X * ms.scale - p3Nest.X * ms.scale);
                                double py = Math.Abs(p3.Y * ms.scale - p3Nest.Y * ms.scale);

                                if ((px < 0.6) && (py < 0.6))
                                {

                                    p3Nest.X = p3.X;
                                    p3Nest.Y = p3.Y;

                                }
                            }

                        }
                        Debug.WriteLine("End of point");
                    }

                }
            }
            catch { }

        }

        private void PanelTypesGlobal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}


//TODO:











