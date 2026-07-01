using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CSharpApp.Api;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"CSharpApp API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A C# API demonstrating Clean Architecture with API Versioning and Swagger."
            });
        }

        options.SwaggerDoc("internal", new OpenApiInfo
        {
            Title = "CSharpApp Internal Diagnostics API",
            Version = "v1",
            Description = "Internal diagnostics and metrics endpoints."
        });
    }
}
