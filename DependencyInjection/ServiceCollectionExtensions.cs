using System.Text.Json;
using FeatureFlagDemo.Services;
using Unleash;

namespace FeatureFlagDemo.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureFlagService(this IServiceCollection services, UnleashSettings settings)
    {
        var unleash = new DefaultUnleash(settings);

        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<object>>();

        unleash.ConfigureEvents(events =>
        {
            events.TogglesUpdatedEvent = (args) =>
            {
                logger.LogInformation("🔄 Toggles updated at {UpdatedOn}", args.UpdatedOn);
            };

            events.ErrorEvent = (args) =>
            {
                logger.LogError("❌ Error: {ErrorMessage}", args.Error.Message);
            };

            events.ImpressionEvent = (args) =>
            {
                logger.LogInformation("✅ Impression: {ImpressionData}", JsonSerializer.Serialize(args));
            };
        });

        services.AddSingleton<IUnleash>(c => unleash);

        services.AddSingleton<IFeatureService, FeatureService>();
        services.AddSingleton<IAttributeCacheService, AttributeCacheService>();
        services.AddSingleton<IUnleashContextBuilder, UnleashContextBuilder>();
        services.AddSingleton<FeatureFlagFilter>();
        services.AddSingleton<FeatureFlagVariantFilter>();

        // TEST: HostedService 등록
        services.AddHostedService<FeatureFlagTestHostedService>();

        services.AddMvcCore(options =>
        {
            options.Filters.Add<FeatureFlagFilter>();
            options.Filters.Add<FeatureFlagVariantFilter>();
        });
        return services;
    }
}
