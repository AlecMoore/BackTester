using Binance.Net.Enums;
using CryptoClients.Net.Interfaces;
using TradingBots.Extensions;
using TradingBots.Interfaces;
using TradingBots.Models;


namespace TradingBots.Services
{
    public class KlineDataService
    {
        private readonly IKlineDataRepository _klineDataRepository;
        private readonly IExchangeRestClient _exhangeRestClient;

        public KlineDataService(IKlineDataRepository klineDataRepository, IExchangeRestClient exhangeRestClient)
        {
            _klineDataRepository = klineDataRepository;
            _exhangeRestClient = exhangeRestClient;
        }

        /// <summary>
        /// Adds kline data to the database from binance API.
        /// Binance API has a entry limit of 1000, so runs the call multiple times.
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task SyncKlineDataAsync(string pair, string exchange)
        {
            int numberOfRuns = 20;
            while(numberOfRuns > 0) { 
                var databaseKlines = _klineDataRepository.GetKlineData(pair, exchange);

                var timeRange = DetermineTimeRange(pair, exchange);

                DateTime? startTime = timeRange.StartTime;
                DateTime? endTime = timeRange.EndTime;

                if (exchange == "Binance")
                {
                    var binanceKlines = _exhangeRestClient.Binance.SpotApi.ExchangeData.GetKlinesAsync(pair, KlineInterval.OneMinute, startTime, endTime, limit: 1000);
                    await Task.WhenAll(binanceKlines);

                    if (binanceKlines.Result.Success)
                    {
                        foreach (var kline in binanceKlines.Result.Data)
                        {
                            KlineData klineData = new KlineData()
                            {
                                ClosePrice = kline.ClosePrice,
                                CloseTime = kline.CloseTime,
                                HighPrice = kline.HighPrice,
                                LowPrice = kline.LowPrice,
                                OpenPrice = kline.OpenPrice,
                                OpenTime = kline.OpenTime,
                                QuoteVolume = kline.QuoteVolume,
                                TakerBuyBaseVolume = kline.TakerBuyBaseVolume,
                                TakerBuyQuoteVolume = kline.TakerBuyQuoteVolume,
                                TradeCount = kline.TradeCount,
                                Volume = kline.Volume,
                                Pair = pair,
                                Exchange = exchange,
                            };

                            if (!databaseKlines.Contains(klineData))
                            {
                                _klineDataRepository.AddKlineData(klineData);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(binanceKlines.Result.Error);
                        return;
                    }

                    numberOfRuns--;
                }
            }
        }

        /// <summary>
        /// Ensures the dates provided to the API will first be up to current date. 
        /// Then get as many entries as it can before the earliest entry.
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public (DateTime? StartTime, DateTime? EndTime) DetermineTimeRange(string pair, string exchange)
        {
            var databaseKlines = _klineDataRepository.GetKlineData(pair, exchange);
            DateTime? startTime = null;
            DateTime? endTime = null;

            // If the dataset is empty
            if (!databaseKlines.Any())
            {
                endTime = DateTime.UtcNow;
                startTime = null;
            }
            else
            {
                var sortedDatabaseKlines = databaseKlines.OrderBy(kline => kline.CloseTime).ToList();

                // Get the latest and earliest dates in the dataset
                DateTime latestEntry = sortedDatabaseKlines.Max(kline => kline.CloseTime);
                DateTime earliestEntry = sortedDatabaseKlines.Min(kline => kline.CloseTime);

                // Normalize to minute precision
                latestEntry = DateTimeExtensionMethods.NormaliseToMinute(latestEntry);
                DateTime now = DateTimeExtensionMethods.NormaliseToMinute(DateTime.UtcNow);

                // Check if the latest entry matches the current date/time
                if (latestEntry < now)
                {
                    startTime = latestEntry;
                    endTime = DateTime.UtcNow;
                }
                else
                {
                    endTime = earliestEntry;
                }
            }

            return (startTime, endTime);
        }
    }

}
