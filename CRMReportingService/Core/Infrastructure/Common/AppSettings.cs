namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Common
{
    using System.Data;

    using DNAide.Core;

    public class AppSettings
    {
        public static string GetSettingName(DataTable settings, int settingId)
        {
            var settingName = "N/A";
            try
            {
                var query =
                    (from s in settings.AsEnumerable() where s.Field<int>("SettingId") == settingId select s).AsDataView
                        <DataRow>();
                settingName = CastAide.AsString(query[0]["SettingValue"]);
            }
            catch { }
            return settingName;
        }

        public static string GetCountryName(DataTable countries, int countryId)
        {
            var settingName = "N/A";
            try
            {
                var query = (from s in countries.AsEnumerable() where s.Field<int>("CountryId") == countryId select s).AsDataView<DataRow>();
                settingName = CastAide.AsString(query[0]["CountryName"]);
            }
            catch { }
            return settingName;
        }

        public static string GetContactDetail(string contactDetail, int contactType)
        {
            var result = string.Empty;
            
            result = string.Format("- {0}: {1} ", GetContactTypeValue(contactType), contactDetail);

            return result;
        }

        private static string GetContactTypeValue(int contactType)
        {
            var result = string.Empty;
            switch (contactType)
            {
                case 1:
                    result = "MSN";
                    break;
                case 2:
                    result = "Skype";
                    break;
                case 3:
                    result = "Email";
                    break;
                default:
                    break;
            }
            return result;
        }

        public static DataTable Countries { get; set; }
        public static DataTable Settings { get; set; }
    }
}
