using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Concreate;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.DtoLayer.Dtos.TransactionHistoryDto;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpTransactionHistoryDal : GenericRepository<TransactionHistory>, ITransactionHistoryDal
    {
        private readonly IDbConnection _connection;

        public DpTransactionHistoryDal(IDbConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task<decimal> GetTotalAmountLast24HoursAsync(int accountId)
        {
            var sql = @"
        SELECT SUM(Amount) 
        FROM TransactionHistory 
        WHERE AccountID = @AccountID
        AND TransactionDate >= @StartDate";

            var startDate = DateTime.UtcNow.AddHours(-24);
            return await _connection.ExecuteScalarAsync<decimal>(sql, new { AccountID = accountId, StartDate = startDate });
        }


        public async Task<List<TransactionHistory>> GetLastFiveTransactionsAsync(int accountId)
        {
            var sql = @"
                SELECT * 
                FROM TransactionHistory 
                WHERE AccountID = @AccountID
                ORDER BY TransactionDate DESC
                LIMIT 5"
            ;

            return (await _connection.QueryAsync<TransactionHistory>(sql, new { AccountID = accountId })).ToList();
        }


        public async Task<IEnumerable<LastTransfersDto>> GetLast5TransfersUsersAsync(int accountId)
        {
            var sql = @"
        SELECT u.FullName, a.AccountNumber
        FROM TransactionHistory th
        INNER JOIN Accounts a ON th.RecipientAccountNumber = a.AccountNumber
        INNER JOIN Users u ON a.UserID = u.ID
        WHERE th.AccountID = @AccountID
        AND th.TransactionType::integer = @TransactionType
        ORDER BY th.TransactionDate DESC
        LIMIT 5";

            var parameters = new
            {
                AccountID = accountId,
                TransactionType = (int)TransactionType.Transfer // Enum değerini integer'a dönüştürün
            };

            var result = await _connection.QueryAsync<LastTransfersDto>(sql, parameters);
            return result ?? Enumerable.Empty<LastTransfersDto>(); // Sonuç boşsa boş bir liste döndür
        }


        public async Task<IEnumerable<TransactionHistory>> GetPagedTransactionHistoryAsync(int accountId, int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var sql = @"
                SELECT ID, TransactionType, Amount, TransactionDate, RecipientAccountNumber, RecipientName, Description
                FROM TransactionHistory
                WHERE AccountID = @AccountID
                ORDER BY TransactionDate DESC
                LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                AccountID = accountId,
                PageSize = pageSize,
                Offset = offset
            };

            return await _connection.QueryAsync<TransactionHistory>(sql, parameters);
        }


        public async Task<decimal> GetTransactionVolumeLast24Hours()
        {
            string query = @"
                SELECT SUM(Amount) 
                FROM TransactionHistory 
                WHERE TransactionDate >= @StartDate";

            var parameters = new { StartDate = DateTime.Now.AddHours(-24) };

            return await _connection.QueryFirstOrDefaultAsync<decimal>(query, parameters);
        }


    }



}



