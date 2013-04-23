using System.Drawing;
using ExcelUtil.Spreadsheet;

namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper
{
    public class ExcelUtils
    {
        public CellStyle GetStyleIntNumber(string symbol, Color bgColor, bool isBold, bool isPossitiveColor)
        {
            var style = new CellStyle();
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            style.NumberFormat = string.Format("{0}#,##0{1}_);-#,##0{2}", isPossitiveColor ? "[RED]" : "", symbol, symbol);
            style.FillPattern.SetSolid(bgColor);
            style.Font.Name = "Tahoma";
            style.Font.Size = 160;
            if (isBold)
                style.Font.Weight = ExcelFont.BoldWeight;

            return style;
        }

        public CellStyle GetStyleIntNumber(string symbol, Color bgColor, Color fontColor)
        {
            var style = new CellStyle();
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            style.NumberFormat = "#,##0" + symbol + "_);(#,##0)" + symbol;
            style.FillPattern.SetSolid(bgColor);
            style.Font.Name = "Tahoma";
            style.Font.Size = 160;

            style.Font.Weight = ExcelFont.BoldWeight;
            style.Font.Color = fontColor;

            return style;
        }

        public CellStyle GetStyleNumberCenter(decimal num, bool isEven, string symbol)
        {
            var style = new CellStyle();
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            style.Font.Name = "Tahoma";
            style.Font.Size = 160;
            if (isEven)
                style.FillPattern.SetSolid(Color.FromArgb(223, 234, 255));
            if (num < 0)
                style.Font.Color = Color.Red;
            return style;
        }

        public CellStyle GetStyleTitle(Color color)
        {
            var celltitle = new CellStyle();
            celltitle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            celltitle.VerticalAlignment = VerticalAlignmentStyle.Center;
            celltitle.FillPattern.SetSolid(color);
            celltitle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            celltitle.Font.Color = Color.Black;
            celltitle.Font.Name = "Tahoma";
            celltitle.Font.Size = 160;
            celltitle.Font.Weight = ExcelFont.BoldWeight;
            celltitle.WrapText = true;
            return celltitle;
        }

        public CellStyle GetStyleTitle(Color color, HorizontalAlignmentStyle horizontal, int fontBold)
        {
            var celltitle = new CellStyle();
            celltitle.HorizontalAlignment = horizontal;
            celltitle.VerticalAlignment = VerticalAlignmentStyle.Center;
            celltitle.FillPattern.SetSolid(color);
            celltitle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            celltitle.Font.Color = Color.Black;
            celltitle.Font.Name = "Tahoma";
            celltitle.Font.Size = 160;
            celltitle.Font.Weight = fontBold;
            celltitle.WrapText = true;
            return celltitle;
        }
    }
}
