using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyKeeper.DAL.Entities;

namespace MoneyKeeper.DAL.Repositories
{
	internal class DapperTokensRepository : ITokensRepository
	{
		private readonly IDapperRepository dapperRepository;

		public DapperTokensRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<RefreshToken> GetToken(string token)
		{
			var getTokenQuery = "select * from RTokens where Token = @token";
			
			return (await dapperRepository.QueryAny<RefreshToken>(getTokenQuery, new { token })).FirstOrDefault();
		}

		public async Task<User> GetUserByRefreshToken(string token)
		{
			var sql = @"
				select 
					Users.* from RTokens
				join 
					Users on RTokens.UserId = Users.Id
				where 
					Token = @token
			";
			
			return (await dapperRepository.QueryAny<User>(sql, new { token })).FirstOrDefault();
		}

		public async Task AddRefreshToken(RefreshToken token)
		{
			string sql = @"
				insert into RTokens
					(UserId, Token, Expires, Created, CreatedByIp, Revoked, ReplacedByToken, ReasonRevoked, RevokedByIp)
				values
					(@UserId, @Token, @Expires, @Created, @CreatedByIp, @Revoked, @ReplacedByToken, @ReasonRevoked, @RevokedByIp)
			";

			await dapperRepository.ExecuteAny(sql, token);
		}

		public async Task RemoveOldRefreshTokensOf(int userId)
		{
			var sql = "delete from RTokens where UserId = @userId and Expires <= CURRENT_TIMESTAMP";

			await dapperRepository.ExecuteAny(sql, new { userId });
		}

		public async Task RemoveRefreshToken(string token)
		{
			string sql = "delete from RTokens where Token = @token";

			await dapperRepository.ExecuteAny(sql, new { token });
		}
	}
}
