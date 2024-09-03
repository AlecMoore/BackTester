using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Models
{
    public class DataBaseObject
    {
        public ExchangeEnum Exchange { get; set; } 
        public string? Pair { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
