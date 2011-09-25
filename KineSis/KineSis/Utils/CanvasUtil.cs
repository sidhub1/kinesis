using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineSis.ContentManagement.Progress;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using KineSis.Profiles;
using System.Windows.Media.Effects;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media.Imaging;


namespace KineSis.Utils {
    class CanvasUtil {

        /// <summary>
        /// Draw processing progress
        /// </summary>
        /// <param name="canvas">canvas where the progress will be drawn</param>
        /// <param name="pp">active processing progress</param>
        public static void DrawProgress(Canvas canvas, ProcessingProgress pp) {
            canvas.Background = ProfileManager.ActiveProfile.BackgroundColor;

            String currentOperationName = pp.CurrentOperationName;
            Double currentOperation = ( (Double)pp.CurrentOperationElement * 100 / (Double)pp.CurrentOperationTotalElements );
            String currentOperationProgress = String.Format("{0:00.00}", currentOperation) + " %";

            String overallOperationName = pp.OverallOperationName;
            Double overallOperation = ( (Double)pp.OverallOperationElement * 100 / (Double)pp.OverallOperationTotalElements );
            String overallOperationProgress = String.Format("{0:00.00}", overallOperation) + " %";

            canvas.Children.Clear();

            double sixth = canvas.Height / 6;
            double tenth = canvas.Width / 10;

            //current
            System.Windows.Shapes.Rectangle currentOperationOuterRect = new System.Windows.Shapes.Rectangle();
            currentOperationOuterRect.Width = canvas.Width - tenth;
            currentOperationOuterRect.Height = sixth;
            currentOperationOuterRect.Stroke = ProfileManager.ActiveProfile.SecondaryColor;
            currentOperationOuterRect.StrokeThickness = 2;
            currentOperationOuterRect.Fill = ProfileManager.ActiveProfile.BackgroundColor;
            currentOperationOuterRect.Margin = new Thickness(tenth / 2, sixth, 0, 0);


            System.Windows.Shapes.Rectangle currentOperationInnerRect = new System.Windows.Shapes.Rectangle();
            currentOperationInnerRect.Width = ( ( canvas.Width - tenth - 6 ) * currentOperation ) / 100;
            currentOperationInnerRect.Height = sixth - 6;
            currentOperationInnerRect.Stroke = ProfileManager.ActiveProfile.BackgroundColor;
            currentOperationInnerRect.StrokeThickness = 2;
            currentOperationInnerRect.Fill = ProfileManager.ActiveProfile.SecondaryColor;
            currentOperationInnerRect.Margin = new Thickness(( tenth + 6 ) / 2, sixth + 3, 0, 0);

            TextBlock currentOperationTextBlock = new TextBlock();
            currentOperationTextBlock.Text = currentOperationName + "  [ " + currentOperationProgress + " ]";
            currentOperationTextBlock.Foreground = ProfileManager.ActiveProfile.SecondaryColor;
            currentOperationTextBlock.FontSize = sixth / 1.5;
            currentOperationTextBlock.Width = canvas.Width;
            currentOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("SF Fedora Titles");

            currentOperationTextBlock.TextAlignment = TextAlignment.Center;

            //overall
            System.Windows.Shapes.Rectangle overallOperationOuterRect = new System.Windows.Shapes.Rectangle();
            overallOperationOuterRect.Width = canvas.Width - tenth;
            overallOperationOuterRect.Height = sixth;
            overallOperationOuterRect.Stroke = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationOuterRect.StrokeThickness = 2;
            overallOperationOuterRect.Fill = ProfileManager.ActiveProfile.BackgroundColor;
            overallOperationOuterRect.Margin = new Thickness(tenth / 2, 4 * sixth, 0, 0);


            System.Windows.Shapes.Rectangle overallOperationInnerRect = new System.Windows.Shapes.Rectangle();
            overallOperationInnerRect.Width = ( ( canvas.Width - tenth - 6 ) * overallOperation ) / 100;
            overallOperationInnerRect.Height = sixth - 6;
            overallOperationInnerRect.Stroke = ProfileManager.ActiveProfile.BackgroundColor;
            overallOperationInnerRect.StrokeThickness = 2;
            overallOperationInnerRect.Fill = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationInnerRect.Margin = new Thickness(( tenth + 6 ) / 2, 4 * sixth + 3, 0, 0);

            TextBlock overallOperationTextBlock = new TextBlock();
            overallOperationTextBlock.Text = overallOperationName + "  [ " + overallOperationProgress + " ]";
            overallOperationTextBlock.Foreground = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationTextBlock.FontSize = sixth / 1.5;
            overallOperationTextBlock.Width = canvas.Width;
            overallOperationTextBlock.Margin = new Thickness(0, 3 * sixth, 0, 0);
            overallOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("SF Fedora Titles");

            overallOperationTextBlock.TextAlignment = TextAlignment.Center;


            canvas.Children.Add(currentOperationTextBlock);
            canvas.Children.Add(currentOperationOuterRect);
            canvas.Children.Add(currentOperationInnerRect);

            canvas.Children.Add(overallOperationTextBlock);
            canvas.Children.Add(overallOperationOuterRect);
            canvas.Children.Add(overallOperationInnerRect);
            canvas.UpdateLayout();

            canvas.Refresh();
        }

