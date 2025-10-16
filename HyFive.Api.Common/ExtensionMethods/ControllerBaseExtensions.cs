using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper.Excel;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class ControllerBaseExtensions
    {
        public static async Task<IActionResult> ExcelFileContentResult(this ControllerBase controller, IEnumerable<object> objects, string fileName)
        {
            var fileNameExcel = $"{DateTime.UtcNow:dd.MM.yyyy-HH.mm.ss-}{fileName}.xlsx";
            var excel = await CreateExcelFileContent(objects);
            return controller.File(excel, "application/xlsx", fileNameExcel);
        }
        
        private static async Task<byte[]> CreateExcelFileContent( IEnumerable<object> objects)
        {
            var culture = CultureInfo.InvariantCulture;
            using var ms = new MemoryStream();
            
            using (var excelWriter = new ExcelWriter(ms, culture, true))
            {
                await excelWriter.WriteRecordsAsync(objects);
            }
            ms.Position = 0;
            return ms.ToArray();
        }
    }
}
