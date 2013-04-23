namespace NexCel.CRM.CRMReportingService.Business.DailyReport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    using ExcelUtil.Spreadsheet;

    using NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Common;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;
    using NexCel.CRM.CRMReportingService.DataAccess;

    public class DailyReportService:IDailyReportService
    {
        public DataTable Countries;

        public void ExportExcelAndSendMailToCSD()
        {
            try
            {
                Console.WriteLine("CRM Reporting Started...");
                #region get common settings
                var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
                AppSettings.Countries = report.SelCountries();
                AppSettings.Settings = report.GetSettings();
                #endregion
               
                var currentDay = DateTime.Now;
                var getTitle = "Daily";
                
                if (AppConfigs.IsGetDateFromConfig)
                    currentDay = AppConfigs.ConfigReportDate;
                var excelAttachFileSubfix = string.Format("-On {0}", currentDay.AddDays(-1).ToString("MM-dd-yyyy"));
                var reportName = string.Empty;
                var mailDateFormat = currentDay.AddDays(-1).ToString("MM-dd-yyyy");

                var reportCasinoName = string.Empty;
                if (currentDay.DayOfWeek == DayOfWeek.Sunday || currentDay.Day == 1)
                {
                    Console.WriteLine("-----------------------");
                    reportName = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                                  "exportExcelName"), getTitle, excelAttachFileSubfix);
                    reportCasinoName = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                                      "exportCasinoExcelName"), getTitle, excelAttachFileSubfix);
                    
                    AttachDataToExcelFile(reportName, reportCasinoName, currentDay, getTitle, true, mailDateFormat);

                    getTitle = "Weekly";
                    var firstDate = Helper.GetFirstDayOfWeek(currentDay.AddDays(-1)).ToString("MM-dd-yyyy");
                    excelAttachFileSubfix = string.Format("-From{0}-To{1}",
                                                          firstDate, currentDay.AddDays(-1).ToString("MM-dd-yyyy"));
                    mailDateFormat = string.Format("{0}-->{1}", firstDate, currentDay.AddDays(-1).ToString("MM-dd-yyyy"));
                }

                reportName = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                                  "exportExcelName"), getTitle, excelAttachFileSubfix);
                reportCasinoName = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                                  "exportCasinoExcelName"), getTitle, excelAttachFileSubfix);

                Console.WriteLine("-----------------------");
                AttachDataToExcelFile(reportName, reportCasinoName, currentDay, getTitle, false, mailDateFormat);

                throw new Exception("testing exception");
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.WriteLog("--Daily Report Service-->" + ex.Message);
            }
        }
        
        private void AttachDataToExcelFile(string ibcReportName, string casinoReportName, DateTime currentDay, string getTitle, bool isGetDaily, string mailDate)
        {
            var excelIbcReportFile = new ExcelFile();
            var excelCasinoReportFile = new ExcelFile();
            #region get data
            #region ibc
            //get daily summary report            
            new DailySummary().GetFeedbackSummaryReport(excelIbcReportFile, currentDay, isGetDaily);
            //get daily complaints report            
            new DailyComplaintsData().GetDailyComplaintsDetailReport(excelIbcReportFile, currentDay, isGetDaily);
            //get daily licensee detail report            
            //new DailyLicenseeData().GetDailyComplaintsDetailReport(excelIbcReportFile, currentDay, isGetDaily);
            #endregion

            #region casino
            //get casino daily summary
            new CasinoDailySummaryReport().GetCasinoSummaryReport(excelCasinoReportFile, currentDay, isGetDaily);
            //get casino daily detail
            new CasinoDailyDetailReport().GetCasinoDailyDetailReport(excelCasinoReportFile, currentDay, isGetDaily);
            #endregion
            #endregion

            #region add files and attach
            //send mail
            #region attach files
            var mailAttachment = new List<MailAttachment>();
            //ibc
            var memoryIbcStream = new MemoryStream();
            excelIbcReportFile.SaveXls(memoryIbcStream);
            mailAttachment.Add(new MailAttachment()
            {
                Content = memoryIbcStream,
                ContentType = "application/ms-excel",
                FileName = string.Format("{0}.xls", ibcReportName)
            });
            //casino
            var memoryCasinoStream = new MemoryStream();
            excelCasinoReportFile.SaveXls(memoryCasinoStream);
            mailAttachment.Add(new MailAttachment()
            {
                Content = memoryCasinoStream,
                ContentType = "application/ms-excel",
                FileName = string.Format("{0}.xls", casinoReportName)
            });
            #endregion

            Console.WriteLine("Sending mail...");
            var isSuccess = SendMailToCSD(mailAttachment, getTitle, mailDate);
            Console.WriteLine(isSuccess ? "Send mail successfully" : "Send mail failed");

            #endregion
        }

        #region send mail
        private bool SendMailToCSD(List<MailAttachment> mailAttachment, string mailRefix, string mailDate)
        {
            var result = true;
            #region mail settings
            var title = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "mailDailyReportTitle"), mailRefix);
            var subject = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "mailDailyReportSubject"), mailRefix, mailDate);
            var content = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "mailDailyReportContent"), mailRefix, mailDate);
            var mailSettings = MailSection.Instance[MailSection.Setting.Send]["CSDGroup"].Mails["CSD"];

            #endregion

            #region send mail
            var mainSend = new MailSend()
            {
                HostName = MailSection.HostName.Gmail,
                To = mailSettings.To,
                Cc = mailSettings.Cc,
                Bcc = mailSettings.Bcc,
                //Attachment = mailAttachment,
                Title = title,
                Subject = subject,
                Body = string.Format("{0}", content)
            };

            result = SendMailExtension.SendMail(mainSend, mailAttachment);
            #endregion
            return result;
        }
        #endregion
    }
}


