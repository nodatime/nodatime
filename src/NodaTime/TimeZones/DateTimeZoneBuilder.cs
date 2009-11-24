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
using System.Collections.Generic;

// TODO: This is a hack to get the code working. When the real ISoCalendarSystem is ready
//       remove all alias lines in all files in the package and remove the JIsoCalendarSystem.cs file.
using IsoCalendarSystem = NodaTime.TimeZones.JIsoCalendarSystem;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a means of programatically creating complex time zones .
    /// </summary>
    /// <remarks>
    /// <para>
    /// DateTimeZoneBuilder allows complex DateTimeZones to be constructed. Since creating a new
    /// DateTimeZone this way is a relatively expensive operation, built zones can be written to a
    /// file. Reading back the encoded data is a quick operation.
    /// </para>
    /// <para>
    /// DateTimeZoneBuilder itself is mutable and not thread-safe, but the DateTimeZone objects that
    /// it builds are thread-safe and immutable.
    /// </para>
    /// <para>
    /// It is intended that {@link ZoneInfoCompiler} be used to read time zone data files,
    /// indirectly calling DateTimeZoneBuilder. The following complex example defines the
    /// America/Los_Angeles time zone, with all historical transitions:
    /// </para>
    /// <para>
    /// <example>
    ///     DateTimeZone America_Los_Angeles = new DateTimeZoneBuilder()
    ///         .AddCutover(-2147483648, 'w', 1, 1, 0, false, 0)
    ///         .SetStandardOffset(-28378000)
    ///         .SetFixedSavings("LMT", 0)
    ///         .AddCutover(1883, 'w', 11, 18, 0, false, 43200000)
    ///         .SetStandardOffset(-28800000)
    ///         .AddRecurringSavings("PDT", 3600000, 1918, 1919, 'w',  3, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1918, 1919, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PWT", 3600000, 1942, 1942, 'w',  2,  9, 0, false, 7200000)
    ///         .AddRecurringSavings("PPT", 3600000, 1945, 1945, 'u',  8, 14, 0, false, 82800000)
    ///         .AddRecurringSavings("PST",       0, 1945, 1945, 'w',  9, 30, 0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1948, 1948, 'w',  3, 14, 0, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1949, 1949, 'w',  1,  1, 0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1950, 1966, 'w',  4, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1950, 1961, 'w',  9, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1962, 1966, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1967, 2147483647, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1967, 1973, 'w', 4, -1,  7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1974, 1974, 'w', 1,  6,  0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1975, 1975, 'w', 2, 23,  0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1976, 1986, 'w', 4, -1,  7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1987, 2147483647, 'w', 4, 1, 7, true, 7200000)
    ///         .ToDateTimeZone("America/Los_Angeles");
    /// </example>
    /// </para>
    /// <para>
    /// Original name: DateTimeZoneBuilder.
    /// </para>
    /// </remarks>
    public sealed class DateTimeZoneBuilder
    {
        private readonly IList<ZoneRuleSet> ruleSets = new List<ZoneRuleSet>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneBuilder"/> class.
        /// </summary>
        public DateTimeZoneBuilder()
        {
        }

        /// <summary>
        /// Adds a cutover for added rules. The standard offset at the cutover defaults to 0. Call
        /// <see cref="DateTimeZoneBuilder.SetStandardOffset"/> afterwards to change it.
        /// </summary>
        /// <param name="year">The year of cutover.</param>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of the month. If negative, set to ((last day of month)
        /// - ~dayOfMonth). For example, if -1, set to last day of month</param>
        /// <param name="dayOfWeek">The day of week. If 0, ignore.</param>
        /// <param name="advanceDayOfWeek">if dayOfMonth does not fall on dayOfWeek, then if advanceDayOfWeek set to <c>true</c>
        /// advance to dayOfWeek when true, otherwise retreat to dayOfWeek when true.</param>
        /// <param name="ticksOfDay">The <see cref="Duration"/> into the day. Additional precision for specifying time of day of transitions</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder AddCutover(int year,
                                              TransitionMode mode,
                                              int monthOfYear,
                                              int dayOfMonth,
                                              int dayOfWeek,
                                              bool advanceDayOfWeek,
                                              Duration ticksOfDay)
        {
            if (ruleSets.Count > 0) {
                ZoneYearOffset yearOffset = new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advanceDayOfWeek, ticksOfDay);
                LastRuleSet.SetUpperLimit(year, yearOffset);
            }
            AddEndOfTimeRuleSet();
            return this;
        }

        /// <summary>
        /// Sets the standard offset to use for newly added rules until the next cutover is added.
        /// </summary>
        /// <param name="standardOffset">The standard offset.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder SetStandardOffset(Duration standardOffset)
        {
            LastRuleSet.StandardOffset = standardOffset;
            return this;
        }

        /// <summary>
        /// Sets a fixed savings rule at the cutover.
        /// </summary>
        /// <param name="nameKey">The name key of new rule.</param>
        /// <param name="savings">The <see cref="Duration"/> to add to standard offset.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder SetFixedSavings(String nameKey, Duration savings)
        {
            LastRuleSet.SetFixedSavings(nameKey, savings);
            return this;
        }

        /// <summary>
        /// Adds a recurring daylight saving time rule.
        /// </summary>
        /// <param name="nameKey">The name key of new rule.</param>
        /// <param name="savings">The <see cref="Duration"/> to add to standard offset.</param>
        /// <param name="fromYear">First year that rule is in effect. <see cref="Int32.MinValue"/> indicates beginning of time.</param>
        /// <param name="toYear">Last year (inclusive) that rule is in effect. <see cref="Int32.MaxValue"/> indicates end of time.</param>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthYearOffset">The month of year.</param>
        /// <param name="dayOfMonth">The day of the month. If negative, set to ((last day of month)
        /// - ~dayOfMonth). For example, if -1, set to last day of month</param>
        /// <param name="dayOfWeek">The day of week. If 0, ignore.</param>
        /// <param name="advanceDayOfWeek">if dayOfMonth does not fall on dayOfWeek, then if advanceDayOfWeek set to <c>true</c>
        /// advance to dayOfWeek when true, otherwise retreat to dayOfWeek when true.</param>
        /// <param name="ticksOfDay">The <see cref="Duration"/> into the day. Additional precision for specifying time of day of transitions</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder AddRecurringSavings(String nameKey,
                                                       Duration savings,
                                                       int fromYear,
                                                       int toYear,
                                                       TransitionMode mode,
                                                       int monthYearOffset,
                                                       int dayOfMonth,
                                                       int dayOfWeek,
                                                       bool advanceDayOfWeek,
                                                       Duration ticksOfDay)
        {
            if (fromYear <= toYear) {
                ZoneYearOffset yearOffset = new ZoneYearOffset(mode, monthYearOffset, dayOfMonth, dayOfWeek, advanceDayOfWeek, ticksOfDay);
                ZoneRecurrence recurrence = new ZoneRecurrence(yearOffset, nameKey, savings);
                ZoneRule rule = new ZoneRule(recurrence, fromYear, toYear);
                LastRuleSet.AddRule(rule);
            }
            return this;
        }

        /// <summary>
        /// Gets the last rule set if there are no rule sets one that spans all of time is created and returned.
        /// </summary>
        /// <value>The last rule set.</value>
        private ZoneRuleSet LastRuleSet
        {
            get
            {
                if (ruleSets.Count == 0) {
                    AddEndOfTimeRuleSet();
                }
                return ruleSets[ruleSets.Count - 1];
            }
        }

        /// <summary>
        /// Adds a rule set that spans from the last one to the end of time.
        /// </summary>
        private void AddEndOfTimeRuleSet()
        {
            ruleSets.Add(new ZoneRuleSet());
        }
    }
}
#if FOO

