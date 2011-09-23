using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KineSis {
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window {
        public CanvasWindow() {
            InitializeComponent();
            // Add a Line Element

            canvas.Background = System.Windows.Media.Brushes.Transparent;
            //canvas.Background.Opacity = 50;
            /*
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Red;
            myLine.Opacity = 0.2;
            myLine.X1 = 1;
            myLine.X2 = 500;
            myLine.Y1 = 1;
            myLine.Y2 = 500;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            canvas.Children.Add(myLine);*/
        }
    }
}
