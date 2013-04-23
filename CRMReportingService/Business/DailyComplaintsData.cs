namespace NexCel.CRM.CRMReportingService.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;

    using DNAide.Core;

    using ExcelUtil.Spreadsheet;

    using NexCel.CRM.CRMReportingService.Core.Constant;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Common;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;
    using NexCel.CRM.CRMReportingService.DataAccess;
    using NexCel.CRM.CRMReportingService.Entities;

    public class DailyComplaintsData
    {
        public void GetDailyComplaintsDetailReport(ExcelFile excelFile, DateTime currentDay, bool isGetDaily)
        {
            var firstDay = currentDay.AddDays(-1);
            var titleRefix = "Daily";
            var titleSubfix = string.Format("ON {0}", firstDay.ToString("MMMM dd, yyyy"));

            if ((currentDay.DayOfWeek == DayOfWeek.Sunday || currentDay.Day == 1) && !isGetDaily)
            {
                Console.WriteLine("Get weekly detail report...");
                firstDay = Helper.GetFirstDayOfWeek(currentDay.AddDays(-1));
                titleRefix = "Weekly";
                titleSubfix = string.Format("FROM {0} TO {1}", firstDay.Day, currentDay.AddDays(-1).ToString("dd MMMM yy"));
            }
            else
                Console.WriteLine("Get daily detail report...");

            var sheetName = "Credit Site";
            AssignReportDataToExcelFile(firstDay, currentDay.AddDays(-1), excelFile, sheetName, string.Format("IBC {0}", titleRefix), titleSubfix);
            sheetName = "All Licensee";
            AssignReportDataToExcelFile(firstDay, currentDay.AddDays(-1), excelFile, sheetName, string.Format("Licensee {0}", titleRefix), titleSubfix,
                                        Enumerations.ReportTypes.Licensee);
        }

        private void AssignReportDataToExcelFile(DateTime fDate, DateTime tDate, ExcelFile excelFile, string sheetName, string titleRefix, string tilteSubfix, Enumerations.ReportTypes siteType = Enumerations.ReportTypes.CreditSite)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var db = report.GetDailyComplaintsReport(fDate, tDate, siteType);
            var dailyComplaintsData = new DailyComplaintsDataDTO();
            dailyComplaintsData.DailyComplaints = new List<DailyComplaintsDetail>();

            if (db != null && db.Tables.Count > 0)
            {
                var settings = AppSettings.Settings;
                foreach (DataRow dr in db.Tables[0].Rows)
                {
                    var complains = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Description"]));
                    var solution = Helper.ReplaceHtmlTag(CastAide.AsString(dr["Solution"]));

                    dailyComplaintsData.DailyComplaints.Add(new DailyComplaintsDetail()
                    {
                        FeedbackDate = DateTimeUtilize.ChangeTimeZoneToClient(dr["FeedbackDate"].ToString()),
                        Licensee = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["SiteID"])),
                        CustomerName = CastAide.AsString(dr["UserName"]),
                        CommunicationTool = new CommunicationTool()
                        {
                            ContactBy = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["CommunicationID"])),
                            Address =
                                string.Format(
                                    "{0}\n{1}\n{2}\n- Phone: {3}\n- Mobile: {4}",
                                    AppSettings.GetContactDetail(CastAide.AsString(dr["msn"]), 1),
                                    AppSettings.GetContactDetail(CastAide.AsString(dr["Skype"]), 2),
                                    AppSettings.GetContactDetail(CastAide.AsString(dr["Email"]), 3),
                                    CastAide.AsString(dr["Phone"]),
                                    CastAide.AsString(dr["MobilePhone"]))
                        },
                        ExtNo = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["StationID"])),
                        Country = AppSettings.GetCountryName(AppSettings.Countries, CastAide.AsInt32(dr["CountryID"])),
                        Category = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["MainCategoryID"])),
                        ProductName = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["ProductID"])),
                        Complaints = complains,
                        Solution = solution,
                        Response = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["ResponseID"])),
                        Status = AppSettings.GetSettingName(settings, CastAide.AsInt32(dr["StatusID"])),
                        HandlyBy = CastAide.AsString(dr["CreatorName"]),
                        DutyShift = DateTimeUtilize.ChangeTimeZoneToClient(dr["CreateDate"].ToString())
                    });
                }
            }

            ExportDailyComplaintsDetail(dailyComplaintsData, tDate, excelFile, sheetName, titleRefix, tilteSubfix);
            if (siteType == Enumerations.ReportTypes.Licensee && dailyComplaintsData.DailyComplaints.Count > 0)
            {
                var sites = (from q in dailyComplaintsData.DailyComplaints select q.Licensee).Distinct();
                foreach (var site in sites)
                {
                    var licenseeData =
                        (from q in dailyComplaintsData.DailyComplaints
                         where q.Licensee.ToLower().Equals(site.ToLower())
                         select q).ToList();
                    var dailyComplaintsDataFeachLicensee = new DailyComplaintsDataDTO();
                    dailyComplaintsDataFeachLicensee.DailyComplaints = licenseeData;
                    ExportDailyComplaintsDetail(dailyComplaintsDataFeachLicensee, tDate, excelFile, site, titleRefix.Replace("Licensee", site), tilteSubfix);
                }
            }
        }

        #region Export 2 Excel
        private void ExportDailyComplaintsDetail(DailyComplaintsDataDTO complaints, DateTime reportDate, ExcelFile excelFile, string sheetName, string titlePrefix, string tilteSubfix)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;

            var nextRow = 0;
            var wsDailyData =
                excelFile.Worksheets.Add(sheetName.Replace("N/A", "N_A"));
            #endregion

            #region Header
            ExportDailyComplaintsDetailAssignHeader(ref wsDailyData, startRow, startCol, reportDate, titlePrefix, tilteSubfix);
            #endregion

            #region Content
            ExportDailyComplaintsDetailAssignContent(ref wsDailyData, startRow, startCol, nextRow, reportDate, complaints);
            #endregion

            return;
        }

        private void ExportDailyComplaintsDetailAssignHeader(ref ExcelWorksheet wsDailyData, int startRow, int startCol, DateTime reportDate, string titleRefix, string tilteSubfix)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));

            #region sencond row
            var nextCol = 0;
            wsDailyData.Rows[startRow - 2].Height = 255 * 2;
            wsDailyData.Rows[startRow - 1].Height = 255 * 2;

            wsDailyData.Cells[startRow - 1, startCol].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "date");
            wsDailyData.Columns[startCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "Time");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "customer");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "site");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "CommunicationTools");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "ExtNo");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 10;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "ContactMSNSKypeEmail");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 30;

            wsDailyData.Cells[startRow - 1, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
            wsDailyData.Cells[startRow - 1, startCol + nextCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "product");
            wsDailyData.Columns[startCol + nextCol].Width = 255 * 15;

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

            #region first row - report title
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = bgHeaderMainFormat;
            wsDailyData.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                       "exportDailyComplaintsTitle"), titleRefix.ToUpper(), tilteSubfix);
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
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.Licensee;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CommunicationTool.ContactBy;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.ExtNo;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.CommunicationTool.Address;

                wsDailyData.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgFormat;
                wsDailyData.Cells[startRow + nextRow, startCol + nextCol].Value = detail.ProductName;

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
