using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Shared.Exceptions
{
    public class ConflictException  :Exception
    {
        public List<string> ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ConflictException(List<string>errorMessage=default ,  HttpStatusCode statusCode=HttpStatusCode.Conflict)
        {
            ErrorMessage = errorMessage;
            statusCode=statusCode;
        }
    }
}
