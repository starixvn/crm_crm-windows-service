namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions
{
    using System;
    using System.Resources;

    public class ResourceExtension
    {
        public enum Resources
        {
            Language
        }
        public static string GetResourceValue(Resources resource, string key)
        {
            string resourceValue = string.Empty;
            try
            {   
                ResourceManager resourceManager = new ResourceManager("NexCel.CRM.CRMReportingService.AppFiles.GlobalResources."+ resource.ToString(), System.Reflection.Assembly.GetExecutingAssembly());                
                resourceValue = resourceManager.GetString(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resourceValue = string.Empty;
            }
            return resourceValue;
        }
    }
}