public class DateTimeZoneBuilder {
    /**
     * Decodes a built DateTimeZone from the given stream, as encoded by
     * writeTo.
     *
     * @param in input stream to read encoded DateTimeZone from.
     * @param id time zone id to assign
     */
    public static DateTimeZone readFrom(InputStream in, String id) throws IOException {
        if (in instanceof DataInput) {
            return readFrom((DataInput)in, id);
        } else {
            return readFrom((DataInput)new DataInputStream(in), id);
        }
    }

    /**
     * Decodes a built DateTimeZone from the given stream, as encoded by
     * writeTo.
     *
     * @param in input stream to read encoded DateTimeZone from.
     * @param id time zone id to assign
     */
    public static DateTimeZone readFrom(DataInput in, String id) throws IOException {
        switch (in.readUnsignedByte()) {
        case 'F':
            DateTimeZone fixed = new FixedDateTimeZone
                (id, in.readUTF(), (int)readMillis(in), (int)readMillis(in));
            if (fixed.equals(DateTimeZone.UTC)) {
                fixed = DateTimeZone.UTC;
            }
            return fixed;
        case 'C':
            return CachedDateTimeZone.forZone(PrecalculatedZone.readFrom(in, id));
        case 'P':
            return PrecalculatedZone.readFrom(in, id);
        default:
            throw new IOException("Invalid encoding");
        }
    }

