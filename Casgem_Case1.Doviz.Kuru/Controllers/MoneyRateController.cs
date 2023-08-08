using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Xml.Linq;

namespace Casgem_Case1.Doviz.Kuru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyRateController : ControllerBase
    {
        //Veriyi TCMB'nin Web Servisinden çekebilmemiz için kullanacağımız client yani istemci(sunucuya istek atıp cevaplar alan nesnedir).
        private readonly HttpClient _httpClient;

        public MoneyRateController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRates()
        {
            //Bitiş bugünün tarihini güngün-ayay/yılyılyıl şeklinde verdik.
            var endDate = DateTime.Now.Date.ToString("dd-MM-yyyy");
            //Başlangıç bugünden 30 gün evvele güngün-ayay/yılyılyıl şeklinde verdik.
            var startDate = DateTime.Now.Date.AddDays(-30).ToString("dd-MM-yyyy");
            //T.C. Merkez Bankasından döviz kuru URL'i oluşturduk.
            string url = $"https://evds2.tcmb.gov.tr/service/evds/series=TP.DK.USD.S.YTL-TP.DK.EUR.S.YTL-TP.DK.CHF.S.YTL-TP.DK.GBP.S.YTL-TP.DK.JPY.S.YTL&startDate={startDate}&endDate={endDate}&type=xml&key=Keyinizi giriniz";
            //İlgili URL'e GET isteği attık ve cevap aldık.
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            //Cevabın statu kodu Success olduysa if bloğuna gir dedik.
            if (response.IsSuccessStatusCode)
            {
                //Gelen cevaptaki string veriyi string formatında okuduk.
                string xmlString = await response.Content.ReadAsStringAsync();
                //string veriyi xml formarına parselledik.
                XDocument doc = XDocument.Parse(xmlString);
                //Gelen itemleri / kayıtları büyükten küçüğe doğru(descending) sıraladık ve nesneye eşledik.
                var exchangeRates = doc.Descendants("items").Select(item => new
                {
                    date = item.Element("Tarih").Value,
                    usd = string.IsNullOrEmpty(item.Element("TP_DK_USD_S_YTL").Value) ? "0" : item.Element("TP_DK_USD_S_YTL").Value,
                    eur = string.IsNullOrEmpty(item.Element("TP_DK_EUR_S_YTL").Value) ? "0" : item.Element("TP_DK_EUR_S_YTL").Value,
                    chf = string.IsNullOrEmpty(item.Element("TP_DK_CHF_S_YTL").Value) ? "0" : item.Element("TP_DK_CHF_S_YTL").Value,
                    gbp = string.IsNullOrEmpty(item.Element("TP_DK_CHF_S_YTL").Value) ? "0" : item.Element("TP_DK_GBP_S_YTL").Value,
                    jpy = string.IsNullOrEmpty(item.Element("TP_DK_CHF_S_YTL").Value) ? "0" : item.Element("TP_DK_JPY_S_YTL").Value,
                });
                //exchangeRates nesnesinden LINQ sorgusu ile ilgili kolonları / propertyleri eşledik.
                XElement root = new XElement("ExchangeRates",
                    from ex in exchangeRates
                    select new XElement("items",
                        new XElement("date", ex.date),
                        new XElement("usd", ex.usd),
                        new XElement("eur", ex.eur),
                        new XElement("che", ex.chf),
                        new XElement("gbp", ex.gbp),
                        new XElement("jpy", ex.jpy)
                    )
                );
                //Geriye XML olarak (application/xml) formatında veriyi response(cevap) olarak döndük.
                return Content(root.ToString(), "application/xml");
            }
            else //gelen statu kodu 200 ok değilse 
            {
                return NotFound(); //bulunamadı 404 döndük.
            }
        }
    }
}
