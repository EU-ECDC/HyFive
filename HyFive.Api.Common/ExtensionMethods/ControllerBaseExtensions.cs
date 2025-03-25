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
        public static async Task<IActionResult> ExcelFileContentResult(this ControllerBase controller, IEnumerable<object> objekter, string filnavn)
        {
            var filnavnExcel = $"{DateTime.Now:yyyyMMddHHmmss_}{filnavn}.xlsx";
            var excel = await LagExcelFilInnhold(objekter);
            return controller.File(excel, "application/xlsx", filnavnExcel);
        }
        
        private static async Task<byte[]> LagExcelFilInnhold( IEnumerable<object> objekter)
        {
            var norskKultur = CultureInfo.InvariantCulture;
            using var ms = new MemoryStream();
            
            using (var excelWriter = new ExcelWriter(ms, norskKultur, true))
            {
                await excelWriter.WriteRecordsAsync(objekter);
            }
            ms.Position = 0;
            return ms.ToArray();
        }
    }
}
