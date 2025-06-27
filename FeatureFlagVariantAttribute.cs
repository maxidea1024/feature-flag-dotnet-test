namespace FeatureFlagDemo;

/// <summary>
/// Attribute to mark a controller action as a feature flag variant.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class FeatureFlagVariantAttribute : Attribute
{
    /// <summary>
    /// Feature name to check.
    /// </summary>
    public string FeatureName { get; }

    /// <summary>
    /// Variant name to check.
    /// </summary>
    public string Variant { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureFlagVariantAttribute"/> class.
    /// </summary>
    /// <param name="featureName">Feature name to check.</param>
    /// <param name="variant">Variant name to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="featureName"/> or <paramref name="variant"/> is null.</exception>
    public FeatureFlagVariantAttribute(string featureName, string variant)
    {
        FeatureName = featureName ?? throw new ArgumentNullException(nameof(featureName));
        Variant = variant ?? throw new ArgumentNullException(nameof(variant));
    }
}
