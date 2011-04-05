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
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    public sealed class ParseFailureInfo
    {
        public ParseFailureInfo()
        {
            Failure = ParseFailureKind.None;
        }

        internal ParseFailureInfo(ParseFailureKind failure, object[] parameters)
        {
            Failure = failure;
            FailureMessageParameters = parameters;
        }

        internal ParseFailureInfo(string argumentName)
        {
            Failure = ParseFailureKind.ArgumentNull;
            FailureArgumentName = argumentName;
            FailureMessageParameters = new object[0];
        }

        private ParseFailureKind Failure { get; set; }
        private string FailureArgumentName { get; set; }
        private object[] FailureMessageParameters { get; set; }

        internal void Validate(ParseInfo parseInfo)
        {
            Assert.AreEqual(Failure, parseInfo.Failure, "Failure kind mismatch");
            if (Failure != ParseFailureKind.None)
            {
                Assert.AreEqual(FailureMessageParameters, parseInfo.FailureMessageParameters, "Failure message parameters mismatch");
                if (FailureArgumentName == null)
                {
                    Assert.IsNull(parseInfo.FailureArgumentName, "failure argument name should be null");
                }
                else
                {
                    Assert.AreEqual(FailureArgumentName, parseInfo.FailureArgumentName, "Failure argument name mismatch");
                }
            }
        }
    }
}