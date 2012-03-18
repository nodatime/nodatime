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
    /// Converts a <see cref="LocalTime"/> to and from the ISO 8601 time format without a timezone specified (e.g. 12:53).
    /// </summary>
    /// <summary>
    /// Simple converter for types represented by a single string in JSON, which can be
    /// parsed and formatted with a Noda Time pattern.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public class NodaLocalTimeConverter : NodaPatternConverter<LocalTime>
    {
        private static readonly LocalTimePattern LocalTimePattern = LocalTimePattern.CreateWithInvariantInfo("HH':'mm':'ss.FFFFFFF");

        public NodaLocalTimeConverter() : base(LocalTimePattern)
        {
        }
    }
}
