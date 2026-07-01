using MediatR;

namespace CSharpApp.Application.Categories.Commands;

public record UpdateCategoryCommand(int Id, UpdateCategoryDto CategoryDto) : IRequest<Category?>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Category?>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(ICategoriesService categoriesService, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    public async Task<Category?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCategoryCommand for Id: {Id}", request.Id);
        return await _categoriesService.UpdateCategory(request.Id, request.CategoryDto);
    }
}
