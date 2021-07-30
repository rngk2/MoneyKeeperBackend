﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Repositories;

namespace MoneyKeeper.Services
{
	public class UserService : IUserService
	{
		private readonly IUsersRepository repository;

		public UserService(IUsersRepository usersRepository)
		{
			repository = usersRepository;
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			return await repository.GetUsers();
		}

		public async Task<User> GetUser(int id)
		{
			var user = await repository.GetUser(id);
			return user;
		}

		public async Task<User> GetUser(string email)
		{
			var user = await repository.GetUser(email);
			return user;
		}
		
		public async Task<User> CreateUser(CreateUserDto userDto)
		{
			User newUser = new()
			{
				FirstName = userDto.FirstName,
				LastName = userDto.LastName,
				Email = userDto.Email,
				Password = userDto.Password.Hash()
			};

			int createdId = await repository.CreateUser(newUser);

			User created = newUser with
			{
				Id = createdId
			};

			return created; 
		}


		public async Task UpdateUser([NotNull] User existingUser, UpdateUserDto data)
		{
			Func<object, object, object> returnDefaultIfNull = (object nullable, object @default)
				=> nullable is null ? @default : nullable;

			User updatedUser = existingUser with
			{
				FirstName = returnDefaultIfNull(data.FirstName, existingUser.FirstName).ToString(),
				LastName = returnDefaultIfNull(data.LastName, existingUser.LastName).ToString(),
				Email = returnDefaultIfNull(data.Email, existingUser.Email).ToString(),
				Password = returnDefaultIfNull(data.Password?.Hash(), existingUser.Password).ToString()
			};

			await repository.UpdateUser(updatedUser);

		}

		public async Task DeleteUser(int id)
		{
			await repository.DeleteUser(id);
		}
	}
}