using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.EntityLayer.Concreate;
using System.Data;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpAccountDal : GenericRepository<Account>, IAccountDal
    {
        public DpAccountDal(IDbConnection connection) : base(connection)
        {
        }
    }
}
