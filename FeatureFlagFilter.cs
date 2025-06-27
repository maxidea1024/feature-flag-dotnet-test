using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Reflection;
using FeatureFlagDemo.Services;

namespace FeatureFlagDemo;

internal class FeatureFlagFilter : IActionFilter
{
    private readonly IFeatureService _featureService;
    private readonly ILogger<FeatureFlagFilter> _logger;
    private readonly IAttributeCacheService _attributeCache;
    private readonly IUnleashContextBuilder _contextBuilder;

    public FeatureFlagFilter(IFeatureService featureService, ILogger<FeatureFlagFilter> logger, IAttributeCacheService attributeCache, IUnleashContextBuilder contextBuilder)
    {
        _featureService = featureService ?? throw new ArgumentNullException(nameof(featureService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _attributeCache = attributeCache ?? throw new ArgumentNullException(nameof(attributeCache));
        _contextBuilder = contextBuilder ?? throw new ArgumentNullException(nameof(contextBuilder));
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is not Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor actionDescriptor)
        {
            return;
        }

        var featureAttribute = _attributeCache.GetCachedAttribute<FeatureFlagAttribute>(actionDescriptor.MethodInfo);
        if (featureAttribute != null)
        {
            // Build UnleashContext from HTTP request
            var unleashContext = _contextBuilder.BuildContext(context.HttpContext);

            if (!_featureService.IsEnabled(featureAttribute.FeatureName, unleashContext))
            {
                // _logger.LogWarning("Feature '{FeatureName}' is disabled. Returning 403.", featureAttribute.FeatureName);

                var errorResponse = new
                {
                    error = "FeatureDisabled",
                    message = $"Feature '{featureAttribute.FeatureName}' is disabled."
                };

                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                };
                return;
            }

            // Console.WriteLine($"Feature '{featureAttribute.FeatureName}' is enabled. Proceeding with action.");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Console.WriteLine("Action executed.");
    }
}
