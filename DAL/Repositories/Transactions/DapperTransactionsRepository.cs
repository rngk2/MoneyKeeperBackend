using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using MoneyKeeper.Globals;

namespace DAL.Repositories
{
	internal class DapperTransactionsRepository : ITransactionsRepository
	{
		private readonly IDapperRepository repository;

		public DapperTransactionsRepository(IDapperRepository repository)
		{
			this.repository = repository;
		}

		public async Task<Transaction> GetTransaction(int id)
		{
			string sql = @"
				select * 
					from Transactions
				where
					Id = @id
			";

			return (await repository.QueryAny<Transaction>(sql, new { id })).FirstOrDefault();
		}

		public async Task<Transaction> GetTransaction(int id, int userId)
		{
			string sql = @"
				select * 
					from Transactions
				where
					Id = @id and UserId = @userId
			";

			return (await repository.QueryAny<Transaction>(sql, new { id, userId })).FirstOrDefault();
		}

		public async Task<IEnumerable<Transaction>> GetTransactions()
		{
			string sql = "select * from Transactions";

			return await repository.QueryAny<Transaction>(sql);
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsOfUser(
			int userId, 
			Range range, 
			string? like = null, 
			DateTimeOffset? when = null,
			string? orderBy = "Timestamp",
			string? order = "desc"
			)
		{
			string sql = @$"
				select 
					Transactions.*, Categories.Name CategoryName
				from
					Transactions
				join
					Categories on Categories.Id = Transactions.CategoryId
				where
					(Transactions.UserId = @userId)
					{ (like is not null ? " and (Comment like @like or Categories.Name like @like)" : "") }
					{ (when is not null ? " and (month(Timestamp) = month(@when))" : "") }
				order by
					@orderBy @order
				offset
					@start rows
				fetch
					next @next rows only
			";

			return await repository.QueryAny<Transaction>(sql, new 
			{ 
				start = range.Start.Value, 
				next = range.End.Value - range.Start.Value, 
				userId, 
				like, 
				when, 
				orderBy, 
				order 
			});
		}

		public async Task<int> CreateTransaction(Transaction transaction)
		{
			string sql = @"
				insert into Transactions
					(UserId, CategoryId, Amount, Timestamp, Comment)
				output 
					inserted.Id
				values 
					(@UserId, @CategoryId, @Amount, @Timestamp, @Comment)
			";

			return await repository.QuerySingleWithOutput<int>(sql, transaction);
		}

		public async Task<bool> DeleteTransaction(int id)
		{
			string sql = @"
				delete 
					from Transactions 
				where 
					Id = @id
			";

			return await repository.ExecuteAny(sql, new { id }) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}

