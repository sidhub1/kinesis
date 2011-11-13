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
using KineSis.Profiles;
using KineSis.ContentManagement.Model;
using KineSis.Utils;
using System.IO;

namespace KineSis {
    /// <summary>
    /// Interaction logic for ArchiveWindow.xaml
    /// </summary>
    public partial class ArchiveWindow : Window {

        private MainWindow mw;

        public ArchiveWindow(MainWindow mw) {
            InitializeComponent();
            this.mw = mw;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            foreach (Doc d in ProfileManager.ActiveProfile.Documents) {
                documentsListBox.Items.Add(d.Name);
            }
        }

        private void documentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (documentsListBox.SelectedIndex >= 0) {
                deleteButton.IsEnabled = true;
                openButton.IsEnabled = true;
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];
                nameLabel.Content = d.Name;

                String extension = d.Name.Substring(d.Name.LastIndexOf("."));
                String type = "Text Document";
                String image = "text";

                if (extension.Equals(".docx")) {
                    type = "Word Document";
                    image = "docx";
                } else if (extension.Equals(".xlsx")) {
                    type = "Excel Workbook";
                    image = "xlsx";
                } else if (extension.Equals(".pptx")) {
                    type = "PowerPoint Presentation";
                    image = "pptx";
                } else if (extension.Equals(".bmp") || extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png") || extension.Equals(".tif") || extension.Equals(".tiff")) {
                    type = "Image";
                    image = extension.Substring(1);
                }

                Image img = ImageUtil.GetResourceImage(image);

                image1.Source = img.Source;

                typeLabel.Content = type;

                int year = int.Parse(d.Location.Substring(0, 4));
                int month = int.Parse(d.Location.Substring(4, 2));
                int day = int.Parse(d.Location.Substring(6, 2));
                int hour = int.Parse(d.Location.Substring(8, 2));
                int minute = int.Parse(d.Location.Substring(10, 2));
                int second = int.Parse(d.Location.Substring(12, 2));
                DateTime dt = new DateTime(year, month, day, hour, minute, second);
                creationDateLabel.Content = dt.ToString();
            } else {
                deleteButton.IsEnabled = false;
                openButton.IsEnabled = false;
                nameLabel.Content = "";
                typeLabel.Content = "";
                creationDateLabel.Content = "";
                image1.Source = null;
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e) {
            if (documentsListBox.SelectedIndex >= 0) {
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];
                Document doc = Document.deserialize(ProfileManager.ActiveProfile.TempFolder + "\\" + d.Location + ".xml");
                mw.OpenDocument(doc);
                this.Close();
            }
        }

        private void documentsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (documentsListBox.SelectedIndex >= 0) {
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];
                Document doc = Document.deserialize(ProfileManager.ActiveProfile.TempFolder + "\\" + d.Location + ".xml");
                mw.OpenDocument(doc);
                this.Close();
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e) {
            if (documentsListBox.SelectedIndex >= 0) {
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];

                FileInfo file = new FileInfo(ProfileManager.ActiveProfile.TempFolder + "\\" + d.Location + ".xml");
                DirectoryInfo dir = new DirectoryInfo(ProfileManager.ActiveProfile.TempFolder + "\\" + d.Location);

                file.Delete();
                dir.Delete(true);

                ProfileManager.ActiveProfile.Documents.RemoveAt(documentsListBox.SelectedIndex);
                ProfileManager.Serialize();

                documentsListBox.Items.Clear();
                foreach (Doc d1 in ProfileManager.ActiveProfile.Documents) {
                    documentsListBox.Items.Add(d1.Name);
                }
                mw.CheckOpenedDocument();
            }
        }
    }
}
