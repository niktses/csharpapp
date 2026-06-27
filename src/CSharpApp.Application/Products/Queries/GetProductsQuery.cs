using MediatR;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Application.Products.Queries;

public record GetProductsQuery(int? Limit = null) : IRequest<IReadOnlyCollection<Product>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<Product>>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(IProductsService productsService, ILogger<GetProductsQueryHandler> logger)
    {
        _productsService = productsService;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetProductsQuery with Limit: {Limit}", request.Limit);

        var products = await _productsService.GetProducts();

        if (request.Limit.HasValue)
        {
            return products.Take(request.Limit.Value).ToList().AsReadOnly();
        }

        return products;
    }
}
