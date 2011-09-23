using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using System.IO;
using System.Threading;

namespace KineSis {
    public partial class BrowserForm : Form {
        private double zoom = 100;
        public BrowserForm() {
            InitializeComponent();
            String startupPage = Directory.GetCurrentDirectory() + "\\Startup\\startup.html";
            webBrowser1.Navigate(startupPage);
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
            } catch (Exception) {

            }
        }

        public void GoTo(Object obj) {
            String path = obj as String;
            webBrowser1.Navigate(new Uri("file:///" + path));
        }

        public void open(String path) {
            Thread thread = new Thread(new ParameterizedThreadStart(GoTo));
            thread.Start(path);
            //thread.Join();
        }

        public void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            try {
                webBrowser1.Document.Body.Style = "zoom: " + this.zoom + "%";

                this.Update();
                this.Refresh();
            } catch (Exception) {

            }
        }

    }
}
