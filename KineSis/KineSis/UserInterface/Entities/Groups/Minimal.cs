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
using KineSis.Profiles;
using KineSis.Utils;
using System.Windows.Media;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// minimal group is combining the user view and presentation. It is designed to fit in only one screen and to use the minimum of control over document (navigate and scroll)
    /// </summary>
    class Minimal : Group
    {
        String Group.Name
        {
            get
            {
                return "minimal";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return true;
            }
        }

        static readonly Minimal instance = new Minimal();

        static Minimal()
        {
        }

        Minimal()
        {
        }

        public static Minimal Instance
        {
            get
            {
                return instance;
            }
        }
        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean upSelected = false;
        private Boolean downSelected = false;

        private static List<Group> groups = new List<Group>();

        void Group.Draw(Canvas c)
        {
            if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected)
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

                double rightAreaX = UIManager.initialX + 1.25 * UIManager.SUBMENU_DIAMETER;
                double rightAreaY = UIManager.initialY;

                double upAreaX = UIManager.initialX;
                double upAreaY = UIManager.initialY - 1.25 * UIManager.SUBMENU_DIAMETER;

                double downAreaX = UIManager.initialX;
                double downAreaY = UIManager.initialY + 1.25 * UIManager.SUBMENU_DIAMETER;

                if (UIManager.FirstHand.X > rightAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < rightAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > rightAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < rightAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    if (UIManager.MinimalScrollLock)
                    {
                        UIManager.ScrollRight();
                    }
                    else if (!rightSelected)
                    {
                        UIManager.ToNextPage();
                    }
                    rightSelected = true;
                    leftSelected = false;
                    upSelected = false;
                    downSelected = false;
                }
                else if (UIManager.FirstHand.X > leftAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < leftAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > leftAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < leftAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    if (UIManager.MinimalScrollLock)
                    {
                        UIManager.ScrollLeft();
                    }
                    else if (!leftSelected)
                    {
                        UIManager.ToPreviousPage();
                    }
                    leftSelected = true;
                    rightSelected = false;
                    upSelected = false;
                    downSelected = false;
                }
                else if (UIManager.FirstHand.X > upAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < upAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > upAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < upAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    upSelected = true;
                    leftSelected = false;
                    rightSelected = false;
                    downSelected = false;
                    UIManager.ScrollUp();
                }
                else if (UIManager.FirstHand.X > downAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < downAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > downAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < downAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    downSelected = true;
                    leftSelected = false;
                    rightSelected = false;
                    upSelected = false;
                    UIManager.ScrollDown();
                }
                else if (UIManager.FirstHand.X > centerX - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.X < centerX + UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y > centerY - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y < centerY + UIManager.MENU_DIAMETER / 2)
                {
                    leftSelected = false;
                    rightSelected = false;
                    upSelected = false;
                    downSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, UIManager.MENU_DIAMETER, UIManager.MENU_DIAMETER, primaryColor, fill, null);

                if (leftSelected)
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, secondaryColor);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                if (rightSelected)
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, secondaryColor);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                if (upSelected)
                {
                    CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, secondaryColor);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                if (downSelected)
                {
                    CanvasUtil.DrawEllipse(c, downAreaX, downAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, secondaryColor);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, downAreaX, downAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage(UIManager.MinimalScrollLock ? "scroll" : "navigate");
                CanvasUtil.DrawImageInCircle(c, image0, UIManager.MENU_DIAMETER, centerX, centerY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("left");
                CanvasUtil.DrawImageInCircle(c, image1, UIManager.SUBMENU_DIAMETER, leftAreaX, leftAreaY);

                System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("right");
                CanvasUtil.DrawImageInCircle(c, image2, UIManager.SUBMENU_DIAMETER, rightAreaX, rightAreaY);

                System.Windows.Controls.Image image3 = ImageUtil.GetResourceImage("up");
                CanvasUtil.DrawImageInCircle(c, image3, UIManager.SUBMENU_DIAMETER, upAreaX, upAreaY);

                System.Windows.Controls.Image image4 = ImageUtil.GetResourceImage("down");
                CanvasUtil.DrawImageInCircle(c, image4, UIManager.SUBMENU_DIAMETER, downAreaX, downAreaY);
            }
            else
            {
                UIManager.inMenuSession = false;
                leftSelected = false;
                rightSelected = false;
                upSelected = false;
                downSelected = false;
            }
        }
    }
}
