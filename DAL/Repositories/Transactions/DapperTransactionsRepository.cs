using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Utils;

namespace DAL.Repositories
{
	public class DapperTransactionsRepository : ITransactionsRepository
	{
		private readonly IDapperRepository repository;

		private const string TRANSACTIONS_TABLE_NAME = "Transactions"; 

		public DapperTransactionsRepository(IDapperRepository repository)
		{
			this.repository = repository;
		}

		public async Task<int> CreateTransaction(Transaction transaction)
		{
			string sql = @$"
				insert into {TRANSACTIONS_TABLE_NAME}
					(UserId, CategoryId, Amount, Timestamp, Comment)
				output 
					inserted.Id
				values 
					(@UserId, @CategoryId, @Amount, @Timestamp, @Comment)
			";

			return await repository.QuerySingleWithOutput<int>(sql, transaction);
		}

		public async Task DeleteTransaction(int id)
		{
			string sql = @$"
				delete 
					from {TRANSACTIONS_TABLE_NAME} 
				where 
					Id=@id
			";

			await repository.ExecuteAny(sql, new { id});
		}

		public async Task<Transaction> GetTransaction(int id)
		{
			string sql = $@"
				select * 
					from {TRANSACTIONS_TABLE_NAME}
				where
					Id=@id
			";

			return (await repository.QueryAny<Transaction>(sql, new { id })).FirstOrDefault();
		}

		public async Task<IEnumerable<Transaction>> GetTransactions()
		{
			string sql = $"select * from {TRANSACTIONS_TABLE_NAME}";

			return await repository.QueryAny<Transaction>(sql);
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsOfUser(
			int userId, Range range, string like = null, DateTimeOffset when = default)
		{
			string sql = @$"
				select 
					Transactions.*, Categories.Name CategoryName
				from
					Transactions
				join
					Categories on Categories.Id = Transactions.CategoryId
				where
					Transactions.UserId = @userId
					{ (like is not null ? "and Comment like @like or Categories.Name like @like" : "") }
					{ (when != default ? $" and month(Timestamp) = month(@when)" : "") }
				order by
					Timestamp desc
				offset
					@start rows
				fetch
					next @next rows only
			";

			return await repository.QueryAny<Transaction>(sql, new { start = range.Start.Value, next = range.End.Value - range.Start.Value, userId, like, when });
		}

	}
}

