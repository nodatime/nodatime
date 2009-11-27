using System;

namespace NodaTime.TimeZones
{
    internal class DSTZone
        : IDateTimeZone
    {

        internal Duration StandardOffset { get; set; }
        internal ZoneRecurrence StartRecurrence { get; set; }
        internal ZoneRecurrence EndRecurrence { get; set; }

        internal DSTZone(String id, Duration standardOffset,
                ZoneRecurrence startRecurrence, ZoneRecurrence endRecurrence)
        {
            Id = id;
            StandardOffset = standardOffset;
            StartRecurrence = startRecurrence;
            EndRecurrence = endRecurrence;
        }

        public String getNameKey(LocalInstant instant)
        {
            return findMatchingRecurrence(instant).Name;
        }

        public Duration getOffset(LocalInstant instant)
        {
            return StandardOffset + findMatchingRecurrence(instant).Savings;
        }

        public Duration getStandardOffset(LocalInstant instant)
        {
            return StandardOffset;
        }

        public LocalInstant nextTransition(LocalInstant instant)
        {
            Duration standardOffset = StandardOffset;
            ZoneRecurrence startRecurrence = StartRecurrence;
            ZoneRecurrence endRecurrence = EndRecurrence;

            LocalInstant start, end;

            try {
                start = startRecurrence.Next(instant, standardOffset, endRecurrence.Savings);
                if (instant > LocalInstant.LocalUnixEpoch && start < LocalInstant.LocalUnixEpoch) {
                    // Overflowed.
                    start = instant;
                }
            }
            catch (ArgumentException) {
                // Overflowed.
                start = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                start = instant;
            }

            try {
                end = endRecurrence.Next(instant, standardOffset, startRecurrence.Savings);
                if (instant > LocalInstant.LocalUnixEpoch && end < LocalInstant.LocalUnixEpoch) {
                    // Overflowed.
                    end = instant;
                }
            }
            catch (ArgumentException) {
                // Overflowed.
                end = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                end = instant;
            }

            return (start > end) ? end : start;
        }

        public LocalInstant previousTransition(LocalInstant instant)
        {
            // Increment in order to handle the case where instant is exactly at
            // a transition.
            instant = instant + Duration.One;

            Duration standardOffset = StandardOffset;
            ZoneRecurrence startRecurrence = StartRecurrence;
            ZoneRecurrence endRecurrence = EndRecurrence;

            LocalInstant start, end;

            try {
                start = startRecurrence.Previous(instant, standardOffset, endRecurrence.Savings);
                if (instant < LocalInstant.LocalUnixEpoch && start > LocalInstant.LocalUnixEpoch) {
                    // Overflowed.
                    start = instant;
                }
            }
            catch (ArgumentException) {
                // Overflowed.
                start = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                start = instant;
            }

            try {
                end = endRecurrence.Previous(instant, standardOffset, startRecurrence.Savings);
                if (instant < LocalInstant.LocalUnixEpoch && end > LocalInstant.LocalUnixEpoch) {
                    // Overflowed.
                    end = instant;
                }
            }
            catch (ArgumentException) {
                // Overflowed.
                end = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                end = instant;
            }

            return new LocalInstant(((start > end) ? start : end).Ticks - 1);
        }

        public bool equals(Object obj)
        {
            if (this == obj) {
                return true;
            }
            if (obj is DSTZone) {
                DSTZone other = (DSTZone)obj;
                return
                    Id == other.Id &&
                    StandardOffset == other.StandardOffset &&
                    StartRecurrence.Equals(other.StartRecurrence) &&
                    EndRecurrence.Equals(other.EndRecurrence);
            }
            return false;
        }

        private ZoneRecurrence findMatchingRecurrence(LocalInstant instant)
        {
            Duration standardOffset = StandardOffset;
            ZoneRecurrence startRecurrence = StartRecurrence;
            ZoneRecurrence endRecurrence = EndRecurrence;

            LocalInstant start, end;

            try {
                start = startRecurrence.Next(instant, standardOffset, endRecurrence.Savings);
            }
            catch (ArgumentException) {
                // Overflowed.
                start = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                start = instant;
            }

            try {
                end = endRecurrence.Next(instant, standardOffset, startRecurrence.Savings);
            }
            catch (ArgumentException) {
                // Overflowed.
                end = instant;
            }
            catch (ArithmeticException) {
                // Overflowed.
                end = instant;
            }

            return (start > end) ? startRecurrence : endRecurrence;
        }

        #region IDateTimeZone Members

        public Instant? NextTransition(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Instant? PreviousTransition(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Duration GetOffsetFromUtc(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Duration GetOffsetFromLocal(LocalDateTime localTime)
        {
            throw new NotImplementedException();
        }

        public string Id { get; private set; }

        public bool IsFixed { get; private set; }

        #endregion
    }

}
