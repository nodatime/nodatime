#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using NodaTime.Text;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="Instant"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
    /// </summary>
    public class NodaInstantConverter : NodaPatternConverter<Instant>
    {
        // TODO(Post-V1): We should really expose things like this as properties on InstantPattern.
        private static readonly InstantPattern InstantPattern = InstantPattern.CreateWithInvariantInfo("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'");

        public NodaInstantConverter() : base(InstantPattern)
        {            
        }
    }
}
