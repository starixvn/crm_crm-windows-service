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

    public class CasinoDailyDetailReport
    {
        public void GetCasinoDailyDetailReport(ExcelFile excelFile, DateTime currentDate, bool isGetDaily)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var firstDay = currentDate.AddDays(-1);
            var reportType = 1;
            var title = "Daily";
            
            if ((currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.Day == 1) && !isGetDaily)
            {
                Console.WriteLine("Get weekly casino detail report...");
                firstDay = Helper.GetFirstDayOfWeek(currentDate.AddDays(-1));
                reportType = 2;
                title = "Weekly";
            }
            else
                Console.WriteLine("Get daily casino detail report...");

            var db = report.GetCasinoDailyDetailReport(reportType, firstDay, currentDate.AddDays(-1));

            var dailyComplaintsData = new DailyComplaintsDataDTO();
            dailyComplaintsData.DailyComplaints = new List<DailyComplaintsDetail>();

            if (db != null && db.Tables.Count > 0)
            {
                
                var settings = AppSettings.Settings;
                var countries = AppSettings.Countries;

                foreach (DataRow dr in db.Tables[0].Rows)
                {
                    var complains = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Description"]));
                    var solution = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Solution"]));

                    dailyComplaintsData.DailyComplaints.Add(new DailyComplaintsDetail()
                    {
                        FeedbackDate = DateTimeUtilize.ChangeTimeZoneToClient(dr["FeedbackDate"].ToString()),
                        CustomerName = CastAide.AsString(dr["UserName"]),
                        CommunicationTool = new CommunicationTool()
                        {
                            ContactBy = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["CommunicationID"])),
                            Address = string.Format("{0}\n{1}\n{2}\n- Phone: {3}\n- Mobile: {4}", AppSettings.GetContactDetail(CastAide.AsString(dr["msn"]), 1),
                                        AppSettings.GetContactDetail(CastAide.AsString(dr["Skype"]), 2), 
                                        AppSettings.GetContactDetail(CastAide.AsString(dr["Email"]), 3),
                                        CastAide.AsString(dr["Phone"]), CastAide.AsString(dr["MobilePhone"]))
                        },
                        Site = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["SiteID"])),
                        ExtNo = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["StationID"])),
                        ProductName = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["ProductID"])),
                        Country = AppSettings.GetCountryName(countries, CastAide.AsInt32(dr["CountryId"])),
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

            ExportDailyComplaintsDetail(dailyComplaintsData, currentDate, excelFile, title);

        }

        #region Export 2 Excel
        private void ExportDailyComplaintsDetail(DailyComplaintsDataDTO complaints, DateTime reportDate, ExcelFile excelFile, string reportTitle)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;

            var nextRow = 0;
            var wsDailyData = excelFile.Worksheets.Add(string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportCasinoDailyDetailSheet"), reportTitle));
            #endregion

            #region Header
            ExportDailyComplaintsDetailAssignHeader(ref wsDailyData, startRow, startCol, reportTitle);
            #endregion

            #region Content
            ExportDailyComplaintsDetailAssignContent(ref wsDailyData, startRow, startCol, nextRow, reportDate, complaints);
            #endregion

            return;
        }

        private void ExportDailyComplaintsDetailAssignHeader(ref ExcelWorksheet wsDailyData, int startRow, int startCol, string reportTitle)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));

            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 15).Merged = true;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 15).Style = bgHeaderMainFormat;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 15).Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportIbcCasinoRelatedTitle");

            #region sencond row
            var nextCol = 0;
            wsDailyData.Rows[startRow - 2].Height = 255 * 2;
            wsDailyData.Rows[startRow - 1].Height = 255 * 2;

            wsDailyData.Cells[startRow - 1, startCol].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "date");
            wsDailyData.Columns[startCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "Time");
            wsDailyData.Columns[startCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "customer");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "site");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "typeofcontact");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "productname");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "ExtNo");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "ContactMSNSKypeEmail");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 30;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "country");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

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

        private void ExportDailyComplaintsDetailAssignContent(ref ExcelWorksheet wsDailyData, int startRow, int startCol, int nextRow, DateTime reportDate, DailyComplaintsDataDTO complaints)
        {
            var bgFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(255, 255, 255), HorizontalAlignmentStyle.Center, ExcelFont.NormalWeight);

            foreach (DailyComplaintsDetail detail in complaints.DailyComplaints)
            {
                var nextCol = 0;
                
                wsDailyData.Cells[startRow + nextRow, startCol].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol].Value = CastAide.AsDateTime(detail.FeedbackDate).ToShortDateString();

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = CastAide.AsDateTime(detail.FeedbackDate).ToShortTimeString();

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CustomerName;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Site;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CommunicationTool.ContactBy;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.ProductName;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.ExtNo;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CommunicationTool.Address;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Country;

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
