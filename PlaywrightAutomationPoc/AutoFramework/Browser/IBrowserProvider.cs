using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IBrowserProvider
    {
        Task<IBrowser> LaunchChromeBrowserAsync(BrowserTypeLaunchOptions options);
        Task<IBrowser> LaunchChromiumBrowserAsync(BrowserTypeLaunchOptions options);
        Task<IBrowser> LaunchFirefoxBrowserAsync(BrowserTypeLaunchOptions options);
        Task<IBrowser> LaunchWebkitBrowserAsync(BrowserTypeLaunchOptions options);
        Task<IBrowser> LaunchEdgeBrowserAsync(BrowserTypeLaunchOptions options);
    }
}