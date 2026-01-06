using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization;
using Tenantix.Application.Features.Products;
using Tenantix.Application.Features.Products.Commands;
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
    }

}
