using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization;
using Tenantix.Application.Features.Products.Commands;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Application.Features.Products.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [StoreTenantOnly]
    public class ProductsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(StoreActions.Create, StoreFeatures.Products)]
        public async Task<IActionResult> CreateProductAsync(
            [FromBody] CreateProductRequest createProductRequest)
        {
            var response = await Sender.Send(
                new CreateProductCommand { CreateProduct = createProductRequest });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpGet]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Products)]
        public async Task<IActionResult> GetProductsAsync(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var response = await Sender.Send(new GetProductsQuery
            {
                Page = page,
                PageSize = pageSize
            });

            return Ok(response);
        }
        [HttpGet("{id:guid}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Products)]
        public async Task<IActionResult> GetProductByIdAsync(Guid id)
        {
            var response = await Sender.Send(new GetProductByIdQuery(id));

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }
    }

}
