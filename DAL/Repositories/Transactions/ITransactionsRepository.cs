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

		Task<IEnumerable<Transaction>> GetTransactionsOfUser(int userId, Range range);

		Task<Transaction> GetTransaction(int id);

		Task<int> CreateTransaction(Transaction transaction);

		Task DeleteTransaction(int id);
	}
}
