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

		Task<Transaction> GetTransaction(int id);

		Task<Transaction> CreateTransaction(CreateTransactionDto transactionDto);

		Task DeleteTransaction(int id);
	}
}
