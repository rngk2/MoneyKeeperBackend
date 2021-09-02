using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using DAL.Repositories;
using DAL.Entities;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;

namespace BL.Services
{
	public class TransactionService : ITransactionService
	{
		private readonly ITransactionsRepository repository;

		public TransactionService(ITransactionsRepository repository)
		{
			this.repository = repository;
		}

		public async Task<Result<Transaction>> CreateTransaction(CreateTransactionDto transactionDto)
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

		public async Task<Result<Transaction>> DeleteTransaction(int id, int userId)
		{
			var (toDelete, error) = await GetTransaction(id, userId).Unwrap();

			if (error)
			{
				return error.Wrap();
			}

			return await repository.DeleteTransaction(id)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE.ToString(), $"Error occured while deleting");
		}

		public async Task<Transaction> GetTransaction(int id)
		{
			return await repository.GetTransaction(id);
		}

		public async Task<Result<Transaction>> GetTransaction(int id, int userId)
		{
			Transaction transaction = await repository.GetTransaction(id, userId);

			if (transaction is null)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND.ToString(), $"User #{userId} has no transaction with id: {id}");
			}

			return transaction;
		}

		public async Task<Result<IEnumerable<Transaction>>> GetTransactions()
		{
			return new SuccessResult<IEnumerable<Transaction>>(await repository.GetTransactions());
		}

		public async Task<Result<IEnumerable<Transaction>>> GetTransactionsOfUser(int userId, Range range, string? like = null, DateTimeOffset? when = default)
		{
			return new SuccessResult<IEnumerable<Transaction>>(await repository.GetTransactionsOfUser(userId, range, like, when));
		}
	}
}
