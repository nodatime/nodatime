using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    /// Exception thrown by IParser[T], providing information about what failed to parse.
    /// This provides rich diagnostics, although the details are designed to be helpful to developers
    /// rather than for flow control.
    /// TODO: Is this actually better than just having a big error message?
    /// </summary>
    public class ParseException : FormatException
    {
        // TODO: Make this a top level class?
        public enum FailureType
        {
            /// <summary>
            /// The input was not in the appropriate pattern, e.g. a value of "2010-06-19"
            /// against a pattern of "yyyy/MM/dd".
            /// </summary>
            PatternNotMatched,
            /// <summary>
            /// We tried to parse a number, but couldn't, e.g. "2010-bad-19"
            /// </summary>
            NumberParsingFailed,
            /// <summary>
            /// We found an identifier, but it wasn't in the range of expected values -
            /// e.g. a month name or a time zone identifier.
            /// </summary>
            UnrecognizedIdentifier
        }

        // This is only a quick sketch. Needs more detail.
        
        /// <summary>
        /// Overall text which could not be parsed.
        /// </summary>
        public string CompleteText { get; private set; }

        /// <summary>
        /// Which section of the text could not be parsed (e.g. in 2010-wibble-10 this might be "wibble")
        /// </summary>
        public string FailedSectionText { get; private set; }

        /// <summary>
        /// The name of the type of field we were attempting to find, e.g. "Month" or "Hour" - or
        /// possibly "Pattern" if we were looking for a fixed piece of text from the pattern.
        /// </summary>
        public string FailedFieldName { get; private set; }

        /// <summary>
        /// The nature of the failure.
        /// </summary>
        public FailureType Cause { get; private set; }
    }
}
