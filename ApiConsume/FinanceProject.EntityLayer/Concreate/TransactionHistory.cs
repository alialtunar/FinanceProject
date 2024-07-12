using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.EntityLayer.Concreate
{
    public class TransactionHistory
    {
        public int ID { get; set; }
      
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? RecipientAccountNumber { get; set; }
        public string? RecipientName { get; set; }
        public string? Description { get; set; }

        public int AccountID { get; set; }

    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
}
