namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers
{
    /// <summary>
    /// Summary description for BootStrapperManager
    /// </summary>
    public class BootStrapperManager
    {
        private static CommonBootStrapper _bootStrapper;

        public static CommonBootStrapper BootStrapper
        {
            get { return _bootStrapper; }
        }

        public static void Initialize(CommonBootStrapper bootStrapper)
        {
            _bootStrapper = bootStrapper;
        }
    }
}