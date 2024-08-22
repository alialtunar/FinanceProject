using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.ApplicationLayer.Dtos.AccountDto
{
    public class AccountWithUserDto
    {
        public int ID { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }  // User tablosundan alınacak bilgi
    }
}
