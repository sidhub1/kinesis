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

namespace KineSis.ContentManagement.Service {
    class DocumentService {
        public static String TEMP_DIRECTORY;
        public static int CHART_HORIZONTAL_FACES;
        public static int CHART_VERTICAL_FACES;
        public static int CHART_WIDTH;
        public static int SLIDE_WIDTH;
        public static int THUMB_WIDTH = 256;

        public static Boolean FORCE_CHART_SIZE = false;
        public static Boolean FOCUS_SHAPES = false;

        public static String IMAGE_EXTENSION = ImageUtil.PNG_EXTENSION;
        public static String IMAGE_FILTER = ImageUtil.PNG_FILTER;
        public static PpShapeFormat IMAGE_FORMAT = ImageUtil.PNG_FORMAT;

        private static TextHighlightConfiguration textHighlightConfiguration;

        /// <summary>
        /// Get text highlight configuration for active user
        /// </summary>
        public static TextHighlightConfiguration TextHighlightConfiguration {
            get {
                if (textHighlightConfiguration == null) {
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
        public static Document CreateNewDocument(String path, BackgroundWorker worker) {
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

            if (helper == null) {
                return null;
            }

            //create a processing progress
            ProcessingProgress pp = new ProcessingProgress(worker);

            //start parsing
            Document document = helper.ParseNewDocument(path, pp);
            return document;
        }

        /// <summary>
        /// After document was parsed, now it's time for charts, processed concurrent with presentation
        /// </summary>
        /// <param name="path">Source document path</param>
        /// <param name="worker">Background Worker</param>
        /// <param name="document">Current KineSis Document</param>
        public static void CreateNewDocumentCharts(String path, BackgroundWorker worker, Document document) {
            FileInfo file = new FileInfo(path);

            DocumentHelper helper = GetHelperForDocument(path);

            if (helper != null) {
                ProcessingProgress pp = new ProcessingProgress(worker);
                if (helper is PowerPointDocumentHelper) {
                    PowerPointDocumentHelper h = (PowerPointDocumentHelper)helper;
                    h.ParseNewDocumentCharts(path, pp, document);
                } else if (helper is ExcelDocumentHelper) {
                    ExcelDocumentHelper h = (ExcelDocumentHelper)helper;
                    h.ParseNewDocumentCharts(path, pp, document);
                } else if (helper is WordDocumentHelper) {
                    WordDocumentHelper h = (WordDocumentHelper)helper;
                    h.ParseNewDocumentCharts(path, pp, document);
                }

                Document.serialize(document, TEMP_DIRECTORY + "\\" + document.Location + ".kinesis");

                ProfileManager.AddDocumentToActive(document.Name, document.Location);
                ProfileManager.Serialize();
            }
        }

        /// <summary>
        /// Need to be removed, not used anymore
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Document LoadExistingDocument(String path) {
            DocumentHelper helper = new ExcelDocumentHelper();

            Document document = null;

            return document;
        }

        /// <summary>
        /// Syntax Highlighter comes as Embedded resource copied to bin. It need to be unzipped to user's temp directory
        /// </summary>
        private static void UnZipSyntaxHighlighterToTemp() {
            String path = Directory.GetCurrentDirectory() + "\\syntaxhighlighter_3.0.83.zip";
            FileInfo fi = new FileInfo(path);

            string zipToUnpack = path;
            string unpackDirectory = TEMP_DIRECTORY;
            if (!Directory.Exists(unpackDirectory + "\\syntaxhighlighter_3.0.83")) {
                using (ZipFile zip1 = ZipFile.Read(zipToUnpack)) {
                    foreach (ZipEntry e in zip1) {
                        e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            } else {
                Console.WriteLine(unpackDirectory + "\\syntaxhighlighter_3.0.83 already exists");
            }
        }

        /// <summary>
        /// Copy default Extensions.xml file to user's temp directory
        /// </summary>
        private static void CopyExtensionsFile() {
            String path = Directory.GetCurrentDirectory() + "\\Extensions.xml";
            string path2 = TEMP_DIRECTORY + "\\Extensions.xml";

            try {
                if (!File.Exists(path2)) {
                    File.Copy(path, path2);
                } else {
                    Console.WriteLine(path2 + " already exists");
                }
            } catch {
                Console.WriteLine("Cannot copy the Extensions.xml file");
            }
        }

        /// <summary>
        /// Get helper for processing a document, based on document extension
        /// </summary>
        /// <param name="path">Source document full path</param>
        /// <returns></returns>
        private static DocumentHelper GetHelperForDocument(String path) {
            DocumentHelper helper = null;
            FileInfo fi = new FileInfo(path);
            String ext = fi.Extension.Substring(1).ToLower();

            if (ext.Equals("docx")) {
                helper = new WordDocumentHelper();
            } else if (ext.Equals("xlsx")) {
                helper = new ExcelDocumentHelper();
            } else if (ext.Equals("pptx")) {
                helper = new PowerPointDocumentHelper();
            } else if (ext.Equals("jpeg") || ext.Equals("jpg") || ext.Equals("png") || ext.Equals("bmp") || ext.Equals("gif") || ext.Equals("tiff")) {
                helper = new PictureDocumentHelper();
            } else {
                Encoding enc = Encoding.Default;
                if (IsText(enc, path, 10)) {
                    helper = new TextDocumentHelper();
                }
            }
            return helper;
        }

        /// <summary>
        /// Detect if a file is text and detect the encoding.
        /// </summary>
        /// <param name="encoding">
        /// The detected encoding.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="windowSize">
        /// The number of characters to use for testing.
        /// </param>
        /// <returns>
        /// true if the file is text.
        /// </returns>
        public static bool IsText(Encoding encoding, string fileName, int windowSize) {
            using (var fileStream = File.OpenRead(fileName)) {
                var rawData = new byte[windowSize];
                var text = new char[windowSize];
                var isText = true;

                // Read raw bytes
                var rawLength = fileStream.Read(rawData, 0, rawData.Length);
                fileStream.Seek(0, SeekOrigin.Begin);

                // Detect encoding correctly (from Rick Strahl's blog)
                // http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
                if (rawData[0] == 0xef && rawData[1] == 0xbb && rawData[2] == 0xbf) {
                    encoding = Encoding.UTF8;
                } else if (rawData[0] == 0xfe && rawData[1] == 0xff) {
                    encoding = Encoding.Unicode;
                } else if (rawData[0] == 0 && rawData[1] == 0 && rawData[2] == 0xfe && rawData[3] == 0xff) {
                    encoding = Encoding.UTF32;
                } else if (rawData[0] == 0x2b && rawData[1] == 0x2f && rawData[2] == 0x76) {
                    encoding = Encoding.UTF7;
                } else {
                    encoding = Encoding.Default;
                }

                // Read text and detect the encoding
                using (var streamReader = new StreamReader(fileStream)) {
                    streamReader.Read(text, 0, text.Length);
                }

                using (var memoryStream = new MemoryStream()) {
                    using (var streamWriter = new StreamWriter(memoryStream, encoding)) {
                        // Write the text to a buffer
                        streamWriter.Write(text);
                        streamWriter.Flush();

                        // Get the buffer from the memory stream for comparision
                        var memoryBuffer = memoryStream.GetBuffer();

                        // Compare only bytes read
                        for (var i = 0; i < rawLength && isText; i++) {
                            isText = rawData[i] == memoryBuffer[i];
                        }
                    }
                }
                return isText;
            }
        }
    }
}
