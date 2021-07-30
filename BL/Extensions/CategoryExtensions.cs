using BL.Dtos.Category;
using DAL.Entities;

namespace BL.Extensions
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
