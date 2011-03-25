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

namespace NodaTime.Format
{
    internal class ParseString
    {
        private readonly string value;
        private int length;
        private int index;

        internal ParseString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            value = s;
            length = s.Length;
            Move(-1);
        }

        public char Current { get; private set; }

        internal bool Match(char character)
        {
            if (Current == character)
            {
                MoveNext();
                return true;
            }
            return false;
        }

        internal bool Match(string match)
        {
            if (string.CompareOrdinal(value, index, match, 0, match.Length) == 0)
            {
                Move(index + match.Length);
                return true;
            }
            return false;
        }

        internal bool MoveNext()
        {
            return Move(index + 1);
        }

        internal bool ParseDigits(int minimumDigits, int maximumDigits, out int result)
        {
            result = 0;
            int startIndex = index;
            int count = 0;
            while (count < maximumDigits)
            {
                if (!IsDigit())
                {
                    MovePrevious();
                    break;
                }
                result = (result * 10) + GetDigit();
                count++;
                if (!MoveNext())
                {
                    break;
                }
            }
            if (count < minimumDigits)
            {
                Move(startIndex);
                return false;
            }
            return true;
        }

        private int GetDigit()
        {
            return (int)Char.GetNumericValue(Current);
        }

        private bool IsDigit()
        {
            return Char.IsNumber(Current);
        }

        private bool Move(int targetIndex)
        {
            var outOfRange = targetIndex < 0 || targetIndex >= length;
            index = targetIndex;
            Current = outOfRange ? '\u0000' : value[index];
            return !outOfRange;
        }

        private bool MovePrevious()
        {
            return Move(index - 1);
        }

        internal void TrimTail()
        {
            while (length > 0 && Char.IsWhiteSpace(value[length - 1]))
            {
                length--;
            }
        }
    }
}