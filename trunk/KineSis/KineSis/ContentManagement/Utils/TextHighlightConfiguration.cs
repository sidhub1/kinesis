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
using KineSis.ContentManagement.Model;
using System.Xml;

namespace KineSis.ContentManagement.Utils
{

    /// <summary>
    /// TextHighlighting configuration - for text documents
    /// </summary>
    class TextHighlightConfiguration
    {

        private static List<TextHighlight> textHighlights;

        /// <summary>
        /// Construct a configuration from a configuration xml file
        /// </summary>
        /// <param name="configurationXmlFile">configuration xml file</param>
        public TextHighlightConfiguration(String configurationXmlFile)
        {
            if (textHighlights == null)
            {
                XmlTextReader textReader = new XmlTextReader(configurationXmlFile);
                textHighlights = new List<TextHighlight>();
                while (textReader.Read())
                {
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("sourceCodeFormatterConf"))
                    {
                        while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("sourceCodeFormatterConf")))
                        {
                            if (textReader.Read())
                            {
                                if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("conf"))
                                {
                                    TextHighlight th = new TextHighlight();

                                    while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("conf")))
                                    {
                                        if (textReader.Read())
                                        {
                                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("brushAlias"))
                                            {
                                                String text = null;
                                                while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("brushAlias")))
                                                {
                                                    if (textReader.Read())
                                                    {
                                                        if (textReader.NodeType == XmlNodeType.Text)
                                                        {
                                                            text = textReader.Value.ToString();
                                                        }
                                                    }
                                                }
                                                th.BrushAlias = text;
                                            }
                                            else if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("jsName"))
                                            {
                                                String text = null;
                                                while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("jsName")))
                                                {
                                                    if (textReader.Read())
                                                    {
                                                        if (textReader.NodeType == XmlNodeType.Text)
                                                        {
                                                            text = textReader.Value.ToString();
                                                        }
                                                    }
                                                }
                                                th.JsName = text;
                                            }
                                            else if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("filenameExtension"))
                                            {
                                                String text = null;
                                                while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("filenameExtension")))
                                                {
                                                    if (textReader.Read())
                                                    {
                                                        if (textReader.NodeType == XmlNodeType.Text)
                                                        {
                                                            text = textReader.Value.ToString();
                                                        }
                                                    }
                                                }
                                                th.FilenameExtension = text;
                                            }
                                            else if (textReader.NodeType == XmlNodeType.Element && textReader.Name.Equals("theme"))
                                            {
                                                String text = null;
                                                while (!(textReader.NodeType == XmlNodeType.EndElement && textReader.Name.Equals("theme")))
                                                {
                                                    if (textReader.Read())
                                                    {
                                                        if (textReader.NodeType == XmlNodeType.Text)
                                                        {
                                                            text = textReader.Value.ToString();
                                                        }
                                                    }
                                                }
                                                th.Theme = text;
                                            }
                                        }
                                    }
                                    if (th.IsAccepted)
                                    {
                                        textHighlights.Add(th);
                                        System.Console.WriteLine("New Conf:" + th.ToString());
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Skipped:" + th.ToString());
                                    }
                                }
                            }
                        }

                    }
                }
                textReader.Close();
            }
        }

        /// <summary>
        /// All available text highlights
        /// </summary>
        public List<TextHighlight> TextHighlights
        {
            get
            {
                return textHighlights;
            }
        }

        /// <summary>
        /// String representation of configuration
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = base.ToString();
            if (textHighlights != null)
            {
                str = "{ \"textHighlights\": ";
                foreach (TextHighlight th in textHighlights)
                {
                    str += th.ToString();
                    str += ", ";
                }
                str = str.Substring(0, str.LastIndexOf(", "));
                str += " }";
            }
            return str;
        }

        /// <summary>
        /// Get a highlighter for a file based on extension
        /// </summary>
        /// <param name="path">source file full path</param>
        /// <returns></returns>
        public TextHighlight GetTextHighlightForTextFile(String path)
        {
            String extension = path.Substring(path.LastIndexOf("."));
            TextHighlight th = null;
            foreach (TextHighlight t in textHighlights)
            {
                if (t.FilenameExtension.Equals(extension))
                {
                    th = t;
                    break;
                }
            }
            return th;
        }
    }
}
