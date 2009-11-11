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

using System;

namespace NodaTime.Base
{
    /// <summary>
    /// Original name: AbstractInstant.
    /// </summary>
    public abstract class AbstractInstant : IInstant
    {
        public abstract IChronology Chronology { get; }

        public DateTimeZone Zone
        {
            get { return Chronology.Zone; }
        }

        public virtual long Milliseconds
        {
            get { throw new NotImplementedException(); }
        }

        public virtual int CompareTo(IInstant instant)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsEqual(IInstant instant)
        {
            throw new NotImplementedException();
        }

        public bool IsEqual(long other)
        {
            throw new NotImplementedException();
        }

        public bool IsEqualNow()
        {
            throw new NotImplementedException();
        }

        public bool IsBefore(long other)
        {
            throw new NotImplementedException();
        }

        public bool IsBeforeNow()
        {
            throw new NotImplementedException();
        }

        public bool IsBefore(IInstant instant)
        {
            throw new NotImplementedException();
        }

        public bool IsAfter(long other)
        {
            throw new NotImplementedException();
        }

        public bool IsAfterNow()
        {
            throw new NotImplementedException();
        }

        public bool IsAfter(IInstant instant)
        {
            throw new NotImplementedException();
        }

        public Instant ToInstant()
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime()
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(DateTimeZone paris)
        {
            throw new NotImplementedException();
        }

        public int this[DateTimeFieldType field]
        {
            get { throw new NotImplementedException(); }
        }

        public int this[DateTimeField second]
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ToDateTime(IChronology chronology)
        {
            throw new NotImplementedException();
        }
    }
}