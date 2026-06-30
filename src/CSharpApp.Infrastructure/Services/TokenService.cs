using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CSharpApp.Infrastructure.Services;

public sealed class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<TokenService> _logger;
    
    private TokenCache? _cache;

    public TokenService(IHttpClientFactory httpClientFactory, IOptions<RestApiSettings> restApiSettings, ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var cache = _cache;
        if (cache != null && cache.Expiry > DateTime.UtcNow.AddSeconds(30))
        {
            return cache.Token;
        }

        _logger.LogInformation("Requesting a new JWT token...");
        var (token, expiry) = await FetchNewTokenAsync(cancellationToken);
        _cache = new TokenCache(token, expiry);

        _logger.LogInformation("JWT token cached. Expiry (UTC): {Expiry}", expiry);
        return token;
    }

    public void InvalidateToken()
    {
        _cache = null;
        _logger.LogWarning("Cached JWT token has been invalidated.");
    }

    private async Task<(string Token, DateTime Expiry)> FetchNewTokenAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("AuthClient");
        var payload = new { email = _restApiSettings.Username, password = _restApiSettings.Password };
        var response = await client.PostAsJsonAsync(_restApiSettings.Auth!.TrimStart('/'), payload, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        if (result == null || string.IsNullOrEmpty(result.AccessToken))
        {
            throw new InvalidOperationException("Response did not contain a valid access token.");
        }

        var expiry = GetTokenExpiration(result.AccessToken) ?? DateTime.UtcNow.AddMinutes(10);
        return (result.AccessToken, expiry);
    }

    private static DateTime? GetTokenExpiration(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return null;
            var payload = parts[1];
            var bytes = Convert.FromBase64String(payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '='));
            using var doc = JsonDocument.Parse(bytes);
            return DateTimeOffset.FromUnixTimeSeconds(doc.RootElement.GetProperty("exp").GetInt64()).UtcDateTime;
        }
        catch
        {
            return null;
        }
    }

    private record TokenCache(string Token, DateTime Expiry);
    private record LoginResponse([property: JsonPropertyName("access_token")] string AccessToken);
}
