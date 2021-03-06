using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using DAL.Settings;

namespace DAL.Repositories
{
	public class DapperRepository : IDapperRepository
	{
		private readonly string connectionString;

		public DapperRepository(IOptions<DapperSettings> options)
		{
			connectionString = options.Value.ConnectionString;
		}

		public async Task<IEnumerable<T>> QueryAny<T>(string sql, object @params = null)
		{
			await using var conn = GetDbConnection();
			return await conn.QueryAsync<T>(sql, @params);
		}

		public async Task ExecuteAny(string sql, object @params = null)
		{
			await using var conn = GetDbConnection();
			await conn.ExecuteAsync(sql, @params);
		}

		private SqlConnection GetDbConnection() => new SqlConnection(connectionString);

		public async Task<O> QuerySingleWithOutput<O>(string sql, object @params = null)
		{
			await using var conn = GetDbConnection();
			return await Task.FromResult(conn.QuerySingle<O>(sql, @params));
		}

	}
}
