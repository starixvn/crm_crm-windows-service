namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers
{
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using NexCel.CRM.CRMReportingService.Business.DailyReport;
    using NexCel.CRM.CRMReportingService.DataAccess;

    public class UnityBootStrapper : CommonBootStrapper
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            IUnityContainer container = new UnityContainer();
            Register(container);
            return new UnityServiceLocator(container);
        }

        private static void Register(IUnityContainer container)
        {
            container
                .RegisterType<IReportDAO, ReportDAO>()
                .RegisterType<IDailyReportService, DailyReportService>();
        }
    }
}
