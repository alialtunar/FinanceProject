﻿using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IAccountDal : IGenericDal<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);

    }
}
