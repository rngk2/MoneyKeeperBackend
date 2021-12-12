using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MoneyKeeper.DAL.Entities;
using MoneyKeeper.DAL.Settings;
using MoneyKeeper.Globals;
using MoneyKeeper.DAL;

namespace MoneyKeeper.DAL.Repositories
{
	internal class DapperUsersRepository : DapperRepository, IUsersRepository
	{
		public DapperUsersRepository(ICategoriesRepository categoriesRepository, IOptions<DapperSettings> options)
			: base(options)	{ }

		public async Task<User> GetUser(int id)
		{
			string sql = "select * from Users where Id = @id";

			return (await QueryAny<User>(sql, new { id })).FirstOrDefault();
		}

		public async Task<User> GetUser(string email)
		{
			string sql = "select * from Users where Email = @email";

			return (await QueryAny<User>(sql, new { email })).FirstOrDefault();
		}

		public async Task<int> CreateUser(User user)
		{
			string sql = @"
					insert into Users
						([FirstName], [LastName], [Email], [Password])
					output 
						inserted.Id
					values 
						(@FirstName, @LastName, @Email, @Password)
			";

			int createdId = await QuerySingleWithOutput<int>(sql, user);

			return createdId;
		}

		
		public async Task<bool> UpdateUser(User userData)
		{
			string sql = @"
					update
						Users
					set 
						FirstName = @FirstName,
						LastName = @LastName,
						Email = @Email,
						Password = @Password
					where
						Id = @Id		
			";

			return await ExecuteAny(sql, userData) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteUser(int id)
		{
			string sql_deleteUser = @"
					delete from Users
					where
						Id = @id
			";

			return await ExecuteAny(sql_deleteUser, new { id }) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}
