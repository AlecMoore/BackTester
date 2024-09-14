using CryptoClients.Net.Enums;

namespace BackTester.Models
{
    public class OpenOrder : DataBaseObject
    {
        public OpenOrder(string id, decimal price, decimal quantity, decimal quantityFilled, string orderType,
            string orderStatus, string orderSide, DateTime orderTime, Exchange exchange, string symbol,
            DateTime createDate)
        {
            Id = id;
            Price = price;
            Quantity = quantity;
            QuantityFilled = quantityFilled;
            OrderType = orderType;
            OrderStatus = orderStatus;
            OrderSide = orderSide;
            OrderTime = orderTime;
            Exchange = exchange;
            Symbol = symbol;
            CreateDate = createDate;
        }

        public string Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityFilled { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderSide { get; set; }
        public DateTime OrderTime { get; set; }
    }
}