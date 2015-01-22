// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Represents an era used in a calendar.
    /// </summary>
    /// <remarks>All the built-in calendars in Noda Time use the values specified by the static
    /// read-only fields in this class. These may be compared for reference equality to check for specific
    /// eras.</remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class Era
    {
        /// <summary>
        /// The "Common" era (CE), also known as Anno Domini (AD). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static Era Common { get; } = new Era("CE", "Eras_Common");

        /// <summary>
        /// The "before common" era (BCE), also known as Before Christ (BC). This is used in the ISO, Gregorian and Julian calendars.
        /// </summary>
        public static Era BeforeCommon { get; } = new Era("BCE", "Eras_BeforeCommon");

        /// <summary>
        /// The "Anno Martyrum" or "Era of the Martyrs". This is the sole era used in the Coptic calendar.
        /// </summary>
        public static Era AnnoMartyrum { get; } = new Era("AM", "Eras_AnnoMartyrum");

        /// <summary>
        /// The "Anno Hegira" era. This is the sole era used in the Hijri (Islamic) calendar.
        /// </summary>
        public static Era AnnoHegirae { get; } = new Era("EH", "Eras_AnnoHegirae");

        /// <summary>
        /// The "Anno Mundi" era. This is the sole era used in the Hebrew calendar.
        /// </summary>
        public static Era AnnoMundi { get; } = new Era("AM", "Eras_AnnoMundi");

        /// <summary>
        /// The "Anno Persico" era. This is the sole era used in the Persian calendar.
        /// </summary>
        public static Era AnnoPersico { get; } = new Era("AP", "Eras_AnnoPersico");

        internal string ResourceIdentifier { get; }

        /// <summary>
        /// Returns the name of this era, e.g. "CE" or "BCE".
        /// </summary>
        /// <value>The name of this era.</value>
        public string Name { get; }

        internal Era(string name, string resourceIdentifier)
        {
            this.Name = name;
            this.ResourceIdentifier = resourceIdentifier;
        }

        /// <summary>
        /// Returns the name of this era.
        /// </summary>
        /// <returns>The name of this era.</returns>
        public override string ToString() => Name;
    }
}
