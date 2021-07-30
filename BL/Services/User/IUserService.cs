using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dtos.User;
using DAL.Entities;

namespace BL.Services
{
	public interface IUserService
	{
		Task<User> CreateUser(CreateUserDto userDto);

		Task DeleteUser(int id);

		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<IEnumerable<User>> GetUsers();

		Task UpdateUser(User existingUser, UpdateUserDto userDto);
	}
}