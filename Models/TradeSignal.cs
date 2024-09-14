using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Models
{
    public class TradeSignal
    {
        public TradeAction Action { get; }

        public TradeSignal(TradeAction action)
        {
            Action = action;
        }
    }

    public enum TradeAction
    {
        Buy,
        Sell,
        Hold
    }
}
