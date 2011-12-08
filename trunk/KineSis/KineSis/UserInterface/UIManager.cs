/*
   Copyright 2011 Alexandru Albu - sandu.albu@gmail.com

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
using KineSis.UserInterface.Entities;
using KineSis.UserInterface.Entities.Groups;
using System.Windows.Controls;
using KineSis.Profiles;
using System.Windows.Shapes;
using KineSis.ContentManagement.Model;
using KineSis.Utils;
using KineSis.Geometry;
using System.Windows.Media;

namespace KineSis.UserInterface
{
    /// <summary>
    /// UIManager is the central brain of user interface. Its role is to link the user movements to presentation flow.
    /// </summary>
    class UIManager
    {

        private static Hand leftHand;
        private static Hand rightHand;
        private static int firstHand; //0-none; 1=left; 2=right;
        private static MainWindow mainw = null;
        private static Boolean zoomFit = false;
        private static Boolean inPaint = false;
        public static double initialX;
        public static double initialY;
        public static Boolean inMenuSession;
        public static Boolean messOnScreen;
        private static Boolean rightHandOnTop;
        private static Boolean leftHandOnTop;

        /// <summary>
        /// minimal scroll lock - decides for minimal view if the menu should act like scroll or like navigate
        /// </summary>
        public static Boolean MinimalScrollLock
        {
            get
            {
                return (mainw != null && mainw.document != null && mainw.document.Pages.Count == 1);
            }
        }

        /// <summary>
        /// secondary menu is shown if user have one hand up and the other one selected
        /// </summary>
        public static Boolean ShowSecondaryMenu
        {
            get
            {
                return (firstHand == 1 && rightHandOnTop) || (firstHand == 2 && leftHandOnTop) && FirstHand.IsSelected;
            }
        }

        /// <summary>
        /// right hand is up or not
        /// </summary>
        public static Boolean RightHandOnTop
        {
            get
            {
                return rightHandOnTop;
            }

            set
            {
                rightHandOnTop = value;
            }
        }

        /// <summary>
        /// left hand is up or not
        /// </summary>
        public static Boolean LeftHandOnTop
        {
            get
            {
                return leftHandOnTop;
            }

            set
            {
                leftHandOnTop = value;
            }
        }

        /// <summary>
        /// computes the menu diameter based on user canvas height.
        /// </summary>
        public static Double MENU_DIAMETER
        {
            get
            {
                return mainw.userCanvas.Height / 5;
            }
        }

        /// <summary>
        /// computes the submenu diameter based on user canvas height.
        /// </summary>
        public static Double SUBMENU_DIAMETER
        {
            get
            {
                if (mainw != null && mainw.userCanvas != null)
                {
                    return mainw.userCanvas.Height / 6;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static Group mainGroup = KineSis.UserInterface.Entities.Groups.Menu.Instance;
        private static Group selectedGroup = mainGroup;

        /// <summary>
        /// active document
        /// </summary>
        public static Document ActiveDocument
        {
            get
            {
                if (mainw != null)
                {
                    return mainw.document;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// active document page
        /// </summary>
        public static int ActiveDocumentPage
        {
            get
            {
                if (mainw != null)
                {
                    return mainw.currentPage;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// active document page chart
        /// </summary>
        public static Chart ActiveDocumentChart
        {
            get
            {
                if (mainw != null)
                {
                    return mainw.currentChart;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// inPaint is on when user is drawing something on the screen
        /// </summary>
        public static Boolean InPaint
        {
            get
            {
                return inPaint;
            }

            set
            {
                inPaint = value;
            }
        }

        /// <summary>
        /// return the status of zoom. If zoomFit is on, then the zoom in or out is disabled.
        /// </summary>
        public static Boolean ZoomFit
        {
            get
            {
                return zoomFit;
            }

            set
            {
                zoomFit = value;
                if (mainw != null)
                {
                    mainw.ZoomFit();
                }
            }
        }

        /// <summary>
        /// return number of the first hand: 0-none, 1-left, 2-right
        /// </summary>
        public static int FirstHandNumber
        {
            get
            {
                return firstHand;
            }
        }

        /// <summary>
        /// return second touched hand
        /// </summary>
        public static Hand SecondHand
        {
            get
            {
                if (firstHand == 1 && rightHand != null && rightHand.IsSelected)
                {
                    return rightHand;
                }
                else if (firstHand == 2 && leftHand != null && leftHand.IsSelected)
                {
                    return leftHand;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// return first touched hand
        /// </summary>
        public static Hand FirstHand
        {
            get
            {
                if (firstHand == 1)
                {
                    return leftHand;
                }
                else if (firstHand == 2)
                {
                    return rightHand;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// return left hand
        /// </summary>
        public static Hand LeftHand
        {
            get
            {
                return leftHand;
            }

            set
            {
                leftHand = value;
                if ((rightHand == null || !rightHand.IsSelected) && (leftHand == null || !leftHand.IsSelected))
                {
                    firstHand = 0;
                }
                else if (leftHand != null && leftHand.IsSelected && firstHand == 0)
                {
                    firstHand = 1;
                }
            }
        }

        /// <summary>
        /// return right hand
        /// </summary>
        public static Hand RightHand
        {
            get
            {
                return rightHand;
            }

            set
            {
                rightHand = value;
                if ((rightHand == null || !rightHand.IsSelected) && (leftHand == null || !leftHand.IsSelected))
                {
                    firstHand = 0;
                }
                else if (rightHand != null && rightHand.IsSelected && firstHand == 0)
                {
                    firstHand = 2;
                }
            }
        }

        /// <summary>
        /// selected group is the active group/menu
        /// </summary>
        public static Group SelectedGroup
        {
            get
            {
                if (ProfileManager.MinimalView)
                {
                    return Minimal.Instance;
                }
                else
                {
                    return selectedGroup;
                }
            }

            set
            {
                selectedGroup = value;
            }
        }

        /// <summary>
        /// main group is the starting group, the root of groups
        /// </summary>
        public static Group MainGroup
        {
            get
            {
                return mainGroup;
            }

            set
            {
                mainGroup = value;
            }
        }

        /// <summary>
        /// call next page of the document
        /// </summary>
        public static void ToNextPage()
        {
            if (mainw != null)
            {
                if (messOnScreen)
                {
                    Clear();
                }
                if (UIManager.ActiveDocument != null && UIManager.ActiveDocumentChart != null)
                {
                    CloseChart();
                }
                mainw.ToNextPage();
            }
        }

        /// <summary>
        /// call previous page of the document
        /// </summary>
        public static void ToPreviousPage()
        {
            if (mainw != null)
            {
                if (messOnScreen)
                {
                    Clear();
                }
                if (UIManager.ActiveDocument != null && UIManager.ActiveDocumentChart != null)
                {
                    CloseChart();
                }
                mainw.ToPreviousPage();
            }
        }

        /// <summary>
        /// go to a specific page number
        /// </summary>
        /// <param name="pageNumber"></param>
        public static void GoToPage(int pageNumber)
        {
            if (mainw != null)
            {
                if (messOnScreen)
                {
                    Clear();
                }
                if (UIManager.ActiveDocument != null && UIManager.ActiveDocumentChart != null)
                {
                    CloseChart();
                }
                mainw.GoToPage(pageNumber);
            }
        }

        /// <summary>
        /// call scroll right
        /// </summary>
        public static void ScrollRight()
        {
            if (mainw != null)
            {
                mainw.ScrollRight();
            }
        }

        /// <summary>
        /// call scroll left
        /// </summary>
        public static void ScrollLeft()
        {
            if (mainw != null)
            {
                mainw.ScrollLeft();
            }
        }

        /// <summary>
        /// call scroll up
        /// </summary>
        public static void ScrollUp()
        {
            if (mainw != null)
            {
                mainw.ScrollUp();
            }
        }

        /// <summary>
        /// call scroll down
        /// </summary>
        public static void ScrollDown()
        {
            if (mainw != null)
            {
                mainw.ScrollDown();
            }
        }

        /// <summary>
        /// call zoom in
        /// </summary>
        public static void ZoomIn()
        {
            if (mainw != null)
            {
                mainw.ZoomIn();
            }
        }

        /// <summary>
        /// call zoom out
        /// </summary>
        public static void ZoomOut()
        {
            if (mainw != null)
            {
                mainw.ZoomOut();
            }
        }

        /// <summary>
        /// go to a specific chart in current page
        /// </summary>
        /// <param name="chartNumber"></param>
        public static void GoToChart(int chartNumber)
        {
            if (mainw != null)
            {
                mainw.GoToChart(chartNumber);
            }
        }

        /// <summary>
        /// close opened chart
        /// </summary>
        public static void CloseChart()
        {
            if (mainw != null)
            {
                mainw.CloseChart();
            }
        }

        /// <summary>
        /// call roate chart right
        /// </summary>
        public static void RotateRight()
        {
            if (mainw != null)
            {
                mainw.RotateRight();
            }
        }

        /// <summary>
        /// call rotate chart left
        /// </summary>
        public static void RotateLeft()
        {
            if (mainw != null)
            {
                mainw.RotateLeft();
            }
        }

        /// <summary>
        /// call rotate chart up
        /// </summary>
        public static void RotateUp()
        {
            if (mainw != null)
            {
                mainw.RotateUp();
            }
        }

        /// <summary>
        /// call rotate chart down
        /// </summary>
        public static void RotateDown()
        {
            if (mainw != null)
            {
                mainw.RotateDown();
            }
        }

        /// <summary>
        /// call clear all drawn lines on both screens
        /// </summary>
        public static void Clear()
        {
            if (mainw != null)
            {
                mainw.userCanvasWindow.canvas.Children.Clear();
                mainw.presentationCanvasWindow.canvas.Children.Clear();
                messOnScreen = false;
            }
        }

        /// <summary>
        /// draw a line on both screens
        /// </summary>
        /// <param name="myLine"></param>
        public static void Draw(Line myLine)
        {
            if (mainw != null)
            {

                Line myLine1 = new Line();
                myLine1.Stroke = myLine.Stroke;
                myLine1.X1 = myLine.X1 + mainw.userCanvasWindow.canvas.Width / 2 - mainw.userCanvas.Width / 2;
                myLine1.X2 = myLine.X2 + mainw.userCanvasWindow.canvas.Width / 2 - mainw.userCanvas.Width / 2;
                myLine1.Y1 = myLine.Y1 + mainw.userCanvasWindow.canvas.Height / 2 - mainw.userCanvas.Height / 2;
                myLine1.Y2 = myLine.Y2 + mainw.userCanvasWindow.canvas.Height / 2 - mainw.userCanvas.Height / 2;
                myLine1.StrokeThickness = 10;

                mainw.userCanvasWindow.canvas.Children.Add(myLine1);

                Line myLine2 = new Line();
                myLine2.Stroke = myLine.Stroke;
                myLine2.X1 = myLine1.X1 * mainw.presentationCanvasWindow.canvas.Width / mainw.userCanvasWindow.canvas.Width;
                myLine2.X2 = myLine1.X2 * mainw.presentationCanvasWindow.canvas.Width / mainw.userCanvasWindow.canvas.Width;
                myLine2.Y1 = myLine1.Y1 * mainw.presentationCanvasWindow.canvas.Height / mainw.userCanvasWindow.canvas.Height;
                myLine2.Y2 = myLine1.Y2 * mainw.presentationCanvasWindow.canvas.Height / mainw.userCanvasWindow.canvas.Height;
                myLine2.StrokeThickness = 10;

                mainw.presentationCanvasWindow.canvas.Children.Add(myLine2);
                messOnScreen = true;
            }
        }

        /// <summary>
        /// draw a circle (point) on both screens
        /// </summary>
        /// <param name="myLine"></param>
        /// <param name="stroke"></param>
        /// <param name="fill"></param>
        public static void Draw(Point2D myPoint, Brush stroke, Brush fill)
        {
            if (mainw != null)
            {

                Point2D myPoint1 = new Point2D(myPoint.X, myPoint.Y);

                myPoint1.X = myPoint.X + mainw.userCanvasWindow.canvas.Width / 2 - mainw.userCanvas.Width / 2;
                myPoint1.Y = myPoint.Y + mainw.userCanvasWindow.canvas.Height / 2 - mainw.userCanvas.Height / 2;
                CanvasUtil.DrawEllipse(mainw.userCanvasWindow.canvas, myPoint1.X, myPoint1.Y, 50, 50, stroke, fill, null, 5);

                Point2D myPoint2 = new Point2D(myPoint.X, myPoint.Y);
                myPoint2.X = myPoint1.X * mainw.presentationCanvasWindow.canvas.Width / mainw.userCanvasWindow.canvas.Width;
                myPoint2.Y = myPoint1.Y * mainw.presentationCanvasWindow.canvas.Height / mainw.userCanvasWindow.canvas.Height;

                CanvasUtil.DrawEllipse(mainw.presentationCanvasWindow.canvas, myPoint2.X, myPoint2.Y, 50, 50, stroke, fill, null, 5);
            }
        }

        /// <summary>
        /// process a frame by calling the Draw method of selected group
        /// </summary>
        /// <param name="main"></param>
        public static void Process(MainWindow main)
        {
            if (mainw == null)
            {
                mainw = main;
            }

            SelectedGroup.Draw(main.userCanvas);
        }
    }
}
