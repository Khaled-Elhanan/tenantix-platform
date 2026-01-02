using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace Infrastructure.OpenApi;

public sealed class SwaggerGlobalAuthProcessor : IOperationProcessor
{
    private readonly string _scheme;

    public SwaggerGlobalAuthProcessor(string scheme = "JWT")
    {
        _scheme = scheme;
    }

    public bool Process(OperationProcessorContext context)
    {
        if (context is not AspNetCoreOperationProcessorContext aspNetContext)
            return true;

        var metadata = aspNetContext.ApiDescription
            .ActionDescriptor
            .TryGetPropertyValue<IList<object>>("EndpointMetadata");

        if (metadata is null)
            return true;

  
        if (metadata.OfType<AllowAnonymousAttribute>().Any())
            return true;

        context.OperationDescription.Operation.Security ??=
            new List<OpenApiSecurityRequirement>();

     
        if (context.OperationDescription.Operation.Security.Any())
            return true;

        context.OperationDescription.Operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                { _scheme, Array.Empty<string>() }
            });

        return true;
    }
}

public static class ObjectExtensions
{
    public static T TryGetPropertyValue<T>(
        this object obj,
        string propertyName,
        T defaultValue = default!)
    {
        if (obj is null)
            return defaultValue;

        var property = obj.GetType()
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (property is null)
            return defaultValue;

        return property.GetValue(obj) is T value
            ? value
            : defaultValue;
    }
}