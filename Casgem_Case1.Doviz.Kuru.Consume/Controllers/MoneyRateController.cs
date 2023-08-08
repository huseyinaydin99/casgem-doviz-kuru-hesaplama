using Casgem_Case1.Doviz.Kuru.Consume.Models;
using Casgem_Case1.Doviz.Kuru.Core.Utilities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Casgem_Case1.Doviz.Kuru.Consume.Controllers
{
    public class MoneyRateController : Controller
    {
        public async Task<List<MoneyRateModel>> GetExchangeRatesAsync()
        {
            return await DataFetchUtilitiy.GetMoneysRatesAsync();
        }

        public async Task<IActionResult> WriteMoneyRateExcelFile()
        {
            // Döviz kur verileri
            var items = await DataFetchUtilitiy.GetMoneysRatesAsync();
            var usdRates = items.OrderByDescending(x => x.usd).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.usd }).ToList();
            var eurRates = items.OrderByDescending(x => x.eur).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.eur }).ToList();
            var chfRates = items.OrderByDescending(x => x.chf).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.chf }).ToList();
            var gbpRates = items.OrderByDescending(x => x.gbp).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.gbp }).ToList();
            var jpyRates = items.OrderByDescending(x => x.jpy).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.jpy }).ToList();
            // Tüm döviz kurları
            var allRates = new List<Dictionary<string, object>>();
            allRates.AddRange(usdRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "USD" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(eurRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "EUR" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(chfRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "CHF" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(gbpRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "GBP" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(jpyRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "JPY" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));

            // Excel dosyası oluşturma
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Doviz Kuru Verileri");
                // Başlık satırı
                worksheet.Cell("A1").Value = "Döviz Cinsi";
                worksheet.Cell("B1").Value = "Kur Değeri";
                // Veri satırları
                int row = 2;
                foreach (var rate in allRates)
                {
                    worksheet.Cell($"A{row}").Value = rate["Döviz Cinsi"].ToString();
                    worksheet.Cell($"B{row}").Value = rate["Kur Değeri"].ToString();
                    row++;
                }
                // Excel dosyasını stream olarak kaydettim ve kullanıcıya indirme işlemi için son aşama
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Doviz Kuru Verileri.xlsx");
                }
            }
        }
    }
}