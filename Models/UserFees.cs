
using Bybit.Net.Objects.Models.V5;

namespace BackTester.Models
{
    public class UserFees : DataBaseObject
    {
        public UserFees(decimal makerCommission, decimal takerCommission, ExchangeEnum exchange, string pair, DateTime createDate)
        {
            MakerCommission = makerCommission;
            TakerCommission = takerCommission;
            Exchange = exchange;
            Pair = pair;
            CreateDate = createDate;
        }

        public decimal MakerCommission { get; set; }
        public decimal TakerCommission { get; set; }
    }
}
