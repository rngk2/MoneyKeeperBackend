using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using DAL.Entities;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface ITransactionService
	{
		
		//Task<Result<IEnumerable<Transaction>>> GetTransactions();

		Task<Result<IEnumerable<Transaction>>> GetTransactionsOfUser(int userId, Range range, string? like = null, DateTimeOffset? when = null);

		Task<Result<Transaction>> GetTransaction(int id, int userId);

		Task<Result<Transaction>> CreateTransaction(CreateTransactionDto transactionDto);

		Task<Result<Transaction>> DeleteTransaction(int id, int userId);
	}
}
