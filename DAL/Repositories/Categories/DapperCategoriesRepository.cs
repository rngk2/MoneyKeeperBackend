using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories.Categories
{
	public class DapperCategoriesRepository : ICategoriesRepository
	{
		private readonly IDapperRepository dapperRepository;

		private const string CATEGORIES_TABLE_NAME = "Categories";

		public DapperCategoriesRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<IEnumerable<Category>> GetCategoriesOfUser(int userId)
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME} where UserId = @userId";

			return await dapperRepository.QueryAny<Category>(sql, new { userId });
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME}";

			return await dapperRepository.QueryAny<Category>(sql);
		}

		public async Task<Category> GetCategory(int id)
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME} where Id = @id";

			return (await dapperRepository.QueryAny<Category>(sql, new { id })).FirstOrDefault();
		}

		public async Task<Category> GetCategory(int userId, string categoryName)
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME} where UserId = @userId and Name = @categoryName";

			return (await dapperRepository.QueryAny<Category>(sql, new { userId, categoryName })).FirstOrDefault();
		}

		public async Task<int> AddCategoryToUser(Category category)
		{
			string sql = @$"
					insert into {CATEGORIES_TABLE_NAME}
						(Name, UserId)
					output 
						inserted.Id
					values
						(@Name, @UserId)
			";

			return await dapperRepository.QuerySingleWithOutput<int>(sql, category);
		}

		public async Task DeleteCategory(int id)
		{
			string sql = @$"
					delete from {CATEGORIES_TABLE_NAME}
					where
						Id = @id
							
			";

			await dapperRepository.ExecuteAny(sql, new { id });
		}

		public async Task UpdateCategoryToUser(Category category)
		{
			string sql = @$"
					update 
						{CATEGORIES_TABLE_NAME}
					set
						Name = @Name
					where
						Id = @Id
			";

			await dapperRepository.ExecuteAny(sql, category);
		}

		public async Task DeleteCategoryToUser(int userId, string categoryName)
		{
			var sql = $"delete from {CATEGORIES_TABLE_NAME} where UserId = @userId and Name = @categoryName";

			await dapperRepository.ExecuteAny(sql, new { userId, categoryName });
		}
	}
}
