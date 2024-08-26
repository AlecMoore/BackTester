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
    }
}
