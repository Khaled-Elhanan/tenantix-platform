using Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tenantix.Application.Features.StoreAuth.Commands;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Application.Features.StoreAuth.Queries;

namespace Tenantix_WebApi.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreAuthController : BaseApiController
    {
        
        [HttpPost("register")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to register a new customer account for the current store.")]
        public async Task<IActionResult> RegisterAsync([FromBody] StoreRegisterRequest request)
        {
            var response = await Sender.Send(new RegisterCustomerCommand
            {
                Register = request
            });

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

      
        [HttpPost("login")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to login a customer for the current store.")]
        public async Task<IActionResult> LoginAsync([FromBody] StoreLoginRequest request)
        {
            var response = await Sender.Send(new LoginCustomerQuery
            {
                Login = request
            });

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to request a password reset for a customer account.")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] StoreForgotPasswordRequest request)
        {
            var response = await Sender.Send(new ForgotPasswordCustomerCommand
            {
                Request = request
            });

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to reset a customer password using a reset token.")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] StoreResetPasswordRequest request)
        {
            var response = await Sender.Send(new ResetPasswordCustomerCommand
            {
                Request = request
            });

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

    }
}