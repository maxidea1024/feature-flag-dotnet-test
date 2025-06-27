using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FeatureFlagDemo.Services;
using System.Net;

namespace FeatureFlagDemo;

internal class FeatureFlagVariantFilter : IActionFilter
{
    private readonly IFeatureService _featureService;
    private readonly ILogger<FeatureFlagVariantFilter> _logger;
    private readonly IAttributeCacheService _attributeCache;
    private readonly IUnleashContextBuilder _contextBuilder;

    public FeatureFlagVariantFilter(IFeatureService featureService, ILogger<FeatureFlagVariantFilter> logger, IAttributeCacheService attributeCache, IUnleashContextBuilder contextBuilder)
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

        var variantAttribute = _attributeCache.GetCachedAttribute<FeatureFlagVariantAttribute>(actionDescriptor.MethodInfo);
        if (variantAttribute != null)
        {
            var unleashContext = _contextBuilder.BuildContext(context.HttpContext);

            if (!_featureService.IsEnabled(variantAttribute.FeatureName, unleashContext))
            {
                _logger.LogWarning("Feature '{FeatureName}' is disabled. Returning 403.", variantAttribute.FeatureName);

                var errorResponse = new
                {
                    error = "FeatureDisabled",
                    message = $"Feature '{variantAttribute.FeatureName}' is disabled."
                };

                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                };
                return;
            }

            var currentVariant = _featureService.GetVariant(variantAttribute.FeatureName, unleashContext);
            if (currentVariant.Name != variantAttribute.Variant)
            {
                _logger.LogWarning("Feature '{FeatureName}' variant '{ExpectedVariant}' is not active. Current variant: '{CurrentVariant}'. Returning 403.",
                    variantAttribute.FeatureName, variantAttribute.Variant, currentVariant.Name);

                var errorResponse = new
                {
                    error = "FeatureVariantDisabled",
                    message = $"Feature '{variantAttribute.FeatureName}' is enabled but variant '{variantAttribute.Variant}' is not active. Current variant: '{currentVariant.Name}'."
                };

                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                };
                return;
            }

            _logger.LogInformation("Feature '{FeatureName}' with variant '{Variant}' is enabled. Proceeding with action.",
                variantAttribute.FeatureName, variantAttribute.Variant);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Action executed.");
    }
}
