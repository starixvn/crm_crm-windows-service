// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication
{
    using System;
    using System.ServiceProcess;

    using Fanex.CRM.CRMReplication.Core.Extensions;

    class Program
    {
        static void Main()
        {
            try
            {

                var servicesToRun = new ServiceBase[] 
                                                  { 
                                                      new CRMService()
                                                  };
                ServiceBase.Run(servicesToRun);
                
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
            }
        }
    }
}