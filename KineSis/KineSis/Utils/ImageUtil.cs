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
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace KineSis.Utils
{

    /// <summary>
    /// Utility class for manipulating images from disk
    /// </summary>
    class ImageUtil
    {

        /// <summary>
        /// get an image from resources (image resources are located in [current_dir]\Drawables and all have .png extension)
        /// </summary>
        /// <param name="imageName">image name without extension</param>
        /// <returns></returns>
        public static Image GetResourceImage(String imageName)
        {
            if (imageName.CompareTo("menu") == 0) imageName = "main";
            if (imageName.CompareTo("minimal") == 0) imageName = "scroll";
            Image image = new Image();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Directory.GetCurrentDirectory() + "\\Drawables\\" + imageName + ".png");
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.UriSource = new Uri(fileInfo.FullName, UriKind.Relative);
            src.EndInit();
            image.Source = src;
            return image;
        }

        /// <summary>
        /// get an image from a specified path on disk
        /// </summary>
        /// <param name="imagePath">full image path</param>
        /// <returns></returns>
        public static Image GetImage(String imagePath)
        {
            Image image = new Image();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(imagePath);
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.UriSource = new Uri(fileInfo.FullName);
            src.EndInit();
            image.Source = src;
            return image;
        }
    }
}
