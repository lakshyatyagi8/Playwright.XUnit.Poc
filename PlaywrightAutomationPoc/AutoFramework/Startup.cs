using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;

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
