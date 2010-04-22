#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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

using NodaTime.Benchmarks.Timing;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks
{
    internal class CachedDateTimeZoneBenchmarks
    {
        private readonly Instant[] cacheInstants = new Instant[100];
        private readonly LocalInstant[] cacheLocalInstants = new LocalInstant[100];
        private readonly Instant[] noCacheInstants = new Instant[500];
        private readonly LocalInstant[] noCacheLocalInstants = new LocalInstant[500];
        private readonly IDateTimeZone paris = DateTimeZones.ForId("Europe/Paris");
        private readonly Instant[] twoYearsCacheInstants = new Instant[365];
        private readonly LocalInstant[] twoYearsCacheLocalInstants = new LocalInstant[365];
        private int cacheIndex;
        private int cacheLocalIndex;
        private int noCacheIndex;
        private int noCacheLocalIndex;
        private int twoYearsCacheIndex;
        private int twoYearsCacheLocalIndex;

        public CachedDateTimeZoneBenchmarks()
        {
            var adjustment = new Duration(NodaConstants.TicksPerDay * 365);
            for (int i = 0; i < this.noCacheInstants.Length; i++)
            {
                this.noCacheInstants[i] = Instant.UnixEpoch + (adjustment * i);
            }
            for (int i = 0; i < this.cacheInstants.Length; i++)
            {
                this.cacheInstants[i] = Instant.UnixEpoch + (adjustment * i);
            }
            var twoDays = new Duration(NodaConstants.TicksPerDay * 2);
            for (int i = 0; i < this.twoYearsCacheInstants.Length; i++)
            {
                this.twoYearsCacheInstants[i] = Instant.UnixEpoch + (twoDays * i);
            }

            for (int i = 0; i < this.noCacheLocalInstants.Length; i++)
            {
                this.noCacheLocalInstants[i] = LocalInstant.LocalUnixEpoch + (adjustment * i);
            }
            for (int i = 0; i < this.cacheLocalInstants.Length; i++)
            {
                this.cacheLocalInstants[i] = LocalInstant.LocalUnixEpoch + (adjustment * i);
            }
            for (int i = 0; i < this.twoYearsCacheLocalInstants.Length; i++)
            {
                this.twoYearsCacheLocalInstants[i] = LocalInstant.LocalUnixEpoch + (twoDays * i);
            }
        }

        [Benchmark]
        public void GetPeriodInstant()
        {
            this.paris.GetZoneInterval(Instant.UnixEpoch);
        }

        [Benchmark]
        public void GetPeriodInstant_NoCache()
        {
            this.paris.GetZoneInterval(this.noCacheInstants[this.noCacheIndex]);
            this.noCacheIndex = (this.noCacheIndex + 1) % this.noCacheInstants.Length;
        }

        [Benchmark]
        public void GetPeriodInstant_Cache()
        {
            this.paris.GetZoneInterval(this.cacheInstants[this.cacheIndex]);
            this.cacheIndex = (this.cacheIndex + 1) % this.cacheInstants.Length;
        }

        [Benchmark]
        public void GetPeriodInstant_TwoYears()
        {
            this.paris.GetZoneInterval(this.twoYearsCacheInstants[this.twoYearsCacheIndex]);
            this.twoYearsCacheIndex = (this.twoYearsCacheIndex + 1) % this.twoYearsCacheInstants.Length;
        }

        [Benchmark]
        public void GetPeriodLocalInstant()
        {
            this.paris.GetZoneInterval(LocalInstant.LocalUnixEpoch);
        }

        [Benchmark]
        public void GetPeriodLocalInstant_NoCache()
        {
            this.paris.GetZoneInterval(this.noCacheLocalInstants[this.noCacheLocalIndex]);
            this.noCacheLocalIndex = (this.noCacheLocalIndex + 1) % this.noCacheLocalInstants.Length;
        }

        [Benchmark]
        public void GetPeriodLocalInstant_Cache()
        {
            this.paris.GetZoneInterval(this.cacheLocalInstants[this.cacheLocalIndex]);
            this.cacheLocalIndex = (this.cacheLocalIndex + 1) % this.cacheLocalInstants.Length;
        }

        [Benchmark]
        public void GetPeriodLocalInstant_TwoYears()
        {
            this.paris.GetZoneInterval(this.twoYearsCacheLocalInstants[this.twoYearsCacheLocalIndex]);
            this.twoYearsCacheLocalIndex = (this.twoYearsCacheLocalIndex + 1) % this.twoYearsCacheLocalInstants.Length;
        }

        [Benchmark]
        public void GetOffsetFromUtc()
        {
            this.paris.GetOffsetFromUtc(Instant.UnixEpoch);
        }

        [Benchmark]
        public void GetOffsetFromLocal()
        {
            this.paris.GetOffsetFromLocal(LocalInstant.LocalUnixEpoch);
        }

        [Benchmark]
        public void Name()
        {
            this.paris.GetName(Instant.UnixEpoch);
        }
    }
}