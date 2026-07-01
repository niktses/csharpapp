using System.Net;

namespace CSharpApp.Core.Exceptions;

public class DownstreamApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Content { get; }

    public DownstreamApiException(HttpStatusCode statusCode, string content, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Content = content;
    }
}
