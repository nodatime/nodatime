// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.IO;

namespace SnippetExtractor
{
    /// <summary>
    /// A snippet after rewriting and execution, ready to be written to a file.
    /// </summary>
    public class RewrittenSnippet
    {
        public string Script { get; }
        public string Output { get; }
        public string Uid { get; }

        public RewrittenSnippet(string script, string output, string uid)
        {
            Script = script;
            Output = output;
            Uid = uid;
        }

        /// <summary>
        /// Writes the snippet in a form suitable for DocFX.
        /// </summary>
        internal void Write(TextWriter writer)
        {
            writer.WriteLine("---");
            writer.WriteLine($"uid: {Uid}");
            writer.WriteLine("snippet: *content");
            writer.WriteLine("---");
            writer.WriteLine();
            writer.WriteLine("```csharp");
            writer.WriteLine(Script);
            writer.WriteLine("```");
            writer.WriteLine();
            writer.WriteLine("Output:");
            writer.WriteLine();
            writer.WriteLine("```text");
            writer.WriteLine(Output);
            writer.WriteLine("```");
            writer.WriteLine();
        }
    }
}
