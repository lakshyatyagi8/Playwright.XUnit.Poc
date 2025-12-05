using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class BrowserProvider : IBrowserProvider
    {
        public async Task<IBrowser> LaunchChromeBrowserAsync(BrowserTypeLaunchOptions options)
        {
            options.Channel = "chrome";
            return await GetBrowserAsync(BrowserType.Chromium, options);
        }

        public async Task<IBrowser> LaunchFirefoxBrowserAsync(BrowserTypeLaunchOptions options)
        {
            options.Channel = "firefox";
            return await GetBrowserAsync(BrowserType.Firefox, options);
        }

        public async Task<IBrowser> LaunchWebkitBrowserAsync(BrowserTypeLaunchOptions options)
        {
            options.Channel = "";
            return await GetBrowserAsync(BrowserType.Webkit, options);
        }

        public async Task<IBrowser> LaunchChromiumBrowserAsync(BrowserTypeLaunchOptions options)
        {
            options.Channel = "chromium";
            return await GetBrowserAsync(BrowserType.Chromium, options);
        }

        public async Task<IBrowser> LaunchEdgeBrowserAsync(BrowserTypeLaunchOptions options)
        {
            options.Channel = "msedge";
            return await GetBrowserAsync(BrowserType.Chromium, options);
        }


        private async Task<IBrowser> GetBrowserAsync(BrowserType browserType, BrowserTypeLaunchOptions options)
        {            
            var playwright = await Playwright.CreateAsync();
            return await playwright[browserType.ToString().ToLower()].LaunchAsync(options);
        }
    }
}