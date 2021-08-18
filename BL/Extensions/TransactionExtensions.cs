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
			UserId = transaction.UserId,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.CategoryName,
			Amount = transaction.Amount,
			Timestamp = transaction.Timestamp,
			Comment = transaction.Comment
		};
	}
}
