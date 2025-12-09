using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Author
    {
        public int Author_ID { get; set; }
        public required string Name { get; set; }
    }
}
