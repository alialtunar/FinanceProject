using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DtoLayer.Dtos.UserDto
{
    public class UserRegisterDto
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
