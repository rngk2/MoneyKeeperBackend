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

		private const string TABLE_NAME = "Transactions"; 

		public DapperTransactionsRepository(IDapperRepository repository)
		{
			this.repository = repository;
		}

		public async Task<int> CreateTransaction(Transaction transaction)
		{
			var sql = @$"
					insert into {TABLE_NAME}
						(CategoryId, Amount)
					output inserted.Id
					values 
						(@CategoryId, @Amount)
			";
			return await repository.QuerySingleWithOutput<int>(sql, transaction);
		}

		public async Task DeleteTransaction(int id)
		{
			var sql = SqlQueryGenerator.GenerateDeleteQuerySecure(TABLE_NAME, "Id");
			await repository.ExecuteAny(sql, new { Id = id});
		}

		public async Task<Transaction> GetTransaction(int id)
		{
			var sql = SqlQueryGenerator.GenerateSelectQuerySecure(TABLE_NAME, new { Id = id });
			return (await repository.QueryAny<Transaction>(sql, new { Id = id })).FirstOrDefault();
		}

		public async Task<IEnumerable<Transaction>> GetTransactions()
		{
			var sql = $"select * from {TABLE_NAME}";
			return await repository.QueryAny<Transaction>(sql);
		}
	}
}
