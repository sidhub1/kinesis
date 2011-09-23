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
using Microsoft.Office.Interop.PowerPoint;

namespace KineSis.ContentManagement.Utils {

    /// <summary>
    /// Images constants class. Provides extensions, filters and formats used by document helpers in exporting pages and shapes
    /// </summary>
    class ImageUtil {
        public static String BMP_EXTENSION = ".bmp";
        public static String BMP_FILTER = "bmp";
        public static PpShapeFormat BMP_FORMAT = PpShapeFormat.ppShapeFormatBMP;

        public static String GIF_EXTENSION = ".gif";
        public static String GIF_FILTER = "gif";
        public static PpShapeFormat GIF_FORMAT = PpShapeFormat.ppShapeFormatGIF;

        public static String JPG_EXTENSION = ".jpg";
        public static String JPG_FILTER = "jpg";
        public static PpShapeFormat JPG_FORMAT = PpShapeFormat.ppShapeFormatJPG;

        public static String PNG_EXTENSION = ".png";
        public static String PNG_FILTER = "png";
        public static PpShapeFormat PNG_FORMAT = PpShapeFormat.ppShapeFormatPNG;
    }
}
