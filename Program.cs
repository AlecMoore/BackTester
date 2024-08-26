﻿using CryptoClients.Net;
using CryptoClients.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingBots.Interfaces;
using TradingBots.Repositories;
using TradingBots.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add the appsettings.json file
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<IExchangeRestClient, ExchangeRestClient>(provider =>
        {
            return new ExchangeRestClient(binanceRestOptions: (options) =>
            {
                options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("BinanceKey", "BinanceSecret");
            });
        });
        services.AddScoped<IKlineDataRepository, KlineDataRepository>(provider =>
        {
            var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
            return new KlineDataRepository(connectionString ?? "");
        });
        services.AddScoped<KlineDataService>();
    })
    .Build();

var host = builder.Services.GetRequiredService<KlineDataService>();

//Add ETH Klines to DB
await host.SyncKlineDataAsync("ETHUSDT", "Binance");