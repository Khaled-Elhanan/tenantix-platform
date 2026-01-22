using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Features.Categories.Commands;
using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Application.Features.Categories.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(StoreActions.Create , StoreFeatures.Categories)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
        {
           var response = await Sender.Send(new CreateCategoryCommand
           {
               CreateCategory = request
           });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet ]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Categories)]
        public async Task<IActionResult> GetCategoriesAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await Sender.Send(new GetCategoriesQuery
            {
                Page = page,
                PageSize = pageSize
            });

            return Ok(response);
        }
        [HttpGet("{id:guid}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Categories)]
        public async Task<IActionResult> GetCategoryByIdAsync(Guid id)
        {
            var response = await Sender.Send(new GetCategoryByIdQuery(id));

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        [HttpPut("{id:guid}")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Categories)]
        public async Task<IActionResult> UpdateCategoryAsync(
            Guid id,
            [FromBody] UpdateCategoryRequest request)
        {
            var response = await Sender.Send(new UpdateCategoryCommand
            {
                Id = id,
                Category = request
            });

            if (response.IsSuccessful) return Ok(response);
            return NotFound(response);
        }

        [HttpDelete("{id:guid}")]
        [ShouldHavePermission(StoreActions.Delete, StoreFeatures.Categories)]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            var response = await Sender.Send(new DeleteCategoryCommand
            {
                Id = id
            });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }
    }

}

