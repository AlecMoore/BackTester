using Bitfinex.Net.Interfaces.Clients;
using BackTester.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using BackTester.Exchanges;
using CryptoClients.Net.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackTester.Repositories
{
    internal class BitfinexExchangeRepository : IExchangeRepository
    {
        private readonly IBitfinexRestClient _restClient;
        private readonly IBitfinexSocketClient _socketClient;


        public BitfinexExchangeRepository(IBitfinexRestClient restClient, IBitfinexSocketClient socketClient)
        {
            _restClient = restClient;
            _socketClient = socketClient;
        }

        public Exchange GetExchange()
        {
            return Exchange.Bitfinex;
        }

        public async Task<WebCallResult> CancelOrder(string id, string? symbol = null)
        {
            var result = await _restClient.SpotApi.Trading.CancelOrderAsync(long.Parse(id));

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
            var result = await _restClient.SpotApi.Account.GetBalancesAsync();

            if (result.Success && result.Data != null)
            {
                return result.Data.ToDictionary(b => b.Asset, b => b.Total);
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            var result = await _restClient.SpotApi.Trading.GetOpenOrdersAsync();

            if (result.Success && result.Data != null)
            {
                return result.Data.Select(o => new OpenOrder(o.Price, o.Quantity, o.Quantity - o.QuantityRemaining, o.Type.ToString(),
                                o.Status.ToString(), o.Side.ToString(), o.CreateTime, Exchange.Bitfinex, o.Symbol, DateTime.UtcNow)
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
            var result = await _restClient.SpotApi.ExchangeData.GetTickerAsync(symbol);

            if (result.Success && result.Data != null)
            {
                return result.Data.LastPrice;
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return 0;
            }
        }

        public async Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            var result = await _restClient.SpotApi.Trading.PlaceOrderAsync(
                symbol,
                side.ToLower() == "buy" ? Bitfinex.Net.Enums.OrderSide.Buy : Bitfinex.Net.Enums.OrderSide.Sell,
                type == "market" ? Bitfinex.Net.Enums.OrderType.Market : Bitfinex.Net.Enums.OrderType.Limit,
                quantity,
                price: price ?? 0);

            if (result.Success && result.Data != null)
            {
                return result.As(result.Data?.Id.ToString());
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var result = await _socketClient.SpotApi.SubscribeToTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));

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
            var result = await _restClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, Bitfinex.Net.Enums.KlineInterval.OneMinute, limit: 10000, startTime: startTime, endTime: endTime);
            if (result.Success && result.Data != null)
            {
                return result.Data.Select(r => new KlineData(0, r.ClosePrice, r.OpenTime.AddMinutes(1), r.HighPrice, r.LowPrice,
                                r.OpenPrice, r.OpenTime, 0, 0, 0, 0,
                                r.Volume, Exchange.Bitfinex, symbol, DateTime.UtcNow)
                            );
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<UserFees> GetUserFees(string symbol)
        {
            var result = await _restClient.SpotApi.Account.Get30DaySummaryAndFeesAsync();

            if(result.Success && result.Data != null)
            {
                return new UserFees(result.Data.Fees.MakerFees.MakerFee, result.Data.Fees.TakerFees.TakerFeeFiat, Exchange.Bitfinex, symbol, DateTime.UtcNow);
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }
    }
}