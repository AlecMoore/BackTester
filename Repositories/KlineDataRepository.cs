using System.Data.SqlClient;
using BackTester.Interfaces;
using BackTester.Models;
using CryptoClients.Net.Enums;

namespace BackTester.Repositories
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
                command.Parameters.AddWithValue("@Pair", klineData.Symbol);
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
        public IEnumerable<KlineData> GetKlineData(string Pair, Exchange Exchange)
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
                            klines.Add(new KlineData(reader.GetInt32(0), reader.GetDecimal(1), reader.GetDateTime(2), reader.GetDecimal(3),
                                reader.GetDecimal(4), reader.GetDecimal(5), reader.GetDateTime(6), reader.GetDecimal(7), reader.GetDecimal(8),
                                reader.GetDecimal(9), reader.GetInt32(10), reader.GetDecimal(11), (Exchange)reader.GetInt32(13), 
                                reader.GetString(12), reader.GetDateTime(14)));
                        }
                    }
                }
            }
            return klines;
        }
    }

}
