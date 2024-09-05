using CryptoClients.Net.Enums;

namespace BackTester.Models
{
    public class KlineData : DataBaseObject
    {
        public KlineData(int id, decimal closePrice, DateTime closeTime, decimal highPrice, decimal lowPrice, 
            decimal openPrice, DateTime openTime, decimal quoteVolume, decimal takerBuyBaseVolume, 
            decimal takerBuyQuoteVolume, int tradeCount, decimal volume, Exchange exchange, string symbol, DateTime createDate)
        {
            Id = id;
            ClosePrice = closePrice;
            CloseTime = closeTime;
            HighPrice = highPrice;
            LowPrice = lowPrice;
            OpenPrice = openPrice;
            OpenTime = openTime;
            QuoteVolume = quoteVolume;
            TakerBuyBaseVolume = takerBuyBaseVolume;
            TakerBuyQuoteVolume = takerBuyQuoteVolume;
            TradeCount = tradeCount;
            Volume = volume;
            Exchange = exchange;
            Symbol = symbol;
            CreateDate = createDate;
        }

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
    }
}
