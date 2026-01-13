using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommand : IRequest<ResponseWrapper> , IValidateMe
    {
        public Guid Id { get; init; }    
        public UpdateCustomerRequest Customer{ get ; init; } = default!;
    }
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ResponseWrapper>
    {
        private readonly ICustomerService _customerService;
        public UpdateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }
      
        public async Task<ResponseWrapper> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var updated = await _customerService.UpdateAsync(request.Id, request.Customer, cancellationToken);
            if (!updated)
            {
                return await ResponseWrapper
                    .FailAsync("Product not found .");
            }
            return await ResponseWrapper
                .SuccessAsync("Product updated successfully");
        }
    }

}                                              
