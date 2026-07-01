using System.Net;
using System.Text;
using CSharpApp.Core.Exceptions;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(HttpClient httpClient, 
        IOptions<RestApiSettings> restApiSettings, 
        ILogger<ProductsService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        await EnsureSuccessResponseAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res!.AsReadOnly();
    }

    public async Task<Product?> GetProductById(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccessResponseAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Product>(content);
    }

    public async Task<Product?> CreateProduct(CreateProductDto createProductDto)
    {
        var json = JsonSerializer.Serialize(createProductDto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_restApiSettings.Products, content);
        await EnsureSuccessResponseAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Product>(responseContent);
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