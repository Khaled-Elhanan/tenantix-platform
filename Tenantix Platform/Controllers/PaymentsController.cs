using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Features.Payments.Commands;
using Tenantix.Application.Features.Payments.Queries;
using Tenantix.Domain.Enums;
using Tenantix.Infrastructure.Identity.Auth;

namespace Tenantix_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseApiController
    {
        [HttpPost("create")]
        [ShouldHavePermission(StoreActions.Create, StoreFeatures.Payments)]
        public async Task<IActionResult> CreateAsync(
            [FromQuery] Guid orderId,
            [FromQuery] PaymentProvider provider,
            CancellationToken ct)
        {
            var response = await Sender.Send(new CreatePaymentForOrderCommand
            {
                OrderId = orderId,
                Provider = provider
            }, ct);

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("{paymentId:guid}/initiate")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Payments)]
        public async Task<IActionResult> InitiateAsync(Guid paymentId, CancellationToken ct)
        {
            var response = await Sender.Send(new InitiatePaymentCommand
            {
                PaymentId = paymentId
            }, ct);

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("{paymentId:guid}/paid")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Payments)]
        public async Task<IActionResult> MarkAsPaidAsync(Guid paymentId, CancellationToken ct)
        {
            var response = await Sender.Send(new MarkPaymentAsPaidCommand
            {
                PaymentId = paymentId
            }, ct);

            if (response.IsSuccessful)
                return NoContent();

            return BadRequest(response);
        }

       
        [HttpPut("{paymentId:guid}/failed")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Payments)]
        public async Task<IActionResult> MarkAsFailedAsync(Guid paymentId, CancellationToken ct)
        {
            var response = await Sender.Send(new MarkPaymentAsFailedCommand
            {
                PaymentId = paymentId
            }, ct);

            if (response.IsSuccessful)
                return NoContent();

            return BadRequest(response);
        }

        
        [HttpPut("{paymentId:guid}/refund")]
        [ShouldHavePermission(StoreActions.Update, StoreFeatures.Payments)]
        public async Task<IActionResult> RefundAsync(Guid paymentId, CancellationToken ct)
        {
            var response = await Sender.Send(new RefundPaymentCommand
            {
                PaymentId = paymentId
            }, ct);

            if (response.IsSuccessful)
                return NoContent();

            return BadRequest(response);
        }

        [HttpGet]
        [ShouldHavePermission(StoreActions.Read, StoreFeatures.Payments)]
        public async Task<IActionResult> GetPagedAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await Sender.Send(new GetPaymentsPagedQuery
            {
                Page = page,
                PageSize = pageSize
            });

            return Ok(response);
        }



    }
}
