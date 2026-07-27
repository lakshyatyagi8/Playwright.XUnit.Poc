using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class BrowserFactory : IBrowserFactory
    {
        public async Task<IBrowser> LaunchAsync(BrowserType browserType, BrowserTypeLaunchOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.Channel = GetChannelName(browserType);
            var playwright = await Playwright.CreateAsync();
            return await playwright[GetBrowserName(browserType)].LaunchAsync(options);
        }

        public static string GetBrowserName(BrowserType browserType) => browserType switch
        {
            BrowserType.Chromium => "chromium",
            BrowserType.Firefox => "firefox",
            BrowserType.Webkit => "webkit",
            BrowserType.Chrome => "chromium",
            BrowserType.MsEdge => "chromium",
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };

        public static string GetChannelName(BrowserType browserType) => browserType switch
        {
            BrowserType.Chromium => "chromium",
            BrowserType.Firefox => "firefox",
            BrowserType.Webkit => string.Empty,
            BrowserType.Chrome => "chrome",
            BrowserType.MsEdge => "msedge",
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };
    }
}