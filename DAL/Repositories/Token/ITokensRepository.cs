using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories
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
