using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Settings;

namespace MoneyKeeper.Repositories
{
	public class DapperUsersRepository : IUsersRepository
	{
		private readonly string connectionString;

		public DapperUsersRepository(IOptions<DapperSettings> options)
		{
			connectionString = options.Value.DefaultConnection;
		}

		public async Task<User> GetUser(int id)
		{
			const string getUserQuery = "select * from users where Id = @id";
			await using var connection = new SqlConnection(connectionString);
			
			return (await connection.QueryAsync<User>(getUserQuery, new { id }))
				.ToList().FirstOrDefault();
		}

		public async Task<User> GetUser(string email)
		{
			const string getUserQuery = "select * from users where Email = @email";
			await using var connection = new SqlConnection(connectionString);

			return (await connection.QueryAsync<User>(getUserQuery, new { email }))
				.ToList().FirstOrDefault();
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			const string getUsersQuery = "select * from users";
			await using var connection = new SqlConnection(connectionString);
			
			return await connection.QueryAsync<User>(getUsersQuery); // users
		}

		public async Task CreateUser(User user)
		{
			const string createUserQuery = @"
								insert into [dbo].[Users]
									([Id], [FirstName], [LastName], [Email], [Password])
								values 
									(@Id, @FirstName, @LastName, @Email, @Password)
			";
			await using var connection = new SqlConnection(connectionString);

			await connection.ExecuteAsync(createUserQuery, user);
		}

		public async Task UpdateUser(User userData)
		{
			const string createUserQuery = @"
					update [dbo].[Users]
						set 
							FirstName = @FirstName,
							LastName = @LastName,
							Email = @Email,
							Password = @Password
						where
							Id = @Id
							
			";
			await using var connection = new SqlConnection(connectionString);

			await connection.ExecuteAsync(createUserQuery, userData);
		}

		public async Task DeleteUser(int id)
		{
			const string createUserQuery = @"
					delete from [dbo].[Users]
						where
							Id = @id
							
			";
			await using var connection = new SqlConnection(connectionString);

			await connection.ExecuteAsync(createUserQuery, new { id });
		}

	}
}
