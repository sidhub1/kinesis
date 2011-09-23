using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineSis.Utils;
using KineSis.Profiles;
using System.Windows.Media;

namespace KineSis.UserInterface.Entities.Groups {
    class Zoom : Group {
        String Group.Name {
            get {
                return "zoom";
            }
        }
        
        private Group parent;

        private double initialX;
        private double initialY;
        private Boolean inSession = false;
        private Boolean selection = false;
        private Boolean preSelected = false;
        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean upSelected = false;

        public Zoom(Group parent) {
            this.parent = parent;
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
                double centerY = initialY;

                double leftAreaX = initialX - 175;
                double leftAreaY = initialY;

                double rightAreaX = initialX + 175;
                double rightAreaY = initialY;

                double upAreaX = initialX;
                double upAreaY = initialY - 200;

                if (UIManager.FirstHand.X > rightAreaX - 100 && UIManager.FirstHand.X < rightAreaX + 100 && UIManager.FirstHand.Y > rightAreaY - 100 && UIManager.FirstHand.Y < rightAreaY + 100 && !UIManager.ZoomFit) {
                    rightSelected = true;
                    selection = true;
                    UIManager.ZoomIn();
                } else if (UIManager.FirstHand.X > leftAreaX - 100 && UIManager.FirstHand.X < leftAreaX + 100 && UIManager.FirstHand.Y > leftAreaY - 100 && UIManager.FirstHand.Y < leftAreaY + 100  && !UIManager.ZoomFit) {
                    leftSelected = true;
                    selection = true;
                    UIManager.ZoomOut();
                } else if (UIManager.FirstHand.X > upAreaX - 100 && UIManager.FirstHand.X < upAreaX + 100 && UIManager.FirstHand.Y > upAreaY - 100 && UIManager.FirstHand.Y < upAreaY + 100 && !selection && !upSelected) {
                    upSelected = true;
                    selection = true;
                    UIManager.ZoomFit = !UIManager.ZoomFit;
                } else if (UIManager.FirstHand.X > centerX - 100 && UIManager.FirstHand.X < centerX + 100 && UIManager.FirstHand.Y > centerY - 100 && UIManager.FirstHand.Y < centerY + 100) {
                    selection = false;
                    leftSelected = false;
                    rightSelected = false;
                    upSelected = false;
                }

                CanvasUtil.DrawEllipse(c, centerX, centerY, 200, 200, primaryColor, fill, null);
                String caption = UIManager.ZoomFit ? "zoom locked" : MainWindow.presentationZoom.ToString() + " %";

                CanvasUtil.DrawTextBlock(c, caption, 0.2 * 200, System.Windows.Media.Brushes.White, primaryColor, centerX, centerY + 0.75 * 200 / 2 * Math.Sqrt(2));

                if (!UIManager.ZoomFit) {
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

                    if (upSelected) {
                        CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, 150, 150, secondaryColor, fill, Brushes.White);
                    } else {
                        CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, 150, 150, secondaryColor, fill, null);
                    }

                    System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("zoom_out");
                    CanvasUtil.DrawImageInCircle(c, image1, 150, leftAreaX, leftAreaY);

                    System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("zoom_in");
                    CanvasUtil.DrawImageInCircle(c, image2, 150, rightAreaX, rightAreaY);

                    System.Windows.Controls.Image image3 = ImageUtil.GetResourceImage("fit");
                    CanvasUtil.DrawImageInCircle(c, image3, 150, upAreaX, upAreaY);
                } else {
                    if (upSelected) {
                        CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, 150, 150, primaryColor, fill, Brushes.White);
                    } else {
                        CanvasUtil.DrawEllipse(c, upAreaX, upAreaY, 150, 150, primaryColor, fill, null);
                    }

                    System.Windows.Controls.Image image3 = ImageUtil.GetResourceImage("unfit");
                    CanvasUtil.DrawImageInCircle(c, image3, 150, upAreaX, upAreaY);
                }
            } else {
                inSession = false;
            }
        }
    }
}
