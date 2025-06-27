namespace FeatureFlagDemo.Services;

/// <summary>
/// Background service for testing feature flags periodically
/// </summary>
public class FeatureFlagTestHostedService(
    ILogger<FeatureFlagTestHostedService> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<FeatureFlagTestHostedService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // 30초마다 실행

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FeatureFlagTestHostedService started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // 정상적인 종료
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in FeatureFlagTestHostedService");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // 에러 발생 시 5초 대기
            }
        }

        _logger.LogInformation("FeatureFlagTestHostedService stopped at: {time}", DateTimeOffset.Now);
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var featureService = scope.ServiceProvider.GetRequiredService<IFeatureService>();

        // _logger.LogInformation("=== Feature Flag Status Check ===");

        // // 테스트할 feature flag 목록
        // var featureNames = new[] { "FeatureA", "FeatureB", "FeatureC", "FeatureD" };

        // foreach (var featureName in featureNames)
        // {
        //     var isEnabled = featureService.IsFeatureEnabled(featureName);
        //     var variant = featureService.GetFeatureVariant(featureName);

        //     _logger.LogInformation(
        //         "Feature '{FeatureName}': Enabled={IsEnabled}, Variant={Variant}",
        //         featureName,
        //         isEnabled,
        //         variant ?? "None");
        // }

        // _logger.LogInformation("=== End of Feature Flag Status Check ===");

        // 비동기 작업 시뮬레이션
        await Task.CompletedTask;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("FeatureFlagTestHostedService is starting...");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("FeatureFlagTestHostedService is stopping...");
        await base.StopAsync(cancellationToken);
    }
}
