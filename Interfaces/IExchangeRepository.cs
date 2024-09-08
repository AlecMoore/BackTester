using BackTester.Models;
using CryptoClients.Net.Enums;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace BackTester.Exchanges
{
    public interface IExchangeRepository
    {
        Exchange GetExchange();
        Task<decimal> GetPrice(string symbol);
        Task<IEnumerable<OpenOrder>> GetOpenOrders();
        Task<Dictionary<string, decimal>> GetBalances();
        Task<WebCallResult> CancelOrder(string id, string? symbol = null);
        Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price);
        Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler);
        Task<IEnumerable<KlineData>> GetKlineData(string symbol, DateTime? startTime, DateTime? endTime);
        Task<UserFees> GetUserFees(string symbol);

    }
}