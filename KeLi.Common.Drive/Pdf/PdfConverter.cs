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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using O2S.Components.PDFRender4NET;

namespace KeLi.Common.Drive.Pdf
{
    /// <summary>
    /// A pdf converter.
    /// </summary>
    public static class PdfConverter
    {
        /// <summary>
        /// Convers pdf to images.
        /// </summary>
        /// <param name="param"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<string> ToImages(this PdfParam param)
        {
            var results = new List<string>();
            var pdfFile = PDFFile.Open(param.PdfPath.FullName);

            if (param.StartPage <= 0)
                param.StartPage = 1;

            if (param.EndPage > pdfFile.PageCount)
                param.EndPage = pdfFile.PageCount;

            if (param.StartPage > param.EndPage)
            {
                param.StartPage = param.EndPage;
                param.EndPage = param.StartPage;
            }

            for (var i = param.StartPage; i <= param.EndPage; i++)
            {
                var withNum = i.ToString().PadLeft(pdfFile.PageCount.ToString().Length, '0');

                withNum = param.StartPage == param.EndPage ? null : "_" + withNum;

                var imgPage = pdfFile.GetPageImage(i - 1, 56 * param.Resolution);
                var filePath = Path.Combine(param.PdfPath.DirectoryName, param.ImgName + withNum + "." + param.Format.ToString().ToLower());

                results.Add(filePath);
                imgPage.Save(filePath, param.Format);
                imgPage.Dispose();
            }

            pdfFile.Dispose();

            return results;
        }

        /// <summary>
        /// Gets file page's size.
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public static Size GetPdfSize(this string pdfPath)
        {
            var reader = new PdfReader(pdfPath);
            var width = reader.GetPageSizeWithRotation(1).Width;
            var height = reader.GetPageSizeWithRotation(1).Height;

            reader.Close();

            return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }

        /// <summary>
        /// Splits the pdf file to the multi pdf files.
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="targetPdfs"></param>
        public static void SplitedPdfs(string sourcePdf, out List<string> targetPdfs)
        {
            targetPdfs = new List<string>();

            var reader = new PdfReader(sourcePdf);
            var titled = GetPageMark(reader);

            if (reader.NumberOfPages == 1)
                targetPdfs = new List<string> { sourcePdf };
            else
            {
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    var targetPath = Path.GetDirectoryName(sourcePdf);
                    var targetName = Path.GetFileNameWithoutExtension(sourcePdf);

                    if (titled.ContainsKey(i))
                        targetName += "_" + titled[i] + Path.GetExtension(sourcePdf);
                    else
                        targetName += "_" + i + Path.GetExtension(sourcePdf);

                    var targetPdf = Path.Combine(targetPath, targetName);

                    CopyPdf(sourcePdf, targetPdf, i, i);
                    targetPdfs.Add(targetPdf);
                }
            }

            reader.Close();
        }

        /// <summary>
        /// Copys the pdf range content to the new pdf file.
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="targetPdf"></param>
        /// <param name="startPage"></param>
        /// <param name="endPage"></param>
        public static void CopyPdf(string sourcePdf, string targetPdf, int startPage, int endPage)
        {
            var reader = new PdfReader(sourcePdf);
            var doc = new Document(reader.GetPageSizeWithRotation(startPage));
            var writer = PdfWriter.GetInstance(doc, new FileStream(targetPdf, FileMode.Create));

            doc.Open();

            var content = writer.DirectContent;

            for (var i = startPage - 1; i < endPage; i++)
            {
                doc.SetPageSize(reader.GetPageSizeWithRotation(startPage));
                doc.NewPage();

                var page = writer.GetImportedPage(reader, startPage);
                var rotation = reader.GetPageRotation(startPage);

                if (rotation == 90 || rotation == 270)
                {
                    switch (rotation)
                    {
                        case 90:
                            content.AddTemplate(page, 0, -1f, 1f, 0, 0,
                                reader.GetPageSizeWithRotation(startPage).Height);
                            break;

                        case 270:
                            content.AddTemplate(page, 0, 1.0F, -1.0F, 0,
                                reader.GetPageSizeWithRotation(startPage).Width, 0);
                            break;
                    }
                }
                else
                    content.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);

                startPage++;
            }

            doc.Close();
            reader.Close();
        }

        /// <summary>
        /// Extracts the pdf range content to new pdf file.
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="targetPdf"></param>
        /// <param name="startPage"></param>
        /// <param name="endPage"></param>
        public static void ExtractPdf(string sourcePdf, string targetPdf, int startPage, int endPage)
        {
            var reader = new PdfReader(sourcePdf);
            var doc = new Document(reader.GetPageSizeWithRotation(startPage));
            var copy = new PdfCopy(doc, new FileStream(targetPdf, FileMode.Create));

            doc.Open();

            for (var i = startPage; i <= endPage; i++)
            {
                var importPage = copy.GetImportedPage(reader, i);

                copy.AddPage(importPage);
            }

            doc.Close();
            reader.Close();
        }

        /// <summary>
        /// Extracts the pdf pages content to new pdf file.
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="targetPdf"></param>
        /// <param name="extractPages"></param>
        public static void ExtractPdf(string sourcePdf, string targetPdf, List<int> extractPages)
        {
            var reader = new PdfReader(sourcePdf);
            var doc = new Document(reader.GetPageSizeWithRotation(extractPages[0]));
            var copy = new PdfCopy(doc, new FileStream(targetPdf, FileMode.Create));

            doc.Open();

            foreach (var pageNumber in extractPages)
            {
                var importedPage = copy.GetImportedPage(reader, pageNumber);

                copy.AddPage(importedPage);
            }

            doc.Close();
            reader.Close();
        }

        /// <summary>
        /// Gets the pdf page bookmark. 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Dictionary<int, string> GetPageMark(PdfReader reader)
        {
            var marks = SimpleBookmark.GetBookmark(reader);
            var results = new Dictionary<int, string>();

            foreach (Hashtable mark in marks)
            {
                var title = string.Empty;
                var page = 0;

                foreach (DictionaryEntry kv in mark)
                {
                    switch (kv.Key.ToString())
                    {
                        case "Action":
                            continue;
                        case "Title":
                            title = kv.Value.ToString();
                            break;

                        case "Page":
                            page = Convert.ToInt32(kv.Value.ToString().Split(' ')[0]);
                            break;
                    }
                }

                results.Add(page, title);
            }

            return results;
        }
    }
}