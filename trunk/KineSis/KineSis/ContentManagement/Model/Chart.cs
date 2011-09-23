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
using KineSis.Utils;
using System.Drawing;

namespace KineSis.ContentManagement.Model {

    /// <summary>
    /// Chart model
    /// </summary>
    [Serializable]
    public class Chart {

        public String thumbUrl;
        private String title;

        private List<ChartHorizontalView> views = new List<ChartHorizontalView>();

        private String bitmap;
        private int hIndex = 0;
        private int vIndex = 0;

        /// <summary>
        /// The name of the chart, ussualy teh title
        /// </summary>
        public String Title {
            get {
                return title;
            }

            set {
                title = value;
            }
        }

        /// <summary>
        /// Set the thumbnail url
        /// </summary>
        /// <param name="thumbUrl"></param>
        public void SetThumbnailUrl(String thumbUrl) {
            this.thumbUrl = thumbUrl;
        }

        /// <summary>
        /// Get the thumbnail image
        /// </summary>
        public System.Windows.Controls.Image Thumbnail {
            get {
                System.Windows.Controls.Image bmp = null;
                if (thumbUrl != null && thumbUrl.Length > 0) {
                    try {
                        bmp = ImageUtil.GetImage(thumbUrl);
                    } catch (Exception) {
                    }
                }
                return bmp;
            }
        }

        /// <summary>
        /// Horizontal views of the chart
        /// </summary>
        public List<ChartHorizontalView> Views {
            get {
                return views;
            }

            set {
                views = value;
            }
        }

        /// <summary>
        /// Get current image of the chart
        /// </summary>
        /// <returns></returns>
        public String GetImageUrl() {
            if (bitmap == null) {
                bitmap = views[0].ImageUrl;
            }
            
            return bitmap;
        }

        /// <summary>
        /// Check if an image exists
        /// </summary>
        /// <returns></returns>
        public Boolean HasImage() {
            return bitmap != null;
        }

        /// <summary>
        /// Check if the chart has more up images
        /// </summary>
        /// <returns></returns>
        public Boolean HasUpImage() {
            Boolean has = false;
                if (vIndex < views[hIndex].Up.Count) {
                    has = true;
                }
            return has;
        }

        /// <summary>
        /// Get logically up image of the chart in rotation
        /// </summary>
        /// <returns></returns>
        public String GetUpImageUrl() {
            if (HasUpImage()) {
                vIndex++;
                if (vIndex == 0) {
                    bitmap = views[hIndex].ImageUrl;
                } else if (vIndex > 0) {
                    bitmap = views[hIndex].Up[vIndex - 1].ImageUrl;
                } else {
                    bitmap = views[hIndex].Down[-vIndex - 1].ImageUrl;
                }
            }
            return GetImageUrl();
        }

        /// <summary>
        /// Check if the chart has more down images
        /// </summary>
        /// <returns></returns>
        public Boolean HasDownImage() {
            Boolean has = false;
                if (-vIndex < views[hIndex].Down.Count) {
                    has = true;
                }
            return has;
        }

        /// <summary>
        /// Get logically down image of the chart in rotation
        /// </summary>
        /// <returns></returns>
        public String GetDownImageUrl() {
            if (HasDownImage()) {
                vIndex--;
                if (vIndex == 0) {
                    bitmap = views[hIndex].ImageUrl;
                } else if (vIndex > 0) {
                    bitmap = views[hIndex].Up[vIndex - 1].ImageUrl;
                } else {
                    bitmap = views[hIndex].Down[-vIndex - 1].ImageUrl;
                }
            }
            return GetImageUrl();
        }

        /// <summary>
        /// Check if the chart hare more left images
        /// </summary>
        /// <returns></returns>
        public Boolean HasLeftImage() {
            Boolean has = false;
                if (views.Count > 1) {
                    return true;
                }
            return has;
        }

        /// <summary>
        /// Get logically left image of the chart in rotation
        /// </summary>
        /// <returns></returns>
        public String GetLeftImageUrl() {
            if (HasLeftImage()) {
                if (hIndex == 0) {
                    hIndex = views.Count - 1;
                } else {
                    hIndex--;
                }

                if (vIndex == 0) {
                    bitmap = views[hIndex].ImageUrl;
                } else if (vIndex > 0) {
                    bitmap = views[hIndex].Up[vIndex - 1].ImageUrl;
                } else {
                    bitmap = views[hIndex].Down[-vIndex - 1].ImageUrl;
                }
            }
            return GetImageUrl();
        }

        /// <summary>
        /// Check if the chart has more right images
        /// </summary>
        /// <returns></returns>
        public Boolean HasRightImage() {
            Boolean has = false;
                if (views.Count > 1) {
                    return true;
                }
            return has;
        }

        /// <summary>
        /// Get logically right image of the chart in rotation
        /// </summary>
        /// <returns></returns>
        public String GetRightImageUrl() {
            if (HasRightImage()) {
                if (hIndex == views.Count - 1) {
                    hIndex = 0;
                } else {
                    hIndex++;
                }

                if (vIndex == 0) {
                    bitmap = views[hIndex].ImageUrl;
                } else if (vIndex > 0) {
                    bitmap = views[hIndex].Up[vIndex - 1].ImageUrl;
                } else {
                    bitmap = views[hIndex].Down[-vIndex - 1].ImageUrl;
                }
            }
            return GetImageUrl();
        }
    }
}