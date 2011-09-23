using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineSis.Profiles;
using KineSis.Utils;
using System.Windows.Shapes;
using System.Windows.Media;

namespace KineSis.UserInterface.Entities.Groups {
    class Navigate : Group {

        private Group parent;

        private double initialX;
        private double initialY;
        private Boolean inSession = false;
        private Boolean selection = false;
        private Boolean preSelected = false;
        private Boolean leftSelected = false;
        private Boolean rightSelected = false;

        public Navigate(Group parent) {
            this.parent = parent;
        }

        String Group.Name {
            get {
                return "navigate";
            }
        }

        void Group.Draw(Canvas c) {
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
                        UIManager.SelectedGroup = parent;
                        preSelected = false;
                    }
                } else if (preSelected && ( UIManager.LeftHand.Y >= leftSelectionY + 75 || UIManager.LeftHand.Y <= leftSelectionY - 75 )) {
                    preSelected = false;
                }

                String caption = "back 2 " + parent.Name;

                CanvasUtil.DrawSubmenu(c, parent.Name, caption, true, null, centerX, centerY, centerDiameter, 150, leftAreaX, leftAreaY, rightAreaX, rightAreaY, leftSelectionX, leftSelectionY, rightSelectionX, rightSelectionY, preSelected);
            } else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {
                if (!inSession) {
                    initialX = UIManager.FirstHand.X;
                    initialY = UIManager.FirstHand.Y;
                    inSession = true;
                }

                System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
                System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;
                System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

                double centerX = initialX;
                double centerY = UIManager.FirstHand.Y;

                double leftAreaX = initialX - 250;
                double leftAreaY = UIManager.FirstHand.Y;

                double rightAreaX = initialX + 250;
                double rightAreaY = UIManager.FirstHand.Y;

                if (UIManager.FirstHand.X > rightAreaX - 75 && UIManager.FirstHand.X < rightAreaX + 75 && selection == false) {
                    rightSelected = true;
                    selection = true;
                    UIManager.ToNextPage();
                } else if (UIManager.FirstHand.X > leftAreaX - 75 && UIManager.FirstHand.X < leftAreaX + 75 && selection == false) {
                    leftSelected = true;
                    selection = true;
                    UIManager.ToPreviousPage();
                } else if (UIManager.FirstHand.X > centerX - 75 && UIManager.FirstHand.X < centerX + 75)  {
                    selection = false;
                    leftSelected = false;
                    rightSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, 150, 150, secondaryColor, fill, null);

                if (leftSelected) {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, 150, 150, secondaryColor, fill, Brushes.White);
                } else {
                    CanvasUtil.DrawEllipse(c, leftAreaX, leftAreaY, 150, 150, secondaryColor, fill, null);
                }

                if (rightSelected) {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, 150, 150, secondaryColor, fill, Brushes.White);
                } else {
                    CanvasUtil.DrawEllipse(c, rightAreaX, rightAreaY, 150, 150, secondaryColor, fill, null);
                }

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("left");
                CanvasUtil.DrawImageInCircle(c, image1, 150, leftAreaX, leftAreaY);

                System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("right");
                CanvasUtil.DrawImageInCircle(c, image2, 150, rightAreaX, rightAreaY);
            } else {
                inSession = false;
            }
        }
    }
}
