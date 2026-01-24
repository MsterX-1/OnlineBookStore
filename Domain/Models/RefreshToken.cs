using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken
    {
        public int User_ID { get; set; }
        public required string Token { get; set; }
        public required DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;
    }
}
