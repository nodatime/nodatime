// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;

namespace NodaTime.Test.Globalization
{
    /// <summary>
    /// Throws an exception if called. This forces the testing code set or pass a valid culture in all
    /// tests. The tests cannot be guaranteed to work if the culture is not set as formatting and parsing
    /// are culture dependent.
    /// </summary>
    public class FailingCultureInfo : CultureInfo
    {
        private const string CultureNotSet = "The formatting and parsing code tests should have set the correct culture.";

        public static FailingCultureInfo Instance = new FailingCultureInfo();

        public FailingCultureInfo()
            : base("en-US")
        {
        }

        public override DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                throw new NotSupportedException(CultureNotSet);
            }
            set
            {
                throw new NotSupportedException(CultureNotSet);
            }
        }

        public override NumberFormatInfo NumberFormat
        {
            get
            {
                throw new NotSupportedException(CultureNotSet);
            }
            set
            {
                throw new NotSupportedException(CultureNotSet);
            }
        }

        public override string Name
        {
            get
            {
                return "Failing";
            }
        }

        public override object GetFormat(Type formatType)
        {
            throw new NotSupportedException(CultureNotSet);
        }
    }
}
