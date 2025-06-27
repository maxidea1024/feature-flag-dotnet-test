namespace FeatureFlagDemo;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class FeatureFlagAttribute : Attribute
{
    public string FeatureName { get; }

    public FeatureFlagAttribute(string featureName)
    {
        FeatureName = featureName ?? throw new ArgumentNullException(nameof(featureName));
    }
}
