using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreFramework(this IServiceCollection services)
    {
        // Register core drivers and configuration
        services.AddSingleton<ITestSetting, TestSetting>();
        services.AddScoped<IBrowserProvider, BrowserProvider>();
        services.AddScoped<PlaywrightDriver>();
        services.AddScoped<IPageFactory, PageFactory>();
        return services;
    }

    public static IServiceCollection AddGoogleMapsPages(this IServiceCollection services)
    {
        // Register feature-specific page objects
        services.AddTransient<IMapsPage, MapsPage>();
        return services;
    }
}