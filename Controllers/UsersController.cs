using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Repositories;
using MoneyKeeper.Services;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
            this.userService = userService;
		}

        // GET /users
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return await userService.GetUsers();
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await userService.GetUser(id);
            return user ?? NotFound();
        }

        // POST /users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
			try
			{
                UserDto created = await userService.CreateUser(userDto);

                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = created.Id },
                    created
                );
            }
			catch (SqlException e)
			{
				if (e.Number == 2627) // dublicate key error number
				{
					return StatusCode(409, $"User with email={userDto.Email} already exist");
				}
                return StatusCode(500);
			}
        }

		// PUT /users/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateUser(int id, UpdateUserDto userDto)
		{
            var result = await userService.UpdateUser(id, userDto);
			return result is ConflictResult ? StatusCode(409, $"User with email={userDto.Email} already exist") : result;
		}

		// DELETE /users/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
            return await userService.DeleteUser(id);
		}
	}
}
