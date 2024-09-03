using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using BackTester.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using BackTester.Exchanges;
using System.Security.Principal;

namespace BackTester.Repositories
{
    internal class BinanceExchangeRepository : IExchangeRepository
    {
        private IBinanceSocketClient _socketClient = new BinanceSocketClient();

        public async Task<WebCallResult> CancelOrder(string symbol, string id)
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.Trading.CancelOrderAsync(symbol, long.Parse(id));
            return result.AsDataless();
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.Account.GetAccountInfoAsync();
            return result.Data.Balances.ToDictionary(b => b.Asset, b => b.Total);
        }

        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.Trading.GetOpenOrdersAsync();
            // Should check result success status here
            return result.Data.Select(o => new OpenOrder(o.Price, o.Quantity, o.QuantityFilled, o.Type.ToString(),
                o.Status.ToString(), o.Side.ToString(), o.CreateTime, ExchangeEnum.Binance, o.Symbol, DateTime.UtcNow)
            );
        }

        public async Task<decimal> GetPrice(string symbol)
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.ExchangeData.GetPriceAsync(symbol);
            // Should check result success status here
            return result.Data.Price;
        }

        public async Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.Trading.PlaceOrderAsync(
                symbol,
                side.ToLower() == "buy" ? Binance.Net.Enums.OrderSide.Buy : Binance.Net.Enums.OrderSide.Sell,
                type == "market" ? Binance.Net.Enums.SpotOrderType.Market : Binance.Net.Enums.SpotOrderType.Limit,
                quantity,
                price: price,
                timeInForce: type == "market" ? null : Binance.Net.Enums.TimeInForce.GoodTillCanceled);
            return result.As(result.Data?.Id.ToString());
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var sub = await _socketClient.SpotApi.ExchangeData.SubscribeToMiniTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));
            return sub.Data;
        }

        public async Task<IEnumerable<KlineData>> GetKlineData(string symbol, DateTime? startTime, DateTime? endTime)
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.ExchangeData.GetKlinesAsync(symbol, Binance.Net.Enums.KlineInterval.OneMinute, startTime, endTime, limit: 1000);
            // Should check result success status here
            return result.Data.Select(r => new KlineData(0, r.ClosePrice, r.CloseTime, r.HighPrice, r.LowPrice,
                r.OpenPrice, r.OpenTime, r.QuoteVolume, r.TakerBuyBaseVolume, r.TakerBuyQuoteVolume, r.TradeCount,
                r.Volume, ExchangeEnum.Binance, symbol, DateTime.UtcNow)
            );
        }

        public async Task<UserFees> GetUserFees(string symbol)
        {
            using var client = new BinanceRestClient();
            var result = await client.SpotApi.Account.GetTradeFeeAsync(symbol);
            // Should check result success status here


            return result.Data.Select(f => new UserFees(f.MakerFee, f.TakerFee, ExchangeEnum.Binance, symbol, DateTime.UtcNow)).FirstOrDefault();
        }
    }
}