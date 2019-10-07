// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Demo
{
    // TODO: Move this into an entirely separate project, along with the
    // code to consume it.
    public static class Snippet
    {
        /// <summary>
        /// Indicates that the block containing the invocation of this method
        /// should be used to generate a sample for the member used to produce the value.
        /// </summary>
        /// <typeparam name="T">The type of value produced</typeparam>
        /// <param name="value">The value; at execution time, this is returned directly...
        /// but at sample generation time, the expression is used to examine which
        /// member to document.
        /// </param>
        /// <returns><paramref name="value"/></returns>
        public static T For<T>(T value) => value;

        /// <summary>
        /// Indicates that the expression within <paramref name="action"/> should be used to
        /// generate a sample for the member invoked.
        /// </summary>
        /// <param name="action">An action, which should be expressed as an expression-bodied lambda expression.</param>
        public static void ForAction(Action action) { }

        /// <summary>
        /// Indicates that the expression within <paramref name="action"/> should be used to
        /// generate a sample for the member invoked, but that the expression should be removed entirely
        /// rather than just replacing the Snippet call.
        /// </summary>
        /// <param name="action">An action, which should be expressed as an expression-bodied lambda expression.</param>
        public static void SilentForAction(Action action) { }

        // TODO: Allow multiple values in a "For" expression, to allow for easy overloading.
        // (Challenge is to work out what the resulting snippet should look like - especially
        // if there are multiple Snippet.For calls. Maybe prohibit that scenario...)
    }
}
