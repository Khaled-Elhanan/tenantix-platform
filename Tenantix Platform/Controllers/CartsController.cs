using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Features.Carts.Commands;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Application.Features.Carts.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StoreTenantOnly]
    public class CartsController : BaseApiController
    {
        // GET: api/carts/{customerId}
        [HttpGet("{customerId:guid}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Carts)]
        public async Task<IActionResult> GetCartAsync(Guid customerId)
        {
            var response = await Sender.Send(new GetCartByCustomerIdQuery(customerId));

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        // GET: api/carts/{customerId}/summary
        [HttpGet("{customerId:guid}/summary")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Carts)]
        public async Task<IActionResult> GetCartSummaryAsync(Guid customerId)
        {
            var response = await Sender.Send(new GetCartSummaryQuery(customerId));
            return Ok(response);
        }

        // POST: api/carts/{customerId}/items/add
        [HttpPost("{customerId:guid}/items/add")]
        [ShouldHavePermission(StoreActions.Create, StoreFeatures.Carts)]
        public async Task<IActionResult> AddItemAsync(Guid customerId, [FromBody] AddCartItemRequest request)
        {
            var response = await Sender.Send(new AddCartItemCommand
            {
                CustomerId = customerId,
                Item = request
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        // PUT: api/carts/{customerId}/items/update
        [HttpPut("{customerId:guid}/items/update")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Carts)]
        public async Task<IActionResult> UpdateItemAsync(Guid customerId, [FromBody] UpdateCartItemRequest request)
        {
            var response = await Sender.Send(new UpdateCartItemCommand
            {
                CustomerId = customerId,
                Item = request
            });

            if (response.IsSuccessful)
                return Ok(response);

            // ممكن تبقى NotFound أو Conflict حسب middleware/response wrapper
            return BadRequest(response);
        }

        // DELETE: api/carts/{customerId}/items/{productId}
        [HttpDelete("{customerId:guid}/items/{productId:guid}")]
        [ShouldHavePermission(StoreActions.Delete, StoreFeatures.Carts)]
        public async Task<IActionResult> RemoveItemAsync(Guid customerId, Guid productId)
        {
            var response = await Sender.Send(new RemoveCartItemCommand
            {
                CustomerId = customerId,
                ProductId = productId
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        // DELETE: api/carts/{customerId}/clear
        [HttpDelete("{customerId:guid}/clear")]
        [ShouldHavePermission(StoreActions.Delete, StoreFeatures.Carts)]
        public async Task<IActionResult> ClearCartAsync(Guid customerId)
        {
            var response = await Sender.Send(new ClearCartCommand
            {
                CustomerId = customerId
            });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
