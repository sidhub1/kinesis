using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineSis.UserInterface.Entities;
using KineSis.UserInterface.Entities.Groups;
using System.Windows.Controls;
using KineSis.Profiles;
using System.Windows.Shapes;

namespace KineSis.UserInterface {
    class UIManager {

        private static Hand leftHand;
        private static Hand rightHand;
        private static int firstHand;
        private static Double delta = 1;
        private static MainWindow mainw = null;
        private static Boolean zoomFit = false;
        private static Boolean inPaint = false;


        
        private static Group mainGroup = Main.Instance;
        private static Group selectedGroup = mainGroup;


        public static Boolean InPaint {
            get {
                return inPaint;
            }

            set {
                inPaint = value;
            }
        }

        public static Boolean ZoomFit {
            get {
                return zoomFit;
            }

            set {
                zoomFit = value;
                if (mainw != null) {
                    mainw.ZoomFit();
                }
            }
        }

        public static Double Delta {
            get {
                return delta;
            }

            set {
                delta = value;
            }
        }

        public static int FirstHandNumber {
            get {
                return firstHand;
            }
        }

        public static Hand SecondHand {
            get {
                if (firstHand == 1 && rightHand != null && rightHand.IsSelected) {
                    return rightHand;
                } else if (firstHand == 2 && leftHand != null && leftHand.IsSelected) {
                    return leftHand;
                } else {
                    return null;
                }
            }
        }

        public static Hand FirstHand {
            get {
                if (firstHand == 1) {
                    return leftHand;
                } else if (firstHand == 2) {
                    return rightHand;
                } else {
                    return null;
                }
            }
        }

        public static Hand LeftHand {
            get {
                return leftHand;
            }
            
            set {
                leftHand = value;
                if ((rightHand == null || !rightHand.IsSelected) && (leftHand == null || !leftHand.IsSelected)) {
                    firstHand = 0;
                } else if (leftHand != null && leftHand.IsSelected && firstHand == 0) {
                    firstHand = 1;
                }
            }
        }

        public static Hand RightHand {
            get {
                return rightHand;
            }

            set {
                rightHand = value;
                if (( rightHand == null || !rightHand.IsSelected ) && ( leftHand == null || !leftHand.IsSelected )) {
                    firstHand = 0;
                } else if (rightHand != null && rightHand.IsSelected && firstHand == 0) {
                    firstHand = 2;
                }
            }
        }

        public static Group SelectedGroup {
            get {
                return selectedGroup;
            }

            set {
                selectedGroup = value;
            }
        }

        public static Group MainGroup {
            get {
                return mainGroup;
            }

            set {
                mainGroup = value;
            }
        }

        public static void ToNextPage() {
            if (mainw != null) {
                mainw.ToNextPage();
            }
        }

        public static void ToPreviousPage() {
            if (mainw != null) {
                mainw.ToPreviousPage();
            }
        }

        public static void GoToPage(int pageNumber) {
            if (mainw != null) {
                mainw.GoToPage(pageNumber);
            }
        }

        public static void ScrollRight() {
            if (mainw != null) {
                mainw.ScrollRight();
            }
        }

        public static void ScrollLeft() {
            if (mainw != null) {
                mainw.ScrollLeft();
            }
        }

        public static void ScrollUp() {
            if (mainw != null) {
                mainw.ScrollUp();
            }
        }

        public static void ScrollDown() {
            if (mainw != null) {
                mainw.ScrollDown();
            }
        }

        public static void ZoomIn() {
            if (mainw != null) {
                mainw.ZoomIn();
            }
        }

        public static void ZoomOut() {
            if (mainw != null) {
                mainw.ZoomOut();
            }
        }

        public static void GoToChart(int chartNumber) {
            if (mainw != null) {
                mainw.GoToChart(chartNumber);
            }
        }

        public static void CloseChart() {
            if (mainw != null) {
                mainw.CloseChart();
            }
        }

        public static void RotateRight() {
            if (mainw != null) {
                mainw.RotateRight();
            }
        }

        public static void RotateLeft() {
            if (mainw != null) {
                mainw.RotateLeft();
            }
        }

        public static void RotateUp() {
            if (mainw != null) {
                mainw.RotateUp();
            }
        }

        public static void RotateDown() {
            if (mainw != null) {
                mainw.RotateDown();
            }
        }

        public static void Clear() {
            if (mainw != null) {
                mainw.userCanvasWindow.canvas.Children.Clear();
                mainw.presentationCanvasWindow.canvas.Children.Clear();
            }
        }

        public static void Draw(Line myLine) {
            if (mainw != null) {

                Line myLine1 = new Line();
                myLine1.Stroke = myLine.Stroke;
                myLine1.X1 = myLine.X1 + mainw.userCanvasWindow.canvas.Width/2 - mainw.userCanvas.Width/2;
                myLine1.X2 = myLine.X2 + mainw.userCanvasWindow.canvas.Width/2 - mainw.userCanvas.Width/2;
                myLine1.Y1 = myLine.Y1 + mainw.userCanvasWindow.canvas.Height/2 - mainw.userCanvas.Height/2;
                myLine1.Y2 = myLine.Y2 + mainw.userCanvasWindow.canvas.Height/2 - mainw.userCanvas.Height/2;
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
            }
        }

        public static void Process(MainWindow main) {
            if (mainw == null) {
                mainw = main;
            }

            selectedGroup.Draw(main.userCanvas);
        }
    }
}
