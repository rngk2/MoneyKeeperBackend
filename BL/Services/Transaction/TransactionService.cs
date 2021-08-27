using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using DAL.Repositories;
using DAL.Entities;

namespace BL.Services
{
	public class TransactionService : ITransactionService
	{
		private readonly ITransactionsRepository repository;

		public TransactionService(ITransactionsRepository repository)
		{
			this.repository = repository;
		}

		public async Task<Transaction> CreateTransaction(CreateTransactionDto transactionDto)
		{
			Transaction newTransaction = new()
			{
				UserId = transactionDto.UserId,
				CategoryId = transactionDto.CategoryId,
				Amount = transactionDto.Amount,
				Timestamp = transactionDto.Timestamp,
				Comment = transactionDto.Comment
			};

			int createdId = await repository.CreateTransaction(newTransaction);

			Transaction created = newTransaction with
			{
				Id = createdId
			};

			return created;
		}

		public async Task DeleteTransaction(int id)
		{
			await repository.DeleteTransaction(id);
		}

		public async Task<Transaction> GetTransaction(int id)
		{
			return await repository.GetTransaction(id);
		}

		public async Task<IEnumerable<Transaction>> GetTransactions()
		{
			return await repository.GetTransactions();
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsOfUser(int userId, Range range, string? like = null, DateTimeOffset? when = default)
		{
			return await repository.GetTransactionsOfUser(userId, range, like, when);
		}
	}
}
