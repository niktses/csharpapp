using System.Net;
using System.Net.Http.Headers;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Infrastructure.Services;

public class JwtAuthDelegatingHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public JwtAuthDelegatingHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenService.InvalidateToken();
        }

        return response;
    }
}
