/*
   Copyright 2012 Alexandru Albu - sandu.albu@gmail.com

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

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// this group is generic. It is used for mocking groups which doesn't have their own subgroups. So that group will act like a button
    /// </summary>
    class Generic : Group
    {
        private String name = "generic";

        public Generic(String name)
        {
            this.name = name;
        }

        String Group.Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// always active
        /// </summary>
        Boolean Group.IsActive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// cant' enter in a generic group. It will never be the active group
        /// </summary>
        /// <param name="c"></param>
        void Group.Draw(Canvas c)
        {

        }
    }
}
