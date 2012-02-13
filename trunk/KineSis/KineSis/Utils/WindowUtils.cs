/*
   Copyright 2012 Alexandru Albu - sandu.albu@gmail.com

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

namespace KineSis.Utils
{
    static class WindowUtils
    {
        public static Screen[] Screens
        {
            get
            {
                return Screen.AllScreens;
            }
        }

        /// <summary>
        /// Switch a window to full screen state on the screen specified
        /// </summary>
        /// <param name="window">window to full screen</param>
        /// <param name="screen">on wich screen to extend the window</param>
        public static void FullScreen(Window window, int screen)
        {
            window.Topmost = true;
            window.Left = Screens[screen].Bounds.Left;
            window.Top = Screens[screen].Bounds.Top;
            window.WindowStyle = WindowStyle.None;
            window.Width = Screens[screen].Bounds.Width;
            window.Height = Screens[screen].Bounds.Height;
            window.ResizeMode = ResizeMode.NoResize;
        }

        /// <summary>
        /// Switch a form to full screen state on the screen specified
        /// </summary>
        /// <param name="window">window to full screen</param>
        /// <param name="screen">on wich screen to extend the window</param>
        public static void FullScreen(System.Windows.Forms.Form window, int screen)
        {
            window.Left = Screens[screen].Bounds.Left;
            window.Top = Screens[screen].Bounds.Top;
            window.Width = Screens[screen].Bounds.Width;
            window.Height = Screens[screen].Bounds.Height;
            window.FormBorderStyle = FormBorderStyle.None;
        }
    }
}
