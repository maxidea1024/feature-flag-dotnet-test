using System.Collections.Concurrent;
using System.Reflection;

namespace FeatureFlagDemo.Services;

/// <summary>
/// Service for caching method attributes to improve performance
/// </summary>
public interface IAttributeCacheService
{
    /// <summary>
    /// Gets a cached attribute from a method, or retrieves and caches it if not found
    /// </summary>
    /// <typeparam name="T">The attribute type to retrieve</typeparam>
    /// <param name="methodInfo">The method to get the attribute from</param>
    /// <returns>The attribute instance, or null if not found</returns>
    T? GetCachedAttribute<T>(MethodInfo methodInfo) where T : Attribute;
}

/// <summary>
/// Implementation of attribute caching service using ConcurrentDictionary for thread safety
/// </summary>
public class AttributeCacheService : IAttributeCacheService
{
    private readonly ConcurrentDictionary<(MethodInfo Method, Type AttributeType), Attribute?> _attributeCache = new();

    public T? GetCachedAttribute<T>(MethodInfo methodInfo) where T : Attribute
    {
        var key = (methodInfo, typeof(T));
        
        var cachedAttribute = _attributeCache.GetOrAdd(key, _ => methodInfo.GetCustomAttribute<T>());
        
        return cachedAttribute as T;
    }
}