        public static void DrawException(Canvas canvas, String exception) {
            canvas.Background = ProfileManager.ActiveProfile.BackgroundColor;

            double sixth = canvas.Height / 6;
            double tenth = canvas.Width / 10;

            canvas.Children.Clear();

            TextBlock overallOperationTextBlock = new TextBlock();
            overallOperationTextBlock.Text = exception;
            overallOperationTextBlock.Foreground = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationTextBlock.FontSize = sixth / 1.5;
            overallOperationTextBlock.Width = canvas.Width;
            overallOperationTextBlock.Margin = new Thickness(0, 3 * sixth, 0, 0);
            overallOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("SF Fedora Titles");

            overallOperationTextBlock.TextAlignment = TextAlignment.Center;

            canvas.Children.Add(overallOperationTextBlock);
            canvas.UpdateLayout();

            canvas.Refresh();
        }

        /// <summary>
        /// Draw an ellipse or circle
        /// </summary>
        /// <param name="c">canvas where the ellipse will be drawn</param>
        /// <param name="X">X coordinate of the center</param>
        /// <param name="Y">Y coordinate of the center</param>
        /// <param name="width">width of the ellipse</param>
        /// <param name="height">height of the ellipse</param>
        /// <param name="stroke">stroke color</param>
        /// <param name="fill">fill color</param>
        /// <param name="background">background will apply a blur bitmap effect</param>
        public static void DrawEllipse(Canvas c, double X, double Y, double width, double height, System.Windows.Media.Brush stroke, System.Windows.Media.Brush fill, System.Windows.Media.Brush background) {

            /*
            if (background != null) {
                Ellipse ellipseB = new Ellipse();
                ellipseB.Width = width;
                ellipseB.Height = height;
                ellipseB.Stroke = background;
                BlurEffect effect = new BlurEffect();
                effect.Radius = 75;
                ellipseB.Effect = effect;
                ellipseB.StrokeThickness = 10;
                ellipseB.Fill = fill;
                Canvas.SetTop(ellipseB, Y - 0.5 * ellipseB.Width);
                Canvas.SetLeft(ellipseB, X - 0.5 * ellipseB.Height);
                c.Children.Add(ellipseB);
            }
            */
            Ellipse ellipse = new Ellipse();
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Stroke = stroke;
            ellipse.StrokeThickness = 10;

            if (background != null) {
                ellipse.Fill = background;
            } else {
                ellipse.Fill = fill;
            }


            
            Canvas.SetTop(ellipse, Y - 0.5 * ellipse.Width);
            Canvas.SetLeft(ellipse, X - 0.5 * ellipse.Height);
            c.Children.Add(ellipse);
        }

        /// <summary>
        /// Draw a text block
        /// </summary>
        /// <param name="c">canvas where the text will be drawn</param>
        /// <param name="text">text string</param>
        /// <param name="fontSize">font size</param>
        /// <param name="background">if not null, the background will be used for drawing a blurry background effect</param>
        /// <param name="foreground">text color</param>
        /// <param name="X">X coordinate of the center of the text. depending on text length, the horizontal aligment will be made</param>
        /// <param name="top">distance from top of the canvas to text block</param>
        public static void DrawTextBlock(Canvas c, String text, double fontSize, System.Windows.Media.Brush background, System.Windows.Media.Brush foreground,  double X, double top) {

            System.Drawing.Size size = TextRenderer.MeasureText(text, new Font("SF Fedora Titles", (float)fontSize));
            if (background != null) {
                TextBlock submenuEffect = new TextBlock();
                submenuEffect.FontFamily = new System.Windows.Media.FontFamily("SF Fedora Titles");
                submenuEffect.Background = background;
                submenuEffect.Text = text;
                submenuEffect.Foreground = foreground;
                submenuEffect.FontSize = (float)fontSize;

                BlurEffect effect = new BlurEffect();
                effect.Radius = 75;
                submenuEffect.Effect = effect;
                Canvas.SetTop(submenuEffect, top);
                Canvas.SetLeft(submenuEffect, X - 0.3 * size.Width);
                c.Children.Add(submenuEffect);
            }

            TextBlock submenu = new TextBlock();
            submenu.FontFamily = new System.Windows.Media.FontFamily("SF Fedora Titles");
            submenu.Text = text;
            submenu.Foreground = foreground;
            submenu.FontSize = (float) fontSize;
            Canvas.SetTop(submenu, top);
            Canvas.SetLeft(submenu, X - 0.3 * size.Width);
            c.Children.Add(submenu);
        }

