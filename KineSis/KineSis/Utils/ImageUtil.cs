using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace KineSis.Utils {

    /// <summary>
    /// Utility class for manipulating images from disk
    /// </summary>
    class ImageUtil {

        /// <summary>
        /// get an image from resources (image resources are located in [current_dir]\Drawables and all have .png extension)
        /// </summary>
        /// <param name="imageName">image name without extension</param>
        /// <returns></returns>
        public static Image GetResourceImage(String imageName) {
            Image image = new Image();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Directory.GetCurrentDirectory() + "\\Drawables\\"+imageName+".png");
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
        public static Image GetImage(String imagePath) {
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
