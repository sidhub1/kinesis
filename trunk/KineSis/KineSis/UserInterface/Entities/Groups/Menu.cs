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
using System.Windows.Shapes;
using KineSis.Profiles;
using System.Windows.Media;
using KineSis.Utils;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.IO;
using System.Windows.Forms;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// the main group. it's the start point in controlling presentation. as submenus, it have the Pointer, Shapes, Naigation and Paint.
    /// </summary>
    class Menu : Group
    {
        private static List<Group> groups = new List<Group>();

        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean upSelected = false;
        private Boolean downSelected = false;


        static readonly Menu instance = new Menu();

        static Menu()
        {
        }

        Menu()
        {
        }

        public static Menu Instance
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
                return "menu";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return true;
            }
        }

        public static List<Group> Groups
        {
            get
            {
                if (groups.Count == 0)
                {
                    Group pointer = Pointer.Instance;
                    groups.Add(pointer);
                    Group pages = Pages.Instance;
                    groups.Add(pages);
                    Group shapes = Shapes.Instance;
                    groups.Add(shapes);
                    Group paint = Paint.Instance;
                    groups.Add(paint);
                }

                return groups;
            }
        }

        /// <summary>
        /// Draw submenu depending on skeleton
        /// </summary>
        /// <param name="c"></param>
        void Group.Draw(Canvas c)
        {
            //if one hand is selecting
            if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected)
            {
                //create a menu session
                if (!UIManager.inMenuSession)
                {
                    UIManager.initialX = UIManager.FirstHand.X;
                    UIManager.initialY = UIManager.FirstHand.Y;
                    UIManager.inMenuSession = true;
                }

                //get colors
                System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
                System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;
                System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

                //compute submenus locations
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

                //handle the touches
                if (UIManager.FirstHand.X > rightAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < rightAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > rightAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < rightAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    rightSelected = true;
                    leftSelected = false;
                    upSelected = false;
                    downSelected = false;
                }
                else if (UIManager.FirstHand.X > leftAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < leftAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > leftAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < leftAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
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
                }
                else if (UIManager.FirstHand.X > downAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < downAreaX + UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y > downAreaY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < downAreaY + UIManager.SUBMENU_DIAMETER / 2)
                {
                    downSelected = true;
                    leftSelected = false;
                    rightSelected = false;
                    upSelected = false;
                }
                else if (UIManager.FirstHand.X > centerX - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.X < centerX + UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y > centerY - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.Y < centerY + UIManager.MENU_DIAMETER / 2)
                {
                    if (leftSelected && Groups[0].IsActive)
                    {
                        UIManager.SelectedGroup = Groups[0];
                    }
                    else if (rightSelected && Groups[1].IsActive)
                    {
                        UIManager.SelectedGroup = Groups[1];
                    }
                    else if (upSelected && Groups[2].IsActive)
                    {
                        UIManager.SelectedGroup = Groups[2];
                    }
                    else if (downSelected && Groups[3].IsActive)
                    {
                        UIManager.SelectedGroup = Groups[3];
                    }

                    leftSelected = false;
                    rightSelected = false;
                    upSelected = false;
                    downSelected = false;
                }

                //draw the submenus
                CanvasUtil.DrawEllipse(c, centerX, centerY, UIManager.MENU_DIAMETER, UIManager.MENU_DIAMETER, primaryColor, fill, null);

                if (leftSelected)
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[0].IsActive ? secondaryColor : Brushes.LightGray, fill, System.Windows.Media.Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[0].IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                if (rightSelected)
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[1].IsActive ? secondaryColor : Brushes.LightGray, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[1].IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                if (upSelected)
                {
                    CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[2].IsActive ? secondaryColor : Brushes.LightGray, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[2].IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                if (downSelected)
                {
                    CanvasUtil.DrawEllipse(c, downAreaX, downAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[3].IsActive ? secondaryColor : Brushes.LightGray, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, downAreaX, downAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, Groups[3].IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                //draw submenus images
                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage(((Group)instance).Name);
                CanvasUtil.DrawImageInCircle(c, image0, UIManager.MENU_DIAMETER, centerX, centerY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage(Groups[0].Name);
                CanvasUtil.DrawImageInCircle(c, image1, UIManager.SUBMENU_DIAMETER, leftAreaX, leftAreaY);

                System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage(Groups[1].Name);
                CanvasUtil.DrawImageInCircle(c, image2, UIManager.SUBMENU_DIAMETER, rightAreaX, rightAreaY);

                System.Windows.Controls.Image image3 = ImageUtil.GetResourceImage(Groups[2].Name);
                CanvasUtil.DrawImageInCircle(c, image3, UIManager.SUBMENU_DIAMETER, upAreaX, upAreaY);

                System.Windows.Controls.Image image4 = ImageUtil.GetResourceImage(Groups[3].Name);
                CanvasUtil.DrawImageInCircle(c, image4, UIManager.SUBMENU_DIAMETER, downAreaX, downAreaY);
            }
            else
            {
                //dispose menu session
                UIManager.inMenuSession = false;
                leftSelected = false;
                rightSelected = false;
                upSelected = false;
                downSelected = false;
            }
        }
    }
}
