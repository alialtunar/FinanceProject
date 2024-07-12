using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.EntityLayer.Concreate
{
    public class Account
    {
        public int ID { get; set; }

        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public int UserID { get; set; }
  
    }
}
