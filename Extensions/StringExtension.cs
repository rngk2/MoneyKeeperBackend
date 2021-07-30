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
        public static string AsSHA256Hash(this string str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);

            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}
