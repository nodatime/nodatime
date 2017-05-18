// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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

        // TODO: Snippets for a void method. Probably accept Action and execute it...
        // TODO: Allow multiple values in a "For" expression, to allow for easy overloading.
        // (Challenge is to work out what the resulting snippet should look like - especially
        // if there are multiple Snippet.For calls. Maybe prohibit that scenario...)
    }
}
