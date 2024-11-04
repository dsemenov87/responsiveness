using responsiveness.Models;
using Microsoft.Extensions.Options;
using responsiveness.Abstractions;
using responsiveness.Benchmark;
using responsiveness.Web.CommonServices;
using responsiveness.Web.Components;
using responsiveness.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
return;

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<UriBenchmarkOptions>(configuration.GetSection(UriBenchmarkOptions.SectionName));
    services.AddHttpClient(nameof(UriBenchmarkService))
        .ConfigureHttpClient((sp, c) =>
        {
            var config = sp.GetRequiredService<IOptions<UriBenchmarkOptions>>().Value;
            c.Timeout = TimeSpan.FromSeconds(config.HttpClientTimeoutSec);
        })
        .AddStandardResilienceHandler();
    services.AddSingleton<IStatsCalculator, StatsCalculator>();
    services.AddSingleton<IUriBenchmarkService, UriBenchmarkService>();
    services.AddSingleton<UriMonitoringModel>();
    services.AddSingleton<IUriMonitoringLauncher<UriMonitoringProcess>, UriMonitoringLauncher>();
    services.AddRazorComponents().AddInteractiveServerComponents();
}

