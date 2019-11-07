/*
 * MIT License
 *
 * Copyright(c) 2019 kelicto
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: kelicto                       |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using KeLi.Common.Converter.Converter;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KeLi.Common.Drive.Excel
{
    /// <summary>
    /// A excel assist.
    /// </summary>
    public static class ExcelAssist
    {
        /// <summary>
        /// Reads the excel to the two dimension array.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object[,] As2DArray(this ExcelParam param)
        {
            var fileInfo = new FileInfo(param.FilePath);

            using (var excel = new ExcelPackage(fileInfo))
            {
                var sheets = excel.Workbook.Worksheets;
                var sheet = (param.SheetName == null ? sheets.FirstOrDefault()
                                : sheets[param.SheetName]) ?? sheets.FirstOrDefault();

                if (!(sheet?.Cells.Value is object[,]))
                    return new object[0, 0];

                return (object[,])sheet.Cells.Value;
            }
        }

        /// <summary>
        /// Reads the excel to the cross array.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object[][] AsCrossArray(this ExcelParam param)
        {
            return param.As2DArray().Convert();
        }

        /// <summary>
        ///  Reads the excel to the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> AsList<T>(this ExcelParam param)
        {
            var fileInfo = new FileInfo(param.FilePath);
            var results = new List<T>();
            var ps = typeof(T).GetProperties();

            using (var excel = new ExcelPackage(fileInfo))
            {
                var sheets = excel.Workbook.Worksheets;
                var sheet = (param.SheetName == null ? sheets.FirstOrDefault()
                        : sheets[param.SheetName]) ?? sheets.FirstOrDefault();

                if (!(sheet?.Cells.Value is object[,] cells))
                    return new List<T>();

                for (var i = param.RowIndex; i < sheet.Dimension.Rows; i++)
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));

                    for (var j = param.ColumnIndex; j < typeof(T).GetProperties().Length + param.ColumnIndex; j++)
                    {
                        var columnName = cells[0, j]?.ToString();
                        var pls = ps.Where(w => w.GetDcrp().Equals(columnName) || w.Name.Equals(cells[param.RowIndex, j]));

                        foreach (var p in pls)
                        {
                            var val = Convert.ChangeType(cells[i, j], p.PropertyType);

                            p.SetValue(obj, cells[i, j] != DBNull.Value ? val : null, null);
                            break;
                        }
                    }

                    results.Add(obj);
                }
            }

            return results;
        }

        /// <summary>
        /// Reads the excel to the data table.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable AsDataTable(this ExcelParam param)
        {
            var fileInfo = new FileInfo(param.FilePath);
            var results = new DataTable();

            using (var excel = new ExcelPackage(fileInfo))
            {
                var sheets = excel.Workbook.Worksheets;
                var sheet = (param.SheetName == null ? sheets.FirstOrDefault()
                                : sheets[param.SheetName]) ?? sheets.FirstOrDefault();

                if (!(sheet?.Cells.Value is object[,] cells))
                    return new DataTable();

                for (var j = param.ColumnIndex; j < sheet.Dimension.Columns; j++)
                    results.Columns.Add(new DataColumn(cells[0, j]?.ToString()));

                for (var i = param.RowIndex; i < sheet.Dimension.Rows; i++)
                    for (var j = param.ColumnIndex; j < sheet.Dimension.Columns; j++)
                        results.Rows[i - param.RowIndex][j] = cells[i + param.RowIndex, j];
            }

            return results;
        }

        /// <summary>
        /// Writes the list to the excel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="param"></param>
        /// <param name="createHeader"></param>
        public static void ToExcel<T>(this ExcelParam param, List<T> objs, bool createHeader = true)
        {
            // If exists, auto width setting will throw exception.
            if (File.Exists(param.FilePath))
                File.Delete(param.FilePath);

            File.Copy(param.TemplatePath, param.FilePath);

            // Epplus dll write excel file that column index from 1 to end column index and row index from 0 to end row index.
            param.ColumnIndex += 1;

            var excel = param.GetExcelPackage(out var sheet);
            var ps = typeof(T).GetProperties();

            // The titlt row.
            if (createHeader)
                for (var i = 0; i < ps.Length; i++)
                    sheet.Cells[param.RowIndex, i + param.ColumnIndex].Value = ps[i].GetDcrp();

            // The content row.
            for (var i = 0; i < objs.Count; i++)
                for (var j = 0; j < ps.Length; j++)
                    sheet.Cells[i + param.RowIndex + 1, j + param.ColumnIndex].Value = ps[j].GetValue(objs[i]);

            sheet.SetExcelStyle();
            excel.Save();
        }

        /// <summary>
        /// Writes the cross array to the excel.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ExcelPackage ToExcel(this ExcelParam param, object[][] table)
        {
            // If exists, auto width setting will throw exception.
            if (File.Exists(param.FilePath))
                File.Delete(param.FilePath);

            File.Copy(param.TemplatePath, param.FilePath);

            // Epplus dll write excel file that column index from 1 to end column index and row index from 0 to end row index.
            param.ColumnIndex += 1;

            var excel = param.GetExcelPackage(out var sheet);

            for (var i = 0; i < table.GetLength(0); i++)
                for (var j = 0; j < table[i].Length; j++)
                    sheet.Cells[i + param.RowIndex, j + param.ColumnIndex].Value = table[i][j];

            sheet.SetExcelStyle();
            excel.Save();

            return excel;
        }

        /// <summary>
        /// Writes the two dimension array to the excel.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ExcelPackage ToExcel(this ExcelParam param, object[,] table)
        {
            // If exists, auto width setting will throw exception.
            if (File.Exists(param.FilePath))
                File.Delete(param.FilePath);

            File.Copy(param.TemplatePath, param.FilePath);

            // Epplus dll write excel file that column index from 1 to end column index and row index from 0 to end row index.
            param.ColumnIndex += 1;

            var excel = param.GetExcelPackage(out var sheet);

            for (var i = 0; i < table.GetLength(0); i++)
                for (var j = 0; j < table.GetLength(1); j++)
                    sheet.Cells[i + param.RowIndex, j + param.ColumnIndex].Value = table[i, j];

            sheet.SetExcelStyle();
            excel.Save();

            return excel;
        }

        /// <summary>
        /// Writes the data table to the excel.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="param"></param>
        /// <param name="createHeader"></param>
        public static ExcelPackage ToExcel(this ExcelParam param, DataTable table, bool createHeader = true)
        {
            // If exists, auto width setting will throw exception.
            if (File.Exists(param.FilePath))
                File.Delete(param.FilePath);

            File.Copy(param.TemplatePath, param.FilePath);

            // Epplus dll write excel file that column index from 1 to end column index and row index from 0 to end row index.
            param.ColumnIndex += 1;

            var excel = param.GetExcelPackage(out var sheet);
            var columns = table.Columns.Cast<DataColumn>().ToList();

            // The titlt row.
            for (var i = 0; createHeader && i < columns.Count; i++)
                sheet.Cells[param.RowIndex, i + param.ColumnIndex].Value = columns[i].ColumnName;

            // The cotent row.
            for (var i = 0; i < table.Rows.Count; i++)
                for (var j = 0; j < columns.Count; j++)
                    sheet.Cells[i + param.RowIndex + 1, j + param.ColumnIndex].Value = table.Rows[i][columns[j].ColumnName];

            sheet.SetExcelStyle();
            excel.Save();

            return excel;
        }

        /// <summary>
        /// Sets custom style.
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="action"></param>
        /// <param name="param"></param>
        public static void SetExcelStyle(this ExcelPackage excel, Action<ExcelWorksheet> action, ExcelParam param)
        {
            action?.Invoke(excel.Workbook.Worksheets[param.SheetName]);
            excel.Save();
        }

        /// <summary>
        /// Gets the excel object.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static ExcelPackage GetExcelPackage(this ExcelParam param, out ExcelWorksheet sheet)
        {
            var fileInfo = new FileInfo(param.FilePath);
            var result = new ExcelPackage(fileInfo);
            var sheets = result.Workbook.Worksheets;

            sheet = sheets.FirstOrDefault(f => f.Name.ToLower() == param.SheetName.ToLower()) != null
                ? sheets[param.SheetName]
                : sheets.Add(param.SheetName);

            return result;
        }

        /// <summary>
        /// Gets the property's description attribute value.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetDcrp(this PropertyInfo p)
        {
            var objs = p.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // To throw not exception, must return empty string.
            return objs.Length == 0 ? string.Empty : (objs[0] as DescriptionAttribute)?.Description;
        }

        /// <summary>
        /// Gets the property's span attribute value.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int GetSpan(this PropertyInfo p)
        {
            var objs = p.GetCustomAttributes(typeof(SpanAttribute), false);

            if (objs.Length == 0)
                return 1;

            if (objs[0] is SpanAttribute attr)
                return objs.Length == 0 ? 1 : attr.ColumnSpan;

            return 1;
        }

        /// <summary>
        /// Gets the property's reference attribute value.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetReference(this PropertyInfo p)
        {
            var objs = p.GetCustomAttributes(typeof(ReferenceAttribute), false);

            if (objs.Length == 0)
                return string.Empty;

            if (objs[0] is ReferenceAttribute attr)
                return objs.Length == 0 ? string.Empty : attr.ColumnName;

            return string.Empty;
        }

        /// <summary>
        /// Gets merged range cell value.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetMegerValue(this ExcelWorksheet worksheet, int row, int column)
        {
            var rangeStr = worksheet.MergedCells[row, column];
            var excelRange = worksheet.Cells;
            var cellVal = excelRange[row, column].Value;

            if (rangeStr == null)
                return cellVal?.ToString();

            var startCell = new ExcelAddress(rangeStr).Start;

            return excelRange[startCell.Row, startCell.Column].Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Sets the excel style.
        /// </summary>
        /// <param name="worksheet"></param>
        public static void SetExcelStyle(this ExcelWorksheet worksheet)
        {
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells.AutoFitColumns();
        }
    }
}