using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using KineSis.Profiles;
using System.Windows.Media;
using KineSis.Utils;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.IO;
using System.Windows.Forms;

namespace KineSis.UserInterface.Entities.Groups {
    class Generic : Group {
        private String name = "generic";

        public Generic(String name) {
            this.name = name;
        }

        String Group.Name {
            get {
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

        void Group.Draw(Canvas c) {

        }
    }
}
