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
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;
    using NexCel.CRM.CRMReportingService.DataAccess;
    using NexCel.CRM.CRMReportingService.Entities;

    public class DailySummary
    {
        public void GetFeedbackSummaryReport(ExcelFile excelFile, DateTime currentDay, bool isGetDaily)
        {
            if ((currentDay.DayOfWeek == DayOfWeek.Sunday || currentDay.Day == 1) && !isGetDaily)
            {
                Console.WriteLine("Get weekly Summary Report...");
                GetWeeklySummaryReportNew(excelFile, currentDay.AddDays(-1));
            }
            else
            {
                Console.WriteLine("Get daily Summary Report...");
                GetDailySummaryReport(excelFile, currentDay.AddDays(-1));
            }
        }

        #region daily report
        private void GetDailySummaryReport(ExcelFile excelFile, DateTime currentDay)
        {
            try
            {
                #region credit site
                var dailyCreditSiteSummary = GetDailySummaryReportData(1, currentDay, currentDay);
                ExportDailySummaryReport(dailyCreditSiteSummary, currentDay, excelFile);
                #endregion
                
                #region licenseese
                var dailyLicenseeseSiteSummary = GetDailySummaryReportData(1, currentDay, currentDay, Enumerations.ReportTypes.Licensee);
                ExportDailySummaryReport(dailyLicenseeseSiteSummary, currentDay, excelFile, Enumerations.ReportTypes.Licensee);
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteLog("--IBC Daily Summary-->" + ex.Message);
            }
        }

        private DailySummaryBO GetDailySummaryReportData(int reportType, DateTime firstDay, DateTime currentDay, Enumerations.ReportTypes siteType = Enumerations.ReportTypes.CreditSite)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var db = report.GetDailySummaryReport(reportType, firstDay, currentDay, siteType);

            var dailySummary = new DailySummaryBO();
            dailySummary.DailySummaries = new List<DailySummaryGroup>();
            dailySummary.ReportDay = currentDay;
            if (db != null && db.Tables.Count > 0 && db.Tables[0].Rows.Count > 0)
            {
                var startCol = 4;
                var tbData = db.Tables[0];
                var totalCols = tbData.Columns.Count;

                var oldMainId = 0;
                var oldMain = string.Empty;
                var subTotal = new int[totalCols];
                var grandTotal = new int[totalCols];

                #region set data for subtotal & grand total
                var dailyGroup = new DailySummaryGroup();
                dailyGroup.DailySummaries = new List<DailySummaryDetail>();
                foreach (DataRow dr in tbData.Rows)
                {
                    var currentMainId = CastAide.AsInt32(dr["MainCategoryID"].ToString());
                    var currentMain = CastAide.AsString(dr["MainCategory"]);
                    var currentSummary = new DailySummaryDetail();
                    currentSummary.Cells = new List<DailySummaryType>();
                    currentSummary.MainCategory = CastAide.AsString(dr["SubCategory"]);
                    #region assign data
                    if (currentMainId != oldMainId && oldMainId != 0)
                    {
                        var subTotalCells = new List<DailySummaryType>();
                        var cellsTotal = 0;
                        for (int j = startCol; j < totalCols; j++)
                        {
                            subTotalCells.Add(new DailySummaryType()
                            {
                                Title = CastAide.AsString(dr.Table.Columns[j].ColumnName),
                                Value = subTotal[j]
                            });
                            cellsTotal += subTotal[j];
                            subTotal[j] = 0;
                        }
                        var summaryDetail = new DailySummaryDetail()
                        {
                            MainCategory = oldMain,
                            MainCategoryId = oldMainId,
                            Cells = subTotalCells,
                            Total = cellsTotal
                        };
                        dailyGroup.Total = summaryDetail;
                        dailySummary.DailySummaries.Add(dailyGroup);
                        dailyGroup = new DailySummaryGroup();
                        dailyGroup.DailySummaries = new List<DailySummaryDetail>();
                    }
                    #endregion

                    for (int i = startCol; i < totalCols; i++)
                    {
                        subTotal[i] = subTotal[i] + CastAide.AsInt32(dr[i]);
                        grandTotal[i] = grandTotal[i] + CastAide.AsInt32(dr[i]);
                        
                        currentSummary.Cells.Add(new DailySummaryType()
                        {
                            Value = CastAide.AsInt32(dr[i])
                        });
                        currentSummary.Total += CastAide.AsInt32(dr[i]);

                    }
                    dailyGroup.DailySummaries.Add(currentSummary);
                    oldMainId = currentMainId;
                    oldMain = currentMain;
                }
                #endregion

                #region assign data for last subtotal & grand total
                var lastSubTotalCells = new List<DailySummaryType>();
                var grandTotalCells = new List<DailySummaryType>();
                var lastCellsTotal = 0;
                var cellsGrandTotal = 0;
                for (int j = startCol; j < totalCols; j++)
                {
                    var cellName = tbData.Columns[j].ColumnName;
                    lastSubTotalCells.Add(new DailySummaryType()
                    {
                        Title = CastAide.AsString(cellName),
                        Value = subTotal[j]
                    });
                    grandTotalCells.Add(new DailySummaryType()
                    {
                        Title = CastAide.AsString(cellName),
                        Value = grandTotal[j]
                    });
                    lastCellsTotal += subTotal[j];
                    cellsGrandTotal += grandTotal[j];
                }

                dailyGroup.Total = new DailySummaryDetail()
                {
                    MainCategory = oldMain,
                    MainCategoryId = oldMainId,
                    Cells = lastSubTotalCells,
                    Total = lastCellsTotal
                };
                dailySummary.DailySummaries.Add(dailyGroup);
                dailySummary.GrandTotal = new DailySummaryDetail()
                {
                    MainCategory = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "grandTotal"),
                    MainCategoryId = 0,
                    Cells = grandTotalCells,
                    Total = cellsGrandTotal
                };
                #endregion
            }

            return dailySummary;
        }
        #endregion

        #region weekly report
        #region old version
        private void GetWeeklySummaryReport(ExcelFile excelFile, DateTime currentDay)
        {
            try
            {
                var firstDay = Helper.GetFirstDayOfWeek(currentDay);
                var reportDate = firstDay.Date;
                var weeklySummary = new WeeklySummaryBO();
                weeklySummary.DailyComplaints = new List<DailySummaryBO>();
                #region get daily summary in week
                while (reportDate <= currentDay)
                {
                    var dailySummary = GetDailySummaryReportData(1, reportDate, reportDate);
                    weeklySummary.DailyComplaints.Add(dailySummary);
                    reportDate = reportDate.AddDays(1);
                }
                #endregion

                #region weekly summary - 01
                weeklySummary.FromDay = firstDay;
                weeklySummary.ToDay = currentDay;
                var weeklyStatistic = GetDailySummaryReportData(2, firstDay, currentDay);                
                weeklySummary.ComplaintsStatistic = weeklyStatistic;                
                #endregion

                #region weekly summary - licensee
                var weeklyStatisticByLincensee = GetWeeklyLicenseeSummaryReportData(firstDay, currentDay);                
                weeklySummary.LicenseesStatistic = weeklyStatisticByLincensee;
                #endregion

                ExportWeeklyummaryReport(weeklySummary, firstDay, currentDay, excelFile);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("--IBC Weekly Summary-->" + ex.Message);
            }
        }
        private DailySummaryBO GetWeeklyLicenseeSummaryReportData(DateTime firstDay, DateTime currentDay)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var db = report.GetLicenseeSummaryReport(firstDay, currentDay);

            var dailySummary = new DailySummaryBO();
            dailySummary.DailySummaries = new List<DailySummaryGroup>();
            dailySummary.ReportDay = currentDay;
            if (db != null && db.Tables.Count > 0 && db.Tables[0].Rows.Count > 0)
            {
                var startCol = 4;
                var tbData = db.Tables[0];
                var totalCols = tbData.Columns.Count;

                var oldMainId = 0;
                var oldMain = string.Empty;
                var subTotal = new int[totalCols];
                var grandTotal = new int[totalCols];

                #region set data for subtotal & grand total
                var dailyGroup = new DailySummaryGroup();
                dailyGroup.DailySummaries = new List<DailySummaryDetail>();
                foreach (DataRow dr in tbData.Rows)
                {
                    var currentMainId = CastAide.AsInt32(dr["MainCategoryID"].ToString());
                    var currentMain = CastAide.AsString(dr["MainCategory"]);
                    var currentSummary = new DailySummaryDetail();
                    currentSummary.Cells = new List<DailySummaryType>();
                    currentSummary.MainCategory = CastAide.AsString(dr["SubCategory"]);
                    #region assign data
                    if (currentMainId != oldMainId && oldMainId != 0)
                    {
                        var subTotalCells = new List<DailySummaryType>();
                        var cellsTotal = 0;
                        for (int j = startCol; j < totalCols; j++)
                        {
                            subTotalCells.Add(new DailySummaryType()
                            {
                                Title = CastAide.AsString(dr.Table.Columns[j].ColumnName),
                                Value = subTotal[j]
                            });
                            cellsTotal += subTotal[j];
                            subTotal[j] = 0;
                        }
                        var summaryDetail = new DailySummaryDetail()
                        {
                            MainCategory = oldMain,
                            MainCategoryId = oldMainId,
                            Cells = subTotalCells,
                            Total = cellsTotal
                        };
                        dailyGroup.Total = summaryDetail;
                        dailySummary.DailySummaries.Add(dailyGroup);
                        dailyGroup = new DailySummaryGroup();
                        dailyGroup.DailySummaries = new List<DailySummaryDetail>();
                    }
                    #endregion

                    for (int i = startCol; i < totalCols; i++)
                    {
                        subTotal[i] = subTotal[i] + CastAide.AsInt32(dr[i]);
                        grandTotal[i] = grandTotal[i] + CastAide.AsInt32(dr[i]);


                        currentSummary.Cells.Add(new DailySummaryType()
                        {
                            Value = CastAide.AsInt32(dr[i])
                        });
                    }
                    dailyGroup.DailySummaries.Add(currentSummary);
                    oldMainId = currentMainId;
                    oldMain = currentMain;
                }
                #endregion

                #region assign data for last subtotal & grand total
                var lastSubTotalCells = new List<DailySummaryType>();
                var grandTotalCells = new List<DailySummaryType>();
                var lastCellsTotal = 0;
                var cellsGrandTotal = 0;
                for (int j = startCol; j < totalCols; j++)
                {
                    var cellName = tbData.Columns[j].ColumnName;
                    lastSubTotalCells.Add(new DailySummaryType()
                    {
                        Title = CastAide.AsString(cellName),
                        Value = subTotal[j]
                    });
                    grandTotalCells.Add(new DailySummaryType()
                    {
                        Title = CastAide.AsString(cellName),
                        Value = grandTotal[j]
                    });
                    lastCellsTotal += subTotal[j];
                    cellsGrandTotal += grandTotal[j];
                }
                dailyGroup.Total = new DailySummaryDetail()
                {
                    MainCategory = oldMain,
                    MainCategoryId = oldMainId,
                    Cells = lastSubTotalCells,
                    Total = lastCellsTotal
                };
                dailySummary.DailySummaries.Add(dailyGroup);
                dailySummary.GrandTotal = new DailySummaryDetail()
                {
                    MainCategory = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "grandTotal"),
                    MainCategoryId = 0,
                    Cells = grandTotalCells,
                    Total = cellsGrandTotal
                };
                #endregion
            }

            return dailySummary;
        }
        #endregion

        private void GetWeeklySummaryReportNew(ExcelFile excelFile, DateTime currentDay)
        {
            try
            {
                var firstDay = Helper.GetFirstDayOfWeek(currentDay);
                var reportDate = firstDay.Date;
                var weeklySummary = new WeeklySummaryBO();
                weeklySummary.DailyComplaints = new List<DailySummaryBO>();
                #region get daily summary in week
                while (reportDate <= currentDay)
                {
                    reportDate = reportDate.AddDays(1);
                }
                #endregion

                #region credit site
                var dailyCreditSiteSummary = GetDailySummaryReportData(1, firstDay, currentDay);
                ExportWeeklySummaryReportNew(dailyCreditSiteSummary, firstDay, currentDay, excelFile);
                #endregion

                #region licenseese
                var dailyLicenseeseSiteSummary = GetDailySummaryReportData(1, firstDay, currentDay, Enumerations.ReportTypes.Licensee);
                ExportWeeklySummaryReportNew(dailyLicenseeseSiteSummary, firstDay, firstDay, excelFile, Enumerations.ReportTypes.Licensee);
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteLog("--IBC Daily Summary-->" + ex.Message);
            }
        }
        #endregion

        #region Export 2 Excel
        #region daily summary
        private void ExportDailySummaryReport(DailySummaryBO dailySummaryBO, DateTime reportDate, ExcelFile excelFile, Enumerations.ReportTypes siteType = Enumerations.ReportTypes.CreditSite)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;
            var nextRow = 0;
            var wsDailySummary =
                excelFile.Worksheets.Add(
                    string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                     "exportCreditSiteSummarySheet"),
                                  ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                     siteType.ToString()), "Daily"));
            
            #endregion

            #region Header
            //title

            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.Weight = ExcelFont.BoldWeight;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.
                Color = Color.Blue;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportDailySummaryTitle"),
                    "DAILY", string.Format("ON {0}", reportDate.ToString("MMMM dd, yyyy"))).ToUpper();
            ExportDailySummaryReportAssignHeader(ref wsDailySummary, startRow, startCol, ref nextRow, reportDate, dailySummaryBO, false);
            #endregion

            #region Content
            if (dailySummaryBO != null && dailySummaryBO.DailySummaries != null && dailySummaryBO.DailySummaries.Count > 0)
            {
                foreach (var dailySummaryDetail in dailySummaryBO.DailySummaries)
                {
                    ExportDailySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummaryDetail.Total, false, true);
                    foreach (var row in dailySummaryDetail.DailySummaries)
                    {
                        ExportDailySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, row, false, false);    
                    }
                    
                }
                ExportDailySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummaryBO.GrandTotal, true, false);

            }
            #endregion

            return;
        }

        private void ExportDailySummaryReportAssignHeader(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DateTime reportDate, DailySummaryBO dalySummaryBO, bool isLicensee)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));
            var mergeCols = 0;
            if (dalySummaryBO != null && dalySummaryBO.DailySummaries != null && dalySummaryBO.DailySummaries.Count > 0)
                mergeCols = dalySummaryBO.DailySummaries[0].Total.Cells.Count + 1;

            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Style = bgHeaderMainFormat;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Value = reportDate.ToString("MMMM dd, yyyy");

            if (mergeCols > 1)
            {
                nextRow++;
                var nextCol = 0;
                wsDailySummary.Columns[startCol].Width = 255 * 30;
                wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol].Value = isLicensee
                                                                               ? ResourceExtension.GetResourceValue(
                                                                                   ResourceExtension.Resources.Language,
                                                                                   "licensee")
                                                                               : ResourceExtension.GetResourceValue(
                                                                                   ResourceExtension.Resources.Language,
                                                                                   "exportComplaintsEnquiriesTitle");
                var cells = dalySummaryBO.DailySummaries[0].Total.Cells;

                for (int i = 0; i < cells.Count; i++)
                {
                    wsDailySummary.Cells[startRow + nextRow, startCol+ (nextCol+=1)].Style = bgHeaderNormalFormat;
                    wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Title;
                }

                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "grandTotal");
            }
            nextRow++;
        }

        private void ExportDailySummaryReportAssignContent(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DailySummaryDetail summaryDatail, bool isGrandTotal, bool isTotal)
        {
            var color = isGrandTotal ? Color.FromArgb(130, 199, 255) : (isTotal ? Color.FromArgb(245, 238, 211): Color.FromArgb(255, 255, 255));
            var bgNormalFormat = new ExcelUtils().GetStyleIntNumber("", color, isGrandTotal || isTotal, false);

            wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            wsDailySummary.Cells[startRow + nextRow, startCol].Value = string.Format("{0}{1}", ( isTotal || isGrandTotal ? "" : " - "), summaryDatail.MainCategory);
            var cells = summaryDatail.Cells;

            var nextCol = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;               
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Value;
            }
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = summaryDatail.Total;

            nextRow++;
        }
        #endregion

        #region weekly summary
        #region old
        private void ExportWeeklyummaryReport(WeeklySummaryBO weeklySummaryBO, DateTime firstDate, DateTime reportDate, ExcelFile excelFile)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;

            var nextRow = 0;
            var wsDailySummary =
                excelFile.Worksheets.Add(
                    string.Format(
                        ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                           "exportDailySummarySheet"), "Weekly"));
            #endregion

            #region Header
            //title
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.Weight = ExcelFont.BoldWeight;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.
                Color = Color.Blue;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportDailySummaryTitle"),
                    "WEEKLY",
                    string.Format("FROM {0} TO {1}", firstDate.ToString("dd MMMM yyyy"),
                                  reportDate.ToString("dd MMMM yyyy"))).ToUpper();
            #endregion

            #region Content
            if (weeklySummaryBO != null)
            {
                #region daily
                foreach (DailySummaryBO dailySummary in weeklySummaryBO.DailyComplaints)
                {
                    ExportWeeklySummaryReportAssignHeader(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummary.ReportDay, dailySummary, false, false);

                    if (weeklySummaryBO != null && dailySummary.DailySummaries != null && dailySummary.DailySummaries.Count > 0)
                    {
                        foreach (var dailySummaryDetail in dailySummary.DailySummaries)
                        {
                            ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummaryDetail.Total, false, true);
                            foreach (var row in dailySummaryDetail.DailySummaries)
                            {
                                ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, row, false, false);
                            }
                        }
                        //footer
                        ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummary.GrandTotal, true, false);
                        nextRow++;
                    }
                }
                #endregion

                #region weekly
                var totalMergeCols = 0;
                var bgHeaderFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));

                if (weeklySummaryBO.LicenseesStatistic != null && weeklySummaryBO.LicenseesStatistic.DailySummaries.Count > 0
                    && weeklySummaryBO.ComplaintsStatistic != null && weeklySummaryBO.ComplaintsStatistic.DailySummaries.Count > 0)
                    totalMergeCols += (weeklySummaryBO.ComplaintsStatistic.DailySummaries[0].Total.Cells.Count +
                        weeklySummaryBO.LicenseesStatistic.DailySummaries[0].Total.Cells.Count + 4);

                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + totalMergeCols).Merged = true;
                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + totalMergeCols).Style = bgHeaderFormat;
                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + totalMergeCols).Value = 
                    String.Format("Week {0}: {1} - {2}", Helper.GetWeekOfMonth(weeklySummaryBO.ToDay), weeklySummaryBO.FromDay.ToString("dd"),
                    weeklySummaryBO.ToDay.ToString("dd MMMM yyyy"));
                
                #region weekly statistc
                var weeklyNextRow = 0;
                var weeklyNextCol = startCol;
                if (weeklySummaryBO.ComplaintsStatistic != null && weeklySummaryBO.ComplaintsStatistic.DailySummaries != null && weeklySummaryBO.ComplaintsStatistic.DailySummaries.Count > 0)
                {
                    nextRow++;
                    weeklyNextRow = nextRow;//set current row for licensee 
                    weeklyNextCol = startCol + weeklySummaryBO.ComplaintsStatistic.DailySummaries[0].Total.Cells.Count + 3;

                    ExportWeeklySummaryReportAssignHeader(ref wsDailySummary, startRow, startCol, ref nextRow, weeklySummaryBO.ToDay, weeklySummaryBO.ComplaintsStatistic, true, false);
                    foreach (var weeklySummaryStatistic in weeklySummaryBO.ComplaintsStatistic.DailySummaries)
                    {
                        ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, weeklySummaryStatistic.Total, false, true);
                    }
                    //footer
                    ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, startCol, ref nextRow, weeklySummaryBO.ComplaintsStatistic.GrandTotal, true, true);
                }
                #endregion

                #region weekly statistc by licensee
                if (weeklySummaryBO.LicenseesStatistic != null && weeklySummaryBO.LicenseesStatistic.DailySummaries != null && weeklySummaryBO.LicenseesStatistic.DailySummaries.Count > 0)
                {
                    ExportWeeklySummaryReportAssignHeader(ref wsDailySummary, startRow, weeklyNextCol, ref weeklyNextRow, weeklySummaryBO.ToDay, weeklySummaryBO.LicenseesStatistic, true, true);
                    foreach (var weeklyStatisticByLincensee in weeklySummaryBO.LicenseesStatistic.DailySummaries)
                    {
                        ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, weeklyNextCol, ref weeklyNextRow, weeklyStatisticByLincensee.Total, false, true);
                    }
                    //footer
                    ExportWeeklySummaryReportAssignContent(ref wsDailySummary, startRow, weeklyNextCol, ref weeklyNextRow, weeklySummaryBO.LicenseesStatistic.GrandTotal, true, true);
                }
                #endregion

                #endregion
            }
            #endregion

            return;
        }

        private void ExportWeeklySummaryReportAssignHeader(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DateTime reportDate, DailySummaryBO dalySummaryBO, bool isWeekly, bool isLincensee)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));
            var MergeCols = 0;
            wsDailySummary.Columns[startCol].Width = 255 * 30;

            if (dalySummaryBO != null && dalySummaryBO.DailySummaries != null && dalySummaryBO.DailySummaries.Count > 0)
                MergeCols = dalySummaryBO.DailySummaries[0].Total.Cells.Count + 1;

            if (!isWeekly)
            {
                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Merged = true;
                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Style = bgHeaderMainFormat;
                wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Value = reportDate.ToString("MMMM dd, yyyy");
                nextRow++;
            }

            if (MergeCols > 1)
            {                
                var nextCol = 0;
                
                wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol].Value =
                    isLincensee ? ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "licensee")
                : ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportComplaintsEnquiriesTitle");
                var cells = dalySummaryBO.DailySummaries[0].Total.Cells;

                for (int i = 0; i < cells.Count; i++)
                {
                    wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
                    wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Title;
                }

                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "grandTotal");
            }
            else
            {
                wsDailySummary.Cells[startRow + nextRow, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "thereisnodata");
                nextRow ++;
            }
            nextRow++;
        }

        private void ExportWeeklySummaryReportAssignContent(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DailySummaryDetail summaryDatail, bool isGrandTotal, bool isTotal)
        {
            var color = isGrandTotal ? Color.FromArgb(130, 199, 255) : (isTotal ? Color.FromArgb(245, 238, 211) : Color.FromArgb(255, 255, 255));
            var bgNormalFormat = new ExcelUtils().GetStyleIntNumber("", color, isGrandTotal || isTotal, false);

            wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            wsDailySummary.Cells[startRow + nextRow, startCol].Value = string.Format("{0}{1}", (isTotal || isGrandTotal ? "" : " - "), summaryDatail.MainCategory);
            var cells = summaryDatail.Cells;

            var nextCol = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Value;
            }
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = summaryDatail.Total;

            nextRow++;
        }
        #endregion

        #region new
        private void ExportWeeklySummaryReportNew(DailySummaryBO dailySummaryBO, DateTime fromDate, DateTime toDate, ExcelFile excelFile, Enumerations.ReportTypes siteType = Enumerations.ReportTypes.CreditSite)
        {
            #region private variables
            var startCol = 0;
            var startRow = 2;
            var nextRow = 0;
            var wsDailySummary =
                excelFile.Worksheets.Add(
                    string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                     "exportCreditSiteSummarySheet"),
                                  ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language,
                                                                     siteType.ToString()), "Weekly"));

            #endregion

            #region Header
            //title

            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.Weight = ExcelFont.BoldWeight;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.
                Color = Color.Blue;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportDailySummaryTitle"),
                    "WEEKLY", string.Format("FROM {0} TO {1}", fromDate.ToString("dd MMMM yyyy"), toDate.ToString("dd MMMM yyyy"))).ToUpper();
            ExportWeeklySummaryReportAssignHeaderNew(ref wsDailySummary, startRow, startCol, ref nextRow, toDate, dailySummaryBO, false);
            #endregion

            #region Content
            if (dailySummaryBO != null && dailySummaryBO.DailySummaries != null && dailySummaryBO.DailySummaries.Count > 0)
            {
                #region all data in week
                foreach (var dailySummaryDetail in dailySummaryBO.DailySummaries)
                {
                    //total - for each main category
                    ExportWeeklySummaryReportAssignContentNew(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummaryDetail.Total, false, true);
                    foreach (var row in dailySummaryDetail.DailySummaries)
                    {
                        //detail
                        ExportWeeklySummaryReportAssignContentNew(ref wsDailySummary, startRow, startCol, ref nextRow, row, false, false);
                    }

                }
                //grand total
                ExportWeeklySummaryReportAssignContentNew(ref wsDailySummary, startRow, startCol, ref nextRow, dailySummaryBO.GrandTotal, true, false);
                #endregion
            }
            #endregion

            return;
        }

        private void ExportWeeklySummaryReportAssignHeaderNew(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DateTime reportDate, DailySummaryBO dalySummaryBO, bool isLicensee)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));
            var mergeCols = 0;
            if (dalySummaryBO != null && dalySummaryBO.DailySummaries != null && dalySummaryBO.DailySummaries.Count > 0)
                mergeCols = dalySummaryBO.DailySummaries[0].Total.Cells.Count + 1;

            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Style = bgHeaderMainFormat;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCols).Value = reportDate.ToString("MMMM dd, yyyy");

            if (mergeCols > 1)
            {
                nextRow++;
                var nextCol = 0;
                wsDailySummary.Columns[startCol].Width = 255 * 30;
                wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol].Value = isLicensee
                                                                               ? ResourceExtension.GetResourceValue(
                                                                                   ResourceExtension.Resources.Language,
                                                                                   "licensee")
                                                                               : ResourceExtension.GetResourceValue(
                                                                                   ResourceExtension.Resources.Language,
                                                                                   "exportComplaintsEnquiriesTitle");
                var cells = dalySummaryBO.DailySummaries[0].Total.Cells;

                for (int i = 0; i < cells.Count; i++)
                {
                    wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
                    wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Title;
                }

                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "grandTotal");
            }
            nextRow++;
        }

        private void ExportWeeklySummaryReportAssignContentNew(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DailySummaryDetail summaryDatail, bool isGrandTotal, bool isTotal)
        {
            var color = isGrandTotal ? Color.FromArgb(130, 199, 255) : (isTotal ? Color.FromArgb(245, 238, 211) : Color.FromArgb(255, 255, 255));
            var bgNormalFormat = new ExcelUtils().GetStyleIntNumber("", color, isGrandTotal || isTotal, false);

            wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            wsDailySummary.Cells[startRow + nextRow, startCol].Value = string.Format("{0}{1}", (isTotal || isGrandTotal ? "" : " - "), summaryDatail.MainCategory);
            var cells = summaryDatail.Cells;

            var nextCol = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = cells[i].Value;
            }
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol + (nextCol)].Value = summaryDatail.Total;

            nextRow++;
        }
        #endregion
        #endregion
        #endregion
    }
}

