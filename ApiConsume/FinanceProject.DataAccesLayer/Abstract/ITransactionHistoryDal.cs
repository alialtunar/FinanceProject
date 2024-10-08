﻿using FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface ITransactionHistoryDal:IGenericDal<TransactionHistory>
    {
        Task<List<TransactionHistory>> GetLastFiveTransactionsAsync(int accountId);

        Task<decimal> GetTotalAmountLast24HoursAsync(int accountId);

        Task<IEnumerable<LastTransfersDto>> GetLast5TransfersUsersAsync(int accountId);

        Task<IEnumerable<TransactionHistory>> GetPagedTransactionHistoryAsync(int accountId, int page, int pageSize);

        Task<decimal> GetTransactionVolumeLast24Hours();

        Task<List<TransactionHistory>> GetLastFiveTransactionsAsync();

        Task<IEnumerable<LastTransfersDto>> GetLast5TransfersUsersAsync();

        Task<IEnumerable<TransactionHistory>> GetAdminPagedTransactionHistoryAsync(int page, int pageSize);
    }
}
