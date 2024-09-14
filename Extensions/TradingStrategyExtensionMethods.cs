using BackTester.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Extensions
{
    internal static class TradingStrategyExtensionMethods
    {
        /// <summary>
        /// Gets the moving average of kline data set for certain interval
        /// </summary>
        /// <param name="klineData"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static decimal GetMovingAverage(IEnumerable<KlineData> klineData, int interval)
        {
            if (interval <= 0)
            {
                throw new ArgumentException("Interval must be a positive integer.");
            }

            if (klineData.Count() == 0)
            {
                throw new ArgumentException("Data cannot be empty.");
            }

            // Sort prices by time in descending order (latest first)
            var pricesFromLatest = klineData.OrderByDescending(p => p.CloseTime).ToList();

            // Get the latest time in the data set
            DateTime latestTime = pricesFromLatest.First().CloseTime;

            // Select prices that align with the interval based on DateTime comparison
            var selectedPrices = pricesFromLatest
                .Where(p => (latestTime - p.CloseTime).TotalMinutes % interval == 0)
                .Select(p => p.ClosePrice)
                .ToList();

            // Ensure we have enough data to calculate
            if (!selectedPrices.Any())
            {
                throw new ArgumentException("Not enough price data for the given interval.");
            }

            // Calculate the average of the selected prices
            decimal average = selectedPrices.Average();

            return average;
        }
    }
}
