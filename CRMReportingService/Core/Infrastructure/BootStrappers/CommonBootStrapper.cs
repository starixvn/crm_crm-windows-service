
namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers
{
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Summary description for CommonBootStrapper
    /// </summary>
    public abstract class CommonBootStrapper
    {
        public static IServiceLocator Locator;

        public CommonBootStrapper()
        {
            Locator = CreateServiceLocator();
        }

        protected abstract IServiceLocator CreateServiceLocator();
    }
}