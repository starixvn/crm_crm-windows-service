// -----------------------------------------------------------------------
// <copyright file="DateTimeUtilize.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DNAide.Core;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DateTimeUtilize
    {
        private TimeSpan _timeSpan;
        public DateTimeUtilize()
        {
            this._timeSpan = new TimeSpan(AppConfigs.TimeSpan,  0, 0);
        }

        public static DateTime ResetEndDay(DateTime date)
        {
            return date.AddDays(1).AddSeconds(-1);
        }

        #region Convert time client <->server
        public static string ChangeTimeZoneToClient(DateTime Time)
        {
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return Time.Add(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeZoneToClient(string Time)
        {
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            var Client = CastAide.AsDateTime(Time);
            return Client.Add(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeZoneToClientDate(string Time)
        {
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            var Client = CastAide.AsDateTime(Time);
            return Client.Add(timeSpan._timeSpan).ToString("MM/dd/yyyy");
        }

        public static string ChangeTimeZoneToClientTime(string Time)
        {
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            var Client = CastAide.AsDateTime(Time);
            return Client.Add(timeSpan._timeSpan).ToString("hh:mm tt");
        }


        public static string ChangeTimeZoneToServer(DateTime Time)
        {
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return Time.Subtract(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeZoneToServer(string Time)
        {
            var Client = CastAide.AsDateTime(Time);
            DateTimeUtilize timeSpan = new DateTimeUtilize();

            return Client.Subtract(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }
        #endregion

        #region Convert Time for report client <->server

        public static string ChangeTimeToServerForReportBegin(DateTime Time)
        {
            var TimeServer = Time.Date;
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return TimeServer.Subtract(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeToServerForReportBegin(string Time)
        {
            var Server = CastAide.AsDateTime(Time);
            var TimeServer = Server.Date;
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return TimeServer.Subtract(timeSpan._timeSpan).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeToServerForReportEnd(DateTime Time)
        {
            var TimeServer = Time.Date;
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return TimeServer.Subtract(timeSpan._timeSpan).AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ChangeTimeToServerForReportEnd(string Time)
        {
            var Server = CastAide.AsDateTime(Time);
            var TimeServer = Server.Date;
            DateTimeUtilize timeSpan = new DateTimeUtilize();
            return TimeServer.Subtract(timeSpan._timeSpan).AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy hh:mm tt");
        }

        #endregion
    }
}
