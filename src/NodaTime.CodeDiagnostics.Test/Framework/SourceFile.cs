// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.CodeDiagnostics.Test.Framework
{
    /// <summary>
    /// A logical source file, with a path and content.
    /// </summary>
    internal sealed class SourceFile
    {
        /// <summary>
        /// The source code within the file.
        /// </summary>
        internal string Content { get; }

        /// <summary>
        /// The path to the file.
        /// </summary>
        internal string Path { get; }

        public SourceFile(string content, string path)
        {
            this.Content = content;
            this.Path = path;
        }
    }
}
