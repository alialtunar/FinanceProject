using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DtoLayer.Dtos.AccountDto
{
    public class AccountDetailsDto
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string FullName { get; set; }
    }
}