    /**
     * Millisecond encoding formats:
     *
     * upper two bits  units       field length  approximate range
     * ---------------------------------------------------------------
     * 00              30 minutes  1 byte        +/- 16 hours
     * 01              minutes     4 bytes       +/- 1020 years
     * 10              seconds     5 bytes       +/- 4355 years
     * 11              millis      9 bytes       +/- 292,000,000 years
     *
     * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
     */
    static void writeMillis(DataOutput out, long millis) throws IOException {
        if (millis % (30 * 60000L) == 0) {
            // Try to write in 30 minute units.
            long units = millis / (30 * 60000L);
            if (((units << (64 - 6)) >> (64 - 6)) == units) {
                // Form 00 (6 bits effective precision)
                out.writeByte((int)(units & 0x3f));
                return;
            }
        }

        if (millis % 60000L == 0) {
            // Try to write minutes.
            long minutes = millis / 60000L;
            if (((minutes << (64 - 30)) >> (64 - 30)) == minutes) {
                // Form 01 (30 bits effective precision)
                out.writeInt(0x40000000 | (int)(minutes & 0x3fffffff));
                return;
            }
        }
        
        if (millis % 1000L == 0) {
            // Try to write seconds.
            long seconds = millis / 1000L;
            if (((seconds << (64 - 38)) >> (64 - 38)) == seconds) {
                // Form 10 (38 bits effective precision)
                out.writeByte(0x80 | (int)((seconds >> 32) & 0x3f));
                out.writeInt((int)(seconds & 0xffffffff));
                return;
            }
        }

        // Write milliseconds either because the additional precision is
        // required or the minutes didn't fit in the field.
        
        // Form 11 (64 bits effective precision, but write as if 70 bits)
        out.writeByte(millis < 0 ? 0xff : 0xc0);
        out.writeLong(millis);
    }

    /**
     * Reads encoding generated by writeMillis.
     */
    static long readMillis(DataInput in) throws IOException {
        int v = in.readUnsignedByte();
        switch (v >> 6) {
        case 0: default:
            // Form 00 (6 bits effective precision)
            v = (v << (32 - 6)) >> (32 - 6);
            return v * (30 * 60000L);

        case 1:
            // Form 01 (30 bits effective precision)
            v = (v << (32 - 6)) >> (32 - 30);
            v |= (in.readUnsignedByte()) << 16;
            v |= (in.readUnsignedByte()) << 8;
            v |= (in.readUnsignedByte());
            return v * 60000L;

        case 2:
            // Form 10 (38 bits effective precision)
            long w = (((long)v) << (64 - 6)) >> (64 - 38);
            w |= (in.readUnsignedByte()) << 24;
            w |= (in.readUnsignedByte()) << 16;
            w |= (in.readUnsignedByte()) << 8;
            w |= (in.readUnsignedByte());
            return w * 1000L;

        case 3:
            // Form 11 (64 bits effective precision)
            return in.readLong();
        }
    }

    private static DateTimeZone buildFixedZone(String id, String nameKey,
                                               int wallOffset, int standardOffset) {
        if ("UTC".equals(id) && id.equals(nameKey) &&
            wallOffset == 0 && standardOffset == 0) {
            return DateTimeZone.UTC;
        }
        return new FixedDateTimeZone(id, nameKey, wallOffset, standardOffset);
    }

    private RuleSet getLastRuleSet() {
        if (iRuleSets.size() == 0) {
            addCutover(Integer.MIN_VALUE, 'w', 1, 1, 0, false, 0);
        }
        return (RuleSet)iRuleSets.get(iRuleSets.size() - 1);
    }
    
