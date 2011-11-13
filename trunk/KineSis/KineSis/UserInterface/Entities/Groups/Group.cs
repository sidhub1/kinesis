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
using System.IO;

namespace KineSis.UserInterface.Entities.Groups
{
    /// <summary>
    /// interface for groups. a group is basically a menu, available in UI to user. It have a name, an availability, and can render a menu or an user action
    /// </summary>
    interface Group
    {
        /// <summary>
        /// group's name
        /// </summary>
        String Name
        {
            get;
        }

        /// <summary>
        /// can be available or not
        /// </summary>
        Boolean IsActive
        {
            get;
        }

        /// <summary>
        /// when the group is active, this method renders the menu presented to user, depending on skeleton parts position, document and more. 
        /// </summary>
        /// <param name="c"></param>
        void Draw(Canvas c);
    }
}
