
using MediatR;
using Tenantix.Application.Features.Tenancy;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Tenancy.Queries;

public class GetTenantByIdQuery : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}
public class GetTenantByIdQueryHandler:IRequestHandler<GetTenantByIdQuery , IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public GetTenantByIdQueryHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public  async Task<IResponseWrapper> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(request.TenantId);
        if (tenant is not null)
        {
            return await ResponseWrapper<TenantResponse>.SuccessAsync(data: tenant);
            
        }
        return await ResponseWrapper<TenantResponse>.FailAsync("Tenant not found");
    }
}