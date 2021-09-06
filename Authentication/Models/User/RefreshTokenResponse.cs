using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models
{
	public record RefreshTokenResponse (string Token)
    {
        public static implicit operator RefreshTokenResponse(string token) => new(token);
    }
}
