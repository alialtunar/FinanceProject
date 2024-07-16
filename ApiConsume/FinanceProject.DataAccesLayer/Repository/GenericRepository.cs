using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        private readonly IDbConnection _connection;

        public GenericRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task DeleteAsync(int id)
        {
            string tableName = GetTableName();
            string query = $"DELETE FROM \"{tableName}\" WHERE Id = @Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<List<T>> GetAllAsync()
        {
            string tableName = GetTableName();
            string query = $"SELECT * FROM \"{tableName}\"";
            var result = await _connection.QueryAsync<T>(query);
            return result.ToList();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            string tableName = GetTableName();
            string query = $"SELECT * FROM \"{tableName}\" WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task InsertAsync(T entity)
        {
            string tableName = GetTableName();
            var properties = typeof(T).GetProperties().Where(p => p.Name != "ID").Select(p => p.Name).ToList();
            var columnNames = string.Join(", ", properties);
            var parameterNames = string.Join(", ", properties.Select(p => "@" + p));

            string query = $"INSERT INTO \"{tableName}\" ({columnNames}) VALUES ({parameterNames})";
            await _connection.ExecuteAsync(query, entity);
        }

        public async Task UpdateAsync(T entity)
        {
            string tableName = GetTableName();
            var properties = typeof(T).GetProperties().Select(p => p.Name).ToList();
            var setClause = string.Join(", ", properties.Select(p => $"{p} = @{p}"));

            string query = $"UPDATE \"{tableName}\" SET {setClause} WHERE Id = @Id";
            await _connection.ExecuteAsync(query, entity);
        }

        private string GetTableName()
        {
            if (typeof(T) == typeof(User))
                return "users";
            else if (typeof(T) == typeof(TransactionHistory))
                return "transactionhistory";
            else if (typeof(T) == typeof(Account))
                return "accounts";

            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }
    }
}
