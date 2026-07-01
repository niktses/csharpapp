using MediatR;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Product?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(IProductsService productsService, ILogger<GetProductByIdQueryHandler> logger)
    {
        _productsService = productsService;
        _logger = logger;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetProductByIdQuery for Id: {Id}", request.Id);

        return await _productsService.GetProductById(request.Id);
    }
}
