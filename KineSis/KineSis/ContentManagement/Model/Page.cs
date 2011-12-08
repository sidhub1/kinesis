/*
   Copyright 2011 Alexandru Albu - sandu.albu@gmail.com

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
using System.Windows.Media.Imaging;

namespace KineSis.ContentManagement.Model
{

    /// <summary>
    /// KineSis Document page
    /// </summary>
    [Serializable]
    public class Page
    {
        private String name;
        private String location;
        private String locationNoZoom;
        private List<Chart> charts = new List<Chart>();
        public String thumbUrl;

        /// <summary>
        /// Set the url of the thumbnail image
        /// </summary>
        /// <param name="thumbUrl"></param>
        public void SetThumbnailUrl(String thumbUrl)
        {
            this.thumbUrl = thumbUrl;
        }

        /// <summary>
        /// Get thumbnail image
        /// </summary>
        public System.Windows.Controls.Image Thumbnail
        {
            get
            {
                System.Windows.Controls.Image bmp = null;
                if (thumbUrl != null && thumbUrl.Length > 0)
                {
                    try
                    {
                        bmp = ImageUtil.GetImage(thumbUrl);
                    }
                    catch (Exception)
                    {

                    }
                }
                return bmp;
            }
        }

        /// <summary>
        /// Location of the page in Zoom Locked mode (fit to screen, scroll not available)
        /// </summary>
        public String LocationNoZoom
        {
            get
            {
                if (locationNoZoom != null && locationNoZoom.Length > 0)
                {
                    return locationNoZoom;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                locationNoZoom = value;
            }
        }

        /// <summary>
        /// Name of the page / slide
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Location of the page
        /// </summary>
        public String Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        /// <summary>
        /// Charts on this page
        /// </summary>
        public List<Chart> Charts
        {
            get
            {
                return charts;
            }

            set
            {
                charts = value;
            }
        }
    }
}
