using BackTester.Models;
using Bybit.Net.Clients;
using Bybit.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace BackTester.Exchanges
{
    internal class BybitExchange : IExchangeRepository
    {
        private IBybitSocketClient _socketClient = new BybitSocketClient();

        public async Task<WebCallResult> CancelOrder(string symbol, string id)
        {
            using var client = new BybitRestClient();
            var result = await client.V5Api.Trading.CancelOrderAsync(Bybit.Net.Enums.Category.Spot, symbol, id);
            return result.AsDataless();
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            using var client = new BybitRestClient();
            var result = await client.V5Api.Account.GetBalancesAsync(Bybit.Net.Enums.AccountType.Spot);
            return result.Data.List.First().Assets.ToDictionary(d => d.Asset, d => d.WalletBalance);
        }

        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            using var client = new BybitRestClient();
            var order = await client.V5Api.Trading.GetOrdersAsync(Bybit.Net.Enums.Category.Spot);
            return order.Data.List.Select(o => new OpenOrder(o.Price ?? 0, o.Quantity, o.QuantityFilled ?? 0, o.OrderType.ToString(),
                o.Status.ToString(), o.Side.ToString(), o.CreateTime, ExchangeEnum.Bybit, o.Symbol, DateTime.UtcNow)
            );
        }

        public async Task<decimal> GetPrice(string symbol)
        {
            using var client = new BybitRestClient();
            var result = await client.V5Api.ExchangeData.GetSpotTickersAsync(symbol);
            return result.Data.List.First().LastPrice;
        }

        public async Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            using var client = new BybitRestClient();
            var result = await client.V5Api.Trading.PlaceOrderAsync(
                Bybit.Net.Enums.Category.Spot,
                symbol,
                side.ToLower() == "buy" ? Bybit.Net.Enums.OrderSide.Buy : Bybit.Net.Enums.OrderSide.Sell,
                type == "market" ? Bybit.Net.Enums.NewOrderType.Market : Bybit.Net.Enums.NewOrderType.Limit,
                quantity,
                price: price);
            return result.As(result.Data?.OrderId.ToString());
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var sub = await _socketClient.V5SpotApi.SubscribeToTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));
            return sub.Data;
        }
    }
}