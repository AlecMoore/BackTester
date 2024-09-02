﻿using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoClients.Net.Interfaces;
using System.Collections.Generic;
using TradingBots.Extensions;
using TradingBots.Interfaces;
using TradingBots.Models;
using TradingBots.Repositories;


namespace TradingBots.Services
{
    public class FeeDataService
    {
        private readonly IExchangeRestClient _exhangeRestClient;

        public FeeDataService(IExchangeRestClient exhangeRestClient)
        {
            _exhangeRestClient = exhangeRestClient;
        }

        /// <summary>
        /// Gets fee values from api
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task GetFeeDataAsync(string pair, string exchange)
        {
            if (exchange == "Binance")
            {
                var binanceFees = _exhangeRestClient.Binance.SpotApi.Account.GetTradeFeeAsync(pair);
                await Task.WhenAll(binanceFees);

                if (binanceFees.Result.Success)
                {
                    foreach (var fee in binanceFees.Result.Data)
                    {
                        var feeData = FeeData.FromBinanceInterface(fee, pair, exchange);
                        Console.WriteLine(feeData.ToString());

                    }
                }
                else
                {
                    Console.WriteLine(binanceFees.Result.Error);
                    return;
                }
            }
        }
    }
}