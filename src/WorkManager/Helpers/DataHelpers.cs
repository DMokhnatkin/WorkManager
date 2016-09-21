using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManager.Helpers
{
    public static class DateHelpers
    {
        public static DateTime GetStartOfWeek(DateTime date, CultureInfo culture)
        {
            int diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetEndOfWeek(DateTime date, CultureInfo culture)
        {
            return GetStartOfWeek(date, culture).Add(new TimeSpan(7, 0, 0, 0));
        }

        public static DateTime GetStartOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 0);
        }

        public static DateTime GetEndOfMonth(DateTime dt)
        {
            return GetStartOfMonth(dt).AddMonths(1);
        }
    }
}
