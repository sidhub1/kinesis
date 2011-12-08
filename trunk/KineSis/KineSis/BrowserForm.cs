/*
   Copyright 2011 Alexandru Albu - sandu.albu@gmail.com

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using System.IO;
using System.Threading;

namespace KineSis
{
    public partial class BrowserForm : Form
    {
        private double zoom = 100;
        public BrowserForm()
        {
            InitializeComponent();
            String startupPage = Directory.GetCurrentDirectory() + "\\Startup\\startup.html";
            webBrowser1.Navigate(startupPage);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        public void ScrollDown(int amount)
        {
            webBrowser1.Document.Body.ScrollTop = webBrowser1.Document.Body.ScrollTop + amount;
        }

        public void ScrollUp(int amount)
        {
            webBrowser1.Document.Body.ScrollTop = webBrowser1.Document.Body.ScrollTop - amount;
        }

        public void ScrollRight(int amount)
        {
            webBrowser1.Document.Body.ScrollLeft = webBrowser1.Document.Body.ScrollLeft + amount;
        }

        public void ScrollLeft(int amount)
        {
            webBrowser1.Document.Body.ScrollLeft = webBrowser1.Document.Body.ScrollLeft - amount;
        }

        public void SetZoom(double zoom)
        {
            this.zoom = zoom;
            try
            {
                webBrowser1.Document.Body.Style = "zoom: " + this.zoom + "%";
            }
            catch (Exception)
            {

            }
        }

        public void GoTo(Object obj)
        {
            String path = obj as String;
            webBrowser1.Navigate(new Uri("file:///" + path));
        }

        public void open(String path)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GoTo));
            thread.Start(path);
        }

        public void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.Document.Body.Style = "zoom: " + this.zoom + "%";

                this.Update();
                this.Refresh();
            }
            catch (Exception)
            {

            }
        }

    }
}
