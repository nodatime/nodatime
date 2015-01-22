// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using System;

namespace NodaTime.CodeDiagnostics.Test.Framework
{
    internal sealed class FileLocation : IEquatable<FileLocation>, IComparable<FileLocation>
    {
        public string File { get; }

        /// <summary>
        /// 1-based line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// 1-based column number.
        /// </summary>
        public int Column { get; }

        public FileLocation(string file, int line, int column)
        {
            File = file;
            Line = line;
            Column = column;
        }

        public static FileLocation FromLocation(Location location)
        {
            var span = location.GetLineSpan();
            return new FileLocation(span.Path, span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileLocation);
        }

        public bool Equals(FileLocation other)
        {
            return other != null &&
                   other.File == File &&
                   other.Line == Line &&
                   other.Column == Column;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + File.GetHashCode();
            hash = hash * 31 + Line.GetHashCode();
            hash = hash * 31 + Column.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1},{2}", File, Line, Column);
        }

        public int CompareTo(FileLocation other)
        {
            int ret = string.CompareOrdinal(File, other.File);
            if (ret != 0)
            {
                return ret;
            }
            ret = Line.CompareTo(other.Line);
            if (ret != 0)
            {
                return ret;
            }
            return Column.CompareTo(other.Column);
        }
    }
}
