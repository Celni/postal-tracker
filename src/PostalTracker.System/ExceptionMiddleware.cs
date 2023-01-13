using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PostalTracker.System.Exceptions;

namespace PostalTracker.System;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext).ConfigureAwait(false);
        }
        catch (PostalException ex)
        {
            await WriteJsonToResponseAsync(httpContext, ex.HttpStatusCode, ex.Message).ConfigureAwait(false);
            _logger.LogWarning("Http code: {HttpStatusCode}", ex.HttpStatusCode);
        }
        catch (Exception exception)
        {
            await WriteJsonToResponseAsync(httpContext, 500, exception.Message).ConfigureAwait(false);
            _logger.LogError(exception, exception.Message);
        }
    }

    private static Task WriteJsonToResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var textMessage = JsonSerializer.Serialize(new { message });
        return context.Response.WriteAsync(textMessage);
    }
}