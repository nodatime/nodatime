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

using System;
using System.Diagnostics;
using System.Text;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides a cursor over text being parsed. None of the methods in this class throw exceptions (unless
    /// there is a bug in Noda Time, in which case an exception is appropriate) and none of the methods
    /// have ref parameters indicating failures, unlike subclasses. This class is used as the basis for both
    /// value and pattern parsing, so can make no judgement about what's wrong (i.e. it wouldn't know what
    /// type of failure to indicate). Instead, methods return Boolean values to indicate success or failure.
    /// </summary>
    [DebuggerStepThrough]
    internal abstract class TextCursor
    {
        /// <summary>
        /// A nul character. This character is not allowed in any parsable string and is used to
        /// indicate that the current character is not set.
        /// </summary>
        internal const char Nul = '\0';

        /// <summary>
        /// Initializes a new instance to parse the given value.
        /// </summary>
        protected TextCursor(string value)
        {
            // Validated by caller.
            Value = value;
            Length = value.Length;
            Move(-1);
        }

        /// <summary>
        /// Gets the current character.
        /// </summary>
        internal char Current { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has more characters.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has more characters; otherwise, <c>false</c>.
        /// </value>
        internal bool HasMoreCharacters { get { return Index + 1 < Length; } }

        /// <summary>
        /// Gets the current index into the string being parsed.
        /// </summary>
        internal int Index { get; private set; }

        /// <summary>
        /// Gets the length of the string being parsed.
        /// </summary>
        internal int Length { get; private set; }

        /// <summary>
        /// Gets the string being parsed.
        /// </summary>
        internal string Value { get; private set; }

        /// <summary>
        /// Gets the remainder the string that has not been parsed yet.
        /// </summary>
        internal string Remainder { get { return Value.Substring(Index); } }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Index < 0)
            {
                builder.Append("<<>>");
                builder.Append(Value);
            }
            else if (Index >= Length)
            {
                builder.Append(Value);
                builder.Append("<<>>");
            }
            else
            {
                builder.Append(Value.Substring(0, Index));
                builder.Append("<<");
                builder.Append(Current);
                builder.Append(">>");
                if (Index < Length - 1)
                {
                    builder.Append(Value.Substring(Index + 1, Length - Index - 1));
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the next character if there is one or <see cref="Nul" /> if there isn't.
        /// </summary>
        /// <returns></returns>
        internal char PeekNext()
        {
            return HasMoreCharacters ? Value[Index + 1] : Nul;
        }

        /// <summary>
        /// Moves the specified target index. If the new index is out of range of the valid indicies
        /// for this string then the index is set to the beginning or the end of the string whichever
        /// is nearest the requested index.
        /// </summary>
        /// <param name="targetIndex">Index of the target.</param>
        /// <returns><c>true</c> if the requested index is in range.</returns>
        internal bool Move(int targetIndex)
        {
            if (targetIndex >= 0)
            {
                if (targetIndex < Length)
                {
                    Index = targetIndex;
                    Current = Value[Index];
                    return true;
                }
                else
                {
                    Current = Nul;
                    Index = Length;
                    return false;
                }
            }
            Current = Nul;
            Index = -1;
            return false;
        }

        /// <summary>
        /// Moves to the next character.
        /// </summary>
        /// <returns><c>true</c> if the requested index is in range.</returns>
        internal bool MoveNext()
        {
            // Logically this is Move(Index + 1), but it's micro-optimized as we
            // know we'll never hit the lower limit this way.
            int targetIndex = Index + 1;
            if (targetIndex < Length)
            {
                Index = targetIndex;
                Current = Value[Index];
                return true;
            }
            Current = Nul;
            Index = Length;
            return false;
        }

        /// <summary>
        /// Moves to the previous character.
        /// </summary>
        /// <returns><c>true</c> if the requested index is in range.</returns>
        internal bool MovePrevious()
        {
            // Logically this is Move(Index - 1), but it's micro-optimized as we
            // know we'll never hit the upper limit this way.
            if (Index > 0)
            {
                Index--;
                Current = Value[Index];
                return true;
            }
            Current = Nul;
            Index = -1;
            return false;
        }
    }
}
