using Asp.Versioning;
using Asp.Versioning.Builder;
using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CSharpApp.Api.Endpoints;

public static class ProductEndpoints
{
    public static IVersionedEndpointRouteBuilder MapProductEndpoints(this IVersionedEndpointRouteBuilder builder)
    {
        // V1 Endpoints
        builder.MapGet("api/v{version:apiVersion}/getproducts", async (IMediator mediator) =>
            {
                var products = await mediator.Send(new GetProductsQuery());
                return products;
            })
            .WithName("GetProductsV1")
            .HasApiVersion(1.0);

        // V2 Endpoints
        builder.MapGet("api/v{version:apiVersion}/getproducts", async (IMediator mediator) =>
            {
                var products = await mediator.Send(new GetProductsQuery());
                return products;
            })
            .WithName("GetProductsV2")
            .HasApiVersion(2.0);

        builder.MapGet("api/v{version:apiVersion}/getproduct/{id:int}", async (int id, IMediator mediator) =>
            {
                var product = await mediator.Send(new GetProductByIdQuery(id));
                return product is not null ? Results.Ok(product) : Results.NotFound();
            })
            .WithName("GetProductByIdV2")
            .HasApiVersion(2.0);

        builder.MapPost("api/v{version:apiVersion}/createproduct", async (CreateProductDto dto, IMediator mediator) =>
            {
                var product = await mediator.Send(new CreateProductCommand(dto));
                return product is not null ? Results.Created($"/api/v2/getproduct/{product.Id}", product) : Results.BadRequest();
            })
            .WithName("CreateProductV2")
            .HasApiVersion(2.0);

        return builder;
    }
}
