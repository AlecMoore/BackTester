using BackTester.Exchanges;
using BackTester.Extensions;
using BackTester.Interfaces;
using BackTester.Models;
using CryptoClients.Net.Enums;
using CryptoExchange.Net.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Services
{
    public class BacktestService : IBacktestService
    {
        private readonly IKlineDataRepository _klineDataRepository;
        private readonly IExchangeRepository _backTestExchangeRepository;

        public BacktestService(IKlineDataRepository klineDataRepository, IExchangeRepository backTestExchangeRepository)
        {
            _klineDataRepository = klineDataRepository;
            _backTestExchangeRepository = backTestExchangeRepository;
        }

        public async Task<BacktestResult> RunBacktest(Exchange Exchange, ITradingStrategy strategy, string Symbol, DateTime startDate, DateTime endDate)
        {
            var backtestResult = new BacktestResult();
            decimal cashBalance = 100;    // Starting with $100
            decimal currentPosition = 0;  // Track the number of assets bought
            decimal entryPrice = 0;       // Track the price at which the current position was opened
            decimal totalProfit = 0;      // Accumulate total profit

            try
            {
                DateTime currentDate = startDate;
                var databaseKlines = _klineDataRepository.GetKlineData(Symbol, Exchange)
                                                         .Where(k => k.CloseTime >= startDate && k.CloseTime <= endDate)
                                                         .OrderBy(k => k.CloseTime)
                                                         .ToList();

                int klineIndex = 0;

                while (currentDate <= endDate && klineIndex < databaseKlines.Count)
                {
                    var currentKline = databaseKlines[klineIndex];
                    DateTime normalizedKlineTime = DateTimeExtensionMethods.NormaliseToMinute(currentKline.CloseTime);

                    // Check if the current date is in sync with the Kline time, otherwise skip to the next available Kline
                    if (normalizedKlineTime < DateTimeExtensionMethods.NormaliseToMinute(currentDate))
                    {
                        klineIndex++;
                        continue;
                    }

                    if (normalizedKlineTime > DateTimeExtensionMethods.NormaliseToMinute(currentDate))
                    {
                        // If there's a gap, skip the missing time and move to the next available Kline time
                        currentDate = normalizedKlineTime;
                    }

                    // Get trading signal based on the current Kline data
                    var signal = strategy.GenerateSignal(databaseKlines, normalizedKlineTime);
                    decimal currentPrice = currentKline.ClosePrice;

                    if (signal.Action == TradeAction.Buy && currentPosition == 0)
                    {
                        // Calculate how much we can buy with the available cash balance
                        decimal quantityToBuy = cashBalance / currentPrice;

                        // Enter a position (Buy)
                        var trade = await _backTestExchangeRepository.PlaceOrder(Symbol, "buy", "market", quantityToBuy, currentPrice);
                        backtestResult.Trades.Add(trade);

                        // Update position info
                        currentPosition = trade.Quantity;    // The quantity bought based on the cash balance
                        entryPrice = trade.Price;            // Capture entry price
                        cashBalance = 0;                     // All cash is used to buy the position
                    }
                    else if (signal.Action == TradeAction.Sell && currentPosition > 0)
                    {
                        // Exit the position (Sell)
                        var trade = await _backTestExchangeRepository.PlaceOrder(Symbol, "sell", "market", currentPosition, currentPrice);
                        backtestResult.Trades.Add(trade);

                        // Calculate profit/loss
                        decimal exitPrice = trade.Price;
                        decimal profit = (exitPrice - entryPrice) * currentPosition;
                        totalProfit += profit;

                        // After selling, update cash balance with the value of sold assets
                        cashBalance = currentPosition * exitPrice;

                        // Reset position after selling
                        currentPosition = 0;
                        entryPrice = 0;
                    }

                    // Advance to the next Kline and update the current time
                    klineIndex++;
                    currentDate = currentDate.AddMinutes(1);
                }

                // Store total profit in the backtest result
                backtestResult.TotalProfit = totalProfit;
                backtestResult.NetProfit = totalProfit; // In this case, net profit is same as total profit
                return backtestResult;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return backtestResult;
            }
        }
    }
}
