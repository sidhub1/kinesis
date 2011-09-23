using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KineSis.UserInterface.Entities {
    class Element {
        private String name;
        private Image thumbnail;

        public String Name {
            get {
                return name;
            }

            set {
                name = value;
            }
        }

        public Image Thumbnail {
            get {
                return thumbnail;
            }

            set {
                thumbnail = value;
            }
        }
    }
}
