using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.EntityLayer.Concreate;
using System.Data;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpAccountDal : GenericRepository<Account>, IAccountDal
    {
        private readonly IDbConnection _dbConnection;

        public DpAccountDal(IDbConnection connection) : base(connection)
        {
            _dbConnection = connection;
        }

        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            try
            {
                var query = "SELECT * FROM Accounts WHERE AccountNumber = @AccountNumber";
                return await _dbConnection.QueryFirstOrDefaultAsync<Account>(query, new { AccountNumber = accountNumber });
            }
            catch (Exception ex)
            {
                throw new Exception($"Hesap numarasına göre hesap alınamadı. Hata: {ex.Message}");
            }
        }
    }
}
