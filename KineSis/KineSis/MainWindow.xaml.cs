/*
   Copyright 2011 Alexandru Albu - http://code.google.com/p/kinesis/

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using KineSis.Utils;
using mshtml;

namespace KineSis {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public CanvasWindow userCanvasWindow = new CanvasWindow();
        public CanvasWindow infoCanvasWindow = new CanvasWindow();
        public CanvasWindow presentationCanvasWindow = new CanvasWindow();
        public BrowserForm userBrowserForm = new BrowserForm();
        public BrowserForm presentationBrowserForm = new BrowserForm();
        public Console console = new Console();

        public static int USER_SCREEN_NUMBER = 0;

        public static int PRESENTATION_SCREEN_NUMBER = 1;

        public static Brush skeleton_brush = System.Windows.Media.Brushes.Black;

        public static String NAME = "MainWindow";

        public MainWindow() {
            InitializeComponent();
            console.log(NAME, "init successfull" );
            presentationCanvasWindow.Show();
            presentationBrowserForm.Show();
            userCanvasWindow.Show();
            infoCanvasWindow.Show();
            userBrowserForm.Show();

            console.log(NAME, "windows shown");
            
        }
        Runtime nui;
        DateTime lastTime = DateTime.MaxValue;
        SettingsWindow settingsWindow = new SettingsWindow();

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];


        Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

        private void Window_Loaded(object sender, EventArgs e) {

            goFullScreen();
            nui = new Runtime();

            try {
                nui.Initialize(RuntimeOptions.UseSkeletalTracking);
            } catch (InvalidOperationException) {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }

            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
        }


        private Point getDisplayPosition(Joint joint) {
            float depthX, depthY;
            nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320));  //convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240));  //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)( userCanvas.Width * colorX / 640.0 ), (int)( userCanvas.Height * colorY / 480 ));
        }

        Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids) {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i) {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            userCanvas.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons) {
                if (SkeletonTrackingState.Tracked == data.TrackingState) {
                    // Draw bones
                    Brush brush = skeleton_brush; //brushes[iSkeleton % brushes.Length];
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

                    // Draw joints
                    foreach (Joint joint in data.Joints) {
                        Point jointPos = getDisplayPosition(joint);
                        Line jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.ID];
                        jointLine.StrokeThickness = 6;
                        userCanvas.Children.Add(jointLine);
                    }
                }
                iSkeleton++;
            } // for each skeleton
        }

        private void Window_Closed(object sender, EventArgs e) {
            nui.Uninitialize();
            Environment.Exit(0);
        }

        private void goFullScreen() {

            console.log(NAME, "gone fullscreen");

            if ((double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width <= 0.75) {
                userCanvas.Height = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height;
                userCanvas.Width = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height * 4 / 3;
            } else {
                userCanvas.Height = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width * 3 / 4;
                userCanvas.Width = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width;
            }

            userBrowserForm.webBrowser1.Height = (int)( userCanvas.Height * 3 ) / 4;
            userBrowserForm.webBrowser1.Width = (int)( userCanvas.Width * 3 ) / 4;

            userCanvasWindow.Height = (int)( userCanvas.Height * 3 ) / 4;
            userCanvasWindow.canvas.Height = (int)( userCanvas.Height * 3 ) / 4;
            userCanvasWindow.Width = (int)( userCanvas.Width * 3 ) / 4;
            userCanvasWindow.canvas.Width = (int)( userCanvas.Width * 3 ) / 4;

            infoCanvasWindow.Width = userCanvas.Width;
            infoCanvasWindow.canvas.Width = userCanvas.Width;
            infoCanvasWindow.Height = ( userCanvas.Height - userCanvas.Height * 3 / 4 ) / 2;
            infoCanvasWindow.canvas.Height = ( userCanvas.Height - userCanvas.Height * 3 / 4 ) / 2;
            

            Double marginLeft = ( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width - userCanvas.Width ) / 2;
            Double marginTop = ( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height - userCanvas.Height ) / 2;
            userCanvas.Margin = new Thickness(marginLeft, marginTop, 0, 0);

            //( (System.Windows.Controls.MenuItem)userCanvas.ContextMenu.Items.GetItemAt(2) ).Header = "Exit Full Screen";

            WindowUtils.FullScreen(presentationBrowserForm, PRESENTATION_SCREEN_NUMBER);
            WindowUtils.FullScreen(presentationCanvasWindow, PRESENTATION_SCREEN_NUMBER);

            WindowUtils.FullScreen(this, USER_SCREEN_NUMBER);
            WindowUtils.FullScreen(userCanvasWindow, USER_SCREEN_NUMBER);
            
            WindowUtils.FullScreen(userBrowserForm, USER_SCREEN_NUMBER);
            WindowUtils.FullScreen(infoCanvasWindow, USER_SCREEN_NUMBER);

            if ((double) WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width <= 0.75) {
                presentationBrowserForm.webBrowser1.Height = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height;
                presentationBrowserForm.webBrowser1.Width = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height * 4 / 3;
            } else {
                presentationBrowserForm.webBrowser1.Height = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width * 3 / 4;
                presentationBrowserForm.webBrowser1.Width = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width;
            }


            presentationCanvasWindow.Height = presentationBrowserForm.webBrowser1.Height;
            presentationCanvasWindow.canvas.Height = presentationBrowserForm.webBrowser1.Height;
            presentationCanvasWindow.Width = presentationBrowserForm.webBrowser1.Width;
            presentationCanvasWindow.canvas.Width = presentationBrowserForm.webBrowser1.Width;

            infoCanvasWindow.canvas.Children.Clear();
            infoCanvasWindow.canvas.Background = System.Windows.Media.Brushes.Transparent;
           
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "zoom: " + (( (double)userBrowserForm.webBrowser1.Width / (double)presentationBrowserForm.webBrowser1.Width ) * 100) + "%\npresentation ratio = " + ((double)WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width) +
                 "\nuser ratio = " + ((double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width) + 
                "\npresentation width = " + presentationBrowserForm.webBrowser1.Width + 
                "\npresentation height = " + presentationBrowserForm.webBrowser1.Height;
            textBlock.Foreground = System.Windows.Media.Brushes.Red;
            textBlock.FontSize = 20;
            infoCanvasWindow.canvas.Children.Add(textBlock);

            presentationBrowserForm.webBrowser1.Left = (int)( WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width - presentationBrowserForm.webBrowser1.Width) / 2;
            presentationBrowserForm.webBrowser1.Top = (int)( WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height - presentationBrowserForm.webBrowser1.Height ) / 2;

            presentationCanvasWindow.Left = presentationBrowserForm.webBrowser1.Left + presentationBrowserForm.Left;
            presentationCanvasWindow.Top = presentationBrowserForm.webBrowser1.Top;

            userBrowserForm.webBrowser1.Left = (int)( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width - userBrowserForm.webBrowser1.Width ) / 2;
            userBrowserForm.webBrowser1.Top = (int)( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height - userBrowserForm.webBrowser1.Height ) / 2;


            userBrowserForm.SetZoom((( (double)userBrowserForm.webBrowser1.Width / (double)presentationBrowserForm.webBrowser1.Width ) * 100));

            infoCanvasWindow.Top = userCanvas.Height / 2 - infoCanvasWindow.canvas.Height/2;
        }

        private void UserCanvas_CM_SwitchScreens_Click(object sender, RoutedEventArgs e) {
            console.log(NAME, "switched screens");
            int x = USER_SCREEN_NUMBER;
            USER_SCREEN_NUMBER = PRESENTATION_SCREEN_NUMBER;
            PRESENTATION_SCREEN_NUMBER = x;
            goFullScreen();
        }

        private void UserCanvas_CM_Open_Click(object sender, RoutedEventArgs e) {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".*"; // Default file extension
            dlg.Filter = "Any (.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true) {
                // Open document
                string filename = dlg.FileName;
                userBrowserForm.open(filename);
                presentationBrowserForm.open(filename);
            }

        }

        private void UserCanvas_CM_Settings_Click(object sender, RoutedEventArgs e) {
            //bf.SetZoom(20);
                //settingsWindow = new SettingsWindow();
                settingsWindow.Topmost = true;
                settingsWindow.Show();
        }

        private void UserCanvas_CM_Show_Console_Click(object sender, RoutedEventArgs e) {
            //userBrowserForm.ScrollDown(10);
            //presentationBrowserForm.ScrollDown(10);
            console.Hide();
            console.Topmost = true;
            console.Show();
            console.WindowState = WindowState.Normal;
        }
    }
}