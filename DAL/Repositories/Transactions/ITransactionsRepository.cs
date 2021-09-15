using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories
{
	public interface ITransactionsRepository
	{
		Task<IEnumerable<Transaction>> GetTransactions();

		Task<IEnumerable<Transaction>> GetTransactions(
			int userId,
			Range range,
			string orderByField,
			string order,
			string? searchPattern = null
		);

		Task<Transaction> GetTransaction(int id);

		Task<Transaction> GetTransaction(int id, int userId);

		Task<int> CreateTransaction(Transaction transaction);

		Task<bool> DeleteTransaction(int id);

	}
}
