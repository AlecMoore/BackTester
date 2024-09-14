using Binance.Net.Interfaces.Clients;
using BackTester.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using BackTester.Exchanges;
using CryptoClients.Net.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackTester.Repositories
{
    internal class BinanceExchangeRepository : IExchangeRepository
    {
        private readonly IBinanceRestClient _restClient;
        private readonly IBinanceSocketClient _socketClient;


        public BinanceExchangeRepository(IBinanceRestClient restClient, IBinanceSocketClient socketClient)
        {
            _restClient = restClient;
            _socketClient = socketClient;
        }

        public Exchange GetExchange()
        {
            return Exchange.Binance;
        }

        public async Task<WebCallResult> CancelOrder(string id, string symbol)
        {
            var result = await _restClient.SpotApi.Trading.CancelOrderAsync(symbol ?? "", long.Parse(id));

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
            var result = await _restClient.SpotApi.Account.GetAccountInfoAsync();

            if (result.Success && result.Data != null)
            {
                return result.Data.Balances.ToDictionary(b => b.Asset, b => b.Total);
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
                return result.Data.Select(o => new OpenOrder(o.Id.ToString(), o.Price, o.Quantity, o.QuantityFilled, o.Type.ToString(),
                                o.Status.ToString(), o.Side.ToString(), o.CreateTime, Exchange.Binance, o.Symbol, DateTime.UtcNow)
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
            var result = await _restClient.SpotApi.ExchangeData.GetPriceAsync(symbol);

            if (result.Success && result.Data != null)
            {
                return result.Data.Price;
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return 0;
            }
        }

        public async Task<OpenOrder> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            var result = await _restClient.SpotApi.Trading.PlaceOrderAsync(
                symbol,
                side.ToLower() == "buy" ? Binance.Net.Enums.OrderSide.Buy : Binance.Net.Enums.OrderSide.Sell,
                type == "market" ? Binance.Net.Enums.SpotOrderType.Market : Binance.Net.Enums.SpotOrderType.Limit,
                quantity,
                price: price,
                timeInForce: type == "market" ? null : Binance.Net.Enums.TimeInForce.GoodTillCanceled);

            if (result.Success && result.Data != null)
            {
                return new OpenOrder(result.Data.Id.ToString(), result.Data.Price, result.Data.Quantity, result.Data.QuantityFilled, result.Data.Type.ToString(),
                                result.Data.Status.ToString(), result.Data.Side.ToString(), result.Data.CreateTime, Exchange.Binance, result.Data.Symbol, DateTime.UtcNow);
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var result = await _socketClient.SpotApi.ExchangeData.SubscribeToMiniTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));

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
            var result = await _restClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, Binance.Net.Enums.KlineInterval.OneMinute, startTime, endTime, limit: 1000);
            if (result.Success && result.Data != null)
            {
                return result.Data.Select(r => new KlineData(0, r.ClosePrice, r.CloseTime, r.HighPrice, r.LowPrice,
                                r.OpenPrice, r.OpenTime, r.QuoteVolume, r.TakerBuyBaseVolume, r.TakerBuyQuoteVolume, r.TradeCount,
                                r.Volume, Exchange.Binance, symbol, DateTime.UtcNow)
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
            var result = await _restClient.SpotApi.Account.GetTradeFeeAsync(symbol);

            if(result.Success && result.Data != null)
            {
                return result.Data.Select(f => new UserFees(f.MakerFee, f.TakerFee, Exchange.Binance, symbol, DateTime.UtcNow)).FirstOrDefault();
            }
            else
            {
                Console.Error.WriteLine(result.Error);
                return null;
            }
        }
    }
}