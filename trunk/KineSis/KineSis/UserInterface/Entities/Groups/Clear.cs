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

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// group for clear all lines drawn on the screen. The Generic cannot be used instead of this because the availability of this group is conditional
    /// </summary>
    class Clear : Group
    {

        static readonly Clear instance = new Clear();

        static Clear()
        {
        }

        Clear()
        {
        }

        public static Clear Instance
        {
            get
            {
                return instance;
            }
        }

        String Group.Name
        {
            get
            {
                return "clear";
            }
        }

        /// <summary>
        /// available only if something was drawn
        /// </summary>
        Boolean Group.IsActive
        {
            get
            {
                return UIManager.messOnScreen;
            }
        }

        /// <summary>
        /// doesn't draw anything, because you can't enter in this group
        /// </summary>
        /// <param name="c"></param>
        void Group.Draw(Canvas c)
        {
        }
    }
}
