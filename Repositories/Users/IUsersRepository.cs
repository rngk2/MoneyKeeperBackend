using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<IEnumerable<User>> GetUsers();

		Task<int> CreateUser(User user);

		Task UpdateUser(User user);

		Task DeleteUser(int id);

	}
}
