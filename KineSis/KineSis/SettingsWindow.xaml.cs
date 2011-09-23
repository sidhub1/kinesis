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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KineSis.Utils;
using System.Windows.Forms;
using KineSis.Profiles;
using System.ComponentModel;

namespace KineSis {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {

        private static System.Windows.Forms.ColorDialog colorDialog = new ColorDialog();
        public MainWindow mw;

        public SettingsWindow() {
            InitializeComponent();
            for (int i = 0; i < WindowUtils.Screens.Length; i++) {
                String device = "Display " + i + " ";
                device += WindowUtils.Screens[i].Primary ? "[Primary] " : "";
                device += "(" + WindowUtils.Screens[i].Bounds.Width + "x" + WindowUtils.Screens[i].Bounds.Height + ") " + WindowUtils.Screens[i].BitsPerPixel + " bpp";
                screensCB.Items.Add(device);
            }

            List<Profile> profiles = ProfileManager.Profiles;
            

            Profile profile = ProfileManager.ActiveProfile;
            PopulateForm(profile);
            int index = 0;
            for (int i = 0; i < profiles.Count; i++) {
                String prof = profiles[i].Name;
                profilesCB.Items.Add(prof);
                if (prof.Equals(profile.Name)) {
                    index = i;
                }
            }
            profilesCB.SelectedIndex = index;
        }

        private void Window_Closed(object sender, EventArgs e) {
            this.Hide();
        }

        protected override void OnClosing(CancelEventArgs e) {
            this.Hide();
            base.OnClosing(e);
            e.Cancel = true;
        }

        private void PopulateForm(Profile profile) {
            //profilesCB.SelectedIndex = 
            screensCB.SelectedIndex = profile.PresentationScreen;
            tempFolderTextBox.Text = profile.TempFolder;
            deleteCheckBox.IsChecked = profile.DeleteTempAfterPresentation;
            primaryC.Background = profile.PrimaryColor;
            primaryB.Content = profile.PrimaryColor.ToString();
            secondaryC.Background = profile.SecondaryColor;
            secondaryB.Content = profile.SecondaryColor.ToString();
            backgroundC.Background = profile.BackgroundColor;
            backgroundB.Content = profile.BackgroundColor.ToString();
            skeletonC.Background = profile.SkeletonColor;
            skeletonB.Content = profile.SkeletonColor.ToString();
            slideWidthTB.Text = profile.SlideWidth.ToString();
            chartWidthTB.Text = profile.ChartWidth.ToString();
            chfTB.Text = profile.ChartHorizontalFaces.ToString();
            cvfTB.Text = profile.ChartVerticalFaces.ToString();

            switch (profile.SlideWidth) {
                case 640 : 
                    slideWidthSlider.Value = 0; 
                    break;
                case 800 : 
                    slideWidthSlider.Value = 1; 
                    break;
                case 1280:
                    slideWidthSlider.Value = 2; 
                    break;
                case 1920:
                    slideWidthSlider.Value = 3; 
                    break;
            }

            switch (profile.ChartWidth) {
                case 640:
                    chartWidthSlider.Value = 0;
                    break;
                case 800:
                    chartWidthSlider.Value = 1;
                    break;
                case 1280:
                    chartWidthSlider.Value = 2;
                    break;
            }

            switch (profile.ChartHorizontalFaces) {
                case 0:
                    chfSlider.Value = 0;
                    break;
                case 2:
                    chfSlider.Value = 1;
                    break;
                case 4:
                    chfSlider.Value = 2;
                    break;
                case 8:
                    chfSlider.Value = 3;
                    break;
                case 16:
                    chfSlider.Value = 4;
                    break;
            }

            cvfSlider.Value = profile.ChartVerticalFaces;
        }

