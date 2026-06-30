namespace CSharpApp.Core.Interfaces;

public interface ICategoriesService
{
    Task<IReadOnlyCollection<Category>> GetCategories();
    Task<Category?> GetCategoryById(int id);
    Task<Category?> CreateCategory(CreateCategoryDto createCategoryDto);
    Task<Category?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto);
}
