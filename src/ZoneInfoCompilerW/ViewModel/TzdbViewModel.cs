#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NodaTime.ZoneInfoCompiler;
using ZoneInfoCompilerW.Properties;

namespace ZoneInfoCompilerW.ViewModel
{
    /// <summary>
    ///   Provides the view model for the TZDB time zone file processor page.
    /// </summary>
    internal class TzdbViewModel : ViewModel
    {
        private ResourceOutputType outputType = ResourceOutputType.ResX;
        private string sourceDirectoryName;
        private string targetFileName;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WindowsMapViewModel" /> class.
        /// </summary>
        public TzdbViewModel()
        {
            SourceDirectoryName = Settings.Default.TzdbSourceDirectoryName;
            TargetFileName = Settings.Default.TzdbTargetFileName;
            OutputType = Settings.Default.TzdbResourceOutputType;
        }

        /// <summary>
        ///   Gets and sets if the output should be in ResX format.
        /// </summary>
        public bool IsOutputResX
        {
            get { return OutputType == ResourceOutputType.ResX; }
            set { OutputType = value ? ResourceOutputType.ResX : ResourceOutputType.Resource; }
        }

        /// <summary>
        ///   Gets and sets if the output should be in Resource format.
        /// </summary>
        public bool IsOutputResource
        {
            get { return OutputType == ResourceOutputType.Resource; }
            set { OutputType = value ? ResourceOutputType.Resource : ResourceOutputType.ResX; }
        }

        /// <summary>
        ///   Gets and sets the output format.
        /// </summary>
        public ResourceOutputType OutputType
        {
            get { return outputType; }
            set
            {
                outputType = value;
                NotifyPropertyChanged("OutputType");
                NotifyPropertyChanged("IsOutputResX");
                NotifyPropertyChanged("IsOutputResource");
            }
        }

        /// <summary>
        ///   Gets or sets the source directory name.
        /// </summary>
        public string SourceDirectoryName
        {
            get { return sourceDirectoryName; }
            set
            {
                sourceDirectoryName = value;
                NotifyPropertyChanged("SourceDirectoryName");
            }
        }

        /// <summary>
        ///   Gets or sets the target file name.
        /// </summary>
        public string TargetFileName
        {
            get { return targetFileName; }
            set
            {
                targetFileName = value;
                NotifyPropertyChanged("TargetFileName");
            }
        }
    }
}
