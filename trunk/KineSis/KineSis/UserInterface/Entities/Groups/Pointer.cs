/*
   Copyright 2011 Alexandru Albu - sandu.albu@gmail.com

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
using KineSis.Profiles;
using KineSis.Utils;
using System.Windows.Media;
using System.Collections;
using KineSis.Geometry;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// pointer group
    /// </summary>
    class Pointer : Group
    {
        private Boolean inSession = false;
        private Boolean leftSelected = false;
        private static Queue<Point2D> pointsLeft = new Queue<Point2D>();
        private static Queue<Point2D> pointsLeft2 = new Queue<Point2D>();

        static readonly Pointer instance = new Pointer();
        List<String> colors = new List<String>() { "#9999FF00", "#9999FF33", "#9999FF66", "#9999FF99", "#9999FFCC", "#9999FFFF", "#9999CCFF", "#999999FF", "#999966FF", "#999933FF", "#999900FF" };

        static Pointer()
        {
        }

        Pointer()
        {
        }

        public static Pointer Instance
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
                return "pointer";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return true;
            }
        }

        void Group.Draw(Canvas c)
        {
            UIManager.Clear();
            if (UIManager.ShowSecondaryMenu)
            {
                if (!UIManager.inMenuSession)
                {
                    UIManager.initialX = UIManager.FirstHand.X;
                    UIManager.initialY = UIManager.FirstHand.Y;
                    UIManager.inMenuSession = true;
                }

                System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
                System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;
                System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

                double centerX = UIManager.initialX;
                double centerY = UIManager.initialY;

                double leftAreaX = UIManager.initialX - 1.25 * UIManager.SUBMENU_DIAMETER;
                double leftAreaY = UIManager.initialY;

                if (UIManager.FirstHandNumber == 2)
                {
                    leftAreaX = UIManager.initialX + 1.25 * UIManager.SUBMENU_DIAMETER;
                }

                if (UIManager.FirstHand.X > leftAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < leftAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > leftAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < leftAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    leftSelected = true;

                }

                else if (UIManager.FirstHand.X > centerX - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.X < centerX + UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y > centerY - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y < centerY + UIManager.MENU_DIAMETER / 2)
                {
                    if (leftSelected)
                    {
                        UIManager.SelectedGroup = UIManager.MainGroup;
                    }


                    leftSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, UIManager.MENU_DIAMETER, UIManager.MENU_DIAMETER, primaryColor, fill, null);

                if (leftSelected)
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, UIManager.MainGroup.IsActive ? secondaryColor : Brushes.LightGray, fill, System.Windows.Media.Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, UIManager.MainGroup.IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage(((Group)this).Name);
                CanvasUtil.DrawImageInCircle(c, image0, UIManager.MENU_DIAMETER, centerX, centerY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage(UIManager.MainGroup.Name);
                CanvasUtil.DrawImageInCircle(c, image1, UIManager.SUBMENU_DIAMETER, leftAreaX, leftAreaY);
            }
            else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected && !UIManager.inMenuSession)
            {
                if (!inSession)
                {
                    UIManager.initialX = UIManager.FirstHand.X;
                    UIManager.initialY = UIManager.FirstHand.Y;
                    inSession = true;
                }

                System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
                System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;
                System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

                Brush color = null;

                int i = 0;
                foreach (Point2D p in pointsLeft)
                {
                    if (UIManager.FirstHandNumber == 1)
                    {
                        color = ColorUtil.FromHTML(colors[i]);
                    }
                    else
                    {
                        color = ColorUtil.FromHTML(colors[colors.Count - 1 - i]);
                    }
                    UIManager.Draw(p, color, Brushes.Transparent);
                    i++;
                }


                if (pointsLeft.Count > 10)
                {
                    pointsLeft.Dequeue();
                }

                Point2D currentPoint = new Point2D(UIManager.FirstHand.X, UIManager.FirstHand.Y);
                pointsLeft.Enqueue(currentPoint);
            }
            else
            {
                pointsLeft.Clear();
                inSession = false;
                UIManager.inMenuSession = false;
                leftSelected = false;
            }
        }
    }
}
