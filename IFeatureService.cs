using Unleash;
using Unleash.Internal;

namespace FeatureFlagDemo;

public interface IFeatureService
{
    bool IsEnabled(string featureName, UnleashContext context);
    Variant GetVariant(string featureName, UnleashContext context);

    bool BoolVariation(string featureName, UnleashContext context, bool defaultValue);
    string StringVariation(string featureName, UnleashContext context, string defaultValue);
    int IntVariation(string featureName, UnleashContext context, int defaultValue);
    float FloatVariation(string featureName, UnleashContext context, float defaultValue);
    double DoubleVariation(string featureName, UnleashContext context, double defaultValue);
    T JsonVariation<T>(string featureName, UnleashContext context, T defaultValue);

    string Variation(string featureName, UnleashContext context, string defaultVariantName);
}
