using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Models;

namespace DAL.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<IEnumerable<User>> GetUsers();

		Task<int> CreateUser(User user);

		Task UpdateUser(User user);

		Task DeleteUser(int id);

		Task<IEnumerable<SummaryUnit>> GetSummaryForUser(int id);

		Task<IEnumerable<SummaryUnit>> GetSummaryForUser_ForMonth(int id);

		Task<IEnumerable<SummaryUnit>> GetSummaryForUser_ForYear(int id);

	}
}
