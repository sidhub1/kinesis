using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace KineSis.UserInterface.Entities.Groups {
    interface Group {
        String Name {
            get;
        }

        void Draw(Canvas c);
    }
}
