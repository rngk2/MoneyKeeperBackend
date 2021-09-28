using System.Collections.Generic;
using System.Threading.Tasks;
using MoneyKeeper.BL.Dtos.User;
using MoneyKeeper.Utils.Results;

namespace MoneyKeeper.BL.Services
{
	public interface IUserService
	{
		Task<Result<User>> GetUser(int id);

		Task<Result<User>> GetUser(string email);

		Task<Result<User>> CreateUser(CreateUser userDto);

		Task<Result<User>> UpdateUser(int id, UpdateUser userDto);

		Task<Result<User>> DeleteUser(int id);
	}
}