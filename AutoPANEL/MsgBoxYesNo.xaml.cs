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
    public partial class MsgBoxYesNo : Window
    {
        public MsgBoxYesNo()
        {
            InitializeComponent();

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
    }

}