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
    public class TransactionHistoryManager : ITransactionHistoryService
    {
        private readonly ITransactionHistoryDal _transactionHistoryDal;

        public TransactionHistoryManager(ITransactionHistoryDal transactionHistoryDal)
        {
            _transactionHistoryDal = transactionHistoryDal;
        }

        public void TDelete(int id)
        {
           _transactionHistoryDal.Delete(id);
        }

        public List<TransactionHistory> TGetAll()
        {
            return _transactionHistoryDal.GetAll();
        }

        public TransactionHistory TGetById(int id)
        {
           return _transactionHistoryDal.GetById(id);
        }

        public void TInsert(TransactionHistory entity)
        {
            _transactionHistoryDal.Insert(entity);
        }

        public void TUpdate(TransactionHistory entity)
        {
           _transactionHistoryDal.Update(entity);
        }
    }
}
