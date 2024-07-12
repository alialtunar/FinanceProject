using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FinanceProject.DataAccesLayer.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        private readonly IDbConnection _connection;

        public GenericRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Delete(int id)
        {
            string tableName = GetTableName();
            string query = $"DELETE FROM \"{tableName}\" WHERE Id = @Id";
            _connection.Execute(query, new { Id = id });
        }

        public List<T> GetAll()
        {
            string tableName = GetTableName();
            string query = $"SELECT * FROM \"{tableName}\"";
            return _connection.Query<T>(query).ToList();
        }

        public T GetById(int id)
        {
            string tableName = GetTableName();
            string query = $"SELECT * FROM \"{tableName}\" WHERE Id = @Id";
            return _connection.QuerySingleOrDefault<T>(query, new { Id = id });
        }

        public void Insert(T entity)
        {
            string tableName = GetTableName();
            var properties = typeof(T).GetProperties().Where(p => p.Name != "ID").Select(p => p.Name).ToList();
            var columnNames = string.Join(", ", properties);
            var parameterNames = string.Join(", ", properties.Select(p => "@" + p));

            string query = $"INSERT INTO \"{tableName}\" ({columnNames}) VALUES ({parameterNames})";
            _connection.Execute(query, entity);
        }


        public void Update(T entity)
        {
            string tableName = GetTableName();
            var properties = typeof(T).GetProperties().Select(p => p.Name).ToList();
            var setClause = string.Join(", ", properties.Select(p => $"{p} = @{p}"));

            string query = $"UPDATE \"{tableName}\" SET {setClause} WHERE Id = @Id";
            _connection.Execute(query, entity);
        }

        private string GetTableName()
        {
            // T is generic type, you should define how to get table name here
            // Example: if T is User, return "users"
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
