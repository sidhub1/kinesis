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

namespace KineSis.Geometry
{

    /// <summary>
    /// Geometry Utility class
    /// </summary>
    class GeometryUtil
    {

        /// <summary>
        /// Get distance between 2 points in an XOY plane
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point b</param>
        /// <returns></returns>
        public static Double GetDistance2D(Point2D a, Point2D b)
        {
            Double dist = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            return dist;
        }
    }
}
