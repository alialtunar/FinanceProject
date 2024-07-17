using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.EntityLayer.Concreate
{
    public class VerificationCode
    {
        public int ID { get; set; }
        public int AccountId { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; } = false;
    }

}
