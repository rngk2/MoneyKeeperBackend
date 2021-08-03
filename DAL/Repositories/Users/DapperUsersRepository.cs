using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DAL.Entities;
using DAL.Settings;

namespace DAL.Repositories
{
	public class DapperUsersRepository : IUsersRepository
	{
		private readonly IDapperRepository dapperRepository;

		public DapperUsersRepository(IDapperRepository dapperRepository) {
			this.dapperRepository = dapperRepository;
		}

		public async Task<User> GetUser(int id)
		{
			const string getUserQuery = "select * from users where Id = @id";
			return (await dapperRepository.QueryAny<User>(getUserQuery, new { id })).FirstOrDefault();
		}

		public async Task<User> GetUser(string email)
		{
			const string getUserQuery = "select * from users where Email = @email";
			return (await dapperRepository.QueryAny<User>(getUserQuery, new { email })).FirstOrDefault();
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			const string getUsersQuery = "select * from users";
			return await dapperRepository.QueryAny<User>(getUsersQuery);
		}

		public async Task<int> CreateUser(User user)
		{
			const string createUserQuery = @"
								insert into [dbo].[Users]
									([FirstName], [LastName], [Email], [Password])
								output inserted.Id
								values 
									(@FirstName, @LastName, @Email, @Password)
			";

			return await dapperRepository.QuerySingleWithOutput<int>(createUserQuery, user);
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

			await dapperRepository.ExecuteAny<User>(updateUserQuery, userData);
		}

		public async Task DeleteUser(int id)
		{
			const string deleteUserQuery = @"
					delete from [dbo].[Users]
						where
							Id = @id
							
			";
			
			await dapperRepository.ExecuteAny<User>(deleteUserQuery, new { id });
		}

		public async Task AddRefreshToken(RefreshToken token)
		{
			string sql = Utils.SqlUtils.SqlInsertBuild(token, "RTokens");
			await dapperRepository.ExecuteAny<RefreshToken>(sql);
		}

		public async Task<User> GetUserByRefreshToken(string token)
		{
			var getUserId = "select * from RTokens where token = @token";
			var userId = (await dapperRepository.QueryAny<RefreshToken>(getUserId, new { token })).FirstOrDefault().UserId;
			var getUserById = "select * from users where Id = @userId";
			return (await dapperRepository.QueryAny<User>(getUserById, new { userId })).FirstOrDefault();
		}

		public async Task<RefreshToken> GetToken(string token)
		{
			var getTokenQuery = "select * from RTokens where token = @token";
			return (await dapperRepository.QueryAny<RefreshToken>(getTokenQuery, new { token })).FirstOrDefault();
		}
	}
}
