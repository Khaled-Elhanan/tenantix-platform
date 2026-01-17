using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Features.Orders.Commands;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Application.Features.Orders.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StoreTenantOnly]
    public class OrdersController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(StoreActions.Create, StoreFeatures.Orders)]
        public async Task<IActionResult> CreateOrderAsync(
            [FromBody] CreateOrderRequest request)
        {
            var response = await Sender.Send(new CreateOrderCommand
            {
                CreateOrder = request
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Orders)]
        public async Task<IActionResult> GetOrdersAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await Sender.Send(new GetOrdersPagedQuery
            {
                Page = page,
                PageSize = pageSize
            });

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Orders)]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            var response = await Sender.Send(new GetOrderByIdQuery { Id= id});
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
       
    }
}
