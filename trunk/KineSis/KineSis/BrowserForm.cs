using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KineSis {
    public partial class BrowserForm : Form {
        private double zoom = 100;
        public BrowserForm() {
            InitializeComponent();
            webBrowser1.Navigate(new Uri("file:///C:/Users/sandu/Desktop/Book1.htm"));
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        public void ScrollDown(int amount) {
            webBrowser1.Document.Body.ScrollTop = webBrowser1.Document.Body.ScrollTop + amount;
        }

        public void ScrollUp(int amount) {
            webBrowser1.Document.Body.ScrollTop = webBrowser1.Document.Body.ScrollTop - amount;
        }

        public void ScrollRight(int amount) {
            webBrowser1.Document.Body.ScrollLeft = webBrowser1.Document.Body.ScrollLeft + amount;
        }

        public void ScrollLeft(int amount) {
            webBrowser1.Document.Body.ScrollLeft = webBrowser1.Document.Body.ScrollLeft - amount;
        }

        public void SetZoom(double zoom) {
            this.zoom = zoom;
            try {
                webBrowser1.Document.Body.Style = "zoom: " + this.zoom + "%";
            } catch (Exception ex) {

            }
        }

        public void open(String path) {
            webBrowser1.Navigate(new Uri("file:///" + path));
        }

        public void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            try {
                webBrowser1.Document.Body.Style = "zoom: " + this.zoom + "%";
            } catch (Exception ex) {

            }
        }

    }
}
