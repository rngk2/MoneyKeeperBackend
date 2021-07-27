using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MoneyKeeper.Entities;
using MoneyKeeper.Settings;

namespace MoneyKeeper.Repositories
{
	public class DapperUsersRepository : DapperRepository<User>, IUsersRepository
	{
		public DapperUsersRepository(IOptions<DapperSettings> options) : base(options) { }

		public async Task<User> GetUser(int id)
		{
			const string getUserQuery = "select * from users where Id = @id";
			return (await QueryAny(getUserQuery, new { id })).FirstOrDefault();
		}

		public async Task<User> GetUser(string email)
		{
			const string getUserQuery = "select * from users where Email = @email";
			return (await QueryAny(getUserQuery, new { email })).FirstOrDefault();
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			const string getUsersQuery = "select * from users";
			return await QueryAny(getUsersQuery);
		}

		public async Task CreateUser(User user)
		{
			const string createUserQuery = @"
								insert into [dbo].[Users]
									([Id], [FirstName], [LastName], [Email], [Password])
								values 
									(@Id, @FirstName, @LastName, @Email, @Password)
			";

			await ExecuteAny(createUserQuery, user);
		}

		public async Task UpdateUser(User userData)
		{
			const string updateUserQuery = @"
					update [dbo].[Users]
						set 
							FirstName = @FirstName,
							LastName = @LastName,
							Email = @Email,
							Password = @Password
						where
							Id = @Id
							
			";

			await ExecuteAny(updateUserQuery, userData);
		}

		public async Task DeleteUser(int id)
		{
			const string deleteUserQuery = @"
					delete from [dbo].[Users]
						where
							Id = @id
							
			";
			
			await ExecuteAny(deleteUserQuery, new { id });
		}

	}
}
