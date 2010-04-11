#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.IO;
using System.Reflection;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Helper methods to get various Assembly information.
    /// </summary>
    public static class AssemblyInfo
    {
        /// <summary>
        /// Gets the main assembly's title.
        /// </summary>
        public static string Title
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    object[] attributes = program.GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute) attributes[0];
                        if (!string.IsNullOrEmpty(titleAttribute.Title))
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return Path.GetFileNameWithoutExtension(program.CodeBase);
                }
                return "Title";
            }
        }

        /// <summary>
        /// Gets the main assembly's version.
        /// </summary>
        public static string Version
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    return program.GetName().Version.ToString();
                }
                return "0.0.0.1";
            }
        }

        /// <summary>
        /// Gets the main assembly's description.
        /// </summary>
        public static string Description
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    object[] attributes = program.GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        return ((AssemblyDescriptionAttribute) attributes[0]).Description;
                    }
                }
                return "Description";
            }
        }

        /// <summary>
        /// Gets the main assembly's product name.
        /// </summary>
        public static string Product
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    object[] attributes = program.GetCustomAttributes(typeof (AssemblyProductAttribute), false);
                    if (attributes.Length > 0)
                    {
                        return ((AssemblyProductAttribute) attributes[0]).Product;
                    }
                }
                return "Product";
            }
        }

        /// <summary>
        /// Gets the main assembly's copyright.
        /// </summary>
        public static string Copyright
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    object[] attributes = program.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
                    if (attributes.Length > 0)
                    {
                        return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
                    }
                }
                return "Copyright";
            }
        }

        /// <summary>
        /// Gets the main assembly's company name.
        /// </summary>
        public static string Company
        {
            get
            {
                Assembly program = Assembly.GetEntryAssembly();
                if (program != null)
                {
                    object[] attributes = program.GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
                    if (attributes.Length > 0)
                    {
                        return ((AssemblyCompanyAttribute) attributes[0]).Company;
                    }
                }
                return "Company";
            }
        }
    }
}