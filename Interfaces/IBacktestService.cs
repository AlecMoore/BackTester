using BackTester.Models;
using CryptoClients.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Interfaces
{
    public interface IBacktestService
    {
        Task<BacktestResult> RunBacktest(Exchange Exchange, ITradingStrategy strategy, string Symbol, DateTime startDate, DateTime endDate);
    }

}
