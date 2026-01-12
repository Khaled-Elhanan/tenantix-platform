using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Features.Customers.Commands;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Application.Features.Customers.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StoreTenantOnly]
    public class CustomerController : BaseApiController
    {
        [HttpPost]
        [ShouldHavePermission(StoreActions.Create, StoreFeatures.Customers)]
        public async Task<IActionResult> CreateCustomerAsync(
          [FromBody] CreateCustomerRequest request)
        {
            var response = await Sender.Send(new CreateCustomerCommand
            {
                Customer = request
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Customers)]
        public async Task<IActionResult> GetCustomersAsync(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var response = await Sender.Send(new GetCustomersQuery
            {
                Page = page,
                PageSize = pageSize
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }
        [HttpGet("{id:guid}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Customers)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var response = await Sender.Send(
                new GetCustomerByIdQuery(id));

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }


    }
}
