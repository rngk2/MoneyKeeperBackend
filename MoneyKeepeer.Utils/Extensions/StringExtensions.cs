using BCrypt.Net;

namespace MoneyKeeper.Utils.Extensions
{
	public static class StringExtensions
	{
        public static string Hash(this string str) => BCrypt.Net.BCrypt.HashPassword(str, BCrypt.Net.BCrypt.GenerateSalt(12));
        
        public static bool HashEquals(this string hashed, string toCheck) => BCrypt.Net.BCrypt.Verify(toCheck, hashed);
    }
}
