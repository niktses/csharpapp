using MediatR;

namespace CSharpApp.Application.Categories.Queries;

public record GetCategoryByIdQuery(int Id) : IRequest<Category?>;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(ICategoriesService categoriesService, ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCategoryByIdQuery for Id: {Id}", request.Id);
        return await _categoriesService.GetCategoryById(request.Id);
    }
}
