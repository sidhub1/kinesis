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
using System.Linq;
using System.Text;
using KineSis.ContentManagement.Utils;
using Microsoft.Office.Interop.PowerPoint;
using KineSis.ContentManagement.Model;
using System.Windows.Forms;
using System.IO;
using KineSis.ContentManagement.Service.Helper;
using KineSis.ContentManagement.Progress;
using Ionic.Zip;
using System.ComponentModel;
using KineSis.Profiles;

namespace KineSis.ContentManagement.Service
{

    /// <summary>
    /// Document Service decides which helper to call in order to transform a given document
    /// </summary>
    class DocumentService
    {
        public static String TEMP_DIRECTORY = ProfileManager.ActiveProfile.TempFolder;
        public static int CHART_HORIZONTAL_FACES;
        public static int CHART_VERTICAL_FACES;
        public static int CHART_WIDTH;
        public static int SLIDE_WIDTH;
        public static int THUMB_WIDTH = 256;

        public static Boolean FORCE_CHART_SIZE = false;

        public static String IMAGE_EXTENSION = ImageUtil.PNG_EXTENSION;
        public static String IMAGE_FILTER = ImageUtil.PNG_FILTER;
        public static PpShapeFormat IMAGE_FORMAT = ImageUtil.PNG_FORMAT;

        private static TextHighlightConfiguration textHighlightConfiguration;

        /// <summary>
        /// Get text highlight configuration for active user
        /// </summary>
        public static TextHighlightConfiguration TextHighlightConfiguration
        {
            get
            {
                if (textHighlightConfiguration == null)
                {
                    UnZipSyntaxHighlighterToTemp();
                    CopyExtensionsFile();
                    textHighlightConfiguration = new TextHighlightConfiguration(TEMP_DIRECTORY + "\\Extensions.xml");
                }
                return textHighlightConfiguration;
            }
        }

        /// <summary>
        /// Create new Document
        /// </summary>
        /// <param name="path">Source document path</param>
        /// <param name="worker">Who do all the job</param>
        /// <returns>created KineSis document</returns>
        public static Document CreateNewDocument(String path, BackgroundWorker worker)
        {
            Profile profile = ProfileManager.ActiveProfile;

            //update static variables with active profile ones
            DocumentService.SLIDE_WIDTH = profile.SlideWidth;
            DocumentService.TEMP_DIRECTORY = profile.TempFolder;
            DocumentService.CHART_WIDTH = profile.ChartWidth;
            DocumentService.CHART_HORIZONTAL_FACES = profile.ChartHorizontalFaces;
            DocumentService.CHART_VERTICAL_FACES = profile.ChartVerticalFaces;

            //get a helper for parsing document
            FileInfo file = new FileInfo(path);
            DocumentHelper helper = GetHelperForDocument(path);

            if (helper == null)
            {
                return null;
            }

            //create a processing progress
            ProcessingProgress pp = new ProcessingProgress(worker);

            //start parsing
            Document document = null;
            try
            {
                document = helper.ParseNewDocument(path, pp);
            }
            catch (Exception ex)
            {
                pp.OverallOperationName = "[Exception] " + ex.Message;
                //    pp.OverallOperationTotalElements = 1;
                //    MessageBox.Show(ex.Message);
            }
            return document;
        }

        /// <summary>
        /// After document was parsed, now it's time for charts, processed concurrent with presentation
        /// </summary>
        /// <param name="path">Source document path</param>
        /// <param name="worker">Background Worker</param>
        /// <param name="document">Current KineSis Document</param>
        public static void CreateNewDocumentCharts(String path, BackgroundWorker worker, Document document)
        {
            FileInfo file = new FileInfo(path);

            DocumentHelper helper = GetHelperForDocument(path);

            if (helper != null)
            {
                ProcessingProgress pp = new ProcessingProgress(worker);
                try
                {
                    if (helper is PowerPointDocumentHelper)
                    {
                        PowerPointDocumentHelper h = (PowerPointDocumentHelper)helper;
                        h.ParseNewDocumentCharts(path, pp, document);
                    }
                    else if (helper is ExcelDocumentHelper)
                    {
                        ExcelDocumentHelper h = (ExcelDocumentHelper)helper;
                        h.ParseNewDocumentCharts(path, pp, document);
                    }
                    else if (helper is WordDocumentHelper)
                    {
                        WordDocumentHelper h = (WordDocumentHelper)helper;
                        h.ParseNewDocumentCharts(path, pp, document);
                    }
                }
                catch (Exception ex)
                {
                    pp.OverallOperationName = "[Exception] " + ex.Message;
                    //    pp.OverallOperationTotalElements = 1;
                    //    MessageBox.Show(ex.Message);
                }

                Document.serialize(document, TEMP_DIRECTORY + "\\" + document.Location + ".xml");

                ProfileManager.AddDocumentToActive(document.Name, document.Location);
                ProfileManager.Serialize();
            }
        }

