
namespace BackTester.Models
{
    public class OpenOrder : DataBaseObject
    {
        public OpenOrder(decimal price, decimal quantity, decimal quantityFilled, string orderType,
            string orderStatus, string orderSide, DateTime orderTime, ExchangeEnum exchange, string pair,
            DateTime createDate)
        {
            Price = price;
            Quantity = quantity;
            QuantityFilled = quantityFilled;
            OrderType = orderType;
            OrderStatus = orderStatus;
            OrderSide = orderSide;
            OrderTime = orderTime;
            Exchange = exchange;
            Pair = pair;
            CreateDate = createDate;
        }

        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityFilled { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderSide { get; set; }
        public DateTime OrderTime { get; set; }
    }
}