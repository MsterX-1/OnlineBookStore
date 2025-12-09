using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BookAuthor
    {
        public required string ISBN { get; set; }
        public int Author_ID { get; set; }
    }
}
