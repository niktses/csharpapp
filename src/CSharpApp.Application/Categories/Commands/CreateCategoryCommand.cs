using MediatR;

namespace CSharpApp.Application.Categories.Commands;

public record CreateCategoryCommand(CreateCategoryDto CategoryDto) : IRequest<Category?>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Category?>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(ICategoriesService categoriesService, ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    public async Task<Category?> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCategoryCommand for Name: {Name}", request.CategoryDto.Name);
        return await _categoriesService.CreateCategory(request.CategoryDto);
    }
}
