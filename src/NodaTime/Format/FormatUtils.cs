#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    /// <summary>
    /// Utility methods used by formatters.
    /// FormatUtils is thread-safe and immutable.
    /// </summary>
    internal static class FormatUtils
    {
        private const int LENGTH_OF_SAMPLE_TEXT = 32;
        private const int LENGTH_OF_DOTS = 3;
        private const string DOTS = "...";

        internal static string CreateErrorMessage(string text, int errorPosition)
        {
            int sampleLen = errorPosition + LENGTH_OF_SAMPLE_TEXT;
            String sampleText;
            if (text.Length <= sampleLen + LENGTH_OF_DOTS)
                sampleText = text;
            else
                sampleText = text.Substring(0, sampleLen) + DOTS;

            if(errorPosition <=0)
                return "Invalid format: \"" + sampleText + '"';

            if (errorPosition >= text.Length)
                return "Invalid format: \"" + sampleText + "\" is too short";

            return "Invalid format: \"" + sampleText + "\" is malformed at \"" +
                sampleText.Substring(errorPosition) + '"';
        }
    }
}
