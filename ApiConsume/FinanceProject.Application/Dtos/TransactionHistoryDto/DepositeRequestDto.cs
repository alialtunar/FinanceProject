using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto
{
    public class DepositeRquestDto
    {
        public int userId { get; set; }
        public decimal amount { get; set; }
    }
}
