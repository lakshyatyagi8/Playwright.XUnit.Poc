// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using BrowserKind = PlaywrightAutomationPoc.AutoFramework.Enum.BrowserType;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class BrowserFactory : IBrowserFactory
    {
        public async Task<IBrowser> LaunchAsync(BrowserKind browserType, BrowserTypeLaunchOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.Channel = GetChannelName(browserType);
            var playwright = await Playwright.CreateAsync();
            return await playwright[GetBrowserName(browserType)].LaunchAsync(options);
        }

        public static string GetBrowserName(BrowserKind browserType) => browserType switch
        {
            BrowserKind.Chromium => "chromium",
            BrowserKind.Firefox => "firefox",
            BrowserKind.Webkit => "webkit",
            BrowserKind.Chrome => "chromium",
            BrowserKind.MsEdge => "chromium",
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };

        public static string GetChannelName(BrowserKind browserType) => browserType switch
        {
            BrowserKind.Chromium => "chromium",
            BrowserKind.Firefox => "firefox",
            BrowserKind.Webkit => string.Empty,
            BrowserKind.Chrome => "chrome",
            BrowserKind.MsEdge => "msedge",
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };
    }
}