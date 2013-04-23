// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Core
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    using DNAide.Core;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AppSettings
    {
        public static string CRMConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
            }
        }

        public static string MongoConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["connectionString"];
            }
        }

        public static string MongoDbName
        {
            get
            {
                return ConfigurationManager.AppSettings["dbname"];
            }
        }

        public static string TableCustomerName
        {
            get
            {
                return ConfigurationManager.AppSettings["tbCustomers"];
            }
        }

        public static int CustomerTimer
        {
            get
            {
                return CastAide.AsInt32(ConfigurationManager.AppSettings["CustomerTimer"]);
            }
        }

        public static int CheckMemoryUsageTimer
        {
            get
            {
                return CastAide.AsInt32(ConfigurationManager.AppSettings["CheckMemoryUsageTimer"]);
            }
        }

        public static int MemoryUsageLimit
        {
            get
            {
                return CastAide.AsInt32(ConfigurationManager.AppSettings["MemoryUsageLimit"]);
            }
        }

        public static string CustomerMaxTsTamp
        {
            get
            {
                var timesTampFileDir = string.Format(
                    "{0}/AppFiles/{1}", Application.StartupPath, Constant.TimestampFiles.CustomerTimestamp);
                if (!System.IO.File.Exists(timesTampFileDir)) System.IO.File.Create(timesTampFileDir);

                var fileInfo = new System.IO.FileInfo(timesTampFileDir);
                var sReader = fileInfo.OpenText();
                var timesTampValue = sReader.ReadToEnd();
                if (string.IsNullOrEmpty(timesTampValue))
                    return string.Empty;

                timesTampValue =  timesTampValue.Trim().Replace(Environment.NewLine, string.Empty);
                sReader.Close();
                return timesTampValue;
            }
            set
            {
                var timesTampFileDir = string.Format(
                    "{0}/AppFiles/{1}", Application.StartupPath, Constant.TimestampFiles.CustomerTimestamp);
                var fileInfo = new System.IO.FileInfo(timesTampFileDir);
                if (!fileInfo.Exists)
                    return;
                
                var file = new System.IO.StreamWriter(timesTampFileDir, false);
                file.WriteLine(value);
                file.Close();
            }
        }
    }
}
