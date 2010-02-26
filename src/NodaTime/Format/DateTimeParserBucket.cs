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
using NodaTime.Fields;
using System;
using NodaTime.Calendars;
namespace NodaTime.Format
{
    /// <summary>
    /// DateTimeParserBucket is an advanced class, intended mainly for parser
    /// implementations. It can also be used during normal parsing operations to
    /// capture more information about the parse.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class allows fields to be saved in any order, but be physically set in
    /// a consistent order. This is useful for parsing against formats that allow
    /// field values to contradict each other.
    /// </para>
    /// <para>
    /// Field values are applied in an order where the "larger" fields are set
    /// first, making their value less likely to stick.  A field is larger than
    /// another when it's range duration is longer. If both ranges are the same,
    /// then the larger field has the longer duration. If it cannot be determined
    /// which field is larger, then the fields are set in the order they were saved.
    /// </para>
    /// <para>
    /// For example, these fields were saved in this order: dayOfWeek, monthOfYear,
    /// dayOfMonth, dayOfYear. When computeMillis is called, the fields are set in
    /// this order: monthOfYear, dayOfYear, dayOfMonth, dayOfWeek.
    /// </para>
    /// <para>
    /// DateTimeParserBucket is mutable and not thread-safe.
    /// </para>
    /// </remarks>
    public class DateTimeParserBucket
    {
        private class SavedField
        {
            private readonly IDateTimeField field;
            private readonly int value;
            private readonly string text;
            private readonly IFormatProvider provider;

            public SavedField(IDateTimeField field, int value)
            {
                this.field = field;
                this.value = value;
            }

            public SavedField(IDateTimeField field, string text, IFormatProvider provider)
            {
                this.field = field;
                this.text = text;
                this.provider = provider;
            }

            public LocalInstant Set(LocalInstant instant, bool reset)
            {
                instant = text == null ? field.SetValue(instant, value) 
                                        : field.SetValue(instant, text, provider);

                if (reset)
                {
                    instant = field.RoundFloor(instant);
                }

                return instant;
            }

        }

        private readonly Chronology chronology;
        private Offset offset;
        private IDateTimeZone zone;
        private readonly LocalInstant instant;
        private readonly IFormatProvider provider;
        private int? pivotYear;

        private SavedField[] savedFields = new SavedField[8];
        private int savedFieldsCount;
        private bool savedFieldsShared;
        private object savedFieldsState;


        /// <summary>
        /// Initializes a bucket, with the option of specifying the pivot year for
        /// two-digit year parsing.
        /// </summary>
        /// <param name="instant">The initial local instant</param>
        /// <param name="chronology">The chronology to use</param>
        /// <param name="provider">The format provider to use</param>
        /// <param name="pivotYear">The pivot year to use when parsing two-digit years</param>
        public DateTimeParserBucket(LocalInstant instant, Chronology chronology, IFormatProvider provider, int? pivotYear)
        {
            if (chronology == null)
            {
                throw new ArgumentNullException("chronology");
            }

            this.instant = instant;
            this.chronology = chronology;
            this.provider = provider;
            this.pivotYear = pivotYear;
        }

        /// <summary>
        /// Initializes a bucket.
        /// </summary>
        /// <param name="instant">The initial local instant</param>
        /// <param name="chronology">The chronology to use</param>
        /// <param name="provider">The format provider to use</param>
        public DateTimeParserBucket(LocalInstant instant, Chronology chronology, IFormatProvider provider)
            :this(instant, chronology, provider, null)
        {
        }

        /// <summary>
        /// Gets the chronology of the bucket
        /// </summary>
        public Chronology Chronology { get { return chronology; } }

        /// <summary>
        /// Gets the format provider to be used during parsing.
        /// </summary>
        public IFormatProvider Provider { get { return provider; } }

        /// <summary>
        /// Gets/sets the pivot year to use when parsing two digit years.
        /// </summary>
        public int? PivotYear
        {
            get { return pivotYear; }
            set { pivotYear = value; }
        }

        public Offset Offset
        {
            get { return offset; }
            set
            {
                savedFieldsState = null;
                offset = value;
                zone = null;
            }

        }
        /// <summary>
        /// Saves a datetime field value.
        /// </summary>
        /// <param name="field">The field, whose chronology must match that of this bucket</param>
        /// <param name="value">The value</param>
        public void SaveField(IDateTimeField field, int value)
        {
            SaveField(new SavedField(field, value));
        }

        /// <summary>
        /// Saves a datetime field value.
        /// </summary>
        /// <param name="fieldType">The field type</param>
        /// <param name="value">The value</param>
        public void SaveField(DateTimeFieldType fieldType, int value)
        {
            SaveField(new SavedField(fieldType.GetField(chronology.Calendar), value));
        }

        /// <summary>
        /// Saves a datetime field text value.
        /// </summary>
        /// <param name="field">The field type</param>
        /// <param name="text">The text value</param>
        /// <param name="provider">The format provider to use</param>
        public void SaveField(DateTimeFieldType fieldType, string text, IFormatProvider provider)
        {
            SaveField(new SavedField(fieldType.GetField(chronology.Calendar), text, provider));
        }

        private void SaveField(SavedField field)
        {
            SavedField[] savedFieldsLocal = savedFields;
            int savedFieldsCountLocal = savedFieldsCount;

            if (savedFieldsCountLocal == savedFieldsLocal.Length || savedFieldsShared)
            {
                // Expand capacity or merely copy if saved fields are shared.
                SavedField[] newArray = new SavedField
                    [savedFieldsCountLocal == savedFieldsLocal.Length ? savedFieldsCountLocal * 2 : savedFieldsLocal.Length];
                Array.Copy(savedFieldsLocal, newArray, savedFieldsCountLocal);

                savedFields = savedFieldsLocal = newArray;
                savedFieldsShared = false;
            }

            savedFieldsState = null;
            savedFields[savedFieldsCount] = field;
            savedFieldsCount = savedFieldsCountLocal + 1;
        }

        public Object SaveState()
        {
            return null;
        }

        public bool RestoreState(Object state)
        {
            return true;
        }
    }
}
