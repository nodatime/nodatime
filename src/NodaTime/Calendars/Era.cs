// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Represents an era used in a calendar.
    /// </summary>
    /// <remarks>All the built-in calendars in Noda Time use the values specified by the static
    /// read-only fields in this class. These may be compared for reference equality to check for specific
    /// eras.</remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class Era
    {
        /// <summary>
        /// The "Common" era (CE), also known as Anno Domini (AD). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static readonly Era Common = new Era("CE", "Eras_Common"); // CE

        /// <summary>
        /// The "before common" era (BCE), also known as Before Christ (BC). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static readonly Era BeforeCommon = new Era("BCE", "Eras_BeforeCommon"); // BCE

        /// <summary>
        /// The "Anno Martyrum" or "Era of the Martyrs". This is the sole era used in the Coptic calendar.
        /// </summary>
        public static readonly Era AnnoMartyrm = new Era("AM", "Eras_AnnoMartyrum"); // AM

        /// <summary>
        /// The "Anno Hegira" era. This is the sole era used in the Hijri (Islamic) calendar.
        /// </summary>
        public static readonly Era AnnoHegirae = new Era("EH", "Eras_AnnoHegirae"); // AH

        private readonly string name;
        private readonly string resourceIdentifier;

        internal string ResourceIdentifier { get { return resourceIdentifier; } }

        internal Era(string name, string resourceIdentifier)
        {
            this.name = name;
            this.resourceIdentifier = resourceIdentifier;
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
