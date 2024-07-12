using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Concreate;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpTransactionHistoryDal : GenericRepository<TransactionHistory>, ITransactionHistoryDal
    {
        public DpTransactionHistoryDal(IDbConnection connection) : base(connection)
        {
        }
    }
}
