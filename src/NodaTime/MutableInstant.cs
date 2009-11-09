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

namespace NodaTime
{
    /// <summary>
    /// Original name: MutableInstant.
    /// </summary>
    public class MutableInstant : IMutableInstant
    {
        public int CompareTo(IInstant other)
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

        public int Get(DateTimeFieldType field)
        {
            throw new NotImplementedException();
        }

        public int Get(DateTimeField second)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IChronology chronology)
        {
            throw new NotImplementedException();
        }

        public DateTimeZone Zone
        {
            get { throw new NotImplementedException(); }
        }

        public MutableDateTime ToMutableDateTime(DateTimeZone paris)
        {
            throw new NotImplementedException();
        }

        public MutableDateTime ToMutableDateTime(IChronology chronology)
        {
            throw new NotImplementedException();
        }

        public MutableDateTime ToMutableDateTime()
        {
            throw new NotImplementedException();
        }
    }
}