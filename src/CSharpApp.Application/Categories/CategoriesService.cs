using System.Net;
using System.Text;
using CSharpApp.Core.Exceptions;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(HttpClient httpClient, 
        IOptions<RestApiSettings> restApiSettings, 
        ILogger<CategoriesService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Categories);
        await EnsureSuccessResponseAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Category>>(content);
        
        return res!.AsReadOnly();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccessResponseAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Category>(content);
    }

    public async Task<Category?> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var json = JsonSerializer.Serialize(createCategoryDto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_restApiSettings.Categories, content);
        await EnsureSuccessResponseAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Category>(responseContent);
    }

    public async Task<Category?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        var json = JsonSerializer.Serialize(updateCategoryDto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_restApiSettings.Categories}/{id}", content);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccessResponseAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Category>(responseContent);
    }

    private async Task EnsureSuccessResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        _logger.LogError("Downstream API call failed with status code {StatusCode}. Response: {Response}", 
            response.StatusCode, content);

        throw new DownstreamApiException(response.StatusCode, content, 
            $"Downstream API call failed with status code {response.StatusCode}.");
    }
}
