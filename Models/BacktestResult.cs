using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Models
{
    public class BacktestResult
    {
        public decimal TotalProfit { get; set; }
        public decimal NetProfit { get; set; }
        public List<TradeResult> Trades { get; set; } = new List<TradeResult>();
    }

}
