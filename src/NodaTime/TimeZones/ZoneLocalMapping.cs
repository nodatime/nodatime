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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Complete information about the mapping from a local date and time within a time zone. This is usually
    /// obtained from a call to <see cref="DateTimeZone.MapLocalDateTime"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result will either be unambiguous (a single ZonedDateTime), ambiguous (two ZonedDateTime values
    /// which both map to the same LocalDateTime due to the clocks going back at a daylight saving
    /// transition) or a gap (the given LocalDateTime was skipped in this time zone due to the clocks going
    /// forward at a daylight saving transition).
    /// </para>
    /// <para>
    /// This type is effectively a discriminated union of the three result types. If you attempt to use a
    /// property corresponding to a different result type, it will throw InvalidOperationException.
    /// </para>
    /// </remarks>
    public abstract class ZoneLocalMapping
    {
        // TODO(Post-V1): Include zone and LocalDateTime information for completeness?

        // TODO(Post-V1): Just have the Transition for the gap instead of the two ZoneIntervals? Significantly
        // less information that way.

        /// <summary>
        /// The type of result represented by this mapping. The underlying integer value is the same
        /// as the number of ZonedDateTime values which map to the original LocalDateTime.
        /// </summary>
        public enum ResultType
        {
            /// <summary>
            /// The original LocalDateTime was skipped in this time zone due to the clocks going
            /// forward at a daylight saving transition. Use the ZoneIntervalBeforeTransition and
            /// ZoneIntervalAfterTransition properties to find out more about the gap.
            /// </summary>
            Skipped = 0,

            /// <summary>
            /// The original LocalDateTime was unambiguous. Use the UnambiguousMapping property
            /// to obtain the corresponding ZonedDateTime.
            /// </summary>
            Unambiguous = 1,

            /// <summary>
            /// The original LocalDateTime was ambiguous due to the clocks going back at a daylight saving
            /// transition. Use the EarlierMapping and LaterMapping properties to obtain the corresponding
            /// ZonedDateTime values.
            /// </summary>
            Ambiguous = 2
        }

        /// <summary>
        /// Private constructor, called by nested subclasses. Note that this means that the *only*
        /// implementations can be the nested types here.
        /// </summary>
        private ZoneLocalMapping(ResultType type)
        {
            this.type = type;
        }

        // TODO(Post-V1): Determine whether it's better for this to be a variable with a non-virtual property,
        // or a virtual property overridden in every subclass.
        private readonly ResultType type;

        /// <summary>
        /// Returns the type of this result, which will always be one of the values defined in the
        /// <see cref="ResultType"/> enum.
        /// </summary>
        public ResultType Type { get { return type; } }

        /// <summary>
        /// In an unambiguous mapping, returns the sole ZonedDateTime which maps to the original LocalDateTime.
        /// </summary>
        /// <exception cref="InvalidOperationException">The mapping isn't unambiguous.</exception>
        public virtual ZonedDateTime UnambiguousMapping { get { throw new InvalidOperationException("UnambiguousMapping property should not be called on a result of type " + type); } }

        /// <summary>
        /// In an ambiguous mapping, returns the earlier of the two ZonedDateTimes which map to the original LocalDateTime.
        /// </summary>
        /// <exception cref="InvalidOperationException">The mapping isn't ambiguous.</exception>
        public virtual ZonedDateTime EarlierMapping { get { throw new InvalidOperationException("EarlierMapping property should not be called on a result of type " + type); } }

        /// <summary>
        /// In an ambiguous mapping, returns the later of the two ZonedDateTimes which map to the original LocalDateTime.
        /// </summary>
        /// <exception cref="InvalidOperationException">The mapping isn't ambiguous.</exception>
        public virtual ZonedDateTime LaterMapping { get { throw new InvalidOperationException("LaterMapping property should not be called on a result of type " + type); } }

        /// <summary>
        /// In a mapping where the original LocalDateTime value is skipped in the time zone,
        /// returns the time zone interval from before the daylight saving transition.
        /// </summary>
        /// <exception cref="InvalidOperationException">The mapping doesn't skip the original LocalDateTime.</exception>
        public virtual ZoneInterval ZoneIntervalBeforeTransition { get { throw new InvalidOperationException("ZoneIntervalBeforeGap property should not be called on a result of type " + type); } }

        /// <summary>
        /// In a mapping where the original LocalDateTime value is skipped in the time zone,
        /// returns the time zone interval from after the daylight saving transition.
        /// </summary>
        /// <exception cref="InvalidOperationException">The mapping doesn't skip the original LocalDateTime.</exception>
        public virtual ZoneInterval ZoneIntervalAfterTransition { get { throw new InvalidOperationException("ZoneIntervalAfterGap property should not be called on a result of type " + type); } }

        private class SkippedMappingResult : ZoneLocalMapping
        {
            private readonly ZoneInterval beforeTransition;
            private readonly ZoneInterval afterTransition;

            internal SkippedMappingResult(ZoneInterval beforeTransition, ZoneInterval afterTransition) : base(ResultType.Skipped)
            {
                this.beforeTransition = beforeTransition;
                this.afterTransition = afterTransition;
            }

            public override ZoneInterval ZoneIntervalBeforeTransition { get { return beforeTransition; } }
            public override ZoneInterval ZoneIntervalAfterTransition { get { return afterTransition; } }
        }

        private class UnambiguousMappingResult : ZoneLocalMapping
        {
            private readonly ZonedDateTime unambiguousMapping;

            internal UnambiguousMappingResult(ZonedDateTime unambiguousMapping) : base(ResultType.Unambiguous)
            {
                this.unambiguousMapping = unambiguousMapping;
            }

            public override ZonedDateTime UnambiguousMapping { get { return unambiguousMapping; } }
        }

        private class AmbiguousMappingResult : ZoneLocalMapping
        {
            private readonly ZonedDateTime earlier;
            private readonly ZonedDateTime later;

            internal AmbiguousMappingResult(ZonedDateTime earlier, ZonedDateTime later) : base(ResultType.Ambiguous)
            {
                this.earlier = earlier;
                this.later = later;
            }

            public override ZonedDateTime EarlierMapping { get { return earlier; } }
            public override ZonedDateTime LaterMapping { get { return later; } }
        }

        internal static ZoneLocalMapping SkippedResult(ZoneInterval beforeTransition, ZoneInterval afterTransition)
        {
            return new SkippedMappingResult(beforeTransition, afterTransition);
        }

        internal static ZoneLocalMapping UnambiguousResult(ZonedDateTime value)
        {
            return new UnambiguousMappingResult(value);
        }

        internal static ZoneLocalMapping AmbiguousResult(ZonedDateTime earlier, ZonedDateTime later)
        {
            return new AmbiguousMappingResult(earlier, later);
        }
    }
}
