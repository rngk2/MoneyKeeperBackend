using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using DAL.Entities;

namespace BL.Services
{
	public interface ITransactionService
	{
		
		Task<IEnumerable<Transaction>> GetTransactions();

		Task<IEnumerable<Transaction>> GetTransactionsOfUser(int userId, Range range, string? like = null, DateTimeOffset? when = null);

		Task<Transaction> GetTransaction(int id);

		Task<Transaction> CreateTransaction(CreateTransactionDto transactionDto);

		Task DeleteTransaction(int id);
	}
}
