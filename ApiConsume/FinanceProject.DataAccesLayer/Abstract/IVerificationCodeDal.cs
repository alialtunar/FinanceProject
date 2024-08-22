using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IVerificationCodeDal : IGenericDal<VerificationCode>
    {
        Task<VerificationCode> GetByCodeAsync(string code);
        Task UpdateAsync(VerificationCode verificationCode);

        Task<IEnumerable<VerificationCode>> GetAdminPagedVerificationCodesAsync(int page, int pageSize);
    }
}
