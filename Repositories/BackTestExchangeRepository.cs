using Binance.Net.Interfaces.Clients;
using BackTester.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using BackTester.Exchanges;
using CryptoClients.Net.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackTester.Repositories
{
    internal class BackTestExchangeRepository : IExchangeRepository
    {
        public Exchange GetExchange()
        {
            throw new NotImplementedException();
        }

        public async Task<WebCallResult> CancelOrder(string id, string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> GetPrice(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<OpenOrder> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<KlineData>> GetKlineData(string symbol, DateTime? startTime, DateTime? endTime)
        {
            throw new NotImplementedException();
        }

        public async Task<UserFees> GetUserFees(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}