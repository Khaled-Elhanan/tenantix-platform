using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Customers.Queries
{
    public record GetCustomerByIdQuery (Guid Id):IRequest<IResponseWrapper>;
     public class GetCustomerByIdQueryHandler
        : IRequestHandler<GetCustomerByIdQuery, IResponseWrapper>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerByIdQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IResponseWrapper> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken)
        {
            var customer = await _customerService.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (customer is null)
            {
                return await ResponseWrapper<CustomerResponse>
                    .FailAsync("Customer not found");
            }

            return await ResponseWrapper<CustomerResponse>
                .SuccessAsync(customer);
        }
    }
}
