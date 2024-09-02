using CryptoExchange.Net.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBots.Models
{
    public class KlineData
    {
        public int Id { get; set; }
        public decimal ClosePrice { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal OpenPrice { get; set; }
        public DateTime OpenTime { get; set; }
        public decimal QuoteVolume { get; set; }
        public decimal TakerBuyBaseVolume { get; set; }
        public decimal TakerBuyQuoteVolume { get; set; }
        public int TradeCount { get; set; }
        public decimal Volume { get; set; }
        public string? Pair { get; set; }
        public string? Exchange { get; set; }
        public DateTime CreateDate { get; set; }

        //Factory Methods
        public static KlineData FromBinanceInterface(Binance.Net.Interfaces.IBinanceKline kline, string pair, string exchange)
        {
            return new KlineData
            {
                ClosePrice = kline.ClosePrice,
                CloseTime = kline.CloseTime,
                HighPrice = kline.HighPrice,
                LowPrice = kline.LowPrice,
                OpenPrice = kline.OpenPrice,
                OpenTime = kline.OpenTime,
                QuoteVolume = kline.QuoteVolume,
                TakerBuyBaseVolume = kline.TakerBuyBaseVolume,
                TakerBuyQuoteVolume = kline.TakerBuyQuoteVolume,
                TradeCount = kline.TradeCount,
                Volume = kline.Volume,
                Pair = pair,
                Exchange = exchange,
                CreateDate = DateTime.UtcNow
            };
        }
    }
}
