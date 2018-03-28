// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Threading;

namespace NodaTime.Test
{
    /// <summary>
    /// Provides a simple method for setting the culture of a thread so it can be reset
    /// to what it was. Designed to be used in a <c>using</c> statement.
    /// </summary>
    /// <example>
    ///   using (CultureSaver.SetUiCulture(new CultureInfo("en-US"))) {
    ///   // Code to run under the United States English culture
    ///   }
    /// </example>
    /// <remarks>
    /// Currently only SetCultures is used, in order to ensure that the UI culture is
    /// not used; the remaining methods are preserved at the moment in case we need them
    /// later.
    /// </remarks>
    public static class CultureSaver
    {
        // Abstractions over Thread.CurrentThread.CurrentCulture / CultureInfo.CurrentCulture etc.

#if NETCOREAPP1_0
        private static CultureInfo CurrentCulture
        {
            get { return CultureInfo.DefaultThreadCurrentCulture; }
            set { CultureInfo.DefaultThreadCurrentCulture = value; }
        }

        private static CultureInfo CurrentUICulture
        {
            get { return CultureInfo.DefaultThreadCurrentUICulture; }
            set { CultureInfo.DefaultThreadCurrentUICulture = value; }
        }
#else
        private static CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
            set { Thread.CurrentThread.CurrentCulture  = value; }
        }

        private static CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }
#endif

        /// <summary>
        /// Sets the basic culture.
        /// </summary>
        /// <param name="newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref="IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetBasicCulture(CultureInfo newCultureInfo) => new BasicSaver(newCultureInfo);

        /// <summary>
        /// Sets the UI culture of the current thread.
        /// </summary>
        /// <param name="newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref="IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetUiCulture(CultureInfo newCultureInfo) => new UiSaver(newCultureInfo);

        /// <summary>
        /// Sets both the UI and basic cultures of the current thread.
        /// </summary>
        /// <param name="newCultureInfo">The new culture info.</param>
        /// <returns>An <see cref="IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetCultures(CultureInfo newCultureInfo) => new BothSaver(newCultureInfo, newCultureInfo);

        /// <summary>
        /// Sets both the UI and basic cultures of the current thread.
        /// </summary>
        /// <param name="newCultureInfo">The new culture info.</param>
        /// <param name="newUiCultureInfo">The new UI culture info.</param>
        /// <returns>An <see cref="IDisposable" /> so the culture can be reset.</returns>
        public static IDisposable SetCultures(CultureInfo newCultureInfo, CultureInfo newUiCultureInfo)
            => new BothSaver(newCultureInfo, newUiCultureInfo);

        /// <summary>
        /// Provides the <see cref="IDisposable" /> for saving the original basic culture and resetting
        /// it back.
        /// </summary>
        private sealed class BasicSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;

            /// <summary>
            /// Initializes a new instance of the <see cref="BasicSaver" /> class.
            /// </summary>
            /// <param name="newCulture">The new basic culture to set.</param>
            public BasicSaver(CultureInfo newCulture)
            {
                oldCulture = CurrentCulture;
                CurrentCulture = newCulture;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                CurrentCulture = oldCulture;
            }
        }

        /// <summary>
        /// Provides the <see cref="IDisposable" /> for saving the original cultures and resetting
        /// them back.
        /// </summary>
        private sealed class BothSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;
            private readonly CultureInfo oldUiCulture;

            /// <summary>
            /// Initializes a new instance of the <see cref="UiSaver" /> class.
            /// </summary>
            /// <param name="newCulture">The new basic culture to set.</param>
            /// <param name="newUiCulture">The new UI culture to set.</param>
            public BothSaver(CultureInfo newCulture, CultureInfo newUiCulture)
            {
                oldCulture = CurrentCulture;
                oldUiCulture = CurrentUICulture;

                CurrentCulture = newCulture;
                CurrentUICulture = newUiCulture;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                CurrentCulture = oldCulture;
                CurrentUICulture = oldUiCulture;
            }
        }
        /// <summary>
        /// Provides the <see cref="IDisposable" /> for saving the original UI culture and resetting
        /// it back.
        /// </summary>
        private sealed class UiSaver : IDisposable
        {
            private readonly CultureInfo oldCulture;

            /// <summary>
            /// Initializes a new instance of the <see cref="UiSaver" /> class.
            /// </summary>
            /// <param name="newCulture">The new UI culture to set.</param>
            public UiSaver(CultureInfo newCulture)
            {
                oldCulture = CurrentUICulture;
                CurrentUICulture = newCulture;
            }

            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                CurrentUICulture = oldCulture;
            }
        }
    }
}
