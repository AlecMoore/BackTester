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
        private readonly ITradeExecutor _tradeExecutor;

        public BacktestService(IKlineDataRepository klineDataRepository, ITradeExecutor tradeExecutor)
        {
            _klineDataRepository = klineDataRepository;
            _tradeExecutor = tradeExecutor;
        }

        public BacktestResult RunBacktest(Exchange Exchange, ITradingStrategy strategy, string Symbol, DateTime startDate, DateTime endDate)
        {
            var backtestResult = new BacktestResult();

            try
            {
                DateTime currentDate = startDate;

                var databaseKlines = _klineDataRepository.GetKlineData(Symbol, Exchange);

                while (currentDate <= endDate)
                {
                    var signal = strategy.GenerateSignal(databaseKlines, DateTimeExtensionMethods.NormaliseToMinute(currentDate));
                    var tradeResult = _tradeExecutor.ExecuteTrade(signal);

                    backtestResult.Trades.Add(tradeResult);
                    backtestResult.TotalProfit += tradeResult.Profit;

                    currentDate = currentDate.AddMinutes(1);
                }

                backtestResult.NetProfit = backtestResult.TotalProfit; // In this case, net profit is total profit
                return backtestResult;
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return backtestResult;
            }

        }
    }

}
