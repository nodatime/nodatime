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

using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    internal class MockCountingDurationField : DurationFieldBase
    {
        // FIXME: Use a proper mock?
        internal static int int32Additions;
        internal static int int64Additions;
        internal static int differences;

        internal MockCountingDurationField(DurationFieldType fieldType)
            : base(fieldType)
        {
        }

        public override bool IsPrecise { get { return true; } }

        public override long UnitTicks { get { return 60; } }

        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return 0;
        }

        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(0);
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            int32Additions++;
            return new LocalInstant(localInstant.Ticks + value * 60L);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            int64Additions++;
            return new LocalInstant(localInstant.Ticks + value * 60L);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            differences++;
            return 30;
        }
    }
}