        /// <summary>
        /// Need to be removed, not used anymore
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Document LoadExistingDocument(String path)
        {
            DocumentHelper helper = new ExcelDocumentHelper();

            Document document = null;

            return document;
        }

        /// <summary>
        /// Syntax Highlighter comes as Embedded resource copied to bin. It need to be unzipped to user's temp directory
        /// </summary>
        private static void UnZipSyntaxHighlighterToTemp()
        {
            String path = Directory.GetCurrentDirectory() + "\\syntaxhighlighter_3.0.83.zip";
            FileInfo fi = new FileInfo(path);

            string zipToUnpack = path;
            string unpackDirectory = TEMP_DIRECTORY;
            if (!Directory.Exists(unpackDirectory + "\\syntaxhighlighter_3.0.83"))
            {
                using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            else
            {
                Console.WriteLine(unpackDirectory + "\\syntaxhighlighter_3.0.83 already exists");
            }
        }

        /// <summary>
        /// Copy default Extensions.xml file to user's temp directory
        /// </summary>
        private static void CopyExtensionsFile()
        {
            String path = Directory.GetCurrentDirectory() + "\\Extensions.xml";
            string path2 = TEMP_DIRECTORY + "\\Extensions.xml";

            try
            {
                if (!File.Exists(path2))
                {
                    File.Copy(path, path2);
                }
                else
                {
                    Console.WriteLine(path2 + " already exists");
                }
            }
            catch
            {
                Console.WriteLine("Cannot copy the Extensions.xml file");
            }
        }

        /// <summary>
        /// Get helper for processing a document, based on document extension
        /// </summary>
        /// <param name="path">Source document full path</param>
        /// <returns></returns>
        private static DocumentHelper GetHelperForDocument(String path)
        {
            DocumentHelper helper = null;
            FileInfo fi = new FileInfo(path);
            String ext = fi.Extension.Substring(1).ToLower();

            if (ext.Equals("docx"))
            {
                helper = new WordDocumentHelper();
            }
            else if (ext.Equals("xlsx"))
            {
                helper = new ExcelDocumentHelper();
            }
            else if (ext.Equals("pptx"))
            {
                helper = new PowerPointDocumentHelper();
            }
            else if (ext.Equals("jpeg") || ext.Equals("jpg") || ext.Equals("png") || ext.Equals("bmp") || ext.Equals("gif") || ext.Equals("tiff"))
            {
                helper = new PictureDocumentHelper();
            }
            else
            {
                if (TextHighlightConfiguration.GetTextHighlightForTextFile(path) != null)
                {
                    helper = new TextDocumentHelper();
                }
            }
            return helper;
        }

        /// <summary>
        /// Evaluates a file by it's extension and decides if it is supported or not by KineSis
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean IsFileSupported(String path)
        {
            Boolean supported = false;
            FileInfo fi = new FileInfo(path);
            String ext = fi.Extension.Substring(1).ToLower();

            if (ext.Equals("docx"))
            {
                supported = true;
            }
            else if (ext.Equals("xlsx"))
            {
                supported = true;
            }
            else if (ext.Equals("pptx"))
            {
                supported = true;
            }
            else if (ext.Equals("jpeg") || ext.Equals("jpg") || ext.Equals("png") || ext.Equals("bmp") || ext.Equals("gif") || ext.Equals("tiff"))
            {
                supported = true;
            }
            else if (TextHighlightConfiguration.GetTextHighlightForTextFile(path) != null)
            {
                supported = true;
            }
            return supported;
        }
    }
}
