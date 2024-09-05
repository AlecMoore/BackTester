using CryptoClients.Net.Enums;

namespace BackTester.Models
{
    public class DataBaseObject
    {
        public Exchange Exchange { get; set; } 
        public string? Symbol { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
