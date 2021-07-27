﻿using System;
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

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUsersRepository repository;

		public UsersController(IUsersRepository usersRepository)
		{
			repository = usersRepository;
		}

        // GET /users
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return (await repository.GetUsers()).Select(user => user.AsDto());
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await repository.GetUser(id);
            return user is null ? NotFound() : user.AsDto();
        }

        // POST /users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
            User newUser = new()
            {
                FirstName = userDto.FirstName,
                LastName  = userDto.LastName,
                Email     = userDto.Email,
                Password  = userDto.Password.AsSHA256Hash()
            };

			try
			{
			    await repository.CreateUser(newUser);
			}
			catch (SqlException e)
			{
				if (e.Number == 2627) // dublicate key error number
				{
					return StatusCode(409, $"User with email={newUser.Email} already exist");
				}
			}

			return CreatedAtAction(
                    nameof(GetUser),
                    new { id = newUser.Id },
                    newUser.AsDto()
                );
        }

		// PUT /users/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateUser(int id, UpdateUserDto userDto)
		{
            Func<object, object, object> returnDefaultIfNull = (object nullable, object @default)
                => nullable is null ? @default : nullable;

            var existingUser = await repository.GetUser(id);

            if (existingUser is null) return NotFound();

            User updatedUser = existingUser with
            {
                FirstName = returnDefaultIfNull(userDto.FirstName, existingUser.FirstName).ToString(),
                LastName  = returnDefaultIfNull(userDto.LastName, existingUser.LastName).ToString(),
                Email     = returnDefaultIfNull(userDto.Email, existingUser.Email).ToString(),
                Password  = returnDefaultIfNull(userDto.Password?.AsSHA256Hash(), existingUser.Password).ToString()
            };

            await repository.UpdateUser(updatedUser);

            return NoContent();
        }

		// DELETE /users/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
            await repository.DeleteUser(id);

            return NoContent();
		}
	}
}