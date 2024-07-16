using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface ITransactionHistoryService:IGenericService<TransactionHistory>
    {
        Task Deposit(int accountId, decimal amount, string description = null);
        Task Withdraw(int accountId, decimal amount, string description = null);
    }
}
