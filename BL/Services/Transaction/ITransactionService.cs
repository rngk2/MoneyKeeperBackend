using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface ITransactionService
	{
		Task<Result<IEnumerable<Transaction>>> GetTransactionsForCategory(int userId, int categoryId, Range range);

		Task<Result<IEnumerable<Transaction>>> GetTransactions(
			int userId, 
			Range range,
			string orderByField,
			string order,
			string? searchPattern = null
		);

		Task<Result<Transaction>> GetTransaction(int id, int userId);

		Task<Result<IEnumerable<Transaction>>> GetSummaryForUser(int id);

		Task<Result<Dictionary<string, decimal>>> GetTotalForUser(int id);

		Task<Result<Dictionary<string, decimal>>> GetTotalForUserForMonth(int id);

		Task<Result<Dictionary<string, decimal>>> GetTotalForUserForYear(int id);

		Task<Result<Transaction>> CreateTransaction(CreateTransaction transactionDto);

		Task<Result<Transaction>> DeleteTransaction(int id, int userId);
	}
}
