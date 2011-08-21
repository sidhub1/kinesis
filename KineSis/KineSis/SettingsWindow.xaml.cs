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

namespace KineSis {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();
            for (int i = 0; i < WindowUtils.Screens.Length; i++) {
                String device = "Display " + i + " ";
                device += WindowUtils.Screens[i].Primary ? "[Primary] " : "";
                device += "(" + WindowUtils.Screens[i].Bounds.Width + "x" + WindowUtils.Screens[i].Bounds.Height + ") " + WindowUtils.Screens[i].BitsPerPixel + " bpp";
                comboBox1.Items.Add(device);
                comboBox2.Items.Add(device);
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            this.Hide();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}
