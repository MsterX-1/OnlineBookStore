using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Book
    {
        public required string ISBN { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? Pub_Year { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int Stock_Qty { get; set; }
        public int Threshold { get; set; }
        public int? Publisher_ID { get; set; }
        public byte[]? BookPhoto { get; set; }
    }
}
