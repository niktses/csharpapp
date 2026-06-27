using MediatR;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Application.Products.Commands;

public record CreateProductCommand(CreateProductDto ProductDto) : IRequest<Product?>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product?>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductsService productsService, ILogger<CreateProductCommandHandler> logger)
    {
        _productsService = productsService;
        _logger = logger;
    }

    public async Task<Product?> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateProductCommand for Title: {Title}", request.ProductDto.Title);

        return await _productsService.CreateProduct(request.ProductDto);
    }
}
