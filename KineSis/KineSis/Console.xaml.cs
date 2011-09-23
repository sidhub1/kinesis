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
using System.ComponentModel;

namespace KineSis {
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : Window {
        public Console() {
            InitializeComponent();

            richTextBox1.SetValue(Paragraph.LineHeightProperty, 1.0);
        }

        private void Window_Closed(object sender, EventArgs e) {
            this.Hide();
        }

        protected override void OnClosing(CancelEventArgs e) {
            this.Hide();
            base.OnClosing(e);
            e.Cancel = true;
        }

        public void log(String sender, String message) {
            DateTime dt = DateTime.Now;
            String text = dt.ToString() + " [ " + sender + " ] : " + message;
            richTextBox1.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public static void WriteLine(String msg) {
            //richTextBox1.Document.Blocks.Add(new Paragraph(new Run(msg)));
        }
    }

    
}
