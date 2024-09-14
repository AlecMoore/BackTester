using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Models
{
    public class TradeResult
    {
        public DateTime Timestamp { get; set; }
        public TradeAction Action { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
    }

}
