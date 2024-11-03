using responsiveness.CommonServices;
using responsiveness.Components;
using responsiveness.Models;

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
    services.Configure<UriMonitoringOptions>(configuration.GetSection(UriMonitoringOptions.SectionName));
    services.AddHttpClient();
    services.AddSingleton<IStatsCalculator, StatsCalculator>();
    services.AddSingleton<IUriBenchmarkService, UriBenchmarkService>();
    services.AddSingleton<UriMonitoringModel>();
    services.AddSingleton<IUriMonitoringLauncher, UriMonitoringLauncher>();
    services.AddRazorComponents().AddInteractiveServerComponents();
}
