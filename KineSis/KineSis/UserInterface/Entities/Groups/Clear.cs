using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KineSis.UserInterface.Entities.Groups {
    class Clear : Group{

        static readonly Clear instance = new Clear();

        static Clear() {
        }

        Clear() {
        }

        public static Clear Instance {
            get {
                return instance;
            }
        }

        String Group.Name {
            get {
                return "clear";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return UIManager.messOnScreen;
            }
        }

        void Group.Draw(Canvas c) {
        }
    }
}
