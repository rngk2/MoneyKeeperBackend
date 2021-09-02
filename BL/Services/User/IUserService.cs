using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dtos.User;
using DAL.Entities;
using DAL.Models;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface IUserService
	{
		Task<Result<User>> CreateUser(CreateUserDto userDto);

		Task<Result<User>> DeleteUser(int id);

		Task<Result<User>> GetUser(int id);

		Task<Result<User>> GetUser(string email);

		Task<IEnumerable<User>> GetUsers();

		Task<Result<User>> UpdateUser(int id, UpdateUserDto userDto);

		Task<Result<IEnumerable<SummaryUnit>>> GetSummaryForUser(int id);

		Task<Result<Dictionary<string, decimal>>> GetTotalForUser_ForMonth(int id);

		Task<Result<Dictionary<string, decimal>>> GetTotalForUser_ForYear(int id);
	}
}