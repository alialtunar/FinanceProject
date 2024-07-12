using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace FinanceProject.DataAccesLayer.Concreate
{
    public class ConnectionHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ConnectionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("PostgresConnection");
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
