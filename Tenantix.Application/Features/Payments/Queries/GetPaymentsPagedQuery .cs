using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Payments.DTOs;
using Tenantix.Shared.Models;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Payments.Queries
{
    public class GetPaymentsPagedQuery : IRequest<IResponseWrapper>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetPaymentsPagedQueryHandler
        : IRequestHandler<GetPaymentsPagedQuery, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentsPagedQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            GetPaymentsPagedQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

            return await ResponseWrapper<PagedResponse<PaymentResponse>>
                .SuccessAsync(result);
        }
    }
}
