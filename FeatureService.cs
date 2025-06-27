using Unleash;
using Unleash.Internal;

namespace FeatureFlagDemo;

internal class FeatureService : IFeatureService
{
    private readonly IUnleash _unleash;

    public FeatureService(IUnleash unleash)
    {
        _unleash = unleash;
    }

    public bool IsEnabled(string featureName, UnleashContext context)
    {
        return _unleash.IsEnabled(featureName, context);
    }

    public Variant GetVariant(string featureName, UnleashContext context)
    {
        return _unleash.GetVariant(featureName, context, Variant.DISABLED_VARIANT);
    }

    public ToggleProxy GetToggle(string featureName, UnleashContext context)
    {
        var knownToggles = _unleash.ListKnownToggles();
        var isDefined = knownToggles.Any(toggle => toggle.Name == featureName);
        return new ToggleProxy(
            featureName,
            isDefined,
            _unleash.GetVariant(featureName, context, Variant.DISABLED_VARIANT)
        );
    }

    public bool BoolVariation(string featureName, UnleashContext context, bool defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.BoolVariation(defaultValue);
    }

    public string StringVariation(string featureName, UnleashContext context, string defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.StringVariation(defaultValue);
    }

    public int IntVariation(string featureName, UnleashContext context, int defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.IntVariation(defaultValue);
    }

    public float FloatVariation(string featureName, UnleashContext context, float defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.FloatVariation(defaultValue);
    }

    public double DoubleVariation(string featureName, UnleashContext context, double defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.DoubleVariation(defaultValue);
    }

    public T JsonVariation<T>(string featureName, UnleashContext context, T defaultValue)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.JsonVariation(defaultValue);
    }

    public string Variation(string featureName, UnleashContext context, string defaultVariantName)
    {
        var toggle = GetToggle(featureName, context);
        return toggle.Variation(defaultVariantName);
    }
}
