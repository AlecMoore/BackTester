using CryptoExchange.Net.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingBots.Models;

namespace TradingBots.Interfaces
{
    public interface IKlineDataRepository
    {
        void AddKlineData(KlineData klineData);
        IEnumerable<KlineData> GetKlineData(string Pair, string Exchange);
    }
}
