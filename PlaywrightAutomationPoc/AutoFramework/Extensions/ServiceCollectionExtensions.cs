// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.AutoFramework.Reporter;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;
using Microsoft.Playwright;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreFramework(this IServiceCollection services)
    {
        // Register core drivers and configuration
        services.AddSingleton<ITestSetting, TestSetting>();
        services.AddScoped<IBrowserFactory, BrowserFactory>();
        services.AddScoped<IPlaywrightDriver, PlaywrightDriver>();
        // ✅ Add this line to register the missing IPageFactory dependency
        services.AddScoped<IPageFactory, PageFactory>();
        // ✅ Add this line to dynamically resolve IPage for any Page Object
        services.AddScoped<IPage>(serviceProvider => 
        {
            var driver = serviceProvider.GetRequiredService<IPlaywrightDriver>();
            return driver.Page; 
        });
        // Register the report generator
        services.AddSingleton<IReportGenerator, ExtentReportGenerator>();
        return services;
    }

    public static IServiceCollection AddGoogleMapsPages(this IServiceCollection services)
    {
        // Register feature-specific page objects
        services.AddTransient<MapsPage>();
        return services;
    }
}