        /// <summary>
        /// Draw an image in a circle, excluding circle
        /// </summary>
        /// <param name="c">canvas where the image will be drawn</param>
        /// <param name="image">image to be drawn</param>
        /// <param name="circleDiameter">diameter of the circle where the image will be centerd</param>
        /// <param name="X">X coordinate of the circle</param>
        /// <param name="Y">Y coordinate of the circle</param>
        public static void DrawImageInCircle(Canvas c, System.Windows.Controls.Image image, double circleDiameter, double X, double Y) {
            image.Opacity = 0.9;
            image.Width = circleDiameter / 2 * Math.Sqrt(2);
            image.Height = circleDiameter / 2 * Math.Sqrt(2);
            image.Stretch = Stretch.Uniform;
            c.Children.Add(image);
            Canvas.SetTop(image, Y - image.Width / 2);
            Canvas.SetLeft(image, X - image.Height / 2);
        }

        /// <summary>
        /// Draw an image in a rectangle, including rectangle
        /// </summary>
        /// <param name="c">canvas where the image will be drawn</param>
        /// <param name="thumb">image</param>
        /// <param name="rectangleColor">color of the rectange</param>
        /// <param name="X">center X</param>
        /// <param name="Y">center Y</param>
        /// <param name="maxWidthOrHight">maximum width or height permitted</param>
        public static void DrawImageInRectangle(Canvas c, System.Windows.Controls.Image thumb, System.Windows.Media.Brush rectangleColor, double X, double Y, double maxWidthOrHight, double opacity) {
            double W, H;

            if (thumb.ActualHeight > maxWidthOrHight) {
                W = ( thumb.ActualWidth * maxWidthOrHight ) / thumb.ActualHeight;
                H = maxWidthOrHight;
            } else {
                H = ( thumb.ActualHeight * maxWidthOrHight ) / thumb.ActualWidth;
                W = maxWidthOrHight;
            }

            thumb.Height = H - 20;
            thumb.Width = W;

            thumb.Opacity = opacity;
            Canvas.SetTop(thumb, Y - ( H - 20 ) / 2);
            Canvas.SetLeft(thumb, X - W / 2);

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Stroke = rectangleColor;
            r.StrokeThickness = 10;
            r.Opacity = opacity;
            r.Width = W - 20;
            r.Height = H;
            r.RadiusX = 20;
            r.RadiusY = 20;
            Canvas.SetTop(r, Y - H / 2);
            Canvas.SetLeft(r, X - ( W - 20 ) / 2);

            System.Windows.Shapes.Rectangle r1 = new System.Windows.Shapes.Rectangle();
            r1.Stroke = System.Windows.Media.Brushes.White;
            r1.StrokeThickness = 10;
            r1.Fill = System.Windows.Media.Brushes.White;
            r1.Opacity = opacity;
            r1.Width = W - 20;
            r1.Height = H;
            r1.RadiusX = 20;
            r1.RadiusY = 20;
            Canvas.SetTop(r1, Y - H / 2);
            Canvas.SetLeft(r1, X - ( W - 20 ) / 2);

            c.Children.Add(r1);
            c.Children.Add(thumb);
            c.Children.Add(r);
            
        }

