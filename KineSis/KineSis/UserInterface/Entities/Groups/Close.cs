using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KineSis.UserInterface.Entities.Groups {
    class Close : Group {
        String Group.Name {
            get {
                return "close";
            }
        }

        void Group.Draw(Canvas c) {
        }
    }
}
