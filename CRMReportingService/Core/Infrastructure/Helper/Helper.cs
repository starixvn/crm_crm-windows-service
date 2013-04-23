using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper
{
    public class Helper
    {
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            var firstDay = DayOfWeek.Sunday;
            var firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                if (firstDayInWeek.AddDays(-1).Month == firstDayInWeek.Month)
                    firstDayInWeek = firstDayInWeek.AddDays(-1);
                else
                
                    return firstDayInWeek;
                
            }

            return firstDayInWeek;
        }

        public static int GetWeekOfMonth(DateTime date)
        {
            var beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        public static int GetLastDayOfMonth(DateTime currentDate)
        {
            var day = 30;
            day = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            return day;
        }

        public static string ReplaceHtmlTag(string value)
        {
            
            var result = value.Replace("<br/>", "\n").Replace("<br>", "\n");
            result = System.Web.HttpUtility.HtmlDecode(result);
            result = Regex.Replace(result, "<[^<>]+>", "");
            return result;
        }
    }
}
