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
using System.Windows.Controls;

namespace KineSis.UserInterface.Entities
{
    /// <summary>
    /// element used by Select group. Can be a page or a chart
    /// </summary>
    class Element
    {
        private String name;
        private Image thumbnail;

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

        public Image Thumbnail
        {
            get
            {
                return thumbnail;
            }

            set
            {
                thumbnail = value;
            }
        }
    }
}
