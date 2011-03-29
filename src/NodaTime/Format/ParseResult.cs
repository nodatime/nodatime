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
using NodaTime.Utility;

namespace NodaTime.Format
{
    internal abstract class ParseResult
    {
        internal ParseResult()
        {
            Failure = ParseFailureKind.None;
        }

        private ParseFailureKind Failure { get; set; }
        private string FailureArgumentName { get; set; }
        private object FailureMessageFormatArgument { get; set; }
        private string FailureMessageId { get; set; }
        internal bool Failed { get { return Failure != ParseFailureKind.None; } }

        internal Exception GetParseException()
        {
            switch (Failure)
            {
                case ParseFailureKind.ArgumentNull:
                    return new ArgumentNullException(FailureArgumentName, ResourceHelper.GetMessage(FailureMessageId));
                case ParseFailureKind.Format:
                    return new FormatException(ResourceHelper.GetMessage(FailureMessageId));
                case ParseFailureKind.FormatWithParameter:
                    return new FormatException(ResourceHelper.GetMessage(FailureMessageId, FailureMessageFormatArgument));
            }
            return null;
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId)
        {
            return SetFailure(failure, failureMessageId, null, null);
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId, object failureMessageFormatArgument)
        {
            return SetFailure(failure, failureMessageId, failureMessageFormatArgument, null);
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId, object failureMessageFormatArgument, string failureArgumentName)
        {
            Failure = failure;
            FailureMessageId = failureMessageId;
            FailureMessageFormatArgument = failureMessageFormatArgument;
            FailureArgumentName = failureArgumentName;
            ProcessFailure();
            return false;
        }

        /// <summary>
        /// Called after the failure has been set into this object to allow for sub-classes to
        /// do any post-processing.
        /// </summary>
        protected virtual void ProcessFailure()
        {
            // Do nothing
        }
    }

    #region Nested type: ParseFailureKind
    internal enum ParseFailureKind
    {
        None,
        ArgumentNull,
        Format,
        FormatWithParameter
    }
    #endregion
}