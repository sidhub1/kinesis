/*
   Copyright 2011 Alexandru Albu - http://code.google.com/p/kinesis/

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
using KineSis.Profiles;
using System.Windows.Media;
using KineSis.ContentManagement.Model;
using System.Threading;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// group for selecting pages or charts
    /// </summary>
    class Select : Group
    {
        String Group.Name
        {
            get
            {
                return "select";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                if (parent is Shapes)
                {
                    return UIManager.ActiveDocument != null && (UIManager.ActiveDocumentChart == null || UIManager.ActiveDocument.Pages[UIManager.ActiveDocumentPage].Charts.Count > 1);
                }
                else if (parent is Pages)
                {
                    return UIManager.ActiveDocument != null && UIManager.ActiveDocument.Pages.Count > 1;
                }
                else
                {
                    return true;
                }
            }
        }

        private Group parent;
        private Boolean inSession = false;
        private Boolean selection = false;
        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean selSelected = false;
        private int pageIndex = 0;



        public Select(Group parent)
        {
            this.parent = parent;
        }

        void Group.Draw(Canvas c)
        {
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
                        UIManager.SelectedGroup = parent;
                    }


                    leftSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, UIManager.MENU_DIAMETER, UIManager.MENU_DIAMETER, primaryColor, fill, null);

                if (leftSelected)
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, parent.IsActive ? secondaryColor : Brushes.LightGray, fill, System.Windows.Media.Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, parent.IsActive ? secondaryColor : Brushes.LightGray, fill, null);
                }

                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage(((Group)this).Name);
                CanvasUtil.DrawImageInCircle(c, image0, UIManager.MENU_DIAMETER, centerX, centerY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage(parent.Name);
                CanvasUtil.DrawImageInCircle(c, image1, UIManager.SUBMENU_DIAMETER, leftAreaX, leftAreaY);
            }
            else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected && GetElements().Count > 0 && !UIManager.inMenuSession)
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

                double centerX = UIManager.initialX;
                double centerY = UIManager.FirstHand.Y;

                double selX = UIManager.initialX;
                double selY = UIManager.initialY - 1.25 * UIManager.SUBMENU_DIAMETER;

                double leftAreaX = UIManager.initialX - 1.25 * UIManager.SUBMENU_DIAMETER;
                double leftAreaY = UIManager.FirstHand.Y;

                double rightAreaX = UIManager.initialX + 1.25 * UIManager.SUBMENU_DIAMETER;
                double rightAreaY = UIManager.FirstHand.Y;

                double selectionAreaX = c.Width / 2;
                double selectionAreaY = c.Height - UIManager.MENU_DIAMETER;

                double prevAreaX = selectionAreaX - 1.25 * UIManager.SUBMENU_DIAMETER;
                double prevAreaY = c.Height - UIManager.SUBMENU_DIAMETER;

                double nextAreaX = selectionAreaX + 1.25 * UIManager.SUBMENU_DIAMETER;
                double nextAreaY = c.Height - UIManager.SUBMENU_DIAMETER;

                if (UIManager.FirstHand.Y > selY - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.Y < selY + UIManager.SUBMENU_DIAMETER / 2 && selSelected == false)
                {
                    selSelected = true;
                    ExecuteSelect();
                }
                else if (UIManager.FirstHand.X > rightAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < rightAreaX + UIManager.SUBMENU_DIAMETER / 2 && selection == false)
                {
                    rightSelected = true;
                    selection = true;
                    IncrementIndex();
                }
                else if (UIManager.FirstHand.X > leftAreaX - UIManager.SUBMENU_DIAMETER / 2 && UIManager.FirstHand.X < leftAreaX + UIManager.SUBMENU_DIAMETER / 2 && selection == false)
                {
                    leftSelected = true;
                    selection = true;
                    DecrementIndex();
                }
                else if (UIManager.FirstHand.X > centerX - UIManager.MENU_DIAMETER / 2 && UIManager.FirstHand.X < centerX + UIManager.MENU_DIAMETER / 2)
                {
                    if (UIManager.FirstHand.Y >= selY + UIManager.SUBMENU_DIAMETER / 2)
                    {
                        selSelected = false;
                    }
                    selection = false;
                    leftSelected = false;
                    rightSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, UIManager.MENU_DIAMETER, UIManager.MENU_DIAMETER, primaryColor, fill, null);

                if (leftSelected)
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                if (rightSelected)
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }

                if (selSelected)
                {
                    CanvasUtil.DrawEllipse(c, selX, selY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, Brushes.White);
                }
                else
                {
                    CanvasUtil.DrawEllipse(c, selX, selY, UIManager.SUBMENU_DIAMETER, UIManager.SUBMENU_DIAMETER, secondaryColor, fill, null);
                }


                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage("select");
                CanvasUtil.DrawImageInCircle(c, image0, UIManager.SUBMENU_DIAMETER, selX, selY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("left");
                CanvasUtil.DrawImageInCircle(c, image1, UIManager.SUBMENU_DIAMETER, leftAreaX, leftAreaY);

                System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("right");
                CanvasUtil.DrawImageInCircle(c, image2, UIManager.SUBMENU_DIAMETER, rightAreaX, rightAreaY);


                if (GetElements().Count > 2)
                {

                    CanvasUtil.DrawImageInRectangle(c, GetPrevElement().Thumbnail, secondaryColor, prevAreaX, prevAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.3);
                    CanvasUtil.DrawImageInRectangle(c, GetNextElement().Thumbnail, secondaryColor, nextAreaX, nextAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.3);
                    CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.9);

                }
                else if (GetElements().Count == 2)
                {
                    if (pageIndex == 0)
                    {
                        CanvasUtil.DrawImageInRectangle(c, GetNextElement().Thumbnail, secondaryColor, nextAreaX, nextAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.3);
                        CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.9);
                    }
                    else
                    {
                        CanvasUtil.DrawImageInRectangle(c, GetPrevElement().Thumbnail, secondaryColor, prevAreaX, prevAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.3);
                        CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.9);
                    }
                }
                else if (GetElements().Count == 1)
                {
                    CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, UIManager.SUBMENU_DIAMETER * 2, 0.9);
                }

                CanvasUtil.DrawTextBlock(c, GetElement().Name, 0.1 * UIManager.SUBMENU_DIAMETER * 2, System.Windows.Media.Brushes.White, primaryColor, selectionAreaX, selectionAreaY - 1.25 * UIManager.SUBMENU_DIAMETER);

            }
            else
            {
                inSession = false;
                UIManager.inMenuSession = false;
                selection = false;
                leftSelected = false;
                rightSelected = false;
            }
        }

        private void ExecuteSelect()
        {
            if (parent is Pages)
            {
                UIManager.GoToPage(pageIndex);
            }
            else if (parent is Shapes)
            {
                UIManager.GoToChart(pageIndex);
            }
        }

        private List<Element> GetElements()
        {
            if (parent is Pages)
            {
                return Pages.Elements;
            }
            else if (parent is Shapes)
            {
                return Shapes.Elements;
            }
            else
            {
                return new List<Element>();
            }
        }

        private void IncrementIndex()
        {
            if (pageIndex < GetElements().Count - 1)
            {
                pageIndex++;
            }
            else if (pageIndex == GetElements().Count - 1)
            {
                pageIndex = 0;
            }
        }

        private void DecrementIndex()
        {
            if (pageIndex > 0)
            {
                pageIndex--;
            }
            else if (pageIndex == 0)
            {
                pageIndex = GetElements().Count - 1;
            }
        }

        private Element GetElement()
        {
            Element e = GetElements()[pageIndex];
            if (e.Thumbnail == null)
            {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            }
            return e;
        }

        private Element GetPrevElement()
        {
            int prevIndex = pageIndex > 0 ? pageIndex - 1 : GetElements().Count - 1;
            Element e = GetElements()[prevIndex];
            if (e.Thumbnail == null)
            {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            }
            return e;
        }

        private Element GetNextElement()
        {
            int nextIndex = pageIndex < GetElements().Count - 1 ? pageIndex + 1 : 0;
            Element e = GetElements()[nextIndex];
            if (e.Thumbnail == null)
            {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            }
            return e;
        }
    }
}
