using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.ApplicationLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using System.Data;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpAccountDal : GenericRepository<Account>, IAccountDal
    {
        private readonly IDbConnection _dbConnection;

        public DpAccountDal(IDbConnection connection) : base(connection)
        {
            _dbConnection = connection;
        }

        public async Task<IEnumerable<AccountWithUserDto>> GetAdminPagedAccountsAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var sql = @"
        SELECT a.ID, a.AccountNumber, a.Balance, a.UserID, u.FullName
        FROM Accounts a
        INNER JOIN Users u ON a.UserID = u.ID
        ORDER BY a.ID ASC
        LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                PageSize = pageSize,
                Offset = offset
            };

            return await _dbConnection.QueryAsync<AccountWithUserDto>(sql, parameters);
        }


        public async Task<Account> GetAccountByUserId(int userId)
        {
           
                var query = "SELECT * FROM Accounts Where UserId = @UserId";
                return await _dbConnection.QueryFirstOrDefaultAsync<Account>(query, new { UserId = userId });
          
               
            
        }

        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
           
                var query = "SELECT * FROM Accounts WHERE AccountNumber = @AccountNumber";
                return await _dbConnection.QueryFirstOrDefaultAsync<Account>(query, new { AccountNumber = accountNumber });
           
        }
    }
}
