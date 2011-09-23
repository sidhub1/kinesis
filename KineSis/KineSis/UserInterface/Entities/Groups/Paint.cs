using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineSis.Utils;
using System.Windows.Media;
using KineSis.Profiles;
using KineSis.Geometry;
using System.Windows.Shapes;

namespace KineSis.UserInterface.Entities.Groups {
    class Paint : Group {

        private static List<Group> groups = new List<Group>();
        private int pos = 0;
        private Boolean posChanged = false;
        private Boolean preSelected = false;
        private Brush brush = null;

        private double initialX;
        private double initialY;
        private double lastX;
        private double lastY;
        private Boolean inSession = false;

        String Group.Name {
            get {
                return "paint";
            }
        }

        static readonly Paint instance = new Paint();

        static Paint() {
        }

        Paint() {
        }

        public static Paint Instance {
            get {
                return instance;
            }
        }

        public static List<Group> Groups {
            get {
                if (groups.Count == 0) {
                    Group clear = Clear.Instance;
                    groups.Add(clear);
                    Group red = new PaintColor("red");
                    groups.Add(red);
                    Group green = new PaintColor("green");
                    groups.Add(green);
                    Group blue = new PaintColor("blue");
                    groups.Add(blue);
                    Group orange = new PaintColor("orange");
                    groups.Add(orange);
                    Group main = UIManager.MainGroup;
                    groups.Add(main);
                }
                return groups;
            }
        }

        void Group.Draw(Canvas c) {

            if (UIManager.SecondHand != null && UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {
                UIManager.InPaint = false;

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
                        if (Groups[pos].Name.Equals(UIManager.MainGroup.Name)) {
                            UIManager.SelectedGroup = Groups[pos];
                            brush = null;
                        } else if (Groups[pos].Name.Equals("clear")) {
                            UIManager.Clear();
                        } else {
                            brush = ( (PaintColor)Groups[pos] ).Brush;
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
                if (GetSubmenu().Contains("_")) {
                    caption = GetSubmenu().Substring(GetSubmenu().IndexOf("_") + 1);
                } else if (GetSubmenu().Equals(UIManager.MainGroup.Name)) {
                    caption = "back 2 " + GetSubmenu();
                }

                CanvasUtil.DrawSubmenu(c, GetSubmenu(), caption, true, null, centerX, centerY, centerDiameter, 150, leftAreaX, leftAreaY, rightAreaX, rightAreaY, leftSelectionX, leftSelectionY, rightSelectionX, rightSelectionY, preSelected);
            } else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {
                if (!inSession) {
                    initialX = UIManager.FirstHand.X;
                    initialY = UIManager.FirstHand.Y;
                    inSession = true;
                    lastX = initialX;
                    lastY = initialY;
                }

                if (brush != null) {
                    UIManager.InPaint = true;

                    CanvasUtil.DrawEllipse(c, UIManager.FirstHand.X, UIManager.FirstHand.Y, 40, 40, Brushes.Black, brush, brush);


                    System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
                    System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;
                    System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

                    double centerX = initialX;
                    double centerY = initialY;

                    Point2D last = new Point2D(lastX, lastY);
                    Point2D current = new Point2D(UIManager.FirstHand.X, UIManager.FirstHand.Y);

                    if (GeometryUtil.GetDistance2D(last, current) > 50) {
                        Line myLine = new Line();
                        myLine.Stroke = brush;
                        myLine.X1 = lastX;
                        myLine.X2 = UIManager.FirstHand.X;
                        myLine.Y1 = lastY;
                        myLine.Y2 = UIManager.FirstHand.Y;
                        myLine.StrokeThickness = 10;

                        UIManager.Draw(myLine);

                        lastX = UIManager.FirstHand.X;
                        lastY = UIManager.FirstHand.Y;
                    }
                }


            } else {
                inSession = false;
            }

            if (this.brush != null) {
                String resource = "paint_";
                if (brush.ToString().Equals("#FFFF0000")) {
                    resource += "red";
                } else if (brush.ToString().Equals("#FF0000FF")) {
                    resource += "blue";
                } else if (brush.ToString().Equals("#FF00FF00")) {
                    resource += "green";
                } else {
                    resource += "orange";
                }

                CanvasUtil.DrawEllipse(c, c.Width / 2, 75, 150, 150, ProfileManager.ActiveProfile.PrimaryColor, ColorUtil.FromHTML("#88FFFFFF"), Brushes.White);
                CanvasUtil.DrawImageInCircle(c, ImageUtil.GetResourceImage(resource), 150, c.Width / 2, 75);
            }
            

        }

        private void IncrementPos() {
            if (pos < Groups.Count - 1 && !posChanged) {
                pos++;
            } else if (pos == Groups.Count - 1 && !posChanged) {
                pos = 0;
            }
        }

        private void DecrementPos() {
            if (pos > 0 && !posChanged) {
                pos--;
            } else if (pos == 0 && !posChanged) {
                pos = Groups.Count - 1;
            }
        }

        private String GetSubmenu() {
            return Groups[pos].Name;
        }
    }
}
