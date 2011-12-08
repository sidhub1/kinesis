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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KineSis.ContentManagement.Model
{

    /// <summary>
    /// KineSis Document model
    /// </summary>
    [Serializable]
    public class Document
    {
        private String name;
        private String location;
        private String type;
        private List<Page> pages = new List<Page>();

        /// <summary>
        /// Name of the document
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Location of the document withing the user's temp directory
        /// </summary>
        public String Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        /// <summary>
        /// Type of the document
        /// </summary>
        public String Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Document pages list
        /// </summary>
        public List<Page> Pages
        {
            get
            {
                return pages;
            }

            set
            {
                pages = value;
            }
        }

        public static void serialize(Document document, String path)
        {
            Stream b = File.OpenWrite(path);
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(document.GetType());
            x.Serialize(b, document);
            b.Close();
        }

        public static Document deserialize(String path)
        {
            Document doc = new Document();

            Stream b = File.OpenRead(path);
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(doc.GetType());
            doc = x.Deserialize(b) as Document;
            b.Close();

            return doc;
        }
    }
}
