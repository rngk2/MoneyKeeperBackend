using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MoneyKeeper.Extensions
{
	public static class StringExtension
	{
        public static string Hash(this string str) => BCrypt.Net.BCrypt.HashPassword(str, BCrypt.Net.BCrypt.GenerateSalt(12));
        
        public static bool HashEquals(this string hashed, string toCheck) => BCrypt.Net.BCrypt.Verify(toCheck, hashed);
    }
}
