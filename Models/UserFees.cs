using CryptoClients.Net.Enums;

namespace BackTester.Models
{
    public class UserFees : DataBaseObject
    {
        public UserFees(decimal makerCommission, decimal takerCommission, Exchange exchange, string symbol, DateTime createDate)
        {
            MakerCommission = makerCommission;
            TakerCommission = takerCommission;
            Exchange = exchange;
            Symbol = symbol;
            CreateDate = createDate;
        }

        public decimal MakerCommission { get; set; }
        public decimal TakerCommission { get; set; }
    }
}
