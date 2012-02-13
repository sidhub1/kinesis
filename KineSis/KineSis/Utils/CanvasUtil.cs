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
using KineSis.ContentManagement.Progress;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using KineSis.Profiles;
using System.Windows.Media.Effects;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media.Imaging;
using KineSis.UserInterface;


namespace KineSis.Utils
{
    class CanvasUtil
    {

        /// <summary>
        /// Draw processing progress
        /// </summary>
        /// <param name="canvas">canvas where the progress will be drawn</param>
        /// <param name="pp">active processing progress</param>
        public static void DrawProgress(Canvas canvas, ProcessingProgress pp)
        {
            canvas.Background = ProfileManager.MinimalView ? System.Windows.Media.Brushes.Transparent : ProfileManager.ActiveProfile.BackgroundColor;

            String currentOperationName = pp.CurrentOperationName;
            Double currentOperation = ((Double)pp.CurrentOperationElement * 100 / (Double)pp.CurrentOperationTotalElements);
            String currentOperationProgress = String.Format("{0:00.00}", currentOperation) + " %";

            String overallOperationName = pp.OverallOperationName;
            Double overallOperation = ((Double)pp.OverallOperationElement * 100 / (Double)pp.OverallOperationTotalElements);
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
            currentOperationInnerRect.Width = ((canvas.Width - tenth - 6) * currentOperation) / 100;
            currentOperationInnerRect.Height = sixth - 6;
            currentOperationInnerRect.Stroke = ProfileManager.ActiveProfile.BackgroundColor;
            currentOperationInnerRect.StrokeThickness = 2;
            currentOperationInnerRect.Fill = ProfileManager.ActiveProfile.SecondaryColor;
            currentOperationInnerRect.Margin = new Thickness((tenth + 6) / 2, sixth + 3, 0, 0);

            TextBlock currentOperationTextBlock = new TextBlock();
            currentOperationTextBlock.Text = currentOperationName + "  [ " + currentOperationProgress + " ]";
            currentOperationTextBlock.Foreground = ProfileManager.ActiveProfile.SecondaryColor;
            currentOperationTextBlock.FontSize = sixth / 1.5;
            currentOperationTextBlock.Width = canvas.Width;
            currentOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("Century Gothic");

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
            overallOperationInnerRect.Width = ProfileManager.MinimalView ? ((canvas.Width - tenth) * overallOperation) / 100 : ((canvas.Width - tenth - 6) * overallOperation) / 100;
            overallOperationInnerRect.Height = ProfileManager.MinimalView ? sixth - 2 : sixth - 6;
            overallOperationInnerRect.Stroke = ProfileManager.ActiveProfile.BackgroundColor;
            overallOperationInnerRect.StrokeThickness = ProfileManager.MinimalView ? 0 : 2;
            overallOperationInnerRect.Fill = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationInnerRect.Margin = ProfileManager.MinimalView ? new Thickness(tenth / 2 + 1, 4 * sixth + 1, 0, 0) : new Thickness((tenth + 6) / 2, 4 * sixth + 3, 0, 0);

            TextBlock overallOperationTextBlock = new TextBlock();
            overallOperationTextBlock.Text = overallOperationName + "  [ " + overallOperationProgress + " ]";
            overallOperationTextBlock.Foreground = ProfileManager.ActiveProfile.PrimaryColor;
            overallOperationTextBlock.FontSize = sixth / 1.5;
            overallOperationTextBlock.Width = canvas.Width;
            overallOperationTextBlock.Margin = new Thickness(0, 3 * sixth, 0, 0);
            overallOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("Century Gothic");

            overallOperationTextBlock.TextAlignment = TextAlignment.Center;


            if (!ProfileManager.MinimalView) canvas.Children.Add(currentOperationTextBlock);
            if (!ProfileManager.MinimalView) canvas.Children.Add(currentOperationOuterRect);
            if (!ProfileManager.MinimalView) canvas.Children.Add(currentOperationInnerRect);

            if (!ProfileManager.MinimalView) canvas.Children.Add(overallOperationTextBlock);
            canvas.Children.Add(overallOperationOuterRect);
            canvas.Children.Add(overallOperationInnerRect);
            canvas.UpdateLayout();

            canvas.Refresh();
        }

