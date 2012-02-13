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
using System.Windows.Media;
using KineSis.Utils;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// colors for paint
    /// </summary>
    class PaintColor : Group
    {
        private String color;

        public Brush Brush
        {
            get
            {
                Brush brush = null;
                if (color.Equals("red"))
                {
                    brush = ColorUtil.FromHTML("#FFFF0000");
                }
                else if (color.Equals("blue"))
                {
                    brush = ColorUtil.FromHTML("#FF0000FF");
                }
                else if (color.Equals("green"))
                {
                    brush = ColorUtil.FromHTML("#FF00FF00");
                }
                else if (color.Equals("orange"))
                {
                    brush = ColorUtil.FromHTML("#FFFB940B");
                }

                return brush;
            }
        }

        String Group.Name
        {
            get
            {
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

        public PaintColor(String color)
        {
            this.color = color;
        }

        void Group.Draw(Canvas c)
        {
        }
    }
}
