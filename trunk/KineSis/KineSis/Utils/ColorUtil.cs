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
using System.Windows.Media;
using System.Globalization;

namespace KineSis.Utils
{
    /// <summary>
    /// utility class for colors
    /// </summary>
    class ColorUtil
    {
        /// <summary>
        /// create a System.Windows.Media.Brush from a color hex code
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Brush FromHTML(String hexColor)
        {
            byte a = byte.Parse(hexColor.Substring(1, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hexColor.Substring(3, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexColor.Substring(5, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexColor.Substring(7, 2), NumberStyles.HexNumber);
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
        }

        /// <summary>
        /// create a System.Drawing.Color from a color hex code
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static System.Drawing.Color DrawingColorFromHTML(String hexColor)
        {
            byte a = byte.Parse(hexColor.Substring(1, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hexColor.Substring(3, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexColor.Substring(5, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexColor.Substring(7, 2), NumberStyles.HexNumber);
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }
    }
}
