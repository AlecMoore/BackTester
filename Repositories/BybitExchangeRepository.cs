using BackTester.Models;
using Bybit.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoClients.Net.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackTester.Exchanges
{
    internal class BybitExchangeRepository : IExchangeRepository
    {
        private readonly IBybitRestClient _restClient;
        private readonly IBybitSocketClient _socketClient;

        public BybitExchangeRepository(IBybitRestClient restClient, IBybitSocketClient socketClient)
        {
            _restClient = restClient;
            _socketClient = socketClient;
        }

        public Exchange GetExchange()
        {
            return Exchange.Bybit;
        }

        public async Task<WebCallResult> CancelOrder(string id, string symbol)
        {
            var result = await _restClient.V5Api.Trading.CancelOrderAsync(Bybit.Net.Enums.Category.Spot, symbol ?? "", id);

            if (result.Success && result.Data != null)
            {
                return result.AsDataless();
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            var result = await _restClient.V5Api.Account.GetBalancesAsync(Bybit.Net.Enums.AccountType.Spot);

            if (result.Success && result.Data != null)
            {
                return result.Data.List.First().Assets.ToDictionary(d => d.Asset, d => d.WalletBalance);
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            var result = await _restClient.V5Api.Trading.GetOrdersAsync(Bybit.Net.Enums.Category.Spot);

            if (result.Success && result.Data != null)
            {
                return result.Data.List.Select(o => new OpenOrder(o.OrderId, o.Price ?? 0, o.Quantity, o.QuantityFilled ?? 0, o.OrderType.ToString(),
                                o.Status.ToString(), o.Side.ToString(), o.CreateTime, Exchange.Bybit, o.Symbol, DateTime.UtcNow)
                            );
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<decimal> GetPrice(string symbol)
        {
            var result = await _restClient.V5Api.ExchangeData.GetSpotTickersAsync(symbol);

            if (result.Success && result.Data != null)
            {
                return result.Data.List.First().LastPrice;
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return 0;
            }
        }

        public async Task<OpenOrder> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            var result = await _restClient.V5Api.Trading.PlaceOrderAsync(
                Bybit.Net.Enums.Category.Spot,
                symbol,
                side.ToLower() == "buy" ? Bybit.Net.Enums.OrderSide.Buy : Bybit.Net.Enums.OrderSide.Sell,
                type == "market" ? Bybit.Net.Enums.NewOrderType.Market : Bybit.Net.Enums.NewOrderType.Limit,
                quantity,
                price: price);

            if (result.Success && result.Data != null)
            {
                var order = (await GetOpenOrders()).First(o => o.Id == result.Data.OrderId);

                return (await GetOpenOrders()).First(o => o.Id == result.Data.OrderId);
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var result = await _socketClient.V5SpotApi.SubscribeToTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
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