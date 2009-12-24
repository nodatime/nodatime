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
using NUnit.Framework;
using NodaTime.Format;
using System.Text;
using System.IO;
using NodaTime.Periods;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {
        public class PeriodPrinterMock : IPeriodPrinter
        {
            public bool CalculatePrintedLengthCalled;
            public IPeriod CalculatePrintedLengthPeriodArgument;
            public IFormatProvider CalculatePrintedLengthProviderArgument;

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                CalculatePrintedLengthCalled = true;
                CalculatePrintedLengthPeriodArgument = period;
                CalculatePrintedLengthProviderArgument = provider;
                return 42;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public bool PrintToBuilderCalled;
            public StringBuilder PrintToBuilderArgument;
            public IPeriod PrintToBuilderToPeriodArgument;
            public IFormatProvider PrintToBuilderProviderArgument;
            
            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                PrintToBuilderCalled = true;
                PrintToBuilderArgument = stringBuilder;
                PrintToBuilderToPeriodArgument = period;
                PrintToBuilderProviderArgument = provider;
            }

            public bool PrintToWriterCalled;
            public TextWriter PrintToWriterArgument;
            public IPeriod PrintToWriterPeriodArgument;
            public IFormatProvider PrintToWriterProviderArgument;

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                PrintToWriterCalled = true;
                PrintToWriterArgument = textWriter;
                PrintToWriterPeriodArgument = period;
                PrintToWriterProviderArgument = provider;
            }

        }

        public class PeriodParserMock : IPeriodParser
        {

            public bool ParseIntoCalled;
            public string ParseIntoStringArgument;
            public int ParseIntoPositionArgument;
            public IFormatProvider ParseIntoProviderArgument;
            public int ParseIntoPositionToReturn = 42;

            public int Parse(string periodString, int position, PeriodBuilder builder, IFormatProvider provider)
            {
                ParseIntoCalled = true;
                ParseIntoStringArgument = periodString;
                ParseIntoPositionArgument = position;
                ParseIntoProviderArgument = provider;
                return ParseIntoPositionToReturn;
            }

        }

    }
}
