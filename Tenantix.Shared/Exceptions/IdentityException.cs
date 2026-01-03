using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Shared.Exceptions
{
    public class IdentityException : Exception
    {
        public List<string> ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public IdentityException(List<string> errorMessage = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}
