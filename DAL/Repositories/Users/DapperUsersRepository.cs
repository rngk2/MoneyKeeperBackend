using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DAL.Entities;
using DAL.Settings;
using DAL.Utils;
using DAL.Models;

namespace DAL.Repositories
{
	public class DapperUsersRepository : IUsersRepository
	{
		private readonly IDapperRepository dapperRepository;

		private const string USERS_TABLE_NAME = "Users";

		public DapperUsersRepository(IDapperRepository dapperRepository) {
			this.dapperRepository = dapperRepository;
		}

		public async Task<User> GetUser(int id)
		{
			string sql = $"select * from {USERS_TABLE_NAME} where Id = @id";

			return (await dapperRepository.QueryAny<User>(sql, new { id })).FirstOrDefault();
		}

		public async Task<User> GetUser(string email)
		{
			string sql = $"select * from {USERS_TABLE_NAME} where Email = @email";

			return (await dapperRepository.QueryAny<User>(sql, new { email })).FirstOrDefault();
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			string sql = "select * from users";

			return await dapperRepository.QueryAny<User>(sql);
		}

		public async Task<int> CreateUser(User user)
		{
			string sql = @$"
					insert into {USERS_TABLE_NAME}
						([FirstName], [LastName], [Email], [Password])
					output 
						inserted.Id
					values 
						(@FirstName, @LastName, @Email, @Password)
			";

			return await dapperRepository.QuerySingleWithOutput<int>(sql, user);
		}

		public async Task UpdateUser(User userData)
		{
			string sql = @$"
					update
						{USERS_TABLE_NAME}
					set 
						FirstName = @FirstName,
						LastName = @LastName,
						Email = @Email,
						Password = @Password
					where
						Id = @Id		
			";

			await dapperRepository.ExecuteAny(sql, userData);
		}

		public async Task DeleteUser(int id)
		{
			string sql = @$"
					delete from {USERS_TABLE_NAME}
					where
						Id = @id
			";

			await dapperRepository.ExecuteAny(sql, new { id });
		}

		public async Task<IEnumerable<SummaryUnit>> GetSummaryForUser_ForMonth(int id)
		{
			string sql = @$"
					select 
						Users.Id UserId, Categories.Name CategoryName, Categories.Id CategoryId, Transactions.*
					from 
						{USERS_TABLE_NAME} 
					join 
						Categories on Users.Id = Categories.UserId
					left outer join 
						Transactions on Categories.Id = Transactions.CategoryId
					where 
						Users.Id=@id and month(Timestamp) = month(getdate())
			";

			return await dapperRepository.QueryAny<SummaryUnit>(sql, new { id });
		}
		
		public async Task<IEnumerable<SummaryUnit>> GetSummaryForUser_ForYear(int id)
		{
			string sql = @$"
					select 
						Users.Id UserId, Categories.Name CategoryName, Categories.Id CategoryId, Transactions.*
					from 
						{USERS_TABLE_NAME} 
					join 
						Categories on Users.Id = Categories.UserId
					left outer join 
						Transactions on Categories.Id = Transactions.CategoryId
					where 
						Users.Id=@id and year(Timestamp) = year(getdate())
			";

			return await dapperRepository.QueryAny<SummaryUnit>(sql, new { id });
		}
	}
}
