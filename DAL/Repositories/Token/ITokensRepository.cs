using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyKeeper.DAL.Entities;

namespace MoneyKeeper.DAL.Repositories
{
	public interface ITokensRepository
	{
		Task<RefreshToken> GetToken(string token);
	
		Task<User> GetUserByRefreshToken(string token);
		
		Task AddRefreshToken(RefreshToken token);

		Task RemoveRefreshToken(string token);
		
		Task RemoveOldRefreshTokensOf(int userId);
	}
}
