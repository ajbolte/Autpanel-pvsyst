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

namespace AutoPANEL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Layout : Window
    {
        DxfDocument load;
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        Point distanceStart = new Point(0, 0);
        Point distanceFinish = new Point(0, 0);
        public Layout(Canvas canvas, BitmapImage image)
        {
            InitializeComponent();
            scrollo.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollo.ScrollChanged += OnScrollViewerScrollChanged;


            scrollo.MouseMove += OnMouseMove;
            slider.ValueChanged += OnSliderValueChanged;
            piccy.Source = image;
            IEnumerable<Polygon> query1 = canvas.Children.OfType<Polygon>();

            foreach (Polygon fruit in query1)
            {
                Polygon polygon = fruit;
                //  Polygon myobj2 = polygon.MemberwiseClone();
                // canny1.Children.Add(polygon);

            }

            // MessageBox.Show(canny1.Children.Count.ToString());

            load = DxfDocument.Load(@"C:\Users\ajbol\Documents\dxf.dxf");

            IEnumerable<netDxf.Entities.Line> lines = load.Lines;
            var drawLine = new Line();
            foreach (netDxf.Entities.Line l in lines)
            {
                drawLine = new Line();
                drawLine.X1 = l.StartPoint.X;
                drawLine.Y1 = l.StartPoint.Y;
                drawLine.X2 = l.EndPoint.X;
                drawLine.Y2 = l.EndPoint.Y;
                drawLine.Stroke = new SolidColorBrush(Colors.Green);

                drawLine.StrokeThickness = 1;
                //  Debug.WriteLine("X1: " + drawLine.X1.ToString());
                //  Debug.WriteLine("X2: " + drawLine.X2.ToString());
                //  Debug.WriteLine("Y1: " + drawLine.Y1.ToString());
                //  Debug.WriteLine("Y2: " + drawLine.Y2.ToString());
                canny.Children.Add(drawLine);
            }

            IEnumerable<netDxf.Entities.Text> text = load.Texts;
            TextBlock tb = new TextBlock();
            ScaleTransform sc = new ScaleTransform();
            sc.ScaleY = -1;
            foreach (netDxf.Entities.Text t in text)
            {
                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Left;
                tb.VerticalAlignment = VerticalAlignment.Bottom;
                tb.Text = t.Value;
                tb.FontSize = t.Height;
                tb.LayoutTransform = sc;
                Canvas.SetLeft(tb, t.Position.X);
                Canvas.SetTop(tb, t.Position.Y);
                canny.Children.Add(tb);
                Debug.WriteLine(t.Value.ToString());
                Debug.WriteLine(t.Position.X.ToString());
                Debug.WriteLine(t.Position.Y.ToString());
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void PrintPDF()
        {
            var v = scrollo.VerticalScrollBarVisibility;
            try
            {
                PrintDialog dialog = new PrintDialog();


                scrollo.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollo.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                if (dialog.ShowDialog() != true)
                    return;
                dialog.PrintVisual(canny, "IFMS Print Screen");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Screen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           // scrollo.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //scrollo.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintPDF();
        }
        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(piccy);

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
        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                scaleTransform.ScaleX = e.NewValue;
                scaleTransform.ScaleY = e.NewValue;
                //sBox.Text = scaleTransform.ScaleX.ToString();



                var centerOfViewport = new Point(scrollo.ViewportWidth / 2,
                    scrollo.ViewportHeight / 2);
                lastCenterPositionOnTarget = scrollo.TranslatePoint(centerOfViewport, piccy);
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
                        var centerOfViewport = new Point(scrollo.ViewportWidth / 2,
                                                         scrollo.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              scrollo.TranslatePoint(centerOfViewport, piccy);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(piccy);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / piccy.Width;
                    double multiplicatorY = e.ExtentHeight / piccy.Height;
                    //  MessageBox.Show(e.ExtentWidth.ToString() + " " + grid.Width.ToString());
                    //  MessageBox.Show(e.ExtentHeight.ToString() + " " + grid.Height.ToString());
                    double newOffsetX = scrollo.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollo.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {

                        return;
                    }

                    scrollo.ScrollToHorizontalOffset(newOffsetX);
                    scrollo.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(scrollo);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                scrollo.ScrollToHorizontalOffset(scrollo.HorizontalOffset - dX);
                scrollo.ScrollToVerticalOffset(scrollo.VerticalOffset - dY);
            }
        }
    }
}