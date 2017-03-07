using JetBrains.Annotations;
using NodaTime.Utility;
using System;
using System.Collections.Generic;

namespace NodaTime.Calendars.Wondrous
{
    /// <summary>
    /// Helper and extension methods for the Wondrous Calendar.
    /// </summary>
    /// <remarks>
    /// The Wondrous Calendar Helper is in a distinct namespace so that the Extensions are only available when this
    /// namespace is included.
    /// </remarks>
    public static class WondrousCalendarHelper
    {
        private const int Month18 = 18;
        private const int DaysInMonth = 19;


        /// <remarks>
        ///     There are 19 months in a year. Between the 18th and 19th month are the "days of Ha" (Ayyam-i-Ha).
        ///     Options for numbering the months:
        ///     - treat Ayyam-i-Ha as month 0. This will cause problems if 0 is treated as unknown. When stored internally,
        ///       NodaTime months are converted to 0-based numbers, so this would by -1 which is not supported.
        ///     - treat Ayyam-i-Ha as month -1. This would be a problem if the base classes convert to 0 based months
        ///     - treat Ayyam-i-Ha as month 19. This is confusing because the 19th month would be displayed as month #20.
        ///     - treat Ayyam-i-Ha as month 20. This will cause problems if months are sorted naively.
        ///     - treat Ayyam-i-Ha as extra days in month 18. This would require special handling to sometimes display
        ///       days in "month 18" as being Ayyam-i-Ha, but we have no direct control over display.
        ///       
        /// This version will treat Ayyam-i-Ha days as extra days in month 18.
        /// 
        /// Users interacting with the system should use 0 as Ayyam-i-Ha.
        /// </remarks>
        internal const int AyyamiHaMonth0 = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wondrousYear">Year in the Wondrous calendar</param>
        /// <param name="wondrousMonth">Month (use 0 for Ayyam-i-Ha)</param>
        /// <param name="wondrousDay">Day in month</param>
        /// <returns></returns>
        public static LocalDate CreateDate(int wondrousYear, int wondrousMonth, int wondrousDay)
        {
            if (wondrousMonth == AyyamiHaMonth0)
            {
                var maxDay = GetYearInfo(wondrousYear).DaysInAyyamiHa;

                Preconditions.CheckArgumentRange(nameof(wondrousDay), wondrousDay, 1, maxDay);

                // move Ayyam-i-Ha days to fall after the last day of month 18.
                wondrousMonth = Month18;
                wondrousDay += DaysInMonth;
            }
            return new LocalDate(wondrousYear, wondrousMonth, wondrousDay, CalendarSystem.Wondrous);
        }

        /// <summary>
        /// Create a date in the Wondrous calendar.
        /// </summary>
        /// <param name="wondrousYear"></param>
        /// <param name="wondrousMonth"></param>
        /// <param name="wondrousDay"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static LocalDateTime CreateDateTime(int wondrousYear, int wondrousMonth, int wondrousDay, int hour, int minute = 0, int second = 0, int millisecond = 0)
        {
            if (wondrousMonth == AyyamiHaMonth0)
            {
                // move Ayyam-i-Ha days to fall after the last day of month 18.
                wondrousMonth = Month18;
                wondrousDay += DaysInMonth;
            }
            return new LocalDateTime(wondrousYear, wondrousMonth, wondrousDay, hour, minute, second, millisecond, CalendarSystem.Wondrous);
        }

        /// <summary>
        /// Return the month of this date. If in Ayyam-i-Ha, returns 0.
        /// </summary>
        /// <remarks>Deals with Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousMonthNum(this LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Wondrous, nameof(input), "Only valid when using the Wondrous calendar");

            if (input.Month == Month18 && input.Day > DaysInMonth)
            {
                return 0;
            }
            return input.Month;
        }

        /// <summary>
        /// Return the day of this month. 
        /// </summary>
        /// <remarks>Deals with days in Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousDayNum(this LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Wondrous, nameof(input), "Only valid when using the Wondrous calendar");

