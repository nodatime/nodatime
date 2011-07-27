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
    internal class ParseString : Parsable
    {
        internal ParseString(string value)
            : base(value)
        {
        }


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
            if (string.CompareOrdinal(Value, Index, match, 0, match.Length) == 0)
            {
                Move(Index + match.Length);
                return true;
            }
            return false;
        }

        internal bool ParseDigits(int minimumDigits, int maximumDigits, out int result)
        {
            result = 0;
            int startIndex = Index;
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
            return (int)char.GetNumericValue(Current);
        }

        private bool IsDigit()
        {
            return char.IsNumber(Current);
        }
    }
}