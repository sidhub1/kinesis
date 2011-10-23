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
        private Brush brush = null;

        private double lastX;
        private double lastY;
        private double initialX;
        private double initialY;
        private Boolean inSession = false;

        private Boolean leftSelected = false;
        private Boolean rightSelected = false;
        private Boolean upSelected = false;
        private Boolean downSelected = false;

        String Group.Name {
            get {
                return "paint";
            }
        }

        Boolean Group.IsActive
        {
            get
            {
                return true;
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

        public List<Group> Groups {
            get {
                //if (groups.Count == 0) {
                groups = new List<Group>();
                    Group main = UIManager.MainGroup;
                    groups.Add(main);
                    Group clear = Clear.Instance;
                    groups.Add(clear);
                    if (instance.brush == null || (instance.brush != null && !instance.brush.ToString().Equals("#FFFF0000")))
                    {
                        Group red = new PaintColor("red");
                        groups.Add(red);
                    }
                    if (instance.brush == null || (instance.brush != null && !instance.brush.ToString().Equals("#FF00FF00")))
                    {
                        Group green = new PaintColor("green");
                        groups.Add(green);
                    }
                    if (instance.brush == null || (instance.brush != null && !instance.brush.ToString().Equals("#FF0000FF")))
                    {
                        Group blue = new PaintColor("blue");
                        groups.Add(blue);
                    }
                //}
                return groups;
            }
        }

        void Group.Draw(Canvas c) {

            if ((UIManager.SecondHand != null || instance.brush == null || UIManager.inMenuSession) && UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {
                UIManager.InPaint = false;

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
                        brush = null;
                    }
                    else if (rightSelected && Groups[1].IsActive)
                    {
                        UIManager.Clear();
                    }
                    else if (upSelected && Groups[2].IsActive)
                    {
                        brush = ((PaintColor)Groups[2]).Brush;
                    }
                    else if (downSelected && Groups[3].IsActive)
                    {
                        brush = ((PaintColor)Groups[3]).Brush;
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

                System.Windows.Controls.Image image0 = ImageUtil.GetResourceImage("paint");
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
            else if (UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected && !UIManager.inMenuSession)
            {
                if (!inSession) {
                    initialX = UIManager.FirstHand.X;
                    initialY = UIManager.FirstHand.Y;
                    inSession = true;
                    lastX = initialX;
                    lastY = initialY;
                }

                if (brush != null) {
                    UIManager.InPaint = true;
                    UIManager.inMenuSession = false;

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
                UIManager.inMenuSession = false;
                leftSelected = false;
                rightSelected = false;
                upSelected = false;
                downSelected = false;
            }

            if (brush != null) {
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
    }
}