            if (input.Month == Month18 && input.Day > DaysInMonth)
            {
                return input.Day - DaysInMonth;
            }
            return input.Day;
        }


        /// <summary>
        /// Return the month of this date. If in Ayyam-i-Ha, returns 0.
        /// </summary>
        /// <remarks>Deals with Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousMonthNum(this LocalDateTime input)
        {
            return input.Date.WondrousMonthNum();
        }

        /// <summary>
        /// Return the day of this month. 
        /// </summary>
        /// <remarks>Deals with days in Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousDayNum(this LocalDateTime input)
        {
            return input.Date.WondrousDayNum();
        }

        //public static WondrousYearMonthDay AsWondrousYearMonthDay(this LocalDate input)
        //{
        //    return new WondrousYearMonthDay(
        //        input.Year,
        //        input.WondrousMonthNum(),
        //        input.WondrousDayNum());
        //}

        /// <summary>
        /// Get a text representation of the date.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format">TO BE IMPLEMENTED</param>
        /// <returns></returns>
        [NotNull] public static string AsWondrousString(this LocalDate input, [CanBeNull] string format = null)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Wondrous, nameof(input), "Only valid when using the Wondrous calendar");

            // to be enhanced...

            var year = input.Year;
            var month = input.WondrousMonthNum();
            var day = input.WondrousDayNum();

            return $"{year}-{month}-{day}";
        }

        internal static WondrousYearInfo GetYearInfo(int year)
        {
            switch (year)
            {
                case 1:
                    return new WondrousYearInfo(4, 21);
                case 2:
                    return new WondrousYearInfo(4, 21);
                case 3:
                    return new WondrousYearInfo(4, 21);
                case 4:
                    return new WondrousYearInfo(5, 21);
                case 5:
                    return new WondrousYearInfo(4, 21);
                case 6:
                    return new WondrousYearInfo(4, 21);
                case 7:
                    return new WondrousYearInfo(4, 21);
                case 8:
                    return new WondrousYearInfo(5, 21);
                case 9:
                    return new WondrousYearInfo(4, 21);
                case 10:
                    return new WondrousYearInfo(4, 21);
                case 11:
                    return new WondrousYearInfo(4, 21);
                case 12:
                    return new WondrousYearInfo(5, 21);
                case 13:
                    return new WondrousYearInfo(4, 21);
                case 14:
                    return new WondrousYearInfo(4, 21);
                case 15:
                    return new WondrousYearInfo(4, 21);
                case 16:
                    return new WondrousYearInfo(5, 21);
                case 17:
                    return new WondrousYearInfo(4, 21);
                case 18:
                    return new WondrousYearInfo(4, 21);
                case 19:
                    return new WondrousYearInfo(4, 21);
                case 20:
                    return new WondrousYearInfo(5, 21);
                case 21:
                    return new WondrousYearInfo(4, 21);
                case 22:
                    return new WondrousYearInfo(4, 21);
                case 23:
                    return new WondrousYearInfo(4, 21);
                case 24:
                    return new WondrousYearInfo(5, 21);
                case 25:
                    return new WondrousYearInfo(4, 21);
                case 26:
                    return new WondrousYearInfo(4, 21);
                case 27:
                    return new WondrousYearInfo(4, 21);
                case 28:
                    return new WondrousYearInfo(5, 21);
                case 29:
                    return new WondrousYearInfo(4, 21);
                case 30:
                    return new WondrousYearInfo(4, 21);
                case 31:
                    return new WondrousYearInfo(4, 21);
                case 32:
                    return new WondrousYearInfo(5, 21);
                case 33:
                    return new WondrousYearInfo(4, 21);
                case 34:
                    return new WondrousYearInfo(4, 21);
                case 35:
                    return new WondrousYearInfo(4, 21);
                case 36:
                    return new WondrousYearInfo(5, 21);
                case 37:
                    return new WondrousYearInfo(4, 21);
                case 38:
                    return new WondrousYearInfo(4, 21);
                case 39:
                    return new WondrousYearInfo(4, 21);
                case 40:
                    return new WondrousYearInfo(5, 21);
                case 41:
                    return new WondrousYearInfo(4, 21);
                case 42:
                    return new WondrousYearInfo(4, 21);
                case 43:
                    return new WondrousYearInfo(4, 21);
                case 44:
                    return new WondrousYearInfo(5, 21);
                case 45:
                    return new WondrousYearInfo(4, 21);
                case 46:
                    return new WondrousYearInfo(4, 21);
                case 47:
                    return new WondrousYearInfo(4, 21);
                case 48:
                    return new WondrousYearInfo(5, 21);
                case 49:
                    return new WondrousYearInfo(4, 21);
                case 50:
                    return new WondrousYearInfo(4, 21);
                case 51:
                    return new WondrousYearInfo(4, 21);
                case 52:
                    return new WondrousYearInfo(5, 21);
                case 53:
                    return new WondrousYearInfo(4, 21);
                case 54:
                    return new WondrousYearInfo(4, 21);
                case 55:
                    return new WondrousYearInfo(4, 21);
                case 56:
                    return new WondrousYearInfo(4, 21);
                case 57:
                    return new WondrousYearInfo(4, 21);
                case 58:
                    return new WondrousYearInfo(4, 21);
                case 59:
                    return new WondrousYearInfo(4, 21);
                case 60:
                    return new WondrousYearInfo(5, 21);
                case 61:
                    return new WondrousYearInfo(4, 21);
                case 62:
                    return new WondrousYearInfo(4, 21);
                case 63:
                    return new WondrousYearInfo(4, 21);
                case 64:
                    return new WondrousYearInfo(5, 21);
                case 65:
                    return new WondrousYearInfo(4, 21);
                case 66:
                    return new WondrousYearInfo(4, 21);
                case 67:
                    return new WondrousYearInfo(4, 21);
                case 68:
                    return new WondrousYearInfo(5, 21);
                case 69:
                    return new WondrousYearInfo(4, 21);
                case 70:
                    return new WondrousYearInfo(4, 21);
                case 71:
                    return new WondrousYearInfo(4, 21);
                case 72:
                    return new WondrousYearInfo(5, 21);
                case 73:
                    return new WondrousYearInfo(4, 21);
                case 74:
                    return new WondrousYearInfo(4, 21);
                case 75:
                    return new WondrousYearInfo(4, 21);
                case 76:
                    return new WondrousYearInfo(5, 21);
                case 77:
                    return new WondrousYearInfo(4, 21);
                case 78:
                    return new WondrousYearInfo(4, 21);
                case 79:
                    return new WondrousYearInfo(4, 21);
                case 80:
                    return new WondrousYearInfo(5, 21);
                case 81:
                    return new WondrousYearInfo(4, 21);
                case 82:
                    return new WondrousYearInfo(4, 21);
                case 83:
                    return new WondrousYearInfo(4, 21);
                case 84:
                    return new WondrousYearInfo(5, 21);
                case 85:
                    return new WondrousYearInfo(4, 21);
                case 86:
                    return new WondrousYearInfo(4, 21);
                case 87:
                    return new WondrousYearInfo(4, 21);
                case 88:
                    return new WondrousYearInfo(5, 21);
                case 89:
                    return new WondrousYearInfo(4, 21);
                case 90:
                    return new WondrousYearInfo(4, 21);
                case 91:
                    return new WondrousYearInfo(4, 21);
                case 92:
                    return new WondrousYearInfo(5, 21);
                case 93:
                    return new WondrousYearInfo(4, 21);
                case 94:
                    return new WondrousYearInfo(4, 21);
                case 95:
                    return new WondrousYearInfo(4, 21);
                case 96:
                    return new WondrousYearInfo(5, 21);
                case 97:
                    return new WondrousYearInfo(4, 21);
                case 98:
                    return new WondrousYearInfo(4, 21);
                case 99:
                    return new WondrousYearInfo(4, 21);
                case 100:
                    return new WondrousYearInfo(5, 21);
                case 101:
                    return new WondrousYearInfo(4, 21);
                case 102:
                    return new WondrousYearInfo(4, 21);
                case 103:
                    return new WondrousYearInfo(4, 21);
                case 104:
                    return new WondrousYearInfo(5, 21);
                case 105:
                    return new WondrousYearInfo(4, 21);
                case 106:
                    return new WondrousYearInfo(4, 21);
                case 107:
                    return new WondrousYearInfo(4, 21);
                case 108:
                    return new WondrousYearInfo(5, 21);
                case 109:
                    return new WondrousYearInfo(4, 21);
                case 110:
                    return new WondrousYearInfo(4, 21);
                case 111:
                    return new WondrousYearInfo(4, 21);
                case 112:
                    return new WondrousYearInfo(5, 21);
                case 113:
                    return new WondrousYearInfo(4, 21);
                case 114:
                    return new WondrousYearInfo(4, 21);
                case 115:
                    return new WondrousYearInfo(4, 21);
                case 116:
                    return new WondrousYearInfo(5, 21);
                case 117:
                    return new WondrousYearInfo(4, 21);
                case 118:
                    return new WondrousYearInfo(4, 21);
                case 119:
                    return new WondrousYearInfo(4, 21);
                case 120:
                    return new WondrousYearInfo(5, 21);
                case 121:
                    return new WondrousYearInfo(4, 21);
                case 122:
                    return new WondrousYearInfo(4, 21);
                case 123:
                    return new WondrousYearInfo(4, 21);
                case 124:
                    return new WondrousYearInfo(5, 21);
                case 125:
                    return new WondrousYearInfo(4, 21);
                case 126:
                    return new WondrousYearInfo(4, 21);
                case 127:
                    return new WondrousYearInfo(4, 21);
                case 128:
                    return new WondrousYearInfo(5, 21);
                case 129:
                    return new WondrousYearInfo(4, 21);
                case 130:
                    return new WondrousYearInfo(4, 21);
                case 131:
                    return new WondrousYearInfo(4, 21);
                case 132:
                    return new WondrousYearInfo(5, 21);
                case 133:
                    return new WondrousYearInfo(4, 21);
                case 134:
                    return new WondrousYearInfo(4, 21);
                case 135:
                    return new WondrousYearInfo(4, 21);
                case 136:
                    return new WondrousYearInfo(5, 21);
                case 137:
                    return new WondrousYearInfo(4, 21);
                case 138:
                    return new WondrousYearInfo(4, 21);
                case 139:
                    return new WondrousYearInfo(4, 21);
                case 140:
                    return new WondrousYearInfo(5, 21);
                case 141:
                    return new WondrousYearInfo(4, 21);
                case 142:
                    return new WondrousYearInfo(4, 21);
                case 143:
                    return new WondrousYearInfo(4, 21);
                case 144:
                    return new WondrousYearInfo(5, 21);
                case 145:
                    return new WondrousYearInfo(4, 21);
                case 146:
                    return new WondrousYearInfo(4, 21);
                case 147:
                    return new WondrousYearInfo(4, 21);
                case 148:
                    return new WondrousYearInfo(5, 21);
                case 149:
                    return new WondrousYearInfo(4, 21);
                case 150:
                    return new WondrousYearInfo(4, 21);
                case 151:
                    return new WondrousYearInfo(4, 21);
                case 152:
                    return new WondrousYearInfo(5, 21);
                case 153:
                    return new WondrousYearInfo(4, 21);
                case 154:
                    return new WondrousYearInfo(4, 21);
                case 155:
                    return new WondrousYearInfo(4, 21);
                case 156:
                    return new WondrousYearInfo(5, 21);
                case 157:
                    return new WondrousYearInfo(4, 21);
                case 158:
                    return new WondrousYearInfo(4, 21);
                case 159:
                    return new WondrousYearInfo(4, 21);
                case 160:
                    return new WondrousYearInfo(5, 21);
                case 161:
                    return new WondrousYearInfo(4, 21);
                case 162:
                    return new WondrousYearInfo(4, 21);
                case 163:
                    return new WondrousYearInfo(4, 21);
                case 164:
                    return new WondrousYearInfo(5, 21);
                case 165:
                    return new WondrousYearInfo(4, 21);
                case 166:
                    return new WondrousYearInfo(4, 21);
                case 167:
                    return new WondrousYearInfo(4, 21);
                case 168:
                    return new WondrousYearInfo(5, 21);
                case 169:
                    return new WondrousYearInfo(4, 21);
                case 170:
                    return new WondrousYearInfo(4, 21);
                case 171:
                    return new WondrousYearInfo(4, 21);
                case 172:
                    return new WondrousYearInfo(4, 21);
                case 173:
                    return new WondrousYearInfo(4, 20);
                case 174:
                    return new WondrousYearInfo(5, 20);
                case 175:
                    return new WondrousYearInfo(4, 21);
                case 176:
                    return new WondrousYearInfo(4, 21);
                case 177:
                    return new WondrousYearInfo(4, 20);
                case 178:
                    return new WondrousYearInfo(5, 20);
                case 179:
                    return new WondrousYearInfo(4, 21);
                case 180:
                    return new WondrousYearInfo(4, 21);
                case 181:
                    return new WondrousYearInfo(4, 20);
                case 182:
                    return new WondrousYearInfo(5, 20);
                case 183:
                    return new WondrousYearInfo(4, 21);
                case 184:
                    return new WondrousYearInfo(4, 21);
                case 185:
                    return new WondrousYearInfo(4, 20);
                case 186:
                    return new WondrousYearInfo(4, 20);
                case 187:
                    return new WondrousYearInfo(5, 20);
                case 188:
                    return new WondrousYearInfo(4, 21);
                case 189:
                    return new WondrousYearInfo(4, 20);
                case 190:
                    return new WondrousYearInfo(4, 20);
                case 191:
                    return new WondrousYearInfo(5, 20);
                case 192:
                    return new WondrousYearInfo(4, 21);
                case 193:
                    return new WondrousYearInfo(4, 20);
                case 194:
                    return new WondrousYearInfo(4, 20);
                case 195:
                    return new WondrousYearInfo(5, 20);
                case 196:
                    return new WondrousYearInfo(4, 21);
                case 197:
                    return new WondrousYearInfo(4, 20);
                case 198:
                    return new WondrousYearInfo(4, 20);
                case 199:
                    return new WondrousYearInfo(5, 20);
                case 200:
                    return new WondrousYearInfo(4, 21);
                case 201:
                    return new WondrousYearInfo(4, 20);
                case 202:
                    return new WondrousYearInfo(4, 20);
                case 203:
                    return new WondrousYearInfo(5, 20);
                case 204:
                    return new WondrousYearInfo(4, 21);
                case 205:
                    return new WondrousYearInfo(4, 20);
                case 206:
                    return new WondrousYearInfo(4, 20);
                case 207:
                    return new WondrousYearInfo(5, 20);
                case 208:
                    return new WondrousYearInfo(4, 21);
                case 209:
                    return new WondrousYearInfo(4, 20);
                case 210:
                    return new WondrousYearInfo(4, 20);
                case 211:
                    return new WondrousYearInfo(5, 20);
                case 212:
                    return new WondrousYearInfo(4, 21);
                case 213:
                    return new WondrousYearInfo(4, 20);
                case 214:
                    return new WondrousYearInfo(4, 20);
                case 215:
                    return new WondrousYearInfo(4, 20);
                case 216:
                    return new WondrousYearInfo(5, 20);
                case 217:
                    return new WondrousYearInfo(4, 20);
                case 218:
                    return new WondrousYearInfo(4, 20);
                case 219:
                    return new WondrousYearInfo(4, 20);
                case 220:
                    return new WondrousYearInfo(5, 20);
                case 221:
                    return new WondrousYearInfo(4, 20);
                case 222:
                    return new WondrousYearInfo(4, 20);
                case 223:
                    return new WondrousYearInfo(4, 20);
                case 224:
                    return new WondrousYearInfo(5, 20);
                case 225:
                    return new WondrousYearInfo(4, 20);
                case 226:
                    return new WondrousYearInfo(4, 20);
                case 227:
                    return new WondrousYearInfo(4, 20);
                case 228:
                    return new WondrousYearInfo(5, 20);
                case 229:
                    return new WondrousYearInfo(4, 20);
                case 230:
                    return new WondrousYearInfo(4, 20);
                case 231:
                    return new WondrousYearInfo(4, 20);
                case 232:
                    return new WondrousYearInfo(5, 20);
                case 233:
                    return new WondrousYearInfo(4, 20);
                case 234:
                    return new WondrousYearInfo(4, 20);
                case 235:
                    return new WondrousYearInfo(4, 20);
                case 236:
                    return new WondrousYearInfo(5, 20);
                case 237:
                    return new WondrousYearInfo(4, 20);
                case 238:
                    return new WondrousYearInfo(4, 20);
                case 239:
                    return new WondrousYearInfo(4, 20);
                case 240:
                    return new WondrousYearInfo(5, 20);
                case 241:
                    return new WondrousYearInfo(4, 20);
                case 242:
                    return new WondrousYearInfo(4, 20);
                case 243:
                    return new WondrousYearInfo(4, 20);
                case 244:
                    return new WondrousYearInfo(5, 20);
                case 245:
                    return new WondrousYearInfo(4, 20);
                case 246:
                    return new WondrousYearInfo(4, 20);
                case 247:
                    return new WondrousYearInfo(4, 20);
                case 248:
                    return new WondrousYearInfo(4, 20);
                case 249:
                    return new WondrousYearInfo(5, 19);
                case 250:
                    return new WondrousYearInfo(4, 20);
                case 251:
                    return new WondrousYearInfo(4, 20);
                case 252:
                    return new WondrousYearInfo(4, 20);
                case 253:
                    return new WondrousYearInfo(5, 19);
                case 254:
                    return new WondrousYearInfo(4, 20);
                case 255:
                    return new WondrousYearInfo(4, 20);
                case 256:
                    return new WondrousYearInfo(4, 20);
                case 257:
                    return new WondrousYearInfo(5, 20);
                case 258:
                    return new WondrousYearInfo(4, 21);
                case 259:
                    return new WondrousYearInfo(4, 21);
                case 260:
                    return new WondrousYearInfo(4, 21);
                case 261:
                    return new WondrousYearInfo(5, 20);
                case 262:
                    return new WondrousYearInfo(4, 21);
                case 263:
                    return new WondrousYearInfo(4, 21);
                case 264:
                    return new WondrousYearInfo(4, 21);
                case 265:
                    return new WondrousYearInfo(5, 20);
                case 266:
                    return new WondrousYearInfo(4, 21);
                case 267:
                    return new WondrousYearInfo(4, 21);
                case 268:
                    return new WondrousYearInfo(4, 21);
                case 269:
                    return new WondrousYearInfo(5, 20);
                case 270:
                    return new WondrousYearInfo(4, 21);
                case 271:
                    return new WondrousYearInfo(4, 21);
                case 272:
                    return new WondrousYearInfo(4, 21);
                case 273:
                    return new WondrousYearInfo(5, 20);
                case 274:
                    return new WondrousYearInfo(4, 21);
                case 275:
                    return new WondrousYearInfo(4, 21);
                case 276:
                    return new WondrousYearInfo(4, 21);
                case 277:
                    return new WondrousYearInfo(5, 20);
                case 278:
                    return new WondrousYearInfo(4, 21);
                case 279:
                    return new WondrousYearInfo(4, 21);
                case 280:
                    return new WondrousYearInfo(4, 21);
                case 281:
                    return new WondrousYearInfo(4, 20);
                case 282:
                    return new WondrousYearInfo(5, 20);
                case 283:
                    return new WondrousYearInfo(4, 21);
                case 284:
                    return new WondrousYearInfo(4, 21);
                case 285:
                    return new WondrousYearInfo(4, 20);
                case 286:
                    return new WondrousYearInfo(5, 20);
                case 287:
                    return new WondrousYearInfo(4, 21);
                case 288:
                    return new WondrousYearInfo(4, 21);
                case 289:
                    return new WondrousYearInfo(4, 20);
                case 290:
                    return new WondrousYearInfo(5, 20);
                case 291:
                    return new WondrousYearInfo(4, 21);
                case 292:
                    return new WondrousYearInfo(4, 21);
                case 293:
                    return new WondrousYearInfo(4, 20);
                case 294:
                    return new WondrousYearInfo(5, 20);
                case 295:
                    return new WondrousYearInfo(4, 21);
                case 296:
                    return new WondrousYearInfo(4, 21);
                case 297:
                    return new WondrousYearInfo(4, 20);
                case 298:
                    return new WondrousYearInfo(5, 20);
                case 299:
                    return new WondrousYearInfo(4, 21);
                case 300:
                    return new WondrousYearInfo(4, 21);
                case 301:
                    return new WondrousYearInfo(4, 20);
                case 302:
                    return new WondrousYearInfo(5, 20);
                case 303:
                    return new WondrousYearInfo(4, 21);
                case 304:
                    return new WondrousYearInfo(4, 21);
                case 305:
                    return new WondrousYearInfo(4, 20);
                case 306:
                    return new WondrousYearInfo(5, 20);
                case 307:
                    return new WondrousYearInfo(4, 21);
                case 308:
                    return new WondrousYearInfo(4, 21);
                case 309:
                    return new WondrousYearInfo(4, 20);
                case 310:
                    return new WondrousYearInfo(5, 20);
                case 311:
                    return new WondrousYearInfo(4, 21);
                case 312:
                    return new WondrousYearInfo(4, 21);
                case 313:
                    return new WondrousYearInfo(4, 20);
                case 314:
                    return new WondrousYearInfo(4, 20);
                case 315:
                    return new WondrousYearInfo(5, 20);
                case 316:
                    return new WondrousYearInfo(4, 21);
                case 317:
                    return new WondrousYearInfo(4, 20);
                case 318:
                    return new WondrousYearInfo(4, 20);
                case 319:
                    return new WondrousYearInfo(5, 20);
                case 320:
                    return new WondrousYearInfo(4, 21);
                case 321:
                    return new WondrousYearInfo(4, 20);
                case 322:
                    return new WondrousYearInfo(4, 20);
                case 323:
                    return new WondrousYearInfo(5, 20);
                case 324:
                    return new WondrousYearInfo(4, 21);
                case 325:
                    return new WondrousYearInfo(4, 20);
                case 326:
                    return new WondrousYearInfo(4, 20);
                case 327:
                    return new WondrousYearInfo(5, 20);
                case 328:
                    return new WondrousYearInfo(4, 21);
                case 329:
                    return new WondrousYearInfo(4, 20);
                case 330:
                    return new WondrousYearInfo(4, 20);
                case 331:
                    return new WondrousYearInfo(5, 20);
                case 332:
                    return new WondrousYearInfo(4, 21);
                case 333:
                    return new WondrousYearInfo(4, 20);
                case 334:
                    return new WondrousYearInfo(4, 20);
                case 335:
                    return new WondrousYearInfo(5, 20);
                case 336:
                    return new WondrousYearInfo(4, 21);
                case 337:
                    return new WondrousYearInfo(4, 20);
                case 338:
                    return new WondrousYearInfo(4, 20);
                case 339:
                    return new WondrousYearInfo(5, 20);
                case 340:
                    return new WondrousYearInfo(4, 21);
                case 341:
                    return new WondrousYearInfo(4, 20);
                case 342:
                    return new WondrousYearInfo(4, 20);
                case 343:
                    return new WondrousYearInfo(5, 20);
                case 344:
                    return new WondrousYearInfo(4, 21);
                case 345:
                    return new WondrousYearInfo(4, 20);
                case 346:
                    return new WondrousYearInfo(4, 20);
                case 347:
                    return new WondrousYearInfo(4, 20);
                case 348:
                    return new WondrousYearInfo(5, 20);
                case 349:
                    return new WondrousYearInfo(4, 20);
                case 350:
                    return new WondrousYearInfo(4, 20);
                case 351:
                    return new WondrousYearInfo(4, 20);
                case 352:
                    return new WondrousYearInfo(5, 20);
                case 353:
                    return new WondrousYearInfo(4, 20);
                case 354:
                    return new WondrousYearInfo(4, 20);
                case 355:
                    return new WondrousYearInfo(4, 20);
                case 356:
                    return new WondrousYearInfo(5, 20);
                case 357:
                    return new WondrousYearInfo(4, 21);
                case 358:
                    return new WondrousYearInfo(4, 21);
                case 359:
                    return new WondrousYearInfo(4, 21);
                case 360:
                    return new WondrousYearInfo(5, 21);
                case 361:
                    return new WondrousYearInfo(4, 21);
                case 362:
                    return new WondrousYearInfo(4, 21);
                case 363:
                    return new WondrousYearInfo(4, 21);
                case 364:
                    return new WondrousYearInfo(5, 21);
                case 365:
                    return new WondrousYearInfo(4, 21);
                case 366:
                    return new WondrousYearInfo(4, 21);
                case 367:
                    return new WondrousYearInfo(4, 21);
                case 368:
                    return new WondrousYearInfo(5, 21);
                case 369:
                    return new WondrousYearInfo(4, 21);
                case 370:
                    return new WondrousYearInfo(4, 21);
                case 371:
                    return new WondrousYearInfo(4, 21);
                case 372:
                    return new WondrousYearInfo(5, 21);
                case 373:
                    return new WondrousYearInfo(4, 21);
                case 374:
                    return new WondrousYearInfo(4, 21);
                case 375:
                    return new WondrousYearInfo(4, 21);
                case 376:
                    return new WondrousYearInfo(5, 21);
                case 377:
                    return new WondrousYearInfo(4, 21);
                case 378:
                    return new WondrousYearInfo(4, 21);
                case 379:
                    return new WondrousYearInfo(4, 21);
                case 380:
                    return new WondrousYearInfo(4, 21);
                case 381:
                    return new WondrousYearInfo(5, 20);
                case 382:
                    return new WondrousYearInfo(4, 21);
                case 383:
                    return new WondrousYearInfo(4, 21);
                case 384:
                    return new WondrousYearInfo(4, 21);
                case 385:
                    return new WondrousYearInfo(5, 20);
                case 386:
                    return new WondrousYearInfo(4, 21);
                case 387:
                    return new WondrousYearInfo(4, 21);
                case 388:
                    return new WondrousYearInfo(4, 21);
                case 389:
                    return new WondrousYearInfo(5, 20);
                case 390:
                    return new WondrousYearInfo(4, 21);
                case 391:
                    return new WondrousYearInfo(4, 21);
                case 392:
                    return new WondrousYearInfo(4, 21);
                case 393:
                    return new WondrousYearInfo(5, 20);
                case 394:
                    return new WondrousYearInfo(4, 21);
                case 395:
                    return new WondrousYearInfo(4, 21);
                case 396:
                    return new WondrousYearInfo(4, 21);
                case 397:
                    return new WondrousYearInfo(5, 20);
                case 398:
                    return new WondrousYearInfo(4, 21);
                case 399:
                    return new WondrousYearInfo(4, 21);
                case 400:
                    return new WondrousYearInfo(4, 21);
                case 401:
                    return new WondrousYearInfo(5, 20);
                case 402:
                    return new WondrousYearInfo(4, 21);
                case 403:
                    return new WondrousYearInfo(4, 21);
                case 404:
                    return new WondrousYearInfo(4, 21);
                case 405:
                    return new WondrousYearInfo(5, 20);
                case 406:
                    return new WondrousYearInfo(4, 21);
                case 407:
                    return new WondrousYearInfo(4, 21);
                case 408:
                    return new WondrousYearInfo(4, 21);
                case 409:
                    return new WondrousYearInfo(5, 20);
                case 410:
                    return new WondrousYearInfo(4, 21);
                case 411:
                    return new WondrousYearInfo(4, 21);
                case 412:
                    return new WondrousYearInfo(4, 21);
                case 413:
                    return new WondrousYearInfo(4, 20);
                case 414:
                    return new WondrousYearInfo(5, 20);
                case 415:
                    return new WondrousYearInfo(4, 21);
                case 416:
                    return new WondrousYearInfo(4, 21);
                case 417:
                    return new WondrousYearInfo(4, 20);
                case 418:
                    return new WondrousYearInfo(5, 20);
                case 419:
                    return new WondrousYearInfo(4, 21);
                case 420:
                    return new WondrousYearInfo(4, 21);
                case 421:
                    return new WondrousYearInfo(4, 20);
                case 422:
                    return new WondrousYearInfo(5, 20);
                case 423:
                    return new WondrousYearInfo(4, 21);
                case 424:
                    return new WondrousYearInfo(4, 21);
                case 425:
                    return new WondrousYearInfo(4, 20);
                case 426:
                    return new WondrousYearInfo(5, 20);
                case 427:
                    return new WondrousYearInfo(4, 21);
                case 428:
                    return new WondrousYearInfo(4, 21);
                case 429:
                    return new WondrousYearInfo(4, 20);
                case 430:
                    return new WondrousYearInfo(5, 20);
                case 431:
                    return new WondrousYearInfo(4, 21);
                case 432:
                    return new WondrousYearInfo(4, 21);
                case 433:
                    return new WondrousYearInfo(4, 20);
                case 434:
                    return new WondrousYearInfo(5, 20);
                case 435:
                    return new WondrousYearInfo(4, 21);
                case 436:
                    return new WondrousYearInfo(4, 21);
                case 437:
                    return new WondrousYearInfo(4, 20);
                case 438:
                    return new WondrousYearInfo(5, 20);
                case 439:
                    return new WondrousYearInfo(4, 21);
                case 440:
                    return new WondrousYearInfo(4, 21);
                case 441:
                    return new WondrousYearInfo(4, 20);
                case 442:
                    return new WondrousYearInfo(5, 20);
                case 443:
                    return new WondrousYearInfo(4, 21);
                case 444:
                    return new WondrousYearInfo(4, 21);
                case 445:
                    return new WondrousYearInfo(4, 20);
                case 446:
                    return new WondrousYearInfo(4, 20);
                case 447:
                    return new WondrousYearInfo(5, 20);
                case 448:
                    return new WondrousYearInfo(4, 21);
                case 449:
                    return new WondrousYearInfo(4, 20);
                case 450:
                    return new WondrousYearInfo(4, 20);
                case 451:
                    return new WondrousYearInfo(5, 20);
                case 452:
                    return new WondrousYearInfo(4, 21);
                case 453:
                    return new WondrousYearInfo(4, 20);
                case 454:
                    return new WondrousYearInfo(4, 20);
                case 455:
                    return new WondrousYearInfo(5, 20);
                case 456:
                    return new WondrousYearInfo(4, 21);
                case 457:
                    return new WondrousYearInfo(4, 21);
                case 458:
                    return new WondrousYearInfo(4, 21);
                case 459:
                    return new WondrousYearInfo(5, 21);
                case 460:
                    return new WondrousYearInfo(4, 22);
                case 461:
                    return new WondrousYearInfo(4, 21);
                case 462:
                    return new WondrousYearInfo(4, 21);
                case 463:
                    return new WondrousYearInfo(5, 21);
                case 464:
                    return new WondrousYearInfo(4, 22);
                case 465:
                    return new WondrousYearInfo(4, 21);
                case 466:
                    return new WondrousYearInfo(4, 21);
                case 467:
                    return new WondrousYearInfo(5, 21);
                case 468:
                    return new WondrousYearInfo(4, 22);
                case 469:
                    return new WondrousYearInfo(4, 21);
                case 470:
                    return new WondrousYearInfo(4, 21);
                case 471:
                    return new WondrousYearInfo(5, 21);
                case 472:
                    return new WondrousYearInfo(4, 22);
                case 473:
                    return new WondrousYearInfo(4, 21);
                case 474:
                    return new WondrousYearInfo(4, 21);
                case 475:
                    return new WondrousYearInfo(5, 21);
                case 476:
                    return new WondrousYearInfo(4, 22);
                case 477:
                    return new WondrousYearInfo(4, 21);
                case 478:
                    return new WondrousYearInfo(4, 21);
                case 479:
                    return new WondrousYearInfo(4, 21);
                case 480:
                    return new WondrousYearInfo(5, 21);
                case 481:
                    return new WondrousYearInfo(4, 21);
                case 482:
                    return new WondrousYearInfo(4, 21);
                case 483:
                    return new WondrousYearInfo(4, 21);
                case 484:
                    return new WondrousYearInfo(5, 21);
                case 485:
                    return new WondrousYearInfo(4, 21);
                case 486:
                    return new WondrousYearInfo(4, 21);
                case 487:
                    return new WondrousYearInfo(4, 21);
                case 488:
                    return new WondrousYearInfo(5, 21);
                case 489:
                    return new WondrousYearInfo(4, 21);
                case 490:
                    return new WondrousYearInfo(4, 21);
                case 491:
                    return new WondrousYearInfo(4, 21);
                case 492:
                    return new WondrousYearInfo(5, 21);
                case 493:
                    return new WondrousYearInfo(4, 21);
                case 494:
                    return new WondrousYearInfo(4, 21);
                case 495:
                    return new WondrousYearInfo(4, 21);
                case 496:
                    return new WondrousYearInfo(5, 21);
                case 497:
                    return new WondrousYearInfo(4, 21);
                case 498:
                    return new WondrousYearInfo(4, 21);
                case 499:
                    return new WondrousYearInfo(4, 21);
                case 500:
                    return new WondrousYearInfo(5, 21);
                case 501:
                    return new WondrousYearInfo(4, 21);
                case 502:
                    return new WondrousYearInfo(4, 21);
                case 503:
                    return new WondrousYearInfo(4, 21);
                case 504:
                    return new WondrousYearInfo(5, 21);
                case 505:
                    return new WondrousYearInfo(4, 21);
                case 506:
                    return new WondrousYearInfo(4, 21);
                case 507:
                    return new WondrousYearInfo(4, 21);
                case 508:
                    return new WondrousYearInfo(5, 21);
                case 509:
                    return new WondrousYearInfo(4, 21);
                case 510:
                    return new WondrousYearInfo(4, 21);
                case 511:
                    return new WondrousYearInfo(4, 21);
                case 512:
                    return new WondrousYearInfo(4, 21);
                case 513:
                    return new WondrousYearInfo(5, 20);
                case 514:
                    return new WondrousYearInfo(4, 21);
                case 515:
                    return new WondrousYearInfo(4, 21);
                case 516:
                    return new WondrousYearInfo(4, 21);
                case 517:
                    return new WondrousYearInfo(5, 20);
                case 518:
                    return new WondrousYearInfo(4, 21);
                case 519:
                    return new WondrousYearInfo(4, 21);
                case 520:
                    return new WondrousYearInfo(4, 21);
                case 521:
                    return new WondrousYearInfo(5, 20);
                case 522:
                    return new WondrousYearInfo(4, 21);
                case 523:
                    return new WondrousYearInfo(4, 21);
                case 524:
                    return new WondrousYearInfo(4, 21);
                case 525:
                    return new WondrousYearInfo(5, 20);
                case 526:
                    return new WondrousYearInfo(4, 21);
                case 527:
                    return new WondrousYearInfo(4, 21);
                case 528:
                    return new WondrousYearInfo(4, 21);
                case 529:
                    return new WondrousYearInfo(5, 20);
                case 530:
                    return new WondrousYearInfo(4, 21);
                case 531:
                    return new WondrousYearInfo(4, 21);
                case 532:
                    return new WondrousYearInfo(4, 21);
                case 533:
                    return new WondrousYearInfo(5, 20);
                case 534:
                    return new WondrousYearInfo(4, 21);
                case 535:
                    return new WondrousYearInfo(4, 21);
                case 536:
                    return new WondrousYearInfo(4, 21);
                case 537:
                    return new WondrousYearInfo(5, 20);
                case 538:
                    return new WondrousYearInfo(4, 21);
                case 539:
                    return new WondrousYearInfo(4, 21);
                case 540:
                    return new WondrousYearInfo(4, 21);
                case 541:
                    return new WondrousYearInfo(4, 20);
                case 542:
                    return new WondrousYearInfo(5, 20);
                case 543:
                    return new WondrousYearInfo(4, 21);
                case 544:
                    return new WondrousYearInfo(4, 21);
                case 545:
                    return new WondrousYearInfo(4, 20);
                case 546:
                    return new WondrousYearInfo(5, 20);
                case 547:
                    return new WondrousYearInfo(4, 21);
                case 548:
                    return new WondrousYearInfo(4, 21);
                case 549:
                    return new WondrousYearInfo(4, 20);
                case 550:
                    return new WondrousYearInfo(5, 20);
                case 551:
                    return new WondrousYearInfo(4, 21);
                case 552:
                    return new WondrousYearInfo(4, 21);
                case 553:
                    return new WondrousYearInfo(4, 20);
                case 554:
                    return new WondrousYearInfo(5, 20);
                case 555:
                    return new WondrousYearInfo(4, 21);
                case 556:
                    return new WondrousYearInfo(4, 21);
                case 557:
                    return new WondrousYearInfo(4, 20);
                case 558:
                    return new WondrousYearInfo(5, 20);
                case 559:
                    return new WondrousYearInfo(4, 21);
                case 560:
                    return new WondrousYearInfo(4, 21);
                case 561:
                    return new WondrousYearInfo(4, 20);
                case 562:
                    return new WondrousYearInfo(5, 20);
                case 563:
                    return new WondrousYearInfo(4, 21);
                case 564:
                    return new WondrousYearInfo(4, 21);
                case 565:
                    return new WondrousYearInfo(4, 20);
                case 566:
                    return new WondrousYearInfo(5, 20);
                case 567:
                    return new WondrousYearInfo(4, 21);
                case 568:
                    return new WondrousYearInfo(4, 21);
                case 569:
                    return new WondrousYearInfo(4, 20);
                case 570:
                    return new WondrousYearInfo(5, 20);
                case 571:
                    return new WondrousYearInfo(4, 21);
                case 572:
                    return new WondrousYearInfo(4, 21);
                case 573:
                    return new WondrousYearInfo(4, 20);
                case 574:
                    return new WondrousYearInfo(5, 20);
                case 575:
                    return new WondrousYearInfo(4, 21);
                case 576:
                    return new WondrousYearInfo(4, 21);
                case 577:
                    return new WondrousYearInfo(4, 20);
                case 578:
                    return new WondrousYearInfo(4, 20);
                case 579:
                    return new WondrousYearInfo(5, 20);
                case 580:
                    return new WondrousYearInfo(4, 21);
                case 581:
                    return new WondrousYearInfo(4, 20);
                case 582:
                    return new WondrousYearInfo(4, 20);
                case 583:
                    return new WondrousYearInfo(5, 20);
                case 584:
                    return new WondrousYearInfo(4, 21);
                case 585:
                    return new WondrousYearInfo(4, 20);
                case 586:
                    return new WondrousYearInfo(4, 20);
                case 587:
                    return new WondrousYearInfo(5, 20);
                case 588:
                    return new WondrousYearInfo(4, 21);
                case 589:
                    return new WondrousYearInfo(4, 20);
                case 590:
                    return new WondrousYearInfo(4, 20);
                case 591:
                    return new WondrousYearInfo(5, 20);
                case 592:
                    return new WondrousYearInfo(4, 21);
                case 593:
                    return new WondrousYearInfo(4, 20);
                case 594:
                    return new WondrousYearInfo(4, 20);
                case 595:
                    return new WondrousYearInfo(5, 20);
                case 596:
                    return new WondrousYearInfo(4, 21);
                case 597:
                    return new WondrousYearInfo(4, 20);
                case 598:
                    return new WondrousYearInfo(4, 20);
                case 599:
                    return new WondrousYearInfo(5, 20);
                case 600:
                    return new WondrousYearInfo(4, 21);
                case 601:
                    return new WondrousYearInfo(4, 20);
                case 602:
                    return new WondrousYearInfo(4, 20);
                case 603:
                    return new WondrousYearInfo(5, 20);
                case 604:
                    return new WondrousYearInfo(4, 21);
                case 605:
                    return new WondrousYearInfo(4, 20);
                case 606:
                    return new WondrousYearInfo(4, 20);
                case 607:
                    return new WondrousYearInfo(5, 20);
                case 608:
                    return new WondrousYearInfo(4, 21);
                case 609:
                    return new WondrousYearInfo(4, 20);
                case 610:
                    return new WondrousYearInfo(4, 20);
                case 611:
                    return new WondrousYearInfo(4, 20);
                case 612:
                    return new WondrousYearInfo(5, 20);
                case 613:
                    return new WondrousYearInfo(4, 20);
                case 614:
                    return new WondrousYearInfo(4, 20);
                case 615:
                    return new WondrousYearInfo(4, 20);
                case 616:
                    return new WondrousYearInfo(5, 20);
                case 617:
                    return new WondrousYearInfo(4, 20);
                case 618:
                    return new WondrousYearInfo(4, 20);
                case 619:
                    return new WondrousYearInfo(4, 20);
                case 620:
                    return new WondrousYearInfo(5, 20);
                case 621:
                    return new WondrousYearInfo(4, 20);
                case 622:
                    return new WondrousYearInfo(4, 20);
                case 623:
                    return new WondrousYearInfo(4, 20);
                case 624:
                    return new WondrousYearInfo(5, 20);
                case 625:
                    return new WondrousYearInfo(4, 20);
                case 626:
                    return new WondrousYearInfo(4, 20);
                case 627:
                    return new WondrousYearInfo(4, 20);
                case 628:
                    return new WondrousYearInfo(5, 20);
                case 629:
                    return new WondrousYearInfo(4, 20);
                case 630:
                    return new WondrousYearInfo(4, 20);
                case 631:
                    return new WondrousYearInfo(4, 20);
                case 632:
                    return new WondrousYearInfo(5, 20);
                case 633:
                    return new WondrousYearInfo(4, 20);
                case 634:
                    return new WondrousYearInfo(4, 20);
                case 635:
                    return new WondrousYearInfo(4, 20);
                case 636:
                    return new WondrousYearInfo(5, 20);
                case 637:
                    return new WondrousYearInfo(4, 20);
                case 638:
                    return new WondrousYearInfo(4, 20);
                case 639:
                    return new WondrousYearInfo(4, 20);
                case 640:
                    return new WondrousYearInfo(5, 20);
                case 641:
                    return new WondrousYearInfo(4, 20);
                case 642:
                    return new WondrousYearInfo(4, 20);
                case 643:
                    return new WondrousYearInfo(4, 20);
                case 644:
                    return new WondrousYearInfo(4, 20);
                case 645:
                    return new WondrousYearInfo(5, 19);
                case 646:
                    return new WondrousYearInfo(4, 20);
                case 647:
                    return new WondrousYearInfo(4, 20);
                case 648:
                    return new WondrousYearInfo(4, 20);
                case 649:
                    return new WondrousYearInfo(5, 19);
                case 650:
                    return new WondrousYearInfo(4, 20);
                case 651:
                    return new WondrousYearInfo(4, 20);
                case 652:
                    return new WondrousYearInfo(4, 20);
                case 653:
                    return new WondrousYearInfo(5, 19);
                case 654:
                    return new WondrousYearInfo(4, 20);
                case 655:
                    return new WondrousYearInfo(4, 20);
                case 656:
                    return new WondrousYearInfo(4, 20);
                case 657:
                    return new WondrousYearInfo(5, 20);
                case 658:
                    return new WondrousYearInfo(4, 21);
                case 659:
                    return new WondrousYearInfo(4, 21);
                case 660:
                    return new WondrousYearInfo(4, 21);
                case 661:
                    return new WondrousYearInfo(5, 20);
                case 662:
                    return new WondrousYearInfo(4, 21);
                case 663:
                    return new WondrousYearInfo(4, 21);
                case 664:
                    return new WondrousYearInfo(4, 21);
                case 665:
                    return new WondrousYearInfo(5, 20);
                case 666:
                    return new WondrousYearInfo(4, 21);
                case 667:
                    return new WondrousYearInfo(4, 21);
                case 668:
                    return new WondrousYearInfo(4, 21);
                case 669:
                    return new WondrousYearInfo(5, 20);
                case 670:
                    return new WondrousYearInfo(4, 21);
                case 671:
                    return new WondrousYearInfo(4, 21);
                case 672:
                    return new WondrousYearInfo(4, 21);
                case 673:
                    return new WondrousYearInfo(4, 20);
                case 674:
                    return new WondrousYearInfo(5, 20);
                case 675:
                    return new WondrousYearInfo(4, 21);
                case 676:
                    return new WondrousYearInfo(4, 21);
                case 677:
                    return new WondrousYearInfo(4, 20);
                case 678:
                    return new WondrousYearInfo(5, 20);
                case 679:
                    return new WondrousYearInfo(4, 21);
                case 680:
                    return new WondrousYearInfo(4, 21);
                case 681:
                    return new WondrousYearInfo(4, 20);
                case 682:
                    return new WondrousYearInfo(5, 20);
                case 683:
                    return new WondrousYearInfo(4, 21);
                case 684:
                    return new WondrousYearInfo(4, 21);
                case 685:
                    return new WondrousYearInfo(4, 20);
                case 686:
                    return new WondrousYearInfo(5, 20);
                case 687:
                    return new WondrousYearInfo(4, 21);
                case 688:
                    return new WondrousYearInfo(4, 21);
                case 689:
                    return new WondrousYearInfo(4, 20);
                case 690:
                    return new WondrousYearInfo(5, 20);
                case 691:
                    return new WondrousYearInfo(4, 21);
                case 692:
                    return new WondrousYearInfo(4, 21);
                case 693:
                    return new WondrousYearInfo(4, 20);
                case 694:
                    return new WondrousYearInfo(5, 20);
                case 695:
                    return new WondrousYearInfo(4, 21);
                case 696:
                    return new WondrousYearInfo(4, 21);
                case 697:
                    return new WondrousYearInfo(4, 20);
                case 698:
                    return new WondrousYearInfo(5, 20);
                case 699:
                    return new WondrousYearInfo(4, 21);
                case 700:
                    return new WondrousYearInfo(4, 21);
                case 701:
                    return new WondrousYearInfo(4, 20);
                case 702:
                    return new WondrousYearInfo(5, 20);
                case 703:
                    return new WondrousYearInfo(4, 21);
                case 704:
                    return new WondrousYearInfo(4, 21);
                case 705:
                    return new WondrousYearInfo(4, 20);
                case 706:
                    return new WondrousYearInfo(4, 20);
                case 707:
                    return new WondrousYearInfo(5, 20);
                case 708:
                    return new WondrousYearInfo(4, 21);
                case 709:
                    return new WondrousYearInfo(4, 20);
                case 710:
                    return new WondrousYearInfo(4, 20);
                case 711:
                    return new WondrousYearInfo(5, 20);
                case 712:
                    return new WondrousYearInfo(4, 21);
                case 713:
                    return new WondrousYearInfo(4, 20);
                case 714:
                    return new WondrousYearInfo(4, 20);
                case 715:
                    return new WondrousYearInfo(5, 20);
                case 716:
                    return new WondrousYearInfo(4, 21);
                case 717:
                    return new WondrousYearInfo(4, 20);
                case 718:
                    return new WondrousYearInfo(4, 20);
                case 719:
                    return new WondrousYearInfo(5, 20);
                case 720:
                    return new WondrousYearInfo(4, 21);
                case 721:
                    return new WondrousYearInfo(4, 20);
                case 722:
                    return new WondrousYearInfo(4, 20);
                case 723:
                    return new WondrousYearInfo(5, 20);
                case 724:
                    return new WondrousYearInfo(4, 21);
                case 725:
                    return new WondrousYearInfo(4, 20);
                case 726:
                    return new WondrousYearInfo(4, 20);
                case 727:
                    return new WondrousYearInfo(5, 20);
                case 728:
                    return new WondrousYearInfo(4, 21);
                case 729:
                    return new WondrousYearInfo(4, 20);
                case 730:
                    return new WondrousYearInfo(4, 20);
                case 731:
                    return new WondrousYearInfo(5, 20);
                case 732:
                    return new WondrousYearInfo(4, 21);
                case 733:
                    return new WondrousYearInfo(4, 20);
                case 734:
                    return new WondrousYearInfo(4, 20);
                case 735:
                    return new WondrousYearInfo(5, 20);
                case 736:
                    return new WondrousYearInfo(4, 21);
                case 737:
                    return new WondrousYearInfo(4, 20);
                case 738:
                    return new WondrousYearInfo(4, 20);
                case 739:
                    return new WondrousYearInfo(4, 20);
                case 740:
                    return new WondrousYearInfo(5, 20);
                case 741:
                    return new WondrousYearInfo(4, 20);
                case 742:
                    return new WondrousYearInfo(4, 20);
                case 743:
                    return new WondrousYearInfo(4, 20);
                case 744:
                    return new WondrousYearInfo(5, 20);
                case 745:
                    return new WondrousYearInfo(4, 20);
                case 746:
                    return new WondrousYearInfo(4, 20);
                case 747:
                    return new WondrousYearInfo(4, 20);
                case 748:
                    return new WondrousYearInfo(5, 20);
                case 749:
                    return new WondrousYearInfo(4, 20);
                case 750:
                    return new WondrousYearInfo(4, 20);
                case 751:
                    return new WondrousYearInfo(4, 20);
                case 752:
                    return new WondrousYearInfo(5, 20);
                case 753:
                    return new WondrousYearInfo(4, 20);
                case 754:
                    return new WondrousYearInfo(4, 20);
                case 755:
                    return new WondrousYearInfo(4, 20);
                case 756:
                    return new WondrousYearInfo(5, 20);
                case 757:
                    return new WondrousYearInfo(4, 21);
                case 758:
                    return new WondrousYearInfo(4, 21);
                case 759:
                    return new WondrousYearInfo(4, 21);
                case 760:
                    return new WondrousYearInfo(5, 21);
                case 761:
                    return new WondrousYearInfo(4, 21);
                case 762:
                    return new WondrousYearInfo(4, 21);
                case 763:
                    return new WondrousYearInfo(4, 21);
                case 764:
                    return new WondrousYearInfo(5, 21);
                case 765:
                    return new WondrousYearInfo(4, 21);
                case 766:
                    return new WondrousYearInfo(4, 21);
                case 767:
                    return new WondrousYearInfo(4, 21);
                case 768:
                    return new WondrousYearInfo(5, 21);
                case 769:
                    return new WondrousYearInfo(4, 21);
                case 770:
                    return new WondrousYearInfo(4, 21);
                case 771:
                    return new WondrousYearInfo(4, 21);
                case 772:
                    return new WondrousYearInfo(4, 21);
                case 773:
                    return new WondrousYearInfo(5, 20);
                case 774:
                    return new WondrousYearInfo(4, 21);
                case 775:
                    return new WondrousYearInfo(4, 21);
                case 776:
                    return new WondrousYearInfo(4, 21);
                case 777:
                    return new WondrousYearInfo(5, 20);
                case 778:
                    return new WondrousYearInfo(4, 21);
                case 779:
                    return new WondrousYearInfo(4, 21);
                case 780:
                    return new WondrousYearInfo(4, 21);
                case 781:
                    return new WondrousYearInfo(5, 20);
                case 782:
                    return new WondrousYearInfo(4, 21);
                case 783:
                    return new WondrousYearInfo(4, 21);
                case 784:
                    return new WondrousYearInfo(4, 21);
                case 785:
                    return new WondrousYearInfo(5, 20);
                case 786:
                    return new WondrousYearInfo(4, 21);
                case 787:
                    return new WondrousYearInfo(4, 21);
                case 788:
                    return new WondrousYearInfo(4, 21);
                case 789:
                    return new WondrousYearInfo(5, 20);
                case 790:
                    return new WondrousYearInfo(4, 21);
                case 791:
                    return new WondrousYearInfo(4, 21);
                case 792:
                    return new WondrousYearInfo(4, 21);
                case 793:
                    return new WondrousYearInfo(5, 20);
                case 794:
                    return new WondrousYearInfo(4, 21);
                case 795:
                    return new WondrousYearInfo(4, 21);
                case 796:
                    return new WondrousYearInfo(4, 21);
                case 797:
                    return new WondrousYearInfo(5, 20);
                case 798:
                    return new WondrousYearInfo(4, 21);
                case 799:
                    return new WondrousYearInfo(4, 21);
                case 800:
                    return new WondrousYearInfo(4, 21);
                case 801:
                    return new WondrousYearInfo(5, 20);
                case 802:
                    return new WondrousYearInfo(4, 21);
                case 803:
                    return new WondrousYearInfo(4, 21);
                case 804:
                    return new WondrousYearInfo(4, 21);
                case 805:
                    return new WondrousYearInfo(4, 20);
                case 806:
                    return new WondrousYearInfo(5, 20);
                case 807:
                    return new WondrousYearInfo(4, 21);
                case 808:
                    return new WondrousYearInfo(4, 21);
                case 809:
                    return new WondrousYearInfo(4, 20);
                case 810:
                    return new WondrousYearInfo(5, 20);
                case 811:
                    return new WondrousYearInfo(4, 21);
                case 812:
                    return new WondrousYearInfo(4, 21);
                case 813:
                    return new WondrousYearInfo(4, 20);
                case 814:
                    return new WondrousYearInfo(5, 20);
                case 815:
                    return new WondrousYearInfo(4, 21);
                case 816:
                    return new WondrousYearInfo(4, 21);
                case 817:
                    return new WondrousYearInfo(4, 20);
                case 818:
                    return new WondrousYearInfo(5, 20);
                case 819:
                    return new WondrousYearInfo(4, 21);
                case 820:
                    return new WondrousYearInfo(4, 21);
                case 821:
                    return new WondrousYearInfo(4, 20);
                case 822:
                    return new WondrousYearInfo(5, 20);
                case 823:
                    return new WondrousYearInfo(4, 21);
                case 824:
                    return new WondrousYearInfo(4, 21);
                case 825:
                    return new WondrousYearInfo(4, 20);
                case 826:
                    return new WondrousYearInfo(5, 20);
                case 827:
                    return new WondrousYearInfo(4, 21);
                case 828:
                    return new WondrousYearInfo(4, 21);
                case 829:
                    return new WondrousYearInfo(4, 20);
                case 830:
                    return new WondrousYearInfo(5, 20);
                case 831:
                    return new WondrousYearInfo(4, 21);
                case 832:
                    return new WondrousYearInfo(4, 21);
                case 833:
                    return new WondrousYearInfo(4, 20);
                case 834:
                    return new WondrousYearInfo(5, 20);
                case 835:
                    return new WondrousYearInfo(4, 21);
                case 836:
                    return new WondrousYearInfo(4, 21);
                case 837:
                    return new WondrousYearInfo(4, 20);
                case 838:
                    return new WondrousYearInfo(4, 20);
                case 839:
                    return new WondrousYearInfo(5, 20);
                case 840:
                    return new WondrousYearInfo(4, 21);
                case 841:
                    return new WondrousYearInfo(4, 20);
                case 842:
                    return new WondrousYearInfo(4, 20);
                case 843:
                    return new WondrousYearInfo(5, 20);
                case 844:
                    return new WondrousYearInfo(4, 21);
                case 845:
                    return new WondrousYearInfo(4, 20);
                case 846:
                    return new WondrousYearInfo(4, 20);
                case 847:
                    return new WondrousYearInfo(5, 20);
                case 848:
                    return new WondrousYearInfo(4, 21);
                case 849:
                    return new WondrousYearInfo(4, 20);
                case 850:
                    return new WondrousYearInfo(4, 20);
                case 851:
                    return new WondrousYearInfo(5, 20);
                case 852:
                    return new WondrousYearInfo(4, 21);
                case 853:
                    return new WondrousYearInfo(4, 20);
                case 854:
                    return new WondrousYearInfo(4, 20);
                case 855:
                    return new WondrousYearInfo(5, 20);
                case 856:
                    return new WondrousYearInfo(4, 21);
                case 857:
                    return new WondrousYearInfo(4, 21);
                case 858:
                    return new WondrousYearInfo(4, 21);
                case 859:
                    return new WondrousYearInfo(5, 21);
                case 860:
                    return new WondrousYearInfo(4, 22);
                case 861:
                    return new WondrousYearInfo(4, 21);
                case 862:
                    return new WondrousYearInfo(4, 21);
                case 863:
                    return new WondrousYearInfo(5, 21);
                case 864:
                    return new WondrousYearInfo(4, 22);
                case 865:
                    return new WondrousYearInfo(4, 21);
                case 866:
                    return new WondrousYearInfo(4, 21);
                case 867:
                    return new WondrousYearInfo(5, 21);
                case 868:
                    return new WondrousYearInfo(4, 22);
                case 869:
                    return new WondrousYearInfo(4, 21);
                case 870:
                    return new WondrousYearInfo(4, 21);
                case 871:
                    return new WondrousYearInfo(4, 21);
                case 872:
                    return new WondrousYearInfo(5, 21);
                case 873:
                    return new WondrousYearInfo(4, 21);
                case 874:
                    return new WondrousYearInfo(4, 21);
                case 875:
                    return new WondrousYearInfo(4, 21);
                case 876:
                    return new WondrousYearInfo(5, 21);
                case 877:
                    return new WondrousYearInfo(4, 21);
                case 878:
                    return new WondrousYearInfo(4, 21);
                case 879:
                    return new WondrousYearInfo(4, 21);
                case 880:
                    return new WondrousYearInfo(5, 21);
                case 881:
                    return new WondrousYearInfo(4, 21);
                case 882:
                    return new WondrousYearInfo(4, 21);
                case 883:
                    return new WondrousYearInfo(4, 21);
                case 884:
                    return new WondrousYearInfo(5, 21);
                case 885:
                    return new WondrousYearInfo(4, 21);
                case 886:
                    return new WondrousYearInfo(4, 21);
                case 887:
                    return new WondrousYearInfo(4, 21);
                case 888:
                    return new WondrousYearInfo(5, 21);
                case 889:
                    return new WondrousYearInfo(4, 21);
                case 890:
                    return new WondrousYearInfo(4, 21);
                case 891:
                    return new WondrousYearInfo(4, 21);
                case 892:
                    return new WondrousYearInfo(5, 21);
                case 893:
                    return new WondrousYearInfo(4, 21);
                case 894:
                    return new WondrousYearInfo(4, 21);
                case 895:
                    return new WondrousYearInfo(4, 21);
                case 896:
                    return new WondrousYearInfo(5, 21);
                case 897:
                    return new WondrousYearInfo(4, 21);
                case 898:
                    return new WondrousYearInfo(4, 21);
                case 899:
                    return new WondrousYearInfo(4, 21);
                case 900:
                    return new WondrousYearInfo(5, 21);
                case 901:
                    return new WondrousYearInfo(4, 21);
                case 902:
                    return new WondrousYearInfo(4, 21);
                case 903:
                    return new WondrousYearInfo(4, 21);
                case 904:
                    return new WondrousYearInfo(4, 21);
                case 905:
                    return new WondrousYearInfo(5, 20);
                case 906:
                    return new WondrousYearInfo(4, 21);
                case 907:
                    return new WondrousYearInfo(4, 21);
                case 908:
                    return new WondrousYearInfo(4, 21);
                case 909:
                    return new WondrousYearInfo(5, 20);
                case 910:
                    return new WondrousYearInfo(4, 21);
                case 911:
                    return new WondrousYearInfo(4, 21);
                case 912:
                    return new WondrousYearInfo(4, 21);
                case 913:
                    return new WondrousYearInfo(5, 20);
                case 914:
                    return new WondrousYearInfo(4, 21);
                case 915:
                    return new WondrousYearInfo(4, 21);
                case 916:
                    return new WondrousYearInfo(4, 21);
                case 917:
                    return new WondrousYearInfo(5, 20);
                case 918:
                    return new WondrousYearInfo(4, 21);
                case 919:
                    return new WondrousYearInfo(4, 21);
                case 920:
                    return new WondrousYearInfo(4, 21);
                case 921:
                    return new WondrousYearInfo(5, 20);
                case 922:
                    return new WondrousYearInfo(4, 21);
                case 923:
                    return new WondrousYearInfo(4, 21);
                case 924:
                    return new WondrousYearInfo(4, 21);
                case 925:
                    return new WondrousYearInfo(5, 20);
                case 926:
                    return new WondrousYearInfo(4, 21);
                case 927:
                    return new WondrousYearInfo(4, 21);
                case 928:
                    return new WondrousYearInfo(4, 21);
                case 929:
                    return new WondrousYearInfo(5, 20);
                case 930:
                    return new WondrousYearInfo(4, 21);
                case 931:
                    return new WondrousYearInfo(4, 21);
                case 932:
                    return new WondrousYearInfo(4, 21);
                case 933:
                    return new WondrousYearInfo(5, 20);
                case 934:
                    return new WondrousYearInfo(4, 21);
                case 935:
                    return new WondrousYearInfo(4, 21);
                case 936:
                    return new WondrousYearInfo(4, 21);
                case 937:
                    return new WondrousYearInfo(4, 20);
                case 938:
                    return new WondrousYearInfo(5, 20);
                case 939:
                    return new WondrousYearInfo(4, 21);
                case 940:
                    return new WondrousYearInfo(4, 21);
                case 941:
                    return new WondrousYearInfo(4, 20);
                case 942:
                    return new WondrousYearInfo(5, 20);
                case 943:
                    return new WondrousYearInfo(4, 21);
                case 944:
                    return new WondrousYearInfo(4, 21);
                case 945:
                    return new WondrousYearInfo(4, 20);
                case 946:
                    return new WondrousYearInfo(5, 20);
                case 947:
                    return new WondrousYearInfo(4, 21);
                case 948:
                    return new WondrousYearInfo(4, 21);
                case 949:
                    return new WondrousYearInfo(4, 20);
                case 950:
                    return new WondrousYearInfo(5, 20);
                case 951:
                    return new WondrousYearInfo(4, 21);
                case 952:
                    return new WondrousYearInfo(4, 21);
                case 953:
                    return new WondrousYearInfo(4, 20);
                case 954:
                    return new WondrousYearInfo(5, 20);
                case 955:
                    return new WondrousYearInfo(4, 21);
                case 956:
                    return new WondrousYearInfo(4, 21);
                case 957:
                    return new WondrousYearInfo(4, 20);
                case 958:
                    return new WondrousYearInfo(5, 20);
                case 959:
                    return new WondrousYearInfo(4, 21);
                case 960:
                    return new WondrousYearInfo(4, 21);
                case 961:
                    return new WondrousYearInfo(4, 20);
                case 962:
                    return new WondrousYearInfo(5, 20);
                case 963:
                    return new WondrousYearInfo(4, 21);
                case 964:
                    return new WondrousYearInfo(4, 21);
                case 965:
                    return new WondrousYearInfo(4, 20);
                case 966:
                    return new WondrousYearInfo(5, 20);
                case 967:
                    return new WondrousYearInfo(4, 21);
                case 968:
                    return new WondrousYearInfo(4, 21);
                case 969:
                    return new WondrousYearInfo(4, 20);
                case 970:
                    return new WondrousYearInfo(4, 20);
                case 971:
                    return new WondrousYearInfo(5, 20);
                case 972:
                    return new WondrousYearInfo(4, 21);
                case 973:
                    return new WondrousYearInfo(4, 20);
                case 974:
                    return new WondrousYearInfo(4, 20);
                case 975:
                    return new WondrousYearInfo(5, 20);
                case 976:
                    return new WondrousYearInfo(4, 21);
                case 977:
                    return new WondrousYearInfo(4, 20);
                case 978:
                    return new WondrousYearInfo(4, 20);
                case 979:
                    return new WondrousYearInfo(5, 20);
                case 980:
                    return new WondrousYearInfo(4, 21);
                case 981:
                    return new WondrousYearInfo(4, 20);
                case 982:
                    return new WondrousYearInfo(4, 20);
                case 983:
                    return new WondrousYearInfo(5, 20);
                case 984:
                    return new WondrousYearInfo(4, 21);
                case 985:
                    return new WondrousYearInfo(4, 20);
                case 986:
                    return new WondrousYearInfo(4, 20);
                case 987:
                    return new WondrousYearInfo(5, 20);
                case 988:
                    return new WondrousYearInfo(4, 21);
                case 989:
                    return new WondrousYearInfo(4, 20);
                case 990:
                    return new WondrousYearInfo(4, 20);
                case 991:
                    return new WondrousYearInfo(5, 20);
                case 992:
                    return new WondrousYearInfo(4, 21);
                case 993:
                    return new WondrousYearInfo(4, 20);
                case 994:
                    return new WondrousYearInfo(4, 20);
                case 995:
                    return new WondrousYearInfo(5, 20);
                case 996:
                    return new WondrousYearInfo(4, 21);
                case 997:
                    return new WondrousYearInfo(4, 20);
                case 998:
                    return new WondrousYearInfo(4, 20);
                case 999:
                    return new WondrousYearInfo(5, 20);
                case 1000:
                    return new WondrousYearInfo(4, 21);
                default:
                    throw new ArgumentOutOfRangeException(nameof(year));
            }
        }
    }

    ///// <summary>
    ///// A simple structure representing the Wondrous date.
    ///// </summary>
    //public class WondrousYearMonthDay
    //{
    //    public WondrousYearMonthDay(int year, int month, int day)
    //    {
    //        Year = year;
    //        Month = month;
    //        Day = day;
    //    }

    //    public int Year { private set; get; }
    //    public int Month { private set; get; }
    //    public int Day { private set; get; }
    //}

    internal struct WondrousYearInfo
    {
        public byte NawRuzDayInMarch;
        public byte DaysInAyyamiHa;

        public WondrousYearInfo(byte daysInAyyamiHa, byte nawRuzDayInMarch)
        {
            NawRuzDayInMarch = nawRuzDayInMarch;
            DaysInAyyamiHa = daysInAyyamiHa;
        }
    }
}