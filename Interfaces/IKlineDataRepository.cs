using BackTester.Models;
using CryptoClients.Net.Enums;

namespace BackTester.Interfaces
{
    public interface IKlineDataRepository
    {
        void AddKlineData(KlineData klineData);
        IEnumerable<KlineData> GetKlineData(string Symbol, Exchange Exchange);
    }
}
