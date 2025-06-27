using FeatureFlagDemo.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add session support for UnleashContext
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddFeatureFlagService(new Unleash.UnleashSettings
{
    AppName = "FeatureFlagDemo",
    InstanceTag = "demo",
    UnleashApi = new Uri("https://us.app.unleash-hosted.com/usii0012/api/"),
    CustomHttpHeaders = new Dictionary<string, string>
    {
        { "Authorization", "*:development.8d662424920812bad929a7f778d607a00779c75a2e8a25575541d5f3" }
    },
    FetchTogglesInterval = TimeSpan.FromSeconds(5),
    SendMetricsInterval = TimeSpan.FromSeconds(60),
});

var app = builder.Build();

app.UseSession();
app.UseRouting();
app.MapControllers();

app.Run();