    /**
     * Processes all the rules and builds a DateTimeZone.
     *
     * @param id  time zone id to assign
     * @param outputID  true if the zone id should be output
     */
    public DateTimeZone toDateTimeZone(String id, boolean outputID) {
        if (id == null) {
            throw new IllegalArgumentException();
        }

        // Discover where all the transitions occur and store the results in
        // these lists.
        ArrayList transitions = new ArrayList();

        // Tail zone picks up remaining transitions in the form of an endless
        // DST cycle.
        DSTZone tailZone = null;

        long millis = Long.MIN_VALUE;
        int saveMillis = 0;
            
        int ruleSetCount = iRuleSets.size();
        for (int i=0; i<ruleSetCount; i++) {
            RuleSet rs = (RuleSet)iRuleSets.get(i);
            Transition next = rs.firstTransition(millis);
            if (next == null) {
                continue;
            }
            addTransition(transitions, next);
            millis = next.getMillis();
            saveMillis = next.getSaveMillis();

            // Copy it since we're going to destroy it.
            rs = new RuleSet(rs);

            while ((next = rs.nextTransition(millis, saveMillis)) != null) {
                if (addTransition(transitions, next)) {
                    if (tailZone != null) {
                        // Got the extra transition before DSTZone.
                        break;
                    }
                }
                millis = next.getMillis();
                saveMillis = next.getSaveMillis();
                if (tailZone == null && i == ruleSetCount - 1) {
                    tailZone = rs.buildTailZone(id);
                    // If tailZone is not null, don't break out of main loop until
                    // at least one more transition is calculated. This ensures a
                    // correct 'seam' to the DSTZone.
                }
            }

            millis = rs.getUpperLimit(saveMillis);
        }

        // Check if a simpler zone implementation can be returned.
        if (transitions.size() == 0) {
            if (tailZone != null) {
                // This shouldn't happen, but handle just in case.
                return tailZone;
            }
            return buildFixedZone(id, "UTC", 0, 0);
        }
        if (transitions.size() == 1 && tailZone == null) {
            Transition tr = (Transition)transitions.get(0);
            return buildFixedZone(id, tr.getNameKey(),
                                  tr.getWallOffset(), tr.getStandardOffset());
        }

        PrecalculatedZone zone = PrecalculatedZone.create(id, outputID, transitions, tailZone);
        if (zone.isCachable()) {
            return CachedDateTimeZone.forZone(zone);
        }
        return zone;
    }

    private boolean addTransition(ArrayList transitions, Transition tr) {
        int size = transitions.size();
        if (size == 0) {
            transitions.add(tr);
            return true;
        }

        Transition last = (Transition)transitions.get(size - 1);
        if (!tr.isTransitionFrom(last)) {
            return false;
        }

        // If local time of new transition is same as last local time, just
        // replace last transition with new one.
        int offsetForLast = 0;
        if (size >= 2) {
            offsetForLast = ((Transition)transitions.get(size - 2)).getWallOffset();
        }
        int offsetForNew = last.getWallOffset();

        long lastLocal = last.getMillis() + offsetForLast;
        long newLocal = tr.getMillis() + offsetForNew;

        if (newLocal != lastLocal) {
            transitions.add(tr);
            return true;
        }

        transitions.remove(size - 1);
        return addTransition(transitions, tr);
    }

    /**
     * Encodes a built DateTimeZone to the given stream. Call readFrom to
     * decode the data into a DateTimeZone object.
     *
     * @param out output stream to receive encoded DateTimeZone.
     * @since 1.5 (parameter added)
     */
    public void writeTo(String zoneID, OutputStream out) throws IOException {
        if (out instanceof DataOutput) {
            writeTo(zoneID, (DataOutput)out);
        } else {
            writeTo(zoneID, (DataOutput)new DataOutputStream(out));
        }
    }

    /**
     * Encodes a built DateTimeZone to the given stream. Call readFrom to
     * decode the data into a DateTimeZone object.
     *
     * @param out output stream to receive encoded DateTimeZone.
     * @since 1.5 (parameter added)
     */
    public void writeTo(String zoneID, DataOutput out) throws IOException {
        // pass false so zone id is not written out
        DateTimeZone zone = toDateTimeZone(zoneID, false);

        if (zone instanceof FixedDateTimeZone) {
            out.writeByte('F'); // 'F' for fixed
            out.writeUTF(zone.getNameKey(0));
            writeMillis(out, zone.getOffset(0));
            writeMillis(out, zone.getStandardOffset(0));
        } else {
            if (zone instanceof CachedDateTimeZone) {
                out.writeByte('C'); // 'C' for cached, precalculated
                zone = ((CachedDateTimeZone)zone).getUncachedZone();
            } else {
                out.writeByte('P'); // 'P' for precalculated, uncached
            }
            ((PrecalculatedZone)zone).writeTo(out);
        }
    }
}
#endif
