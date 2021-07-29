using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos;

namespace MoneyKeeper.Services
{
	public interface IUserService
	{
		Task<UserDto> CreateUser(CreateUserDto userDto);
		Task<ActionResult> DeleteUser(int id);
		Task<ActionResult<UserDto>> GetUser(int id);
		Task<IEnumerable<UserDto>> GetUsers();
		Task<ActionResult> UpdateUser(int id, UpdateUserDto userDto);
	}
}