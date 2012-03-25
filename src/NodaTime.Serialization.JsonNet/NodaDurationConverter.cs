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
using System.IO;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    public class NodaDurationConverter : NodaConverterBase<Duration>
    {
        protected override Duration ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            var durationText = (string)reader.Value;

            var parts = durationText.Split(':');
            if (parts.Length < 2)
            {
                throw new InvalidDataException("A Duration must have at least hours and minutes in hh:mm format.");
            }

            if (parts.Length > 3)
            {
                throw new InvalidDataException("Too many components provided for a duration.  Should be in hh:mm:ss.fffffff format.");
            }

            var duration = Duration.Zero;

            long hours;
            if (!long.TryParse(parts[0], out hours))
            {
                throw new InvalidDataException("Invalid hours component of duration.");
            }

            duration += Duration.FromHours(hours);

            long minutes;
            if (!long.TryParse(parts[1], out minutes))
            {
                throw new InvalidDataException("Invalid minutes component of duration.");
            }

            duration += Duration.FromMinutes(minutes);

            decimal seconds;
            if (!decimal.TryParse(parts[2], out seconds))
            {
                throw new InvalidDataException("Invalid seconds component of duration.");
            }

            // if we convert to ticks, we get all fractional parts of the seconds component at once.
            var ticks = Convert.ToInt64(seconds * 1000 * 10000);

            duration += Duration.FromTicks(ticks);

            return duration;
        }

        protected override void WriteJsonImpl(JsonWriter writer, Duration value, JsonSerializer serializer)
        {
            var hours = value.Ticks / NodaConstants.TicksPerHour;
            var minutes = (value.Ticks % NodaConstants.TicksPerHour) / NodaConstants.TicksPerMinute;
            var seconds = (value.Ticks % NodaConstants.TicksPerMinute) / NodaConstants.TicksPerSecond;
            var milliseconds = (value.Ticks % NodaConstants.TicksPerSecond) / NodaConstants.TicksPerMillisecond;
            var ticks = value.Ticks % NodaConstants.TicksPerMillisecond;

            var durationText = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

            if (milliseconds > 0 || ticks > 0)
            {
                durationText += string.Format(".{0:D3}", milliseconds);

                if (ticks > 0)
                {
                    durationText += string.Format("{0:D4}", ticks);
                }
            }

            writer.WriteValue(durationText);
        }
    }
}
