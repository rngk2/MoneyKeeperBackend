using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.DAL.Entities;

namespace MoneyKeeper.DAL.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<int> CreateUser(User user);

		Task<bool> UpdateUser(User user);

		Task<bool> DeleteUser(int id);
	}
}
