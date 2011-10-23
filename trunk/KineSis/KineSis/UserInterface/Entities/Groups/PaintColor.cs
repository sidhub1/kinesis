using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using KineSis.Utils;

namespace KineSis.UserInterface.Entities.Groups {
    class PaintColor : Group {
        private String color;

        public Brush Brush{
            get {
                Brush brush = null;
                if (color.Equals("red")) {
                    brush = ColorUtil.FromHTML("#FFFF0000");
                } else if (color.Equals("blue")) {
                    brush = ColorUtil.FromHTML("#FF0000FF");
                } else if (color.Equals("green")) {
                    brush = ColorUtil.FromHTML("#FF00FF00");
                } else if (color.Equals("orange")) {
                    brush = ColorUtil.FromHTML("#FFFB940B");
                }

                return brush;
            }
        }

        String Group.Name {
            get {
                String name = "paint_" + color;

                return name;
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return true;
            }
        }

        public PaintColor(String color) {
            this.color = color;
        }

        void Group.Draw(Canvas c) {
        }
    }
}
