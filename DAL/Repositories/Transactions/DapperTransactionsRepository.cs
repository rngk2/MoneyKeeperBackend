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
					(CategoryId, Amount, Timestamp, Comment)
				output 
					inserted.Id
				values 
					(@CategoryId, @Amount, @Timestamp, @Comment)
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

		public async Task<IEnumerable<Transaction>> GetTransactionsOfUser(int userId, Range range, string like = null)
		{
			string sql = @$"
				select * from 
					(select ROW_NUMBER() over (order by Timestamp desc) 
					as Row#, Transactions.*, Categories.Name CategoryName, Categories.UserId from Transactions
				join 
					Categories on Categories.Id=Transactions.CategoryId ) tbl
				where 
					Row# between {range.Start} and {range.End} 
					and UserId = @userId 
					{(like is not null ? $"and Comment like @like" : "")}
			";

			return await repository.QueryAny<Transaction>(sql, new { userId, like });
		}

	}
}