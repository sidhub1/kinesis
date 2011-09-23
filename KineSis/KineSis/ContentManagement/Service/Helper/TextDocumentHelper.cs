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

namespace KineSis.ContentManagement.Service.Helper {

    /// <summary>
    /// Handles the text documents
    /// generates a html file which combine the document text with syntaxhighlighter 3.0.83 for a nice styling. 
    /// </summary>
    class TextDocumentHelper : DocumentHelper {

        private static String DD = "\\"; //directory delimiter
        private static String DOC_FILES = "document_files"; //document_files
        private static String DOCUMENT_HTML = "document.html";

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
            pp.CurrentOperationTotalElements = 2;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            Document document = new Document();
            document.Name = path.Substring(path.LastIndexOf(DD) + 1);
            document.Location = folderName;

            //write directory for pages
            String pagesPath = System.IO.Path.Combine(documentPath + DD + DOC_FILES);
            System.IO.Directory.CreateDirectory(pagesPath);

            Page page = new Page();
            page.Name = document.Name;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            pp.OverallOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //location where the page is generated
            page.Location = WriteTextDocument(path, documentPath + DD + DOC_FILES + DD + DOCUMENT_HTML);
            document.Pages.Add(page);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 2;
            pp.OverallOperationElement = 2;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //return the built document
            return document;
        }

        /// <summary>
        /// Build the nice html document based on input source
        /// </summary>
        /// <param name="path">full source name</param>
        /// <param name="destination">full destination name</param>
        /// <returns></returns>
        private String WriteTextDocument(String path, String destination) {
            //read the source
            System.IO.StreamReader fileI = new System.IO.StreamReader(path);
            String content = fileI.ReadToEnd();
            fileI.Close();

            //write the destination
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(destination, false);
            fileO.Write(GetDocumentHeader(path));   //header
            fileO.Write(content); //content
            fileO.Write(GetDocumentFooter()); //footer
            fileO.Flush();
            fileO.Close();
            return destination;
        }

        /// <summary>
        /// Build the document header (opening) based on input filename extension and existing text highlight configurations
        /// </summary>
        /// <param name="path">full source name</param>
        /// <returns></returns>
        private String GetDocumentHeader(String path) {
            //get text higlight for given path
            TextHighlight th = DocumentService.TextHighlightConfiguration.GetTextHighlightForTextFile(path);
            //the base string
            String result = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><script type=\"text/javascript\" src=\"../../syntaxhighlighter_3.0.83/scripts/shCore.js\"></script>";

            //this will be changed depending on extension
            result += "<script type=\"text/javascript\" src=\"../../syntaxhighlighter_3.0.83/scripts/";

            //language
            result += th.JsName;

            result += "\"></script>";

            //this is about theme
            result += "<link type=\"text/css\" rel=\"stylesheet\" href=\"../../syntaxhighlighter_3.0.83/styles/";

            result += th.Theme;

            result += "\"/><script type=\"text/javascript\">SyntaxHighlighter.all();</script></head><body style=\"background: white; font-family: Helvetica;  margin: 0px\"><script type=\"syntaxhighlighter\"  class=\"brush: ";

            //brush
            result += th.BrushAlias;

            //and the last one
            result += "\"><![CDATA[";

            return result;
        }

        /// <summary>
        /// return footer for document
        /// </summary>
        /// <returns>the closure for generating html document</returns>
        private String GetDocumentFooter() {
            return "]]></script></html>";
        }
    }
}
