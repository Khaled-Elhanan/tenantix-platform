using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Shared.Models;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Queries
{
    public class GetOrdersPagedQuery:IRequest<IResponseWrapper>
    {
         public int Page { get; set; }
         public int PageSize { get; set; }
    }
    public class GetOrdersPagedQueryHandler : IRequestHandler<GetOrdersPagedQuery, IResponseWrapper>
    {
        private readonly IOrderService _orderService;

        public GetOrdersPagedQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(GetOrdersPagedQuery request, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetPagedAsync(
                request.Page, request.PageSize, cancellationToken);

            return await ResponseWrapper<PagedResponse<OrderListItemResponse>>
            .SuccessAsync(result);
        }
    }
}
