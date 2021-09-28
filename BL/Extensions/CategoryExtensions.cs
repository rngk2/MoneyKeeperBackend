using BL.Dtos.Category;
using Entities = DAL.Entities;

namespace BL.Extensions
{
	public static class CategoryExtensions
	{
		public static Category AsDto(this Entities.Category category) => new Category
		{
			Id = category.Id,
			Name = category.Name,
			UserId = category.UserId
		};

		public static Entities.Category AsEntity(this Category category) => new Entities.Category
		{
			Id = category.Id,
			Name = category.Name,
			UserId = category.UserId
		};
	}
}
