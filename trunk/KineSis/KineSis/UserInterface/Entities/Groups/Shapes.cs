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
using KineSis.Utils;
using System.Windows.Media;
using KineSis.Profiles;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// group for shapes. Submenus for selecting a chart, rotation and closing.
    /// </summary>
    class Shapes : Group
    {

        private static List<Group> groups = new List<Group>();
        private int pos = 0;
        private Boolean posChanged = false;
        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean upSelected = false;
        private Boolean downSelected = false;
        static readonly Shapes instance = new Shapes();

        String Group.Name
        {
            get
            {
                return "shapes";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return UIManager.ActiveDocument != null && UIManager.ActiveDocument.Pages[UIManager.ActiveDocumentPage].Charts.Count > 0;
            }
        }

        static Shapes()
        {
        }

        Shapes()
        {
        }

        public static Shapes Instance
        {
            get
            {
                return instance;
            }
        }

        private static List<Element> elements;

        public static List<Element> Elements
        {
            get
            {
                return elements;
            }

            set
            {
                elements = value;
            }
        }

        public static List<Group> Groups
        {
            get
            {
                if (groups.Count == 0)
                {
                    Group main = UIManager.MainGroup;
                    groups.Add(main);
                    Group close = new Close();
                    groups.Add(close);
                    Group select = new Select(Shapes.Instance);
                    groups.Add(select);
                    Group rotate = new Rotate(Shapes.Instance);
                    groups.Add(rotate);
                }

                return groups;
            }
        }

        void Group.Draw(Canvas c)
        {
            if (/*UIManager.SecondHand != null &&*/ UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected)
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
                        UIManager.CloseChart();
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
                UIManager.inMenuSession = false;
                leftSelected = false;
                rightSelected = false;
                upSelected = false;
                downSelected = false;
            }

            /*
            if (UIManager.SecondHand != null && UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {

                double centerX = ( UIManager.FirstHand.X + UIManager.SecondHand.X ) / 2;
                double centerY = ( UIManager.FirstHand.Y + UIManager.SecondHand.Y ) / 2;

                Double delta = Math.Abs(UIManager.Delta) + 1;

                Double centerDiameter = Math.Sqrt(Math.Pow(UIManager.FirstHand.X - UIManager.SecondHand.X, 2) + Math.Pow(UIManager.FirstHand.Y - UIManager.SecondHand.Y, 2)) - 150;

                if (centerDiameter > 2 * ( 1.75 * delta - 225 )) {
                    centerDiameter = 2 * ( 1.75 * delta - 225 );
                }
                if (centerDiameter <= 0) {
                    centerDiameter = 1;
                }

                double rightAreaX = UIManager.RightHand.X;
                double rightAreaY = UIManager.LeftHand.Y;

                if (rightAreaX > centerX + 1.75 * delta - 150) {
                    rightAreaX = centerX + 1.75 * delta - 150;
                }

                double rightSelectionX = centerX + 1.75 * delta;
                double rightSelectionY = centerY;

                double leftSelectionX = centerX - 1.75 * delta;
                double leftSelectionY = centerY;

                double leftAreaX = UIManager.LeftHand.X;
                double leftAreaY = UIManager.RightHand.Y;

                if (leftAreaX < centerX - 1.75 * delta + 150) {
                    leftAreaX = centerX - 1.75 * delta + 150;
                }

                if (UIManager.LeftHand.X < leftSelectionX + 75 && UIManager.LeftHand.Y < leftSelectionY + 75 && UIManager.LeftHand.Y > leftSelectionY - 75) {
                    preSelected = true;
                } else if (UIManager.LeftHand.X > leftSelectionX + 150 && UIManager.LeftHand.Y < leftSelectionY + 75 && UIManager.LeftHand.Y > leftSelectionY - 75) {
                    if (preSelected) {
                        if (Groups[pos].Name.Equals("close")) {
                            UIManager.CloseChart();
                        } else {
                            UIManager.SelectedGroup = Groups[pos];
                        }
                        preSelected = false;
                        pos = 0;
                        posChanged = false;
                    }
                } else if (preSelected && ( UIManager.LeftHand.Y >= leftSelectionY + 75 || UIManager.LeftHand.Y <= leftSelectionY - 75 )) {
                    preSelected = false;
                }

                if (UIManager.LeftHand.Y > leftAreaY - 75 && UIManager.LeftHand.Y < leftAreaY + 75) {
                    posChanged = false;
                } else if (UIManager.LeftHand.Y <= leftAreaY - 75 && !preSelected) {
                    IncrementPos();
                    posChanged = true;
                } else if (UIManager.LeftHand.Y >= leftAreaY + 75 && !preSelected) {
                    DecrementPos();
                    posChanged = true;
                }

                String caption = null;
                if (GetSubmenu().Equals(UIManager.MainGroup.Name)) {
                    caption = "back 2 " + GetSubmenu();
                }

                CanvasUtil.DrawSubmenu(c, GetSubmenu(), caption, true, null, centerX, centerY, centerDiameter, 150, leftAreaX, leftAreaY, rightAreaX, rightAreaY, leftSelectionX, leftSelectionY, rightSelectionX, rightSelectionY, preSelected);
            }*/
        }

        private void IncrementPos()
        {
            if (pos < Groups.Count - 1 && !posChanged)
            {
                pos++;
            }
            else if (pos == Groups.Count - 1 && !posChanged)
            {
                pos = 0;
            }
        }

        private void DecrementPos()
        {
            if (pos > 0 && !posChanged)
            {
                pos--;
            }
            else if (pos == 0 && !posChanged)
            {
                pos = Groups.Count - 1;
            }
        }

        private String GetSubmenu()
        {
            return Groups[pos].Name;
        }
    }
}
