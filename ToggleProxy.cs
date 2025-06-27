using Unleash.Internal;

namespace FeatureFlagDemo;

public class ToggleProxy
{
    public string FeatureName { get; }
    public bool IsDefined { get; }
    public Variant Variant { get; }

    public bool IsEnabled(bool defaultValue = false)
    {
        return BoolVariation(defaultValue);
    }

    public bool BoolVariation(bool defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "bool")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        if (!bool.TryParse(Variant.Payload.Value, out var result))
        {
            return defaultValue;
        }

        return result;
    }

    public string StringVariation(string defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "string")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        return Variant.Payload.Value;
    }

    public int IntVariation(int defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "number")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        if (!int.TryParse(Variant.Payload.Value, out var result))
        {
            return defaultValue;
        }

        return result;
    }

    public float FloatVariation(float defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "number")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        if (!float.TryParse(Variant.Payload.Value, out var result))
        {
            return defaultValue;
        }

        return result;
    }

    public double DoubleVariation(double defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "number")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        if (!double.TryParse(Variant.Payload.Value, out var result))
        {
            return defaultValue;
        }

        return result;
    }

    public T JsonVariation<T>(T defaultValue)
    {
        if (!IsDefined)
        {
            return defaultValue;
        }

        if (Variant.Payload == null)
        {
            return defaultValue;
        }

        if (Variant.Payload.Type != "json")
        {
            return defaultValue;
        }

        if (string.IsNullOrEmpty(Variant.Payload.Value))
        {
            return defaultValue;
        }

        try
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(Variant.Payload.Value);
            return result ?? defaultValue;
        }
        catch (System.Text.Json.JsonException)
        {
            return defaultValue;
        }
    }

    public string Variation(string defaultVariantName)
    {
        return IsDefined ? Variant.Name : defaultVariantName;
    }

    internal ToggleProxy(string featureName, bool isDefined, Variant variant)
    {
        FeatureName = featureName;
        IsDefined = isDefined;
        Variant = variant;
    }
}
