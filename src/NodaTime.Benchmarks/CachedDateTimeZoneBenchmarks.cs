// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Timing;

namespace NodaTime.Benchmarks
{
    internal class CachedDateTimeZoneBenchmarks
    {
        private readonly Instant[] cacheInstants = new Instant[100];
        private readonly Instant[] noCacheInstants = new Instant[500];
        private readonly DateTimeZone paris = DateTimeZoneProviders.Tzdb["Europe/Paris"];
        private readonly Instant[] twoYearsCacheInstants = new Instant[365];
        private int cacheIndex;
        private int noCacheIndex;
        private int twoYearsCacheIndex;

        public CachedDateTimeZoneBenchmarks()
        {
            var adjustment = new Duration(NodaConstants.TicksPerStandardDay * 365);
            for (int i = 0; i < noCacheInstants.Length; i++)
            {
                noCacheInstants[i] = NodaConstants.UnixEpoch + (adjustment * i);
            }
            for (int i = 0; i < cacheInstants.Length; i++)
            {
                cacheInstants[i] = NodaConstants.UnixEpoch + (adjustment * i);
            }
            var twoDays = new Duration(NodaConstants.TicksPerStandardDay * 2);
            for (int i = 0; i < twoYearsCacheInstants.Length; i++)
            {
                twoYearsCacheInstants[i] = NodaConstants.UnixEpoch + (twoDays * i);
            }
        }

        [Benchmark]
        public void GetPeriodInstant()
        {
            paris.GetZoneInterval(NodaConstants.UnixEpoch);
        }

        [Benchmark]
        public void GetPeriodInstant_NoCache()
        {
            paris.GetZoneInterval(noCacheInstants[noCacheIndex]);
            noCacheIndex = (noCacheIndex + 1) % noCacheInstants.Length;
        }

        [Benchmark]
        public void GetPeriodInstant_Cache()
        {
            paris.GetZoneInterval(cacheInstants[cacheIndex]);
            cacheIndex = (cacheIndex + 1) % cacheInstants.Length;
        }

        [Benchmark]
        public void GetPeriodInstant_TwoYears()
        {
            paris.GetZoneInterval(twoYearsCacheInstants[twoYearsCacheIndex]);
            twoYearsCacheIndex = (twoYearsCacheIndex + 1) % twoYearsCacheInstants.Length;
        }

        [Benchmark]
        public void GetUtcOffset()
        {
            paris.GetUtcOffset(NodaConstants.UnixEpoch);
        }
    }
}