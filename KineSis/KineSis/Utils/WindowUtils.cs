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
using System.Windows.Forms;
using System.Windows;

namespace KineSis.Utils {
    static class WindowUtils {
        public static Screen[] Screens {
            get {
                return Screen.AllScreens;
            }
        }

        /// <summary>
        /// Get the number of screen where the Window is shown
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static int DetectScreen(Window form) {
            int screen = 0;

            double right = form.Left + form.Width;
            double left = form.Left;
            double center = ( left + right ) / 2;
            if (Screens[0].Bounds.Left <= center && center <= Screens[0].Bounds.Right) {
                screen = 0;
            } else {
                screen = 1;
            }
            return screen;
        }

        /// <summary>
        /// Switch a window to full screen state on the screen where currently is
        /// </summary>
        /// <param name="window">window to full screen</param>
        /// <param name="fullscreen">boolean indicating the current state of the window</param>
        /// <param name="originalWidth">original width of the window before full screen</param>
        /// <param name="originalHeight">original height of the window before full screen</param>
        /// <param name="originalLeft">original left of the window before full screen</param>
        /// <param name="originalTop">original top of the window before full screen</param>
        public static void FullScreen(Window window, Boolean fullscreen, Double originalWidth, Double originalHeight, Double originalLeft, Double originalTop) {
            int screen = DetectScreen(window);
            if (fullscreen == false) {
                window.Topmost = true;
                window.Left = Screens[screen].Bounds.Left;
                window.Top = Screens[screen].Bounds.Top;
                window.WindowStyle = WindowStyle.None;
                window.Width = Screens[screen].Bounds.Width;
                window.Height = Screens[screen].Bounds.Height;
                window.ResizeMode = ResizeMode.NoResize;
                fullscreen = true;
            } else {
                window.Topmost = false;
                window.Left = originalLeft;
                window.Top = originalTop;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.Width = originalWidth;
                window.Height = originalHeight;
                fullscreen = false;
            }
        }
    }
}
