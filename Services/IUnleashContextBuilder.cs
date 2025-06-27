using Unleash;
using Microsoft.AspNetCore.Http;

namespace FeatureFlagDemo.Services;

/// <summary>
/// Service for building UnleashContext from HTTP request information
/// </summary>
public interface IUnleashContextBuilder
{
    /// <summary>
    /// Builds an UnleashContext from the current HTTP request
    /// </summary>
    /// <param name="httpContext">The current HTTP context</param>
    /// <returns>An UnleashContext populated with request information</returns>
    UnleashContext BuildContext(HttpContext httpContext);
}

/// <summary>
/// Default implementation of IUnleashContextBuilder
/// </summary>
internal class UnleashContextBuilder : IUnleashContextBuilder
{
    public UnleashContext BuildContext(HttpContext httpContext)
    {
        var context = new UnleashContext();

        // Extract user information
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            context.UserId = httpContext.User.Identity.Name;
        }

        // Extract session ID if available
        if (httpContext.Session?.IsAvailable == true)
        {
            context.SessionId = httpContext.Session.Id;
        }

        // Extract remote IP address
        context.RemoteAddress = GetClientIpAddress(httpContext);

        // Extract additional properties from headers and request
        context.Properties = ExtractProperties(httpContext);

        // Log context as JSON for debugging
        var jsonContext = System.Text.Json.JsonSerializer.Serialize(context, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        // Console.WriteLine($"UnleashContext: {jsonContext}");

        return context;
    }

    private string? GetClientIpAddress(HttpContext httpContext)
    {
        // Check for forwarded IP first (common in load balancer scenarios)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For can contain multiple IPs, take the first one
            return forwardedFor.Split(',')[0].Trim();
        }

        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fall back to connection remote IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private Dictionary<string, string> ExtractProperties(HttpContext httpContext)
    {
        var properties = new Dictionary<string, string>();

        // Add user agent
        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        if (!string.IsNullOrEmpty(userAgent))
        {
            properties["userAgent"] = userAgent;
        }

        // Add custom headers that might be useful for feature flagging
        var customHeaders = new[]
        {
            "X-Client-Version",
            "X-Platform",
            "X-Device-Type",
            "X-User-Role",
            "X-Tenant-Id",
            "X-Environment"
        };

        foreach (var headerName in customHeaders)
        {
            var headerValue = httpContext.Request.Headers[headerName].FirstOrDefault();
            if (!string.IsNullOrEmpty(headerValue))
            {
                // Convert header name to property name (remove X- prefix and convert to camelCase)
                var propertyName = headerName.StartsWith("X-")
                    ? char.ToLowerInvariant(headerName[2]) + headerName[3..].Replace("-", "")
                    : headerName;

                properties[propertyName] = headerValue;
            }
        }

        // Add request path for context
        properties["requestPath"] = httpContext.Request.Path.Value ?? "";

        // Add HTTP method
        properties["httpMethod"] = httpContext.Request.Method;

        return properties;
    }
}
