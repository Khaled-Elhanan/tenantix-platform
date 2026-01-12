using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Shared.Models;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Customers.Queries
{
    public class GetCustomersQuery  :IRequest<IResponseWrapper>
    {
            public int Page { get; init;  }
            public int PageSize { get; init;  }
    }

    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IResponseWrapper>
    {

        private readonly ICustomerService _customerService;

        public GetCustomersQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async  Task<IResponseWrapper> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
           var result = await _customerService.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            return await ResponseWrapper<PagedResponse<CustomerListItemResponse>>
                .SuccessAsync(data: result);

        }
    }
}
