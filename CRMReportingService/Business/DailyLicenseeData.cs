namespace NexCel.CRM.CRMReportingService.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;

    using DNAide.Core;

    using ExcelUtil.Spreadsheet;

    using NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Common;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;
    using NexCel.CRM.CRMReportingService.DataAccess;
    using NexCel.CRM.CRMReportingService.Entities;

    public class DailyLicenseeData
    {

        public void GetDailyComplaintsDetailReport(ExcelFile excelFile, DateTime currentDay, bool isGetDaily)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var firstDay = currentDay.AddDays(-1);
            var titleRefix = "Daily";
            var tilteSubfix = string.Format("ON {0}", firstDay.ToString("MMMM dd, yyyy"));

            if ((currentDay.DayOfWeek == DayOfWeek.Sunday || currentDay.Day == 1) && !isGetDaily)
            {
                Console.WriteLine("Get weekly Licensse report...");
                firstDay = Helper.GetFirstDayOfWeek(currentDay.AddDays(-1));
                titleRefix = "Weekly";
                tilteSubfix = string.Format("FROM {0} TO {1}", firstDay.Day, currentDay.AddDays(-1).ToString("dd MMMM yy"));
            }
            else
            {
                return;
            }
            

            var db = report.GetDailyLicenseeReport(firstDay, currentDay.AddDays(-1));

            var dailyComplaintsData = new DailyLicenseeDTO();
            dailyComplaintsData.LicenseeComplaints = new List<DailyLicenseeDetail>();

            if (db != null && db.Tables.Count > 0)
            {
                var settings = AppSettings.Settings;

                foreach (DataRow dr in db.Tables[0].Rows)
                {
                    var complains = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Description"]));
                    var solution = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Solution"]));

                    dailyComplaintsData.LicenseeComplaints.Add(new DailyLicenseeDetail()
                    {
                        FeedbackDate = DateTimeUtilize.ChangeTimeZoneToClient(dr["FeedbackDate"].ToString()),
                        Licensee = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["SiteID"])),
                        CustomerName = CastAide.AsString(dr["UserName"]),
                        ContactAddress = string.Format("{0}\n{1}\n{2}\n- Phone: {3}\n- Mobile: {4}", AppSettings.GetContactDetail(CastAide.AsString(dr["msn"]), 1),
                                        AppSettings.GetContactDetail(CastAide.AsString(dr["Skype"]), 2), 
                                        AppSettings.GetContactDetail(CastAide.AsString(dr["Email"]), 3),
                                        CastAide.AsString(dr["Phone"]), CastAide.AsString(dr["MobilePhone"])),

                        Category = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["MainCategoryID"])),
                        Complaints = complains,
                        Solution = solution,
                        Status = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["StatusID"])),
                        Response = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["ResponseID"])),
                        HandlyBy = CastAide.AsString(dr["CreatorName"]),
                        DutyShift = DateTimeUtilize.ChangeTimeZoneToClient(dr["CreateDate"].ToString())
                    });
                }
            }

            ExportDailyComplaintsDetail(dailyComplaintsData, currentDay, excelFile, titleRefix, tilteSubfix);
        }

        #region Export 2 Excel
        private ExcelFile ExportDailyComplaintsDetail(DailyLicenseeDTO complaints, DateTime reportDate, ExcelFile excelFile, string titlePrefix, string tilteSubfix)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;

            var nextRow = 0;
            var wsDailyData = excelFile.Worksheets.Add(string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "licenseeDataSheet"), titlePrefix));
            #endregion

            #region Header
            ExportDailyComplaintsDetailAssignHeader(ref wsDailyData, startRow, startCol, reportDate, titlePrefix, tilteSubfix);
            #endregion

            #region Content
            ExportDailyComplaintsDetailAssignContent(ref wsDailyData, startRow, startCol, nextRow, reportDate, complaints);
            #endregion
            return excelFile;
        }

        private void ExportDailyComplaintsDetailAssignHeader(ref ExcelWorksheet wsDailyData, int startRow, int startCol, DateTime reportDate, string titleRefix, string tilteSubfix)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));

            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 10).Merged = true;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 10).Style = bgHeaderMainFormat;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 10).Value = string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportDailyLicenseeInquiriesTitle"), titleRefix.ToUpper(), tilteSubfix);

            #region sencond row
            var nextCol = 0;
            wsDailyData.Rows[startRow - 2].Height = 255 * 2;
            wsDailyData.Rows[startRow - 1].Height = 255 * 2;

            wsDailyData.Cells[startRow - 1, startCol].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "date");
            wsDailyData.Columns[startCol].Width = 255 * 20;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "licensee");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "customer");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "ContactMSNSKypeEmail");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 30;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "category");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 30;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "complaintsEnquiries");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 60;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "solution");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 60;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "status");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "response");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "handleBy");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "CreationTime");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;
            #endregion
        }
        private void ExportDailyComplaintsDetailAssignContent(ref ExcelWorksheet wsDailyData, int startRow, int startCol, int nextRow, DateTime reportDate, DailyLicenseeDTO complaints)
        {
            var bgFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(255, 255, 255), HorizontalAlignmentStyle.Center, ExcelFont.NormalWeight);

            foreach (DailyLicenseeDetail detail in complaints.LicenseeComplaints)
            {
                var nextCol = 0;

                wsDailyData.Cells[startRow + nextRow, startCol].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol].Value = detail.FeedbackDate;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Licensee;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CustomerName;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.ContactAddress;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Category;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Complaints;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Solution;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Status;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Response;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.HandlyBy;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.DutyShift;

                startRow++;
            }
        }
        #endregion
    }
}
