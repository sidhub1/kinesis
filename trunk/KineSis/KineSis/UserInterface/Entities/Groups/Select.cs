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

namespace KineSis.UserInterface.Entities.Groups {
    class Select : Group {
        String Group.Name {
            get {
                return "select";
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
        private Boolean selSelected = false;
        private int pageIndex = 0;

        

        public Select(Group parent) {
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
            } else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected && GetElements().Count > 0) {
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

                double selX = initialX;
                double selY = initialY - 250;

                double leftAreaX = initialX - 250;
                double leftAreaY = UIManager.FirstHand.Y;

                double rightAreaX = initialX + 250;
                double rightAreaY = UIManager.FirstHand.Y;

                double selectionAreaX = initialX;
                double selectionAreaY = UIManager.FirstHand.Y + 250;

                double prevAreaX = initialX - 250;
                double prevAreaY = UIManager.FirstHand.Y + 250;

                double nextAreaX = initialX + 250;
                double nextAreaY = UIManager.FirstHand.Y + 250;

                if (UIManager.FirstHand.Y > selY - 100 && UIManager.FirstHand.Y < selY + 100 && selSelected == false) {
                    selSelected = true;
                    ExecuteSelect();
                    //UIManager.GoToPage(pageIndex);
                } else if (UIManager.FirstHand.X > rightAreaX - 75 && UIManager.FirstHand.X < rightAreaX + 75 && selection == false) {
                    rightSelected = true;
                    selection = true;
                    IncrementIndex();
                } else if (UIManager.FirstHand.X > leftAreaX - 75 && UIManager.FirstHand.X < leftAreaX + 75 && selection == false) {
                    leftSelected = true;
                    selection = true;
                    DecrementIndex();
                } else if (UIManager.FirstHand.X > centerX - 75 && UIManager.FirstHand.X < centerX + 75) {
                    if (UIManager.FirstHand.Y >= selY + 100) {
                        selSelected = false;
                    }
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

                if (selSelected) {
                    CanvasUtil.DrawEllipse(c, selX, selY, 200, 200, primaryColor, fill, Brushes.White);
                } else {
                    CanvasUtil.DrawEllipse(c, selX, selY, 200, 200, primaryColor, fill, null);
                }

                

                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage("select");
                CanvasUtil.DrawImageInCircle(c, image0, 200, selX, selY);

                System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("left");
                CanvasUtil.DrawImageInCircle(c, image1, 150, leftAreaX, leftAreaY);

                System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("right");
                CanvasUtil.DrawImageInCircle(c, image2, 150, rightAreaX, rightAreaY);


                if (GetElements().Count > 2) {

                    CanvasUtil.DrawImageInRectangle(c, GetPrevElement().Thumbnail, secondaryColor, prevAreaX, prevAreaY, 350, 0.3);
                    CanvasUtil.DrawImageInRectangle(c, GetNextElement().Thumbnail, secondaryColor, nextAreaX, nextAreaY, 350, 0.3);
                    CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, 350, 0.9);

                } else if ( GetElements().Count == 2 ) {
                    if (pageIndex == 0) {
                        CanvasUtil.DrawImageInRectangle(c, GetNextElement().Thumbnail, secondaryColor, nextAreaX, nextAreaY, 350, 0.3);
                        CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, 350, 0.9);
                    } else {
                        CanvasUtil.DrawImageInRectangle(c, GetPrevElement().Thumbnail, secondaryColor, prevAreaX, prevAreaY, 350, 0.3);
                        CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, 350, 0.9);
                    }
                } else if (GetElements().Count == 1) {
                    CanvasUtil.DrawImageInRectangle(c, GetElement().Thumbnail, primaryColor, selectionAreaX, selectionAreaY, 350, 0.9);
                }

                CanvasUtil.DrawTextBlock(c, GetElement().Name, 0.1 * 350, System.Windows.Media.Brushes.White, primaryColor, centerX, centerY + 450);

            } else {
                inSession = false;
            }
        }

        private void ExecuteSelect() {
            if (parent is Pages) {
                UIManager.GoToPage(pageIndex);
            } else if (parent is Shapes) {
                UIManager.GoToChart(pageIndex);
            }
        }

        private List<Element> GetElements() {
            if (parent is Pages) {
                return Pages.Elements;
            } else if (parent is Shapes) {
                return Shapes.Elements;
            } else {
                return new List<Element>();
            }
        }

        private void IncrementIndex() {
            if (pageIndex < GetElements().Count - 1) {
                pageIndex++;
            } else if (pageIndex == GetElements().Count - 1) {
                pageIndex = 0;
            }
        }

        private void DecrementIndex() {
            if (pageIndex > 0) {
                pageIndex--;
            } else if (pageIndex == 0) {
                pageIndex = GetElements().Count - 1;
            }
        }

        private Element GetElement() {
            Element e = GetElements()[pageIndex];
            if (e.Thumbnail == null) {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            } else {
                //GrayScaleEffect(e.Thumbnail);
            }
            return e;
        }

        private Element GetPrevElement() {
            int prevIndex = pageIndex > 0 ? pageIndex - 1 : GetElements().Count - 1;
            Element e = GetElements()[prevIndex];
            if (e.Thumbnail == null) {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            } else {
                //GrayScaleEffect(e.Thumbnail);
            }
            return e;
        }

        private Element GetNextElement() {
            int nextIndex = pageIndex < GetElements().Count - 1 ? pageIndex + 1 : 0;
            Element e = GetElements()[nextIndex];
            if (e.Thumbnail == null) {
                e.Thumbnail = ImageUtil.GetResourceImage("sheet");
            } else {
                //GrayScaleEffect(e.Thumbnail);
            }
            return e;
        }

        private static Image GrayScaleEffect(Image image) {
            GrayscaleEffect.GrayscaleEffect effect = new GrayscaleEffect.GrayscaleEffect();
            image.Effect = effect;
            return image;
        }
    }
}
