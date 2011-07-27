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

#region usings
using System;
using System.Globalization;
using System.Threading;
#endregion

namespace NodaTime.Test
{
    /// <summary>
    ///   Provides a simple method for setting the culture of a thread so it can be reset
    ///   to what it was. Designed to be used in a <c>using</c> statement.
    /// </summary>
    /// <example>
    ///   using (CultureSaver.SetUiCulture(new CultureInfo("en-US"))) {
    ///   // Code to run under the United States English culture
    ///   }
    /// </example>
    public static class CultureSaver
    {
        /// <summary>
        ///   Sets the basic culture.
        /// </summary>
        /// <param name = "newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref = "IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetBasicCulture(CultureInfo newCultureInfo)
        {
            return new BasicSaver(newCultureInfo);
        }

        /// <summary>
        ///   Sets the UI culture of the current thread.
        /// </summary>
        /// <param name = "newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref = "IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetUiCulture(CultureInfo newCultureInfo)
        {
            return new UiSaver(newCultureInfo);
        }

        /// <summary>
        ///   Sets both the UI and basic cultures of the current thread.
        /// </summary>
        /// <param name = "newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref = "IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetCultures(CultureInfo newCultureInfo)
        {
            return new BothSaver(newCultureInfo);
        }

        #region Nested type: BasicSaver
        /// <summary>
        ///   Provides the <see cref = "IDisposable" /> for saving the original basic culture and resetting
        ///   it back.
        /// </summary>
        private sealed class BasicSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;

            /// <summary>
            ///   Initializes a new instance of the <see cref = "BasicSaver" /> class.
            /// </summary>
            /// <param name = "newCulture">The new basic culture to set.</param>
            public BasicSaver(CultureInfo newCulture)
            {
                oldCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;
            }

            #region IDisposable Members
            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
            #endregion
        }
        #endregion

        #region Nested type: BothSaver
        /// <summary>
        ///   Provides the <see cref = "IDisposable" /> for saving the original cultures and resetting
        ///   them back.
        /// </summary>
        private sealed class BothSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;
            private readonly CultureInfo oldUiCulture;

            /// <summary>
            ///   Initializes a new instance of the <see cref = "UiSaver" /> class.
            /// </summary>
            /// <param name = "newCulture">The new UI culture to set.</param>
            public BothSaver(CultureInfo newCulture)
            {
                oldCulture = Thread.CurrentThread.CurrentCulture;
                oldUiCulture = Thread.CurrentThread.CurrentUICulture;

                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }

            #region IDisposable Members
            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
                Thread.CurrentThread.CurrentUICulture = oldUiCulture;
            }
            #endregion
        }
        #endregion

        #region Nested type: UiSaver
        /// <summary>
        ///   Provides the <see cref = "IDisposable" /> for saving the original UI culture and resetting
        ///   it back.
        /// </summary>
        private sealed class UiSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;

            /// <summary>
            ///   Initializes a new instance of the <see cref = "UiSaver" /> class.
            /// </summary>
            /// <param name = "newCulture">The new UI culture to set.</param>
            public UiSaver(CultureInfo newCulture)
            {
                oldCulture = Thread.CurrentThread.CurrentUICulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }

            #region IDisposable Members
            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Thread.CurrentThread.CurrentUICulture = oldCulture;
            }
            #endregion
        }
        #endregion
    }
}