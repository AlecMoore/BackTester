using BackTester.Extensions;
using BackTester.Interfaces;
using BackTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BackTester.Strategies
{
    public class MovingAverageStrategy : ITradingStrategy
    {
        private readonly int _shortPeriod;
        private readonly int _longPeriod;

        public MovingAverageStrategy(int shortPeriod, int longPeriod)
        {
            _shortPeriod = shortPeriod;
            _longPeriod = longPeriod;
        }

        public TradeSignal GenerateSignal(IEnumerable<KlineData> klineData, DateTime closeTime)
        {
            var shortMA = TradingStrategyExtensionMethods.GetMovingAverage(klineData, _shortPeriod);
            var longMA = TradingStrategyExtensionMethods.GetMovingAverage(klineData, _longPeriod);

            if (shortMA > longMA)
                return new TradeSignal(TradeAction.Buy);
            else if (shortMA < longMA)
                return new TradeSignal(TradeAction.Sell);

            return new TradeSignal(TradeAction.Hold);
        }
    }

}
