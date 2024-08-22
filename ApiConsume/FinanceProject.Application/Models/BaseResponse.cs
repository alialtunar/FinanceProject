using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.Application.Models
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool isSuccess { get; set; }

        public List<string> ErrorMessages { get; set; }

        public Object Result { get; set; }
    }
}
