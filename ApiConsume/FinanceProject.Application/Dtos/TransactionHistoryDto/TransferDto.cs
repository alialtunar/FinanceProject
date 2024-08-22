using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto
{
    public class TransferDto
    {
        public int userId { get; set; }
        public decimal amount { get; set; }
        public string recipientAccountNumber { get; set; }

        public string recipientName { get; set; }

        public string description { get; set; }
    }
}
