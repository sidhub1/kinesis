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
using KineSis.ContentManagement.Model;
using KineSis.ContentManagement.Progress;
using System.IO;

namespace KineSis.ContentManagement.Service.Helper {

    /// <summary>
    /// Handles the pictures
    /// creates a copy of the picture in temp directory, and generate a html page, just good to be rendered within kinesis
    /// </summary>
    class PictureDocumentHelper : DocumentHelper {

        private static String DD = "\\"; //directory delimiter
        private static String DOC_FILES = "document_files"; //document_files
        private static String DOCUMENT_HTML = "document.html";
        private static String DOCUMENT_NO_ZOOM_HTML = "documentNoZoom.html";

        /// <summary>
        /// parse a picture
        /// </summary>
        /// <param name="path">full path of the picture</param>
        /// <returns>an equivalent kinesis document model</returns>
        Document DocumentHelper.ParseNewDocument(String path, ProcessingProgress pp) {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "Document";
            pp.OverallOperationTotalElements = 2;
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //directory where all the data will be saved
            String folderName = DocumentUtil.GenerateDirectoryName();
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);
            System.IO.Directory.CreateDirectory(documentPath);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationElement = 1;
            pp.CurrentOperationName = "Opening";
            pp.CurrentOperationTotalElements = 3;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            Document document = new Document();
            document.Name = path.Substring(path.LastIndexOf(DD) + 1);   //only the filename alone
            document.Location = folderName;

            //write directory for pages
            String pagesPath = documentPath + DD + DOC_FILES;
            System.IO.Directory.CreateDirectory(pagesPath);

            //new page
            Page page = new Page();
            page.Name = document.Name;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //location of the page content
            page.Location = WritePictureDocument(path, pagesPath);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 2;
            pp.OverallOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            page.LocationNoZoom = WritePictureDocumentNoZoom(path, pagesPath);

            //add page to the document
            document.Pages.Add(page);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 3;
            pp.OverallOperationElement = 2;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //return the built document model
            return document;
        }

        /// <summary>
        /// copy the original picture to temp and create a new html document having copied picture as content
        /// </summary>
        /// <param name="path">path of the original picture</param>
        /// <param name="pagesPath">path to temp pages directory for the document</param>
        /// <returns>full path to the generated html page</returns>
        private String WritePictureDocument(String path, String pagesPath) {
            FileInfo fi = new FileInfo(path);
            FileInfo fic = new FileInfo(pagesPath + DD + "picture" + fi.Extension);
            if (!fic.Exists) {
                fi.CopyTo(fic.FullName);
            }
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(pagesPath + DD + DOCUMENT_HTML, false);
            fileO.WriteLine("<html><body><div align=\"center\"><img src=\"" + fic.FullName + "\"/></div></body></html>");
            fileO.Flush();
            fileO.Close();
            return fic.FullName;
        }

        /// <summary>
        /// copy the original picture to temp and create a new html document having copied picture as content
        /// include some javascript functions and css styling which always scale the picture to browser's width, which makes zoom possibility unavailable
        /// </summary>
        /// <param name="path">path of the original picture</param>
        /// <param name="pagesPath">path to temp pages directory for the document</param>
        /// <returns>full path to the generated html page</returns>
        private String WritePictureDocumentNoZoom(String path, String pagesPath) {
            FileInfo fi = new FileInfo(path);
            FileInfo fic = new FileInfo(pagesPath + DD + "picture" + fi.Extension);
            if (!fic.Exists) {
                fi.CopyTo(fic.FullName);
            }
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(pagesPath + DD + DOCUMENT_NO_ZOOM_HTML, false);
            fileO.Write("<html><head><style type=\"text/css\">*{margin:0;padding:0;}#imgId{width:100%;height:auto;}</style></head><body><center><img id=\"imgId\" src=\"");
            fileO.Write(fic.FullName);
            fileO.Write("\"/></center><script type=\"text/javascript\"> var img = document.getElementById('imgId'); window.onresize = function(){adjustRatio(img);} document.onload = function(){adjustRatio(img);} var imageratio = img.height/img.width;function adjustRatio(img) {var winheight = (document.body.clientHeight === undefined)?window.innerHeight:document.body.clientHeight;var winwidth = (document.body.clientWidth === undefined)?window.innerWidth:document.body.clientWidth;winratio = winheight / winwidth;if(winratio < imageratio){img.style.height = '100%';img.style.width = 'auto';}else{img.style.width = '100%';img.style.height = 'auto';}}setTimeout(\"adjustRatio(img)\",1);</script></body></html>");
            fileO.Flush();
            fileO.Close();
            return fic.FullName;
        }
    }
}
