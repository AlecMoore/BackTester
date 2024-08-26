using CryptoExchange.Net.CommonObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingBots.Interfaces;
using TradingBots.Models;

namespace TradingBots.Repositories
{
    public class KlineDataRepository : IKlineDataRepository
    {
        private readonly string _connectionString;

        public KlineDataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds new kline data to data base
        /// </summary>
        /// <param name="klineData"></param>
        public void AddKlineData(KlineData klineData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO KlineData (ClosePrice, CloseTime, HighPrice, " +
                    "LowPrice, OpenPrice, OpenTime, QuoteVolume, TakerBuyBaseVolume, TakerBuyQuoteVolume, TradeCount, " +
                    "Volume, Pair, Exchange) VALUES (@ClosePrice, @CloseTime, @HighPrice, @LowPrice, @OpenPrice, @OpenTime, " +
                    "@QuoteVolume, @TakerBuyBaseVolume, @TakerBuyQuoteVolume, @TradeCount, @Volume, @Pair, @Exchange)",
                    connection);
                command.Parameters.AddWithValue("@ClosePrice", klineData.ClosePrice);
                command.Parameters.AddWithValue("@CloseTime", klineData.CloseTime);;
                command.Parameters.AddWithValue("@HighPrice", klineData.HighPrice);
                command.Parameters.AddWithValue("@LowPrice", klineData.LowPrice);
                command.Parameters.AddWithValue("@OpenPrice", klineData.OpenPrice);
                command.Parameters.AddWithValue("@OpenTime", klineData.OpenTime); ;
                command.Parameters.AddWithValue("@QuoteVolume", klineData.QuoteVolume);
                command.Parameters.AddWithValue("@TakerBuyBaseVolume", klineData.TakerBuyBaseVolume);
                command.Parameters.AddWithValue("@TakerBuyQuoteVolume", klineData.TakerBuyQuoteVolume);
                command.Parameters.AddWithValue("@TradeCount", klineData.TradeCount);
                command.Parameters.AddWithValue("@Volume", klineData.Volume);
                command.Parameters.AddWithValue("@Pair", klineData.Pair);
                command.Parameters.AddWithValue("@Exchange", klineData.Exchange);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets all existing entries in database for kline data of specific pair at exchange
        /// </summary>
        /// <param name="Pair"></param>
        /// <param name="Exchange"></param>
        /// <returns></returns>
        public IEnumerable<KlineData> GetKlineData(string Pair, string Exchange)
        {
            var klines = new List<KlineData>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM KlineData WHERE Pair = @Pair AND Exchange = @Exchange", connection))
                {
                    command.Parameters.AddWithValue("@Pair", Pair);
                    command.Parameters.AddWithValue("@Exchange", Exchange);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            klines.Add(new KlineData
                            {
                                Id = reader.GetInt32(0),
                                ClosePrice = reader.GetDecimal(1),
                                CloseTime = reader.GetDateTime(2),
                                HighPrice = reader.GetDecimal(3),
                                LowPrice = reader.GetDecimal(4),
                                OpenPrice = reader.GetDecimal(5),
                                OpenTime = reader.GetDateTime(6),
                                QuoteVolume = reader.GetDecimal(7),
                                TakerBuyBaseVolume = reader.GetDecimal(8),
                                TakerBuyQuoteVolume = reader.GetDecimal(9),
                                TradeCount = reader.GetInt32(10),
                                Volume = reader.GetDecimal(11),
                                Pair = reader.GetString(12),
                                Exchange = reader.GetString(13)
                            });
                        }
                    }
                }
            }
            return klines;
        }
    }

}
