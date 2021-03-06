using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }

        public string RevokedByIp { get; set; }

        public string ReplacedByToken { get; set; }

        public string ReasonRevoked { get; set; }

		public bool IsExpired => DateTime.UtcNow >= Expires;

		public bool IsRevoked => Revoked != null && Revoked != (new DateTime()).AddYears(1899);

		public bool IsActive => !IsRevoked && !IsExpired;
	}
}
