namespace NexCel.CRM.CRMReportingService.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;

    using DNAide.Core;

    using ExcelUtil.Spreadsheet;

    using NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Extensions;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;
    using NexCel.CRM.CRMReportingService.DataAccess;
    using NexCel.CRM.CRMReportingService.Entities;

    public class CasinoDailySummaryReport
    {
        public void GetCasinoSummaryReport(ExcelFile excelFile, DateTime currentDate, bool isGetDaily)
        {
            if ((currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.Day == 1)&& !isGetDaily)
                GetCasinoWeeklySummaryReport(excelFile, currentDate.AddDays(-1));
            else
                GetCasinoDailySummaryReport(excelFile, currentDate.AddDays(-1), currentDate.AddDays(-1));
        }

        #region daily summary
        private void GetCasinoDailySummaryReport(ExcelFile excelFile, DateTime currentDay, DateTime toDate)
        {
            Console.WriteLine("Get daily casino summary report...");
            var result = GetCasinoDailySummary(1, currentDay, toDate);
            ExportCasinoDailySummary(result, currentDay, excelFile);
        }
        #endregion

        #region weekly summary
        private void GetCasinoWeeklySummaryReport(ExcelFile excelFile, DateTime currentDay)
        {
            Console.WriteLine("Get weekly casino summary report...");
            var firstDate = Helper.GetFirstDayOfWeek(currentDay);
            var reportDate = firstDate.Date;
            var weeklySummary = new CasinoWeeklySummary();
            weeklySummary.CaisinoWeeklySummaries = new CasinoDailySummary();
            weeklySummary.CasinoDailySummaries = new List<CasinoDailySummary>();
            weeklySummary.WeeklySummary = new DailySummaryBO();

            weeklySummary.FromDate = firstDate;
            weeklySummary.ToDate = currentDay;
            #region daily
            while (reportDate <= currentDay)
            {
                var dailySummary = GetCasinoDailySummary(1, reportDate, reportDate);
                weeklySummary.CasinoDailySummaries.Add(dailySummary);
                reportDate = reportDate.AddDays(1);
            }
            #endregion

            #region weekly summary - detail
            weeklySummary.CaisinoWeeklySummaries = GetCasinoDailySummary(2, firstDate, currentDay);
            #endregion

            #region weekly summary

            var weeklySummaryBySystem = GetDailySummaryReportData(3, firstDate, currentDay);
            weeklySummary.WeeklySummary = weeklySummaryBySystem;
            #endregion
            //export
            ExportCasinoWeeklSummary(weeklySummary, firstDate, currentDay, excelFile);
        }

        #endregion

        #region private functions
        private CasinoDailySummary GetCasinoDailySummary(int reportType, DateTime fromDate, DateTime toDate)
        {
            var result = new CasinoDailySummary();
            try
            {
                result.DailySummary = new List<CasinoDailySummaryDetail>();
                result.SumAllFeedbacks = new List<int>();
                result.ReportDate = fromDate;
                var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
                var getData = report.GetCasinoSummaryDailyReport(reportType, fromDate, toDate);

                if (getData != null  && getData.Tables.Count > 0 &&
                    getData.Tables[0].Rows != null && getData.Tables[0].Rows.Count > 0)
                {
                    var cPrefix = CasinoSection.Instance[CasinoSection.SystemConfigCollection.Systems];
                    var cSubfix = CasinoSection.Instance[CasinoSection.SystemConfigCollection.Communications];
                    var tbData = getData.Tables[0];
                    var oldMainId = 0;
                    var oldMain = string.Empty;
                    var systemSubTotal = new List <SystemDailySummary>();
                    var systemGrandTotal = new List<SystemDailySummary>();
                    #region add systems group

                    for (var i = 0; i < cPrefix.Count; i++)
                    {
                        result.SumAllFeedbacks.Add(new int());
                        var cells = new List<DailySummaryType>();
                        for (var j = 0; j < cSubfix.Count; j++)
                        {
                            cells.Add(new DailySummaryType() {Value = 0});
                        }

                        systemSubTotal.Add(new SystemDailySummary()
                                               {
                                                   Sytem = cPrefix[i].Name,
                                                   SystemId = cPrefix[i].Id,
                                                   Cells = cells
                                               });
                        var cells2 = new List<DailySummaryType>();
                        for (var j = 0; j < cSubfix.Count; j++)
                        {
                            cells2.Add(new DailySummaryType() { Value = 0 });
                        }
                        systemGrandTotal.Add(new SystemDailySummary()
                                                 {
                                                     Sytem = cPrefix[i].Name,
                                                     SystemId = cPrefix[i].Id,
                                                     Cells = cells2
                                                 });
                    }
                    
                    #endregion

                    #region assign data for each item
                    foreach (DataRow dr in tbData.Rows)
                    {
                        var currentMainId = CastAide.AsInt32(dr["MainCategoryID"].ToString());
                        var currentMain = CastAide.AsString(dr["MainCategory"]);
                        #region assign data
                        
                        if (currentMainId != oldMainId && oldMainId != 0)
                        {
                           var  summary = new CasinoDailySummaryDetail()
                                              {
                                                  MainCategory = oldMain,
                                                  MainCategoryId = oldMainId,
                                                  SystemSummary = systemSubTotal
                                              };
                            
                            result.DailySummary.Add(summary);

                            systemSubTotal = new List<SystemDailySummary>();
                            for (var i = 0; i < cPrefix.Count; i++)
                            {
                                var cells = new List<DailySummaryType>();
                                for (var j = 0; j < cSubfix.Count; j++)
                                    cells.Add(new DailySummaryType(){Value = 0});

                                systemSubTotal.Add(new SystemDailySummary()
                                {
                                    Sytem = cPrefix[i].Name,
                                    SystemId = cPrefix[i].Id,
                                    Cells = cells
                                });
                            }
                        }
                        #endregion

                        for (var j = 0; j < cPrefix.Count; j++)
                        {
                            for (var k = 0; k < cSubfix.Count; k++)
                            {
                                var currentVal =
                                    CastAide.AsInt32(dr[string.Format("{0}{1}", cPrefix[j].Prefix, cSubfix[k].Subfix)]);

                                systemSubTotal[j].Cells[k].Title = cSubfix[k].Subfix;
                                systemSubTotal[j].Cells[k].Value = systemSubTotal[j].Cells[k].Value + currentVal;

                                systemGrandTotal[j].Cells[k].Title = cSubfix[k].Subfix;
                                systemGrandTotal[j].Cells[k].Value = systemGrandTotal[j].Cells[k].Value + currentVal;
                                
                                result.SumAllFeedbacks[j] += currentVal;
                            }
                        }
                        
                        oldMainId = currentMainId;
                        oldMain = currentMain;
                    }
                    #endregion

                    #region last subtotal & grand total
                    result.DailySummary.Add(new CasinoDailySummaryDetail()
                    {
                        MainCategory = oldMain,
                        MainCategoryId = oldMainId,
                        SystemSummary = systemSubTotal
                    });


                    result.GrandTotal = new CasinoDailySummaryDetail()
                                            {
                                                MainCategory = "GrandTotal",
                                                MainCategoryId = 0,
                                                SystemSummary = systemGrandTotal
                                            };

                    #endregion
                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex.Message);
            }
            return result;
        }

        private DailySummaryBO GetDailySummaryReportData(int reportType, DateTime firstDay, DateTime currentDay)
        {
            var report = CommonBootStrapper.Locator.GetInstance<IReportDAO>();
            var db = report.GetCasinoSummaryDailyReport(reportType, firstDay, currentDay);

            var dailySummary = new DailySummaryBO();
            dailySummary.DailySummaries = new List<DailySummaryGroup>();
            dailySummary.ReportDay = currentDay;
            if (db != null && db.Tables.Count > 0 && db.Tables[0].Rows.Count > 0)
            {
                var startCol = 2;
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

        #region Export 2 excel
        private void ExportCasinoDailySummary(CasinoDailySummary casinoDailySummary, DateTime reportDate, ExcelFile excelFile)
        {
            #region private variables

            var startCol = 0;
            var startRow = 2;
            var nextRow = 0;
            var wsReport = excelFile.Worksheets.Add(string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportCasinoDailySummarySheet"), "Daily"));
            #endregion

            #region Header

            var subfixTitle = string.Format("ON {0}", reportDate.ToString("MMMM dd, yyyy"));
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Merged = true;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.Weight = ExcelFont.BoldWeight;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.
                Color = Color.Blue;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportCasinoDailySummaryTitle"),
                    "DAILY", subfixTitle).ToUpper();
            #endregion

            if (casinoDailySummary.DailySummary != null && casinoDailySummary.DailySummary.Count > 0)
            {
                #region Content
                ExportCasinoDailySummaryAssignHeader(ref wsReport, startRow, startCol, ref nextRow, reportDate,
                                                     casinoDailySummary, false);
                foreach (CasinoDailySummaryDetail dailySummary in casinoDailySummary.DailySummary)
                {
                    ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow, dailySummary,
                                                          false);
                }

                #endregion

                #region footer

                ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow,
                                                      casinoDailySummary.GrandTotal, true);

                ExportCasinoDailySummaryAssignTotal(ref wsReport, startRow, startCol, ref nextRow,
                                                    casinoDailySummary.GrandTotal, casinoDailySummary.SumAllFeedbacks);
                

                #endregion
            }
            else
                wsReport.Cells[startRow + nextRow, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "thereisnodata");
            
            nextRow++;
        }

        private void ExportCasinoDailySummaryAssignHeader(ref ExcelWorksheet wsReport, int startRow, int startCol, ref int nextRow, DateTime reportDate, CasinoDailySummary casinoDailySummary, bool isWeekly)
        {
            var weeklyHeaderColor = isWeekly ? Color.FromArgb(30, 134, 218) : Color.FromArgb(130, 199, 255);
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(weeklyHeaderColor);
            var mergeCol = 0;

            wsReport.Columns[startCol].Width = 255 * 30;
            #region Merge the header
            if (casinoDailySummary!= null && casinoDailySummary.DailySummary!= null && casinoDailySummary.DailySummary.Count > 0)
            {
                var totalCells = 0;
                for (int i = 0; i < casinoDailySummary.DailySummary[0].SystemSummary.Count; i++)
                {
                    totalCells += casinoDailySummary.DailySummary[0].SystemSummary[i].Cells.Count;
                }
                mergeCol = totalCells;
            }
            #endregion

            var reportTitle = isWeekly
                                  ? string.Format("Week {0}: {1} - {2}", Helper.GetWeekOfMonth(reportDate),
                                                  Helper.GetFirstDayOfWeek(reportDate).ToString("dd"),
                                                  reportDate.ToString("dd MMMM yyyy"))
                                  : reportDate.ToString("MMMM dd, yyyy");

            wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCol).Merged = true;
            wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCol).Style = bgHeaderMainFormat;
            wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + mergeCol).Value = reportTitle;

            nextRow++;
            if(mergeCol>1)
            {
                var nextCol = 0;

                #region left col - main cateogry

                wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + nextCol, startRow + nextRow + 1,
                                                   startCol + nextCol).Merged = true;
                wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + nextCol, startRow + nextRow + 1,
                                                   startCol + nextCol).Style = bgHeaderNormalFormat;
                wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + nextCol, startRow + nextRow + 1,
                                                   startCol + nextCol).Value =
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "category");

                #endregion

                #region right cols - system data

                for (int i = 0; i < casinoDailySummary.DailySummary[0].SystemSummary.Count; i++)
                {
                    var systemSummary = casinoDailySummary.DailySummary[0].SystemSummary[i];
                    var totalMergeCols = systemSummary.Cells.Count -1;

                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol += 1), startRow + nextRow,
                                                       startCol + nextCol + totalMergeCols).Merged = true;
                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol), startRow + nextRow,
                                                       startCol + nextCol + totalMergeCols).Style = bgHeaderNormalFormat;
                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol), startRow + nextRow,
                                                       startCol + nextCol + totalMergeCols).Value = systemSummary.Sytem;


                    var nextCol2 = 0;
                    for (int j = 0; j < systemSummary.Cells.Count; j++)
                    {
                        wsReport.Cells[startRow + nextRow + 1, startCol + (nextCol2) + nextCol].Style = bgHeaderNormalFormat;
                        wsReport.Cells[startRow + nextRow + 1, startCol + (nextCol2) + nextCol].Value =
                            systemSummary.Cells[j].Title;
                        nextCol2 += 1;
                    }

                    nextCol += totalMergeCols;
                }
                nextRow++;

                #endregion
            }
            else
            {
                wsReport.Cells[startRow + nextRow, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "thereisnodata");
            }
            nextRow++;
        }

        private void ExportCasinoDailySummaryAssignContent(ref ExcelWorksheet wsReport, int startRow, int startCol, ref int nextRow, CasinoDailySummaryDetail summaryDetail, bool isGrandTotal)
        {
            if (summaryDetail != null && summaryDetail.SystemSummary.Count > 0)
            {
                var color = isGrandTotal ? Color.FromArgb(205, 222, 255) : Color.FromArgb(255, 255, 255);
                var bgNormalFormat = new ExcelUtils().GetStyleIntNumber("", color, isGrandTotal, false);
                wsReport.Cells[startRow + nextRow, startCol].Style = bgNormalFormat;
                wsReport.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                wsReport.Cells[startRow + nextRow, startCol].Value = summaryDetail.MainCategory;

                var nextCol = 0;
                for (int i = 0; i < summaryDetail.SystemSummary.Count; i++)
                {
                    var systemSummary = summaryDetail.SystemSummary[i];
                    for (int j = 0; j < systemSummary.Cells.Count; j++)
                    {
                        wsReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = bgNormalFormat;
                        wsReport.Cells[startRow + nextRow, startCol + nextCol].Value =
                            systemSummary.Cells[j].Value;
                    }
                }

                nextRow++;
            }
        }

        private  void ExportCasinoDailySummaryAssignTotal(ref ExcelWorksheet wsReport, int startRow, int startCol, ref int nextRow, CasinoDailySummaryDetail grandTotal, List<int> groupTotal)
        {
            if (grandTotal != null && grandTotal.SystemSummary != null)
            {
                var nextCol = 0;
                var bgNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(255, 255, 255));
                wsReport.Cells[startRow + nextRow, startCol + nextCol].Style = bgNormalFormat;
                bgNormalFormat.Font.Color = Color.Blue;

                for (int i = 0; i < grandTotal.SystemSummary.Count; i++)
                {
                    var mergeCols = grandTotal.SystemSummary[i].Cells.Count - 1;
                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol += 1), startRow + nextRow,
                                                       startCol + nextCol + mergeCols).Merged = true;
                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol), startRow + nextRow,
                                                       startCol + nextCol + mergeCols).Style = bgNormalFormat;
                    wsReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol + (nextCol), startRow + nextRow,
                                                       startCol + nextCol + mergeCols).Value = groupTotal[i];
                    nextCol += mergeCols;
                }
                nextRow++;
            }
        }

        private  void ExportCasinoWeeklSummary(CasinoWeeklySummary weeklySummary, DateTime firstDate, DateTime reportDate, ExcelFile excelFile)
        {
            #region private variables

            var startCol = 0;
            var startRow = 2;
            var nextRow = 0;
            var wsReport = excelFile.Worksheets.Add(string.Format(ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportCasinoDailySummarySheet"), "Weekly"));
            #endregion

            #region Header
            var subfixTitle = string.Format("FROM {0} TO {1}", firstDate.ToString("dd MMMM yyyy"), reportDate.ToString("dd MMMM yyyy"));
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Merged = true;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.Weight = ExcelFont.BoldWeight;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Style.Font.
                Color = Color.Blue;
            wsReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + 8).Value =
                string.Format(
                    ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportCasinoDailySummaryTitle"),
                    "WEEKLY", subfixTitle).ToUpper();
            #endregion

            #region content
            
            if (weeklySummary != null && weeklySummary.CaisinoWeeklySummaries!= null && weeklySummary.WeeklySummary.DailySummaries.Count > 0)
            {
                #region daily
                foreach (CasinoDailySummary summaryDaily in weeklySummary.CasinoDailySummaries)
                {
                    //header
                    ExportCasinoDailySummaryAssignHeader(ref wsReport, startRow, startCol, ref nextRow, summaryDaily.ReportDate, summaryDaily, false);
                    //content
                    foreach (CasinoDailySummaryDetail dailySummary in summaryDaily.DailySummary)
                    {
                        ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow, dailySummary, false);
                    }
                    //footer
                    ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow, summaryDaily.GrandTotal, true);
                    ExportCasinoDailySummaryAssignTotal(ref wsReport, startRow, startCol, ref nextRow, summaryDaily.GrandTotal, summaryDaily.SumAllFeedbacks);
                    nextRow++;

                }
                #endregion

                #region weekly
                ExportCasinoDailySummaryAssignHeader(ref wsReport, startRow, startCol, ref nextRow, reportDate, weeklySummary.CaisinoWeeklySummaries, true);
                foreach (CasinoDailySummaryDetail dailySummary in weeklySummary.CaisinoWeeklySummaries.DailySummary)
                {
                    ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow, dailySummary, false);
                }
                ExportCasinoDailySummaryAssignContent(ref wsReport, startRow, startCol, ref nextRow, weeklySummary.CaisinoWeeklySummaries.GrandTotal, true);
                ExportCasinoDailySummaryAssignTotal(ref wsReport, startRow, startCol, ref nextRow, weeklySummary.CaisinoWeeklySummaries.GrandTotal, weeklySummary.CaisinoWeeklySummaries.SumAllFeedbacks);
                nextRow++;

                #endregion

                #region system

                wsReport.Cells[startRow + nextRow, startCol].Value = string.Format("Week {0}", Helper.GetWeekOfMonth(reportDate));
                nextRow++;

                ExportCasinoWeeklySummaryBySystemAssignHeader(ref wsReport, startRow, startCol, ref nextRow, reportDate, weeklySummary.WeeklySummary);
                if (weeklySummary.WeeklySummary != null && weeklySummary.WeeklySummary.DailySummaries != null && weeklySummary.WeeklySummary.GrandTotal != null)
                {
                    foreach (DailySummaryGroup dailySummary in weeklySummary.WeeklySummary.DailySummaries)
                    {
                        ExportCasinoWeeklySummaryBySystemAssignContent(ref wsReport, startRow, startCol, ref nextRow,
                                                                       dailySummary.Total, false);
                    }
                    ExportCasinoWeeklySummaryBySystemAssignContent(ref wsReport, startRow, startCol, ref nextRow,
                                                                   weeklySummary.WeeklySummary.GrandTotal, true);
                }

                #endregion
            }
            #endregion
        }

        private void ExportCasinoWeeklySummaryBySystemAssignHeader(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DateTime reportDate, DailySummaryBO dalySummaryBO)
        {
            var bgHeaderNormalFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(205, 222, 255));
            var bgHeaderMainFormat = new ExcelUtils().GetStyleTitle(Color.FromArgb(130, 199, 255));
            var MergeCols = 0;
            if (dalySummaryBO != null && dalySummaryBO.DailySummaries != null && dalySummaryBO.DailySummaries.Count > 0)
                MergeCols = dalySummaryBO.DailySummaries[0].Total.Cells.Count + 1;

            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Merged = true;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Style = bgHeaderMainFormat;
            wsDailySummary.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + MergeCols).Value = reportDate.ToString("MMMM yyyy");

            if (MergeCols > 1)
            {
                nextRow++;
                var nextCol = 0;
                wsDailySummary.Columns[startCol].Width = 255 * 30;
                wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgHeaderNormalFormat;
                wsDailySummary.Cells[startRow + nextRow, startCol].Value = ResourceExtension.GetResourceValue(ResourceExtension.Resources.Language, "exportComplaintsEnquiriesTitle");
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
        private void ExportCasinoWeeklySummaryBySystemAssignContent(ref ExcelWorksheet wsDailySummary, int startRow, int startCol, ref int nextRow, DailySummaryDetail summaryDatail, bool isGrandTotal)
        {
            var color = isGrandTotal ? Color.FromArgb(130, 199, 255) : Color.FromArgb(255, 255, 255);
            var bgNormalFormat = new ExcelUtils().GetStyleIntNumber("", color, isGrandTotal, false);

            wsDailySummary.Cells[startRow + nextRow, startCol].Style = bgNormalFormat;
            wsDailySummary.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            wsDailySummary.Cells[startRow + nextRow, startCol].Value = summaryDatail.MainCategory;
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
    }
}