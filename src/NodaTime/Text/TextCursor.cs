// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
        private readonly string value;
        private readonly int length;

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
            this.value = value;
            this.length = value.Length;
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
        internal bool HasMoreCharacters { get { unchecked { return Index + 1 < Length; } } }

        /// <summary>
        /// Gets the current index into the string being parsed.
        /// </summary>
        internal int Index { get; private set; }

        /// <summary>
        /// Gets the length of the string being parsed.
        /// </summary>
        internal int Length { get { return length; } }

        /// <summary>
        /// Gets the string being parsed.
        /// </summary>
        internal string Value { get { return value; } }

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
            unchecked
            {
                return HasMoreCharacters ? Value[Index + 1] : Nul;                
            }
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
            unchecked
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
        }

        /// <summary>
        /// Moves to the next character.
        /// </summary>
        /// <returns><c>true</c> if the requested index is in range.</returns>
        internal bool MoveNext()
        {
            unchecked
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
        }

        /// <summary>
        /// Moves to the previous character.
        /// </summary>
        /// <returns><c>true</c> if the requested index is in range.</returns>
        internal bool MovePrevious()
        {
            unchecked
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
}
