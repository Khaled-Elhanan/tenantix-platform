using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization;
using Tenantix.Application.Features.Tenancy;
using Tenantix.Application.Features.Tenancy.Commands;
using Tenantix.Application.Features.Tenancy.Queries;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseApiController
    {

        [HttpPost("add")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Store)]

        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest createTenantRequest)
        {
            var response = await Sender.Send(new CreateTenantCommand { CreateTenant = createTenantRequest });
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
    }

        [HttpPut("{tenantId}/activate")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Store)]
        public async Task<IActionResult> ActivateTenantAsync([FromRoute] string tenantId)
        {
            var response = await Sender.Send(new ActivateTenantCommand { TenantId = tenantId });
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{tenantId}/deactivate")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Store)]
        public async Task<IActionResult> DeactivateTenantAsync([FromRoute] string tenantId)
        {
            var response = await Sender.Send(new DeactivateTenantCommand { TenantId = tenantId });
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{tenantId}/upgrade")]
        [ShouldHavePermission(StoreActions.Upgrade, StoreFeatures.Store)]
        public async Task<IActionResult> UpgradeTenantSubscriptionAsync([FromRoute] string tenantId, [FromBody] UpdateTenantSubscriptionRequest updateTenant)
        {
            updateTenant.TenantId = tenantId;
            var response = await Sender.Send(new UpdateTenantSubscriptionCommand { UpdateTenantSubscription = updateTenant });
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{tenantId}")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Store)]
        public async Task<IActionResult> GetTenantById([FromRoute] string tenantId)
        {
            var response = await Sender.Send(new GetTenantByIdQuery { TenantId = tenantId });
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Store)]
        public async Task<IActionResult> GetTenantsAsync()
        {
            var response = await Sender.Send(new GetTenantsQuery());
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
