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
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];
                nameLabel.Content = d.Name;

                int year = int.Parse(d.Location.Substring(0, 4));
                int month = int.Parse(d.Location.Substring(4, 2));
                int day = int.Parse(d.Location.Substring(6, 2));
                int hour = int.Parse(d.Location.Substring(8, 2));
                int minute = int.Parse(d.Location.Substring(10, 2));
                int second = int.Parse(d.Location.Substring(12, 2));
                DateTime dt = new DateTime(year, month, day, hour, minute, second);
                creationDateLabel.Content = dt.ToString();
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e) {
            if (documentsListBox.SelectedIndex >= 0) {
                Doc d = ProfileManager.ActiveProfile.Documents[documentsListBox.SelectedIndex];
                Document doc = Document.deserialize(ProfileManager.ActiveProfile.TempFolder + "\\" + d.Location + ".kinesis");
                mw.OpenDocument(doc);
                this.Close();
            }
        }
    }
}
