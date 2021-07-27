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
		Task<User> GetUserAsync(int id);

		Task<User> GetUserAsync(string email);

		Task<IEnumerable<User>> GetUsersAsync();

		Task CreateUserAsync(User userDto);

		Task UpdateUserAsync(int id, User userDto);

		Task DeleteUserAsync(int id);

	}
}
