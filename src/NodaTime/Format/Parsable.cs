using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Properties;

namespace NodaTime.Format
{
    internal abstract class Parsable
    {
        internal const char NUL = '\u0000';

        internal string Value { get; private set; }
        internal int Length { get; private set; }
        internal int Index { get; private set; }
        internal char Current { get; private set; }

        protected Parsable(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value == string.Empty)
            {
                throw new ArgumentException(@"string is empty", "value");
            }
            Value = value;
            Length = value.Length;
            Index = 0;
        }

        internal bool MoveNext()
        {
            return Move(Index + 1);
        }

        internal bool MovePrevious()
        {
            return Move(Index - 1);
        }

        internal bool MoveCurrent()
        {
            return Move(Index);
        }

        internal bool Move(int targetIndex)
        {
            var inRange = 0 <= targetIndex && targetIndex < Length;
            Index = inRange ? targetIndex : Math.Min(-1, Math.Max(Length, targetIndex));
            Current = inRange ? Value[Index] : NUL;
            return inRange;
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

        internal void SkipWhiteSpaces()
        {
            while (Current != NUL && char.IsWhiteSpace(Current))
            {
                MoveNext();
            }
        }

        internal void TrimTail()
        {
            while (Length > 0 && char.IsWhiteSpace(Value[Length - 1]))
            {
                Length--;
            }
            Value = Value.Substring(0, Length);
            Length = Value.Length;
            MoveCurrent();
        }

        internal void RemoveLeadingInQuoteSpaces()
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
                        Value = Value.Remove(1, Index);
                        Length = Value.Length;
                    }
                }
            }
            Move(-1);
        }

        internal void RemoveTrailingInQuoteSpaces()
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
    }
}
