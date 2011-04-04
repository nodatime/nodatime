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
using System.Diagnostics;
using NodaTime.Properties;
using System.Text;
#endregion

namespace NodaTime.Format
{
    [DebuggerStepThrough]
    internal abstract class Parsable
    {
        internal const char Nul = '\u0000';

        protected Parsable(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value == string.Empty)
            {
                throw new ArgumentException(Resources.Noda_StringEmpty, "value");
            }
            Value = value;
            Length = value.Length;
            Move(-1);
        }

        internal char Current { get; private set; }
        internal bool HasMoreCharacters { get { return Index + 1 < Length; } }
        internal int Index { get; private set; }
        internal int Length { get; private set; }
        internal string Value { get; private set; }
        internal string Remainder { get { return Value.Substring(Index); } }

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
        ///   Gets the next character.
        /// </summary>
        /// <returns>The next character from the string.</returns>
        /// <exception cref = "FormatException">if there are no more characters.</exception>
        internal char GetNextCharacter()
        {
            if (MoveNext())
            {
                return Current;
            }
            throw new FormatException(Resources.Format_InvalidString);
        }

        internal char PeekNext()
        {
            return HasMoreCharacters ? Value[Index + 1] : Nul;
        }

        internal bool Move(int targetIndex)
        {
            var inRange = 0 <= targetIndex && targetIndex < Length;
            Index = inRange ? targetIndex : Math.Max(-1, Math.Min(Length, targetIndex));
            Current = inRange ? Value[Index] : Nul;
            return inRange;
        }

        internal bool MoveCurrent()
        {
            return Move(Index);
        }

        internal bool MoveNext()
        {
            return Move(Index + 1);
        }

        internal bool MovePrevious()
        {
            return Move(Index - 1);
        }

        internal bool SkipWhiteSpaces()
        {
            while (Current != Nul && char.IsWhiteSpace(Current))
            {
                MoveNext();
            }
            return Current != Nul;
        }

        internal void TrimLeadingInQuoteSpaces()
        {
            if (Length > 2)
            {
                Move(0);
                if (Current == '\'' || Current == '"')
                {
                    while (MoveNext() && char.IsWhiteSpace(Current))
                    {
                    }
                    if (Index > 1)
                    {
                        Value = Value.Remove(1, Index - 1);
                        Length = Value.Length;
                    }
                }
            }
            Move(-1);
        }

        internal void TrimLeadingWhiteSpaces()
        {
            Move(0);
            while (Current != Nul && char.IsWhiteSpace(Current))
            {
                MoveNext();
            }
            if (Index > 0)
            {
                Value = Value.Substring(Index);
                Length = Value.Length;
            }
            Move(-1);
        }

        internal void TrimTrailingInQuoteSpaces()
        {
            if (Length > 2)
            {
                Move(Length - 1);
                while (MovePrevious() && char.IsWhiteSpace(Current))
                {
                }
                Value = Value.Remove(Index + 1, (Length - 2) - Index);
                Length = Value.Length;
            }
            Move(-1);
        }

        internal void TrimTrailingWhiteSpaces()
        {
            while (Length > 0 && char.IsWhiteSpace(Value[Length - 1]))
            {
                Length--;
            }
            Value = Value.Substring(0, Length);
            Length = Value.Length;
            Move(-1);
        }
    }
}