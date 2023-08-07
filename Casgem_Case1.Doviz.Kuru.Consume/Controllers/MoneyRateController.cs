using Casgem_Case1.Doviz.Kuru.Consume.Models;
using Casgem_Case1.Doviz.Kuru.Core.Utilities;
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
    }
}
