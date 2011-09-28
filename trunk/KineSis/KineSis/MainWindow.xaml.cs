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
using KineSis.ContentManagement.Model;
using KineSis.ContentManagement.Service;
using System.Threading;
using System.Windows.Threading;
using KineSis.ContentManagement.Progress;
using System.ComponentModel;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.IO;
using KineSis.Profiles;
using KineSis.UserInterface;
using KineSis.UserInterface.Entities;
using KineSis.UserInterface.Entities.Groups;

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
        public SettingsWindow settings = new SettingsWindow();

        private BackgroundWorker documentProcessingWorker = new BackgroundWorker();
        private BackgroundWorker documentChartProcessingWorker = new BackgroundWorker();

        public Document document = null;

        public static int USER_SCREEN_NUMBER = 0;

        public static int PRESENTATION_SCREEN_NUMBER = 1;

        public static Brush skeleton_brush;

        public static String NAME = "MainWindow";

        public int currentPage = 0;
        public static double presentationZoom = 100;

        public String currentFilename;

        private Boolean leftHandSelected = false;
        private Boolean rightHandSelected = false;
        private int leftHandCounter = 0;
        private int rightHandCounter = 0;

        public Chart currentChart = null;
        public int chartNumber = 0;

        Runtime nui;
        DateTime lastTime = DateTime.MaxValue;

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

        public MainWindow() {

            InitializeComponent();

                documentProcessingWorker.WorkerReportsProgress = true;
                documentProcessingWorker.WorkerSupportsCancellation = false;
                documentProcessingWorker.ProgressChanged += new ProgressChangedEventHandler(documentProcessingWorker_ProgressChanged);
                documentProcessingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(documentProcessingWorker_RunWorkerCompleted);
                documentProcessingWorker.DoWork += new DoWorkEventHandler(documentProcessingWorker_DoWork);

                documentChartProcessingWorker.WorkerReportsProgress = true;
                documentChartProcessingWorker.WorkerSupportsCancellation = false;
                documentChartProcessingWorker.ProgressChanged += new ProgressChangedEventHandler(documentChartProcessingWorker_ProgressChanged);
                documentChartProcessingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(documentChartProcessingWorker_RunWorkerCompleted);
                documentChartProcessingWorker.DoWork += new DoWorkEventHandler(documentChartProcessingWorker_DoWork);

                console.log(NAME, "init successfull");
                presentationCanvasWindow.Show();
                presentationBrowserForm.Show();
                userCanvasWindow.Show();
                infoCanvasWindow.Show();
                userBrowserForm.Show();
                settings.mw = this;

                ApplyProfile();

                console.log(NAME, "windows shown");
        }

        public void ApplyProfile() {
            Profile profile = ProfileManager.ActiveProfile;
            userBrowserForm.BackColor = ColorUtil.DrawingColorFromHTML(profile.BackgroundColor.ToString());
            presentationBrowserForm.BackColor = ColorUtil.DrawingColorFromHTML(profile.BackgroundColor.ToString());
            skeleton_brush = ColorUtil.FromHTML(profile.SkeletonColor.ToString());

            if (PRESENTATION_SCREEN_NUMBER != profile.PresentationScreen || USER_SCREEN_NUMBER != profile.UserScreen) {
                PRESENTATION_SCREEN_NUMBER = profile.PresentationScreen;
                USER_SCREEN_NUMBER = profile.UserScreen;
                goFullScreen();
            }

            /*
            if (PRESENTATION_SCREEN_NUMBER != profile.PresentationScreen) {
                SwitchScreens();
            }*/
        }


        public void documentProcessingWorker_DoWork(object sender, DoWorkEventArgs e) {
            //working here
            BackgroundWorker worker = sender as BackgroundWorker;

            String filename = (String)e.Argument;
            document = DocumentService.CreateNewDocument(filename, worker);
            
        }

        public void documentChartProcessingWorker_DoWork(object sender, DoWorkEventArgs e) {
            //working here
            BackgroundWorker worker = sender as BackgroundWorker;
            String filename = (String)e.Argument;
            DocumentService.CreateNewDocumentCharts(filename, worker, document);
        }

        public void documentProcessingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            try {
                ProcessingProgress pp = (ProcessingProgress)e.UserState;

                if (pp.OverallOperationName.Contains("[Exception]")) {
                    CanvasUtil.DrawException(infoCanvasWindow.canvas, pp.OverallOperationName);
                } else {
                    CanvasUtil.DrawProgress(infoCanvasWindow.canvas, pp);
                }
            } catch (Exception) {

            }
        }

        public void documentChartProcessingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            try {
                ProcessingProgress pp = (ProcessingProgress)e.UserState;
               
                CanvasUtil.DrawProgress(infoCanvasWindow.canvas, pp);
            } catch (Exception) {

            }
        }


        public void documentProcessingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            //show the page and reset parameters
            if (document != null) {

                if (document.Pages.Count > 1) {
                    Pages.Elements = new List<Element>();
                    foreach (KineSis.ContentManagement.Model.Page page in document.Pages) {
                        Element element = new Element();
                        element.Name = page.Name;
                        element.Thumbnail = page.Thumbnail;
                        Pages.Elements.Add(element);
                    }
                }
                presentationZoom = 100;

                currentPage = 0;

                UpdateShapes();

                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                }

                ApplyZoom();

                infoCanvasWindow.canvas.Children.Clear();
                documentChartProcessingWorker.RunWorkerAsync(currentFilename);
            }
        }

        public void documentChartProcessingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            //everything is done now
            if (document != null) {
                UpdateShapes();
                infoCanvasWindow.canvas.Children.Clear();
                infoCanvasWindow.canvas.Background = Brushes.Transparent;
            }
        }

        public void ToNextPage() {
            if (document != null && currentPage < document.Pages.Count - 1) {
                currentPage++;
                UpdateShapes();    

                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                }
            }
        }

        public void UpdateShapes() {
            Shapes.Elements = new List<Element>();
            foreach (KineSis.ContentManagement.Model.Chart chart in document.Pages[currentPage].Charts) {
                Element element = new Element();
                element.Name = chart.Title;
                element.Thumbnail = chart.Thumbnail;
                Shapes.Elements.Add(element);
            }
        }

        public void ToPreviousPage() {
            if (document != null && currentPage > 0) {
                currentPage--;
                UpdateShapes();
                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                }
            }
        }

        public void GoToPage(int pageNumber) {
            if (document != null && pageNumber < document.Pages.Count) {
                currentPage = pageNumber;
                UpdateShapes();
                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                }
            }
        }

        public void GoToChart(int chartNumber) {
            this.chartNumber = chartNumber;
            currentChart = document.Pages[currentPage].Charts[chartNumber];
            CanvasUtil.DrawImage(presentationCanvasWindow, userCanvasWindow, currentChart.GetImageUrl());
        }

        public void RefreshCharts() {
            if (currentChart != null) {
                GoToChart(this.chartNumber);
            }
        }

        public void CloseChart() {
            currentChart = null;
            userCanvasWindow.image.Source = null;
            presentationCanvasWindow.image.Source = null;

            userCanvasWindow.image.Width = 0;
            userCanvasWindow.image.Height = 0;
            presentationCanvasWindow.image.Width = 0;
            presentationCanvasWindow.image.Height = 0;

            userCanvasWindow.Background = System.Windows.Media.Brushes.Transparent;
            presentationCanvasWindow.Background = System.Windows.Media.Brushes.Transparent;
        }

        public void RotateRight() {
            if (document != null && currentChart != null) {
                if (currentChart.HasRightImage()) {
                    String chartImage = currentChart.GetRightImageUrl();
                    CanvasUtil.DrawImage(presentationCanvasWindow, userCanvasWindow, chartImage);
                }
            }
        }

        public void RotateLeft() {
            if (document != null && currentChart != null) {
                if (currentChart.HasLeftImage()) {
                    String chartImage = currentChart.GetLeftImageUrl();
                    CanvasUtil.DrawImage(presentationCanvasWindow, userCanvasWindow, chartImage);
                }
            }
        }

        public void RotateUp() {
            if (document != null && currentChart != null) {
                if (currentChart.HasUpImage()) {
                    String chartImage = currentChart.GetUpImageUrl();
                    CanvasUtil.DrawImage(presentationCanvasWindow, userCanvasWindow, chartImage);
                }
            }
        }

        public void RotateDown() {
            if (document != null && currentChart != null) {
                if (currentChart.HasDownImage()) {
                    String chartImage = currentChart.GetDownImageUrl();
                    CanvasUtil.DrawImage(presentationCanvasWindow, userCanvasWindow, chartImage);
                }
            }
        }

        public void ScrollRight() {
            if (document != null) {
                userBrowserForm.ScrollRight(10);
                presentationBrowserForm.ScrollRight(10);
            }
        }

        public void ScrollLeft() {
            if (document != null) {
                userBrowserForm.ScrollLeft(10);
                presentationBrowserForm.ScrollLeft(10);
            }
        }

        public void ScrollUp() {
            if (document != null) {
                userBrowserForm.ScrollUp(10);
                presentationBrowserForm.ScrollUp(10);
            }
        }

        public void ScrollDown() {
            if (document != null) {
                userBrowserForm.ScrollDown(10);
                presentationBrowserForm.ScrollDown(10);
            }
        }

        private void ApplyZoom() {
            presentationBrowserForm.SetZoom(presentationZoom);
            userBrowserForm.SetZoom(( ( (double)( userBrowserForm.webBrowser1.Height ) * ( presentationZoom + presentationZoom * 0.005) / (double)presentationBrowserForm.webBrowser1.Height ) ));
        }

        public void ZoomIn() {
            if (document != null) {
                presentationZoom += 1;
                ApplyZoom();
            }
        }

        public void ZoomOut() {
            if (document != null) {
                presentationZoom -= 1;
                ApplyZoom();
            }
        }

        public void ZoomFit() {
            if (document != null && currentPage < document.Pages.Count) {
                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                    presentationZoom = 100;
                    ApplyZoom();
                }
            }
        }

        public void OpenDocument(Document doc) {
            document = doc;
            if (document != null) {

                CloseChart();
                presentationCanvasWindow.canvas.Children.Clear();
                userCanvasWindow.canvas.Children.Clear();
                UIManager.SelectedGroup = UIManager.MainGroup;

                if (document.Pages.Count > 1) {
                    Pages.Elements = new List<Element>();
                    foreach (KineSis.ContentManagement.Model.Page page in document.Pages) {
                        Element element = new Element();
                        element.Name = page.Name;
                        element.Thumbnail = page.Thumbnail;
                        Pages.Elements.Add(element);
                    }
                }
                presentationZoom = 100;

                currentPage = 0;

                UpdateShapes();

                if (UIManager.ZoomFit && document.Pages[currentPage].LocationNoZoom != null) {
                    userBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                    presentationBrowserForm.open(document.Pages[currentPage].LocationNoZoom);
                } else {
                    userBrowserForm.open(document.Pages[currentPage].Location);
                    presentationBrowserForm.open(document.Pages[currentPage].Location);
                }

                ApplyZoom();

                infoCanvasWindow.canvas.Children.Clear();

                TextBlock textBlock = new TextBlock();

                textBlock.Text = "Ready";
                textBlock.Foreground = System.Windows.Media.Brushes.Red;
                textBlock.FontSize = 20;
                infoCanvasWindow.canvas.Background = Brushes.Transparent;
                infoCanvasWindow.canvas.Children.Add(textBlock);
                infoCanvasWindow.canvas.UpdateLayout();
                infoCanvasWindow.canvas.Refresh();
            }
        }

        public void CheckOpenedDocument() {
            DirectoryInfo di = new DirectoryInfo(ProfileManager.ActiveProfile.TempFolder + "\\" + document.Location);
            if (!di.Exists) {
                document = null;
                CloseChart();
                presentationCanvasWindow.canvas.Children.Clear();
                userCanvasWindow.canvas.Children.Clear();
                presentationZoom = 100;
                ApplyZoom();
                userBrowserForm.open(Directory.GetCurrentDirectory() + "\\Startup\\startup.html");
                presentationBrowserForm.open(Directory.GetCurrentDirectory() + "\\Startup\\startup.html");
                UIManager.SelectedGroup = UIManager.MainGroup;
            }
        }

        

        private void Window_Loaded(object sender, EventArgs e) {
            if (WindowUtils.Screens.Count() < 2) {
                MessageBoxResult r = MessageBox.Show("You need 2 screens to run KineSis");
                if (r == MessageBoxResult.OK) {
                    Environment.Exit(0);
                }
            } else {
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
            polyline.StrokeThickness = 20;
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

            double minZ = double.MaxValue;
            int userID = -1;

                foreach (SkeletonData data in skeletonFrame.Skeletons) {
                    if (SkeletonTrackingState.Tracked == data.TrackingState) {
                        if (data.Joints[JointID.Head].Position.Z < minZ) {
                            minZ = data.Joints[JointID.Head].Position.Z;
                            userID = data.TrackingID;
                        }
                    }
                }

            foreach (SkeletonData data in skeletonFrame.Skeletons) {
                if (SkeletonTrackingState.Tracked == data.TrackingState) {
                    // Draw bones
                    Brush brush = null; //brushes[iSkeleton % brushes.Length];
                    if (data.TrackingID == userID) {
                        brush = skeleton_brush;
                    } else {

                        brush = ProfileManager.ActiveProfile.SkeletonColor;
                        brush.Opacity = 0.2;
                    }
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.ShoulderCenter));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft));
                    userCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight));

                    Joint leftShoulder = new Joint();
                    Joint rightShoulder = new Joint();
                    Joint leftWrist = new Joint();
                    Joint rightWrist = new Joint();
                    Joint centerShoulder = new Joint();
                    Joint centerHip = new Joint();

                    // Draw joints
                    foreach (Joint joint in data.Joints) {
                        if (joint.ID != JointID.HandLeft && joint.ID != JointID.HandRight && joint.ID != JointID.Spine && joint.ID != JointID.Head && joint.ID != JointID.FootRight && joint.ID != JointID.FootLeft) {
                            Point jointPos = getDisplayPosition(joint);

                            Ellipse ellipse = new Ellipse();
                            ellipse.Width = 30;
                            ellipse.Height = 30;
                            ellipse.Stroke = brush;
                            ellipse.StrokeThickness = 5;
                            ellipse.Fill = jointColors[joint.ID];
                            if (data.TrackingID != userID) {
                                ellipse.Fill = ProfileManager.ActiveProfile.SkeletonColor;
                                ellipse.Fill.Opacity = 0.2;
                            }
                            Canvas.SetTop(ellipse, jointPos.Y - 15);
                            Canvas.SetLeft(ellipse, jointPos.X - 15);
                            userCanvas.Children.Add(ellipse);

                            if (joint.ID == JointID.ShoulderLeft) {
                                leftShoulder = joint;
                            } else if (joint.ID == JointID.ShoulderRight) {
                                rightShoulder = joint;
                            } else if (joint.ID == JointID.WristLeft) {
                                leftWrist = joint;
                            } else if (joint.ID == JointID.WristRight) {
                                rightWrist = joint;
                            } else if (joint.ID == JointID.ShoulderCenter) {
                                centerShoulder = joint;
                            } else if (joint.ID == JointID.HipCenter) {
                                centerHip= joint;
                            }
                        } else if (joint.ID == JointID.Head) {
                            Point jointPos = getDisplayPosition(joint);
                            Ellipse ellipse = new Ellipse();
                            ellipse.Width = 80;
                            ellipse.Height = 80;
                            ellipse.Stroke = brush;
                            ellipse.StrokeThickness = 10;
                            
                            if (data.TrackingID != userID) {
                                ellipse.Fill = ColorUtil.FromHTML("#55FFFFE0");
                                ellipse.Fill.Opacity = 0.2;
                            } else {
                                ellipse.Fill = Brushes.LightYellow;
                            }
                            Canvas.SetTop(ellipse, jointPos.Y - 20 );
                            Canvas.SetLeft(ellipse, jointPos.X - 40);
                            userCanvas.Children.Add(ellipse);

                        }
                    }

                    if (data.TrackingID == userID) {

                        Double lDist = KineSis.Geometry.GeometryUtil.GetDistance2D(new KineSis.Geometry.Point2D(leftWrist.Position.X, leftWrist.Position.Z),
                                                                                        new KineSis.Geometry.Point2D(leftShoulder.Position.X, leftShoulder.Position.Z));
                        Double rDist = KineSis.Geometry.GeometryUtil.GetDistance2D(new KineSis.Geometry.Point2D(rightWrist.Position.X, rightWrist.Position.Z),
                                                                                    new KineSis.Geometry.Point2D(rightShoulder.Position.X, rightShoulder.Position.Z));

                        Double delta = Math.Sqrt(Math.Pow(getDisplayPosition(centerShoulder).X - getDisplayPosition(centerHip).X, 2) + Math.Pow(getDisplayPosition(centerShoulder).Y - getDisplayPosition(centerHip).Y, 2));

                        Ellipse ellipse2 = new Ellipse();
                        ellipse2.Width = 100;
                        ellipse2.Height = 100;
                        ellipse2.Stroke = brush;
                        ellipse2.StrokeThickness = 10;
                        ellipse2.Fill = jointColors[leftWrist.ID];
                        Canvas.SetTop(ellipse2, 0);
                        Canvas.SetLeft(ellipse2, 0);
                        userCanvas.Children.Add(ellipse2);

                        int ld = (int)( ( lDist / 0.4 ) * 10 );
                        String LD = ( ld < 10 ) ? ( "0" + ld ) : ld.ToString();

                        TextBlock tb1 = new TextBlock();
                        tb1.Text = LD;
                        tb1.Foreground = brush;
                        tb1.FontSize = 50;
                        Canvas.SetTop(tb1, 15);
                        Canvas.SetLeft(tb1, 25);
                        userCanvas.Children.Add(tb1);

                        Ellipse ellipse3 = new Ellipse();
                        ellipse3.Width = 100;
                        ellipse3.Height = 100;
                        ellipse3.Stroke = brush;
                        ellipse3.StrokeThickness = 10;
                        ellipse3.Fill = jointColors[rightWrist.ID];
                        Canvas.SetTop(ellipse3, 0);
                        Canvas.SetRight(ellipse3, 0);
                        userCanvas.Children.Add(ellipse3);

                        int rd = (int)( ( rDist / 0.35 ) * 10 );
                        String RD = ( rd < 10 ) ? ( "0" + rd ) : rd.ToString();

                        TextBlock tb2 = new TextBlock();
                        tb2.Text = RD;
                        tb2.Foreground = brush;
                        tb2.FontSize = 50;
                        Canvas.SetTop(tb2, 15);
                        Canvas.SetRight(tb2, 25);
                        userCanvas.Children.Add(tb2);

                        if (lDist > 0.35) {
                            leftHandCounter++;
                            //if (leftHandCounter >= 15) {
                            //    leftHandCounter = 15;
                            leftHandSelected = true;
                            //}
                        } else {
                            leftHandSelected = false;
                            leftHandCounter = 0;
                        }

                        if (leftHandSelected && !UIManager.InPaint) {
                            Ellipse ellipse1 = new Ellipse();
                            ellipse1.Width = 100;
                            ellipse1.Height = 100;
                            ellipse1.Stroke = brush;
                            ellipse1.StrokeThickness = 10;
                            ellipse1.Fill = jointColors[leftWrist.ID];
                            Canvas.SetTop(ellipse1, getDisplayPosition(leftWrist).Y - 50);
                            Canvas.SetLeft(ellipse1, getDisplayPosition(leftWrist).X - 50);
                            userCanvas.Children.Add(ellipse1);
                        }

                        if (rDist > 0.35) {
                            rightHandCounter++;
                            //if (rightHandCounter >= 15) {
                            //    rightHandCounter = 15;
                            rightHandSelected = true;
                            //}
                        } else {
                            rightHandSelected = false;
                            rightHandCounter = 0;
                        }

                        if (rightHandSelected && !UIManager.InPaint) {
                            Ellipse ellipse1 = new Ellipse();
                            ellipse1.Width = 100;
                            ellipse1.Height = 100;
                            ellipse1.Stroke = brush;
                            ellipse1.StrokeThickness = 10;
                            ellipse1.Fill = jointColors[rightWrist.ID];
                            Canvas.SetTop(ellipse1, getDisplayPosition(rightWrist).Y - 50);
                            Canvas.SetLeft(ellipse1, getDisplayPosition(rightWrist).X - 50);
                            userCanvas.Children.Add(ellipse1);
                        }

                        Hand leftHand = new Hand();
                        leftHand.X = getDisplayPosition(leftWrist).X;
                        leftHand.Y = getDisplayPosition(leftWrist).Y;
                        leftHand.IsSelected = leftHandSelected;

                        Hand rightHand = new Hand();
                        rightHand.X = getDisplayPosition(rightWrist).X;
                        rightHand.Y = getDisplayPosition(rightWrist).Y;
                        rightHand.IsSelected = rightHandSelected;

                        UIManager.LeftHand = leftHand;
                        UIManager.RightHand = rightHand;
                        UIManager.Delta = delta;
                        UIManager.Process(this);
                    }
                }
                iSkeleton++;
            } // for each skeleton

            userCanvas.Refresh();
        }

        

        private void Window_Closed(object sender, EventArgs e) {
            try {
                nui.Uninitialize();

            } catch (Exception) {
            }
            Environment.Exit(0);
        }

        private void goFullScreen() {

            console.log(NAME, "gone fullscreen");

            int PS_WIDTH = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width; //4
            int PS_HEIGHT = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height;  //3

            if ((double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width < 0.75) {
                userCanvas.Height = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height;
                userCanvas.Width = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height * 4 / 3;
            } else {
                userCanvas.Height = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width * 3 / 4;
                userCanvas.Width = WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width;
            }

            if ((double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height / (double)WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width < 0.75) {
                userBrowserForm.webBrowser1.Height = (int)userCanvas.Height;
                userBrowserForm.webBrowser1.Width = (int) (userCanvas.Height * (double) PS_WIDTH / (double) PS_HEIGHT);
            } else {
                userBrowserForm.webBrowser1.Height = (int)( ( userCanvas.Width * (double) PS_HEIGHT ) / (double) PS_WIDTH );
                userBrowserForm.webBrowser1.Width = (int) userCanvas.Width;
            }

            userCanvasWindow.Height = userBrowserForm.webBrowser1.Height;
            userCanvasWindow.canvas.Height = userBrowserForm.webBrowser1.Height;
            userCanvasWindow.Width = userBrowserForm.webBrowser1.Width;
            userCanvasWindow.canvas.Width = userBrowserForm.webBrowser1.Width;

            infoCanvasWindow.Width = userCanvas.Width;
            infoCanvasWindow.canvas.Width = userCanvas.Width;
            infoCanvasWindow.Height = ( userCanvas.Height - userCanvas.Height * 3 / 4 ) / 2;
            infoCanvasWindow.canvas.Height = ( userCanvas.Height - userCanvas.Height * 3 / 4 ) / 2;
            

            Double marginLeft = ( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width - userCanvas.Width ) / 2;
            Double marginTop = ( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height - userCanvas.Height ) / 2;
            userCanvas.Margin = new Thickness(marginLeft, marginTop, 0, 0);

            WindowUtils.FullScreen(presentationBrowserForm, PRESENTATION_SCREEN_NUMBER);
            WindowUtils.FullScreen(presentationCanvasWindow, PRESENTATION_SCREEN_NUMBER);

            WindowUtils.FullScreen(this, USER_SCREEN_NUMBER);
            WindowUtils.FullScreen(userCanvasWindow, USER_SCREEN_NUMBER);
            
            WindowUtils.FullScreen(userBrowserForm, USER_SCREEN_NUMBER);
            WindowUtils.FullScreen(infoCanvasWindow, USER_SCREEN_NUMBER);

            presentationBrowserForm.webBrowser1.Height = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height;
            presentationBrowserForm.webBrowser1.Width = WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width;


            presentationCanvasWindow.Height = presentationBrowserForm.webBrowser1.Height;
            presentationCanvasWindow.canvas.Height = presentationBrowserForm.webBrowser1.Height;
            presentationCanvasWindow.Width = presentationBrowserForm.webBrowser1.Width;
            presentationCanvasWindow.canvas.Width = presentationBrowserForm.webBrowser1.Width;

            infoCanvasWindow.canvas.Children.Clear();
            infoCanvasWindow.canvas.Background = System.Windows.Media.Brushes.Transparent;

            presentationBrowserForm.webBrowser1.Left = (int)( WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Width - presentationBrowserForm.webBrowser1.Width) / 2;
            presentationBrowserForm.webBrowser1.Top = (int)( WindowUtils.Screens[PRESENTATION_SCREEN_NUMBER].Bounds.Height - presentationBrowserForm.webBrowser1.Height ) / 2;

            presentationCanvasWindow.Left = presentationBrowserForm.webBrowser1.Left + presentationBrowserForm.Left;
            presentationCanvasWindow.Top = presentationBrowserForm.webBrowser1.Top;

            userBrowserForm.webBrowser1.Left = (int)( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Width - userBrowserForm.webBrowser1.Width ) / 2;
            userBrowserForm.webBrowser1.Top = (int)( WindowUtils.Screens[USER_SCREEN_NUMBER].Bounds.Height - userBrowserForm.webBrowser1.Height ) / 2;

            ApplyZoom();

            infoCanvasWindow.Top = userCanvas.Height / 2 - infoCanvasWindow.canvas.Height/2;

            RefreshCharts();
            UIManager.Clear();
        }



        private void UserCanvas_CM_SwitchScreens_Click(object sender, RoutedEventArgs e) {
            SwitchScreens();
        }

        private void SwitchScreens() {
            console.log(NAME, "switched screens");
            int x = USER_SCREEN_NUMBER;
            USER_SCREEN_NUMBER = PRESENTATION_SCREEN_NUMBER;
            PRESENTATION_SCREEN_NUMBER = x;
            goFullScreen();
        }

        private void UserCanvas_CM_Open_Click(object sender, RoutedEventArgs e) {

            String sources = "";

            foreach (TextHighlight t in DocumentService.TextHighlightConfiguration.TextHighlights) {
                sources += "*" + t.FilenameExtension + ";";
            }

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".*"; // Default file extension
            dlg.Filter = "Microsoft PowerPoint Presentation|*.pptx|Microsoft Word Document|*.docx|Microsoft Excel Workbook|*.xlsx|Pictures|*.jpeg;*.jpg;*.png;*.bmp;*.gif;*tiff;*.tif|Text Documents|"+sources; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true) {
                // Open document
                string filename = dlg.FileName;

                if (DocumentService.IsFileSupported(filename)) {
                    currentFilename = filename;
                    documentProcessingWorker.RunWorkerAsync(filename);
                } else {
                    MessageBox.Show("This file is not supported. Choose another one");
                    UserCanvas_CM_Open_Click(sender, e);
                }

            }

        }

        private void UserCanvas_CM_Settings_Click(object sender, RoutedEventArgs e) {
            settings.Hide();
            settings.Topmost = true;
            settings.ShowDialog();
            settings.WindowState = WindowState.Normal;
        }

        private void UserCanvas_CM_Open_Existing_Click(object sender, RoutedEventArgs e) {
            ArchiveWindow aw = new ArchiveWindow(this);
            aw.Topmost = true;
            aw.ShowDialog();
        }

        private void UserCanvas_CM_Show_Console_Click(object sender, RoutedEventArgs e) {
            console.Hide();
            console.Topmost = true;
            console.ShowDialog();
            console.WindowState = WindowState.Normal;
        }


    }

    public static class ExtensionMethods {
        private static Action EmptyDelegate = delegate() {
        };

        public static void Refresh(this UIElement uiElement) {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}