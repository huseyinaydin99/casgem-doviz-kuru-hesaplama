using Casgem_Case1.Doviz.Kuru.Consume.Models;
using Casgem_Case1.Doviz.Kuru.Core.Constants;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casgem_Case1.Doviz.Kuru.Core.Utilities
{
    public static class DataFetchUtilitiy
    {
        //APILayer'den Consume'a istek atıp veriyi çekebildiğimiz istemci nesne.
        private static readonly HttpClient _httpClient = new HttpClient();

        //Veriyi APILayer'dan çekebildiğimiz static bir method.
        public static async Task<List<MoneyRateModel>> GetMoneysRatesAsync()
        {
            //Çekilen verileri listeye ekleyeceğiz.
            var exchangeRates = new List<MoneyRateModel>();
            //istemci üzerinden ilgili url'ye GET isteği attık.
            var response = await _httpClient.GetAsync(Constant.API_URL);
            //statu kodu 200 ise if bloğuna gir dedik.
            if (response.IsSuccessStatusCode)
            {
                //gelen cevaptan veriyi string olarak okuduk.
                var content = await response.Content.ReadAsStringAsync();
                //okunan veriyi XML şeklinde formatladık.
                var xmlDoc = XDocument.Parse(content);
                //büyükten küçüğe sıralanan veriyi foreach döngüsü ile gezdik.
                foreach (var item in xmlDoc.Descendants("items"))
                {
                    //0 gelen veriyi eledik. 0 değilse if'e gir dedik.
                    if (item.Element("usd").Value != "0")
                    {
                        //replace metodu ile .'lı olarak gelen veriyi ','e çevirdik ve ilgili propertylere atadık.
                        var exchangeRate = new MoneyRateModel
                        {
                            date = DateTime.Parse(item.Element("date").Value),
                            usd = decimal.Parse(item.Element("usd").Value.Replace(".", ",")),
                            eur = decimal.Parse(item.Element("eur").Value.Replace(".", ",")),
                            chf = decimal.Parse(item.Element("che").Value.Replace(".", ",")),
                            gbp = decimal.Parse(item.Element("gbp").Value.Replace(".", ",")),
                            jpy = decimal.Parse(item.Element("jpy").Value.Replace(".", ","))
                        };
                        //her bir oluşan nesneyi listeye ekledik.
                        exchangeRates.Add(exchangeRate);
                    }
                }
            }
            return exchangeRates;//para kuru model nesnesi listesini döndük.
        }

        public static async Task<Dictionary<string, List<(DateTime date, decimal? rate)>>> DovizKurEnYuksek()
        {
            var items = await GetMoneysRatesAsync();
            // Döviz kurlarının  listesi
            var currencyRates = new Dictionary<string, List<(DateTime date, decimal? rate)>>();
            currencyRates.Add("USD", items.OrderByDescending(x => x.usd).Take(5)
                                          .Select(x => (x.date, x.usd)).ToList());
            currencyRates.Add("EUR", items.OrderByDescending(x => x.eur).Take(5)
                                          .Select(x => (x.date, x.eur)).ToList());
            currencyRates.Add("CHF", items.OrderByDescending(x => x.chf).Take(5)
                                          .Select(x => (x.date, x.chf)).ToList());
            currencyRates.Add("GBP", items.OrderByDescending(x => x.gbp).Take(5)
                                          .Select(x => (x.date, x.gbp)).ToList());
            currencyRates.Add("JPY", items.OrderByDescending(x => x.jpy).Take(5)
                                          .Select(x => (x.date, x.jpy)).ToList());
            return currencyRates;
        }
    }
}