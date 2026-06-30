using Asp.Versioning;
using Asp.Versioning.Builder;
using CSharpApp.Application.Categories.Commands;
using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CSharpApp.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IVersionedEndpointRouteBuilder MapCategoryEndpoints(this IVersionedEndpointRouteBuilder builder)
    {
        // V3 Endpoints
        builder.MapGet("api/v{version:apiVersion}/getcategories", async (IMediator mediator) =>
            {
                var categories = await mediator.Send(new GetCategoriesQuery());
                return categories;
            })
            .WithName("GetCategoriesV3")
            .HasApiVersion(3.0);

        builder.MapGet("api/v{version:apiVersion}/getcategory/{id:int}", async (int id, IMediator mediator) =>
            {
                var category = await mediator.Send(new GetCategoryByIdQuery(id));
                return category is not null ? Results.Ok(category) : Results.NotFound();
            })
            .WithName("GetCategoryByIdV3")
            .HasApiVersion(3.0);

        builder.MapPost("api/v{version:apiVersion}/createcategory", async (CreateCategoryDto dto, IMediator mediator) =>
            {
                var category = await mediator.Send(new CreateCategoryCommand(dto));
                return category is not null ? Results.Created($"/api/v3/getcategory/{category.Id}", category) : Results.BadRequest();
            })
            .WithName("CreateCategoryV3")
            .HasApiVersion(3.0);

        builder.MapPut("api/v{version:apiVersion}/updatecategory/{id:int}", async (int id, UpdateCategoryDto dto, IMediator mediator) =>
            {
                var category = await mediator.Send(new UpdateCategoryCommand(id, dto));
                return category is not null ? Results.Ok(category) : Results.NotFound();
            })
            .WithName("UpdateCategoryV3")
            .HasApiVersion(3.0);

        return builder;
     }
}
