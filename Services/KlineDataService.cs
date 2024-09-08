using BackTester.Extensions;
using BackTester.Interfaces;
using BackTester.Exchanges;
using CryptoClients.Net.Enums;

namespace BackTester.Services
{
    public class KlineDataService
    {
        private readonly IKlineDataRepository _klineDataRepository;
        private readonly Dictionary<Exchange, IExchangeRepository> _exchangeRepositories;

        public KlineDataService(IKlineDataRepository klineDataRepository, Dictionary<Exchange, IExchangeRepository> exchangeRepositories)
        {
            _klineDataRepository = klineDataRepository;
            _exchangeRepositories = exchangeRepositories;
        }

        /// <summary>
        /// Adds kline data to the database from exchange API.
        /// Binance API has a entry limit of 1000, so runs the call multiple times.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task SyncKlineDataAsync(string symbol, IEnumerable<Exchange> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                if (_exchangeRepositories.TryGetValue(exchange, out var repository))
                {
                    int numberOfRuns = 20;
                    while (numberOfRuns > 0)
                    {
                        var databaseKlines = _klineDataRepository.GetKlineData(symbol, exchange);

                        var timeRange = DetermineTimeRange(symbol, exchange);

                        DateTime? startTime = timeRange.StartTime;
                        DateTime? endTime = timeRange.EndTime;

                        var klines = await repository.GetKlineData(symbol, startTime, endTime);
                        if (klines != null)
                        {
                            foreach (var kline in klines)
                            {
                                //Checks kline is not already in db
                                if (!databaseKlines.Any(k => k.CloseTime == kline.CloseTime))
                                {
                                    _klineDataRepository.AddKlineData(kline);
                                }
                                else
                                {
                                    Console.WriteLine(exchange.ToString() + symbol + kline.CloseTime + "Already exists in DB");
                                    return;
                                }
                            }

                        } 
                        else
                        {
                            Console.Error.WriteLine("No kline results returned");
                        }
                        
                        numberOfRuns--;
                    }
                }
                else
                {
                    // Handle case where the exchange is not found in the dictionary
                    Console.Error.WriteLine($"No repository found for {exchange}");
                }
            }
        }

        /// <summary>
        /// Ensures the dates provided to the API will first be up to current date. 
        /// Then get as many entries as it can before the earliest entry.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public (DateTime? StartTime, DateTime? EndTime) DetermineTimeRange(string symbol, Exchange exchange)
        {
            var databaseKlines = _klineDataRepository.GetKlineData(symbol, exchange);
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
