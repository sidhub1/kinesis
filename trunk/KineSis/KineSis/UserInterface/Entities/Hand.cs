using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineSis.UserInterface.Entities {
    class Hand {
        private Double x;
        private Double y;
        private Boolean isSelected;

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

        public Boolean IsSelected {
            get {
                return isSelected;
            }

            set {
                isSelected = value;
            }
        }
    }
}
