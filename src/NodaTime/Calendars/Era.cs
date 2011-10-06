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

using System.Collections.Generic;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Represents an era in a calendar.
    /// </summary>
    /// <remarks>All the built-in calendars in Noda Time use the values specified by the static
    /// read-only fields in this class. These may be compared for reference equality to check for specific
    /// eras.</remarks>
    public sealed class Era
    {
        /// <summary>
        /// The "Common" era (CE), also known as Anno Domini (AD). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static readonly Era Common = new Era("CE"); // CE

        /// <summary>
        /// The "before common" era (BCE), also known as Before Christ (BC). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static readonly Era BeforeCommon = new Era("BCE"); // BCE

        /// <summary>
        /// The 'Anno Martyrum' or 'Era of the Martyrs'. This is the sole era in the Coptic calendar.
        /// </summary>
        public static readonly Era AnnoMartyrm = new Era("AM"); // AM

        /// <summary>
        /// Sole era used by the Buddhist calendar.
        /// </summary>
        public static readonly Era Buddhist = new Era("BE"); // BE

        /// <summary>
        /// The "Anno Hegira" era. This is the sole era used by the Hijri (Islamic) calendar.
        /// </summary>
        public static readonly Era AnnoHegirae = new Era("EH"); // AH

        private readonly string name;
        private readonly IList<string> resourceIdentifiers;

        internal IList<string> ResourceIdentifiers { get { return resourceIdentifiers; } }

        private Era(string name, params string[] resourceIdentifiers)
        {
            this.name = name;
            this.resourceIdentifiers = new List<string>(resourceIdentifiers).AsReadOnly();
        }

        /// <summary>
        /// Returns the name of this era, e.g. "CE" or "BCE".
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Returns the name of this era.
        /// </summary>
        /// <returns>The name of this era.</returns>
        public override string ToString()
        {
            return name;
        }
    }
}
