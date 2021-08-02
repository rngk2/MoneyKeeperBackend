using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth;

namespace Auth.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<IEnumerable<User>> GetUsers();

		Task<int> CreateUser(User user);

		Task UpdateUser(User user);

		Task DeleteUser(int id);

		Task AddRefreshToken(RefreshToken token);

		Task<User> GetUserByRefreshToken(string token);

		Task<RefreshToken> GetToken(string token);

	}
}
