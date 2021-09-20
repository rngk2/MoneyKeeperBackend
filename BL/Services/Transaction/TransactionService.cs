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
	internal class TransactionService : ITransactionService
	{
		private readonly ITransactionsRepository transactionsRepository;
		private readonly ICategoriesRepository categoriesRepository;

		public TransactionService(ITransactionsRepository repository, ICategoriesRepository categoriesRepository)
		{
			this.transactionsRepository = repository;
			this.categoriesRepository = categoriesRepository;
		}

		public async Task<Result<Transaction>> GetTransaction(int id, int userId)
		{
			Transaction transaction = await transactionsRepository.GetTransaction(id, userId);

			if (transaction is null)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND, $"User #{userId} has no transaction with id: {id}");
			}

			return transaction;
		}

		public async Task<Result<IEnumerable<Transaction>>> GetSummaryForUser(int id)
		{
			return new SuccessResult<IEnumerable<Transaction>>(await transactionsRepository.GetSummaryForUser(id));
		}

		public async Task<Result<Dictionary<string, decimal>>> GetTotalForUser(int id)
		{
			return ComputeTotal(await transactionsRepository.GetSummaryForUser(id));
		}

		public async Task<Result<Dictionary<string, decimal>>> GetTotalForUserForYear(int id)
		{
			return ComputeTotal(await transactionsRepository.GetSummaryForUserForYear(id));
		}

		public async Task<Result<Dictionary<string, decimal>>> GetTotalForUserForMonth(int id)
		{
			return ComputeTotal(await transactionsRepository.GetSummaryForUserForMonth(id));
		}

		private static Dictionary<string, decimal> ComputeTotal(IEnumerable<Transaction> summaryUnits)
		{
			Dictionary<string, decimal> computed = new();
			foreach (var unit in summaryUnits)
			{
				decimal unitAmount = unit.Amount;
				decimal contained = computed.GetValueOrDefault(unit.CategoryName);
				decimal newVal = contained is default(decimal) ? unitAmount : contained + unitAmount;

				computed[unit.CategoryName] = newVal;
			}

			return computed;
		}

		public async Task<Result<IEnumerable<Transaction>>> GetTransactionsForCategory(int userId, int categoryId, Range range)
		{
			bool userHasCategory = (await categoriesRepository.GetCategories(userId))
				.Select(c => c.Id)
				.Contains(categoryId);

			if (!userHasCategory)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND, $"User: #{userId} has no category with id: {categoryId}");
			}

			return new SuccessResult<IEnumerable<Transaction>>(await transactionsRepository.GetTransactionsForCategory(userId, categoryId, range));
		}

		public async Task<Result<IEnumerable<Transaction>>> GetTransactions(
			int userId,
			Range range,
			string orderByField,
			string order,
			string? searchPattern = null
		) {
			return new SuccessResult<IEnumerable<Transaction>>(
				await transactionsRepository.GetTransactions(userId, range, orderByField, order, searchPattern)
			);
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

			int createdId = await transactionsRepository.CreateTransaction(newTransaction);

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

			return await transactionsRepository.DeleteTransaction(id)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE, $"Error occured while deleting");
		}
	}
}