        private void screensCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog()) {
                dlg.Description = "Choose the TEMP folder";
                String text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();
                dlg.SelectedPath = text;
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    text = dlg.SelectedPath;
                    tempFolderTextBox.Text = text;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {

            colorDialog.Color = System.Drawing.Color.Black;
            colorDialog.FullOpen = false;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            Profile profile = new Profile();
            profile.PresentationScreen = screensCB.SelectedIndex;
            profile.PrimaryColor = primaryC.Background;
            profile.SecondaryColor = secondaryC.Background;
            profile.BackgroundColor = backgroundC.Background;
            profile.SkeletonColor = skeletonC.Background;
            profile.TempFolder = tempFolderTextBox.Text;
            profile.DeleteTempAfterPresentation = deleteCheckBox.IsChecked.Value;
            profile.SlideWidth = int.Parse(slideWidthTB.Text);
            profile.ChartWidth = int.Parse(chartWidthTB.Text);
            profile.ChartHorizontalFaces = int.Parse(chfTB.Text);
            profile.ChartVerticalFaces = int.Parse(cvfTB.Text);
            Boolean exception = false;

            if (saveAsTextBox.Text == null || saveAsTextBox.Text.Length == 0) {
                Profile profile1 = ProfileManager.GetProfile(profilesCB.SelectedItem.ToString());
                if (profile1 != null) {
                    profile.Name = profile1.Name;
                    try {
                        ProfileManager.SaveProfile(profile);
                    } catch (Exception ex) {
                        exception = true;
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
            } else {
                profile.Name = saveAsTextBox.Text;
                try {
                    ProfileManager.AddProfile(profile);
                } catch (Exception ex) {
                    exception = true;
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }

            if (saveAsTextBox.Text != null && saveAsTextBox.Text.Length > 0 && !exception) {
                profilesCB.Items.Clear();

                List<Profile> profiles = ProfileManager.Profiles;
                int index = 0;
                for (int i = 0; i < profiles.Count; i++) {
                    String prof = profiles[i].Name;
                    profilesCB.Items.Add(prof);
                    if (prof.Equals(profile.Name)) {
                        index = i;
                    }
                }
                profilesCB.SelectedIndex = index;
            }

            saveAsTextBox.Text = "";
        }

        private void primaryB_Click(object sender, RoutedEventArgs e) {
            Color c = ( primaryC.Background as SolidColorBrush ).Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
            colorDialog.FullOpen = false;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                primaryC.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
            primaryB.Content = primaryC.Background.ToString();
        }

        private void secondaryB_Click(object sender, RoutedEventArgs e) {
            Color c = ( secondaryC.Background as SolidColorBrush ).Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
            colorDialog.FullOpen = false;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                secondaryC.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
            secondaryB.Content = secondaryC.Background.ToString();
        }

        private void backgroundB_Click(object sender, RoutedEventArgs e) {
            Color c = ( backgroundC.Background as SolidColorBrush ).Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
            colorDialog.FullOpen = false;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                backgroundC.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
            backgroundB.Content = backgroundC.Background.ToString();
        }

        private void skeletonB_Click(object sender, RoutedEventArgs e) {
            Color c = ( skeletonC.Background as SolidColorBrush ).Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
            colorDialog.FullOpen = false;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                skeletonC.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
            skeletonB.Content = skeletonC.Background.ToString();
        }

        private void profilesCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (profilesCB.SelectedItem != null) {
                Profile profile = ProfileManager.GetProfile(profilesCB.SelectedItem.ToString());
                //ProfileManager.ActiveProfile = profile;
                PopulateForm(profile);
            }
        }

        private void slideWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int val = 0;
            val = (int)slideWidthSlider.Value;

            if (val == 0) {
                val = 640;
            } else if (val == 1) {
                val = 800;
            } else if (val == 2) {
                val = 1280;
            } else if (val == 3) {
                val = 1920;
            }

            slideWidthTB.Text = val.ToString();
        }

        private void chartWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int val = 0;
            val = (int)chartWidthSlider.Value;

            if (val == 0) {
                val = 640;
            } else if (val == 1) {
                val = 800;
            } else if (val == 2) {
                val = 1280;
            }

            chartWidthTB.Text = val.ToString();
        }

        private void chfSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int val = 0;
            val = (int)chfSlider.Value;

            if (val == 0) {
                val = 0;
            } else if (val == 1) {
                val = 2;
            } else if (val == 2) {
                val = 4;
            } else if (val == 3) {
                val = 8;
            } else if (val == 4) {
                val = 16;
            }

            chfTB.Text = val.ToString();
        }

        private void cvfSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int val = 0;
            val = (int)cvfSlider.Value;
            cvfTB.Text = val.ToString();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e) {
            if (profilesCB.SelectedItem != null) {
                Profile profile = ProfileManager.GetProfile(profilesCB.SelectedItem.ToString());
                ProfileManager.ActiveProfile = profile;
                //PopulateForm(profile);
                ProfileManager.Serialize();
                mw.ApplyProfile();
                this.Hide();
            }
        }
    }
}
