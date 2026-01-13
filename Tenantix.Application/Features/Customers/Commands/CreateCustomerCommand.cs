using Tenantix.Application.Pipelines;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand:IRequest<IResponseWrapper>,IValidateMe
    {
        public CreateCustomerRequest Customer { get; set; } 
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IResponseWrapper>
    {
        private readonly ICustomerService _customerSerivce;
        public CreateCustomerCommandHandler(ICustomerService customerSerivce)
        {
            _customerSerivce = customerSerivce;
        }
        public async Task<IResponseWrapper> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerId = await _customerSerivce.CreateAsync(request.Customer, cancellationToken);
            return await ResponseWrapper<Guid>
                .SuccessAsync(customerId, "Customer created successfully");
        }
    }
}
