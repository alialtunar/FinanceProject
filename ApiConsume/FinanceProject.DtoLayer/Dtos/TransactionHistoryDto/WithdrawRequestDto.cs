﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DtoLayer.Dtos.TransactionHistoryDto
{
    public class WithdrawRequestDto
    {
        public int userId { get; set; }
        public decimal amount { get; set; }
    }
}
