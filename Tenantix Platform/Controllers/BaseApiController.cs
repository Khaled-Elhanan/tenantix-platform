using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tenantix_WebApi.Controllers
{

    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender =>
            HttpContext.RequestServices.GetRequiredService<ISender>();


    }
}
