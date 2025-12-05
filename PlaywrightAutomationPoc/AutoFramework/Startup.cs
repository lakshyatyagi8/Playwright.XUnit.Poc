using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.GoogleMaps.Pages;

namespace PlaywrightAutomationPoc.AutoFramework
{
    public static class Startup
    {
        public static IServiceCollection AddBrowserServices(this IServiceCollection services)
        {
            services.AddSingleton<IBrowserProvider, BrowserProvider>();
            services.AddSingleton<IPageFactory, PageFactory>();

            return services;
        }
    }
}
