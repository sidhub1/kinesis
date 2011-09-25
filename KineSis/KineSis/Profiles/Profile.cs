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
using System.Windows.Media;
using System.Globalization;
using KineSis.Utils;

namespace KineSis.Profiles {

    /// <summary>
    /// KineSis profile
    /// </summary>
    [Serializable]
    public class Profile {
        private String name;
        private String tempFolder;
        private int presentationScreen;
        private int userScreen;
        private String primaryColor;
        private String secondaryColor;
        private String backgroundColor;
        private String skeletonColor;
        private int slideWidth;
        private int chartWidth;
        private int chartHorizontalFaces;
        private int chartVerticalFaces;
        private List<Doc> documents = new List<Doc>();

        public List<Doc> Documents {
            get {
                return documents;
            }

            set {
                documents = value;
            }
        }

        public String Name {
            get {
                return name;
            }

            set {
                name = value;
            }
        }

        public String TempFolder {
            get {
                return tempFolder;
            }

            set {
                tempFolder = value;
            }
        }

        public int PresentationScreen {
            get {
                return presentationScreen;
            }

            set {
                presentationScreen = value;
            }
        }
        
        public int UserScreen {
            get {
                return userScreen;
            }

            set {
                userScreen = value;
            }
        }
        
        public Brush PrimaryColor {
            get {
                return ColorUtil.FromHTML(primaryColor);
            }

            set {
                primaryColor = value.ToString();
            }
        }

        public Brush SecondaryColor {
            get {
                return ColorUtil.FromHTML(secondaryColor);
            }

            set {
                secondaryColor = value.ToString();
            }
        }

        public Brush BackgroundColor {
            get {
                return ColorUtil.FromHTML(backgroundColor);
            }

            set {
                backgroundColor = value.ToString();
            }
        }

        public Brush SkeletonColor {
            get {
                return ColorUtil.FromHTML(skeletonColor);
            }

            set {
                skeletonColor = value.ToString();
            }
        }

        public int SlideWidth {
            get {
                return slideWidth;
            }

            set {
                slideWidth = value;
            }
        }

        public int ChartWidth {
            get {
                return chartWidth;
            }

            set {
                chartWidth = value;
            }
        }

        public int ChartHorizontalFaces {
            get {
                return chartHorizontalFaces;
            }

            set {
                chartHorizontalFaces = value;
            }
        }

        public int ChartVerticalFaces {
            get {
                return chartVerticalFaces;
            }

            set {
                chartVerticalFaces = value;
            }
        }
    }
}
