using FluentValidation;
using MediatR;

namespace Tenantix.Application.Pipelines;

public interface IValidateMe { }

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidateMe
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // if no validators, continue to the next behavior/handler
        if (!_validators.Any())
            return await next();

        //  validators
        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

    
        var failures = results
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .ToList();


        if (failures.Count > 0)
            throw new ValidationException(failures);

      
        return await next();
    }
}
