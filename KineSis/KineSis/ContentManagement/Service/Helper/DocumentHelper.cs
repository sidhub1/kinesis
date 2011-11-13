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
using KineSis.ContentManagement.Model;
using KineSis.ContentManagement.Progress;

namespace KineSis.ContentManagement.Service.Helper
{

    /// <summary>
    /// interface for all document helpers
    /// </summary>
    interface DocumentHelper
    {

        /// <summary>
        /// parse an input document and return a kinesis document model
        /// </summary>
        /// <param name="path">full path of the document</param>
        /// <returns>equivalent kinesis document model</returns>
        Document ParseNewDocument(String path, ProcessingProgress pp);
    }
}
