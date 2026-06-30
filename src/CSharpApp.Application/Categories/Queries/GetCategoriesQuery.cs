using MediatR;

namespace CSharpApp.Application.Categories.Queries;

public record GetCategoriesQuery() : IRequest<IReadOnlyCollection<Category>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyCollection<Category>>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<GetCategoriesQueryHandler> _logger;

    public GetCategoriesQueryHandler(ICategoriesService categoriesService, ILogger<GetCategoriesQueryHandler> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCategoriesQuery");
        return await _categoriesService.GetCategories();
    }
}
