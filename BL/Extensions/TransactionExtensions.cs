using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using DAL.Entities;

namespace BL.Extensions
{
	public static class TransactionExtensions
	{
		public static TransactionDto AsDto(this Transaction transaction) => new TransactionDto
		{
			Id = transaction.Id,
			CategoryId = transaction.CategoryId,
			Amount = transaction.Amount,
			Timestamp = transaction.Timestamp
		};
	}
}
