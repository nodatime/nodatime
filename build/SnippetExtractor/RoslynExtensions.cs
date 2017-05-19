// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetExtractor
{
    public static class RoslynExtensions
    {
        /// <summary>
        /// Returns the lines within the block.
        /// </summary>
        public static IEnumerable<string> GetLines(this BlockSyntax block) =>
            block.SyntaxTree.GetText().GetSubText(block.Statements.FullSpan).Lines.Select(line => line.ToString());

        public static Compilation CheckSuccessful(this Compilation compilation)
        {
            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
            if (errors.Any())
            {
                throw new Exception("Build error:\r\n" + string.Join("\r\n", errors.Select(e => e.GetMessage())));
            }
            return compilation;
        }
    }
}
