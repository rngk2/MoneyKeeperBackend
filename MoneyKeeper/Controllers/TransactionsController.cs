﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using BL.Extensions;
using BL.Services;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{

		private ITransactionService transactionService;

		public TransactionsController(ITransactionService transactionService)
		{
			this.transactionService = transactionService;
		}

		[HttpGet("{id}")]
		public async Task<TransactionDto> GetTransaction(int id)
		{
			return (await transactionService.GetTransaction(id)).AsDto();
		}

		[HttpGet]
		public async Task<IEnumerable<TransactionDto>> GetTransactions()
		{
			return (await transactionService.GetTransactions()).Select(t => t.AsDto());
		}

		[HttpGet("{userId}/{from}/{to}/{like?}/{when?}")]
		public async Task<IEnumerable<TransactionDto>> GetTransactionsOfUser(int userId, int from, int to, string like = null, DateTimeOffset when = default)
		{
			return (await transactionService.GetTransactionsOfUser(userId, new Range(from, to), like, when)).Select(t => t.AsDto());
		}


		[HttpPost]
		public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
		{
			try
			{
				var createdTransaction = await transactionService.CreateTransaction(transactionDto);

				return CreatedAtAction(
					nameof(GetTransaction),
					new { id = createdTransaction.Id },
					createdTransaction.AsDto());
			}
			catch (SqlException e)
			{
				if (e.Number == (int)SqlErrorCodes.FK_CONFLICT_ERROR)
				{
					return new ConflictObjectResult(e.Message);
				}

				return new StatusCodeResult(500);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(int id)
		{
			await transactionService.DeleteTransaction(id);
			return NoContent();
		}

	}
}
