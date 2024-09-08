using BackTester.Exchanges;
using CryptoClients.Net.Enums;

namespace BackTester.Services
{
    public class FeeDataService
    {
        private readonly Dictionary<Exchange, IExchangeRepository> _exchangeRepositories;

        public FeeDataService (Dictionary<Exchange, IExchangeRepository> exchangeRepositories)
        {
            _exchangeRepositories = exchangeRepositories;
        }

        /// <summary>
        /// Gets fee values from api
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task GetFeeDataAsync(string symbol, IEnumerable<Exchange> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                if (_exchangeRepositories.TryGetValue(exchange, out var repository))
                {
                        var fees = await repository.GetUserFees(symbol);
                        if(fees != null)
                    {
                        Console.WriteLine(fees.ToString());
                    }
                    else
                    {
                        Console.Error.WriteLine("No fee results returned");
                    }
                }
                else
                {
                    // Handle case where the exchange is not found in the dictionary
                    Console.WriteLine($"No repository found for {exchange}");
                }
            }
        }
    }
}