        /// <summary>
        /// Draw a submenu on a canvas
        /// </summary>
        /// <param name="c">desired canvas</param>
        /// <param name="submenuName">name of the submenu</param>
        /// <param name="submenuName">caption for submenu. if not null, will be used instead of submenu name. does not affect the resource name for submenu</param>
        /// <param name="useNameForImage">true if a [submenuName].png image exists in "Drawable" directory</param>
        /// <param name="submenuImage">image used in case a resource for this submenu does not exists (thumbnails case)</param>
        /// <param name="centerX">X of the submenu center</param>
        /// <param name="centerY">Y of the submenu center</param>
        /// <param name="centerDiameter">diameter of the center</param>
        /// <param name="selectionDiameter">diameter of the areas and selections</param>
        /// <param name="leftAreaX">X of the left area (which contains the left hand)</param>
        /// <param name="leftAreaY">Y of the left area (which contains the left hand)</param>
        /// <param name="rightAreaX">X of the right area (which contains the right hand)</param>
        /// <param name="rightAreaY">Y of the right area (which contains the right hand)</param>
        /// <param name="leftSelectionX">X of the left selection (the circle where selection took place in left side)</param>
        /// <param name="leftSelectionY">Y of the left selection (the circle where selection took place in left side)</param>
        /// <param name="rightSelectionX">X of the right selection (the circle where selection took place in right side)</param>
        /// <param name="rightSelectionY">Y of the right selection (the circle where selection took place in right side)</param>
        /// <param name="preselected">boolean indicating if current submenu is preselected</param>
        public static void DrawSubmenu(Canvas c, String submenuName, String submenuCaption, Boolean useNameForImage, System.Windows.Controls.Image submenuImage, double centerX, double centerY, double centerDiameter, double selectionDiameter, double leftAreaX, double leftAreaY, double rightAreaX, double rightAreaY, double leftSelectionX, double leftSelectionY, double rightSelectionX, double rightSelectionY, Boolean preselected) {

            System.Windows.Media.Brush background = null;
            System.Windows.Media.Brush primaryColor = ProfileManager.ActiveProfile.PrimaryColor;
            System.Windows.Media.Brush secondaryColor = ProfileManager.ActiveProfile.SecondaryColor;

            if (preselected) {
                background = System.Windows.Media.Brushes.White;
            }

            DrawEllipse(c, centerX, centerY, centerDiameter, centerDiameter, primaryColor, ColorUtil.FromHTML("#CCFFFFFF"), background);


            System.Windows.Controls.Image image = null;
            if (useNameForImage) {
                image = ImageUtil.GetResourceImage(submenuName);
            } else if (submenuImage != null) {
                image = submenuImage;
            } else {
                image = new System.Windows.Controls.Image();
            }

            DrawImageInCircle(c, image, centerDiameter, centerX, centerY);

            String caption = submenuCaption != null ? submenuCaption : submenuName;

            DrawTextBlock(c, caption, 0.2 * centerDiameter, System.Windows.Media.Brushes.White, primaryColor, centerX, centerY + 0.75 * image.Width);

            System.Windows.Media.Brush fill = ColorUtil.FromHTML("#88FFFFFF");

            DrawEllipse(c, rightAreaX, rightAreaY, selectionDiameter, selectionDiameter, secondaryColor, fill, background);
            DrawEllipse(c, rightSelectionX, rightSelectionY, selectionDiameter, selectionDiameter, primaryColor, fill, background);
            DrawEllipse(c, leftAreaX, leftAreaY, selectionDiameter, selectionDiameter, secondaryColor, fill, background);
            DrawEllipse(c, leftSelectionX, leftSelectionY, selectionDiameter, selectionDiameter, primaryColor, fill, background);

            System.Windows.Controls.Image image1 = ImageUtil.GetResourceImage("updown");

            DrawImageInCircle(c, image1, selectionDiameter, leftAreaX, leftAreaY);

            System.Windows.Controls.Image image2 = ImageUtil.GetResourceImage("updown");

            DrawImageInCircle(c, image2, selectionDiameter, rightAreaX, rightAreaY);
        }

        public static void DrawImage(CanvasWindow c, CanvasWindow user, String b) {
            BitmapImage newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.UriSource = new Uri(b);
            newImage.EndInit();
            c.Background = System.Windows.Media.Brushes.White;
            user.Background = System.Windows.Media.Brushes.White;

            double W, H;

            if (newImage.Width <= c.canvas.Width && newImage.Height <= c.canvas.Height) {
                W = newImage.Width;
                H = newImage.Height;
            } else if (newImage.Width > c.canvas.Width && newImage.Height <= c.canvas.Height) {
                H = ( newImage.Height * c.canvas.Width ) / newImage.Width;
                W = c.canvas.Width;
            } else if (newImage.Width <= c.canvas.Width && newImage.Height > c.canvas.Height) {
                W = ( newImage.Width * c.canvas.Height ) / newImage.Height;
                H = c.canvas.Height;
            } else {
                double widthR = (double)c.canvas.Width / (double)newImage.Width;
                double heightR = (double)c.canvas.Height / (double)newImage.Height;

                if (widthR < heightR) {
                    H = ( newImage.Height * c.canvas.Width ) / newImage.Width;
                    W = c.canvas.Width;
                } else {
                    W = ( newImage.Width * c.canvas.Height ) / newImage.Height;
                    H = c.canvas.Height;
                }
            }

            c.image.Source = newImage;
            //c.image.StretchDirection = StretchDirection.DownOnly;
            c.image.Width = W;
            c.image.Height = H;

            double ratio = user.canvas.Height / c.canvas.Height;

            user.image.Source = newImage;
            //user.image.StretchDirection = StretchDirection.DownOnly;
            user.image.Width = W * ratio;
            user.image.Height = H * ratio;
        }
    }


}
