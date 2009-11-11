#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.Base
{
    /// <summary>
    /// Original name: BaseDateTime.
    /// </summary>
    public abstract class DateTimeBase : AbstractDateTime
    {
        private long millis;
        private IChronology chronology;

        public override IChronology Chronology { get { return chronology; } }

        /// <summary>
        /// Note: Not a property because we can't override the "getter-only" property
        /// with a writable one.
        /// </summary>
        protected void SetChronology(IChronology chronology)
        {
            this.chronology = chronology;
        }

        protected DateTimeBase(long instant, IChronology chronology)
        {
            this.chronology = chronology;
            millis = instant;
        }

        protected DateTimeBase(int year, int month, int day, int hour, int minute, int second, int millis, DateTimeZone chronology) 
        {
            throw new System.NotImplementedException();
        }
    }
}
