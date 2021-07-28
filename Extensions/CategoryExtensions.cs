using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Dtos.Category;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Extensions
{
	public static class CategoryExtensions
	{
		public static CategoryDto AsDto(this Category category) => new CategoryDto
		{
			Id = category.Id,
			Name = category.Name,
			UserId = category.UserId
		};
	}
}
