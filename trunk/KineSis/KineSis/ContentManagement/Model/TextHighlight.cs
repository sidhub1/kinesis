/*
   Copyright 2012 Alexandru Albu - sandu.albu@gmail.com

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

namespace KineSis.ContentManagement.Model
{

    /// <summary>
    /// Text Highlight Utility class
    /// </summary>
    class TextHighlight
    {
        private String filenameExtension;
        private String brushAlias;
        private String jsName;
        private String theme;

        private static Dictionary<String, List<String>> acceptedValues;
        private static List<String> acceptedThemes;

        /// <summary>
        /// Accepted scripts and brushes dictionary...that's all supported in TextHighlight 3.0.83
        /// </summary>
        private static Dictionary<String, List<String>> AcceptedValues
        {
            get
            {
                if (acceptedValues == null)
                {
                    acceptedValues = new Dictionary<String, List<String>>();
                    acceptedValues.Add("shBrushAS3.js", new string[] { "as3", "actionscript3" }.ToList<String>());
                    acceptedValues.Add("shBrushBash.js", new string[] { "bash", "shell" }.ToList<String>());
                    acceptedValues.Add("shBrushColdFusion.js", new string[] { "cf", "coldfusion" }.ToList<String>());
                    acceptedValues.Add("shBrushCSharp.js", new string[] { "c-sharp", "csharp" }.ToList<String>());
                    acceptedValues.Add("shBrushCpp.js", new string[] { "cpp", "c" }.ToList<String>());
                    acceptedValues.Add("shBrushCss.js", new string[] { "css" }.ToList<String>());
                    acceptedValues.Add("shBrushDelphi.js", new string[] { "delphi", "pas", "pascal" }.ToList<String>());
                    acceptedValues.Add("shBrushDiff.js", new string[] { "diff", "patch" }.ToList<String>());
                    acceptedValues.Add("shBrushErlang.js", new string[] { "erl", "erlang" }.ToList<String>());
                    acceptedValues.Add("shBrushGroovy.js", new string[] { "groovy" }.ToList<String>());
                    acceptedValues.Add("shBrushJScript.js", new string[] { "js", "jscript", "javascript" }.ToList<String>());
                    acceptedValues.Add("shBrushJava.js", new string[] { "java" }.ToList<String>());
                    acceptedValues.Add("shBrushJavaFX.js", new string[] { "jfx", "javafx" }.ToList<String>());
                    acceptedValues.Add("shBrushPerl.js", new string[] { "perl", "pl" }.ToList<String>());
                    acceptedValues.Add("shBrushPhp.js", new string[] { "php" }.ToList<String>());
                    acceptedValues.Add("shBrushPlain.js", new string[] { "plain", "text" }.ToList<String>());
                    acceptedValues.Add("shBrushPowerShell.js", new string[] { "ps", "powershell" }.ToList<String>());
                    acceptedValues.Add("shBrushPython.js", new string[] { "py", "python" }.ToList<String>());
                    acceptedValues.Add("shBrushRuby.js", new string[] { "rails", "ror", "ruby" }.ToList<String>());
                    acceptedValues.Add("shBrushScala.js", new string[] { "scala" }.ToList<String>());
                    acceptedValues.Add("shBrushSql.js", new string[] { "sql" }.ToList<String>());
                    acceptedValues.Add("shBrushVb.js", new string[] { "vb", "vbnet" }.ToList<String>());
                    acceptedValues.Add("shBrushXml.js", new string[] { "xml", "xhtml", "xslt", "html" }.ToList<String>());
                }
                return acceptedValues;
            }
        }

        /// <summary>
        /// Accepted themes list...that's all supported in TextHighlight 3.0.83
        /// </summary>
        private static List<String> AcceptedThemes
        {
            get
            {
                if (acceptedThemes == null)
                {
                    acceptedThemes = new List<String>();
                    acceptedThemes.Add("shCoreDefault.css");
                    acceptedThemes.Add("shCoreDjango.css");
                    acceptedThemes.Add("shCoreEclipse.css");
                    acceptedThemes.Add("shCoreEmacs.css");
                    acceptedThemes.Add("shCoreFadeToGrey.css");
                    acceptedThemes.Add("shCoreMDUltra.css");
                    acceptedThemes.Add("shCoreMidnight.css");
                    acceptedThemes.Add("shCoreRDark.css");
                }

                return acceptedThemes;
            }
        }

        /// <summary>
        /// Check if current text highlight is accepted
        /// </summary>
        public Boolean IsAccepted
        {
            get
            {
                Boolean isAccepted = true;
                if (filenameExtension == null || brushAlias == null || jsName == null || theme == null || filenameExtension.Length == 0 || brushAlias.Length == 0 || jsName.Length == 0 || theme.Length == 0)
                {
                    isAccepted = false;
                }
                else if (!AcceptedThemes.Contains(theme))
                {
                    isAccepted = false;
                }
                else if (!AcceptedValues.Keys.Contains(jsName))
                {
                    isAccepted = false;
                }
                else if (!AcceptedValues[jsName].Contains(brushAlias))
                {
                    isAccepted = false;
                }
                else
                {
                    if (filenameExtension[0] != '.')
                    {
                        isAccepted = false;
                    }
                    else
                    {
                        String shortFN = filenameExtension.Substring(1);
                        if (shortFN.Length < 1)
                        {
                            isAccepted = false;
                        }
                        else
                        {
                            for (int i = 0; i < shortFN.Length; i++)
                            {
                                char c = shortFN[i];
                                if ((c < '0') || (c > '9' && c < 'a') || (c > 'z'))
                                {
                                    isAccepted = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                return isAccepted;
            }
        }

        /// <summary>
        /// Filename extension
        /// </summary>
        public String FilenameExtension
        {
            get
            {
                return filenameExtension;
            }

            set
            {
                filenameExtension = value.ToLower();
            }
        }

        /// <summary>
        /// Brush
        /// </summary>
        public String BrushAlias
        {
            get
            {
                return brushAlias;
            }

            set
            {
                brushAlias = value;
            }
        }

        /// <summary>
        /// Script
        /// </summary>
        public String JsName
        {
            get
            {
                return jsName;
            }

            set
            {
                if (value != null)
                {
                    jsName = "shBrush" + value + ".js";
                }
            }
        }

        /// <summary>
        /// Theme
        /// </summary>
        public String Theme
        {
            get
            {
                return theme;
            }

            set
            {
                if (value != null)
                {
                    theme = "shCore" + value + ".css";
                }
            }
        }

        /// <summary>
        /// String representation of the current text highlight
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "{ \"filenameExtension\":\"" + filenameExtension + "\", \"brushAlias\":\"" + brushAlias + "\", \"jsName\":\"" + jsName + "\", \"theme\":\"" + theme + "\" }";
        }
    }
}
