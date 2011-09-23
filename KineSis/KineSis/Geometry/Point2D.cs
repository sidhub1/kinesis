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

namespace KineSis.Geometry {

    /// <summary>
    /// Point in a 2-dimensional space
    /// </summary>
    class Point2D {
        private Double x;
        private Double y;

        public Point2D(Double x, Double y) {
            this.x = x;
            this.y = y;
        }

        public Double X {
            get {
                return x;
            }

            set {
                x = value;
            }
        }

        public Double Y {
            get {
                return y;
            }

            set {
                y = value;
            }
        }
    }
}