        /// <summary>
        /// draw an exception
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="exception"></param>
        public static void DrawException(Canvas canvas, String exception)
        {
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
            overallOperationTextBlock.FontFamily = new System.Windows.Media.FontFamily("Century Gothic");

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
        public static void DrawEllipse(Canvas c, double X, double Y, double width, double height, System.Windows.Media.Brush stroke, System.Windows.Media.Brush fill, System.Windows.Media.Brush background, int thickness = 10)
        {
            if (background != null)
            {
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

            Ellipse ellipse = new Ellipse();
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Stroke = stroke;
            ellipse.StrokeThickness = thickness;

            if (background != null)
            {
                ellipse.Fill = background;
            }
            else
            {
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
        public static void DrawTextBlock(Canvas c, String text, double fontSize, System.Windows.Media.Brush background, System.Windows.Media.Brush foreground, double X, double top)
        {

            System.Drawing.Size size = TextRenderer.MeasureText(text, new Font("Century Gothic", (float)fontSize));
            if (background != null)
            {
                TextBlock submenuEffect = new TextBlock();
                submenuEffect.FontFamily = new System.Windows.Media.FontFamily("Century Gothic");
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
            submenu.FontFamily = new System.Windows.Media.FontFamily("Century Gothic");
            submenu.Text = text;
            submenu.Foreground = foreground;
            submenu.FontSize = (float)fontSize;
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
        public static void DrawImageInCircle(Canvas c, System.Windows.Controls.Image image, double circleDiameter, double X, double Y)
        {
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
        public static void DrawImageInRectangle(Canvas c, System.Windows.Controls.Image thumb, System.Windows.Media.Brush rectangleColor, double X, double Y, double maxWidthOrHight, double opacity)
        {
            double W, H;

            if (thumb.ActualHeight > maxWidthOrHight)
            {
                W = (thumb.ActualWidth * maxWidthOrHight) / thumb.ActualHeight;
                H = maxWidthOrHight;
            }
            else
            {
                H = (thumb.ActualHeight * maxWidthOrHight) / thumb.ActualWidth;
                W = maxWidthOrHight;
            }

            thumb.Height = H - 20;
            thumb.Width = W;

            thumb.Opacity = opacity;
            Canvas.SetTop(thumb, Y - (H - 20) / 2);
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
            Canvas.SetLeft(r, X - (W - 20) / 2);

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
            Canvas.SetLeft(r1, X - (W - 20) / 2);

            c.Children.Add(r1);
            c.Children.Add(thumb);
            c.Children.Add(r);

        }

        /// <summary>
        /// Draw an image
        /// </summary>
        /// <param name="c"></param>
        /// <param name="user"></param>
        /// <param name="b"></param>
        public static void DrawImage(CanvasWindow c, CanvasWindow user, String b)
        {
            BitmapImage newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.UriSource = new Uri(b);
            newImage.EndInit();
            c.Background = System.Windows.Media.Brushes.White;
            user.Background = System.Windows.Media.Brushes.White;

            double W, H;

            if (newImage.Width <= c.canvas.Width && newImage.Height <= c.canvas.Height)
            {
                W = newImage.Width;
                H = newImage.Height;
            }
            else if (newImage.Width > c.canvas.Width && newImage.Height <= c.canvas.Height)
            {
                H = (newImage.Height * c.canvas.Width) / newImage.Width;
                W = c.canvas.Width;
            }
            else if (newImage.Width <= c.canvas.Width && newImage.Height > c.canvas.Height)
            {
                W = (newImage.Width * c.canvas.Height) / newImage.Height;
                H = c.canvas.Height;
            }
            else
            {
                double widthR = (double)c.canvas.Width / (double)newImage.Width;
                double heightR = (double)c.canvas.Height / (double)newImage.Height;

                if (widthR < heightR)
                {
                    H = (newImage.Height * c.canvas.Width) / newImage.Width;
                    W = c.canvas.Width;
                }
                else
                {
                    W = (newImage.Width * c.canvas.Height) / newImage.Height;
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

        /// <summary>
        /// Draw the slected hand
        /// </summary>
        /// <param name="c"></param>
        public static void DrawHand(Canvas c)
        {
            if (UIManager.FirstHand != null && UIManager.FirstHand.IsSelected)
            {
                double x = UIManager.FirstHand.X;
                double y = UIManager.FirstHand.Y;

                DrawEllipse(c, x, y, 50, 50, System.Windows.Media.Brushes.Red, System.Windows.Media.Brushes.Pink, null);
            }
        }
    }
}
