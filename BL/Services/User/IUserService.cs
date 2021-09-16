using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dtos.User;
using DAL.Entities;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface IUserService
	{
		Task<Result<User>> GetUser(int id);

		Task<Result<User>> GetUser(string email);

		Task<Result<User>> CreateUser(CreateUserDto userDto);

		Task<Result<User>> UpdateUser(int id, UpdateUserDto userDto);

		Task<Result<User>> DeleteUser(int id);
	}
}