using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Tenancy.Queries;

public class GetTenantsQuery : IRequest<IResponseWrapper>
{
    
}
public class GetTenantsQueryHanlder : IRequestHandler<GetTenantsQuery , IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public GetTenantsQueryHanlder(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<IResponseWrapper> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetTenantsAsync();
        return await ResponseWrapper<List<TenantResponse>>.SuccessAsync(data: tenants);
    }
}