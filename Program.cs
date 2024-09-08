using CryptoClients.Net;
using CryptoClients.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackTester.Interfaces;
using BackTester.Repositories;
using BackTester.Services;
using BackTester.Exchanges;
using CryptoClients.Net.Enums;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<IExchangeRestClient, ExchangeRestClient>(provider =>
        {
            return new ExchangeRestClient(binanceRestOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BinanceKey", "BinanceSecret");
            },
            bitfinexRestOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BitfinexKey", "BitfinexSecret");
            },
            bybitRestOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BybitKey", "BybitSecret");
            });
        });

        services.AddHttpClient<IExchangeSocketClient, ExchangeSocketClient>(provider =>
        {
            return new ExchangeSocketClient(binanceSocketOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BinanceKey", "BinanceSecret");
            },
            bitfinexSocketOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BitfinexKey", "BitfinexSecret");
            },
            bybitSocketOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BybitKey", "BybitSecret");
            });
        });

        services.AddScoped<IKlineDataRepository, KlineDataRepository>(provider =>
        {
            var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
            return new KlineDataRepository(connectionString ?? "");
        });
        services.AddScoped<KlineDataService>();
        services.AddScoped<FeeDataService>();

        services.AddScoped<BinanceExchangeRepository>(provider =>
        {
            var restClient = provider.GetRequiredService<IExchangeRestClient>().Binance;
            var socketClient = provider.GetRequiredService<IExchangeSocketClient>().Binance;
            return new BinanceExchangeRepository(restClient, socketClient);
        });

        services.AddScoped<BitfinexExchangeRepository>(provider =>
        {
            var restClient = provider.GetRequiredService<IExchangeRestClient>().Bitfinex;
            var socketClient = provider.GetRequiredService<IExchangeSocketClient>().Bitfinex;
            return new BitfinexExchangeRepository(restClient, socketClient);
        });

        services.AddScoped<BybitExchangeRepository>(provider =>
        {
            var restClient = provider.GetRequiredService<IExchangeRestClient>().Bybit;
            var socketClient = provider.GetRequiredService<IExchangeSocketClient>().Bybit;
            return new BybitExchangeRepository(restClient, socketClient);
        });

        // Register the dictionary of exchanges as a scoped service
        services.AddScoped<Dictionary<Exchange, IExchangeRepository>>(provider =>
        {
            return new Dictionary<Exchange, IExchangeRepository>
            {
                { Exchange.Binance, provider.GetRequiredService<BinanceExchangeRepository>() },
                { Exchange.Bitfinex, provider.GetRequiredService<BitfinexExchangeRepository>() },
                { Exchange.Bybit, provider.GetRequiredService<BybitExchangeRepository>() }
            };
        });
    })
    .Build();

// Retrieve the KlineDataService and FeeDataService from the service provider
var klineService = builder.Services.GetRequiredService<KlineDataService>();
var feeService = builder.Services.GetRequiredService<FeeDataService>();

// Set exchange APIs to hit
IEnumerable<Exchange> exchanges = new List<Exchange>() { Exchange.Binance };

// Add ETH Klines to DB
await klineService.SyncKlineDataAsync("ETHUSDT", exchanges);

//await feeService.GetFeeDataAsync("ETHUSDT", exchanges);
