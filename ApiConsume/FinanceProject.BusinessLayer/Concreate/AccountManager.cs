using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class AccountManager : IAccountService
    {
        private readonly IAccountDal _accountDal;

        public AccountManager(IAccountDal accountDal)
        {
            _accountDal = accountDal;
        }
        public void TDelete(int id)
        {
            _accountDal.Delete(id);
        }

        public List<Account> TGetAll()
        {
           return _accountDal.GetAll();
        }

        public Account TGetById(int id)
        {
            return _accountDal.GetById(id);
        }

        public void TInsert(Account entity)
        {
            _accountDal.Insert(entity);
        }

        public void TUpdate(Account entity)
        {
           _accountDal.Update(entity);
        }
    }
}
