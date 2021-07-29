using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Services
{
	public interface IUserService
	{
		Task<User> CreateUser(CreateUserDto userDto);

		Task DeleteUser(int id);

		Task<User> GetUser(int id);

		Task<IEnumerable<User>> GetUsers();

		Task UpdateUser(User existingUser, UpdateUserDto userDto);
	}
}