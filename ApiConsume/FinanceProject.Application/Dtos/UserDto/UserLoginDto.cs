﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.ApplicationLayer.Dtos.UserDto
{
    public class UserLoginDto
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
