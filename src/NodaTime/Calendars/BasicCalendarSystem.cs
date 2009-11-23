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

namespace NodaTime.Calendars
{
    public abstract class BasicCalendarSystem : AssembledCalendarSystem
    {
        private readonly static FieldSet preciseFields = CreatePreciseFields();
        
        private static FieldSet CreatePreciseFields()
        {
            // First create the simple durations, then fill in date/time fields,
            // which rely on the other properties
            FieldSet.Builder builder = new FieldSet.Builder()
            {
                Ticks = TicksDurationField.Instance,
                Milliseconds = new PreciseDurationField(DurationFieldType.Milliseconds, DateTimeConstants.TicksPerMillisecond),
                Seconds = new PreciseDurationField(DurationFieldType.Seconds, DateTimeConstants.TicksPerSecond),
                Minutes = new PreciseDurationField(DurationFieldType.Minutes, DateTimeConstants.TicksPerMinute),
                Hours = new PreciseDurationField(DurationFieldType.Hours, DateTimeConstants.TicksPerHour),
                HalfDays = new PreciseDurationField(DurationFieldType.HalfDays, DateTimeConstants.TicksPerDay / 2),
                Days = new PreciseDurationField(DurationFieldType.Days, DateTimeConstants.TicksPerDay),
                Weeks = new PreciseDurationField(DurationFieldType.Weeks, DateTimeConstants.TicksPerWeek)
            };
            builder.TickOfMillisecond = new PreciseDateTimeField(DateTimeFieldType.TickOfMillisecond, builder.Ticks, builder.Milliseconds);
            builder.TickOfDay = new PreciseDateTimeField(DateTimeFieldType.TickOfDay, builder.Ticks, builder.Days);
            builder.MillisecondOfSecond = new PreciseDateTimeField(DateTimeFieldType.MillisecondOfSecond, builder.Milliseconds, builder.Seconds);
            builder.SecondOfMinute = new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, builder.Seconds, builder.Minutes);
            builder.SecondOfDay = new PreciseDateTimeField(DateTimeFieldType.SecondOfDay, builder.Seconds, builder.Days);
            builder.MinuteOfHour = new PreciseDateTimeField(DateTimeFieldType.MinuteOfHour, builder.Minutes, builder.Hours);
            builder.MinuteOfDay = new PreciseDateTimeField(DateTimeFieldType.MinuteOfDay, builder.Minutes, builder.Days);
            builder.HourOfDay = new PreciseDateTimeField(DateTimeFieldType.HourOfDay, builder.Hours, builder.Days);
            builder.HourOfHalfDay = new PreciseDateTimeField(DateTimeFieldType.HourOfHalfDay, builder.Hours, builder.HalfDays);
            //builder.ClockHourOfDay = new ZeroIsMaxDateTimeField(builder.HourOfDay, DateTimeFieldType.ClockHourOfDay);
            //builder.ClockHourOfHalfDay = new ZeroIsMaxDateTimeField(builder.HourOfHalfDay, DateTimeFieldType.ClockHourOfHalfDay);
            //builder.HalfDayOfDay = new HalfDayField();
            return builder.Build();
        }

        // FIXME
        protected BasicCalendarSystem() : base(null, null) {}
    }
}