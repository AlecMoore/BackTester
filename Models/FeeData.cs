using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBots.Models
{
    public class FeeData
    {
        public decimal MakerCommission { get; set; }
        public decimal TakerCommission { get; set; }
        public string? Pair { get; set; }
        public string? Exchange { get; set; }

        //Factory Methods
        public static FeeData FromBinanceInterface(Binance.Net.Objects.Models.Spot.BinanceTradeFee fee, string pair, string exchange)
        {
            return new FeeData
            {
                MakerCommission = fee.MakerFee,
                TakerCommission = fee.TakerFee,
                Pair = pair,
                Exchange = exchange,
            };
        }

    }
}
