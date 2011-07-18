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

using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A transition resolver represents a strategy for handling local date/times presented for
    /// conversion to zoned date/times (or similar) when the local date/time doesn't represent
    /// a single instant in the target time zone. This happens in two different ways in the course
    /// of a time zone transition. When the wall clock goes forward, some local times are skipped (they
    /// don't occur at all). When the wall clock goes backward, some local times are ambiguous (they
    /// occur twice).
    /// </summary>
    public sealed class TransitionResolver
    {
        /// <summary>
        /// Delegate signature matching both methods - handy as a way of keeping all the behaviour in a single class.
        /// </summary>
        private delegate Instant Resolver(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone);

        /// <summary>
        /// Strategy to use when resolving ambiguities.
        /// </summary>
        public enum AmbiguityStrategy
        {
            /// <summary>
            /// Rejects all ambiguous times as invalid by throwing a AmbiguousTimeException.
            /// </summary>
            Strict = 0,
            /// <summary>
            /// Returns the earlier of the two possible instants which translate to the given local instant.
            /// </summary>
            Earlier = 1,
            /// <summary>
            /// Returns the later of the two possible instants which translate to the given local instant.
            /// </summary>
            Later = 2
        }

        /// <summary>
        /// Strategy to use when resolving gaps.
        /// </summary>
        public enum GapStrategy
        {
            /// <summary>
            /// Rejects all gaps as invalid by throwing a SkippedTimeException.
            /// </summary>
            Strict = 0,
            /// <summary>
            /// Returns an instant one tick before the transition, which will result in a local instant in the
            /// earlier interval.
            /// </summary>
            EndOfEarlyInterval = 1,
            /// <summary>
            /// Returns an instant at the transition, which will result in a local instant as per the later interval.
            /// </summary>
            StartOfLateInterval = 2,
            /// <summary>
            /// Returns an instant which is adjusted forward by the length of the gap. For example, resolving 1.20am in a
            /// gap from 1am to 2am will result in 2.20am.
            /// </summary>
            PushForward = 3,
            /// <summary>
            /// Returns an instant which is adjusted backward by the length of the gap. For example, resolving 1.20am in a
            /// gap from 1am to 2am will result in 12.20am.
            /// </summary>
            PushBackward = 4
        }

        private readonly Resolver ambiguityResolver;
        private readonly Resolver gapResolver;

        private TransitionResolver(Resolver ambiguityResolver, Resolver gapResolver)
        {
            this.ambiguityResolver = ambiguityResolver;
            this.gapResolver = gapResolver;
        }

        /// <summary>
        /// Determines the instant to treat the specified local instant as when there are two
        /// possible intervals involved.
        /// </summary>
        internal Instant ResolveAmbiguity(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return ambiguityResolver(intervalBefore, intervalAfter, localInstant, zone);
        }

        /// <summary>
        /// Determines the instant to treat the specified local instant as when it falls in a gap.
        /// </summary>
        internal Instant ResolveGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return gapResolver(intervalBefore, intervalAfter, localInstant, zone);
        }

        /// <summary>
        /// Creates a transition resolver from the given strategies.
        /// </summary>
        public static TransitionResolver FromStrategies(AmbiguityStrategy ambiguityStrategy, GapStrategy gapStrategy)
        {
            Resolver ambiguityResolver;
            switch (ambiguityStrategy)
            {
                case AmbiguityStrategy.Strict:
                    ambiguityResolver = StrictAmbiguity;
                    break;
                case AmbiguityStrategy.Earlier:
                    ambiguityResolver = EarlyAmbiguity;
                    break;
                case AmbiguityStrategy.Later:
                    ambiguityResolver = LateAmbiguity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ambiguityStrategy", "Invalid ambiguity strategy: " + ambiguityStrategy);
            }
            Resolver gapResolver;
            switch (gapStrategy)
            {
                case GapStrategy.Strict:
                    gapResolver = StrictGap;
                    break;
                case GapStrategy.EndOfEarlyInterval:
                    gapResolver = EndOfEarlyIntervalGap;
                    break;
                case GapStrategy.StartOfLateInterval:
                    gapResolver = StartOfLateIntervalGap;
                    break;
                case GapStrategy.PushBackward:
                    gapResolver = PushBackwardGap;
                    break;
                case GapStrategy.PushForward:
                    gapResolver = PushForwardGap;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gapStrategy", "Invalid gap strategy: " + gapStrategy);
            }
            return new TransitionResolver(ambiguityResolver, gapResolver);
        }

        private static Instant StrictAmbiguity(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            throw new AmbiguousTimeException(localInstant, zone);
        }

        private static Instant EarlyAmbiguity(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return localInstant.Minus(intervalBefore.Offset);
        }

        private static Instant LateAmbiguity(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return localInstant.Minus(intervalAfter.Offset);
        }

        private static Instant StrictGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            throw new SkippedTimeException(localInstant, zone);
        }

        private static Instant EndOfEarlyIntervalGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return intervalBefore.End - Duration.One;
        }

        private static Instant StartOfLateIntervalGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return intervalAfter.Start;
        }

        private static Instant PushBackwardGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return localInstant.Minus(intervalAfter.Offset);
        }

        private static Instant PushForwardGap(ZoneInterval intervalBefore, ZoneInterval intervalAfter, LocalInstant localInstant, DateTimeZone zone)
        {
            return localInstant.Minus(intervalBefore.Offset);
        }

        #region Well-known resolvers
        private static readonly TransitionResolver strict = FromStrategies(AmbiguityStrategy.Strict, GapStrategy.Strict);
        /// <summary>
        /// Resolver which throws an exception in the case of either ambiguity or a gap.
        /// This is the equivalent of FromStrategies(AmbiguityStrategy.Strict, GapStrategy.Strict).
        /// </summary>
        public static TransitionResolver Strict { get { return null; } }

        private static readonly TransitionResolver preTransition = FromStrategies(AmbiguityStrategy.Earlier, GapStrategy.EndOfEarlyInterval);
        /// <summary>
        /// Resolver which returns the tick before the end of the earlier interval for gaps, and the earlier possible
        /// instant for ambiguities.
        /// This is the equivalent of FromStrategies(AmbiguityStrategy.Earlier, GapStrategy.EndOfEarlyInterval).
        /// </summary>
        public static TransitionResolver PreTransition { get { return preTransition; } }

        private static readonly TransitionResolver postTransition = FromStrategies(AmbiguityStrategy.Later, GapStrategy.StartOfLateInterval);
        /// <summary>
        /// Resolver which returns the start of the later interval for gaps, and the later possible
        /// instant for ambiguities.
        /// This is the equivalent of FromStrategies(AmbiguityStrategy.Later, GapStrategy.StartOfLateInterval).
        /// </summary>
        public static TransitionResolver PostTransition { get { return postTransition; } }

        private static readonly TransitionResolver pushForward = TransitionResolver.FromStrategies(AmbiguityStrategy.Earlier, GapStrategy.PushBackward);
        /// <summary>
        /// Resolver which pushes backward by the gap size for gaps, and the earlier possible
        /// instant for ambiguities.
        /// This is the equivalent of FromStrategies(AmbiguityStrategy.Earlier, GapStrategy.PushBackward).
        /// </summary>
        public static TransitionResolver PushForward { get { return pushForward; } }

        private static readonly TransitionResolver pushBackward = TransitionResolver.FromStrategies(AmbiguityStrategy.Later, GapStrategy.PushForward);
        /// <summary>
        /// Resolver which pushes forward by the gap size for gaps, and returns the later possible
        /// instant for ambiguities.
        /// This is the equivalent of FromStrategies(AmbiguityStrategy.Later, GapStrategy.PushForward).
        /// </summary>
        public static TransitionResolver PushBackward { get { return pushBackward; } }
        #endregion
    }
}
