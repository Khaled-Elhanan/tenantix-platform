using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Customers.Commands
{
    public class DeleteCustomerCommand:IRequest<ResponseWrapper>,IValidateMe
    {
        public Guid Id { get; init; }
    }
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, ResponseWrapper>
    {
        private readonly ICustomerService _customerService;
        public DeleteCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public async Task<ResponseWrapper> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _customerService.DeleteAsync(request.Id, cancellationToken);
            if(!deleted)
            {
                return await ResponseWrapper.FailAsync("Customer not found");
            }
            return await ResponseWrapper.SuccessAsync("Customer deleted successfully");
        }
    }
}
