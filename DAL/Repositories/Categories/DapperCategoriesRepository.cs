using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories.Categories
{
	public class DapperCategoriesRepository : ICategoriesRepository
	{
		private readonly IDapperRepository dapperRepository;

		private const string TADALE_NAME = "dbo.Categories";

		public DapperCategoriesRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<IEnumerable<Category>> GetCategoriesOfUser(int userId)
		{
			string getCategoriesQuery = $"select * from {TADALE_NAME} where UserId = @userId";
			return await dapperRepository.QueryAny<Category>(getCategoriesQuery, new { userId });
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			string getCategoriesQuery = $"select * from {TADALE_NAME}";
			return await dapperRepository.QueryAny<Category>(getCategoriesQuery);
		}

		public async Task<Category> GetCategory(int id)
		{
			string getCategoryQuery = $"select * from {TADALE_NAME} where Id = @id";
			return (await dapperRepository.QueryAny<Category>(getCategoryQuery, new { id })).FirstOrDefault();
		}

		public async Task<Category> GetCategory(int userId, string categoryName)
		{
			string getCategoryQuery = $"select * from {TADALE_NAME} where UserId = @userId and Name = @categoryName";
			return (await dapperRepository.QueryAny<Category>(getCategoryQuery, new { userId, categoryName })).FirstOrDefault();
		}

		public async Task<int> AddCategoryToUser(Category category)
		{
			string addCategoryQuery = @$"
				insert into {TADALE_NAME}
					(Name, UserId)
				output inserted.Id
				values
					(@Name, @UserId)
			";
			return await dapperRepository.QuerySingleWithOutput<int>(addCategoryQuery, category);
		}

		public async Task DeleteCategoryToUser(int id)
		{
			string deleteUserQuery = @$"
					delete from {TADALE_NAME}
						where
							Id = @id
							
			";
			await dapperRepository.ExecuteAny(deleteUserQuery, new { id });
		}

		public async Task UpdateCategoryToUser(Category category)
		{
			string updateCategoryQuery = @$"
				update {TADALE_NAME}
					set
						Name = @Name
					where
						Id = @Id
			";
			await dapperRepository.ExecuteAny(updateCategoryQuery, category);
		}
	}
}
