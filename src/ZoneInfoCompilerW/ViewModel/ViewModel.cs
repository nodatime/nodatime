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

using System;
using System.ComponentModel;

namespace ZoneInfoCompilerW.ViewModel
{
    /// <summary>
    ///   Provides a base class for ViewModel objects.
    /// </summary>
    internal class ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        /// <summary>
        ///   Fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        ///   Helper method to fire a property change notification.
        /// </summary>
        /// <remarks>
        ///   To single an "all properies changed" message use an empty string for the
        ///   <paramref name = "propertyName" /> argument.
        /// </remarks>
        /// <param name = "propertyName">The name of the changed property.</param>
        /// <exception cref = "ArgumentNullException">The propertyName argument is null.</exception>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
