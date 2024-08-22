using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.ApplicationLayer.Exceptions
{
    public class ErrorException : Exception
    {
        public int StatusCode { get; }

        public ErrorException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
